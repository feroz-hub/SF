using System.Threading;
using Microsoft.EntityFrameworkCore;
using Zentra.Domain.Entities.Api;
using Zentra.Domain.Models.Api;
using Zentra.DomainServices;
using Zentra.DomainServices.Repository.Api;

namespace Zentra.Infrastructure.Data.Repository.Api;

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
