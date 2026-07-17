using System.Threading;
using HCL.CS.Domain;
using HCL.CS.Domain.Entities.Api;

namespace HCL.CS.DomainServices.Repository.Api;

public interface IUserRepository
{
    Task DeleteAsync(Users entity);
    Task UpdateAsync(Users entity, string[] affectedProperties);
    Task<IList<Users>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken = default);
    Task<Users?> FindByUserNameIncludingDeletedAsync(string userName, CancellationToken cancellationToken = default);
    Task SetAddedStatusAsync<T>(T entity);
    Task SetModifiedStatusAsync<T>(T entity, string concurrencyStamp);
    Task<FrameworkResult> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task EnableIdentityAutoSaveChanges();
    Task DisableIdentityAutoSaveChanges();
    Task<IList<Users>> GetAllUsersAsync(CancellationToken cancellationToken = default);
}
