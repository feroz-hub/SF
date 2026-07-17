using System.Threading;
using HCL.CS.Domain;
using HCL.CS.Domain.Entities.Api;
using HCL.CS.DomainServices.Repository.Api;

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
    Task SetAddedStatusAsync<T>(T entity);
    Task SetConcurrencyOriginalValueAsync<T>(T entity, string concurrencyStamp);
    Task SetModifiedStatusAsync<T>(T entity, string concurrencyStamp);
    Task SetPropertyModifiedStatusAsync<T>(T entity, string property);
    Task<FrameworkResult> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<FrameworkResult> SaveChangesWithHardDeleteAsync(CancellationToken cancellationToken = default);
}
