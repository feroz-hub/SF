namespace HclCsInstallerMVC.Application.DTOs;

public sealed class SeedConfigurationDto
{
    public ClientConfigurationDto Client { get; init; } = new();

    public AdminUserConfigurationDto AdminUser { get; init; } = new();
}
