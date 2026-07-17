using Newtonsoft.Json;
using Zentra.Domain.Entities.Api;
using Zentra.Domain.Enums;

namespace Zentra.Domain.Models.Api;

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
