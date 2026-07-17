using Newtonsoft.Json.Linq;
using Zentra.Domain.Models.Api;
using Zentra.ProxyService.Routes.Extension;
using Zentra.Service.Interfaces.Interfaces.Api.Wrapper;

namespace Zentra.ProxyService.Routes;

internal partial class ApiGateway : BaseApiServiceInstance, IApiGateway
{
    private async Task<bool> AddAuditTrail(string jsonContent)
    {
        var auditTrailModel = jsonContent.JsonDeserialize<IEnumerable<AuditTrailModel>>();
        var frameworkResult = await AuditTrailService.AddAuditTrailAsync(auditTrailModel);
        return await GenerateApiResults(frameworkResult);
    }

    private async Task<bool> AddAuditTrailModel(string jsonContent)
    {
        var auditTrailModel = jsonContent.JsonDeserialize<AuditTrailModel>();
        var frameworkResult = await AuditTrailService.AddAuditTrailAsync(auditTrailModel);
        return await GenerateApiResults(frameworkResult);
    }

    private async Task<bool> GetAuditDetailsAsync(string jsonContent)
    {
        var jsonObjects = JObject.Parse(jsonContent);
        //var createdDate = jsonObjects[ApiRouteParameterConstants.CreatedDate].ToObject<DateTime>();
        //var createdBy = jsonObjects[ApiRouteParameterConstants.CreatedBy].ToObject<string>();

        //var pagingModel = jsonContent.JsonDeserialize<PagingModel>();
        var auditSearchResponseModel = jsonContent.JsonDeserialize<AuditSearchRequestModel>();
        var frameworkResult = await AuditTrailService.GetAuditDetailsAsync(auditSearchResponseModel);
        await GenerateApiResults(frameworkResult);
        return true;
    }

    //private async Task<bool> GetAuditDetailsByCreatedOn(string jsonContent)
    //{
    //    var jsonObjects = JObject.Parse(jsonContent);
    //    var createdOn = jsonObjects[ApiRouteParameterConstants.CreatedBy].ToObject<DateTime>();
    //    var pagingModel = jsonContent.JsonDeserialize<PagingModel>();
    //    var frameworkResult = await AuditTrailService.GetAuditDetailsAsync(createdOn, pagingModel);
    //    await GenerateApiResults<AuditResponseModel>(frameworkResult);
    //    return true;
    //}

    //private async Task<bool> GetAuditDetailsByFromDate(string jsonContent)
    //{
    //    var jsonObjects = JObject.Parse(jsonContent);
    //    var createdBy = jsonObjects[ApiRouteParameterConstants.CreatedBy].ToObject<string>();
    //    var fromDate = jsonObjects[ApiRouteParameterConstants.FromDate].ToObject<DateTime>();
    //    var toDate = jsonObjects[ApiRouteParameterConstants.ToDate].ToObject<DateTime>();

    //    var pagingModel = jsonContent.JsonDeserialize<PagingModel>();
    //    var frameworkResult = await AuditTrailService.GetAuditDetailsAsync(createdBy, fromDate, toDate, pagingModel);
    //    await GenerateApiResults<AuditResponseModel>(frameworkResult);
    //    return true;
    //}

    //private async Task<bool> GetAuditDetailsByActionType(string jsonContent)
    //{
    //    var jsonObjects = JObject.Parse(jsonContent);
    //    var createdBy = jsonObjects[ApiRouteParameterConstants.CreatedBy].ToObject<string>();
    //    var fromDate = jsonObjects[ApiRouteParameterConstants.FromDate].ToObject<DateTime>();
    //    var toDate = jsonObjects[ApiRouteParameterConstants.ToDate].ToObject<DateTime>();
    //    var actionType = jsonObjects[ApiRouteParameterConstants.ActionType].ToObject<AuditType>();

    //    var pagingModel = jsonContent.JsonDeserialize<PagingModel>();
    //    var frameworkResult = await AuditTrailService.GetAuditDetailsAsync(createdBy, actionType, fromDate, toDate, pagingModel);
    //    await GenerateApiResults<AuditResponseModel>(frameworkResult);
    //    return true;
    //}
}
