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
    private static readonly TimeSpan DependencyTimeout = TimeSpan.FromSeconds(3);
    private readonly DbContext dbContext;

    public DatabaseDependencyHealthCheck(IApplicationDbContext dbContext)
    {
        this.dbContext = (DbContext)dbContext;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        using var timeoutToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutToken.CancelAfter(DependencyTimeout);

        try
        {
            var provider = dbContext.Database.ProviderName ?? "Unknown";
            var connectivity = await dbContext.Database.CanConnectAsync(timeoutToken.Token);
            if (!connectivity)
            {
                return HealthCheckResult.Unhealthy("Unable to connect to database.", data: new Dictionary<string, object>
                {
                    { "Provider", provider },
                    { "ConnectivityStatus", false }
                });
            }

            var applied = (await dbContext.Database.GetAppliedMigrationsAsync(timeoutToken.Token)).ToList();
            var pending = (await dbContext.Database.GetPendingMigrationsAsync(timeoutToken.Token)).ToList();
            bool isCompatible = pending.Count == 0;

            var data = new Dictionary<string, object>
            {
                { "Provider", provider },
                { "ConnectivityStatus", true },
                { "SchemaCompatibilityStatus", isCompatible },
                { "AppliedMigrationsCount", applied.Count },
                { "PendingMigrationsCount", pending.Count }
            };

            if (isCompatible)
            {
                return HealthCheckResult.Healthy("Database schema and connectivity validated.", data);
            }

            return HealthCheckResult.Degraded($"Database schema is behind. Pending migration count: {pending.Count}", data: data);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            return HealthCheckResult.Unhealthy("Database health check timed out.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy($"Database health check failed: {ex.Message}");
        }
    }
}
