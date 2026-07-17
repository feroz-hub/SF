using HCL.CS.Domain;
using HCL.CS.Domain.Models.Api;
using HCL.CS.Domain.Models.Api.Response;

namespace HCL.CS.Service.Interfaces.Interfaces.Api;

public interface IAuditTrailService
{
    Task<FrameworkResult> AddAuditTrailAsync(IEnumerable<AuditTrailModel> audits);

    Task<FrameworkResult> AddAuditTrailAsync(AuditTrailModel audit);

    Task<AuditResponseModel> GetAuditDetailsAsync(AuditSearchRequestModel auditSearchRequestModel);

    //Task<AuditResponseModel> GetAuditDetailsAsync(string createdBy, DateTime? createdOn, PagingModel page);

    //Task<AuditResponseModel> GetAuditDetailsAsync(string createdBy, DateTime? fromDate, DateTime? toDate, PagingModel page);

    //Task<AuditResponseModel> GetAuditDetailsAsync(string createdBy, AuditType actionType, DateTime? fromDate, DateTime? toDate, PagingModel page);
}
