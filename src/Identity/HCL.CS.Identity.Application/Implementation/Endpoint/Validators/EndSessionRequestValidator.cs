using System.Security.Claims;
using AutoMapper;
using HCL.CS.Domain;
using HCL.CS.Domain.Constants;
using HCL.CS.Domain.Constants.Endpoint;
using HCL.CS.Domain.Entities.Endpoint;
using HCL.CS.Domain.ErrorCodes;
using HCL.CS.Domain.Models.Endpoint;
using HCL.CS.Domain.Models.Endpoint.Request;
using HCL.CS.DomainServices;
using HCL.CS.DomainServices.Infra;
using HCL.CS.DomainServices.UnitOfWork.Endpoint;
using HCL.CS.Service.Implementation.Endpoint.Extensions;
using HCL.CS.Service.Implementation.Endpoint.Specifications;
using HCL.CS.Service.Interfaces.Interfaces.Endpoint;
using HCL.CS.Service.Interfaces.Interfaces.Endpoint.Validators;

namespace HCL.CS.Service.Implementation.Endpoint.Validators;

internal class EndSessionRequestValidator : IEndSessionRequestValidator
{
    private readonly IRepository<Clients> clientRepository;
    private readonly TokenSettings configSettings;
    private readonly IFrameworkResultService frameworkResultService;
    private readonly Dictionary<string, AsymmetricKeyInfoModel> keyStore;
    private readonly ILoggerService loggerService;
    private readonly IMapper mapper;
    private readonly ISessionManagementService sessionManagementService;
    private readonly IClientsUnitOfWork unitOfWork;

    public EndSessionRequestValidator(
        ILoggerInstance instance,
        IRepository<Clients> clientRepository,
        IMapper mapper,
        ISessionManagementService sessionManagementService,
        HclCsConfig tokenSettings,
        IClientsUnitOfWork unitOfWork,
        IFrameworkResultService frameworkResultService,
        Dictionary<string, AsymmetricKeyInfoModel> keyStore)
    {
        this.clientRepository = clientRepository;
        this.mapper = mapper;
        this.sessionManagementService = sessionManagementService;
        configSettings = tokenSettings.TokenSettings;
        this.unitOfWork = unitOfWork;
        this.keyStore = keyStore;
        this.frameworkResultService = frameworkResultService;
        loggerService = instance.GetLoggerInstance(LoggerKeyConstants.DefaultLoggerKey);
    }

    public async Task<ValidatedEndSessionCallbackRequestModel> ValidateCallbackRequestAsync(
        Dictionary<string, string> requestCollection)
    {
        var result = new ValidatedEndSessionCallbackRequestModel
        {
            IsError = true
        };

        var sessionId =
            requestCollection.GetValueFromDictionary(OpenIdConstants.RoutePathParameters.EndSessionCallback);
        var logoutSession = await sessionId.UnProtectDataAsync<LogoutMessageModel>();
        if (logoutSession.ClientIdCollection.ContainsAny())
        {
            result.IsError = false;
            result.FrontChannelLogoutUrls = await GetFrontChannelLogoutUrlsAsync(logoutSession);
            result.TokenConfigOptions = configSettings;
        }
        else
        {
            result.ErrorCode = "Failed to read end Session callback message";
        }

        return result;
    }

