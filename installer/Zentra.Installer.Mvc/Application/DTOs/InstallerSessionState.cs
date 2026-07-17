namespace ZentraInstallerMVC.Application.DTOs;

public sealed class InstallerSessionState
{
    public DatabaseConfigurationDto? DatabaseConfiguration { get; set; }

    public bool DatabaseConnectionValidated { get; set; }

    public bool MigrationCompleted { get; set; }

    public SeedExecutionResultDto? SeedResult { get; set; }
}
