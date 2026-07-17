namespace Zentra.Domain.Models.Api;

public class UserRoleClaimTypesModel
{
    public virtual Guid UserId { get; set; }

    public virtual string UserName { get; set; }

    public virtual string RoleName { get; set; }

    public virtual string RoleClaimType { get; set; }

    public virtual string RoleClaimValue { get; set; }
}
