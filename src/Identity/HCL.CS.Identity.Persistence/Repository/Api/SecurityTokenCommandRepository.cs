/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

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
