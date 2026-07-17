using Zentra.Domain.Models.Api;
using Zentra.ProxyService.Routes.Extension;
using Zentra.Service.Interfaces.Interfaces.Api.Wrapper;

namespace Zentra.ProxyService.Routes;

internal partial class ApiGateway : BaseApiServiceInstance, IApiGateway
{
    private async Task<bool> AddIdentityResource(string jsonContent)
    {
        var identityResourcesModel = jsonContent.JsonDeserialize<IdentityResourcesModel>();
        var frameworkResult = await IdentityResourceService.AddIdentityResourceAsync(identityResourcesModel);
        return await GenerateApiResults(frameworkResult);
    }

    private async Task<bool> UpdateIdentityResource(string jsonContent)
    {
        var identityResourcesModel = jsonContent.JsonDeserialize<IdentityResourcesModel>();
        var frameworkResult = await IdentityResourceService.UpdateIdentityResourceAsync(identityResourcesModel);
        return await GenerateApiResults(frameworkResult);
    }

    private async Task<bool> DeleteIdentityResourceById(string jsonContent)
    {
        var identityResourceId = jsonContent.JsonDeserialize<Guid>();
        var frameworkResult = await IdentityResourceService.DeleteIdentityResourceAsync(identityResourceId);
        return await GenerateApiResults(frameworkResult);
    }

    private async Task<bool> DeleteIdentityResourceByName(string jsonContent)
    {
        var identityResourceName = jsonContent.JsonDeserialize<string>();
        var frameworkResult = await IdentityResourceService.DeleteIdentityResourceAsync(identityResourceName);
        return await GenerateApiResults(frameworkResult);
    }

    private async Task<bool> GetIdentityResourceById(string jsonContent)
    {
        var identityResourceId = jsonContent.JsonDeserialize<Guid>();
        var frameworkResult = await IdentityResourceService.GetIdentityResourceAsync(identityResourceId);
        await GenerateApiResults(frameworkResult);
        return true;
    }

    private async Task<bool> GetIdentityResourceByName(string jsonContent)
    {
        var identityResourceName = jsonContent.JsonDeserialize<string>();
        var frameworkResult = await IdentityResourceService.GetIdentityResourceAsync(identityResourceName);
        await GenerateApiResults(frameworkResult);
        return true;
    }

    private async Task<bool> GetAllIdentityResources(string jsonContent)
    {
        var frameworkResult = await IdentityResourceService.GetAllIdentityResourcesAsync();
        await GenerateApiResults(frameworkResult);
        return true;
    }

    private async Task<bool> AddIdentityResourceClaim(string jsonContent)
    {
        var identityResourcesClaimModel = jsonContent.JsonDeserialize<IdentityClaimsModel>();
        var frameworkResult = await IdentityResourceService.AddIdentityResourceClaimAsync(identityResourcesClaimModel);
        return await GenerateApiResults(frameworkResult);
    }

    private async Task<bool> DeleteIdentityResourceClaimByResourceIdAsync(string jsonContent)
    {
        var identityResourceId = jsonContent.JsonDeserialize<Guid>();
        var frameworkResult =
            await IdentityResourceService.DeleteIdentityResourceClaimByResourceIdAsync(identityResourceId);
        return await GenerateApiResults(frameworkResult);
    }

    private async Task<bool> DeleteIdentityResourceClaimByIdAsync(string jsonContent)
    {
        var identityResourceClaimId = jsonContent.JsonDeserialize<Guid>();
        var frameworkResult =
            await IdentityResourceService.DeleteIdentityResourceClaimByIdAsync(identityResourceClaimId);
        return await GenerateApiResults(frameworkResult);
    }

    private async Task<bool> DeleteIdentityResourceClaimModel(string jsonContent)
    {
        var identityResourceClaimModel = jsonContent.JsonDeserialize<IdentityClaimsModel>();
        var frameworkResult =
            await IdentityResourceService.DeleteIdentityResourceClaimAsync(identityResourceClaimModel);
        return await GenerateApiResults(frameworkResult);
    }

    private async Task<bool> GetIdentityResourceClaims(string jsonContent)
    {
        var identityClaimsId = jsonContent.JsonDeserialize<Guid>();
        var frameworkResult = await IdentityResourceService.GetIdentityResourceClaimsAsync(identityClaimsId);
        await GenerateApiResults(frameworkResult);
        return true;
    }
}
