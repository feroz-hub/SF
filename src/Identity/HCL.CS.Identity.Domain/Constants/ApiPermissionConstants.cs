/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

namespace HCL.CS.Domain.Constants;

public static class ApiPermissionConstants
{
    public const string BaseName = "hcl-cs.";

    public const string Anonymous = BaseName + "anonymous";

    public const string ApiResourceRead = BaseName + "apiresource.read";

    public const string ApiResourceWrite = BaseName + "apiresource.write";

    public const string ApiResourceDelete = BaseName + "apiresource.delete";

    public const string ApiResourceManage = BaseName + "apiresource.manage";

    // User Permission.

    public const string UserRead = BaseName + "user.read";

    public const string UserWrite = BaseName + "user.write";

    public const string UserDelete = BaseName + "user.delete";

    public const string UserManage = BaseName + "user.manage";

    // Identity Resource Permission Constants.

    public const string IdentityResourceRead = BaseName + "identityresource.read";

    public const string IdentityResourceWrite = BaseName + "identityresource.write";

    public const string IdentityResourceDelete = BaseName + "identityresource.delete";

    public const string IdentityResourceManage = BaseName + "identityresource.manage";

    // Role Permission Constants .

    public const string RoleRead = BaseName + "role.read";

    public const string RoleWrite = BaseName + "role.write";

    public const string RoleDelete = BaseName + "role.delete";

    public const string RoleManage = BaseName + "role.manage";

    public const string AdminRead = BaseName + "adminuser.read";

    public const string AdminWrite = BaseName + "adminuser.write";

    public const string AdminDelete = BaseName + "adminuser.delete";

    public const string AdminManage = BaseName + "adminuser.manage";

    // Audit Trail Permission Constants .

    public const string AuditRead = BaseName + "audittrail.read";

    public const string AuditManage = BaseName + "audittrail.manage";

    // Client Permission Constants .

    public const string ClientRead = BaseName + "client.read";

    public const string ClientWrite = BaseName + "client.write";

    public const string ClientDelete = BaseName + "client.delete";

    public const string ClientManage = BaseName + "client.manage";

    public const string SecurityTokenManage = BaseName + "securitytoken.manage";

    public const string SecurityTokenRead = BaseName + "securitytoken.read";

    // Notification Permission Constants.

    public const string NotificationRead = BaseName + "notification.read";

    public const string NotificationManage = BaseName + "notification.manage";

    // External Auth Permission Constants.

    public const string ExternalAuthRead = BaseName + "externalauth.read";

    public const string ExternalAuthManage = BaseName + "externalauth.manage";
}
