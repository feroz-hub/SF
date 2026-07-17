using System.Threading;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Zentra.Domain;
using Zentra.Domain.Entities.Api;
using Zentra.DomainServices;
using Zentra.DomainServices.Repository.Api;

namespace Zentra.Infrastructure.Data.Repository.Api;

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
