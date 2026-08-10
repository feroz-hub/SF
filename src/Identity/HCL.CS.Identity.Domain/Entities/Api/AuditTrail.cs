/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using HCL.CS.Domain.Enums;

namespace HCL.CS.Domain.Entities.Api;

public class AuditTrail : BaseEntity
{
    public AuditType ActionType { get; set; }

    public string TableName { get; set; }

    public string OldValue { get; set; }

    public string NewValue { get; set; }

    public string AffectedColumn { get; set; }

    public string ActionName { get; set; }
}
