namespace Zentra.Domain.Models.Endpoint;

public class SecurityTokensModel : BaseModel
{
    public virtual string Key { get; set; }

    public virtual string TokenType { get; set; }

    public virtual string TokenValue { get; set; }

    public virtual DateTime? ConsumedTime { get; set; }

    public virtual DateTime? ConsumedAt { get; set; }

    public virtual bool TokenReuseDetected { get; set; }

    public virtual string ClientId { get; set; }

    public virtual string SessionId { get; set; }

    public virtual string UserId { get; set; }

    public virtual string SubjectId { get; set; }

    public virtual DateTime CreationTime { get; set; }

    public virtual int ExpiresAt { get; set; }
}
