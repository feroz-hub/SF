/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using Microsoft.Extensions.DependencyInjection;
using HCL.CS.ProxyService.Proxy;
using HCL.CS.ProxyService.Routes;
using HCL.CS.ProxyService.Validator;
using HCL.CS.Service.Interfaces.Interfaces.Api;
using HCL.CS.Service.Interfaces.Interfaces.Api.Wrapper;

namespace HCL.CS.ProxyService.Extension;

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
