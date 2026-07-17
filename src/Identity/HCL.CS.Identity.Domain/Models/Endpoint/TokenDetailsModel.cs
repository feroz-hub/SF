using HCL.CS.Domain.Models.Api;
using HCL.CS.Domain.Models.Api.Response;

namespace HCL.CS.Domain.Models.Endpoint;

public class TokenDetailsModel
{
    public virtual UserModel User { get; set; }

    public virtual ClientsModel Client { get; set; }

    public virtual IList<IdentityResourcesModel> IdentityResources { get; set; } = new List<IdentityResourcesModel>();

    public virtual IList<ApiResourcesModel> ApiResources { get; set; } = new List<ApiResourcesModel>();

    public virtual IList<ApiScopesModel> ApiScopes { get; set; } = new List<ApiScopesModel>();

    public virtual IList<IdentityResourcesByScopesModel> IdentityResourcesByScopes { get; set; } =
        new List<IdentityResourcesByScopesModel>();

    public virtual IList<ApiResourcesByScopesModel> ApiResourcesByScopes { get; set; } =
        new List<ApiResourcesByScopesModel>();

    public virtual IList<UserRoleClaimTypesModel> UserRoleClaimTypes { get; set; } =
        new List<UserRoleClaimTypesModel>();

    public virtual IList<string> UserRoles { get; set; } = new List<string>();

    public virtual IList<UserRoleClaimsModel> RolePermissions { get; set; } = new List<UserRoleClaimsModel>();
}
