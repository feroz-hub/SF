namespace HCL.CS.DemoClientMvc.Options;

public sealed class OAuthClientOptions
{
    public const string SectionName = "OAuth";

    public string Authority { get; set; } = "https://localhost:5001";

    public string? MetadataAddress { get; set; }

    public string ClientId { get; set; } = string.Empty;

    public string ClientSecret { get; set; } = string.Empty;

    public string CallbackPath { get; set; } = "/signin-oidc";

    public string SignedOutCallbackPath { get; set; } = "/signout-callback-oidc";

    public bool RequireHttpsMetadata { get; set; } = true;

    public string TokenEndpoint { get; set; } = "https://localhost:5001/security/token";

    public string RevocationEndpoint { get; set; } = "https://localhost:5001/security/revocation";

    public string ResourceApiBaseUrl { get; set; } = "https://localhost:5001";

    public int ApiTimeoutSeconds { get; set; } = 30;

    public int RefreshBeforeExpirySeconds { get; set; } = 120;

    public IList<string> Scopes { get; set; } = new List<string>
    {
        "openid",
        "profile",
        "email",
        "offline_access"
    };

    // Optional: dedicated client credentials for ROP flow demo only.
    public string? ResourceOwnerClientId { get; set; }

    public string? ResourceOwnerClientSecret { get; set; }

    public IList<string> ResourceOwnerScopes { get; set; } = new List<string>();
}
