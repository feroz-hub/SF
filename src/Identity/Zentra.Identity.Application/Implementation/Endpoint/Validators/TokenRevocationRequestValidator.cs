using Zentra.Domain;
using Zentra.Domain.Constants;
using Zentra.Domain.Entities.Endpoint;
using Zentra.Domain.Models.Endpoint;
using Zentra.Domain.Models.Endpoint.Request;
using Zentra.DomainServices;
using Zentra.DomainServices.Infra;
using Zentra.Service.Implementation.Endpoint.Specifications;
using Zentra.Service.Interfaces.Interfaces.Endpoint.Validators;

namespace Zentra.Service.Implementation.Endpoint.Validators;

internal class TokenRevocationRequestValidator : ITokenRevocationRequestValidator
{
    private readonly TokenSettings configSettings;
    private readonly IFrameworkResultService frameworkResultService;
    private readonly ILoggerService loggerService;
    private readonly IRepository<SecurityTokens> securityTokenRepository;

    public TokenRevocationRequestValidator(
        ILoggerInstance instance,
        IFrameworkResultService frameworkResultService,
        IRepository<SecurityTokens> securityTokenRepository,
        ZentraConfig tokenSettings)
    {
        configSettings = tokenSettings.TokenSettings;
        this.frameworkResultService = frameworkResultService;
        this.securityTokenRepository = securityTokenRepository;
        loggerService = instance.GetLoggerInstance(LoggerKeyConstants.DefaultLoggerKey);
    }

    public async Task<ValidatedRevocationRequestModel> ValidateRevocationRequestAsync(
        Dictionary<string, string> requestCollection,
        ClientsModel client)
    {
        loggerService.WriteTo(Log.Debug, "Entered into revocation request validation.");

        var validatedRequestModel = new ValidatedRevocationRequestModel
        {
            RequestRawData = requestCollection,
            TokenConfigOptions = configSettings,
            Client = client,
            ClientId = client?.ClientId
        };

        var tokenRevocationRequestValidation = new TokenRevocationRequestSpecification(securityTokenRepository);
        var validationError = await tokenRevocationRequestValidation.ValidateAsync(validatedRequestModel);
        if (!tokenRevocationRequestValidation.IsValid)
        {
            validationError = frameworkResultService.Failed(validationError.ErrorCode, validationError.ErrorMessage);
            validatedRequestModel.ErrorCode = validationError.ErrorCode;
            validatedRequestModel.ErrorDescription = validationError.ErrorMessage;
            return validatedRequestModel;
        }

        validatedRequestModel.IsError = false;
        return validatedRequestModel;
    }
}
