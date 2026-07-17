using DomainValidation.Interfaces.Specification;
using DomainValidation.Validation;
using Zentra.Domain.Constants.Endpoint;
using Zentra.Domain.Entities.Api;
using Zentra.Domain.ErrorCodes;
using Zentra.Domain.Models.Endpoint;
using Zentra.Domain.Models.Endpoint.Request;
using Zentra.Domain.Models.Endpoint.Validation;
using Zentra.DomainServices.Wrappers;
using Zentra.Service.Implementation.Endpoint.Extensions;
using Zentra.Service.Implementation.Endpoint.Validators;
using Zentra.Service.Interfaces.Interfaces.Api;
using Zentra.Service.Interfaces.Interfaces.Endpoint.Validators;

namespace Zentra.Service.Implementation.Endpoint.Specifications;

internal sealed class ResourceOwnerFlowSpecification : BaseRequestModelValidator<ValidatedTokenRequestModel>
{
    internal ResourceOwnerFlowSpecification(IResourceScopeValidator resourceScopeValidator,
        IAuthenticationService authenticationService,
        UserManagerWrapper<Users> userManager)
    {
        Add("CheckClientAuthorizedForGrantType", new Rule<ValidatedTokenRequestModel>(
            new CheckClientAuthorizedForGrantType<ValidatedTokenRequestModel>(new List<string>
            {
                AuthenticationConstants.GrantType.ResourceOwnerPassword
            }),
            OpenIdConstants.Errors.UnauthorizedClient,
            EndpointErrorCodes.ClientNotAuthorizedForGrantType));

        Add("ValidateRequestedROPScopes", new Rule<ValidatedTokenRequestModel>(
            new ValidateRequestedRopScopes(resourceScopeValidator),
            OpenIdConstants.Errors.InvalidScope,
            EndpointErrorCodes.InvalidScopeOrNotAllowed));

        Add("CheckUserName", new Rule<ValidatedTokenRequestModel>(
            new IsRequestNull<ValidatedTokenRequestModel>(request =>
                request.GetValue(OpenIdConstants.TokenRequest.UserName)),
            OpenIdConstants.Errors.InvalidGrant,
            EndpointErrorCodes.UserNameMissing));

        Add("CheckUserNameLength", new Rule<ValidatedTokenRequestModel>(
            new CheckLengthRestrictions<ValidatedTokenRequestModel>(
                request => request.GetValue(OpenIdConstants.TokenRequest.UserName),
                request => request.TokenConfigOptions.InputLengthRestrictionsConfig.UserName,
                request => ">"),
            OpenIdConstants.Errors.InvalidGrant,
            EndpointErrorCodes.UserNameTooLong));

        Add("CheckUserPasswordLength", new Rule<ValidatedTokenRequestModel>(
            new CheckLengthRestrictions<ValidatedTokenRequestModel>(
                request => request.GetValue(OpenIdConstants.TokenRequest.Password),
                request => request.TokenConfigOptions.InputLengthRestrictionsConfig.Password,
                request => ">"),
            OpenIdConstants.Errors.InvalidGrant,
            EndpointErrorCodes.PasswordTooLong));

        Add("CheckUserExists", new Rule<ValidatedTokenRequestModel>(
            new CheckUserExistsByName(userManager),
            OpenIdConstants.Errors.InvalidRequest,
            EndpointErrorCodes.InvalidUser));

        Add("CheckUserTwoFactor", new Rule<ValidatedTokenRequestModel>(
            new CheckUserTwoFactor(userManager),
            OpenIdConstants.Errors.InvalidRequest,
            EndpointErrorCodes.TwoFactorEnabled));

        Add("CheckProfileServices", new Rule<ValidatedTokenRequestModel>(
            new CheckProfileServices(authenticationService),
            OpenIdConstants.Errors.InvalidGrant,
            EndpointErrorCodes.UserAuthenticationFailed));
    }
}

internal class CheckProfileServices : ISpecification<ValidatedTokenRequestModel>
{
    private readonly IAuthenticationService authenticationService;

    internal CheckProfileServices(IAuthenticationService authenticationService)
    {
        this.authenticationService = authenticationService;
    }

