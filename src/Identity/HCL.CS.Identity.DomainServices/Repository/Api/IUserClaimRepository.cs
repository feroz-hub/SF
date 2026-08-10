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

namespace HCL.CS.DomainServices.Repository.Api;

public interface IUserClaimRepository
{
    Task InsertAsync(UserClaims entity);
    Task UpdateAsync(UserClaims entity);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task DeleteAsync(IList<UserClaims> entityList);
    Task<UserClaims> FindIdByClaimAsync(Guid userId, Claim claim, bool isAdminClaim = false, CancellationToken cancellationToken = default);

    Task<UserClaims> FindUserClaimByIdAsync(int userClaimId, CancellationToken cancellationToken = default);

    Task<IList<UserClaims>> GetClaimsAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<IList<UserClaims>> GetAdminUserClaimsAsync(Guid userId, bool getOnlyAdminClaim = true, CancellationToken cancellationToken = default);

    Task<FrameworkResult> SaveChangesAsync(CancellationToken cancellationToken = default);
}
