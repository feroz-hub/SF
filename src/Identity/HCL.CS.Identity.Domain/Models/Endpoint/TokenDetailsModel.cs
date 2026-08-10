/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

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
