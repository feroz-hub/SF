using System.IdentityModel.Tokens.Jwt;
using AutoMapper;
using DomainValidation.Interfaces.Specification;
using DomainValidation.Validation;
using HCL.CS.Domain.Entities.Endpoint;
using HCL.CS.Domain.ErrorCodes;
using HCL.CS.Domain.Models.Endpoint;
using HCL.CS.Domain.Models.Endpoint.Request;
using HCL.CS.DomainServices;
using HCL.CS.DomainServices.UnitOfWork.Endpoint;
using HCL.CS.Service.Implementation.Endpoint.Extensions;
using HCL.CS.Service.Implementation.Endpoint.Utils;
using HCL.CS.Service.Implementation.Endpoint.Validators;
using static HCL.CS.Domain.Constants.Endpoint.OpenIdConstants;

namespace HCL.CS.Service.Implementation.Endpoint.Specifications;

internal sealed class IntrospectionRequestSpecification : BaseRequestModelValidator<ValidatedIntrospectionRequestModel>
{
    internal IntrospectionRequestSpecification(
        IClientsUnitOfWork unitOfWork,
        IRepository<Clients> clientRepository,
        IMapper mapper,
        Dictionary<string, AsymmetricKeyInfoModel> keyStore,
        IRepository<SecurityTokens> securityTokenRepository)
    {
        Add("CheckTokenNull", new Rule<ValidatedIntrospectionRequestModel>(
            new IsRequestNull<ValidatedIntrospectionRequestModel>(request =>
                request.GetValue(IntrospectionRequest.Token)),
            Errors.InvalidToken,
            EndpointErrorCodes.TokenMissing));

        Add("CheckLengthRestrictions", new Rule<ValidatedIntrospectionRequestModel>(
            new CheckLengthRestrictions<ValidatedIntrospectionRequestModel>(
                request => request.GetValue(IntrospectionRequest.Token),
                request => request.TokenConfigOptions.InputLengthRestrictionsConfig.Jwt,
                request => ">"),
            Errors.InvalidToken,
            EndpointErrorCodes.Invalid_Token_Length));

        Add("CheckIntrospectionTokenHintType", new Rule<ValidatedIntrospectionRequestModel>(
            new CheckIntrospectionTokenHintType(),
            Errors.InvalidRequest,
            EndpointErrorCodes.InvalidTokenHintType));

        Add("CheckTokenExpiry", new Rule<ValidatedIntrospectionRequestModel>(
            new CheckTokenExpiry(unitOfWork),
            Errors.InvalidRequest,
            EndpointErrorCodes.TokenExpired));

        Add("ValidateIntrospectionToken", new Rule<ValidatedIntrospectionRequestModel>(
            new ValidateIntrospectionToken(clientRepository, mapper, keyStore),
            Errors.InvalidRequest,
            EndpointErrorCodes.InvalidTokenInIntrospection));

        Add("CheckIsActiveToken", new Rule<ValidatedIntrospectionRequestModel>(
            new CheckIsActiveToken(securityTokenRepository),
            Errors.InvalidToken,
            EndpointErrorCodes.TokenRevoked));
    }
}

internal class CheckTokenExpiry : ISpecification<ValidatedIntrospectionRequestModel>
{
    private readonly IClientsUnitOfWork unitOfWork;

    internal CheckTokenExpiry(IClientsUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    public bool IsSatisfiedBy(ValidatedIntrospectionRequestModel model)
    {
        var token = model.GetValue(IntrospectionRequest.Token);
        if (model.TokenType == TokenResponseType.AccessToken || model.TokenType == "autodetect")
            try
            {
                var jwt = new JwtSecurityToken(token);
                model.TokenType = TokenResponseType.AccessToken;
                var expiryClaim = jwt.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Expiration);
                var creationDateClaim = jwt.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.IssuedAt);
                var scopeclaim = jwt.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Scope);
                if (expiryClaim == null || creationDateClaim == null) return false;

