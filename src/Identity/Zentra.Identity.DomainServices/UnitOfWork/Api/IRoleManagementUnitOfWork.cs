using System.Threading;
using Zentra.Domain;
using Zentra.DomainServices.Repository.Api;

namespace Zentra.DomainServices.UnitOfWork.Api;

public interface IRoleManagementUnitOfWork
{
    IRoleRepository RoleRepository { get; }
    IRoleClaimsRepository RoleClaimsRepository { get; }
    Task SetAddedStatusAsync<T>(T entity);
    Task SetModifiedStatusAsync<T>(T entity, string concurrencyStamp);
    Task<FrameworkResult> SaveChangesAsync(CancellationToken cancellationToken = default);
}
