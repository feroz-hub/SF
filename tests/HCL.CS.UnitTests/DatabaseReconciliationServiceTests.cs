using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using HCL.CS.Infrastructure.Data;
using HCL.CS.Infrastructure.Data.Validation;

namespace HCL.CS.UnitTests;

public class DatabaseReconciliationServiceTests
{
    [Fact]
    public async Task ExactSchemaMatch_DefaultsToDryRunAndReturnsConfirmation()
    {
        await using var context = await CreateContextAsync();
        var service = CreateService(context, CompatibleInspection());

        var result = await service.ReconcileMigrationHistoryAsync(new ReconciliationRequest());

        result.Success.Should().BeTrue();
        result.DryRun.Should().BeTrue();
        result.ConfirmationToken.Should().NotBeNullOrWhiteSpace();
        (await ReadAppliedAsync(context)).Should().NotContain(
            DatabaseReconciliationService.Phase2MigrationId);
    }

    [Theory]
    [InlineData("MissingTable")]
    [InlineData("MissingColumn")]
    [InlineData("WrongDataType")]
    [InlineData("WrongLength")]
    [InlineData("WrongPrecision")]
    [InlineData("WrongNullability")]
    [InlineData("WrongDefault")]
    [InlineData("MissingPrimaryKey")]
    [InlineData("MissingIndex")]
    [InlineData("WrongIndexFilter")]
    [InlineData("MissingForeignKey")]
    [InlineData("WrongDeleteBehaviour")]
    [InlineData("PartialPhase2")]
    [InlineData("PartialPhase2B")]
    [InlineData("PartialPhase2C")]
    [InlineData("DuplicateData")]
    public async Task PhysicalSchemaMismatch_IsReportedAndNeverWrites(string differenceKind)
    {
        await using var context = await CreateContextAsync();
        var inspection = IncompatibleInspection(differenceKind);
        var service = CreateService(context, inspection);

        var result = await service.ReconcileMigrationHistoryAsync(new ReconciliationRequest());

        result.Success.Should().BeFalse();
        result.ErrorCode.Should().Be("SCHEMA_MISMATCH");
        result.Differences.Should().ContainSingle(difference => difference.Kind == differenceKind);
        (await ReadAppliedAsync(context)).Should().NotContain(
            DatabaseReconciliationService.Phase2MigrationId);
    }

    [Fact]
    public async Task DuplicateNormalizedEmail_IsReported()
    {
        await using var context = await CreateContextAsync();
        var service = CreateService(context, IncompatibleInspection("DuplicateData", "HclCs_Users.NormalizedEmail"));

        var result = await service.ReconcileMigrationHistoryAsync(new ReconciliationRequest());

        result.Differences.Should().ContainSingle(difference =>
            difference.Kind == "DuplicateData"
            && difference.ObjectName == "HclCs_Users.NormalizedEmail");
    }

    [Fact]
    public async Task MigrationHistoryAlreadyPresent_ProducesNoWriteToken()
    {
        await using var context = await CreateContextAsync(includeReconciliableHistory: true);
        var service = CreateService(context, CompatibleInspection());

        var result = await service.ReconcileMigrationHistoryAsync(new ReconciliationRequest());

        result.Success.Should().BeTrue();
        result.MissingHistoryMigrations.Should().BeEmpty();
        result.ConfirmationToken.Should().BeEmpty();
    }

    [Fact]
    public async Task UnknownRequestedMigration_IsRejected()
    {
        await using var context = await CreateContextAsync();
        var service = CreateService(context, CompatibleInspection());

        var result = await service.ReconcileMigrationHistoryAsync(
            new ReconciliationRequest
            {
                MigrationIds = new[] { "20990101000000_UnknownMigration" }
            });

        result.Success.Should().BeFalse();
        result.ErrorCode.Should().Be("UNKNOWN_MIGRATION");
    }

    [Fact]
    public async Task UnknownAppliedMigration_IsRejected()
    {
        await using var context = await CreateContextAsync();
        await InsertHistoryAsync(context, "20990101000000_UnknownMigration");
        var service = CreateService(context, CompatibleInspection());

        var result = await service.ReconcileMigrationHistoryAsync(new ReconciliationRequest());

        result.Success.Should().BeFalse();
        result.ErrorCode.Should().Be("UNKNOWN_APPLIED_MIGRATION");
    }

