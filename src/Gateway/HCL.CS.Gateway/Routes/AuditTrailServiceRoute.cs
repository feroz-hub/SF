/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using Newtonsoft.Json.Linq;
using HCL.CS.Domain.Models.Api;
using HCL.CS.ProxyService.Routes.Extension;
using HCL.CS.Service.Interfaces.Interfaces.Api.Wrapper;

namespace HCL.CS.ProxyService.Routes;

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
