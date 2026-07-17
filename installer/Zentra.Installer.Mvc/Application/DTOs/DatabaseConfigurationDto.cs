namespace ZentraInstallerMVC.Application.DTOs;

public sealed class DatabaseConfigurationDto
{
    public DatabaseProviderType Provider { get; init; }

    public string ConnectionString { get; init; } = string.Empty;
}
