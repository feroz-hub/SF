using System.Threading;
using HCL.CS.Domain;
using HCL.CS.Domain.Entities.Api;

namespace HCL.CS.DomainServices.UnitOfWork.Api;

public interface IResourceUnitOfWork
{
    IRepository<ApiResources> ApiResourcesRepository { get; }
    IRepository<ApiResourceClaims> ApiResourceClaimsRepository { get; }
    IRepository<ApiScopes> ApiScopesRepository { get; }
    IRepository<ApiScopeClaims> ApiScopeClaimsRepository { get; }
    IRepository<IdentityResources> IdentityResourcesRepository { get; }
    IRepository<IdentityClaims> IdentityClaimsRepository { get; }
    Task<FrameworkResult> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<FrameworkResult> SaveChangesWithHardDeleteAsync(CancellationToken cancellationToken = default);
}
