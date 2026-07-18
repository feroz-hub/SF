/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using Newtonsoft.Json;
using HCL.CS.Domain.Entities.Api;
using HCL.CS.Domain.Enums;

namespace HCL.CS.Domain.Models.Api;

public class AuditModel
{
    public string TableName { get; set; }

    public string CreatedBy { get; set; }

    public Dictionary<string, object> OldValues { get; } = new();

    public Dictionary<string, object> NewValues { get; } = new();

    public AuditType AuditType { get; set; }

    public string ActionName { get; set; }

    public List<string> AffectedColumns { get; } = new();

    public AuditTrail ToAudit()
    {
        var audit = new AuditTrail();
        audit.ActionType = AuditType;
        audit.TableName = TableName;
        audit.CreatedOn = DateTime.UtcNow;
        audit.CreatedBy = CreatedBy;
        audit.OldValue = OldValues.Count == 0 ? null : JsonConvert.SerializeObject(OldValues);
        audit.NewValue = NewValues.Count == 0 ? null : JsonConvert.SerializeObject(NewValues);
        audit.AffectedColumn = AffectedColumns.Count == 0 ? null : JsonConvert.SerializeObject(AffectedColumns);
        return audit;
    }
}