                var expiryDuration = model.Client.AccessTokenExpiration;
                var expiryTime = Convert.ToInt64(creationDateClaim.Value).ToDateTime().AddSeconds(expiryDuration);
                if (expiryTime > DateTime.UtcNow)
                {
                    model.ExpiresAt = Convert.ToInt64(expiryClaim.Value);
                    model.IsError = false;
                    model.Scopes = scopeclaim.Value;
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
            }

        if (model.TokenType == TokenType.RefreshToken || model.TokenType == "autodetect")
        {
            var tokenHash = token.ComputeSha256Hash();
            var securityTokenList = unitOfWork.SecurityTokensRepository.GetAsync(
                    entity => entity.Key == tokenHash && entity.TokenType == TokenType.RefreshToken,
                    x => new { x.CreationTime, x.ExpiresAt, x.ConsumedAt, x.ConsumedTime, x.TokenReuseDetected })
                .GetAwaiter().GetResult();
            if (!securityTokenList.ContainsAny()) return false;

            var securityToken = securityTokenList.FirstOrDefault();
            if (securityToken == null) return false;

            if (securityToken.TokenReuseDetected || securityToken.ConsumedAt.HasValue ||
                securityToken.ConsumedTime.HasValue) return false;

            model.TokenType = TokenResponseType.RefreshToken;
            var expiredTime = Convert.ToDateTime(securityToken.CreationTime).AddSeconds(securityToken.ExpiresAt);
            if (expiredTime > DateTime.UtcNow)
            {
                model.ExpiresAt = expiredTime.ToUnixTime();
                model.IssuedAt = securityToken.CreationTime.ToUnixTime();
                model.IsError = false;
                return true;
            }

            model.ExpiresAt = expiredTime.ToUnixTime();
            model.IssuedAt = securityToken.CreationTime.ToUnixTime();
            model.IsError = false;
        }

        model.IsError = true;
        return false;
    }
}

internal class ValidateIntrospectionToken : ISpecification<ValidatedIntrospectionRequestModel>
{
    private readonly IRepository<Clients> clientRepository;
    private readonly Dictionary<string, AsymmetricKeyInfoModel> keyStore;
    private readonly IMapper mapper;

    internal ValidateIntrospectionToken(
        IRepository<Clients> clientRepository,
        IMapper mapper,
        Dictionary<string, AsymmetricKeyInfoModel> keyStore)
    {
        this.clientRepository = clientRepository;
        this.mapper = mapper;
        this.keyStore = keyStore;
    }

    public bool IsSatisfiedBy(ValidatedIntrospectionRequestModel model)
    {
        var token = model.GetValue(IntrospectionRequest.Token);
        if (string.IsNullOrWhiteSpace(token)) return false;

        switch (model.TokenType)
        {
            case TokenResponseType.AccessToken:
            {
                try
                {
                    model = GetTokenKey(model, model.Client.ClientId).GetAwaiter().GetResult();

                    if (!model.IsError)
                    {
                        var jwtToken = new JwtSecurityToken(token);
                        if (model.Client.AllowedSigningAlgorithm.Trim() != jwtToken.SignatureAlgorithm.Trim())
                        {
                            model.IsError = true;
                            return false;
                        }

                        var expectedAudience = model.TokenConfigOptions.TokenConfig.ApiIdentifier;
                        if (string.IsNullOrWhiteSpace(expectedAudience)) return false;

                        var subject = jwtToken.Claims;
                        if (subject != null)
                        {
                            var subjectIdentifier = subject.FirstOrDefault(x => x.Type == ClaimTypes.Sub);
                            model.UserId = subjectIdentifier.Value;
                        }

                        if (model.Client.AllowedSigningAlgorithm == Algorithms.RsaSha256
                            || model.Client.AllowedSigningAlgorithm == Algorithms.EcdsaSha256)
                        {
                            (model.DecodedToken, _) = token.ValidateAsymmetricToken(
                                model.Key,
                                model.TokenConfigOptions.TokenConfig.IssuerUri,
                                expectedAudience);
                        }
                        else
                        {
                            model.IsError = true;
                            return false;
                        }

                        model.IsError = false;
                        return true;
                    }

                    break;
                }
                catch (Exception)
                {
                    model.IsError = true;
                    return false;
                }
            }

            case TokenResponseType.RefreshToken:
                model.IsError = false;
                return true;
        }

        return true;
    }

