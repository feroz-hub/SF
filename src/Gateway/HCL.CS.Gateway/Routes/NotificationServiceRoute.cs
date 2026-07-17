using HCL.CS.Domain.Models.Api;
using HCL.CS.ProxyService.Routes.Extension;
using HCL.CS.Service.Interfaces.Interfaces.Api.Wrapper;

namespace HCL.CS.ProxyService.Routes;

internal partial class ApiGateway : BaseApiServiceInstance, IApiGateway
{
    private async Task<bool> GetNotificationLogs(string jsonContent)
    {
        var request = jsonContent.JsonDeserialize<NotificationSearchRequestModel>();
        var result = await NotificationManagementService.GetNotificationLogsAsync(request);
        await GenerateApiResults(result);
        return true;
    }

    private async Task<bool> GetNotificationTemplates(string jsonContent)
    {
        var result = await NotificationManagementService.GetNotificationTemplatesAsync();
        await GenerateApiResults(result);
        return true;
    }

    private async Task<bool> GetProviderConfig(string jsonContent)
    {
        var request = jsonContent.JsonDeserialize<SetActiveProviderRequest>();
        var result = await NotificationManagementService.GetProviderConfigAsync(request.Id);
        await GenerateApiResults(result);
        return true;
    }

    private async Task<bool> GetAllProviderConfigs(string jsonContent)
    {
        var result = await NotificationManagementService.GetAllProviderConfigsAsync();
        await GenerateApiResults(result);
        return true;
    }

    private async Task<bool> SaveProviderConfig(string jsonContent)
    {
        var request = jsonContent.JsonDeserialize<SaveProviderConfigRequest>();
        var frameworkResult = await NotificationManagementService.SaveProviderConfigAsync(request);
        return await GenerateApiResults(frameworkResult);
    }

    private async Task<bool> SetActiveProvider(string jsonContent)
    {
        var request = jsonContent.JsonDeserialize<SetActiveProviderRequest>();
        var frameworkResult = await NotificationManagementService.SetActiveProviderAsync(request);
        return await GenerateApiResults(frameworkResult);
    }

    private async Task<bool> DeleteProviderConfig(string jsonContent)
    {
        var request = jsonContent.JsonDeserialize<DeleteProviderConfigRequest>();
        var frameworkResult = await NotificationManagementService.DeleteProviderConfigAsync(request);
        return await GenerateApiResults(frameworkResult);
    }

    private async Task<bool> GetProviderFieldDefinitions(string jsonContent)
    {
        var result = await NotificationManagementService.GetProviderFieldDefinitionsAsync();
        await GenerateApiResults(result);
        return true;
    }

    private async Task<bool> SendTestNotification(string jsonContent)
    {
        var request = jsonContent.JsonDeserialize<SendTestNotificationRequest>();
        var frameworkResult = await NotificationManagementService.SendTestNotificationAsync(request);
        return await GenerateApiResults(frameworkResult);
    }
}
