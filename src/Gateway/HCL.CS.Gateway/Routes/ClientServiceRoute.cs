/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using HCL.CS.Domain.Models.Endpoint;
using HCL.CS.ProxyService.Routes.Extension;
using HCL.CS.Service.Interfaces.Interfaces.Api.Wrapper;

namespace HCL.CS.ProxyService.Routes;

internal partial class ApiGateway : BaseApiServiceInstance, IApiGateway
{
    private async Task<bool> DeleteClient(string jsonContent)
    {
        var clientId = jsonContent.JsonDeserialize<string>();
        var frameworkResult = await ClientServices.DeleteClientAsync(clientId);
        return await GenerateApiResults(frameworkResult);
    }

    private async Task<bool> GenerateClientSecret(string jsonContent)
    {
        var clientId = jsonContent.JsonDeserialize<string>();
        var frameworkResult = await ClientServices.GenerateClientSecret(clientId);
        await GenerateApiResults(frameworkResult);
        return true;
    }

    private async Task<bool> GetAllClient(string jsonContent)
    {
        var frameworkResult = await ClientServices.GetAllClientAsync();
        await GenerateApiResults(frameworkResult);
        return true;
    }

    private async Task<bool> GetClient(string jsonContent)
    {
        var clientId = jsonContent.JsonDeserialize<string>();
        var frameworkResult = await ClientServices.GetClientAsync(clientId);
        await GenerateApiResults(frameworkResult);
        return true;
    }

    private async Task<bool> RegisterClient(string jsonContent)
    {
        var clientModel = jsonContent.JsonDeserialize<ClientsModel>();
        var frameworkResult = await ClientServices.RegisterClientAsync(clientModel);
        await GenerateApiResults(frameworkResult);
        return true;
    }

    private async Task<bool> UpdateClient(string jsonContent)
    {
        var clientModel = jsonContent.JsonDeserialize<ClientsModel>();
        var frameworkResult = await ClientServices.UpdateClientAsync(clientModel);
        await GenerateApiResults(frameworkResult);
        return true;
    }

    private async Task<bool> ProvisionClient(string jsonContent)
    {
        var clientModel = jsonContent.JsonDeserialize<ClientsModel>();
        var provisionedClient = await ClientServices.ProvisionClientAsync(clientModel);
        await GenerateApiResults(provisionedClient);
        return true;
    }
}
