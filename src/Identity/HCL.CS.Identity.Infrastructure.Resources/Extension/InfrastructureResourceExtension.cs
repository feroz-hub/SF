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
