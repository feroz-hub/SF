/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using HCL.CS.Domain.Enums;
using static HCL.CS.Domain.Constants.Endpoint.OpenIdConstants;

namespace HCL.CS.Domain.Models.Endpoint;

public class ClientsModel : BaseModel
{
    public string ClientId { get; set; }

    public string ClientName { get; set; }

    public string ClientUri { get; set; }

    public DateTime ClientIdIssuedAt { get; set; }

    public DateTime ClientSecretExpiresAt { get; set; }

    public string ClientSecret { get; set; }

    public string LogoUri { get; set; }

    public string TermsOfServiceUri { get; set; }

    public string PolicyUri { get; set; }

    public int RefreshTokenExpiration { get; set; } = 86400;

    public int AccessTokenExpiration { get; set; } = 900;

    public int IdentityTokenExpiration { get; set; } = 3600;

    public int LogoutTokenExpiration { get; set; } = 1800;

    public int AuthorizationCodeExpiration { get; set; } = 600;

    public AccessTokenType AccessTokenType { get; set; } = AccessTokenType.JWT;

    public bool RequirePkce { get; set; }

    public bool IsPkceTextPlain { get; set; }

    public bool RequireClientSecret { get; set; } = true;

    public bool IsFirstPartyApp { get; set; } = true;

    public bool AllowOfflineAccess { get; set; }

    public bool AllowAccessTokensViaBrowser { get; set; }

    public ApplicationType ApplicationType { get; set; }

    public string AllowedSigningAlgorithm { get; set; } = Algorithms.RsaSha256;

    public bool FrontChannelLogoutSessionRequired { get; set; } = false;

    public string FrontChannelLogoutUri { get; set; }

    public bool BackChannelLogoutSessionRequired { get; set; }

    public string BackChannelLogoutUri { get; set; }

    public List<string> SupportedGrantTypes { get; set; }

    public List<string> SupportedResponseTypes { get; set; }

    public List<string> AllowedScopes { get; set; }

    public List<ClientRedirectUrisModel> RedirectUris { get; set; }

    public List<ClientPostLogoutRedirectUrisModel> PostLogoutRedirectUris { get; set; }

    /// <summary>Optional. When set, access tokens for this client use this value as the aud claim (e.g. orders.api, hcl-cs.api).</summary>
    public string PreferredAudience { get; set; }
}
