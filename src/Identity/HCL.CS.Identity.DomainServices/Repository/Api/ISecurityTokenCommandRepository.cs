using System.Threading;

namespace HCL.CS.DomainServices.Repository.Api;

public interface ISecurityTokenCommandRepository
{
    Task<int> ConsumeAuthorizationCodeAsync(Guid id, DateTime consumedAt, CancellationToken cancellationToken = default);

    Task<int> ConsumeRefreshTokenAsync(Guid id, DateTime consumedAt, CancellationToken cancellationToken = default);

    Task<int> ConsumeActiveRefreshTokensAsync(string subjectId, string clientId, DateTime consumedAt, CancellationToken cancellationToken = default);
}
