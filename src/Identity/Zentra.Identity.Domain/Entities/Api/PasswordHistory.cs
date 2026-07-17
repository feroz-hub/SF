namespace Zentra.Domain.Entities.Api;

public class PasswordHistory : BaseEntity
{
    public virtual Guid UserId { get; set; }

    public virtual DateTime ChangedOn { get; set; }

    public virtual string PasswordHash { get; set; }

    public virtual Users User { get; set; }
}
