using System.Threading;
using Zentra.Domain;
using Zentra.Domain.Entities.Api;
using Zentra.Domain.Models.Api;

namespace Zentra.DomainServices.Repository.Api;

public interface IAuditRepository
{
    Task InsertAsync(AuditTrail entity, CancellationToken cancellationToken = default);

    Task<int> GetTotalCountAsync(CancellationToken cancellationToken = default);

    Task<int> GetFilteredCountAsync(AuditSearchRequestModel auditSearchModule, CancellationToken cancellationToken = default);

    Task<FrameworkResult> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<IList<AuditTrail>> GetAuditDetailsAsync(AuditSearchRequestModel auditSearchModule, CancellationToken cancellationToken = default);
}
