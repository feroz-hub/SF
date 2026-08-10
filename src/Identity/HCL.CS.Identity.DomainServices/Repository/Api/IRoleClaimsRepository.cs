/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using System.Security.Claims;
using System.Threading;
using HCL.CS.Domain;
using HCL.CS.Domain.Entities.Api;
using HCL.CS.Domain.Models.Api;

namespace HCL.CS.DomainServices.Repository.Api;

public interface IRoleClaimsRepository
{
    Task InsertAsync(RoleClaims entity);

    Task UpdateAsync(RoleClaims entity);

    Task DeleteAsync(int roleClaimId, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid roleId, CancellationToken cancellationToken = default);

    Task DeleteAsync(RoleClaims entity);

    Task<IList<RoleClaims>> GetClaimsAsync(Guid roleId, CancellationToken cancellationToken = default);

    Task<int> FindIdByClaimAsync(Guid roleId, Claim claim, CancellationToken cancellationToken = default);

    Task<RoleClaims> FindRoleByClaimAsync(Guid roleId, Claim claim, CancellationToken cancellationToken = default);

    Task<RoleClaims> FindRoleClaimByIdAsync(int roleClaimId, CancellationToken cancellationToken = default);

    Task<IList<UserRoleClaimTypesModel>> GetRolesAndClaimsForUser(Guid userId, CancellationToken cancellationToken = default);

    Task<FrameworkResult> SaveChangesAsync(CancellationToken cancellationToken = default);
}
