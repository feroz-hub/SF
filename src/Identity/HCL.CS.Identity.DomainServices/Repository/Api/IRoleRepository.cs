using System.Threading;
using HCL.CS.Domain;
using HCL.CS.Domain.Entities.Api;

namespace HCL.CS.DomainServices.Repository.Api;

public interface IRoleRepository
{
    Task UpdateAsync(Roles entity);
    Task DeleteAsync(Roles entity);
    Task<IList<Roles>> GetAllRolesAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameIncludingDeletedAsync(string roleName, CancellationToken cancellationToken = default);
    Task<FrameworkResult> SaveChangesAsync(CancellationToken cancellationToken = default);
}
