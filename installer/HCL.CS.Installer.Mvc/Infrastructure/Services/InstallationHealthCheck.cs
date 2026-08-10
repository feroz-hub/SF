/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using Microsoft.Extensions.Diagnostics.HealthChecks;
using HclCsInstallerMVC.Application.Abstractions;

namespace HclCsInstallerMVC.Infrastructure.Services;

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
