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
}
