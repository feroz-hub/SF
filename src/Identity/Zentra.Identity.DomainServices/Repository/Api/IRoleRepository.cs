using System.Threading;
using Zentra.Domain;
using Zentra.Domain.Entities.Api;

namespace Zentra.DomainServices.Repository.Api;

public interface IRoleRepository
{
    Task UpdateAsync(Roles entity);
    Task DeleteAsync(Roles entity);
    Task<IList<Roles>> GetAllRolesAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameIncludingDeletedAsync(string roleName, CancellationToken cancellationToken = default);
    Task<FrameworkResult> SaveChangesAsync(CancellationToken cancellationToken = default);
}
