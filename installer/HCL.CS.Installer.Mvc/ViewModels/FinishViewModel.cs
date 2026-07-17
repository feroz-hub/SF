namespace HclCsInstallerMVC.ViewModels;

public sealed class FinishViewModel
{
    public bool InstallationCompleted { get; set; }

    public bool AlreadyInstalled { get; set; }

    public string? Message { get; set; }

    public string? DatabaseProvider { get; set; }

    public string? ClientId { get; set; }

    public string? ClientSecret { get; set; }

    public DateTimeOffset? CompletedOnUtc { get; set; }
}
