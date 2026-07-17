using HclCsInstallerMVC.Application.Abstractions;
using HclCsInstallerMVC.Application.DTOs;
using HclCsInstallerMVC.Application.Exceptions;

namespace HclCsInstallerMVC.Application.Services;

public sealed class InstallerService : IInstallerService
{
    private readonly IDatabaseMigrationService _databaseMigrationService;
    private readonly IInstallationGateService _installationGateService;
    private readonly ISeedDataService _seedDataService;
    private readonly IInstallerStateStore _stateStore;

    public InstallerService(
        IDatabaseMigrationService databaseMigrationService,
        ISeedDataService seedDataService,
        IInstallerStateStore stateStore,
        IInstallationGateService installationGateService)
    {
        _databaseMigrationService = databaseMigrationService;
        _seedDataService = seedDataService;
        _stateStore = stateStore;
        _installationGateService = installationGateService;
    }

    public Task<bool> IsInstallationCompletedAsync(CancellationToken cancellationToken)
    {
        return _installationGateService.IsInstallationCompletedAsync(cancellationToken);
    }

    public Task<InstallerSessionState> GetStateAsync(CancellationToken cancellationToken)
    {
        return _stateStore.GetAsync(cancellationToken);
    }

    public async Task SaveDatabaseConfigurationAsync(DatabaseConfigurationDto configuration,
        CancellationToken cancellationToken)
    {
        await EnsureInstallationIsAllowedAsync(cancellationToken);

        var state = await _stateStore.GetAsync(cancellationToken);
        state.DatabaseConfiguration = configuration;
        state.DatabaseConnectionValidated = false;
        state.MigrationCompleted = false;
        state.SeedResult = null;

        await _stateStore.SaveAsync(state, cancellationToken);
    }

    public async Task<ConnectionValidationResultDto> ValidateDatabaseConnectionAsync(
        CancellationToken cancellationToken)
    {
        await EnsureInstallationIsAllowedAsync(cancellationToken);

        var state = await _stateStore.GetAsync(cancellationToken);
        if (state.DatabaseConfiguration is null)
            throw new InstallerWorkflowException("Database configuration is required before validating connection.");

        var result =
            await _databaseMigrationService.ValidateConnectionAsync(state.DatabaseConfiguration, cancellationToken);
        state.DatabaseConnectionValidated = result.Succeeded;

        if (!result.Succeeded)
        {
            state.MigrationCompleted = false;
            state.SeedResult = null;
        }

        await _stateStore.SaveAsync(state, cancellationToken);
        return result;
    }

    public async Task<MigrationExecutionResultDto> RunMigrationsAsync(CancellationToken cancellationToken)
    {
        await EnsureInstallationIsAllowedAsync(cancellationToken);

        var state = await _stateStore.GetAsync(cancellationToken);
        if (state.DatabaseConfiguration is null)
            throw new InstallerWorkflowException("Database configuration is required before running migrations.");

        if (!state.DatabaseConnectionValidated)
            throw new InstallerWorkflowException("Database connection must be validated before running migrations.");

        var result = await _databaseMigrationService.RunMigrationsAsync(state.DatabaseConfiguration, cancellationToken);
        state.MigrationCompleted = result.Succeeded;

        if (!result.Succeeded) state.SeedResult = null;

        await _stateStore.SaveAsync(state, cancellationToken);
        return result;
    }

    public async Task<SeedExecutionResultDto> SeedInitialDataAsync(SeedConfigurationDto configuration,
        CancellationToken cancellationToken)
    {
        await EnsureInstallationIsAllowedAsync(cancellationToken);

        var state = await _stateStore.GetAsync(cancellationToken);
        if (state.DatabaseConfiguration is null)
            throw new InstallerWorkflowException("Database configuration is required before seeding data.");

        if (!state.MigrationCompleted)
            throw new InstallerWorkflowException("Migrations must complete before seeding data.");

        var result = await _seedDataService.SeedAsync(state.DatabaseConfiguration, configuration, cancellationToken);
        state.SeedResult = result;

        if (result.Succeeded)
            await _installationGateService.MarkInstallationCompletedAsync(
                new InstallationCompletionMetadataDto
                {
                    CompletedOnUtc = DateTimeOffset.UtcNow,
                    DatabaseProvider = state.DatabaseConfiguration.Provider.ToString(),
                    ClientId = result.GeneratedClientId,
                    ClientSecret = result.GeneratedClientSecret
                },
                cancellationToken);

        await _stateStore.SaveAsync(state, cancellationToken);
        return result;
    }

    public async Task MarkInstallationCompletedWithoutSeedAsync(CancellationToken cancellationToken)
    {
        await EnsureInstallationIsAllowedAsync(cancellationToken);

        var state = await _stateStore.GetAsync(cancellationToken);
        if (state.DatabaseConfiguration is null)
            throw new InstallerWorkflowException("Database configuration is required.");
        if (!state.MigrationCompleted)
            throw new InstallerWorkflowException("Migrations must complete before finishing without seed.");

        await _installationGateService.MarkInstallationCompletedAsync(
            new InstallationCompletionMetadataDto
            {
                CompletedOnUtc = DateTimeOffset.UtcNow,
                DatabaseProvider = state.DatabaseConfiguration.Provider.ToString(),
                ClientId = null,
                ClientSecret = null
            },
            cancellationToken);
    }

    public Task<InstallationCompletionMetadataDto?> GetInstallationCompletionMetadataAsync(
        CancellationToken cancellationToken)
    {
        return _installationGateService.GetCompletionMetadataAsync(cancellationToken);
    }

    private async Task EnsureInstallationIsAllowedAsync(CancellationToken cancellationToken)
    {
        if (await _installationGateService.IsInstallationCompletedAsync(cancellationToken))
            throw new InstallationAlreadyCompletedException();
    }
}
