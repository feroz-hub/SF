using HCL.CS.Domain.Enums;
using HclCsInstallerMVC.Application.Abstractions;
using HclCsInstallerMVC.Application.DTOs;
using HclCsInstallerMVC.ViewModels;

namespace HclCsInstallerMVC.Services;

public sealed class InstallerWorkflowService : IInstallerWorkflowService
{
    private readonly IInstallerService _installerService;
    private static readonly bool IsRunningInContainer = string.Equals(
        Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"),
        "true",
        StringComparison.OrdinalIgnoreCase);

    public InstallerWorkflowService(IInstallerService installerService)
    {
        _installerService = installerService;
    }

    public Task<bool> IsInstallationCompletedAsync(CancellationToken cancellationToken)
    {
        return _installerService.IsInstallationCompletedAsync(cancellationToken);
    }

    public async Task<SetupProviderViewModel> GetProviderSelectionAsync(CancellationToken cancellationToken)
    {
        var state = await _installerService.GetStateAsync(cancellationToken);
        return new SetupProviderViewModel { Provider = state.DatabaseConfiguration?.Provider };
    }

    public async Task SaveProviderSelectionAsync(SetupProviderViewModel model, CancellationToken cancellationToken)
    {
        var state = await _installerService.GetStateAsync(cancellationToken);
        await _installerService.SaveDatabaseConfigurationAsync(
            new DatabaseConfigurationDto
            {
                Provider = model.Provider ?? DatabaseProviderType.SqlServer,
                ConnectionString = state.DatabaseConfiguration?.ConnectionString ?? string.Empty
            },
            cancellationToken);
    }

    public async Task<SetupConnectionViewModel> GetConnectionConfigurationAsync(CancellationToken cancellationToken)
    {
        var state = await _installerService.GetStateAsync(cancellationToken);
        var provider = state.DatabaseConfiguration?.Provider;
        var connectionString = state.DatabaseConfiguration?.ConnectionString ?? string.Empty;
        if (string.IsNullOrWhiteSpace(connectionString))
            connectionString = GetSuggestedConnectionString(provider, IsRunningInContainer);

        return new SetupConnectionViewModel
        {
            Provider = provider,
            ConnectionString = connectionString,
            IsRunningInContainer = IsRunningInContainer
        };
    }

    public Task SaveConnectionConfigurationAsync(SetupConnectionViewModel model, CancellationToken cancellationToken)
    {
        return _installerService.SaveDatabaseConfigurationAsync(
            new DatabaseConfigurationDto
            {
                Provider = model.Provider ?? DatabaseProviderType.SqlServer,
                ConnectionString = model.ConnectionString.Trim()
            },
            cancellationToken);
    }

    public async Task<ConnectionValidationViewModel> GetConnectionValidationAsync(CancellationToken cancellationToken)
    {
        var state = await _installerService.GetStateAsync(cancellationToken);

        return new ConnectionValidationViewModel
        {
            HasConfiguration = state.DatabaseConfiguration is not null,
            Provider = state.DatabaseConfiguration?.Provider,
            ConnectionString = state.DatabaseConfiguration?.ConnectionString ?? string.Empty,
            IsRunningInContainer = IsRunningInContainer,
            IsValidated = state.DatabaseConnectionValidated,
            IsSuccessful = state.DatabaseConnectionValidated
        };
    }

    public async Task<ConnectionValidationViewModel> ValidateConnectionAsync(CancellationToken cancellationToken)
    {
        var result = await _installerService.ValidateDatabaseConnectionAsync(cancellationToken);
        var viewModel = await GetConnectionValidationAsync(cancellationToken);

        viewModel.IsValidated = true;
        viewModel.IsSuccessful = result.Succeeded;
        viewModel.ErrorMessage = result.ErrorMessage;

        return viewModel;
    }

    public async Task<MigrationViewModel> GetMigrationViewModelAsync(CancellationToken cancellationToken)
    {
        var state = await _installerService.GetStateAsync(cancellationToken);

        return new MigrationViewModel
        {
            CanRun = state.DatabaseConnectionValidated,
            IsCompleted = state.MigrationCompleted
        };
    }

    public async Task<MigrationViewModel> RunMigrationAsync(CancellationToken cancellationToken)
    {
        var result = await _installerService.RunMigrationsAsync(cancellationToken);
        var model = await GetMigrationViewModelAsync(cancellationToken);
        model.ErrorMessage = result.ErrorMessage;
        model.IsCompleted = result.Succeeded;

        return model;
    }

    public async Task<SeedStepViewModel> GetSeedViewModelAsync(CancellationToken cancellationToken)
    {
        var state = await _installerService.GetStateAsync(cancellationToken);

        return new SeedStepViewModel
        {
            UseAuthorizationCodeGrant = true,
            UseCodeResponseType = true,
            UseDefaultScopes = true,
            IsCompleted = state.SeedResult?.Succeeded == true,
            GeneratedClientId = state.SeedResult?.GeneratedClientId,
            GeneratedClientSecret = state.SeedResult?.GeneratedClientSecret,
            ErrorMessage = state.SeedResult?.ErrorMessage
        };
    }

