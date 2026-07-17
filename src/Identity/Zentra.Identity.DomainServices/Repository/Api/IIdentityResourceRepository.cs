using System.Threading;
using Zentra.Domain.Entities.Api;
using Zentra.Domain.Models.Api;

namespace Zentra.DomainServices.Repository.Api;

public interface IIdentityResourceRepository : IRepository<IdentityResources>
{
    Task<IdentityResources> GetIdentityResourcesAsync(Guid identityResourceId, CancellationToken cancellationToken = default);

    Task<IdentityResources> GetIdentityResourcesAsync(string identityResourceName, CancellationToken cancellationToken = default);

    Task<IList<IdentityResourcesByScopesModel>> GetAllIdentityResourcesByScopesAsync(IList<string> requestScopes, CancellationToken cancellationToken = default);
}
