using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Zentra.Domain;
using Zentra.Domain.Entities.Api;
using Zentra.Domain.Entities.Endpoint;
using Zentra.DomainServices;
using Zentra.DomainServices.Infra;
using Zentra.DomainServices.Wrappers;
using Zentra.Service.Implementation.Endpoint.Extensions;
using Zentra.Service.Implementation.Endpoint.Results;
using Zentra.Service.Interfaces.Interfaces.Endpoint;
using Zentra.Service.Interfaces.Interfaces.Endpoint.Results;
using Zentra.Service.Interfaces.Interfaces.Endpoint.Validators;

namespace Zentra.Service.Implementation.Endpoint;

internal class AuthorizeEndpoint : AuthorizationCodeBase, IEndpoint
{
    public AuthorizeEndpoint(
        ILoggerInstance instance,
        IResourceScopeValidator resourceScopeValidator,
        IAuthorizationService authorizationService,
        IRepository<Clients> clientRepository,
        IMapper mapper,
        ISessionManagementService session,
        SignInManagerWrapper<Users> csSignInManager,
        ZentraConfig tokenSettings,
        IFrameworkResultService frameworkResultService)
        : base(
            instance,
            resourceScopeValidator,
            authorizationService,
            clientRepository,
            mapper,
            session,
            tokenSettings,
            csSignInManager,
            frameworkResultService)
    {
    }

    public async Task<IEndpointResult> ProcessAsync(HttpContext context)
    {
        Dictionary<string, string> requestCollection;
        if (HttpMethods.IsGet(context.Request.Method))
        {
            requestCollection = context.Request.Query.ConvertCollection();
        }
        else if (HttpMethods.IsPost(context.Request.Method))
        {
            if (!context.Request.CheckHeaderContentType())
                return new StatusCodeResult(HttpStatusCode.UnsupportedMediaType);

            requestCollection = context.Request.Form.ConvertCollection();
        }
        else
        {
            return new StatusCodeResult(HttpStatusCode.MethodNotAllowed);
        }

        var result = await ProcessAuthorizeRequestAsync(context, requestCollection);
        LoggerService.WriteTo(Log.Debug, "End of authorize request endpoint");

        return result;
    }
}
