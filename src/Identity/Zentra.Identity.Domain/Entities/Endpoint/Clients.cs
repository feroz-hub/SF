using Zentra.Domain.Enums;

namespace Zentra.Domain.Entities.Endpoint;

public class Clients : BaseEntity
{
    public string ClientId { get; set; }

    public string ClientName { get; set; }

    public string ClientUri { get; set; }

    public long ClientIdIssuedAt { get; set; }

    public long ClientSecretExpiresAt { get; set; }

    public string ClientSecret { get; set; }

    public string LogoUri { get; set; }

    public string TermsOfServiceUri { get; set; }

    public string PolicyUri { get; set; }

    public int RefreshTokenExpiration { get; set; }

    public int AccessTokenExpiration { get; set; }

    public int IdentityTokenExpiration { get; set; }

    public int LogoutTokenExpiration { get; set; }

    public int AuthorizationCodeExpiration { get; set; }

    public AccessTokenType AccessTokenType { get; set; }

    public bool RequirePkce { get; set; }

    public bool IsPkceTextPlain { get; set; }

    public bool RequireClientSecret { get; set; }

    public bool IsFirstPartyApp { get; set; }

    public bool AllowOfflineAccess { get; set; }

    public string AllowedScopes { get; set; }

    public bool AllowAccessTokensViaBrowser { get; set; }

    public List<ClientRedirectUris> RedirectUris { get; set; }

    public List<ClientPostLogoutRedirectUris> PostLogoutRedirectUris { get; set; }

    public ApplicationType ApplicationType { get; set; }

    public string AllowedSigningAlgorithm { get; set; }

    public string SupportedGrantTypes { get; set; }

    public string SupportedResponseTypes { get; set; }

    public bool FrontChannelLogoutSessionRequired { get; set; }

    public string FrontChannelLogoutUri { get; set; }

    public bool BackChannelLogoutSessionRequired { get; set; }

    public string BackChannelLogoutUri { get; set; }

    /// <summary>Optional. When set, access tokens for this client use this value as the aud claim (e.g. rentflow.api, zentra.api).</summary>
    public string PreferredAudience { get; set; }
}
