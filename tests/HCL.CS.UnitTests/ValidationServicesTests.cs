/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using System.Threading.Tasks;
using FluentAssertions;
using HCL.CS.Infrastructure.Data;
using HCL.CS.Infrastructure.Data.Validation;
using HclCsInstallerMVC.Application.DTOs;
using HclCsInstallerMVC.Infrastructure.Configuration;
using HclCsInstallerMVC.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace HCL.CS.UnitTests;

public class ValidationServicesTests
{
    [Fact]
    public async Task DuplicateEmailPreflightService_WhenPhase2BNotPending_ReturnsPassed()
    {
        var options = new DbContextOptionsBuilder<SqLiteApplicationDbContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;

        using var dbContext = new SqLiteApplicationDbContext(options);
        await dbContext.Database.OpenConnectionAsync();
        await dbContext.Database.EnsureCreatedAsync();

        var service = new DuplicateEmailPreflightService(dbContext);
        var result = await service.CheckPreflightAsync();

        result.Should().NotBeNull();
        result.Pass.Should().BeTrue();
        result.DuplicatesFound.Should().BeFalse();
    }

    [Fact]
    public async Task DatabaseMigrationService_PreflightFailure_DoesNotInvokeMigrationExecution()
    {
        var databasePath = Path.Combine(
            Path.GetTempPath(),
            $"hclcs-preflight-{Guid.NewGuid():N}.db");
        var connectionString = $"Data Source={databasePath}";

        try
        {
            var options = new DbContextOptionsBuilder<SqLiteApplicationDbContext>()
                .UseSqlite(connectionString)
                .Options;

            await using (var dbContext = new SqLiteApplicationDbContext(options))
            {
                var migrator = dbContext.Database.GetService<IMigrator>();
                await migrator.MigrateAsync("20260726060000_Phase2HclIdentityProfile");
                await dbContext.Database.ExecuteSqlRawAsync(
                    """
                    INSERT INTO "HclCs_Users" (
                        "Id", "FirstName", "TwoFactorType", "IdentityProviderType", "IsDeleted",
                        "CreatedOn", "CreatedBy", "UserName", "NormalizedUserName", "Email",
                        "NormalizedEmail", "EmailConfirmed", "PasswordHash", "PhoneNumberConfirmed",
                        "TwoFactorEnabled", "LockoutEnabled", "AccessFailedCount", "AuthenticationSource"
                    ) VALUES
                    ('11111111-1111-1111-1111-111111111111', 'Duplicate One', 0, 1, 0,
                        CURRENT_TIMESTAMP, 'test', 'duplicate.one', 'DUPLICATE.ONE',
                        'duplicate@example.test', 'DUPLICATE@EXAMPLE.TEST', 1, 'placeholder-1',
                        0, 0, 1, 0, 'LOCAL'),
                    ('22222222-2222-2222-2222-222222222222', 'Duplicate Two', 0, 1, 0,
                        CURRENT_TIMESTAMP, 'test', 'duplicate.two', 'DUPLICATE.TWO',
                        'duplicate@example.test', 'DUPLICATE@EXAMPLE.TEST', 1, 'placeholder-2',
                        0, 0, 1, 0, 'LOCAL');
                    """);
            }

            var loggerFactory = NullLoggerFactory.Instance;
            var service = new DatabaseMigrationService(
                NullLogger<DatabaseMigrationService>.Instance,
                loggerFactory,
                Options.Create(new DatabaseProvisioningOptions { AllowDatabaseCreation = false }));
            var configuration = new DatabaseConfigurationDto
            {
                Provider = DatabaseProviderType.Sqlite,
                ConnectionString = connectionString
            };

            var result = await service.RunMigrationsAsync(configuration, CancellationToken.None);

            result.Succeeded.Should().BeFalse();
            result.ErrorMessage.Should().Contain("NormalizedEmail=DUPLICATE@EXAMPLE.TEST");
            result.ErrorMessage.Should().Contain("DuplicateCount=2");
            result.ErrorMessage.Should().Contain("11111111-1111-1111-1111-111111111111");
            result.ErrorMessage.Should().Contain("22222222-2222-2222-2222-222222222222");

            await using var verificationContext = new SqLiteApplicationDbContext(options);
            var appliedMigrations = await verificationContext.Database.GetAppliedMigrationsAsync();
            appliedMigrations.Should().NotContain("20260727130000_Phase2BLocalAuthenticationEmailUniqueness");

            var duplicateCount = await verificationContext.Database
                .SqlQueryRaw<long>(
                    """SELECT COUNT(*) AS "Value" FROM "HclCs_Users" WHERE "NormalizedEmail" = 'DUPLICATE@EXAMPLE.TEST'""")
                .SingleAsync();
            duplicateCount.Should().Be(2);
        }
        finally
        {
            File.Delete(databasePath);
        }
    }

    [Fact]
    public async Task RuntimeSchemaCompatibilityService_CanQueryCompatibility()
    {
        var options = new DbContextOptionsBuilder<SqLiteApplicationDbContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;

        using var dbContext = new SqLiteApplicationDbContext(options);
        await dbContext.Database.OpenConnectionAsync();
        await dbContext.Database.EnsureCreatedAsync();

        var service = new RuntimeSchemaCompatibilityService(dbContext);
        var report = await service.ValidateCompatibilityAsync();

        report.Should().NotBeNull();
        report.ConnectivityStatus.Should().BeTrue();
        report.Provider.Should().Contain("Sqlite");
    }

    [Fact]
    public async Task DatabaseReconciliationService_DryRun_Succeeds()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;

        using var dbContext = new ApplicationDbContext(options);
        await dbContext.Database.OpenConnectionAsync();
        await dbContext.Database.EnsureCreatedAsync();

        var service = new DatabaseReconciliationService(
            dbContext,
            new CompatibleReconciliationInspector(),
            new ReconciliationTransactionHook());
        var result = await service.ReconcileMigrationHistoryAsync(dryRun: true);

        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.DryRun.Should().BeTrue();
        result.VerificationLogs.Should().Contain(l => l.Contains("Dry run"));
    }

    [Fact]
    public async Task DatabaseReconciliationService_RealSqliteSchemaInspection_Succeeds()
    {
        var options = new DbContextOptionsBuilder<SqLiteApplicationDbContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;

        await using var dbContext = new SqLiteApplicationDbContext(options);
        await dbContext.Database.OpenConnectionAsync();
        await dbContext.Database.MigrateAsync();

        var service = new DatabaseReconciliationService(dbContext);
        var result = await service.ReconcileMigrationHistoryAsync();

        result.Success.Should().BeTrue(
            string.Join(
                Environment.NewLine,
                result.Differences.Select(difference =>
                    $"{difference.Kind}: {difference.ObjectName} expected {difference.Expected}, actual {difference.Actual}")));
        result.Differences.Should().BeEmpty();
        result.DryRun.Should().BeTrue();
    }

    private sealed class CompatibleReconciliationInspector : IReconciliationSchemaInspector
    {
        public string ProviderName => "Microsoft.EntityFrameworkCore.Sqlite";
        public bool SupportsProvider => true;

        public Task<SchemaInspectionResult> InspectAsync(
            IReadOnlyList<string> migrationIds,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new SchemaInspectionResult
            {
                Fingerprint = "compatible-schema",
                VerificationLogs = new[] { "Provider schema inspected." }
            });
        }
    }
}
