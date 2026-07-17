namespace Zentra.Domain.Entities.Api;

public class IdentityClaims : BaseEntity
{
    public virtual Guid IdentityResourceId { get; set; }

    public virtual string Type { get; set; }

    public virtual string AliasType { get; set; }

    public virtual IdentityResources IdentityResource { get; set; }
}
