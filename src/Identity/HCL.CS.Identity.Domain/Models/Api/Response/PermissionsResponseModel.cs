/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

namespace HCL.CS.Domain.Models.Api.Response;

public class UserPermissionsResponseModel
{
    public virtual Guid UserId { get; set; }

    public virtual IList<UserRoleClaimsModel> RolePermissions { get; set; }
}

public class UserRoleClaimsModel
{
    public virtual Guid RoleId { get; set; }

    public virtual string RoleName { get; set; }

    public virtual IList<RoleClaimModel> Claims { get; set; }
}
