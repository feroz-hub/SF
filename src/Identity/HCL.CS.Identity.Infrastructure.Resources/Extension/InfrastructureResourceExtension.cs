/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using Microsoft.Extensions.DependencyInjection;
using HCL.CS.Domain;
using HCL.CS.Domain.Models.Endpoint;
using HCL.CS.DomainServices.Infra;
using HCL.CS.Infrastructure.Resources;

namespace HCL.CS.Infrastructure.Resources.Extension;

public static class InfrastructureResourceExtension
{
    public static IServiceCollection AddInfrastructureResources(this IServiceCollection services)
    {
        services.AddSingleton<IResourceStringHandler, ResourceStringHandler>();
        // services.AddSingleton<ICSExceptionManager, CustomExceptionManager>();
        return services;
    }

    public static IServiceCollection AddAutoMapper(this IServiceCollection services)
    {
        var config = new Mapper().InitializeMapper();
        var mapper = config.CreateMapper();
        services.AddSingleton(mapper);
        return services;
    }

    public static IServiceCollection AddConfiguration(this IServiceCollection services, HclCsConfig config)
    {
        services.AddSingleton(config);
        return services;
    }

    public static IServiceCollection AddUtilityServices(this IServiceCollection services)
    {
        services.AddTransient<IKeyStore, KeyStore>();
        return services;
    }

    public static IServiceCollection AddSecurityAsymmetricKeystore(this IServiceCollection services,
        IEnumerable<AsymmetricKeyInfoModel> securityKeys)
    {
        var keyStore = new KeyStore().Add(securityKeys);
        if (keyStore.Count > 0) services.AddSingleton(keyStore);

        return services;
    }
}
