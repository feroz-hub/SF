/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using HCL.CS.Domain.Constants;

namespace HCL.CS.ProxyService.Extension;

internal static class ScopesExtension
{
    internal static List<string> ExpandPermissions(this List<string> permissions)
    {
        var newPermissionsList = new List<string>();
        if (permissions != null && permissions.Any())
            foreach (var scope in permissions)
                if (scope.Contains(PermissionConstants.Manage))
                {
                    newPermissionsList.Add(scope.Replace(PermissionConstants.Manage, PermissionConstants.Read));
                    newPermissionsList.Add(scope.Replace(PermissionConstants.Manage, PermissionConstants.Write));
                    newPermissionsList.Add(scope.Replace(PermissionConstants.Manage, PermissionConstants.Delete));
                }
                else
                {
                    newPermissionsList.Add(scope);
                }

        return newPermissionsList.Distinct().ToList();
    }
}
