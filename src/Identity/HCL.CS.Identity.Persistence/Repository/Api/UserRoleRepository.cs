using System.Threading;
using Microsoft.EntityFrameworkCore;
using HCL.CS.Domain;
using HCL.CS.Domain.Entities.Api;
using HCL.CS.DomainServices;
using HCL.CS.DomainServices.Repository.Api;

namespace HCL.CS.Infrastructure.Data.Repository.Api;

internal class UserRoleRepository : BaseDispose, IUserRoleRepository
{
    private readonly IApplicationDbContext context;

    public UserRoleRepository(IApplicationDbContext context)
    {
        this.context = context;
    }

    public Task InsertAsync(UserRoles entity)
    {
        context.UserRoles.Add(entity);
        return Task.CompletedTask;
    }

    public virtual Task UpdateAsync(UserRoles entity)
    {
        context.UserRoles.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(IList<UserRoles> entityList)
    {
        foreach (var entity in entityList) context.UserRoles.Remove(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(UserRoles entity)
    {
        context.UserRoles.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<UserRoles> GetUserRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        var entity = await context.UserRoles.Where(roleClaimEntity => roleClaimEntity.UserId == userId &&
                                                                      roleClaimEntity.RoleId == roleId)
            .FirstOrDefaultAsync(cancellationToken);
        return entity;
    }

    public async Task<IList<UserRoles>> GetUserRoleAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var entity = await context.UserRoles.Where(roleClaimEntity => roleClaimEntity.UserId == userId).ToListAsync(cancellationToken);
        return entity;
    }

    public async Task<FrameworkResult> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }
}
