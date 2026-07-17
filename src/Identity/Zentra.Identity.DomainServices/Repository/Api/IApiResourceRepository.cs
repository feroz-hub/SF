using System.Threading;
using Zentra.Domain.Entities.Api;
using Zentra.Domain.Models.Api;

namespace Zentra.DomainServices.Repository.Api;

public interface IApiResourceRepository : IRepository<ApiResources>
{
    /// <summary>Load resource only (no scopes/claims). Use for update to avoid child RowVersion concurrency.</summary>
    Task<ApiResources> GetApiResourceForUpdateAsync(Guid apiResourceId, CancellationToken cancellationToken = default);

    Task<ApiResources> GetApiResourcesAsync(Guid apiResourceId, CancellationToken cancellationToken = default);

    Task<ApiResources> GetApiResourcesAsync(string apiResourceName, CancellationToken cancellationToken = default);

    Task<IList<ApiResources>> GetAllApiResourcesAsync(CancellationToken cancellationToken = default);

    Task<IList<ApiScopes>> GetAllApiScopesAsync(CancellationToken cancellationToken = default);

    Task<IList<ApiResourcesByScopesModel>> GetAllApiResourcesByScopesAsync(IList<string> requestedScopes, CancellationToken cancellationToken = default);
}
