using AutoMapper;
using Zentra.Domain;
using Zentra.Domain.Models.Api;
using Zentra.Domain.Models.Api.Response;
using Zentra.DomainServices.Infra;
using Zentra.DomainServices.Repository.Api;
using Zentra.Service.Implementation.Api.Services;
using Zentra.Service.Interfaces.Interfaces.Api;
using Zentra.Service.Interfaces.Interfaces.Api.Wrapper;

namespace Zentra.ProxyService.Proxy;

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
