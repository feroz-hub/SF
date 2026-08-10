/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

namespace HclCsInstallerMVC.ViewModels;

public sealed class SeedStepViewModel
{
    public string ClientName { get; set; } = string.Empty;

    public string ClientUri { get; set; } = string.Empty;

    public bool UseAuthorizationCodeGrant { get; set; }

    public bool UseClientCredentialsGrant { get; set; }

    public bool UseHybridGrant { get; set; }

    public bool UseRefreshTokenGrant { get; set; }

    public bool UsePasswordGrant { get; set; }

    public bool UseCodeResponseType { get; set; }

    public bool UseIdTokenResponseType { get; set; }

    public bool UseTokenResponseType { get; set; }

    public bool UseDefaultScopes { get; set; } = true;

    public string AllowedScopes { get; set; } = string.Empty;

    public string RedirectUris { get; set; } = string.Empty;

    public string PostLogoutRedirectUris { get; set; } = string.Empty;

    public string? FrontChannelLogoutUri { get; set; }

    public string? BackChannelLogoutUri { get; set; }

    public string UserName { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string ConfirmPassword { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string? LastName { get; set; }

    public string Email { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string IdentityProvider { get; set; } = "Local";

    public string? ErrorMessage { get; set; }

    public bool IsCompleted { get; set; }

    public string? GeneratedClientId { get; set; }

    public string? GeneratedClientSecret { get; set; }
}
