/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using HCL.CS.Domain;
using HCL.CS.Domain.Entities.Api;
using HCL.CS.DomainServices;
using HCL.CS.DomainServices.Repository.Api;
using HCL.CS.DomainServices.UnitOfWork.Api;
using HCL.CS.Infrastructure.Data.Repository.Api;

namespace HCL.CS.Infrastructure.Data.UnitOfWork.Api;

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
