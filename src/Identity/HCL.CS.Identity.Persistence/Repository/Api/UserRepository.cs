/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using System.Threading;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using HCL.CS.Domain;
using HCL.CS.Domain.Entities.Api;
using HCL.CS.DomainServices;
using HCL.CS.DomainServices.Repository.Api;

namespace HCL.CS.Infrastructure.Data.Repository.Api;

internal class UserRepository : BaseDispose, IUserRepository
{
    private readonly IApplicationDbContext context;

    private readonly
        UserStore<Users, Roles, ApplicationDbContext, Guid, UserClaims, UserRoles, UserLogins, UserTokens, RoleClaims>
        userStoreWrapper;

    public UserRepository(IApplicationDbContext context,
        IUserStore<Users> userStoreWrapper)
    {
        this.context = context;
        // We can only have control on user store by the below approach. Tried with interface, extension methods which not working in autosavechanges.
        this.userStoreWrapper =
            userStoreWrapper as UserStore<Users, Roles, ApplicationDbContext, Guid, UserClaims, UserRoles, UserLogins,
                UserTokens, RoleClaims>;
    }

    public Task DeleteAsync(Users entity)
    {
        entity.IsDeleted = true;
        context.Users.Remove(entity);
        return Task.CompletedTask;
    }

    public virtual Task UpdateAsync(Users entity, string[] affectedProperties)
    {
        context.Users.Attach(entity);
        foreach (var property in affectedProperties) context.SetPropertyModifiedStatus(entity, property);
        return Task.CompletedTask;
    }

    public async Task<IList<Users>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken = default)
    {
        var roleUserIdsQuery =
            from role in context.Roles.AsNoTracking()
            from userRole in context.UserRoles.AsNoTracking()
            where role.Id == userRole.RoleId && role.Name == roleName &&
                  userRole.ValidFrom <= DateTime.UtcNow && userRole.ValidTo >= DateTime.UtcNow
            select userRole.UserId;

        var userList = await context.Users.AsNoTracking().Where(user => roleUserIdsQuery.Contains(user.Id)).ToListAsync(cancellationToken);
        return userList;
    }

    public async Task<Users?> FindByUserNameIncludingDeletedAsync(string userName, CancellationToken cancellationToken = default)
    {
        return await context.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.UserName == userName, cancellationToken);
    }

    public async Task<IList<Users>> FindByDirectoryImmutableIdAsync(
        string directoryImmutableId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(directoryImmutableId)) return Array.Empty<Users>();

        return await context.Users
            .Where(user => user.DirectoryImmutableId == directoryImmutableId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IList<Users>> FindByNormalizedEmailAsync(
        string normalizedEmail,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(normalizedEmail)) return Array.Empty<Users>();

        return await context.Users
            .IgnoreQueryFilters()
            .Where(user => user.NormalizedEmail == normalizedEmail)
            .ToListAsync(cancellationToken);
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

    public Task EnableIdentityAutoSaveChanges()
    {
        userStoreWrapper.AutoSaveChanges = true;
        return Task.CompletedTask;
    }

    public Task DisableIdentityAutoSaveChanges()
    {
        userStoreWrapper.AutoSaveChanges = false;
        return Task.CompletedTask;
    }

    public async Task<FrameworkResult> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IList<Users>> GetAllUsersAsync(CancellationToken cancellationToken = default)
    {
        var users = await context.Users.AsNoTracking().ToListAsync(cancellationToken);
        return users;
    }
}