    public async Task<ValidatedIntrospectionRequestModel> GetTokenKey(ValidatedIntrospectionRequestModel result,
        string clientId)
    {
        var clientsEntity = await clientRepository.GetAsync(
            client => client.ClientId == clientId,
            new System.Linq.Expressions.Expression<Func<HCL.CS.Domain.Entities.Endpoint.Clients, object>>[] { x => x.RedirectUris, x => x.PostLogoutRedirectUris });
        var client = mapper.Map<Clients, ClientsModel>(clientsEntity[0]);
        result.IsError = false;
        if (!string.IsNullOrWhiteSpace(client.AllowedSigningAlgorithm))
        {
            if (client.AllowedSigningAlgorithm == Algorithms.RsaSha256
                || client.AllowedSigningAlgorithm == Algorithms.EcdsaSha256)
            {
                if (keyStore.Values.Count > 0)
                {
                    var signingCredentials =
                        keyStore.GetAsymmetricVerificationCredentials(client.AllowedSigningAlgorithm);
                    if (signingCredentials != null)
                    {
                        result.IsError = false;
                        result.Key = signingCredentials.Key;
                    }
                    else
                    {
                        result.ErrorCode = Errors.InvalidRequest;
                        result.IsError = true;
                    }
                }
                else
                {
                    result.ErrorCode = Errors.InvalidRequest;
                    result.IsError = true;
                }
            }
            else if (client.AllowedSigningAlgorithm.Trim() == "none")
            {
                result.ErrorCode = Errors.UnsupportedAlgorithm;
                result.IsError = true;
            }
            else
            {
                result.ErrorCode = Errors.UnsupportedAlgorithm;
                result.IsError = true;
            }
        }
        else
        {
            if (keyStore.Values.Count > 0)
            {
                var signingCredentials = keyStore.GetAsymmetricVerificationCredentials(Algorithms.RsaSha256);
                if (signingCredentials != null)
                {
                    result.Key = signingCredentials.Key;
                }
                else
                {
                    result.ErrorCode = Errors.InvalidRequest;
                    result.IsError = true;
                }
            }
            else
            {
                result.ErrorCode = Errors.InvalidRequest;
                result.IsError = true;
            }
        }

        return result;
    }
}

internal class CheckIntrospectionTokenHintType : ISpecification<ValidatedIntrospectionRequestModel>
{
    public bool IsSatisfiedBy(ValidatedIntrospectionRequestModel model)
    {
        var tokenHintType = model.GetValue(IntrospectionRequest.TokenHintType);
        if (!string.IsNullOrEmpty(tokenHintType))
        {
            var allowedHintTypes = typeof(TokenHintTypes).GetArray().ToList().ConvertAll(x => x.ToLower());
            if (allowedHintTypes.Contains(tokenHintType))
                model.TokenType = tokenHintType;
            else
                return false;
        }
        else
        {
            model.TokenType = "autodetect";
        }

        return true;
    }
}

internal class CheckIsActiveToken : ISpecification<ValidatedIntrospectionRequestModel>
{
    private readonly IRepository<SecurityTokens> securityTokenRepository;

    internal CheckIsActiveToken(IRepository<SecurityTokens> securityTokenRepository)
    {
        this.securityTokenRepository = securityTokenRepository;
    }

    public bool IsSatisfiedBy(ValidatedIntrospectionRequestModel model)
    {
        var token = model.GetValue(IntrospectionRequest.Token);
        if (new TokenUtil(securityTokenRepository).IsTokenRevoked(token, model.TokenType).GetAwaiter()
            .GetResult()) return false;

        return true;
    }
}
