using System.Threading;
using Microsoft.EntityFrameworkCore;
using Zentra.Domain;
using Zentra.Domain.Entities.Api;
using Zentra.DomainServices;
using Zentra.DomainServices.Repository.Api;

namespace Zentra.Infrastructure.Data.Repository.Api;

internal class RoleRepository : BaseDispose, IRoleRepository
{
    private readonly IApplicationDbContext context;

    public RoleRepository(IApplicationDbContext context)
    {
        this.context = context;
    }

    public Task UpdateAsync(Roles entity)
    {
        context.Roles.Attach(entity);
        context.SetModifiedStatus(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Roles entity)
    {
        context.Roles.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<IList<Roles>> GetAllRolesAsync(CancellationToken cancellationToken = default)
    {
        var roles = await context.Roles.AsNoTracking().ToListAsync(cancellationToken);
        return roles;
    }

    public async Task<bool> ExistsByNameIncludingDeletedAsync(string roleName, CancellationToken cancellationToken = default)
    {
        return await context.Roles
            .IgnoreQueryFilters()
            .AnyAsync(x => x.Name == roleName, cancellationToken);
    }

    public async Task<FrameworkResult> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }
}
