using HCL.CS.Domain.Enums;

namespace HclCsInstallerMVC.Application.DTOs;

public sealed class AdminUserConfigurationDto
{
    public string UserName { get; init; } = string.Empty;

    public string Password { get; init; } = string.Empty;

    public string FirstName { get; init; } = string.Empty;

    public string? LastName { get; init; }

    public string Email { get; init; } = string.Empty;

    public string PhoneNumber { get; init; } = string.Empty;

    public IdentityProvider IdentityProvider { get; init; }
}
