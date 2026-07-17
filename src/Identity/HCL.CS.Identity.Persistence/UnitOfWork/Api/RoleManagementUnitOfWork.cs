using HCL.CS.Domain;
using HCL.CS.DomainServices;
using HCL.CS.DomainServices.Repository.Api;
using HCL.CS.DomainServices.UnitOfWork.Api;
using HCL.CS.Infrastructure.Data.Repository.Api;

namespace HCL.CS.Infrastructure.Data.UnitOfWork.Api;

internal class RoleManagementUnitOfWork : BaseDispose, IRoleManagementUnitOfWork
{
    public readonly IApplicationDbContext context;
    private IRoleClaimsRepository roleClaimsRepository;
    private IRoleRepository roleRepository;

    public RoleManagementUnitOfWork(IApplicationDbContext context)
    {
        this.context = context;
    }

    public IRoleRepository RoleRepository
    {
        get
        {
            if (roleRepository != null) return roleRepository;
            roleRepository = new RoleRepository(context);
            return roleRepository;
        }
    }

    public IRoleClaimsRepository RoleClaimsRepository
    {
        get
        {
            if (roleClaimsRepository != null) return roleClaimsRepository;
            roleClaimsRepository = new RoleClaimsRepository(context);
            return roleClaimsRepository;
        }
    }

    public Task SetAddedStatusAsync<T>(T entity)
    {
        context.SetAddedStatus(entity);
        return Task.CompletedTask;
    }

    public Task SetModifiedStatusAsync<T>(T entity, string concurrencyStamp)
    {
        context.SetConcurrencyStatus(entity, concurrencyStamp);
        return Task.CompletedTask;
    }

    public async Task<FrameworkResult> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }
}
