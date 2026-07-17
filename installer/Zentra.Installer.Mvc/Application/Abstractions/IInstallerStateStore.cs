using ZentraInstallerMVC.Application.DTOs;

namespace ZentraInstallerMVC.Application.Abstractions;

public interface IInstallerStateStore
{
    Task<InstallerSessionState> GetAsync(CancellationToken cancellationToken);

    Task SaveAsync(InstallerSessionState state, CancellationToken cancellationToken);

    Task ClearAsync(CancellationToken cancellationToken);
}
