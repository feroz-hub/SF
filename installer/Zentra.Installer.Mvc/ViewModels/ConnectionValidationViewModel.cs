using ZentraInstallerMVC.Application.DTOs;

namespace ZentraInstallerMVC.ViewModels;

public sealed class ConnectionValidationViewModel
{
    public bool HasConfiguration { get; set; }

    public DatabaseProviderType? Provider { get; set; }

    public string ConnectionString { get; set; } = string.Empty;

    public bool IsRunningInContainer { get; set; }

    public bool IsValidated { get; set; }

    public bool IsSuccessful { get; set; }

    public string? ErrorMessage { get; set; }
}
