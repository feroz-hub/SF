using Zentra.Domain.Models.Api;
using Zentra.ProxyService.Routes.Extension;
using Zentra.Service.Interfaces.Interfaces.Api.Wrapper;

namespace Zentra.ProxyService.Routes;

internal partial class ApiGateway : BaseApiServiceInstance, IApiGateway
{
    private async Task<bool> GetAllExternalAuthProviders(string jsonContent)
    {
        var result = await ExternalAuthManagementService.GetAllProvidersAsync();
        await GenerateApiResults(result);
        return true;
    }

    private async Task<bool> GetExternalAuthProvider(string jsonContent)
    {
        var request = jsonContent.JsonDeserialize<DeleteExternalAuthProviderRequest>();
        var result = await ExternalAuthManagementService.GetProviderAsync(request.Id);
        await GenerateApiResults(result);
        return true;
    }

    private async Task<bool> SaveExternalAuthProvider(string jsonContent)
    {
        var request = jsonContent.JsonDeserialize<SaveExternalAuthProviderRequest>();
        var frameworkResult = await ExternalAuthManagementService.SaveProviderAsync(request);
        return await GenerateApiResults(frameworkResult);
    }

    private async Task<bool> DeleteExternalAuthProvider(string jsonContent)
    {
        var request = jsonContent.JsonDeserialize<DeleteExternalAuthProviderRequest>();
        var frameworkResult = await ExternalAuthManagementService.DeleteProviderAsync(request);
        return await GenerateApiResults(frameworkResult);
    }

    private async Task<bool> TestExternalAuthProvider(string jsonContent)
    {
        var request = jsonContent.JsonDeserialize<TestExternalAuthProviderRequest>();
        var frameworkResult = await ExternalAuthManagementService.TestProviderAsync(request);
        return await GenerateApiResults(frameworkResult);
    }

    private async Task<bool> GetExternalAuthFieldDefinitions(string jsonContent)
    {
        var result = await ExternalAuthManagementService.GetFieldDefinitionsAsync();
        await GenerateApiResults(result);
        return true;
    }
}
