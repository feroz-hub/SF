using HCL.CS.Domain;
using HCL.CS.Domain.Entities.Api;
using HCL.CS.DomainServices;
using HCL.CS.DomainServices.UnitOfWork.Api;

namespace HCL.CS.Infrastructure.Data.UnitOfWork.Api;

internal class ResourceUnitOfWork : BaseDispose, IResourceUnitOfWork
{
    private readonly IApplicationDbContext context;
    private IRepository<ApiResourceClaims> apiClaimsRepository;
    private IRepository<ApiResources> apiResourcesRepository;
    private IRepository<ApiScopeClaims> apiScopeClaimsRepository;
    private IRepository<ApiScopes> apiScopesRepository;
    private IRepository<IdentityClaims> identityClaimsRepository;

    private IRepository<IdentityResources> identityResourcesRepository;

    public ResourceUnitOfWork(IApplicationDbContext context)
    {
        this.context = context;
    }

    public IRepository<IdentityResources> IdentityResourcesRepository
    {
        get
        {
            if (identityResourcesRepository != null) return identityResourcesRepository;
            identityResourcesRepository = new BaseRepository<IdentityResources>(context);
            return identityResourcesRepository;
        }
    }

    public IRepository<IdentityClaims> IdentityClaimsRepository
    {
        get
        {
            if (identityClaimsRepository != null) return identityClaimsRepository;
            identityClaimsRepository = new BaseRepository<IdentityClaims>(context);
            return identityClaimsRepository;
        }
    }

    public IRepository<ApiResources> ApiResourcesRepository
    {
        get
        {
            if (apiResourcesRepository != null) return apiResourcesRepository;
            apiResourcesRepository = new BaseRepository<ApiResources>(context);
            return apiResourcesRepository;
        }
    }

    public IRepository<ApiResourceClaims> ApiResourceClaimsRepository
    {
        get
        {
            if (apiClaimsRepository != null) return apiClaimsRepository;
            apiClaimsRepository = new BaseRepository<ApiResourceClaims>(context);
            return apiClaimsRepository;
        }
    }

    public IRepository<ApiScopes> ApiScopesRepository
    {
        get
        {
            if (apiScopesRepository != null) return apiScopesRepository;
            apiScopesRepository = new BaseRepository<ApiScopes>(context);
            return apiScopesRepository;
        }
    }

    public IRepository<ApiScopeClaims> ApiScopeClaimsRepository
    {
        get
        {
            if (apiScopeClaimsRepository != null) return apiScopeClaimsRepository;
            apiScopeClaimsRepository = new BaseRepository<ApiScopeClaims>(context);
            return apiScopeClaimsRepository;
        }
    }

    public async Task<FrameworkResult> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<FrameworkResult> SaveChangesWithHardDeleteAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesWithHardDeleteAsync(cancellationToken);
    }
}
