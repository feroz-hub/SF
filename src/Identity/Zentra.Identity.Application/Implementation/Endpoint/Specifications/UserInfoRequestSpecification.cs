using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using DomainValidation.Interfaces.Specification;
using DomainValidation.Validation;
using Zentra.Domain;
using Zentra.Domain.Constants.Endpoint;
using Zentra.Domain.Entities.Api;
using Zentra.Domain.Entities.Endpoint;
using Zentra.Domain.ErrorCodes;
using Zentra.Domain.Models.Endpoint;
using Zentra.Domain.Models.Endpoint.Request;
using Zentra.DomainServices;
using Zentra.DomainServices.Wrappers;
using Zentra.Service.Extension;
using Zentra.Service.Implementation.Endpoint.Extensions;
using Zentra.Service.Implementation.Endpoint.Utils;
using Zentra.Service.Implementation.Endpoint.Validators;

namespace Zentra.Service.Implementation.Endpoint.Specifications;

internal sealed class UserInfoRequestSpecification : BaseRequestModelValidator<ValidatedUserInfoRequestModel>
{
    internal UserInfoRequestSpecification(
        IRepository<Clients> clientRepository,
        IMapper mapper,
        Dictionary<string, AsymmetricKeyInfoModel> keyStore,
        UserManagerWrapper<Users> userManager,
        TokenSettings configSettings,
        IRepository<SecurityTokens> securityTokenRepository)
    {
        Add(
            "ValidateTokenFormat", new Rule<ValidatedUserInfoRequestModel>(
                new ValidateTokenFormat(),
                OpenIdConstants.Errors.InvalidFormat,
                EndpointErrorCodes.InvalidTokenFormat));

        Add(
            "ValidateTokenLength",
            new Rule<ValidatedUserInfoRequestModel>(
                new ValidateTokenLength(configSettings),
                OpenIdConstants.Errors.InvalidToken,
                EndpointErrorCodes.TokenMaxLengthExceeded));

        Add(
            "ValidateIsActiveToken",
            new Rule<ValidatedUserInfoRequestModel>(
                new ValidateIsActiveToken(securityTokenRepository),
                OpenIdConstants.Errors.InvalidToken,
                EndpointErrorCodes.TokenRevoked));

        Add(
            "ValidateToken",
            new Rule<ValidatedUserInfoRequestModel>(
                new ValidateToken(clientRepository, mapper, keyStore),
                OpenIdConstants.Errors.InvalidRequest,
                EndpointErrorCodes.TokenIsNullOrInvalid));

        Add(
            "ValidateClient",
            new Rule<ValidatedUserInfoRequestModel>(
                new ValidateClient(clientRepository),
                OpenIdConstants.Errors.InvalidClient,
                EndpointErrorCodes.ClientDoesNotExist));

        Add(
            "ValidateUserClaims",
            new Rule<ValidatedUserInfoRequestModel>(
                new ValidateUserClaims(clientRepository, userManager),
                OpenIdConstants.Errors.InvalidToken,
                EndpointErrorCodes.InvalidUserClaims));

        Add(
            "ValidateScopeClaims",
            new Rule<ValidatedUserInfoRequestModel>(
                new ValidateScopeClaims(),
                OpenIdConstants.Errors.InsufficientScope,
                EndpointErrorCodes.InvalidScopeClaims));
    }
}

internal class ValidateTokenFormat : ISpecification<ValidatedUserInfoRequestModel>
{
    public bool IsSatisfiedBy(ValidatedUserInfoRequestModel model)
    {
        if (!model.Token.Contains(".")) return false;

        return true;
    }
}

internal class ValidateTokenLength : ISpecification<ValidatedUserInfoRequestModel>
{
    private readonly TokenSettings tokenSettings;

    internal ValidateTokenLength(TokenSettings tokenConfig)
    {
        tokenSettings = tokenConfig;
    }

    public bool IsSatisfiedBy(ValidatedUserInfoRequestModel model)
    {
        if (model.Token.Length > tokenSettings.InputLengthRestrictionsConfig.Jwt) return false;

        return true;
    }
}

internal class ValidateIsActiveToken : ISpecification<ValidatedUserInfoRequestModel>
{
    private readonly IRepository<SecurityTokens> securityTokenRepository;

