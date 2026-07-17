namespace Zentra.DemoServerApp.Services.ExternalAuth;

public sealed class ExternalIdentityPayload
{
    public string Issuer { get; init; } = string.Empty;

    public string Subject { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public bool EmailVerified { get; init; }

    public string DisplayName { get; init; } = string.Empty;
}
