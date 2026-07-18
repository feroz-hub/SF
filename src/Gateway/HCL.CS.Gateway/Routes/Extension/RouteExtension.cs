/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using HCL.CS.Domain.Constants.Endpoint;
using HCL.CS.Domain.Entities.Endpoint;
using HCL.CS.Domain.Models.Endpoint;

namespace HCL.CS.ProxyService.Routes.Extension;

internal static class RouteExtension
{
    private const string InvalidCertificate = "Invalid certificate / No certificate found.";
    private const string UnsupportedAlgorithm = "Algorithm not supported.";
    private const string InvalidCertificateRsa = "Invalid Certificate - No RSA private key found.";
    private const string InvalidCertificatEcdsa = "Invalid Certificate - No ECDSA private key found.";
    private static readonly JsonSerializerOptions ResponseSerializerOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        MaxDepth = 0,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        IgnoreReadOnlyProperties = true
    };

    internal static async Task WriteResponseJsonAsync(this HttpResponse response, object content,
        string contentType = null)
    {
        var json = JsonSerializer.Serialize(content, ResponseSerializerOptions);

        response.ContentType = contentType ?? "application/json; charset=UTF-8";
        await response.WriteAsync(json);
    }

    internal static string JsonSerialize<T>(T request)
    {
        return JsonSerializer.Serialize(request, SerializerOptions);
    }

    internal static T JsonDeserialize<T>(this string request)
    {
        return JsonSerializer.Deserialize<T>(request, SerializerOptions);
    }

    internal static SigningCredentials GetSymmetricCredentials(this string clientSecret, string algorithm)
    {
        SigningCredentials credentials = null;
        if (!string.IsNullOrWhiteSpace(algorithm))
        {
            if (algorithm.StartsWith("HS"))
            {
                if (!string.IsNullOrWhiteSpace(clientSecret))
                {
                    var securityKey = Encoding.ASCII.GetBytes(clientSecret);
                    credentials = new SigningCredentials(new SymmetricSecurityKey(securityKey), algorithm);
                }
                else
                {
                    throw new ArgumentNullException(nameof(clientSecret), "No client secret found");
                }
            }
            else
            {
                throw new ArgumentNullException(nameof(algorithm), "Invalid Algorithm");
            }
        }

        return credentials;
    }

    internal static SigningCredentials GetAsymmetricVerificationCredentials(
        this Dictionary<string, AsymmetricKeyInfoModel> keyStore, string algorithm)
    {
        SigningCredentials credentials = null;
        if (!string.IsNullOrWhiteSpace(algorithm))
        {
            if (keyStore.Values.Count > 0)
            {
                var certificate = keyStore[algorithm].Certificate;
                if (certificate == null) throw new InvalidOperationException(InvalidCertificate);

                credentials = certificate.GenerateAsymmetricVerificationCredentials(algorithm);
                if (credentials?.Key != null && !string.IsNullOrWhiteSpace(keyStore[algorithm].KeyId))
                    credentials.Key.KeyId = keyStore[algorithm].KeyId;
            }
            else
            {
                throw
                    new InvalidOperationException(
                        UnsupportedAlgorithm); // TODO: Talk to team on correcting this message text.
            }
        }

        return credentials;
    }

    internal static SigningCredentials GenerateAsymmetricVerificationCredentials(this X509Certificate2 certificate,
        string algorithm)
    {
        SecurityKey securityKey = null;
        if (string.Equals(algorithm, OpenIdConstants.Algorithms.RsaSha256, StringComparison.Ordinal))
        {
            if (certificate != null)
            {
                var rsa = certificate.GetRSAPublicKey();
                if (rsa == null) throw new InvalidOperationException(InvalidCertificateRsa);

                securityKey = new X509SecurityKey(certificate);
            }
        }
        else if (string.Equals(algorithm, OpenIdConstants.Algorithms.EcdsaSha256, StringComparison.Ordinal))
        {
            if (certificate != null)
            {
                var ecdsa = certificate.GetECDsaPublicKey();
                if (ecdsa == null) throw new InvalidOperationException(InvalidCertificatEcdsa);

                securityKey = new X509SecurityKey(certificate);
            }
        }
        else
        {
            throw new InvalidOperationException(UnsupportedAlgorithm);
        }

        var signingCredentials = new SigningCredentials(securityKey, algorithm);
        return signingCredentials;
    }

    internal static SecurityKey GetTokenKey(this Dictionary<string, AsymmetricKeyInfoModel> keyStore, Clients client)
    {
        if (!string.IsNullOrWhiteSpace(client.AllowedSigningAlgorithm))
        {
            if (string.Equals(client.AllowedSigningAlgorithm, OpenIdConstants.Algorithms.RsaSha256,
                    StringComparison.Ordinal)
                || string.Equals(client.AllowedSigningAlgorithm, OpenIdConstants.Algorithms.EcdsaSha256,
                    StringComparison.Ordinal))
            {
                if (keyStore.Values.Count <= 0) return null;

                var signingCredentials = keyStore.GetAsymmetricVerificationCredentials(client.AllowedSigningAlgorithm);
                if (signingCredentials != null) return signingCredentials.Key;

                return null;
            }
        }
        else
        {
            if (keyStore.Values.Count > 0)
            {
                var signingCredentials =
                    keyStore.GetAsymmetricVerificationCredentials(OpenIdConstants.Algorithms.RsaSha256);
                if (signingCredentials != null) return signingCredentials.Key;
            }
        }

        return null;
    }

    internal static (JwtSecurityToken, ClaimsPrincipal) ValidateAsymmetricToken(this string token,
        SecurityKey signingKey, string issuer, string audience)
    {
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = signingKey,
            ValidateLifetime = true,
            RequireExpirationTime = true,
            LifetimeValidator = CustomLifetimeValidator,
            CryptoProviderFactory = new CryptoProviderFactory
            {
                CacheSignatureProviders = false
            }
        };

        var principal =
            new JwtSecurityTokenHandler().ValidateToken(token, validationParameters, out var rawValidatedToken);
        return ((JwtSecurityToken)rawValidatedToken, principal);
    }

    internal static (JwtSecurityToken, ClaimsPrincipal) ValidateSymmetricJwtToken(this string token, string key,
        string issuer, string audience)
    {
        var securityKey = Encoding.ASCII.GetBytes(key);
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(securityKey),
            ValidateLifetime = true,
            RequireExpirationTime = true,
            LifetimeValidator = CustomLifetimeValidator,
            CryptoProviderFactory = new CryptoProviderFactory
            {
                CacheSignatureProviders = false
            }
        };
        var principal =
            new JwtSecurityTokenHandler().ValidateToken(token, validationParameters, out var rawValidatedToken);
        return ((JwtSecurityToken)rawValidatedToken, principal);
    }

    internal static bool CustomLifetimeValidator(DateTime? notBefore, DateTime? expires, SecurityToken tokenToValidate,
        TokenValidationParameters param)
    {
        if (expires != null) return expires > DateTime.UtcNow;

        return false;
    }

    internal static string ConvertSpaceSeparatedString(this IEnumerable<string> list)
    {
        if (list == null) return string.Empty;

        var sb = new StringBuilder(100);
        foreach (var element in list) sb.Append(element + " ");

        return sb.ToString().Trim();
    }
}
