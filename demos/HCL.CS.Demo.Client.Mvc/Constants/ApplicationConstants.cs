/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using static HCL.CS.Domain.Constants.Endpoint.OpenIdConstants;

namespace HCL.CS.DemoClientMvc.Constants;

public static class ApplicationConstants
{
    public const string BackChannelLogoutEvent = "http://schemas.openid.net/event/backchannel-logout";
    public const string SessionUserId = "userid";
    public const string ApplicationName = "HCL.CS Demo";
    public const int RenewRefreshTokenBeforeSeconds = 300;
    public static string userName = string.Empty;
    public static string ClientId => GetRequiredEnvironmentSetting("HCL_CS_OAUTH_CLIENT_ID");
    public static string ClientSecret => GetRequiredEnvironmentSetting("HCL_CS_OAUTH_CLIENT_SECRET");

    public static string AuthenticationServerBaseUrl =>
        Environment.GetEnvironmentVariable("HCL_CS_AUTHORITY") ?? "https://localhost:5001";

    public static string MetadataAddress => Environment.GetEnvironmentVariable("HCL_CS_METADATA_ADDRESS") ??
                                            $"{AuthenticationServerBaseUrl}/.well-known/openid-configuration";

    public static string ResourceServerBaseUrl => Environment.GetEnvironmentVariable("HCL_CS_RESOURCE_API_BASE_URL") ??
                                                  "https://localhost:5002";

    public static string RedirectUri => Environment.GetEnvironmentVariable("HCL_CS_REDIRECT_URI") ??
                                        "https://localhost:5001/index.html";

    public static string Scopes =>
        "openid email profile offline_access phone hcl-cs.apiresource hcl-cs.client hcl-cs.user hcl-cs.role hcl-cs.identityresource hcl-cs.adminuser hcl-cs.securitytoken";

    public static string ClientCredentialsScopes =>
        "hcl-cs.apiresource hcl-cs.client hcl-cs.user hcl-cs.role hcl-cs.identityresource hcl-cs.adminuser hcl-cs.securitytoken";

    public static string AuthorizeEndpoint => $"{AuthenticationServerBaseUrl}/security/authorize";
    public static string TokenEndpoint => $"{AuthenticationServerBaseUrl}/security/token";
    public static string RefreshTokenEndpoint => $"{AuthenticationServerBaseUrl}/security/token";
    public static string JwksEndpoint => $"{AuthenticationServerBaseUrl}/.well-known/openid-configuration/jwks";
    public static string IntrospectionEndpoint => $"{AuthenticationServerBaseUrl}/security/introspect";
    public static string UserInfoEndpoint => $"{AuthenticationServerBaseUrl}/security/userinfo";

    public static string IssuerUri =>
        Environment.GetEnvironmentVariable("HCL_CS_ISSUER") ?? AuthenticationServerBaseUrl;

    private static string GetRequiredEnvironmentSetting(string key)
    {
        var value = Environment.GetEnvironmentVariable(key);
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidOperationException($"Missing required environment variable '{key}'.");

        return value;
    }
}

public class AccessTokenFilters
{
    // filter for claims from an incoming access token (e.g. used at the user profile endpoint)
    public static readonly string[] ClaimsFilter =
    {
        ClaimTypes.AccessTokenHash,
        ClaimTypes.Audience,
        ClaimTypes.AuthorizedParty,
        ClaimTypes.AuthorizationCodeHash,
        ClaimTypes.ClientId,
        ClaimTypes.Expiration,
        ClaimTypes.IssuedAt,
        ClaimTypes.Issuer,
        ClaimTypes.JwtId,
        ClaimTypes.Nonce,
        ClaimTypes.NotBefore,
        ClaimTypes.ReferenceTokenId,
        ClaimTypes.SessionId,
        ClaimTypes.Scope
    };
}
