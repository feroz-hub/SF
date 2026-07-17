using System.Threading;
using Microsoft.EntityFrameworkCore;
using HCL.CS.Domain.Enums;
using HCL.CS.Domain.Models.Api;
using HCL.CS.Domain.Models.Endpoint;
using HCL.CS.DomainServices;
using HCL.CS.DomainServices.Repository.Api;
using static HCL.CS.Domain.Constants.Endpoint.OpenIdConstants;

namespace HCL.CS.Infrastructure.Data.Repository.Api;

internal class SecurityTokenRepository : ISecurityTokenRepository
{
    private readonly IApplicationDbContext context;

    public SecurityTokenRepository(IApplicationDbContext context)
    {
        this.context = context;
    }

    public async Task<IList<TokenModel>> GetSecurityTokenAsync(PagingModel page,
        SecurityTokenOption option,
        DateTime? fromdate = null,
        DateTime? todate = null,
        IList<string> clientIds = null,
        IList<string> userIds = null,
        CancellationToken cancellationToken = default)
    {
        var recordsToSkip = 0;
        IQueryable<TokenModel> result = null;
        var now = DateTime.UtcNow;
        if (page != null)
        {
            page.TotalItems = await context.SecurityTokens.CountAsync(cancellationToken);
            page.CurrentPage = page.CurrentPage == 0 ? 1 : page.CurrentPage;
            recordsToSkip = (page.CurrentPage - 1) * page.ItemsPerPage;
        }

        switch (option)
        {
            case SecurityTokenOption.Client:
                result = (from token in context.SecurityTokens.AsNoTracking()
                        join client in context.Clients.AsNoTracking() on token.ClientId equals client.ClientId
                        join user in context.Users.AsNoTracking() on token.SubjectId equals user.Id.ToString()
                        where clientIds.Contains(token.ClientId) &&
                              !string.IsNullOrWhiteSpace(token.SubjectId) &&
                              token.CreationTime.AddSeconds(token.ExpiresAt) > now &&
                              (token.TokenType == TokenType.RefreshToken || token.TokenType == TokenType.AccessToken)
                        select new TokenModel
                        {
                            ClientId = token.ClientId,
                            ClientName = client.ClientName,
                            Token = token.TokenType == TokenType.RefreshToken ? token.Key : token.TokenValue,
                            TokenTypeHint = token.TokenType == TokenType.RefreshToken
                                ? TokenType.RefreshToken
                                : TokenType.AccessToken,
                            UserName = user.UserName,
                            LoginDateTime = user.LastLoginDateTime
                        })
                    .OrderBy(s => s.ClientName);
                break;
            case SecurityTokenOption.User:
                result = (from token in context.SecurityTokens.AsNoTracking()
                        join client in context.Clients.AsNoTracking() on token.ClientId equals client.ClientId
                        join user in context.Users.AsNoTracking() on token.SubjectId equals user.Id.ToString()
                        where userIds.Contains(token.SubjectId) &&
                              !string.IsNullOrWhiteSpace(token.SubjectId) &&
                              token.CreationTime.AddSeconds(token.ExpiresAt) > now &&
                              (token.TokenType == TokenType.RefreshToken || token.TokenType == TokenType.AccessToken)
                        select new TokenModel
                        {
                            ClientId = token.ClientId,
                            ClientName = client.ClientName,
                            Token = token.TokenType == TokenType.RefreshToken ? token.Key : token.TokenValue,
                            TokenTypeHint = token.TokenType == TokenType.RefreshToken
                                ? TokenType.RefreshToken
                                : TokenType.AccessToken,
                            UserName = user.UserName,
                            LoginDateTime = user.LastLoginDateTime
                        })
                    .OrderBy(s => s.ClientName);
                break;
            case SecurityTokenOption.BetweenDates:
                result = (from token in context.SecurityTokens.AsNoTracking()
                        join client in context.Clients.AsNoTracking() on token.ClientId equals client.ClientId
                        join user in context.Users.AsNoTracking() on token.SubjectId equals user.Id.ToString()
                        where token.CreationTime >= fromdate && token.CreationTime <= todate &&
                              !string.IsNullOrWhiteSpace(token.SubjectId) &&
                              token.CreationTime.AddSeconds(token.ExpiresAt) > now &&
                              (token.TokenType == TokenType.RefreshToken || token.TokenType == TokenType.AccessToken)
                        select new TokenModel
                        {
                            ClientId = token.ClientId,
                            ClientName = client.ClientName,
                            Token = token.TokenType == TokenType.RefreshToken ? token.Key : token.TokenValue,
                            TokenTypeHint = token.TokenType == TokenType.RefreshToken
                                ? TokenType.RefreshToken
                                : TokenType.AccessToken,
                            UserName = user.UserName,
                            LoginDateTime = user.LastLoginDateTime
                        })
                    .OrderBy(s => s.ClientName);
                break;
            case SecurityTokenOption.All:
                result = (from token in context.SecurityTokens.AsNoTracking()
                        join client in context.Clients.AsNoTracking() on token.ClientId equals client.ClientId
                        join user in context.Users.AsNoTracking() on token.SubjectId equals user.Id.ToString()
                        where token.CreationTime >= fromdate && token.CreationTime <= todate &&
                              !string.IsNullOrWhiteSpace(token.SubjectId) &&
                              token.CreationTime.AddSeconds(token.ExpiresAt) > now &&
                              (token.TokenType == TokenType.RefreshToken || token.TokenType == TokenType.AccessToken)
                        select new TokenModel
                        {
                            ClientId = token.ClientId,
                            ClientName = client.ClientName,
                            Token = token.TokenType == TokenType.RefreshToken ? token.Key : token.TokenValue,
                            TokenTypeHint = token.TokenType == TokenType.RefreshToken
                                ? TokenType.RefreshToken
                                : TokenType.AccessToken,
                            UserName = user.UserName,
                            LoginDateTime = user.LastLoginDateTime
                        })
                    .OrderBy(s => s.ClientName);
                break;
        }

        if (page != null) return await result.Skip(recordsToSkip).Take(page.ItemsPerPage).ToListAsync(cancellationToken);

        return await result.ToListAsync(cancellationToken);
    }
}
