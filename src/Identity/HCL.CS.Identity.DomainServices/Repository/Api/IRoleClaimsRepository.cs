using System.Security.Claims;
using System.Threading;
using HCL.CS.Domain;
using HCL.CS.Domain.Entities.Api;
using HCL.CS.Domain.Models.Api;

namespace HCL.CS.DomainServices.Repository.Api;

public interface IRoleClaimsRepository
{
    Task InsertAsync(RoleClaims entity);

    Task UpdateAsync(RoleClaims entity);

    Task DeleteAsync(int roleClaimId, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid roleId, CancellationToken cancellationToken = default);

    Task DeleteAsync(RoleClaims entity);

    Task<IList<RoleClaims>> GetClaimsAsync(Guid roleId, CancellationToken cancellationToken = default);

    Task<int> FindIdByClaimAsync(Guid roleId, Claim claim, CancellationToken cancellationToken = default);

    Task<RoleClaims> FindRoleByClaimAsync(Guid roleId, Claim claim, CancellationToken cancellationToken = default);

    Task<RoleClaims> FindRoleClaimByIdAsync(int roleClaimId, CancellationToken cancellationToken = default);

    Task<IList<UserRoleClaimTypesModel>> GetRolesAndClaimsForUser(Guid userId, CancellationToken cancellationToken = default);

    Task<FrameworkResult> SaveChangesAsync(CancellationToken cancellationToken = default);
}
