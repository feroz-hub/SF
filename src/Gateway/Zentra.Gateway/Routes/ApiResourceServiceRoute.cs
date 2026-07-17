using Zentra.Domain.Models.Api;
using Zentra.ProxyService.Routes.Extension;
using Zentra.Service.Interfaces.Interfaces.Api.Wrapper;

namespace Zentra.ProxyService.Routes;

internal partial class ApiGateway : BaseApiServiceInstance, IApiGateway
{
    private async Task<bool> AddApiResource(string jsonContent)
    {
        var apiResourcesModel = jsonContent.JsonDeserialize<ApiResourcesModel>();
        var frameworkResult = await ApiResourceService.AddApiResourceAsync(apiResourcesModel);
        return await GenerateApiResults(frameworkResult);
    }

    private async Task<bool> UpdateApiResource(string jsonContent)
    {
        var apiResourcesModel = jsonContent.JsonDeserialize<ApiResourcesModel>();
        var frameworkResult = await ApiResourceService.UpdateApiResourceAsync(apiResourcesModel);
        return await GenerateApiResults(frameworkResult);
    }

    private async Task<bool> DeleteApiResourceById(string jsonContent)
    {
        var apiResourceId = jsonContent.JsonDeserialize<Guid>();
        var frameworkResult = await ApiResourceService.DeleteApiResourceAsync(apiResourceId);
        return await GenerateApiResults(frameworkResult);
    }

    private async Task<bool> DeleteApiResourceByName(string jsonContent)
    {
        var apiResourceName = jsonContent.JsonDeserialize<string>();
        var frameworkResult = await ApiResourceService.DeleteApiResourceAsync(apiResourceName);
        return await GenerateApiResults(frameworkResult);
    }

    private async Task<bool> GetApiResourceByName(string jsonContent)
    {
        var apiResourceName = jsonContent.JsonDeserialize<string>();
        var apiresult = await ApiResourceService.GetApiResourceAsync(apiResourceName);
        await GenerateApiResults(apiresult);
        return true;
    }

    private async Task<bool> GetApiResourceById(string jsonContent)
    {
        var apiResourceId = jsonContent.JsonDeserialize<Guid>();
        var frameworkResult = await ApiResourceService.GetApiResourceAsync(apiResourceId);
        await GenerateApiResults(frameworkResult);
        return true;
    }

    private async Task<bool> GetAllApiResources(string jsonContent)
    {
        var frameworkResult = await ApiResourceService.GetAllApiResourcesAsync();
        await GenerateApiResults(frameworkResult);
        return true;
    }

    private async Task<bool> AddApiResourceClaim(string jsonContent)
    {
        var apiResourcesClaimModel = jsonContent.JsonDeserialize<ApiResourceClaimsModel>();
        var frameworkResult = await ApiResourceService.AddApiResourceClaimAsync(apiResourcesClaimModel);
        return await GenerateApiResults(frameworkResult);
    }

    private async Task<bool> DeleteApiResourceClaimById(string jsonContent)
    {
        var apiResourceClaimId = jsonContent.JsonDeserialize<Guid>();
        var frameworkResult = await ApiResourceService.DeleteApiResourceClaimByIdAsync(apiResourceClaimId);
        return await GenerateApiResults(frameworkResult);
    }

    private async Task<bool> DeleteApiResourceClaimByResourceIdAsync(string jsonContent)
    {
        var apiResourceId = jsonContent.JsonDeserialize<Guid>();
        var frameworkResult = await ApiResourceService.DeleteApiResourceClaimByResourceIdAsync(apiResourceId);
        return await GenerateApiResults(frameworkResult);
    }

    private async Task<bool> DeleteApiResourceClaimModel(string jsonContent)
    {
        var apiResourcesClaimModel = jsonContent.JsonDeserialize<ApiResourceClaimsModel>();
        var frameworkResult = await ApiResourceService.DeleteApiResourceClaimAsync(apiResourcesClaimModel);
        return await GenerateApiResults(frameworkResult);
    }

    private async Task<bool> GetApiResourceClaimsById(string jsonContent)
    {
        var apiResourceId = jsonContent.JsonDeserialize<Guid>();
        var frameworkResult = await ApiResourceService.GetApiResourceClaimsAsync(apiResourceId);
        await GenerateApiResults(frameworkResult);
        return true;
    }

