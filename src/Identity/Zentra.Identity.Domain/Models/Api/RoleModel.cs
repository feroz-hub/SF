namespace Zentra.Domain.Models.Api;

public class RoleModel : BaseModel
{
    public virtual string Name { get; set; }

    public virtual string Description { get; set; }

    public virtual List<RoleClaimModel> RoleClaims { get; set; }
}
