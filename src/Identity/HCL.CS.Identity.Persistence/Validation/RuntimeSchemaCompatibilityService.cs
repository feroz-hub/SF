/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using Microsoft.EntityFrameworkCore;
using HCL.CS.Domain;
using HCL.CS.Domain.Entities.Api;
using HCL.CS.Domain.Entities.Endpoint;

namespace HCL.CS.Infrastructure.Data.Validation;

public sealed class RuntimeSchemaCompatibilityService
{
    private readonly DbContext _dbContext;

    public RuntimeSchemaCompatibilityService(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RuntimeSchemaCompatibilityReport> ValidateCompatibilityAsync(CancellationToken cancellationToken = default)
    {
        var provider = _dbContext.Database.ProviderName ?? "Unknown";
        var report = new RuntimeSchemaCompatibilityReport
        {
            Provider = provider
        };

        // 1. Check Connectivity
        try
        {
            report.ConnectivityStatus = await _dbContext.Database.CanConnectAsync(cancellationToken);
        }
        catch
        {
            report.ConnectivityStatus = false;
            report.ActionRequired = "Database endpoint is unreachable. Verify connection string and network accessibility.";
            return report;
        }

        if (!report.ConnectivityStatus)
        {
            report.ActionRequired = "Unable to connect to database.";
            return report;
        }

        // 2. Check Migrations
        try
        {
            var knownMigrations = _dbContext.Database.GetMigrations();
            var appliedMigrations = (await _dbContext.Database.GetAppliedMigrationsAsync(cancellationToken)).ToList();
            var pendingMigrations = (await _dbContext.Database.GetPendingMigrationsAsync(cancellationToken)).ToList();
            var unknownMigrations = appliedMigrations.Except(knownMigrations).ToList();

            report.AppliedMigrations = appliedMigrations;
            report.PendingMigrations = pendingMigrations;
            report.UnknownMigrations = unknownMigrations;

            if (pendingMigrations.Count > 0)
            {
                report.SchemaCompatibilityStatus = false;
                report.ActionRequired = $"Database schema is behind application version. Pending migration(s): {string.Join(", ", pendingMigrations)}. Execute the HCL.CS Installer or approved migration runner.";
            }
            else if (unknownMigrations.Count > 0)
            {
                report.SchemaCompatibilityStatus = false;
                report.ActionRequired = $"Database contains unknown future migration(s): {string.Join(", ", unknownMigrations)}. Upgrade the HCL.CS application version.";
            }
            else
            {
                report.SchemaCompatibilityStatus = true;
            }
        }
        catch (Exception ex)
        {
            report.SchemaCompatibilityStatus = false;
            report.ActionRequired = $"Failed to query migration history: {ex.Message}";
        }

        // 3. Check Bootstrap Data Readiness
        try
        {
            var userCount = await _dbContext.Set<Users>().Take(1).CountAsync(cancellationToken);
            var roleCount = await _dbContext.Set<Roles>().Take(1).CountAsync(cancellationToken);
            var clientCount = await _dbContext.Set<Clients>().Take(1).CountAsync(cancellationToken);

            report.BootstrapReadinessStatus = (userCount > 0 || roleCount > 0 || clientCount > 0);
            if (!report.BootstrapReadinessStatus && report.SchemaCompatibilityStatus)
            {
                report.ActionRequired = "Database schema is up to date, but core HCL.CS framework bootstrap data is missing. Run HCL.CS Installer seed service.";
            }
        }
        catch
        {
            report.BootstrapReadinessStatus = false;
        }

        if (report.SchemaCompatibilityStatus && report.BootstrapReadinessStatus && string.IsNullOrEmpty(report.ActionRequired))
        {
            report.ActionRequired = "None. Database schema and bootstrap data are fully compatible.";
        }

        return report;
    }
}

public sealed class RuntimeSchemaCompatibilityReport
{
    public string Provider { get; set; } = string.Empty;
    public bool ConnectivityStatus { get; set; }
    public bool SchemaCompatibilityStatus { get; set; }
    public bool BootstrapReadinessStatus { get; set; }
    public IReadOnlyList<string> AppliedMigrations { get; set; } = Array.Empty<string>();
    public IReadOnlyList<string> PendingMigrations { get; set; } = Array.Empty<string>();
    public IReadOnlyList<string> UnknownMigrations { get; set; } = Array.Empty<string>();
    public string ActionRequired { get; set; } = string.Empty;
}
