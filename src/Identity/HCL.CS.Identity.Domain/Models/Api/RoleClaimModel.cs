namespace HCL.CS.Domain.Models.Api;

public class RoleClaimModel : BaseTrailModel
{
    public virtual int Id { get; set; }

    public virtual Guid RoleId { get; set; }

    public virtual string ClaimType { get; set; }

    public virtual string ClaimValue { get; set; }
}
