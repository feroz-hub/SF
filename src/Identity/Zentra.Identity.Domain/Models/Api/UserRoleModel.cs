namespace Zentra.Domain.Models.Api;

public class UserRoleModel : BaseModel
{
    public virtual Guid RoleId { get; set; }

    public virtual Guid UserId { get; set; }

    public virtual DateTime? ValidFrom { get; set; }

    public virtual DateTime? ValidTo { get; set; }
}
