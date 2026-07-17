using Microsoft.Extensions.DependencyInjection;

namespace Zentra.Service;

public static class DependencyInjection
{
    public static IServiceCollection AddZentraIdentityApplication(this IServiceCollection services)
    {
        return services;
    }
}
