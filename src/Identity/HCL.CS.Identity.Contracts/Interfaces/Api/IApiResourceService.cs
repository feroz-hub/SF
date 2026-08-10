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

public interface IApiResourceService
{
    Task<FrameworkResult> AddApiResourceAsync(ApiResourcesModel apiResourceModel);

    Task<FrameworkResult> UpdateApiResourceAsync(ApiResourcesModel apiResourceModel);

    Task<FrameworkResult> DeleteApiResourceAsync(Guid apiResourceId);

    Task<FrameworkResult> DeleteApiResourceAsync(string apiResourceName);

    Task<ApiResourcesModel> GetApiResourceAsync(string apiResourceName);

    Task<ApiResourcesModel> GetApiResourceAsync(Guid apiResourceId);

    Task<IList<ApiResourcesModel>> GetAllApiResourcesAsync();

    Task<IList<ApiScopesModel>> GetAllApiScopesAsync();

    Task<IList<ApiResourcesByScopesModel>> GetAllApiResourcesByScopesAsync(IList<string> requestedScopes);

    Task<FrameworkResult> AddApiResourceClaimAsync(ApiResourceClaimsModel apiResourceClaimModel);

    Task<FrameworkResult> DeleteApiResourceClaimByResourceIdAsync(Guid apiResourceId);

    Task<FrameworkResult> DeleteApiResourceClaimByIdAsync(Guid apiResourceClaimId);

    Task<FrameworkResult> DeleteApiResourceClaimAsync(ApiResourceClaimsModel apiResourceClaimModel);

    Task<IList<ApiResourceClaimsModel>> GetApiResourceClaimsAsync(Guid apiResourceId);

    Task<FrameworkResult> AddApiScopeAsync(ApiScopesModel apiScopesModel);

    Task<FrameworkResult> UpdateApiScopeAsync(ApiScopesModel apiScopesModel);

    Task<FrameworkResult> DeleteApiScopeAsync(Guid apiScopeId);

    Task<FrameworkResult> DeleteApiScopeAsync(string apiScopeName);

    Task<ApiScopesModel> GetApiScopeAsync(Guid apiScopeId);

    Task<ApiScopesModel> GetApiScopeAsync(string apiScopeName);

    Task<FrameworkResult> AddApiScopeClaimAsync(ApiScopeClaimsModel apiScopeClaimModel);

    Task<FrameworkResult> DeleteApiScopeClaimByScopeIdAsync(Guid apiScopeId);

    Task<FrameworkResult> DeleteApiScopeClaimByIdAsync(Guid apiScopeClaimId);

    Task<FrameworkResult> DeleteApiScopeClaimAsync(ApiScopeClaimsModel apiScopeClaimModel);

    Task<IList<ApiScopeClaimsModel>> GetApiScopeClaimsAsync(Guid apiScopeId);
}
