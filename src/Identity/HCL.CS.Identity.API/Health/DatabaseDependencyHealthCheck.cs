/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using HCL.CS.DomainServices;

namespace HCL.CS.Hosting.Health;

public class DatabaseDependencyHealthCheck : IHealthCheck
{
    private static readonly TimeSpan DependencyTimeout = TimeSpan.FromSeconds(2);
    private readonly IApplicationDbContext dbContext;

    public DatabaseDependencyHealthCheck(IApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        using var timeoutToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutToken.CancelAfter(DependencyTimeout);

        try
        {
            _ = await dbContext.Users.AnyAsync(timeoutToken.Token);
            return HealthCheckResult.Healthy("Database reachable.");
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            return HealthCheckResult.Unhealthy("Database health check timed out.");
        }
        catch
        {
            return HealthCheckResult.Unhealthy("Database health check failed.");
        }
    }
}
