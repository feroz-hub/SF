using Zentra.Domain.Constants;

namespace Zentra.ProxyService.Extension;

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
