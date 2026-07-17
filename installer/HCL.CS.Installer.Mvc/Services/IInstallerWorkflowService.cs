using HclCsInstallerMVC.ViewModels;

namespace HclCsInstallerMVC.Services;

public interface IInstallerWorkflowService
{
    Task<bool> IsInstallationCompletedAsync(CancellationToken cancellationToken);

    Task<SetupProviderViewModel> GetProviderSelectionAsync(CancellationToken cancellationToken);

    Task SaveProviderSelectionAsync(SetupProviderViewModel model, CancellationToken cancellationToken);

    Task<SetupConnectionViewModel> GetConnectionConfigurationAsync(CancellationToken cancellationToken);

    Task SaveConnectionConfigurationAsync(SetupConnectionViewModel model, CancellationToken cancellationToken);

    Task<ConnectionValidationViewModel> GetConnectionValidationAsync(CancellationToken cancellationToken);

    Task<ConnectionValidationViewModel> ValidateConnectionAsync(CancellationToken cancellationToken);

    Task<MigrationViewModel> GetMigrationViewModelAsync(CancellationToken cancellationToken);

    Task<MigrationViewModel> RunMigrationAsync(CancellationToken cancellationToken);

    Task<SeedStepViewModel> GetSeedViewModelAsync(CancellationToken cancellationToken);

    Task<SeedStepViewModel> ExecuteSeedAsync(SeedStepViewModel model, CancellationToken cancellationToken);

    Task SkipSeedAsync(CancellationToken cancellationToken);

    Task<FinishViewModel> GetCompletionViewModelAsync(CancellationToken cancellationToken);
}
