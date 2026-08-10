/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using HclCsInstallerMVC.Application.DTOs;

namespace HclCsInstallerMVC.Application.Abstractions;

public interface IInstallerService
{
    Task<bool> IsInstallationCompletedAsync(CancellationToken cancellationToken);

    Task<InstallerSessionState> GetStateAsync(CancellationToken cancellationToken);

    Task SaveDatabaseConfigurationAsync(DatabaseConfigurationDto configuration, CancellationToken cancellationToken);

    Task<ConnectionValidationResultDto> ValidateDatabaseConnectionAsync(CancellationToken cancellationToken);

    Task<MigrationExecutionResultDto> RunMigrationsAsync(CancellationToken cancellationToken);

    Task<SeedExecutionResultDto> SeedInitialDataAsync(SeedConfigurationDto configuration,
        CancellationToken cancellationToken);

    Task MarkInstallationCompletedWithoutSeedAsync(CancellationToken cancellationToken);

    Task<InstallationCompletionMetadataDto?>
        GetInstallationCompletionMetadataAsync(CancellationToken cancellationToken);
}
