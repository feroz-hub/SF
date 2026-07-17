using ZentraInstallerMVC.Application.DTOs;

namespace ZentraInstallerMVC.Application.Abstractions;

public interface IDatabaseMigrationService
{
    Task<ConnectionValidationResultDto> ValidateConnectionAsync(
        DatabaseConfigurationDto configuration,
        CancellationToken cancellationToken);

    Task<MigrationExecutionResultDto> RunMigrationsAsync(
        DatabaseConfigurationDto configuration,
        CancellationToken cancellationToken);
}
