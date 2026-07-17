using AutoMapper;
using Microsoft.AspNetCore.Http;
using Zentra.Domain;
using Zentra.Domain.Constants;
using Zentra.Domain.Entities.Api;
using Zentra.Domain.Entities.Endpoint;
using Zentra.Domain.Models.Endpoint.Request;
using Zentra.DomainServices;
using Zentra.DomainServices.Infra;
using Zentra.DomainServices.Wrappers;
using Zentra.Service.Implementation.Endpoint.Extensions;
using Zentra.Service.Implementation.Endpoint.Results;
using Zentra.Service.Implementation.Endpoint.Specifications;
using Zentra.Service.Interfaces.Interfaces.Endpoint;
using Zentra.Service.Interfaces.Interfaces.Endpoint.Results;
using Zentra.Service.Interfaces.Interfaces.Endpoint.Validators;

namespace Zentra.Service.Implementation.Endpoint;

public abstract class AuthorizationCodeBase
{
    protected readonly IAuthorizationService AuthorizationService;
    private readonly IRepository<Clients> clientRepository;
    private readonly TokenSettings configSettings;

    protected readonly SignInManagerWrapper<Users> CsSignInManager;
    private readonly IFrameworkResultService frameworkResultService;

    protected readonly ILoggerService LoggerService;
    private readonly IMapper mapper;
    private readonly IResourceScopeValidator resourceScopeValidator;

    protected readonly ISessionManagementService SessionService;

    protected AuthorizationCodeBase(
        ILoggerInstance instance,
        IResourceScopeValidator resourceScopeValidator,
        IAuthorizationService authorizationService,
        IRepository<Clients> clientRepository,
        IMapper mapper,
        ISessionManagementService session,
        ZentraConfig tokenSettings,
        SignInManagerWrapper<Users> csSignInManager,
        IFrameworkResultService frameworkResultService)
    {
        if (instance is null) throw new ArgumentNullException(nameof(instance));

        if (tokenSettings is null) throw new ArgumentNullException(nameof(tokenSettings));

        LoggerService = instance.GetLoggerInstance(LoggerKeyConstants.DefaultLoggerKey);
        this.resourceScopeValidator =
            resourceScopeValidator ?? throw new ArgumentNullException(nameof(resourceScopeValidator));
        AuthorizationService = authorizationService ?? throw new ArgumentNullException(nameof(authorizationService));
        this.clientRepository = clientRepository ?? throw new ArgumentNullException(nameof(clientRepository));
        this.mapper = mapper;
        configSettings = tokenSettings.TokenSettings;
        CsSignInManager = csSignInManager ?? throw new ArgumentNullException(nameof(csSignInManager));
        this.frameworkResultService = frameworkResultService;
        SessionService = session;
    }

    public virtual async Task<IEndpointResult> ProcessAuthorizeRequestAsync(
        HttpContext context,
        Dictionary<string, string> requestCollection)
    {
        if (context is null) throw new ArgumentNullException(nameof(context));

        var request = new ValidatedAuthorizeRequestModel
        {
            RequestRawData = requestCollection,
            TokenConfigOptions = configSettings
        };

        // TODO: assignment to be confirmed during unit testing.
        request.Subject = await SessionService.GetUserPrincipalFromContextAsync();
        request.SessionId = await SessionService.GetSessionId();

        var authorizeRequest = new AuthorizeRequestSpecification(resourceScopeValidator, clientRepository, mapper,
            SessionService, configSettings);
        var authorizeValidation = await authorizeRequest.ValidateAsync(request);
        if (!authorizeRequest.IsValid)
        {
            authorizeValidation =
                frameworkResultService.Failed(authorizeValidation.ErrorCode, authorizeValidation.ErrorMessage);
            return request.Error(SessionService, authorizeValidation.ErrorCode, authorizeValidation.ErrorMessage);
        }

        // Checking user interaction required - Out of scope - Consent, custom redirect
        LoggerService.WriteTo(Log.Debug, "Check if user interaction is required ");
        var interactionResult = await AuthorizationService.CheckNavigationAsync(request);
        if (interactionResult.IsError)
            return request.Error(SessionService, interactionResult.ErrorCode, interactionResult.ErrorDescription);

        if (interactionResult.IsLogin)
        {
            var requestId = await AuthorizationService.SaveReturnUrlAsync(request);
            return new NavigationPageResult(request, configSettings, requestId);
        }

        LoggerService.WriteTo(Log.Debug, "Generating authorize code response for grant type :" + request.GrantType);
        var response = await AuthorizationService.ProcessAuthorizationCodeAsync(request);
        response.IsError = false;
        return new AuthorizeResult(response, SessionService);
    }
}
