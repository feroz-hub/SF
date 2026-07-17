using System.ComponentModel.DataAnnotations;

namespace HCL.CS.DemoServerApp.Options;

public sealed class GoogleOidcOptions
{
    public const string SectionName = "Authentication:Google";

    public bool Enabled { get; set; }

    [Required]
    public string ClientId { get; set; } = string.Empty;

    [Required]
    public string ClientSecret { get; set; } = string.Empty;

    [Required]
    public string Authority { get; set; } = "https://accounts.google.com";

    [Required]
    public string MetadataAddress { get; set; } = "https://accounts.google.com/.well-known/openid-configuration";

    [Required]
    public string CallbackPath { get; set; } = "/auth/external/google/signin-callback";

    public string[] AllowedRedirectHosts { get; set; } = Array.Empty<string>();
}
