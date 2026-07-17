using Zentra.Domain;
using Zentra.Domain.Constants;
using Zentra.Domain.Models.Api;

namespace Zentra.Service.Interfaces.Interfaces.Api;

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
