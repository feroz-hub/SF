using System.Security.Claims;

namespace HCL.CS.DemoServerApp.Services.ExternalAuth;

public sealed class GoogleExternalAuthProvider : IExternalAuthProvider
{
    public const string Scheme = "GoogleOidc";

    public const string ProviderName = "Google";

    public string Provider => ProviderName;

    public bool CanHandle(string provider)
    {
        return string.Equals(provider, ProviderName, StringComparison.OrdinalIgnoreCase);
    }

    public ExternalIdentityPayload ParseIdentity(ClaimsPrincipal principal)
    {
        var subject = principal.FindFirstValue("sub")
                      ?? principal.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? string.Empty;

        var issuer = principal.FindFirstValue("iss") ?? string.Empty;
        if (string.IsNullOrWhiteSpace(issuer)) issuer = "https://accounts.google.com";

        var email = principal.FindFirstValue(ClaimTypes.Email)
                    ?? principal.FindFirstValue("email")
                    ?? string.Empty;

        var displayName = principal.FindFirstValue(ClaimTypes.Name)
                          ?? principal.FindFirstValue("name")
                          ?? email;

        var emailVerifiedRaw = principal.FindFirstValue("email_verified");
        var emailVerified = string.Equals(emailVerifiedRaw, "true", StringComparison.OrdinalIgnoreCase)
                            || string.Equals(emailVerifiedRaw, "1", StringComparison.OrdinalIgnoreCase);

        return new ExternalIdentityPayload
        {
            Issuer = issuer.Trim(),
            Subject = subject.Trim(),
            Email = email.Trim(),
            EmailVerified = emailVerified,
            DisplayName = displayName.Trim()
        };
    }
}