    public async Task<SeedStepViewModel> ExecuteSeedAsync(SeedStepViewModel model, CancellationToken cancellationToken)
    {
        var result = await _installerService.SeedInitialDataAsync(MapSeedConfiguration(model), cancellationToken);

        model.IsCompleted = result.Succeeded;
        model.GeneratedClientId = result.GeneratedClientId;
        model.GeneratedClientSecret = result.GeneratedClientSecret;
        model.ErrorMessage = result.ErrorMessage;

        return model;
    }

    public Task SkipSeedAsync(CancellationToken cancellationToken)
    {
        return _installerService.MarkInstallationCompletedWithoutSeedAsync(cancellationToken);
    }

    public async Task<FinishViewModel> GetCompletionViewModelAsync(CancellationToken cancellationToken)
    {
        var isCompleted = await _installerService.IsInstallationCompletedAsync(cancellationToken);
        var metadata = await _installerService.GetInstallationCompletionMetadataAsync(cancellationToken);

        return new FinishViewModel
        {
            InstallationCompleted = isCompleted,
            AlreadyInstalled = isCompleted,
            Message = isCompleted
                ? "HCL.CS installation completed and locked against reinstallation."
                : "Installation is not completed yet.",
            DatabaseProvider = metadata?.DatabaseProvider,
            ClientId = metadata?.ClientId,
            ClientSecret = metadata?.ClientSecret,
            CompletedOnUtc = metadata?.CompletedOnUtc
        };
    }

    private static SeedConfigurationDto MapSeedConfiguration(SeedStepViewModel model)
    {
        return new SeedConfigurationDto
        {
            Client = new ClientConfigurationDto
            {
                ClientName = model.ClientName.Trim(),
                ClientUri = model.ClientUri.Trim(),
                GrantTypes = BuildGrantTypes(model),
                ResponseTypes = BuildResponseTypes(model),
                UseDefaultScopes = model.UseDefaultScopes,
                AllowedScopes = model.AllowedScopes.Trim(),
                RedirectUris = SplitLines(model.RedirectUris),
                PostLogoutRedirectUris = SplitLines(model.PostLogoutRedirectUris),
                FrontChannelLogoutUri = model.FrontChannelLogoutUri,
                BackChannelLogoutUri = model.BackChannelLogoutUri
            },
            AdminUser = new AdminUserConfigurationDto
            {
                UserName = model.UserName.Trim(),
                Password = model.Password,
                FirstName = model.FirstName.Trim(),
                LastName = model.LastName,
                Email = model.Email.Trim(),
                PhoneNumber = model.PhoneNumber.Trim(),
                IdentityProvider = model.IdentityProvider.Equals("Ldap", StringComparison.OrdinalIgnoreCase)
                    ? IdentityProvider.Ldap
                    : IdentityProvider.Local
            }
        };
    }

    private static IReadOnlyCollection<string> BuildGrantTypes(SeedStepViewModel model)
    {
        var grantTypes = new List<string>();

        if (model.UseAuthorizationCodeGrant) grantTypes.Add("authorization_code");

        if (model.UseClientCredentialsGrant) grantTypes.Add("client_credentials");

        if (model.UseRefreshTokenGrant) grantTypes.Add("refresh_token");

        if (model.UsePasswordGrant) grantTypes.Add("password");

        return grantTypes;
    }

    private static IReadOnlyCollection<string> BuildResponseTypes(SeedStepViewModel model)
    {
        var responseTypes = new List<string>();

        if (model.UseCodeResponseType) responseTypes.Add("code");

        return responseTypes;
    }

    private static IReadOnlyCollection<string> SplitLines(string values)
    {
        if (string.IsNullOrWhiteSpace(values)) return Array.Empty<string>();

        return values
            .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static string GetSuggestedConnectionString(DatabaseProviderType? provider, bool isRunningInContainer)
    {
        return provider switch
        {
            DatabaseProviderType.SqlServer => "Server=localhost;Database=hclCs_identity;User Id=sa;Password=<strong-password>;Encrypt=True;TrustServerCertificate=False;",
            DatabaseProviderType.MySql => "Server=localhost;Port=3306;Database=hclCs_identity;Uid=root;Pwd=<strong-password>;SslMode=Preferred;",
            DatabaseProviderType.PostgreSql when isRunningInContainer => "Host=postgres;Port=5432;Database=hcl-cs;Username=hcl-cs;Password=hcl-cs;",
            DatabaseProviderType.PostgreSql => "Host=localhost;Port=55433;Database=hcl-cs;Username=hcl-cs;Password=hcl-cs;",
            DatabaseProviderType.Sqlite => "Data Source=.data/hclCs_identity.db;Mode=ReadWriteCreate;Cache=Shared;",
            _ => string.Empty
        };
    }
}
