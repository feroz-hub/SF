namespace ZentraInstallerMVC.ViewModels;

public sealed class MigrationViewModel
{
    public bool CanRun { get; set; }

    public bool IsCompleted { get; set; }

    public string? ErrorMessage { get; set; }
}
