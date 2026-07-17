using Microsoft.Extensions.Diagnostics.HealthChecks;
using ZentraInstallerMVC.Application.Abstractions;

namespace ZentraInstallerMVC.Infrastructure.Services;

public sealed class InstallationHealthCheck : IHealthCheck
{
    private readonly IInstallationGateService _installationGateService;

    public InstallationHealthCheck(IInstallationGateService installationGateService)
    {
        _installationGateService = installationGateService;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var completed = await _installationGateService.IsInstallationCompletedAsync(cancellationToken);
        return completed
            ? HealthCheckResult.Healthy("Installation marker is present.")
            : HealthCheckResult.Degraded("Installation has not completed.");
    }
}
