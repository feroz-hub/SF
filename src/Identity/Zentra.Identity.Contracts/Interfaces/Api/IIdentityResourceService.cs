using Zentra.Domain;
using Zentra.Domain.Models.Api;

namespace Zentra.Service.Interfaces.Interfaces.Api;

public interface IIdentityResourceService
{
    Task<FrameworkResult> AddIdentityResourceAsync(IdentityResourcesModel identityResourceModel);

    Task<FrameworkResult> UpdateIdentityResourceAsync(IdentityResourcesModel identityResourceModel);

    Task<FrameworkResult> DeleteIdentityResourceAsync(Guid identityResourceId);

    Task<FrameworkResult> DeleteIdentityResourceAsync(string identityResourceName);

    Task<IdentityResourcesModel> GetIdentityResourceAsync(Guid identityResourceId);

    Task<IdentityResourcesModel> GetIdentityResourceAsync(string identityResourceName);

    Task<IList<IdentityResourcesModel>> GetAllIdentityResourcesAsync();

    Task<IList<IdentityResourcesByScopesModel>> GetAllIdentityResourcesByScopesAsync(IList<string> requestedScopes);

    Task<FrameworkResult> AddIdentityResourceClaimAsync(IdentityClaimsModel identityClaimsModel);

    Task<FrameworkResult> DeleteIdentityResourceClaimByResourceIdAsync(Guid identityResourceId);

    Task<FrameworkResult> DeleteIdentityResourceClaimByIdAsync(Guid identityResourceClaimId);

    Task<FrameworkResult> DeleteIdentityResourceClaimAsync(IdentityClaimsModel identityClaimsModel);

    Task<IList<IdentityClaimsModel>> GetIdentityResourceClaimsAsync(Guid identityResourceId);
}
