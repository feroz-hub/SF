/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using AutoMapper;
using HCL.CS.Domain;
using HCL.CS.Domain.Models.Api;
using HCL.CS.Domain.Models.Api.Response;
using HCL.CS.DomainServices.Infra;
using HCL.CS.DomainServices.Repository.Api;
using HCL.CS.Service.Implementation.Api.Services;
using HCL.CS.Service.Interfaces.Interfaces.Api;
using HCL.CS.Service.Interfaces.Interfaces.Api.Wrapper;

namespace HCL.CS.ProxyService.Proxy;

public sealed class AuditTrailProxyService : AuditTrailService, IAuditTrailService
{
    private readonly IApiValidator apiValidator;
    private readonly IFrameworkResultService frameworkResult;

    public AuditTrailProxyService(
        IAuditRepository auditRepository,
        ILoggerInstance loggerInstance,
        IMapper mapper,
        IFrameworkResultService frameworkResult,
        IApiValidator apiValidator)
        : base
        (
            auditRepository,
            loggerInstance,
            mapper,
            frameworkResult)
    {
        this.apiValidator = apiValidator;
        this.frameworkResult = frameworkResult;
    }

    public override async Task<FrameworkResult> AddAuditTrailAsync(IEnumerable<AuditTrailModel> audits)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.AddAuditTrailAsync(audits);
    }

    public override async Task<FrameworkResult> AddAuditTrailAsync(AuditTrailModel audit)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.AddAuditTrailAsync(audit);
    }

    public override async Task<AuditResponseModel> GetAuditDetailsAsync(AuditSearchRequestModel auditSearchRequestModel)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed)
            frameworkResult.ThrowCustomMessage(result.Errors.FirstOrDefault().Description);

        return await base.GetAuditDetailsAsync(auditSearchRequestModel);
    }

    //public override async Task<AuditResponseModel> GetAuditDetailsAsync(string createdBy, DateTime? createdOn, PagingModel page)
    //{
    //    var result = await apiValidator.ValidateRequest();
    //    if (result.Status == ResultStatus.Failed)
    //    {
    //        frameworkResult.ThrowCustomMessage(result.Errors.FirstOrDefault().Description);
    //    }

    //    return await base.GetAuditDetailsAsync(createdBy, createdOn, page);
    //}

    //public override async Task<AuditResponseModel> GetAuditDetailsAsync(string createdBy, DateTime? fromDate, DateTime? toDate, PagingModel page)
    //{
    //    var result = await apiValidator.ValidateRequest();
    //    if (result.Status == ResultStatus.Failed)
    //    {
    //        frameworkResult.ThrowCustomMessage(result.Errors.FirstOrDefault().Description);
    //    }

    //    return await base.GetAuditDetailsAsync(createdBy, fromDate, toDate, page);
    //}

    //public override async Task<AuditResponseModel> GetAuditDetailsAsync(string createdBy, AuditType actionType, DateTime? fromDate, DateTime? toDate, PagingModel page)
    //{
    //    var result = await apiValidator.ValidateRequest();
    //    if (result.Status == ResultStatus.Failed)
    //    {
    //        frameworkResult.ThrowCustomMessage(result.Errors.FirstOrDefault().Description);
    //    }

    //    return await base.GetAuditDetailsAsync(createdBy, actionType, fromDate, toDate, page);
    //}
}
