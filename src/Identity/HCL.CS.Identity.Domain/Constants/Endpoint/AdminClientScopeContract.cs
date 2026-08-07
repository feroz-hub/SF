/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace HCL.CS.Domain.Constants.Endpoint;

/// <summary>
/// Canonical OIDC and granular permission scopes for the Installer-created HCL.CS Admin client.
/// Keep the Admin application's mirrored contract protected by cross-repository drift tests.
/// </summary>
public static class AdminClientScopeContract
{
    public const string SpaceSeparated =
        "openid profile email phone offline_access " +
        "hcl-cs.apiresource.read hcl-cs.apiresource.write hcl-cs.apiresource.manage hcl-cs.apiresource.delete " +
        "hcl-cs.identityresource.read hcl-cs.identityresource.write hcl-cs.identityresource.manage hcl-cs.identityresource.delete " +
        "hcl-cs.client.read hcl-cs.client.write hcl-cs.client.manage hcl-cs.client.delete " +
        "hcl-cs.user.read hcl-cs.user.write hcl-cs.user.manage hcl-cs.user.delete " +
        "hcl-cs.role.read hcl-cs.role.write hcl-cs.role.manage hcl-cs.role.delete " +
        "hcl-cs.adminuser.read hcl-cs.adminuser.write hcl-cs.adminuser.manage hcl-cs.adminuser.delete " +
        "hcl-cs.securitytoken.read hcl-cs.securitytoken.manage";

    public static IReadOnlyList<string> AllScopes { get; } =
        SpaceSeparated.Split(' ', StringSplitOptions.RemoveEmptyEntries);

    public static IReadOnlyList<string> IdentityScopes { get; } =
        AllScopes.Where(scope => !scope.StartsWith("hcl-cs.", StringComparison.Ordinal)).ToArray();

    public static IReadOnlyList<string> PermissionScopes { get; } =
        AllScopes.Where(scope => scope.StartsWith("hcl-cs.", StringComparison.Ordinal)).ToArray();
}
