using System.Threading;
using HCL.CS.Domain;
using HCL.CS.Domain.Entities.Api;

namespace HCL.CS.DomainServices.Repository.Api;

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
