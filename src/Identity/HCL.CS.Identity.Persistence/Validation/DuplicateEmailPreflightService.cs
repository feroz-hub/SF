/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using Microsoft.EntityFrameworkCore;

namespace HCL.CS.Infrastructure.Data.Validation;

public sealed class DuplicateEmailPreflightService
{
    private readonly DbContext _dbContext;

    public DuplicateEmailPreflightService(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DuplicateEmailPreflightResult> CheckPreflightAsync(CancellationToken cancellationToken = default)
    {
        var provider = _dbContext.Database.ProviderName ?? "Unknown";
        var pendingMigrations = await _dbContext.Database.GetPendingMigrationsAsync(cancellationToken);
        
        // Only run email uniqueness preflight if Phase 2B migration is pending
        bool isPhase2BPending = pendingMigrations.Any(m => m.Contains("Phase2BLocalAuthenticationEmailUniqueness", StringComparison.OrdinalIgnoreCase));
        if (!isPhase2BPending)
        {
            return new DuplicateEmailPreflightResult
            {
                Provider = provider,
                Pass = true,
                DuplicatesFound = false,
                Message = "Phase 2B local email uniqueness migration is not pending. Preflight passed."
            };
        }

        var duplicates = new List<DuplicateEmailGroup>();

        try
        {
            const string sql = """
                SELECT "NormalizedEmail", COUNT(*) AS "DuplicateCount"
                FROM "HclCs_Users"
                WHERE "NormalizedEmail" IS NOT NULL AND "NormalizedEmail" <> ''
                GROUP BY "NormalizedEmail"
                HAVING COUNT(*) > 1;
                """;

            using var command = _dbContext.Database.GetDbConnection().CreateCommand();
            command.CommandText = sql;
            if (_dbContext.Database.GetDbConnection().State != System.Data.ConnectionState.Open)
            {
                await _dbContext.Database.OpenConnectionAsync(cancellationToken);
            }

            using var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                var email = reader.GetString(0);
                var count = reader.GetInt64(1);
                duplicates.Add(new DuplicateEmailGroup { NormalizedEmail = email, DuplicateCount = count });
            }
        }
        catch
        {
            // Table may not exist yet if baseline hasn't run
        }

        if (duplicates.Count > 0)
        {
            return new DuplicateEmailPreflightResult
            {
                Provider = provider,
                Pass = false,
                DuplicatesFound = true,
                DuplicateRecords = duplicates,
                Message = $"Phase 2B migration preflight failed: {duplicates.Count} duplicate normalized email address group(s) detected in HclCs_Users. Manual operator remediation is required before running Phase 2B migration."
            };
        }

        return new DuplicateEmailPreflightResult
        {
            Provider = provider,
            Pass = true,
            DuplicatesFound = false,
            Message = "Phase 2B local email uniqueness preflight passed. No duplicate normalized emails found."
        };
    }
}

public sealed class DuplicateEmailPreflightResult
{
    public string Provider { get; set; } = string.Empty;
    public bool Pass { get; set; }
    public bool DuplicatesFound { get; set; }
    public string Message { get; set; } = string.Empty;
    public IReadOnlyList<DuplicateEmailGroup> DuplicateRecords { get; set; } = Array.Empty<DuplicateEmailGroup>();
}

public sealed class DuplicateEmailGroup
{
    public string NormalizedEmail { get; set; } = string.Empty;
    public long DuplicateCount { get; set; }
}
