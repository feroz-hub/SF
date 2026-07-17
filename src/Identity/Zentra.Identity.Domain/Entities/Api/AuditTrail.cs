using Zentra.Domain.Enums;

namespace Zentra.Domain.Entities.Api;

public class AuditTrail : BaseEntity
{
    public AuditType ActionType { get; set; }

    public string TableName { get; set; }

    public string OldValue { get; set; }

    public string NewValue { get; set; }

    public string AffectedColumn { get; set; }

    public string ActionName { get; set; }
}
