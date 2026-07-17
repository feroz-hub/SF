using System.Security.Claims;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using HCL.CS.Domain;
using HCL.CS.Domain.Entities.Api;
using HCL.CS.DomainServices;
using HCL.CS.DomainServices.Repository.Api;

namespace HCL.CS.Infrastructure.Data.Repository.Api;

internal class UserClaimRepository : BaseDispose, IUserClaimRepository
{
    private readonly IApplicationDbContext context;

    public UserClaimRepository(IApplicationDbContext context)
    {
        this.context = context;
    }

    public Task InsertAsync(UserClaims entity)
    {
        entity.IsDeleted = false;
        context.UserClaims.Add(entity);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(UserClaims entity)
    {
        context.UserClaims.Attach(entity);
        context.SetModifiedStatus(entity);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await context.UserClaims.FindAsync(new object[] { id }, cancellationToken);
        if (entity != null) context.UserClaims.Remove(entity);
    }

    public Task DeleteAsync(IList<UserClaims> entityList)
    {
        foreach (var entity in entityList) context.UserClaims.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<IList<UserClaims>> GetClaimsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var userClaim = await context.UserClaims
            .Where(entity => entity.UserId == userId && entity.IsAdminClaim == false)
            .ToListAsync(cancellationToken);
        if (userClaim.Any()) return userClaim;

        return null;
    }

    public async Task<IList<UserClaims>> GetAdminUserClaimsAsync(Guid userId, bool getOnlyAdminClaim = true, CancellationToken cancellationToken = default)
    {
        List<UserClaims> userClaims = getOnlyAdminClaim
            ? await context.UserClaims.Where(entity => entity.UserId == userId && entity.IsAdminClaim).ToListAsync(cancellationToken)
            : await context.UserClaims.Where(entity => entity.UserId == userId).ToListAsync(cancellationToken);

        if (userClaims.Any()) return userClaims;

        return null;
    }


    public async Task<UserClaims> FindIdByClaimAsync(Guid userId, Claim claim, bool isAdminClaim = false, CancellationToken cancellationToken = default)
    {
        var userClaim = await context.UserClaims.FirstOrDefaultAsync(entity => entity.UserId == userId &&
                                                                               entity.ClaimType == claim.Type &&
                                                                               entity.ClaimValue == claim.Value &&
                                                                               entity.IsAdminClaim == isAdminClaim, cancellationToken);

        if (userClaim != null) return userClaim;

        return null;
    }

    public async Task<UserClaims> FindUserClaimByIdAsync(int userClaimId, CancellationToken cancellationToken = default)
    {
        return await context.UserClaims.FirstOrDefaultAsync(entity => entity.Id == userClaimId, cancellationToken);
    }

    public async Task<FrameworkResult> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }
}
