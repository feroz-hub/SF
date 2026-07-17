using HclCsInstallerMVC.Application.DTOs;

namespace HclCsInstallerMVC.Application.Abstractions;

public interface IInstallerService
{
    Task<bool> IsInstallationCompletedAsync(CancellationToken cancellationToken);

    Task<InstallerSessionState> GetStateAsync(CancellationToken cancellationToken);

    Task SaveDatabaseConfigurationAsync(DatabaseConfigurationDto configuration, CancellationToken cancellationToken);

    Task<ConnectionValidationResultDto> ValidateDatabaseConnectionAsync(CancellationToken cancellationToken);

    Task<MigrationExecutionResultDto> RunMigrationsAsync(CancellationToken cancellationToken);

    Task<SeedExecutionResultDto> SeedInitialDataAsync(SeedConfigurationDto configuration,
        CancellationToken cancellationToken);

    Task MarkInstallationCompletedWithoutSeedAsync(CancellationToken cancellationToken);

    Task<InstallationCompletionMetadataDto?>
        GetInstallationCompletionMetadataAsync(CancellationToken cancellationToken);
}
