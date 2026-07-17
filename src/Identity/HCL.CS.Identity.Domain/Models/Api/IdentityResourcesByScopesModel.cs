namespace HCL.CS.Domain.Models.Api;

public class IdentityResourcesByScopesModel
{
    public virtual Guid IdentityResourceId { get; set; }

    public virtual string IdentityResourceName { get; set; }

    public virtual string IdentityResourceClaimType { get; set; }

    public virtual string IdentityResourceClaimAliasType { get; set; }
}
