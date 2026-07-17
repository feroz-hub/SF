using System.Threading;
using Zentra.Domain;
using Zentra.Domain.Entities.Endpoint;

namespace Zentra.DomainServices.UnitOfWork.Endpoint;

public interface IClientsUnitOfWork
{
    IRepository<Clients> ClientRepository { get; }
    IRepository<ClientPostLogoutRedirectUris> PostLogoutRedirectUrisRepository { get; }
    IRepository<ClientRedirectUris> RedirectUrisRepository { get; }
    IRepository<SecurityTokens> SecurityTokensRepository { get; }
    Task<FrameworkResult> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<FrameworkResult> SaveChangesWithHardDeleteAsync(CancellationToken cancellationToken = default);
}