    private async Task<bool> AddApiScope(string jsonContent)
    {
        var apiScopeModel = jsonContent.JsonDeserialize<ApiScopesModel>();
        var frameworkResult = await ApiResourceService.AddApiScopeAsync(apiScopeModel);
        return await GenerateApiResults(frameworkResult);
    }

    private async Task<bool> UpdateApiScope(string jsonContent)
    {
        var apiScopeModel = jsonContent.JsonDeserialize<ApiScopesModel>();
        var frameworkResult = await ApiResourceService.UpdateApiScopeAsync(apiScopeModel);
        return await GenerateApiResults(frameworkResult);
    }

    private async Task<bool> DeleteApiScopeById(string jsonContent)
    {
        var apiScopeId = jsonContent.JsonDeserialize<Guid>();
        var frameworkResult = await ApiResourceService.DeleteApiScopeAsync(apiScopeId);
        return await GenerateApiResults(frameworkResult);
    }

    private async Task<bool> DeleteApiScopeByName(string jsonContent)
    {
        var apiScopeName = jsonContent.JsonDeserialize<string>();
        var frameworkResult = await ApiResourceService.DeleteApiScopeAsync(apiScopeName);
        return await GenerateApiResults(frameworkResult);
    }

    private async Task<bool> GetApiScopeById(string jsonContent)
    {
        var apiScopeId = jsonContent.JsonDeserialize<Guid>();
        var frameworkResult = await ApiResourceService.GetApiScopeAsync(apiScopeId);
        await GenerateApiResults(frameworkResult);
        return true;
    }

    private async Task<bool> GetApiScopeByName(string jsonContent)
    {
        var apiScopeName = jsonContent.JsonDeserialize<string>();
        var frameworkResult = await ApiResourceService.GetApiScopeAsync(apiScopeName);
        await GenerateApiResults(frameworkResult);
        return true;
    }

    private async Task<bool> GetAllApiScopes(string jsonContent)
    {
        var frameworkResult = await ApiResourceService.GetAllApiScopesAsync();
        await GenerateApiResults(frameworkResult);
        return true;
    }

    private async Task<bool> AddApiScopeClaim(string jsonContent)
    {
        var apiScopeClaimModel = jsonContent.JsonDeserialize<ApiScopeClaimsModel>();
        var frameworkResult = await ApiResourceService.AddApiScopeClaimAsync(apiScopeClaimModel);
        return await GenerateApiResults(frameworkResult);
    }

    private async Task<bool> DeleteApiScopeClaimByScopeId(string jsonContent)
    {
        var apiScopeId = jsonContent.JsonDeserialize<Guid>();
        var frameworkResult = await ApiResourceService.DeleteApiScopeClaimByScopeIdAsync(apiScopeId);
        return await GenerateApiResults(frameworkResult);
    }

    private async Task<bool> DeleteApiScopeClaimById(string jsonContent)
    {
        var apiScopeClaimId = jsonContent.JsonDeserialize<Guid>();
        var frameworkResult = await ApiResourceService.DeleteApiScopeClaimByIdAsync(apiScopeClaimId);
        return await GenerateApiResults(frameworkResult);
    }

    private async Task<bool> DeleteApiScopeClaimModel(string jsonContent)
    {
        var apiScopeClaimModel = jsonContent.JsonDeserialize<ApiScopeClaimsModel>();
        var frameworkResult = await ApiResourceService.DeleteApiScopeClaimAsync(apiScopeClaimModel);
        return await GenerateApiResults(frameworkResult);
    }

    private async Task<bool> GetApiScopeClaims(string jsonContent)
    {
        var apiScopeId = jsonContent.JsonDeserialize<Guid>();
        var frameworkResult = await ApiResourceService.GetApiScopeClaimsAsync(apiScopeId);
        await GenerateApiResults(frameworkResult);
        return true;
    }

    private async Task<bool> GetAllApiResourcesByScopesAsync(string jsonContent)
    {
        var apiScopeName = jsonContent.JsonDeserialize<IList<string>>();
        var frameworkResult = await ApiResourceService.GetAllApiResourcesByScopesAsync(apiScopeName);
        await GenerateApiResults(frameworkResult);
        return true;
    }
}
