/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using System.Threading;
using HCL.CS.Domain;
using HCL.CS.DomainServices.Repository.Api;

namespace HCL.CS.DomainServices.UnitOfWork.Api;

public interface IRoleManagementUnitOfWork
{
    IRoleRepository RoleRepository { get; }
    IRoleClaimsRepository RoleClaimsRepository { get; }
    Task SetAddedStatusAsync<T>(T entity);
    Task SetModifiedStatusAsync<T>(T entity, string concurrencyStamp);
    Task<FrameworkResult> SaveChangesAsync(CancellationToken cancellationToken = default);
}
