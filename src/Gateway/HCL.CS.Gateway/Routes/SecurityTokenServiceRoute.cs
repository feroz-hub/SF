using Newtonsoft.Json.Linq;
using HCL.CS.Domain.Constants;
using HCL.CS.Domain.Models.Api;
using HCL.CS.Service.Interfaces.Interfaces.Api.Wrapper;

namespace HCL.CS.ProxyService.Routes;

internal partial class ApiGateway : BaseApiServiceInstance, IApiGateway
{
    private async Task<bool> GetActiveSecurityTokensByClientIds(string jsonContent)
    {
        PagingModel paging = null;
        var jsonObjects = JObject.Parse(jsonContent);
        var clientsList = jsonObjects[ApiRouteParameterConstants.ClientsList].ToObject<IList<string>>();
        if (jsonObjects.ContainsKey(ApiRouteParameterConstants.PagingModel))
            paging = jsonObjects[ApiRouteParameterConstants.PagingModel].ToObject<PagingModel>();

        var tokenModel = await SecurityTokenService.GetClientsActiveSecurityTokensAsync(clientsList, paging);
        await GenerateApiResults(tokenModel);
        return true;
    }

    private async Task<bool> GetActiveSecurityTokensByUserIds(string jsonContent)
    {
        PagingModel paging = null;
        var jsonObjects = JObject.Parse(jsonContent);
        var userList = jsonObjects[ApiRouteParameterConstants.UserList].ToObject<IList<string>>();
        if (jsonObjects.ContainsKey(ApiRouteParameterConstants.PagingModel))
            paging = jsonObjects[ApiRouteParameterConstants.PagingModel].ToObject<PagingModel>();

        var tokenModel = await SecurityTokenService.GetUsersActiveSecurityTokensAsync(userList, paging);
        await GenerateApiResults(tokenModel);
        return true;
    }

    private async Task<bool> GetActiveSecurityTokensBetweenDates(string jsonContent)
    {
        PagingModel paging = null;
        var jsonObjects = JObject.Parse(jsonContent);
        var fromdate = jsonObjects[ApiRouteParameterConstants.FromDate].ToObject<DateTime>();
        var todate = jsonObjects[ApiRouteParameterConstants.ToDate].ToObject<DateTime>();
        if (jsonObjects.ContainsKey(ApiRouteParameterConstants.PagingModel))
            paging = jsonObjects[ApiRouteParameterConstants.PagingModel].ToObject<PagingModel>();

        var tokenModel = await SecurityTokenService.GetActiveSecurityTokensAsync(fromdate, todate, paging);
        await GenerateApiResults(tokenModel);
        return true;
    }

    private async Task<bool> GetAllSecurityTokensBetweenDates(string jsonContent)
    {
        PagingModel paging = null;
        var jsonObjects = JObject.Parse(jsonContent);
        var fromdate = jsonObjects[ApiRouteParameterConstants.FromDate].ToObject<DateTime>();
        var todate = jsonObjects[ApiRouteParameterConstants.ToDate].ToObject<DateTime>();
        if (jsonObjects.ContainsKey(ApiRouteParameterConstants.PagingModel))
            paging = jsonObjects[ApiRouteParameterConstants.PagingModel].ToObject<PagingModel>();

        var tokenModel = await SecurityTokenService.GetAllSecurityTokensAsync(fromdate, todate, paging);
        await GenerateApiResults(tokenModel);
        return true;
    }
}
