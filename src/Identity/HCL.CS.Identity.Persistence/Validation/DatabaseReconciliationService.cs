/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
 */

using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace HCL.CS.Infrastructure.Data.Validation;

public sealed class DatabaseReconciliationService
{
    public const string Phase2MigrationId = "20260726060000_Phase2HclIdentityProfile";
    public const string Phase2BMigrationId = "20260727130000_Phase2BLocalAuthenticationEmailUniqueness";
    public const string Phase2CMigrationId = "20260728140000_Phase2CCoreInfrastructureSchema";

    private static readonly IReadOnlyList<string> ReconciliableMigrations =
        new[] { Phase2MigrationId, Phase2BMigrationId, Phase2CMigrationId };

    private readonly DbContext dbContext;
    private readonly IReconciliationSchemaInspector schemaInspector;
    private readonly IReconciliationTransactionHook transactionHook;

    public DatabaseReconciliationService(DbContext dbContext)
        : this(
            dbContext,
            new RelationalReconciliationSchemaInspector(dbContext),
            new ReconciliationTransactionHook())
    {
    }

    public DatabaseReconciliationService(
        DbContext dbContext,
        IReconciliationSchemaInspector schemaInspector,
        IReconciliationTransactionHook transactionHook)
    {
        this.dbContext = dbContext;
        this.schemaInspector = schemaInspector;
        this.transactionHook = transactionHook;
    }

    public Task<ReconciliationResult> ReconcileMigrationHistoryAsync(
        bool dryRun = true,
        CancellationToken cancellationToken = default)
    {
        return ReconcileMigrationHistoryAsync(
            new ReconciliationRequest
            {
                DryRun = dryRun,
                Apply = !dryRun
            },
            cancellationToken);
    }

    public async Task<ReconciliationResult> ReconcileMigrationHistoryAsync(
        ReconciliationRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var requestedMigrations = (request.MigrationIds?.Count > 0
                ? request.MigrationIds
                : ReconciliableMigrations)
            .Distinct(StringComparer.Ordinal)
            .ToArray();
        var unknownRequested = requestedMigrations
            .Except(ReconciliableMigrations, StringComparer.Ordinal)
            .ToArray();
        if (unknownRequested.Length > 0)
        {
            return ReconciliationResult.Failed(
                "UNKNOWN_MIGRATION",
                $"Unknown reconciliation migration ID(s): {string.Join(", ", unknownRequested)}.");
        }

        if (!schemaInspector.SupportsProvider)
        {
            return ReconciliationResult.Failed(
                "UNSUPPORTED_PROVIDER",
                $"Provider '{schemaInspector.ProviderName}' is not approved for reconciliation writes.");
        }

        var appliedMigrations = (await dbContext.Database
                .GetAppliedMigrationsAsync(cancellationToken))
            .ToHashSet(StringComparer.Ordinal);
        var knownMigrations = dbContext.Database.GetMigrations()
            .ToHashSet(StringComparer.Ordinal);
        var unknownApplied = appliedMigrations
            .Except(knownMigrations, StringComparer.Ordinal)
            .ToArray();
        if (unknownApplied.Length > 0)
        {
            return ReconciliationResult.Failed(
                "UNKNOWN_APPLIED_MIGRATION",
                $"Database history contains unknown migration ID(s): {string.Join(", ", unknownApplied)}.");
        }

        var missingHistory = requestedMigrations
            .Where(migrationId => !appliedMigrations.Contains(migrationId))
            .ToArray();
        var inspection = await schemaInspector.InspectAsync(requestedMigrations, cancellationToken);
        var confirmationToken = CreateConfirmationToken(
            schemaInspector.ProviderName,
            missingHistory,
            inspection.Fingerprint);
        var report = new ReconciliationResult
        {
            Success = inspection.IsCompatible,
            DryRun = true,
            Provider = schemaInspector.ProviderName,
            RequestedMigrations = requestedMigrations,
            MissingHistoryMigrations = missingHistory,
            Differences = inspection.Differences,
            VerificationLogs = inspection.VerificationLogs,
            ConfirmationToken = inspection.IsCompatible && missingHistory.Length > 0
                ? confirmationToken
                : string.Empty
        };

        if (!inspection.IsCompatible)
        {
            report.ErrorCode = "SCHEMA_MISMATCH";
            report.ErrorMessage = "Physical schema verification failed. Migration history was not changed.";
            return report;
        }

        if (request.DryRun || !request.Apply)
        {
            report.VerificationLogs = report.VerificationLogs
                .Append("Dry run completed. No migration-history changes were applied.")
                .ToArray();
            return report;
        }

        if (string.IsNullOrWhiteSpace(request.ConfirmationToken)
            || !FixedTimeEquals(request.ConfirmationToken, confirmationToken))
        {
            return ReconciliationResult.Failed(
                "CONFIRMATION_REQUIRED",
                "Apply requires the exact confirmation token produced by a compatible dry run.");
        }

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var inTransactionInspection = await schemaInspector
                .InspectAsync(requestedMigrations, cancellationToken);
            var inTransactionToken = CreateConfirmationToken(
                schemaInspector.ProviderName,
                missingHistory,
                inTransactionInspection.Fingerprint);
            if (!inTransactionInspection.IsCompatible
                || !FixedTimeEquals(request.ConfirmationToken, inTransactionToken))
            {
                throw new ReconciliationValidationException(
                    "Physical schema changed after the dry run; reconciliation was rolled back.");
            }

            var currentApplied = (await dbContext.Database
                    .GetAppliedMigrationsAsync(cancellationToken))
                .ToHashSet(StringComparer.Ordinal);
            var inserted = new List<string>();
            foreach (var migrationId in missingHistory)
            {
                if (currentApplied.Contains(migrationId)) continue;

                await dbContext.Database.ExecuteSqlInterpolatedAsync(
                    $"""
                     INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
                     VALUES ({migrationId}, {"8.0.11"})
                     """,
                    cancellationToken);
                inserted.Add(migrationId);
            }

            await transactionHook.AfterHistoryInsertAsync(inserted, cancellationToken);

            var verifiedHistory = (await dbContext.Database
                    .GetAppliedMigrationsAsync(cancellationToken))
                .ToHashSet(StringComparer.Ordinal);
            if (inserted.Any(migrationId => !verifiedHistory.Contains(migrationId)))
            {
                throw new ReconciliationValidationException(
                    "Post-write migration-history verification failed.");
            }

            var postWriteInspection = await schemaInspector
                .InspectAsync(requestedMigrations, cancellationToken);
            if (!postWriteInspection.IsCompatible)
            {
                throw new ReconciliationValidationException(
                    "Post-write physical schema verification failed.");
            }

            await transaction.CommitAsync(cancellationToken);
            return new ReconciliationResult
            {
                Success = true,
                DryRun = false,
                Provider = schemaInspector.ProviderName,
                RequestedMigrations = requestedMigrations,
                MissingHistoryMigrations = Array.Empty<string>(),
                ReconciledMigrations = inserted,
                Differences = Array.Empty<SchemaDifference>(),
                VerificationLogs = postWriteInspection.VerificationLogs
                    .Append("Migration history verified after write; transaction committed.")
                    .ToArray()
            };
        }
        catch (Exception exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            return ReconciliationResult.Failed(
                "RECONCILIATION_ROLLED_BACK",
                $"Reconciliation transaction rolled back: {exception.Message}");
        }
    }

    private static string CreateConfirmationToken(
        string provider,
        IReadOnlyList<string> missingHistory,
        string schemaFingerprint)
    {
        var payload = string.Join(
            "\n",
            new[]
            {
                provider,
                string.Join(",", missingHistory),
                schemaFingerprint
            });
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(payload));
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    private static bool FixedTimeEquals(string left, string right)
    {
        var leftBytes = Encoding.UTF8.GetBytes(left);
        var rightBytes = Encoding.UTF8.GetBytes(right);
        return leftBytes.Length == rightBytes.Length
               && CryptographicOperations.FixedTimeEquals(leftBytes, rightBytes);
    }
}

