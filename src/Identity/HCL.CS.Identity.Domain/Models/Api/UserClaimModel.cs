namespace HCL.CS.Domain.Models.Api;

public class UserClaimModel : BaseTrailModel
{
    public virtual int Id { get; set; }

    public virtual Guid UserId { get; set; }

    public virtual string ClaimType { get; set; }

    public virtual string ClaimValue { get; set; }

    public virtual bool IsAdminClaim { get; set; } = false;
}
