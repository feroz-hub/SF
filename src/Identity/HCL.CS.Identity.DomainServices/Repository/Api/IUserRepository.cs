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

namespace HCL.CS.DomainServices.Repository.Api;

public interface IUserRepository
{
    Task DeleteAsync(Users entity);
    Task UpdateAsync(Users entity, string[] affectedProperties);
    Task<IList<Users>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken = default);
    Task<Users?> FindByUserNameIncludingDeletedAsync(string userName, CancellationToken cancellationToken = default);
    Task<IList<Users>> FindByDirectoryImmutableIdAsync(
        string directoryImmutableId,
        CancellationToken cancellationToken = default);
    Task<IList<Users>> FindByNormalizedEmailAsync(
        string normalizedEmail,
        CancellationToken cancellationToken = default);
    Task SetAddedStatusAsync<T>(T entity);
    Task SetModifiedStatusAsync<T>(T entity, string concurrencyStamp);
    Task<FrameworkResult> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task EnableIdentityAutoSaveChanges();
    Task DisableIdentityAutoSaveChanges();
    Task<IList<Users>> GetAllUsersAsync(CancellationToken cancellationToken = default);
}
