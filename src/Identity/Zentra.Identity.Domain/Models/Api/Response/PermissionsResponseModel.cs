namespace Zentra.Domain.Models.Api.Response;

public class UserPermissionsResponseModel
{
    public virtual Guid UserId { get; set; }

    public virtual IList<UserRoleClaimsModel> RolePermissions { get; set; }
}

public class UserRoleClaimsModel
{
    public virtual Guid RoleId { get; set; }

    public virtual string RoleName { get; set; }

    public virtual IList<RoleClaimModel> Claims { get; set; }
}