    public bool IsSatisfiedBy(ValidatedTokenRequestModel model)
    {
        var userName = model.GetValue(OpenIdConstants.TokenRequest.UserName);
        var password = model.GetValue(OpenIdConstants.TokenRequest.Password);
        // authenticate user
        var validationModel = new RopValidationModel
        {
            UserName = userName,
            Password = password,
            Request = model
        };

        validationModel = authenticationService.RopValidateCredentialsAsync(validationModel).GetAwaiter().GetResult();
        if (validationModel.IsError)
        {
            validationModel.ErrorCode ??= OpenIdConstants.Errors.InvalidGrant;
            if (validationModel.ErrorCode == OpenIdConstants.Errors.UnsupportedGrantType) return false;

            return false;
        }

        if (validationModel.Subject == null) return false;

        model.UserName = userName;
        model.Subject = validationModel.Subject;

        return true;
    }
}

internal class CheckUserTwoFactor : ISpecification<ValidatedTokenRequestModel>
{
    private readonly UserManagerWrapper<Users> userManager;

    internal CheckUserTwoFactor(UserManagerWrapper<Users> userManager)
    {
        this.userManager = userManager;
    }

    public bool IsSatisfiedBy(ValidatedTokenRequestModel model)
    {
        // make sure user is enabled
        var userName = model.GetValue(OpenIdConstants.TokenRequest.UserName);
        var userInfo = userManager.FindByNameAsync(userName).GetAwaiter().GetResult();
        if (userInfo != null && !userInfo.TwoFactorEnabled) return true;

        return false;
    }
}

internal class CheckUserExistsByName : ISpecification<ValidatedTokenRequestModel>
{
    private readonly UserManagerWrapper<Users> userManager;

    internal CheckUserExistsByName(UserManagerWrapper<Users> userManager)
    {
        this.userManager = userManager;
    }

    public bool IsSatisfiedBy(ValidatedTokenRequestModel model)
    {
        // make sure user is enabled
        var userName = model.GetValue(OpenIdConstants.TokenRequest.UserName);
        var userInfo = userManager.FindByNameAsync(userName).GetAwaiter().GetResult();
        if (userInfo == null) return false;

        model.UserName = userName;
        return true;
    }
}

internal class ValidateRequestedRopScopes : ISpecification<ValidatedTokenRequestModel>
{
    private readonly IResourceScopeValidator resourceScopeValidator;

    internal ValidateRequestedRopScopes(IResourceScopeValidator resourceScopeValidator)
    {
        this.resourceScopeValidator = resourceScopeValidator;
    }

    public bool IsSatisfiedBy(ValidatedTokenRequestModel model)
    {
        var scopes = model.GetValue(OpenIdConstants.TokenRequest.Scope);

        if (string.IsNullOrWhiteSpace(scopes))
        {
            if (model.Client.AllowedScopes.ContainsAny())
            {
                var allowedScopes = model.Client.AllowedScopes.ToList();
                scopes = string.Join(" ", allowedScopes.ToArray());
            }
            else
            {
                return false;
            }
        }

        if (scopes.Length > model.TokenConfigOptions.InputLengthRestrictionsConfig.Scope) return false;

        var requestedScopes = scopes.ParseScopesString().ToList();
        var checkClientScope = resourceScopeValidator
            .ValidateRequestedScopeWithClientAsync(model.Client.AllowedScopes, requestedScopes).GetAwaiter()
            .GetResult();
        AllowedScopesParserModel allowedScopesParser;
        if (checkClientScope)
        {
            model.RequestRawData.TryGetValue(OpenIdConstants.TokenRequest.UserName, out var username);
            var resourceScopeModel = new ResourceScopeModel
            {
                RawData = model.RequestRawData,
                RequestedScope = requestedScopes,
                Client = model.Client,
                UserName = username
            };

            allowedScopesParser = resourceScopeValidator.ValidateRequestedScopesAsync(resourceScopeModel).GetAwaiter()
                .GetResult();
            model.TokenDetails = allowedScopesParser.TokenDetails;
        }
        else
        {
            return false;
        }

        model.AllowedScopesParserModel = allowedScopesParser;

        return true;
    }
}
