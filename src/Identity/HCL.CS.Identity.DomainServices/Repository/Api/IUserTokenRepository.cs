using System.Threading;
using HCL.CS.Domain;
using HCL.CS.Domain.Entities.Api;

namespace HCL.CS.DomainServices.Repository.Api;

public interface IUserTokenRepository
{
    Task<IList<UserTokens>> GetUserTokenAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IList<UserTokens>> GetUserTokenAsync(Guid userId, string name, string loginProvider, CancellationToken cancellationToken = default);
    Task DeleteAsync(IList<UserTokens> entityList);
    Task<FrameworkResult> SaveChangesAsync(CancellationToken cancellationToken = default);
}
