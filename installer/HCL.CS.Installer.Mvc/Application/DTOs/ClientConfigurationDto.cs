/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

namespace HclCsInstallerMVC.Application.DTOs;

public sealed class ClientConfigurationDto
{
    public string ClientName { get; init; } = string.Empty;

    public string ClientUri { get; init; } = string.Empty;

    public IReadOnlyCollection<string> GrantTypes { get; init; } = Array.Empty<string>();

    public IReadOnlyCollection<string> ResponseTypes { get; init; } = Array.Empty<string>();

    public bool UseDefaultScopes { get; init; }

    public string AllowedScopes { get; init; } = string.Empty;

    public IReadOnlyCollection<string> RedirectUris { get; init; } = Array.Empty<string>();

    public IReadOnlyCollection<string> PostLogoutRedirectUris { get; init; } = Array.Empty<string>();

    public string? FrontChannelLogoutUri { get; init; }

    public string? BackChannelLogoutUri { get; init; }
}
