using ZentraInstallerMVC.Application.DTOs;

namespace ZentraInstallerMVC.Application.Abstractions;

public interface IInstallationGateService
{
    Task<bool> IsInstallationCompletedAsync(CancellationToken cancellationToken);

    Task<InstallationCompletionMetadataDto?> GetCompletionMetadataAsync(CancellationToken cancellationToken);

    Task MarkInstallationCompletedAsync(InstallationCompletionMetadataDto metadata,
        CancellationToken cancellationToken);
}
