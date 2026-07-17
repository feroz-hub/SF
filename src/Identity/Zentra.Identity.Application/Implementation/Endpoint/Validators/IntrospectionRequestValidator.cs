using AutoMapper;
using Zentra.Domain;
using Zentra.Domain.Constants;
using Zentra.Domain.Entities.Endpoint;
using Zentra.Domain.Models.Endpoint;
using Zentra.Domain.Models.Endpoint.Request;
using Zentra.DomainServices;
using Zentra.DomainServices.Infra;
using Zentra.DomainServices.UnitOfWork.Endpoint;
using Zentra.Service.Implementation.Endpoint.Specifications;
using Zentra.Service.Interfaces.Interfaces.Endpoint.Validators;

namespace Zentra.Service.Implementation.Endpoint.Validators;

internal class IntrospectionRequestValidator : IIntrospectionRequestValidator
{
    private readonly IRepository<Clients> clientRepository;
    private readonly TokenSettings configSettings;
    private readonly IFrameworkResultService frameworkResultService;
    private readonly Dictionary<string, AsymmetricKeyInfoModel> keyStore;
    private readonly ILoggerService loggerService;
    private readonly IMapper mapper;
    private readonly IRepository<SecurityTokens> securityTokenRepository;
    private readonly IClientsUnitOfWork unitOfWork;

    public IntrospectionRequestValidator(
        ILoggerInstance instance,
        IClientsUnitOfWork unitOfWork,
        IRepository<Clients> clientRepository,
        IMapper mapper,
        Dictionary<string, AsymmetricKeyInfoModel> keyStore,
        IFrameworkResultService frameworkResultService,
        ZentraConfig tokenSettings,
        IRepository<SecurityTokens> securityTokenRepository)
    {
        this.unitOfWork = unitOfWork;
        this.clientRepository = clientRepository;
        this.mapper = mapper;
        this.keyStore = keyStore;
        this.frameworkResultService = frameworkResultService;
        loggerService = instance.GetLoggerInstance(LoggerKeyConstants.DefaultLoggerKey);
        configSettings = tokenSettings.TokenSettings;
        this.securityTokenRepository = securityTokenRepository;
    }

    public async Task<ValidatedIntrospectionRequestModel> ValidateIntrospectionRequestAsync(
        Dictionary<string, string> requestCollection, ClientsModel client)
    {
        loggerService.WriteTo(Log.Debug, "Introspection request validation started.");
        var tokenRequest = new ValidatedIntrospectionRequestModel
        {
            RequestRawData = requestCollection,
            TokenConfigOptions = configSettings,
            Client = client,
            ClientId = client.ClientId
        };

        var requestValidator =
            new IntrospectionRequestSpecification(unitOfWork, clientRepository, mapper, keyStore,
                securityTokenRepository);
        var introspectionRequest = await requestValidator.ValidateAsync(tokenRequest);
        if (!requestValidator.IsValid)
        {
            introspectionRequest =
                frameworkResultService.Failed(introspectionRequest.ErrorCode, introspectionRequest.ErrorMessage);
            tokenRequest.ErrorCode = introspectionRequest.ErrorCode;
            tokenRequest.ErrorDescription = introspectionRequest.ErrorMessage;
        }

        // valid token
        return tokenRequest;
    }
}
