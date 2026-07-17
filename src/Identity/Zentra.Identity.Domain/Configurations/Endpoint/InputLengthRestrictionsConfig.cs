namespace Zentra.Domain.Configurations.Endpoint;

public class InputLengthRestrictionsConfig
{
    private const int DefaultValue = 255;

    public int ClientId { get; set; } = DefaultValue;

    public int ClientSecret { get; set; } = DefaultValue;

    public int Scope { get; set; } = 700;

    public int RedirectUri { get; set; } = DefaultValue;

    public int Nonce { get; set; } = 20;

    // OIDC state can be large (for example ASP.NET Core DataProtection payloads).
    public int State { get; set; } = 2048;

    public int GrantType { get; set; } = DefaultValue;

    public int UserName { get; set; } = DefaultValue;

    public int Password { get; set; } = DefaultValue;

    public int AuthorizationCode { get; set; } = 512;

    public int RefreshToken { get; set; } = 512;

    public int Jwt { get; set; } = 51200;

    public int CodeChallengeMinLength { get; } = 43;

    public int CodeChallengeMaxLength { get; } = 128;

    public int CodeVerifierMinLength { get; } = 43;

    public int CodeVerifierMaxLength { get; } = 128;
}
