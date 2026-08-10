/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using HCL.CS.Domain;
using HCL.CS.Domain.Models.Api;

namespace HCL.CS.Service.Interfaces.Interfaces.Api;

public interface IIdentityResourceService
{
    Task<FrameworkResult> AddIdentityResourceAsync(IdentityResourcesModel identityResourceModel);

    Task<FrameworkResult> UpdateIdentityResourceAsync(IdentityResourcesModel identityResourceModel);

    Task<FrameworkResult> DeleteIdentityResourceAsync(Guid identityResourceId);

    Task<FrameworkResult> DeleteIdentityResourceAsync(string identityResourceName);

    Task<IdentityResourcesModel> GetIdentityResourceAsync(Guid identityResourceId);

    Task<IdentityResourcesModel> GetIdentityResourceAsync(string identityResourceName);

    Task<IList<IdentityResourcesModel>> GetAllIdentityResourcesAsync();

    Task<IList<IdentityResourcesByScopesModel>> GetAllIdentityResourcesByScopesAsync(IList<string> requestedScopes);

    Task<FrameworkResult> AddIdentityResourceClaimAsync(IdentityClaimsModel identityClaimsModel);

    Task<FrameworkResult> DeleteIdentityResourceClaimByResourceIdAsync(Guid identityResourceId);

    Task<FrameworkResult> DeleteIdentityResourceClaimByIdAsync(Guid identityResourceClaimId);

    Task<FrameworkResult> DeleteIdentityResourceClaimAsync(IdentityClaimsModel identityClaimsModel);

    Task<IList<IdentityClaimsModel>> GetIdentityResourceClaimsAsync(Guid identityResourceId);
}
