using HclCsInstallerMVC.Application.DTOs;

namespace HclCsInstallerMVC.Application.Abstractions;

public interface IDatabaseMigrationService
{
    Task<ConnectionValidationResultDto> ValidateConnectionAsync(
        DatabaseConfigurationDto configuration,
        CancellationToken cancellationToken);

    Task<MigrationExecutionResultDto> RunMigrationsAsync(
        DatabaseConfigurationDto configuration,
        CancellationToken cancellationToken);
}
