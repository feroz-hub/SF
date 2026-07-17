using System.Security.Claims;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Zentra.Domain;
using Zentra.Domain.Entities.Api;
using Zentra.Domain.Models.Api;
using Zentra.DomainServices;
using Zentra.DomainServices.Repository.Api;

namespace Zentra.Infrastructure.Data.Repository.Api;

internal class RoleClaimsRepository : BaseDispose, IRoleClaimsRepository
{
    private readonly IApplicationDbContext context;

    public RoleClaimsRepository(IApplicationDbContext context)
    {
        this.context = context;
    }

    public Task InsertAsync(RoleClaims entity)
    {
        context.RoleClaims.Add(entity);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(int roleClaimId, CancellationToken cancellationToken = default)
    {
        var entity = await context.RoleClaims.FindAsync(new object[] { roleClaimId }, cancellationToken);
        if (entity != null)
        {
            context.SetRowVersionStatus(entity, entity.RowVersion);
            context.RoleClaims.Remove(entity);
        }
    }

    public async Task DeleteAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        IList<RoleClaims> entity = await context.RoleClaims.Where(x => x.RoleId == roleId).ToListAsync(cancellationToken);
        if (entity.Count > 0) context.RoleClaims.RemoveRange(entity);
    }

    public Task DeleteAsync(RoleClaims entity)
    {
        context.SetRowVersionStatus(entity, entity.RowVersion);
        context.RoleClaims.Remove(entity);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(RoleClaims entity)
    {
        context.RoleClaims.Attach(entity);
        context.SetModifiedStatus(entity);
        return Task.CompletedTask;
    }

    public async Task<IList<RoleClaims>> GetClaimsAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        return await context.RoleClaims.Where(x => x.RoleId == roleId).ToListAsync(cancellationToken);
    }

    public async Task<int> FindIdByClaimAsync(Guid roleId, Claim claim, CancellationToken cancellationToken = default)
    {
        var roleClaim = await context.RoleClaims.FirstOrDefaultAsync(entity =>
            entity.RoleId == roleId && entity.ClaimType == claim.Type && entity.ClaimValue == claim.Value, cancellationToken);
        if (roleClaim != null) return roleClaim.Id;

        return 0;
    }

    public async Task<RoleClaims> FindRoleByClaimAsync(Guid roleId, Claim claim, CancellationToken cancellationToken = default)
    {
        return await context.RoleClaims.FirstOrDefaultAsync(entity =>
            entity.RoleId == roleId && entity.ClaimType == claim.Type && entity.ClaimValue == claim.Value, cancellationToken);
    }

    public async Task<RoleClaims> FindRoleClaimByIdAsync(int roleClaimId, CancellationToken cancellationToken = default)
    {
        return await context.RoleClaims.FirstOrDefaultAsync(entity => entity.Id == roleClaimId, cancellationToken);
    }

    public async Task<IList<UserRoleClaimTypesModel>> GetRolesAndClaimsForUser(Guid userId, CancellationToken cancellationToken = default)
    {
        var userRoleClaims = await (from users in context.Users.AsNoTracking()
                join userRoles in context.UserRoles.AsNoTracking()
                    on users.Id equals userRoles.UserId into userRoleJoin
                from userRoleModel in userRoleJoin.DefaultIfEmpty()
                where userRoleModel.UserId == userId
                join roles in context.Roles.AsNoTracking()
                    on userRoleModel.RoleId equals roles.Id into rolesJoin
                from rolesModel in rolesJoin.DefaultIfEmpty()
                join roleClaims in context.RoleClaims.AsNoTracking()
                    on rolesModel.Id equals roleClaims.RoleId into roleClaimJoin
                from userRoleClaimModel in roleClaimJoin.DefaultIfEmpty()
                where userRoleClaimModel.ClaimType != null
                select new UserRoleClaimTypesModel
                {
                    UserId = users.Id,
                    UserName = users.UserName,
                    RoleName = rolesModel.Name,
                    RoleClaimType = userRoleClaimModel.ClaimType,
                    RoleClaimValue = userRoleClaimModel.ClaimValue
                })
            .ToListAsync(cancellationToken);

        return userRoleClaims;
    }

    public async Task<FrameworkResult> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }
}
