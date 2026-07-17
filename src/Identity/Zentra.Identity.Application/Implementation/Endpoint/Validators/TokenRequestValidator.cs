using Zentra.Domain;
using Zentra.Domain.Constants;
using Zentra.Domain.Constants.Endpoint;
using Zentra.Domain.Entities.Api;
using Zentra.Domain.ErrorCodes;
using Zentra.Domain.Models.Endpoint.Request;
using Zentra.Domain.Models.Endpoint.Validation;
using Zentra.DomainServices.Infra;
using Zentra.DomainServices.Wrappers;
using Zentra.Service.Implementation.Endpoint.Extensions;
using Zentra.Service.Implementation.Endpoint.Specifications;
using Zentra.Service.Interfaces.Interfaces.Api;
using Zentra.Service.Interfaces.Interfaces.Endpoint;
using Zentra.Service.Interfaces.Interfaces.Endpoint.Validators;

namespace Zentra.Service.Implementation.Endpoint.Validators;

internal class TokenRequestValidator : ITokenRequestValidator
{
    private readonly IAuthenticationService authenticationService;
    private readonly IAuthorizationService authorizationService;
    private readonly TokenSettings configSettings;
    private readonly IFrameworkResultService frameworkResultService;
    private readonly ILoggerService loggerService;
    private readonly IResourceScopeValidator resourceScopeValidator;
    private readonly ITokenGenerationService tokenGenerationService;
    private readonly UserManagerWrapper<Users> userManager;

    public TokenRequestValidator(
        ILoggerInstance instance,
        IFrameworkResultService frameworkResultService,
        UserManagerWrapper<Users> userManager,
        IAuthenticationService authenticationService,
        IAuthorizationService authorizationService,
        ITokenGenerationService tokenGenerationService,
        IResourceScopeValidator resourceScopeValidator,
        ZentraConfig tokenSettings)
    {
        this.frameworkResultService = frameworkResultService;
        this.userManager = userManager;
        this.authenticationService = authenticationService;
        this.authorizationService = authorizationService;
        this.tokenGenerationService = tokenGenerationService;
        this.resourceScopeValidator = resourceScopeValidator;
        configSettings = tokenSettings.TokenSettings;
        loggerService = instance.GetLoggerInstance(LoggerKeyConstants.DefaultLoggerKey);
    }

    public async Task<ValidatedTokenRequestModel> ValidateTokenRequestAsync(
        Dictionary<string, string> requestCollection, ClientSecretValidationModel clientValidationModel)
    {
        var validatedTokenRequestModel = new ValidatedTokenRequestModel
        {
            RequestRawData = requestCollection,
            TokenConfigOptions = configSettings
        };

        if (clientValidationModel == null) frameworkResultService.Throw(EndpointErrorCodes.InvalidClientObject);

        validatedTokenRequestModel.SetClient(clientValidationModel?.Client, clientValidationModel?.Secret);

        var tokenRequestValidation = new TokenRequestSpecification();
        var validationError = await tokenRequestValidation.ValidateAsync(validatedTokenRequestModel);
        if (tokenRequestValidation.IsValid)
        {
            loggerService.WriteTo(Log.Debug, "Entered into validate token request.");
            validatedTokenRequestModel.Issuer = configSettings.TokenConfig.IssuerUri;
            switch (validatedTokenRequestModel.GetValue(OpenIdConstants.TokenRequest.GrantType))
            {
                case OpenIdConstants.GrantTypes.AuthorizationCode:
                    loggerService.WriteTo(Log.Debug, "Entered into authorization code flow.");
                    validatedTokenRequestModel.GrantType = OpenIdConstants.GrantTypes.AuthorizationCode;
                    var authorizationCodeValidation =
                        new AuthorizationCodeFlowSpecification(authorizationService, userManager);
                    validationError = await authorizationCodeValidation.ValidateAsync(validatedTokenRequestModel);
                    if (authorizationCodeValidation.IsValid
                        && (validatedTokenRequestModel.Client.RequirePkce
                            || !string.IsNullOrWhiteSpace(validatedTokenRequestModel.AuthorizationCode.CodeChallenge)))
                    {
                        loggerService.WriteTo(Log.Debug,
                            "Client required a proof key for code exchange. Starting PKCE validation");
                        var proofKeyParametersValidation = new ProofKeyParametersSpecification();
                        validationError = await proofKeyParametersValidation.ValidateAsync(validatedTokenRequestModel);
                    }

                    break;
                case OpenIdConstants.GrantTypes.ClientCredentials:
                    loggerService.WriteTo(Log.Debug, "Entered into client credentials flow.");
                    validatedTokenRequestModel.GrantType = OpenIdConstants.GrantTypes.ClientCredentials;
                    var clientCredentialsValidation =
                        new ClientCredentialsFlowSpecification(resourceScopeValidator);
                    validationError = await clientCredentialsValidation.ValidateAsync(validatedTokenRequestModel);

                    break;
                case OpenIdConstants.GrantTypes.Password:
                    loggerService.WriteTo(Log.Debug, "Entered into resource owner password flow.");
                    validatedTokenRequestModel.GrantType = OpenIdConstants.GrantTypes.Password;
                    var resourceOwnerPasswordValidation =
                        new ResourceOwnerFlowSpecification(resourceScopeValidator, authenticationService, userManager);
                    validationError =
                        await resourceOwnerPasswordValidation.ValidateAsync(validatedTokenRequestModel);

                    break;
                case OpenIdConstants.GrantTypes.RefreshToken:
                    loggerService.WriteTo(Log.Debug, "Entered into refresh token flow.");
                    validatedTokenRequestModel.GrantType = OpenIdConstants.GrantTypes.RefreshToken;
                    var refreshTokenFlowValidation = new RefreshTokenFlowSpecification(tokenGenerationService);
                    validationError = await refreshTokenFlowValidation.ValidateAsync(validatedTokenRequestModel);

                    break;
                case OpenIdConstants.GrantTypes.UserCode:
                    loggerService.WriteTo(Log.Debug, "Entered into user_code (external sign-in) flow.");
                    validatedTokenRequestModel.GrantType = OpenIdConstants.GrantTypes.UserCode;
                    var userCodeFlowValidation =
                        new UserCodeFlowSpecification(resourceScopeValidator, authorizationService, userManager);
                    validationError = await userCodeFlowValidation.ValidateAsync(validatedTokenRequestModel);

                    break;
            }
        }

        if (validationError != null && !string.IsNullOrWhiteSpace(validationError.ErrorCode))
        {
            validationError = frameworkResultService.Failed(validationError.ErrorCode, validationError.ErrorMessage);
            validatedTokenRequestModel.ErrorCode = validationError.ErrorCode;
            validatedTokenRequestModel.ErrorDescription = validationError.ErrorMessage;
            loggerService.WriteTo(Log.Error, validationError.ErrorMessage);
        }
        else
        {
            validatedTokenRequestModel.IsError = false;
        }

        return validatedTokenRequestModel;
    }
}
