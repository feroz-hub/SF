using System.Threading;
using Zentra.Domain;
using Zentra.Domain.Entities.Api;

namespace Zentra.DomainServices.UnitOfWork.Api;

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
