/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using System.Threading;
using HCL.CS.Domain.Entities.Api;
using HCL.CS.Domain.Models.Api;

namespace HCL.CS.DomainServices.Repository.Api;

public interface IApiResourceRepository : IRepository<ApiResources>
{
    /// <summary>Load resource only (no scopes/claims). Use for update to avoid child RowVersion concurrency.</summary>
    Task<ApiResources> GetApiResourceForUpdateAsync(Guid apiResourceId, CancellationToken cancellationToken = default);

    Task<ApiResources> GetApiResourcesAsync(Guid apiResourceId, CancellationToken cancellationToken = default);

    Task<ApiResources> GetApiResourcesAsync(string apiResourceName, CancellationToken cancellationToken = default);

    Task<IList<ApiResources>> GetAllApiResourcesAsync(CancellationToken cancellationToken = default);

    Task<IList<ApiScopes>> GetAllApiScopesAsync(CancellationToken cancellationToken = default);

    Task<IList<ApiResourcesByScopesModel>> GetAllApiResourcesByScopesAsync(IList<string> requestedScopes, CancellationToken cancellationToken = default);
}
