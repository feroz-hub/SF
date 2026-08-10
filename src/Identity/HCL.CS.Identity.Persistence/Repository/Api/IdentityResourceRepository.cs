/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using System.Threading;
using Microsoft.EntityFrameworkCore;
using HCL.CS.Domain.Entities.Api;
using HCL.CS.Domain.Models.Api;
using HCL.CS.DomainServices;
using HCL.CS.DomainServices.Repository.Api;

namespace HCL.CS.Infrastructure.Data.Repository.Api;

internal class IdentityResourceRepository : BaseRepository<IdentityResources>, IIdentityResourceRepository
{
    private readonly IApplicationDbContext context;

    public IdentityResourceRepository(IApplicationDbContext context)
        : base(context)
    {
        this.context = context;
    }

    public async Task<IdentityResources> GetIdentityResourcesAsync(Guid identityResourceId, CancellationToken cancellationToken = default)
    {
        var identityResources = await context.IdentityResources
            .Include(ac => ac.IdentityClaims)
            .FirstOrDefaultAsync(res => res.Id == identityResourceId, cancellationToken);
        return identityResources;
    }

    public async Task<IdentityResources> GetIdentityResourcesAsync(string identityResourceName, CancellationToken cancellationToken = default)
    {
        var identityResources = await context.IdentityResources
            .Include(ac => ac.IdentityClaims)
            .FirstOrDefaultAsync(res => res.Name == identityResourceName, cancellationToken);
        return identityResources;
    }

    public async Task<IList<IdentityResourcesByScopesModel>> GetAllIdentityResourcesByScopesAsync(
        IList<string> requestedScopes, CancellationToken cancellationToken = default)
    {
        var identityResources = await (from idRes in context.IdentityResources.AsNoTracking()
            where requestedScopes.Contains(idRes.Name)
            join idResClaims in context.IdentityClaims.AsNoTracking()
                on idRes.Id equals idResClaims.IdentityResourceId into idModelJoin
            from idModel in idModelJoin.DefaultIfEmpty()
            select new IdentityResourcesByScopesModel
            {
                IdentityResourceId = idRes.Id,
                IdentityResourceName = idRes.Name,
                IdentityResourceClaimType = idModel.Type,
                IdentityResourceClaimAliasType = idModel.AliasType
            }).ToListAsync(cancellationToken);

        return identityResources;
    }
}