    [Fact]
    public async Task UnsupportedProvider_IsRejected()
    {
        await using var context = await CreateContextAsync();
        var inspector = new QueueInspector(
            "Unsupported.Provider",
            supportsProvider: false,
            CompatibleInspection());
        var service = new DatabaseReconciliationService(
            context,
            inspector,
            new ReconciliationTransactionHook());

        var result = await service.ReconcileMigrationHistoryAsync(new ReconciliationRequest());

        result.Success.Should().BeFalse();
        result.ErrorCode.Should().Be("UNSUPPORTED_PROVIDER");
    }

    [Fact]
    public async Task OperatorDidNotConfirm_WriteIsRejected()
    {
        await using var context = await CreateContextAsync();
        var service = CreateService(context, CompatibleInspection());

        var result = await service.ReconcileMigrationHistoryAsync(
            new ReconciliationRequest { DryRun = false, Apply = true });

        result.Success.Should().BeFalse();
        result.ErrorCode.Should().Be("CONFIRMATION_REQUIRED");
        (await ReadAppliedAsync(context)).Should().NotContain(
            DatabaseReconciliationService.Phase2MigrationId);
    }

    [Fact]
    public async Task ConfirmedWrite_UsesTransactionAndVerifiesHistory()
    {
        await using var context = await CreateContextAsync();
        var inspector = new QueueInspector(
            "Microsoft.EntityFrameworkCore.Sqlite",
            true,
            CompatibleInspection(),
            CompatibleInspection(),
            CompatibleInspection(),
            CompatibleInspection());
        var service = new DatabaseReconciliationService(
            context,
            inspector,
            new ReconciliationTransactionHook());
        var dryRun = await service.ReconcileMigrationHistoryAsync(new ReconciliationRequest());

        var applied = await service.ReconcileMigrationHistoryAsync(
            new ReconciliationRequest
            {
                DryRun = false,
                Apply = true,
                ConfirmationToken = dryRun.ConfirmationToken
            });

        applied.Success.Should().BeTrue();
        applied.DryRun.Should().BeFalse();
        applied.ReconciledMigrations.Should().BeEquivalentTo(
            DatabaseReconciliationService.Phase2MigrationId,
            DatabaseReconciliationService.Phase2BMigrationId,
            DatabaseReconciliationService.Phase2CMigrationId);
        (await ReadAppliedAsync(context)).Should().Contain(applied.ReconciledMigrations);
        inspector.InspectionCount.Should().Be(4);
    }

    [Fact]
    public async Task FailureAfterHistoryInsert_RollsBackTransaction()
    {
        await using var context = await CreateContextAsync();
        var service = CreateService(
            context,
            CompatibleInspection(),
            new ThrowingTransactionHook());
        var dryRun = await service.ReconcileMigrationHistoryAsync(new ReconciliationRequest());

        var result = await service.ReconcileMigrationHistoryAsync(
            new ReconciliationRequest
            {
                DryRun = false,
                Apply = true,
                ConfirmationToken = dryRun.ConfirmationToken
            });

        result.Success.Should().BeFalse();
        result.ErrorCode.Should().Be("RECONCILIATION_ROLLED_BACK");
        (await ReadAppliedAsync(context)).Should().NotContain(
            DatabaseReconciliationService.Phase2MigrationId);
    }

    [Fact]
    public async Task PostWriteVerificationFailure_RollsBackTransaction()
    {
        await using var context = await CreateContextAsync();
        var inspector = new QueueInspector(
            "Microsoft.EntityFrameworkCore.Sqlite",
            true,
            CompatibleInspection(),
            CompatibleInspection(),
            CompatibleInspection(),
            IncompatibleInspection("PostWriteMismatch"));
        var service = new DatabaseReconciliationService(
            context,
            inspector,
            new ReconciliationTransactionHook());
        var dryRun = await service.ReconcileMigrationHistoryAsync(new ReconciliationRequest());

        var result = await service.ReconcileMigrationHistoryAsync(
            new ReconciliationRequest
            {
                DryRun = false,
                Apply = true,
                ConfirmationToken = dryRun.ConfirmationToken
            });

        result.Success.Should().BeFalse();
        result.ErrorCode.Should().Be("RECONCILIATION_ROLLED_BACK");
        (await ReadAppliedAsync(context)).Should().NotContain(
            DatabaseReconciliationService.Phase2MigrationId);
    }

