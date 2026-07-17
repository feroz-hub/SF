using System.Threading;
using Zentra.Domain;
using Zentra.Domain.Entities.Api;

namespace Zentra.DomainServices.Repository.Api;

public interface IUserRoleRepository
{
    Task InsertAsync(UserRoles entity);
    Task UpdateAsync(UserRoles entity);
    Task DeleteAsync(UserRoles entity);
    Task DeleteAsync(IList<UserRoles> entityList);
    Task<UserRoles> GetUserRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);
    Task<IList<UserRoles>> GetUserRoleAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<FrameworkResult> SaveChangesAsync(CancellationToken cancellationToken = default);
}
