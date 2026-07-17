using System.Security.Claims;
using System.Threading;
using Zentra.Domain;
using Zentra.Domain.Entities.Api;

namespace Zentra.DomainServices.Repository.Api;

public interface IUserClaimRepository
{
    Task InsertAsync(UserClaims entity);
    Task UpdateAsync(UserClaims entity);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task DeleteAsync(IList<UserClaims> entityList);
    Task<UserClaims> FindIdByClaimAsync(Guid userId, Claim claim, bool isAdminClaim = false, CancellationToken cancellationToken = default);

    Task<UserClaims> FindUserClaimByIdAsync(int userClaimId, CancellationToken cancellationToken = default);

    Task<IList<UserClaims>> GetClaimsAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<IList<UserClaims>> GetAdminUserClaimsAsync(Guid userId, bool getOnlyAdminClaim = true, CancellationToken cancellationToken = default);

    Task<FrameworkResult> SaveChangesAsync(CancellationToken cancellationToken = default);
}
