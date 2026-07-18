/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using HCL.CS.Domain.Enums;

namespace HCL.CS.Domain.Models.Api;

public class AuditSearchRequestModel
{
    public AuditType ActionType { get; set; } = AuditType.None;

    public string CreatedBy { get; set; }

    public DateTime? FromDate { get; set; }

    public DateTime? ToDate { get; set; }

    public PagingModel Page { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string SearchValue { get; set; }
}
