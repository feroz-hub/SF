using ZentraInstallerMVC.Application.DTOs;

namespace ZentraInstallerMVC.ViewModels;

public sealed class SetupConnectionViewModel
{
    public DatabaseProviderType? Provider { get; set; }

    public string ConnectionString { get; set; } = string.Empty;

    public bool IsRunningInContainer { get; set; }
}