    public async Task<ValidatedEndSessionRequestModel> ValidateRequestAsync(
        Dictionary<string, string> requestCollection, ClaimsPrincipal user)
    {
        loggerService.WriteTo(Log.Debug, "Entered into end session request validation.");
        var validatedRequestModel = new ValidatedEndSessionRequestModel
        {
            RequestRawData = requestCollection,
            TokenConfigOptions = configSettings,
            Subject = user
        };

        var idTokenHint = requestCollection.GetValueFromDictionary(OpenIdConstants.EndSessionRequest.IdTokenHint);
        if (!string.IsNullOrWhiteSpace(idTokenHint))
        {
            var endSessionRequestValidation = new EndSessionRequestSpecification(unitOfWork, clientRepository, mapper,
                sessionManagementService, keyStore);
            var errorModel = await endSessionRequestValidation.ValidateAsync(validatedRequestModel);
            if (!endSessionRequestValidation.IsValid)
            {
                errorModel = frameworkResultService.Failed(errorModel.ErrorCode, errorModel.ErrorMessage);
                validatedRequestModel.IsError = true;
                validatedRequestModel.ErrorCode = errorModel.ErrorCode;
                validatedRequestModel.ErrorDescription = errorModel.ErrorMessage;
                return validatedRequestModel;
            }
        }
        else
        {
            validatedRequestModel.SessionId = await sessionManagementService.GetSessionId();
            validatedRequestModel.ClientIds = await sessionManagementService.GetClientListAsync();

            var clientId = requestCollection.GetValueFromDictionary(OpenIdConstants.AuthorizeRequest.ClientId);
            if (!string.IsNullOrWhiteSpace(clientId))
            {
                var clientsEntity = await clientRepository.GetAsync(
                    client => client.ClientId == clientId,
                    new System.Linq.Expressions.Expression<Func<HCL.CS.Domain.Entities.Endpoint.Clients, object>>[] { x => x.RedirectUris, x => x.PostLogoutRedirectUris });

                if (clientsEntity.ContainsAny())
                {
                    validatedRequestModel.Client = mapper.Map<Clients, ClientsModel>(clientsEntity[0]);
                    validatedRequestModel.ClientId = validatedRequestModel.Client.ClientId;

                    var postLogoutRedirectUri =
                        requestCollection.GetValueFromDictionary(OpenIdConstants.EndSessionRequest.PostLogoutRedirectUri);
                    if (!string.IsNullOrWhiteSpace(postLogoutRedirectUri))
                    {
                        var allowedPostLogoutRedirectUris =
                            validatedRequestModel.Client.PostLogoutRedirectUris?.ConvertAll(x => x.PostLogoutRedirectUri)
                            ?? new List<string>();

                        if (allowedPostLogoutRedirectUris.Contains(postLogoutRedirectUri))
                        {
                            validatedRequestModel.PostLogOutUri = postLogoutRedirectUri;
                            var state = requestCollection.GetValueFromDictionary(OpenIdConstants.EndSessionRequest.State);
                            if (!string.IsNullOrWhiteSpace(state))
                            {
                                validatedRequestModel.State = state;
                            }
                        }
                        else
                        {
                            validatedRequestModel.IsError = true;
                            validatedRequestModel.ErrorCode = OpenIdConstants.Errors.InvalidRequest;
                            validatedRequestModel.ErrorDescription = EndpointErrorCodes.InvalidPostLogoutRedirectUri;
                            return validatedRequestModel;
                        }
                    }
                }
            }
        }

        return validatedRequestModel;
    }

    public async Task<IEnumerable<string>> GetFrontChannelLogoutUrlsAsync(LogoutMessageModel logoutMessage)
    {
        var frontChannelUrls = new List<string>();
        foreach (var clientId in logoutMessage.ClientIdCollection)
            if (!string.IsNullOrWhiteSpace(clientId))
            {
                var clientCollection = await clientRepository.GetAsync(
                    client => client.ClientId == clientId,
                    x => new { x.FrontChannelLogoutSessionRequired, x.FrontChannelLogoutUri });
                if (clientCollection.ContainsAny())
                {
                    var client = clientCollection.FirstOrDefault();
                    var frontChannelUrl = client.FrontChannelLogoutUri;
                    if (!string.IsNullOrWhiteSpace(frontChannelUrl))
                    {
                        if (client.FrontChannelLogoutSessionRequired)
                        {
                            frontChannelUrl = frontChannelUrl.AddQueryString(OpenIdConstants.EndSessionRequest.Sid,
                                logoutMessage.SessionId);
                            frontChannelUrl = frontChannelUrl.AddQueryString(OpenIdConstants.EndSessionRequest.Issuer,
                                configSettings.TokenConfig.IssuerUri);
                        }

                        frontChannelUrls.Add(frontChannelUrl);
                    }
                }
            }

        if (frontChannelUrls.ContainsAny())
        {
            var msg = frontChannelUrls.Aggregate((x, y) => x + ", " + y);
            loggerService.WriteTo(Log.Debug, "Client front-channel logout URLs: {0}", msg);
        }
        else
        {
            loggerService.WriteTo(Log.Debug, "No client front-channel logout URLs");
        }

        return frontChannelUrls;
    }
}
