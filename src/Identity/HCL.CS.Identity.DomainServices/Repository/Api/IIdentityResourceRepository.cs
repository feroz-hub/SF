using System.Threading;
using HCL.CS.Domain.Entities.Api;
using HCL.CS.Domain.Models.Api;

namespace HCL.CS.DomainServices.Repository.Api;

public interface IIdentityResourceRepository : IRepository<IdentityResources>
{
    Task<IdentityResources> GetIdentityResourcesAsync(Guid identityResourceId, CancellationToken cancellationToken = default);

    Task<IdentityResources> GetIdentityResourcesAsync(string identityResourceName, CancellationToken cancellationToken = default);

    Task<IList<IdentityResourcesByScopesModel>> GetAllIdentityResourcesByScopesAsync(IList<string> requestScopes, CancellationToken cancellationToken = default);
}