public sealed class ReconciliationRequest
{
    public bool DryRun { get; init; } = true;
    public bool Apply { get; init; }
    public string ConfirmationToken { get; init; } = string.Empty;
    public IReadOnlyList<string> MigrationIds { get; init; } = Array.Empty<string>();
}

public sealed class ReconciliationResult
{
    public bool Success { get; set; }
    public bool DryRun { get; set; } = true;
    public string Provider { get; set; } = string.Empty;
    public string ErrorCode { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    public string ConfirmationToken { get; set; } = string.Empty;
    public IReadOnlyList<string> RequestedMigrations { get; set; } = Array.Empty<string>();
    public IReadOnlyList<string> MissingHistoryMigrations { get; set; } = Array.Empty<string>();
    public IReadOnlyList<string> ReconciledMigrations { get; set; } = Array.Empty<string>();
    public IReadOnlyList<SchemaDifference> Differences { get; set; } = Array.Empty<SchemaDifference>();
    public IReadOnlyList<string> VerificationLogs { get; set; } = Array.Empty<string>();

    public static ReconciliationResult Failed(string errorCode, string errorMessage)
    {
        return new ReconciliationResult
        {
            Success = false,
            ErrorCode = errorCode,
            ErrorMessage = errorMessage
        };
    }
}

public sealed record SchemaDifference(
    string Kind,
    string ObjectName,
    string Expected,
    string Actual);

public sealed class SchemaInspectionResult
{
    public bool IsCompatible => Differences.Count == 0;
    public string Fingerprint { get; init; } = string.Empty;
    public IReadOnlyList<SchemaDifference> Differences { get; init; } = Array.Empty<SchemaDifference>();
    public IReadOnlyList<string> VerificationLogs { get; init; } = Array.Empty<string>();
}

public interface IReconciliationSchemaInspector
{
    string ProviderName { get; }
    bool SupportsProvider { get; }
    Task<SchemaInspectionResult> InspectAsync(
        IReadOnlyList<string> migrationIds,
        CancellationToken cancellationToken = default);
}

public interface IReconciliationTransactionHook
{
    Task AfterHistoryInsertAsync(
        IReadOnlyList<string> insertedMigrations,
        CancellationToken cancellationToken = default);
}

public sealed class ReconciliationTransactionHook : IReconciliationTransactionHook
{
    public Task AfterHistoryInsertAsync(
        IReadOnlyList<string> insertedMigrations,
        CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}

public sealed class ReconciliationValidationException(string message) : Exception(message);
