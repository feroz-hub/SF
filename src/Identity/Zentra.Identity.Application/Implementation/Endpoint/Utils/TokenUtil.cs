using Zentra.Domain.Constants.Endpoint;
using Zentra.Domain.Entities.Endpoint;
using Zentra.DomainServices;
using Zentra.Service.Implementation.Endpoint.Extensions;

namespace Zentra.Service.Implementation.Endpoint.Utils;

internal class TokenUtil
{
    private readonly IRepository<SecurityTokens> securityTokenRepository;

    internal TokenUtil(IRepository<SecurityTokens> securityTokenRepository)
    {
        this.securityTokenRepository = securityTokenRepository;
    }

    internal async Task<bool> IsTokenRevoked(string token, string tokenType = null)
    {
        if (string.IsNullOrWhiteSpace(token)) return true;

        var tokenHash = token.ComputeSha256Hash();
        if (tokenType == OpenIdConstants.TokenResponseType.RefreshToken ||
            tokenType == OpenIdConstants.TokenType.RefreshToken)
        {
            var refreshToken = await securityTokenRepository.GetAsync(entity =>
                entity.TokenType == OpenIdConstants.TokenType.RefreshToken
                && entity.Key == tokenHash
                && !entity.TokenReuseDetected
                && entity.ConsumedAt == null
                && entity.ConsumedTime == null);
            return !refreshToken.ContainsAny();
        }

        if (tokenType == OpenIdConstants.TokenResponseType.AccessToken ||
            tokenType == OpenIdConstants.TokenType.AccessToken)
        {
            var accessToken = await securityTokenRepository.GetAsync(entity =>
                entity.TokenType == OpenIdConstants.TokenType.AccessToken
                && entity.TokenValue == token);
            return !accessToken.ContainsAny();
        }

        var tokenResult = await securityTokenRepository.GetAsync(entity =>
            (entity.TokenType == OpenIdConstants.TokenType.AccessToken && entity.TokenValue == token)
            || (entity.TokenType == OpenIdConstants.TokenType.RefreshToken
                && entity.Key == tokenHash
                && !entity.TokenReuseDetected
                && entity.ConsumedAt == null
                && entity.ConsumedTime == null));
        return !tokenResult.ContainsAny();
    }
}
