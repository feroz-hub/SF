using ZentraInstallerMVC.Application.DTOs;

namespace ZentraInstallerMVC.Application.Abstractions;

public interface IDatabaseProvisioner
{
    Task EnsureDatabaseExistsAsync(
        DatabaseConfigurationDto configuration,
        CancellationToken cancellationToken);
}
