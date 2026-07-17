using Zentra.Domain.Enums;

namespace Zentra.Domain.Entities.Api;

public class Notification : BaseEntity
{
    public Guid UserId { get; set; }

    public string MessageId { get; set; }

    public NotificationTypes Type { get; set; }

    public string Activity { get; set; }

    public NotificationStatus Status { get; set; }

    public string Sender { get; set; }

    public string Recipient { get; set; }

    public virtual Users User { get; set; }
}
