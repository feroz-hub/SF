/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using HCL.CS.Domain;
using HCL.CS.Domain.Models.Api;

namespace HCL.CS.Service.Interfaces.Interfaces.Api;

public interface IRoleService
{
    Task<FrameworkResult> CreateRoleAsync(RoleModel roleModel);

    Task<FrameworkResult> UpdateRoleAsync(RoleModel roleModel);

    Task<FrameworkResult> DeleteRoleAsync(Guid roleId);

    Task<FrameworkResult> DeleteRoleAsync(string roleName);

    Task<RoleModel> GetRoleAsync(Guid roleId);

    Task<RoleModel> GetRoleAsync(string roleName);

    Task<IList<RoleModel>> GetAllRolesAsync();

    Task<FrameworkResult> AddRoleClaimAsync(RoleClaimModel roleClaimModel);

    Task<FrameworkResult> AddRoleClaimsAsync(IList<RoleClaimModel> roleClaimsModel);

    Task<IList<UserRoleClaimTypesModel>> GetRolesAndClaimsForUser(Guid userId);

    Task<FrameworkResult> RemoveRoleClaimAsync(int roleClaimId);

    Task<FrameworkResult> RemoveRoleClaimAsync(RoleClaimModel roleClaimModel);

    Task<FrameworkResult> RemoveRoleClaimsAsync(IList<RoleClaimModel> roleClaimsModel);

    Task<IList<RoleClaimModel>> GetRoleClaimAsync(RoleModel roleModel);
}
