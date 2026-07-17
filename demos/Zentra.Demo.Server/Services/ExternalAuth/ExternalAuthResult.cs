namespace Zentra.DemoServerApp.Services.ExternalAuth;

public sealed class ExternalAuthResult
{
    public bool Succeeded { get; init; }

    public bool RequiresTwoFactor { get; init; }

    public string Message { get; init; } = string.Empty;

    public string RedirectUrl { get; init; } = string.Empty;
}
