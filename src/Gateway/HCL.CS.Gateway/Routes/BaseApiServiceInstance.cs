/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using Microsoft.Extensions.DependencyInjection;
using HCL.CS.Service.Interfaces.Interfaces.Api;

namespace HCL.CS.ProxyService.Routes;

internal abstract class BaseApiServiceInstance
{
    private readonly IServiceProvider serviceProvider;
    private IApiResourceService apiResourceService;
    private IAuditTrailService auditTrailService;
    private IAuthenticationService authenticationService;
    private IClientServices clientServices;
    private IIdentityResourceService identityResourceService;
    private IRoleService roleService;
    private ISecurityTokenService securityTokenService;
    private IUserAccountService userAccountService;
    private INotificationManagementService notificationManagementService;
    private IExternalAuthManagementService externalAuthManagementService;

    protected BaseApiServiceInstance(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public IUserAccountService UserAccountService
    {
        get
        {
            if (userAccountService != null) return userAccountService;

            userAccountService = serviceProvider.GetService<IUserAccountService>();
            return userAccountService;
        }
    }

    public IApiResourceService ApiResourceService
    {
        get
        {
            if (apiResourceService != null) return apiResourceService;

            apiResourceService = serviceProvider.GetService<IApiResourceService>();
            return apiResourceService;
        }
    }

    public IIdentityResourceService IdentityResourceService
    {
        get
        {
            if (identityResourceService != null) return identityResourceService;

            identityResourceService = serviceProvider.GetService<IIdentityResourceService>();
            return identityResourceService;
        }
    }

    public IRoleService RoleService
    {
        get
        {
            if (roleService != null) return roleService;

            roleService = serviceProvider.GetService<IRoleService>();
            return roleService;
        }
    }

    public IAuditTrailService AuditTrailService
    {
        get
        {
            if (auditTrailService != null) return auditTrailService;

            auditTrailService = serviceProvider.GetService<IAuditTrailService>();
            return auditTrailService;
        }
    }

    public IAuthenticationService AuthenticationService
    {
        get
        {
            if (authenticationService != null) return authenticationService;

            authenticationService = serviceProvider.GetService<IAuthenticationService>();
            return authenticationService;
        }
    }

    public IClientServices ClientServices
    {
        get
        {
            if (clientServices != null) return clientServices;

            clientServices = serviceProvider.GetService<IClientServices>();
            return clientServices;
        }
    }

    public ISecurityTokenService SecurityTokenService
    {
        get
        {
            if (securityTokenService != null) return securityTokenService;

            securityTokenService = serviceProvider.GetService<ISecurityTokenService>();
            return securityTokenService;
        }
    }

    public INotificationManagementService NotificationManagementService
    {
        get
        {
            if (notificationManagementService != null) return notificationManagementService;

            notificationManagementService = serviceProvider.GetService<INotificationManagementService>();
            return notificationManagementService;
        }
    }

    public IExternalAuthManagementService ExternalAuthManagementService
    {
        get
        {
            if (externalAuthManagementService != null) return externalAuthManagementService;

            externalAuthManagementService = serviceProvider.GetService<IExternalAuthManagementService>();
            return externalAuthManagementService;
        }
    }
}
