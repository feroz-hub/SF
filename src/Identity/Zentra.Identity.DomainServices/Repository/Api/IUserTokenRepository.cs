using System.Threading;
using Zentra.Domain;
using Zentra.Domain.Entities.Api;

namespace Zentra.DomainServices.Repository.Api;

public interface IUserTokenRepository
{
    Task<IList<UserTokens>> GetUserTokenAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IList<UserTokens>> GetUserTokenAsync(Guid userId, string name, string loginProvider, CancellationToken cancellationToken = default);
    Task DeleteAsync(IList<UserTokens> entityList);
    Task<FrameworkResult> SaveChangesAsync(CancellationToken cancellationToken = default);
}
