using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Zentra.Domain;
using Zentra.Domain.Entities.Api;
using Zentra.DomainServices;
using Zentra.DomainServices.Repository.Api;
using Zentra.DomainServices.UnitOfWork.Api;
using Zentra.Infrastructure.Data.Repository.Api;

namespace Zentra.Infrastructure.Data.UnitOfWork.Api;

internal class UserManagementUnitOfWork : BaseDispose, IUserManagementUnitOfWork
{
    private readonly IApplicationDbContext context;

    private readonly
        UserStore<Users, Roles, ApplicationDbContext, Guid, UserClaims, UserRoles, UserLogins, UserTokens, RoleClaims>
        userStoreWrapper;

    private IRepository<Notification> notificationRepository;
    private IRepository<PasswordHistory> passwordHistoryRepository;
    private IRepository<SecurityQuestions> securityQuestionsRepository;
    private IUserClaimRepository userClaimRepository;
    private IUserRepository userRepository;
    private IUserRoleRepository userRoleRepository;
    private IRepository<UserSecurityQuestions> userSecurityQuestionsRepository;
    private IUserTokenRepository userTokenRepository;


    public UserManagementUnitOfWork(IApplicationDbContext context, IUserStore<Users> userStoreWrapper)
    {
        this.context = context;
        // We can only have control on user store by the below approach. Tried with interface, extension methods which not working in autosavechanges.
        this.userStoreWrapper =
            userStoreWrapper as UserStore<Users, Roles, ApplicationDbContext, Guid, UserClaims, UserRoles, UserLogins,
                UserTokens, RoleClaims>;
    }

    public IUserRepository UserRepository
    {
        get
        {
            if (userRepository != null) return userRepository;
            userRepository = new UserRepository(context, userStoreWrapper);
            return userRepository;
        }
    }

    public IUserClaimRepository UserClaimRepository
    {
        get
        {
            if (userClaimRepository != null) return userClaimRepository;
            userClaimRepository = new UserClaimRepository(context);
            return userClaimRepository;
        }
    }

    public IUserTokenRepository UserTokenRepository
    {
        get
        {
            if (userTokenRepository != null) return userTokenRepository;
            userTokenRepository = new UserTokenRepository(context);
            return userTokenRepository;
        }
    }

    public IUserRoleRepository UserRoleRepository
    {
        get
        {
            if (userRoleRepository != null) return userRoleRepository;
            userRoleRepository = new UserRoleRepository(context);
            return userRoleRepository;
        }
    }

    public IRepository<UserSecurityQuestions> UserSecurityQuestionsRepository
    {
        get
        {
            if (userSecurityQuestionsRepository != null) return userSecurityQuestionsRepository;
            userSecurityQuestionsRepository = new BaseRepository<UserSecurityQuestions>(context);
            return userSecurityQuestionsRepository;
        }
    }

    public IRepository<Notification> NotificationRepository
    {
        get
        {
            if (notificationRepository != null) return notificationRepository;
            notificationRepository = new BaseRepository<Notification>(context);
            return notificationRepository;
        }
    }

    public IRepository<PasswordHistory> PasswordHistoryRepository
    {
        get
        {
            if (passwordHistoryRepository != null) return passwordHistoryRepository;
            passwordHistoryRepository = new BaseRepository<PasswordHistory>(context);
            return passwordHistoryRepository;
        }
    }

    public IRepository<SecurityQuestions> SecurityQuestionsRepository
    {
        get
        {
            if (securityQuestionsRepository != null) return securityQuestionsRepository;
            securityQuestionsRepository = new BaseRepository<SecurityQuestions>(context);
            return securityQuestionsRepository;
        }
    }

    public Task SetAddedStatusAsync<T>(T entity)
    {
        context.SetAddedStatus(entity);
        return Task.CompletedTask;
    }

    public Task SetModifiedStatusAsync<T>(T entity, string concurrencyStamp)
    {
        context.SetConcurrencyStatus(entity, concurrencyStamp);
        return Task.CompletedTask;
    }

    public Task SetConcurrencyOriginalValueAsync<T>(T entity, string concurrencyStamp)
    {
        context.SetConcurrencyOriginalValue(entity, concurrencyStamp);
        return Task.CompletedTask;
    }

    public Task SetPropertyModifiedStatusAsync<T>(T entity, string property)
    {
        context.SetPropertyModifiedStatus(entity, property);
        return Task.CompletedTask;
    }

    public async Task<FrameworkResult> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<FrameworkResult> SaveChangesWithHardDeleteAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesWithHardDeleteAsync(cancellationToken);
    }
}
