using System.Threading;
using HCL.CS.Domain;
using HCL.CS.Domain.Entities.Api;
using HCL.CS.Domain.Models.Api;

namespace HCL.CS.DomainServices.Repository.Api;

public interface IAuditRepository
{
    Task InsertAsync(AuditTrail entity, CancellationToken cancellationToken = default);

    Task<int> GetTotalCountAsync(CancellationToken cancellationToken = default);

    Task<int> GetFilteredCountAsync(AuditSearchRequestModel auditSearchModule, CancellationToken cancellationToken = default);

    Task<FrameworkResult> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<IList<AuditTrail>> GetAuditDetailsAsync(AuditSearchRequestModel auditSearchModule, CancellationToken cancellationToken = default);
}
