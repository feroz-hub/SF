using Zentra.Domain;
using Zentra.Domain.Models.Api;

namespace Zentra.Service.Interfaces.Interfaces.Api;

public interface IApiResourceService
{
    Task<FrameworkResult> AddApiResourceAsync(ApiResourcesModel apiResourceModel);

    Task<FrameworkResult> UpdateApiResourceAsync(ApiResourcesModel apiResourceModel);

    Task<FrameworkResult> DeleteApiResourceAsync(Guid apiResourceId);

    Task<FrameworkResult> DeleteApiResourceAsync(string apiResourceName);

    Task<ApiResourcesModel> GetApiResourceAsync(string apiResourceName);

    Task<ApiResourcesModel> GetApiResourceAsync(Guid apiResourceId);

    Task<IList<ApiResourcesModel>> GetAllApiResourcesAsync();

    Task<IList<ApiScopesModel>> GetAllApiScopesAsync();

    Task<IList<ApiResourcesByScopesModel>> GetAllApiResourcesByScopesAsync(IList<string> requestedScopes);

    Task<FrameworkResult> AddApiResourceClaimAsync(ApiResourceClaimsModel apiResourceClaimModel);

    Task<FrameworkResult> DeleteApiResourceClaimByResourceIdAsync(Guid apiResourceId);

    Task<FrameworkResult> DeleteApiResourceClaimByIdAsync(Guid apiResourceClaimId);

    Task<FrameworkResult> DeleteApiResourceClaimAsync(ApiResourceClaimsModel apiResourceClaimModel);

    Task<IList<ApiResourceClaimsModel>> GetApiResourceClaimsAsync(Guid apiResourceId);

    Task<FrameworkResult> AddApiScopeAsync(ApiScopesModel apiScopesModel);

    Task<FrameworkResult> UpdateApiScopeAsync(ApiScopesModel apiScopesModel);

    Task<FrameworkResult> DeleteApiScopeAsync(Guid apiScopeId);

    Task<FrameworkResult> DeleteApiScopeAsync(string apiScopeName);

    Task<ApiScopesModel> GetApiScopeAsync(Guid apiScopeId);

    Task<ApiScopesModel> GetApiScopeAsync(string apiScopeName);

    Task<FrameworkResult> AddApiScopeClaimAsync(ApiScopeClaimsModel apiScopeClaimModel);

    Task<FrameworkResult> DeleteApiScopeClaimByScopeIdAsync(Guid apiScopeId);

    Task<FrameworkResult> DeleteApiScopeClaimByIdAsync(Guid apiScopeClaimId);

    Task<FrameworkResult> DeleteApiScopeClaimAsync(ApiScopeClaimsModel apiScopeClaimModel);

    Task<IList<ApiScopeClaimsModel>> GetApiScopeClaimsAsync(Guid apiScopeId);
}
