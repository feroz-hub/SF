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
using HCL.CS.DomainServices.Repository.Api;
using HCL.CS.DomainServices.UnitOfWork.Endpoint;

namespace HCL.CS.DomainServices.UnitOfWork.Api;

public interface IUserManagementUnitOfWork
{
    IUserRepository UserRepository { get; }
    IUserClaimRepository UserClaimRepository { get; }
    IUserTokenRepository UserTokenRepository { get; }
    IUserRoleRepository UserRoleRepository { get; }
    IRepository<UserSecurityQuestions> UserSecurityQuestionsRepository { get; }
    IRepository<Notification> NotificationRepository { get; }
    IRepository<PasswordHistory> PasswordHistoryRepository { get; }
    IRepository<SecurityQuestions> SecurityQuestionsRepository { get; }
    Task<IUnitOfWorkTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task SetAddedStatusAsync<T>(T entity);
    Task SetConcurrencyOriginalValueAsync<T>(T entity, string concurrencyStamp);
    Task SetModifiedStatusAsync<T>(T entity, string concurrencyStamp);
    Task SetPropertyModifiedStatusAsync<T>(T entity, string property);
    Task<FrameworkResult> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<FrameworkResult> SaveChangesWithHardDeleteAsync(CancellationToken cancellationToken = default);
}
