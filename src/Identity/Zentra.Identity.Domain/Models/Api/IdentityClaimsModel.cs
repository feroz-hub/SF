namespace Zentra.Domain.Models.Api;

public class IdentityClaimsModel : BaseModel
{
    public virtual Guid IdentityResourceId { get; set; }

    public virtual string Type { get; set; }

    public virtual string AliasType { get; set; }
}
