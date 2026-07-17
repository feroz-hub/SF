using System.Threading;
using HCL.CS.Domain;
using HCL.CS.DomainServices.Repository.Api;

namespace HCL.CS.DomainServices.UnitOfWork.Api;

public interface IRoleManagementUnitOfWork
{
    IRoleRepository RoleRepository { get; }
    IRoleClaimsRepository RoleClaimsRepository { get; }
    Task SetAddedStatusAsync<T>(T entity);
    Task SetModifiedStatusAsync<T>(T entity, string concurrencyStamp);
    Task<FrameworkResult> SaveChangesAsync(CancellationToken cancellationToken = default);
}
