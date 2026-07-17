using HCL.CS.Domain;
using HCL.CS.Domain.Constants;
using HCL.CS.Domain.Models.Api;

namespace HCL.CS.Service.Interfaces.Interfaces.Api;

public interface INotificationManagementService
{
    Task<NotificationLogResponseModel> GetNotificationLogsAsync(NotificationSearchRequestModel request);

    Task<NotificationTemplateResponseModel> GetNotificationTemplatesAsync();

    Task<ProviderConfigModel> GetProviderConfigAsync(Guid id);

    Task<List<ProviderConfigModel>> GetAllProviderConfigsAsync();

    Task<FrameworkResult> SaveProviderConfigAsync(SaveProviderConfigRequest request);

    Task<FrameworkResult> SetActiveProviderAsync(SetActiveProviderRequest request);

    Task<FrameworkResult> DeleteProviderConfigAsync(DeleteProviderConfigRequest request);

    Task<ProviderFieldDefinitionsResponse> GetProviderFieldDefinitionsAsync();

    Task<FrameworkResult> SendTestNotificationAsync(SendTestNotificationRequest request);
}
