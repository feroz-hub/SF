/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using HCL.CS.Domain.Constants.Endpoint;
using HCL.CS.Domain.Entities.Endpoint;
using HCL.CS.DomainServices;
using HCL.CS.Service.Implementation.Endpoint.Extensions;

namespace HCL.CS.Service.Implementation.Endpoint.Utils;

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