    private static DatabaseReconciliationService CreateService(
        DbContext context,
        SchemaInspectionResult inspection,
        IReconciliationTransactionHook hook = null)
    {
        return new DatabaseReconciliationService(
            context,
            new QueueInspector(
                "Microsoft.EntityFrameworkCore.Sqlite",
                true,
                inspection,
                inspection,
                inspection,
                inspection),
            hook ?? new ReconciliationTransactionHook());
    }

    private static SchemaInspectionResult CompatibleInspection()
    {
        return new SchemaInspectionResult
        {
            Fingerprint = "exact-schema",
            VerificationLogs = new[] { "Complete physical schema inspection succeeded." }
        };
    }

    private static SchemaInspectionResult IncompatibleInspection(
        string kind,
        string objectName = "schema.object")
    {
        return new SchemaInspectionResult
        {
            Fingerprint = $"mismatch-{kind}",
            Differences = new[]
            {
                new SchemaDifference(kind, objectName, "expected", "actual")
            }
        };
    }

    private static async Task<SqLiteApplicationDbContext> CreateContextAsync(
        bool includeReconciliableHistory = false)
    {
        var options = new DbContextOptionsBuilder<SqLiteApplicationDbContext>()
            .UseSqlite("Data Source=:memory:")
            .Options;
        var context = new SqLiteApplicationDbContext(options);
        await context.Database.OpenConnectionAsync();
        await context.Database.ExecuteSqlRawAsync(
            """
            CREATE TABLE "__EFMigrationsHistory" (
                "MigrationId" TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
                "ProductVersion" TEXT NOT NULL
            );
            """);
        await InsertHistoryAsync(context, "20230428105853_HclCsSqliteV1");
        if (includeReconciliableHistory)
        {
            await InsertHistoryAsync(context, DatabaseReconciliationService.Phase2MigrationId);
            await InsertHistoryAsync(context, DatabaseReconciliationService.Phase2BMigrationId);
            await InsertHistoryAsync(context, DatabaseReconciliationService.Phase2CMigrationId);
        }
        return context;
    }

    private static Task InsertHistoryAsync(DbContext context, string migrationId)
    {
        return context.Database.ExecuteSqlInterpolatedAsync(
            $"""
             INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
             VALUES ({migrationId}, {"8.0.11"})
             """);
    }

    private static async Task<string[]> ReadAppliedAsync(DbContext context)
    {
        return (await context.Database.GetAppliedMigrationsAsync()).ToArray();
    }

    private sealed class QueueInspector : IReconciliationSchemaInspector
    {
        private readonly Queue<SchemaInspectionResult> inspections;
        private SchemaInspectionResult lastInspection;

        public QueueInspector(
            string providerName,
            bool supportsProvider,
            params SchemaInspectionResult[] inspections)
        {
            ProviderName = providerName;
            SupportsProvider = supportsProvider;
            this.inspections = new Queue<SchemaInspectionResult>(inspections);
            lastInspection = inspections.LastOrDefault() ?? CompatibleInspection();
        }

        public string ProviderName { get; }
        public bool SupportsProvider { get; }
        public int InspectionCount { get; private set; }

        public Task<SchemaInspectionResult> InspectAsync(
            IReadOnlyList<string> migrationIds,
            CancellationToken cancellationToken = default)
        {
            InspectionCount++;
            if (inspections.Count > 0) lastInspection = inspections.Dequeue();
            return Task.FromResult(lastInspection);
        }
    }

    private sealed class ThrowingTransactionHook : IReconciliationTransactionHook
    {
        public Task AfterHistoryInsertAsync(
            IReadOnlyList<string> insertedMigrations,
            CancellationToken cancellationToken = default)
        {
            throw new InvalidOperationException("Forced failure after history insert.");
        }
    }
}
