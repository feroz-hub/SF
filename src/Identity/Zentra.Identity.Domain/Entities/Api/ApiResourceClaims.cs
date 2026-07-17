namespace Zentra.Domain.Entities.Api;

public class ApiResourceClaims : BaseEntity
{
    public virtual Guid ApiResourceId { get; set; }

    public virtual string Type { get; set; }

    public virtual ApiResources ApiResource { get; set; }
}
