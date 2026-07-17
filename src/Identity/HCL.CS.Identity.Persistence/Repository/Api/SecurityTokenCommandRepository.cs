using System.Threading;
using Microsoft.EntityFrameworkCore;
using HCL.CS.Domain.Constants.Endpoint;
using HCL.CS.DomainServices;
using HCL.CS.DomainServices.Repository.Api;

namespace HCL.CS.Infrastructure.Data.Repository.Api;

internal class SecurityTokenCommandRepository : ISecurityTokenCommandRepository
{
    private readonly IApplicationDbContext context;

    public SecurityTokenCommandRepository(IApplicationDbContext context)
    {
        this.context = context;
    }

    public async Task<int> ConsumeAuthorizationCodeAsync(Guid id, DateTime consumedAt, CancellationToken cancellationToken = default)
    {
        return await context.SecurityTokens
            .Where(entity => entity.Id == id
                             && entity.TokenType == OpenIdConstants.TokenType.AuthorizationCode
                             && entity.ConsumedAt == null
                             && entity.ConsumedTime == null)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(entity => entity.ConsumedAt, consumedAt)
                .SetProperty(entity => entity.ConsumedTime, consumedAt), cancellationToken);
    }

    public async Task<int> ConsumeRefreshTokenAsync(Guid id, DateTime consumedAt, CancellationToken cancellationToken = default)
    {
        return await context.SecurityTokens
            .Where(entity => entity.Id == id
                             && entity.TokenType == OpenIdConstants.TokenType.RefreshToken
                             && entity.ConsumedAt == null
                             && entity.ConsumedTime == null
                             && !entity.TokenReuseDetected)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(entity => entity.ConsumedAt, consumedAt)
                .SetProperty(entity => entity.ConsumedTime, consumedAt)
                .SetProperty(entity => entity.TokenReuseDetected, false), cancellationToken);
    }

    public async Task<int> ConsumeActiveRefreshTokensAsync(string subjectId, string clientId, DateTime consumedAt, CancellationToken cancellationToken = default)
    {
        return await context.SecurityTokens
            .Where(entity => entity.SubjectId == subjectId
                             && entity.ClientId == clientId
                             && entity.TokenType == OpenIdConstants.TokenType.RefreshToken
                             && entity.ConsumedAt == null
                             && entity.ConsumedTime == null
                             && !entity.TokenReuseDetected)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(entity => entity.ConsumedAt, consumedAt)
                .SetProperty(entity => entity.ConsumedTime, consumedAt), cancellationToken);
    }
}
