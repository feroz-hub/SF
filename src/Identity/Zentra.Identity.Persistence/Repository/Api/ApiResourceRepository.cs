using System.Threading;
using Microsoft.EntityFrameworkCore;
using Zentra.Domain.Entities.Api;
using Zentra.Domain.Models.Api;
using Zentra.DomainServices;
using Zentra.DomainServices.Repository.Api;

namespace Zentra.Infrastructure.Data.Repository.Api;

internal class ApiResourceRepository : BaseRepository<ApiResources>, IApiResourceRepository
{
    private readonly IApplicationDbContext context;

    public ApiResourceRepository(IApplicationDbContext context)
        : base(context)
    {
        this.context = context;
    }

    public async Task<ApiResources> GetApiResourceForUpdateAsync(Guid apiResourceId, CancellationToken cancellationToken = default)
    {
        return await context.ApiResources
            .FirstOrDefaultAsync(apires => apires.Id == apiResourceId, cancellationToken);
    }

    public async Task<ApiResources> GetApiResourcesAsync(Guid apiResourceId, CancellationToken cancellationToken = default)
    {
        var apiResources = await context.ApiResources
            .Include(ac => ac.ApiResourceClaims)
            .Include(asc => asc.ApiScopes)
            .ThenInclude(asc => asc.ApiScopeClaims)
            .AsSplitQuery()
            .FirstOrDefaultAsync(apires => apires.Id == apiResourceId, cancellationToken);
        return apiResources;
    }

    public async Task<ApiResources> GetApiResourcesAsync(string apiResourceName, CancellationToken cancellationToken = default)
    {
        var apiResources = await context.ApiResources
            .Include(ac => ac.ApiResourceClaims)
            .Include(asc => asc.ApiScopes)
            .ThenInclude(asc => asc.ApiScopeClaims)
            .AsSplitQuery()
            .FirstOrDefaultAsync(api => api.Name == apiResourceName, cancellationToken);
        return apiResources;
    }

    public async Task<IList<ApiResources>> GetAllApiResourcesAsync(CancellationToken cancellationToken = default)
    {
        var apiResources = await context.ApiResources
            .Include(ac => ac.ApiResourceClaims)
            .Include(asc => asc.ApiScopes)
            .ThenInclude(asc => asc.ApiScopeClaims)
            .AsSplitQuery()
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        return apiResources;
    }

    public async Task<IList<ApiScopes>> GetAllApiScopesAsync(CancellationToken cancellationToken = default)
    {
        var apiScopes = await context.ApiScopes
            .Include(ac => ac.ApiScopeClaims)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        return apiScopes;
    }

    public async Task<IList<ApiResourcesByScopesModel>> GetAllApiResourcesByScopesAsync(IList<string> requestedScopes, CancellationToken cancellationToken = default)
    {
        var apiResources = await (from apiRes in context.ApiResources.AsNoTracking()
            join apiResClaims in context.ApiResourceClaims.AsNoTracking()
                on apiRes.Id equals apiResClaims.ApiResourceId into apiResClaimJoin
            from apiResModel in apiResClaimJoin.DefaultIfEmpty()
            join apiScopes in context.ApiScopes.AsNoTracking()
                on apiRes.Id equals apiScopes.ApiResourceId into apiScopesJoin
            from apiScopeModel in apiScopesJoin.DefaultIfEmpty()
            where requestedScopes.Contains(apiScopeModel.Name) || requestedScopes.Contains(apiRes.Name)
            join apiScopeClaims in context.ApiScopeClaims.AsNoTracking()
                on apiScopeModel.Id equals apiScopeClaims.ApiScopeId into apiScopeClaimJoin
            from apiModel in apiScopeClaimJoin.DefaultIfEmpty()
            select new ApiResourcesByScopesModel
            {
                ApiResourceId = apiRes.Id,
                ApiResourceName = apiRes.Name,
                ApiResourceClaimType = apiResModel.Type,
                ApiScopeName = apiScopeModel.Name,
                ApiScopeClaimType = apiModel.Type
            }).ToListAsync(cancellationToken);

        return apiResources;
    }
}
