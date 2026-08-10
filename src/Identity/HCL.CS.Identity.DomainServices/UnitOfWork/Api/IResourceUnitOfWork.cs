/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using System.Threading;
using HCL.CS.Domain;
using HCL.CS.Domain.Entities.Api;

namespace HCL.CS.DomainServices.UnitOfWork.Api;

public interface IResourceUnitOfWork
{
    IRepository<ApiResources> ApiResourcesRepository { get; }
    IRepository<ApiResourceClaims> ApiResourceClaimsRepository { get; }
    IRepository<ApiScopes> ApiScopesRepository { get; }
    IRepository<ApiScopeClaims> ApiScopeClaimsRepository { get; }
    IRepository<IdentityResources> IdentityResourcesRepository { get; }
    IRepository<IdentityClaims> IdentityClaimsRepository { get; }
    Task<FrameworkResult> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<FrameworkResult> SaveChangesWithHardDeleteAsync(CancellationToken cancellationToken = default);
}
