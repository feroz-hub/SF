using Zentra.Domain.Enums;

namespace Zentra.Domain.Models.Api;

public class AuditTrailModel
{
    public virtual Guid Id { get; set; }

    public AuditType ActionType { get; set; } = AuditType.None;

    public string TableName { get; set; }

    public string? OldValue { get; set; }

    public string? NewValue { get; set; }

    public string? AffectedColumn { get; set; }

    public string ActionName { get; set; }

    public string? CreatedBy { get; set; }

    public virtual DateTime CreatedOn { get; set; } = DateTime.UtcNow;
}
