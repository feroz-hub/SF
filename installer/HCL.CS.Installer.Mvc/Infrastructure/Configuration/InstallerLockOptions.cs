namespace HclCsInstallerMVC.Infrastructure.Configuration;

public sealed class InstallerLockOptions
{
    public string MarkerFilePath { get; set; } = "App_Data/installer.lock.json";
}