    internal ValidateIsActiveToken(IRepository<SecurityTokens> securityTokenRepository)
    {
        this.securityTokenRepository = securityTokenRepository;
    }

    public bool IsSatisfiedBy(ValidatedUserInfoRequestModel model)
    {
        if (new TokenUtil(securityTokenRepository).IsTokenRevoked(model.Token, OpenIdConstants.TokenType.AccessToken)
            .GetAwaiter().GetResult()) return false;

        return true;
    }
}

internal class ValidateToken : ISpecification<ValidatedUserInfoRequestModel>
{
    private readonly IRepository<Clients> clientRepository;
    private readonly Dictionary<string, AsymmetricKeyInfoModel> keyStore;
    private readonly IMapper mapper;

    internal ValidateToken(
        IRepository<Clients> clientRepository,
        IMapper mapper,
        Dictionary<string, AsymmetricKeyInfoModel> keyStore)
    {
        this.clientRepository = clientRepository;
        this.mapper = mapper;
        this.keyStore = keyStore;
    }

    public bool IsSatisfiedBy(ValidatedUserInfoRequestModel model)
    {
        var token = model.Token;
        if (!string.IsNullOrWhiteSpace(token))
        {
            var claims = new JwtSecurityToken(token).Claims;
            if (claims == null) return false;

            string clientId = null;
            var client = claims.Where(x => x.Type == OpenIdConstants.ClaimTypes.ClientId).ToList();
            if (client.ContainsAny()) clientId = client[0].Value;

            if (clientId == null) return false;

            var signinKeys = GetTokenKey(clientId).GetAwaiter().GetResult();
            if (!signinKeys.IsError)
            {
                try
                {
                    var expectedAudience = model.TokenConfigOptions.TokenConfig.ApiIdentifier;
                    if (string.IsNullOrWhiteSpace(expectedAudience)) return false;

                    model.Client = signinKeys.Client;
                    model.Key = signinKeys.Key;
                    ClaimsPrincipal sub = null;
                    if (model.Client.AllowedSigningAlgorithm == OpenIdConstants.Algorithms.RsaSha256
                        || model.Client.AllowedSigningAlgorithm == OpenIdConstants.Algorithms.EcdsaSha256)
                    {
                        (model.DecodedToken, sub) = token.ValidateAsymmetricToken(
                            model.Key,
                            model.TokenConfigOptions.TokenConfig.IssuerUri,
                            expectedAudience);
                    }
                    else
                    {
                        model.IsError = true;
                        return false;
                    }
                }
                catch (Exception)
                {
                    model.IsError = true;
                    return false;
                }

                model.IsError = false;
                return true;
            }

            model.ErrorCode = signinKeys.ErrorCode;
            return false;
        }

        return false;
    }

    public async Task<ValidatedUserInfoRequestModel> GetTokenKey(string clientId)
    {
        var result = new ValidatedUserInfoRequestModel();
        var clientsEntity = await clientRepository.GetAsync(
            client => client.ClientId == clientId,
            new System.Linq.Expressions.Expression<Func<Zentra.Domain.Entities.Endpoint.Clients, object>>[] { x => x.RedirectUris, x => x.PostLogoutRedirectUris });

        if (clientsEntity.ContainsAny())
        {
            result.Client = mapper.Map<Clients, ClientsModel>(clientsEntity[0]);
        }
        else
        {
            result.ErrorCode = OpenIdConstants.Errors.InvalidClient;
            result.IsError = true;
            return result;
        }

        var client = result.Client;
        if (!string.IsNullOrWhiteSpace(client.AllowedSigningAlgorithm))
        {
            if (client.AllowedSigningAlgorithm == OpenIdConstants.Algorithms.RsaSha256
                || client.AllowedSigningAlgorithm == OpenIdConstants.Algorithms.EcdsaSha256)
            {
                if (keyStore.Values.Count > 0)
                {
                    //var signingCredentials = keyStore.GetAsymmetricCredentials(client.AllowedSigningAlgorithm);
                    var signingCredentials =
                        keyStore.GetAsymmetricVerificationCredentials(client.AllowedSigningAlgorithm);
                    if (signingCredentials != null)
                    {
                        result.IsError = false;
                        result.Key = signingCredentials.Key;
                    }
                    else
                    {
                        result.ErrorCode = OpenIdConstants.Errors.InvalidRequest;
                    }
                }
                else
                {
                    result.ErrorCode = OpenIdConstants.Errors.InvalidRequest;
                }
            }
            else
            {
                result.ErrorCode = OpenIdConstants.Errors.UnsupportedAlgorithm;
            }
        }
        else
        {
            if (keyStore.Values.Count > 0)
            {
                var signingCredentials =
                    keyStore.GetAsymmetricVerificationCredentials(OpenIdConstants.Algorithms.RsaSha256);
                if (signingCredentials != null)
                    result.Key = signingCredentials.Key;
                else
                    result.ErrorCode = OpenIdConstants.Errors.InvalidRequest;
            }
            else
            {
                result.ErrorCode = OpenIdConstants.Errors.InvalidRequest;
            }
        }

        result.IsError = !string.IsNullOrWhiteSpace(result.ErrorCode) || result.Key == null;
        return result;
    }
}

