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

public interface IIdentityResourceRepository : IRepository<IdentityResources>
{
    Task<IdentityResources> GetIdentityResourcesAsync(Guid identityResourceId, CancellationToken cancellationToken = default);

    Task<IdentityResources> GetIdentityResourcesAsync(string identityResourceName, CancellationToken cancellationToken = default);

    Task<IList<IdentityResourcesByScopesModel>> GetAllIdentityResourcesByScopesAsync(IList<string> requestScopes, CancellationToken cancellationToken = default);
}
