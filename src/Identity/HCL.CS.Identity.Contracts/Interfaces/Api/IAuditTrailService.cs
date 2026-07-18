/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

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
