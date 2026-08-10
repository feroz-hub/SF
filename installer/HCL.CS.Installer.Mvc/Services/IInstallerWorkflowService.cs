/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

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
