using Microsoft.Extensions.DependencyInjection;
using Zentra.ProxyService.Proxy;
using Zentra.ProxyService.Routes;
using Zentra.ProxyService.Validator;
using Zentra.Service.Interfaces.Interfaces.Api;
using Zentra.Service.Interfaces.Interfaces.Api.Wrapper;

namespace Zentra.ProxyService.Extension;

public static class ServiceExtension
{
    public static IServiceCollection AddProxyServices(this IServiceCollection services)
    {
        services.AddTransient<IApiResourceService, ApiResourceProxyService>();
        services.AddTransient<IIdentityResourceService, IdentityResourceProxyService>();
        services.AddTransient<IRoleService, RoleProxyService>();
        services.AddTransient<IUserAccountService, UserAccountProxyServices>();
        services.AddTransient<IAuditTrailService, AuditTrailProxyService>();
        services.AddTransient<IAuthenticationService, AuthenticationProxyService>();
        services.AddTransient<IClientServices, ClientProxyService>();
        services.AddTransient<ISecurityTokenService, SecurityTokenProxyService>();
        services.AddTransient<INotificationManagementService, NotificationManagementProxyService>();
        services.AddTransient<IExternalAuthManagementService, ExternalAuthManagementProxyService>();
        return services;
    }

    public static IServiceCollection AddProxyValidator(this IServiceCollection services)
    {
        services.AddTransient<IApiValidator, ApiValidator>();
        return services;
    }

    public static IServiceCollection AddProxyRoutes(this IServiceCollection services)
    {
        services.AddTransient<IApiGateway, ApiGateway>();
        return services;
    }
}