internal class ValidateClient : ISpecification<ValidatedUserInfoRequestModel>
{
    private readonly IRepository<Clients> clientRepository;

    internal ValidateClient(
        IRepository<Clients> clientRepository)
    {
        this.clientRepository = clientRepository;
    }

    public bool IsSatisfiedBy(ValidatedUserInfoRequestModel model)
    {
        ClientsModel client = null;
        var accessTokenClaims = model.DecodedToken.Claims.ToList();
        var clientId = accessTokenClaims.FirstOrDefault(c => c.Type == OpenIdConstants.ClaimTypes.ClientId);
        if (clientId != null)
        {
            var clientIsExist = clientRepository.ActiveRecordExistsAsync(x => x.ClientId == clientId.Value).GetAwaiter()
                .GetResult();
            if (!clientIsExist) return false;
        }

        var claims = accessTokenClaims;
        var scopes = claims.Where(c => c.Type == OpenIdConstants.ClaimTypes.Scope).ToArray();
        if (scopes.ContainsAny())
            foreach (var scope in scopes)
                if (scope.Value.Contains(" "))
                {
                    claims.Remove(scope);
                    var values = scope.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    foreach (var value in values) claims.Add(new Claim(OpenIdConstants.ClaimTypes.Scope, value));
                }

        model.Claims = claims;
        model.Client = client;

        return true;
    }
}

internal class ValidateUserClaims : ISpecification<ValidatedUserInfoRequestModel>
{
    private readonly IRepository<Clients> clientRepository;
    private readonly UserManagerWrapper<Users> userManager;

    internal ValidateUserClaims(
        IRepository<Clients> clientRepository,
        UserManagerWrapper<Users> userManager)
    {
        this.clientRepository = clientRepository;
        this.userManager = userManager;
    }

    public bool IsSatisfiedBy(ValidatedUserInfoRequestModel model)
    {
        // make sure user is still active (if sub claim is present)
        var userClaim = model.Claims.FirstOrDefault(c => c.Type == OpenIdConstants.ClaimTypes.Sub);
        if (userClaim != null && userClaim.Value.IsGuid())
        {
            var user = userManager.FindByIdAsync(userClaim.Value).GetAwaiter().GetResult();
            if (user == null)
            {
                model.Claims = null;
                return false;
            }
        }

        return true;
    }
}

internal class ValidateScopeClaims : ISpecification<ValidatedUserInfoRequestModel>
{
    public bool IsSatisfiedBy(ValidatedUserInfoRequestModel model)
    {
        // check expected scope(s)
        var scope = model.Claims.FirstOrDefault(c =>
            c.Type == OpenIdConstants.ClaimTypes.Scope && c.Value == AuthenticationConstants.IdentityScopes.OpenId);
        if (scope == null) return false;

        var claims = model.Claims.Where(x => !AuthenticationConstants.AccessTokenFilters.ClaimsFilter.Contains(x.Type));
        var claimsIdentity = new ClaimsIdentity(claims.ToArray(), "UserInfo", OpenIdConstants.ClaimTypes.Name,
            OpenIdConstants.ClaimTypes.Role);
        model.Subject = new ClaimsPrincipal(claimsIdentity);

        return true;
    }
}
