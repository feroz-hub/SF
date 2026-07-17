using HclCsInstallerMVC.Application.DTOs;

namespace HclCsInstallerMVC.Application.Abstractions;

public interface IDatabaseProvisioner
{
    Task EnsureDatabaseExistsAsync(
        DatabaseConfigurationDto configuration,
        CancellationToken cancellationToken);
}
