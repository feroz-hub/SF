using HclCsInstallerMVC.Application.DTOs;

namespace HclCsInstallerMVC.Application.Abstractions;

public interface ISeedDataService
{
    Task<SeedExecutionResultDto> SeedAsync(
        DatabaseConfigurationDto databaseConfiguration,
        SeedConfigurationDto seedConfiguration,
        CancellationToken cancellationToken);
}
