namespace HCL.CS.Domain.Entities.Api;

public class IdentityResources : BaseEntity
{
    public virtual string Name { get; set; }

    public virtual string DisplayName { get; set; }

    public virtual string Description { get; set; }

    public virtual bool Enabled { get; set; }

    public virtual bool Required { get; set; }

    public virtual bool Emphasize { get; set; }

    public virtual List<IdentityClaims> IdentityClaims { get; set; }
}
