/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using HCL.CS.Domain.Models.Api;
using HCL.CS.ProxyService.Routes.Extension;
using HCL.CS.Service.Interfaces.Interfaces.Api.Wrapper;

namespace HCL.CS.ProxyService.Routes;

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
