namespace HclCsInstallerMVC.Application.DTOs;

public sealed class ClientConfigurationDto
{
    public string ClientName { get; init; } = string.Empty;

    public string ClientUri { get; init; } = string.Empty;

    public IReadOnlyCollection<string> GrantTypes { get; init; } = Array.Empty<string>();

    public IReadOnlyCollection<string> ResponseTypes { get; init; } = Array.Empty<string>();

    public bool UseDefaultScopes { get; init; }

    public string AllowedScopes { get; init; } = string.Empty;

    public IReadOnlyCollection<string> RedirectUris { get; init; } = Array.Empty<string>();

    public IReadOnlyCollection<string> PostLogoutRedirectUris { get; init; } = Array.Empty<string>();

    public string? FrontChannelLogoutUri { get; init; }

    public string? BackChannelLogoutUri { get; init; }
}
