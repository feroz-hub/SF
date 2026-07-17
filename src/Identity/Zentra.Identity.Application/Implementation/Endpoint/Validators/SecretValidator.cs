using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Zentra.Domain;
using Zentra.Domain.Constants;
using Zentra.Domain.Constants.Endpoint;
using Zentra.Domain.Models.Endpoint;
using Zentra.DomainServices.Infra;
using Zentra.Service.Implementation.Endpoint.Extensions;
using Zentra.Service.Interfaces.Interfaces.Endpoint.Validators;

// TODO: PrivateKeyJwtSecretValidator, X509NameSecretValidator, X509ThumbprintSecretValidator need to be added
namespace Zentra.Service.Implementation.Endpoint.Validators;

internal class SecretValidator : ISecretValidator
{
    private readonly IFrameworkResultService frameworkResultService;
    private readonly HttpContext httpContext;
    private readonly Dictionary<string, AsymmetricKeyInfoModel> keyStore;
    private readonly ILoggerService loggerService;

    private readonly TokenSettings tokenSettings;
    //private readonly ITokenReplayCache tokenReplayCache;


    public SecretValidator(IHttpContextAccessor httpContextAccessor, ILoggerInstance instance,
        IFrameworkResultService frameworkResultService, Dictionary<string, AsymmetricKeyInfoModel> keyStore,
        ZentraConfig config)
    {
        httpContext = httpContextAccessor.HttpContext;
        this.frameworkResultService = frameworkResultService;
        loggerService = instance.GetLoggerInstance(LoggerKeyConstants.DefaultLoggerKey);
        this.keyStore = keyStore;
        tokenSettings = config.TokenSettings;
    }

    public async Task<bool> ValidateSecretAsync(ClientsModel client, ParsedSecretModel parsedSecret)
    {
        var expiredSecrets = client.ClientSecretExpiresAt < DateTime.UtcNow;
        if (expiredSecrets)
        {
            loggerService.WriteTo(Log.Error, "Expired secret");
            return false;
        }

        if (parsedSecret.Type == AuthenticationConstants.ParsedTypes.SharedSecret)
            return await CompareClientSecret(client.ClientSecret, parsedSecret);

        if (parsedSecret.Type == AuthenticationConstants.ParsedTypes.JwtBearer)
            return await CompareJwtSecret(client, parsedSecret);

        loggerService.WriteTo(Log.Error, "Secret validators could not validate secret.");
        return false;
    }

    internal Task<bool> CompareClientSecret(string secret, ParsedSecretModel parsedSecret)
    {
        var sharedSecret = Convert.ToString(parsedSecret.Credential);
        if (string.IsNullOrWhiteSpace(secret) || string.IsNullOrWhiteSpace(sharedSecret))
            return Task.FromResult(false);

        var secretSha256 = sharedSecret.Sha256();
        var secretSha512 = sharedSecret.Sha512();
        var isValid = secret.CompareStrings(sharedSecret)
                      || secret.CompareStrings(secretSha256)
                      || secret.CompareStrings(secretSha512);

        if (!isValid) loggerService.WriteTo(Log.Debug, "No matching secret found.");
        return Task.FromResult(isValid);
    }

    internal Task<bool> CompareJwtSecret(ClientsModel client, ParsedSecretModel parsedSecret)
    {
        var token = parsedSecret.Credential.ToString();

        try
        {
            var tokenKey = keyStore.GetTokenKey(client);

            if (tokenKey != null)
            {
                var jwt = new JwtSecurityToken(token);

                var audience = httpContext.GetZentraBaseUrl().IncludeEndSlash() +
                               OpenIdConstants.EndpointRoutePaths.Token;
                if (jwt.SignatureAlgorithm.StartsWith("HS"))
                {
                    var (decodedToken, _) =
                        token.ValidateSymmetricJwtToken(client.ClientSecret, client.ClientId, audience);
                    return Task.FromResult(true);
                }

                if (jwt.SignatureAlgorithm.StartsWith("RS") || jwt.SignatureAlgorithm.StartsWith("PS"))
                {
                    var securityKeys = new List<SecurityKey>();
                    foreach (var keyvalue in keyStore)
                    {
                        var key = keyvalue.Value.Certificate
                            .GenerateAsymmetricVerificationCredentials(keyvalue.Key);
                        securityKeys.Add(key.Key);
                    }

                    var tokenValidationParameters = new TokenValidationParameters
                    {
                        IssuerSigningKeys = securityKeys,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = tokenSettings.TokenConfig.IssuerUri,
                        ValidateIssuer = true,

                        ValidAudience = audience,
                        ValidateAudience = true,

                        RequireSignedTokens = true,
                        RequireExpirationTime = true,

                        ClockSkew = TimeSpan.FromMinutes(5)
                    };

                    var handler = new JwtSecurityTokenHandler();
                    handler.ValidateToken(token, tokenValidationParameters, out var validatedtoken);
                    var jwtToken = (JwtSecurityToken)validatedtoken;

                    if (jwtToken.Subject != jwtToken.Issuer)
                    {
                        loggerService.WriteTo(Log.Error,
                            "Both 'sub' and 'iss' in the client assertion token must have a value of client_id.");
                        return Task.FromResult(false);
                    }

                    var exp = jwtToken.Payload.Exp;
                    if (!exp.HasValue)
                    {
                        loggerService.WriteTo(Log.Error, "exp is missing in client assertion JWT.");
                        return Task.FromResult(false);
                    }

                    var jti = jwtToken.Payload.Jti;
                    if (jti.IsNull())
                    {
                        loggerService.WriteTo(Log.Error, "jti is missing in client assertion JWT.");
                        return Task.FromResult(false);
                    }

                    return Task.FromResult(true);
                }

                return Task.FromResult(false);
            }
        }
        catch (Exception)
        {
            return Task.FromResult(false);
        }

        return Task.FromResult(false);
    }
}
