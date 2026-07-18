/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

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
