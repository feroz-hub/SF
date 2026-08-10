/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace IntegrationTests.Endpoint.Helper;

public class JwksTestHelper : HclCsFakeSetup
{
    private Task<SecurityToken> ValidateSymmetricToken(string token, string issuer = null, string audience = null,
        string clientSecret = null)
    {
        var securityKey = Encoding.ASCII.GetBytes(clientSecret);
        var validationParameters = new TokenValidationParameters
        {
            ValidateAudience = audience != null,
            ValidateIssuer = issuer != null,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(securityKey),
            ValidateLifetime = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            LifetimeValidator = CustomLifetimeValidator,
            RequireExpirationTime = true
        };
        new JwtSecurityTokenHandler().ValidateToken(token, validationParameters, out var rawToken);

        return Task.FromResult(rawToken);
    }

    public bool CustomLifetimeValidator(DateTime? notBefore, DateTime? expires, SecurityToken tokenToValidate,
        TokenValidationParameters param)
    {
        if (expires != null) return expires > DateTime.UtcNow;

        return false;
    }

    public async Task<SecurityToken> ValidateToken(string token, string issuer = null, string audience = null,
        string clientSecret = null)
    {
        var jwt = new JwtSecurityToken(token);
        var algorithm = jwt.SignatureAlgorithm;
        if (algorithm.StartsWith("RS") || algorithm.StartsWith("ES") || algorithm.StartsWith("PS"))
            return await ValidateAsymmetricToken(token, issuer, audience);
        if (algorithm.StartsWith("HS"))
            return await ValidateSymmetricToken(token, issuer, audience, clientSecret);

        throw new SecurityTokenInvalidAlgorithmException(
            $"The test token uses unsupported signing algorithm '{algorithm}'.");
    }

    private async Task<SecurityToken> ValidateAsymmetricToken(string token, string issuer = null,
        string audience = null)
    {
        var result = await BackChannelClient.GetAsync(DiscoveryKeysEndpoint);
        result.EnsureSuccessStatusCode();
        var json = await result.Content.ReadAsStringAsync();
        var keys = new JsonWebKeySet(json).Keys;
        var validationParameters = new TokenValidationParameters
        {
            ValidateAudience = audience != null,
            ValidateIssuer = issuer != null,
            ValidateIssuerSigningKey = true,
            IssuerSigningKeys = keys,
            ValidateLifetime = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            LifetimeValidator = CustomLifetimeValidator,
            RequireExpirationTime = true
        };
        new JwtSecurityTokenHandler().ValidateToken(token, validationParameters, out var rawToken);
        return rawToken;
    }
}
