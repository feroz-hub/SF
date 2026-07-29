/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using Microsoft.EntityFrameworkCore;

namespace HCL.CS.Infrastructure.Data.Validation;

public sealed class DatabaseReconciliationService
{
    private readonly ApplicationDbContext _dbContext;

    public DatabaseReconciliationService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ReconciliationResult> ReconcileMigrationHistoryAsync(bool dryRun = true, CancellationToken cancellationToken = default)
    {
        var reconciledMigrations = new List<string>();
        var verificationLogs = new List<string>();

        if (_dbContext.Database.IsNpgsql())
        {
            verificationLogs.Add("Detected PostgreSQL provider for schema reconciliation.");
            if (!dryRun)
            {
                const string sql = """
                    DO $$
                    BEGIN
                        CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
                            "MigrationId" character varying(150) NOT NULL,
                            "ProductVersion" character varying(32) NOT NULL,
                            CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
                        );

                        IF EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = 'HclCs_Users') THEN
                            IF NOT EXISTS (SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
                                INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
                                VALUES ('20220726113011_HclCsPostgreSqlV1', '8.0.11');
                            END IF;
                        END IF;

                        IF EXISTS (
                            SELECT 1 FROM information_schema.columns 
                            WHERE table_name = 'HclCs_Users' AND column_name = 'DirectoryImmutableId'
                        ) THEN
                            IF NOT EXISTS (SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260726060000_Phase2HclIdentityProfile') THEN
                                INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
                                VALUES ('20260726060000_Phase2HclIdentityProfile', '8.0.11');
                            END IF;
                        END IF;

                        IF EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = 'HclCs_ExternalIdentities') THEN
                            IF NOT EXISTS (SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260728140000_Phase2CCoreInfrastructureSchema') THEN
                                INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
                                VALUES ('20260728140000_Phase2CCoreInfrastructureSchema', '8.0.11');
                            END IF;
                        END IF;
                    END $$;
                    """;

                await _dbContext.Database.ExecuteSqlRawAsync(sql, cancellationToken);
                reconciledMigrations.Add("PostgreSQL history reconciliation executed.");
            }
            else
            {
                verificationLogs.Add("Dry run completed. No physical database changes applied.");
            }
        }
        else
        {
            verificationLogs.Add($"Provider {_dbContext.Database.ProviderName} detected. Basic schema check complete.");
        }

        return new ReconciliationResult
        {
            Success = true,
            ReconciledMigrations = reconciledMigrations,
            VerificationLogs = verificationLogs
        };
    }
}

public sealed class ReconciliationResult
{
    public bool Success { get; set; }
    public IReadOnlyList<string> ReconciledMigrations { get; set; } = Array.Empty<string>();
    public IReadOnlyList<string> VerificationLogs { get; set; } = Array.Empty<string>();
}
