using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;
using Zentra.Domain;
using Zentra.Domain.Constants;
using Zentra.Domain.Constants.Endpoint;
using Zentra.Domain.Models.Endpoint;
using Zentra.DomainServices.Infra;
using Zentra.Service.Interfaces.Interfaces.Endpoint;
using Zentra.Service.Interfaces.Interfaces.Endpoint.Results;

namespace Zentra.ProxyService.Hosting;

public class ZentraEndpointMiddleware
{
    private readonly ILoggerService loggerService;
    private readonly RequestDelegate next;
    private readonly TokenSettings tokenSettings;

    public ZentraEndpointMiddleware(RequestDelegate next, ILoggerInstance instance, ZentraConfig configSettings)
    {
        this.next = next;
        tokenSettings = configSettings.TokenSettings;
        loggerService = instance.GetLoggerInstance(LoggerKeyConstants.DefaultLoggerKey);
    }

    public async Task InvokeAsync(HttpContext httpContext, IEnumerable<SecurityEndpointModel> endpoints,
        ISessionManagementService session)
    {
        SetZentraBasePath(httpContext);
        SetZentraLoggedUser(httpContext);

        try
        {
            var endpoint = Find(httpContext, endpoints);
            if (endpoint != null)
            {
                loggerService.WriteTo(Log.Debug, "Invoking endpoint: {endpointType} for {url}",
                    endpoint.GetType().FullName, httpContext.Request.Path.ToString());
                var result = await endpoint.ProcessAsync(httpContext);
                if (result != null) await result.ConstructResponseAsync(httpContext);

                return;
            }
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Exception while processing endpoint middleware request.");
            throw;
        }

        await next(httpContext);
    }

    private IEndpoint Find(HttpContext context, IEnumerable<SecurityEndpointModel> endpoints)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));

        foreach (var endpoint in endpoints)
        {
            var path = endpoint.Path;

            if (context.Request.Path.Equals(path, StringComparison.OrdinalIgnoreCase))
            {
                var permission = GetPermissionForEndpoint(context.Request.Path);
                if (permission) return GetEndpointHandler(endpoint, context);

                return null;
            }
        }

        return null;
    }

    private bool GetPermissionForEndpoint(string path)
    {
        return path switch
        {
            OpenIdConstants.EndpointRoutePaths.Token => tokenSettings.EndpointsConfig.EnableTokenEndpoint,
            OpenIdConstants.EndpointRoutePaths.Authorize => tokenSettings.EndpointsConfig.EnableAuthorizeEndpoint,
            OpenIdConstants.EndpointRoutePaths.EndSession => tokenSettings.EndpointsConfig.EnableEndSessionEndpoint,
            OpenIdConstants.EndpointRoutePaths.Introspection => tokenSettings.EndpointsConfig
                .EnableIntrospectionEndpoint,
            OpenIdConstants.EndpointRoutePaths.Revocation =>
                tokenSettings.EndpointsConfig.EnableTokenRevocationEndpoint,
            OpenIdConstants.EndpointRoutePaths.UserInfo => tokenSettings.EndpointsConfig.EnableUserInfoEndpoint,
            _ => true
        };
    }

    private IEndpoint GetEndpointHandler(SecurityEndpointModel endpoint, HttpContext context)
    {
        if (context.RequestServices.GetService(endpoint.Handler) is IEndpoint handler) return handler;

        return null;
    }

    private void SetZentraBasePath(HttpContext httpContext)
    {
        var path = httpContext.Request.PathBase.Value;
        if (path != null && path.EndsWith("/")) path = path[..^1];

        httpContext.Items[AuthenticationConstants.EnvironmentPaths.ZentraBasePath] = path;
    }

    private void SetZentraLoggedUser(HttpContext httpContext)
    {
        var authorizationHeader = httpContext.Request?.Headers["Authorization"].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(authorizationHeader)) return;

        var header = authorizationHeader.Trim();
        if (!header.StartsWith(OpenIdConstants.AuthenticationSchemes.AuthorizationHeaderBearer,
                StringComparison.OrdinalIgnoreCase))
            return;

        var tokenValue = header[OpenIdConstants.AuthenticationSchemes.AuthorizationHeaderBearer.Length..].Trim();
        if (string.IsNullOrWhiteSpace(tokenValue)) return;

        var tokenHandler = new JwtSecurityTokenHandler();
        if (!tokenHandler.CanReadToken(tokenValue)) return;

        try
        {
            var parsedToken = tokenHandler.ReadJwtToken(tokenValue);
            var user = parsedToken.Claims.FirstOrDefault(x => x.Type == OpenIdConstants.ClaimTypes.Sub)?.Value;
            if (!string.IsNullOrWhiteSpace(user)) loggerService.SetLoggedUserName(user);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Debug, ex, "Unable to parse bearer token for logging context.");
        }
    }
}
