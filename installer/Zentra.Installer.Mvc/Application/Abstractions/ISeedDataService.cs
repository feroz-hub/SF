using ZentraInstallerMVC.Application.DTOs;

namespace ZentraInstallerMVC.Application.Abstractions;

public interface ISeedDataService
{
    Task<SeedExecutionResultDto> SeedAsync(
        DatabaseConfigurationDto databaseConfiguration,
        SeedConfigurationDto seedConfiguration,
        CancellationToken cancellationToken);
}
