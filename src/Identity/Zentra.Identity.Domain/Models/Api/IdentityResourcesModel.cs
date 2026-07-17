namespace Zentra.Domain.Models.Api;

public class IdentityResourcesModel : BaseModel
{
    public virtual string Name { get; set; }

    public virtual string DisplayName { get; set; }

    public virtual string Description { get; set; }

    public virtual bool Enabled { get; set; } = true;

    public virtual bool Required { get; set; } = false;

    public virtual bool Emphasize { get; set; } = false;

    public virtual List<IdentityClaimsModel> IdentityClaims { get; set; }
}
