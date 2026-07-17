using DomainValidation.Interfaces.Specification;
using DomainValidation.Validation;
using HCL.CS.Domain.Entities.Endpoint;
using HCL.CS.Domain.ErrorCodes;
using HCL.CS.Domain.Models.Endpoint.Request;
using HCL.CS.DomainServices;
using HCL.CS.Service.Implementation.Endpoint.Extensions;
using HCL.CS.Service.Implementation.Endpoint.Validators;
using static HCL.CS.Domain.Constants.Endpoint.OpenIdConstants;

namespace HCL.CS.Service.Implementation.Endpoint.Specifications;

internal sealed class TokenRevocationRequestSpecification : BaseRequestModelValidator<ValidatedRevocationRequestModel>
{
    internal TokenRevocationRequestSpecification(IRepository<SecurityTokens> securityTokenRepository)
    {
        Add("CheckTokenIsPresent", new Rule<ValidatedRevocationRequestModel>(
            new CheckTokenIsPresent(),
            Errors.InvalidRequest,
            EndpointErrorCodes.NoTokenFound));

        Add("CheckTokenTypeHint", new Rule<ValidatedRevocationRequestModel>(
            new CheckTokenTypeHint(),
            Errors.UnsupportedTokenType,
            EndpointErrorCodes.UnsupportedTokenType));

        Add("CheckTokenAndRevoke", new Rule<ValidatedRevocationRequestModel>(
            new CheckTokenAndRevoke(securityTokenRepository),
            Errors.InvalidRequest,
            EndpointErrorCodes.InvalidRevocationRequest));
    }
}

internal class CheckTokenIsPresent : ISpecification<ValidatedRevocationRequestModel>
{
    public bool IsSatisfiedBy(ValidatedRevocationRequestModel model)
    {
        var token = model.GetValue(RevocationRequest.Token);
        if (!string.IsNullOrWhiteSpace(token))
        {
            model.Token = token;
            return true;
        }

        return false;
    }
}

internal class CheckTokenTypeHint : ISpecification<ValidatedRevocationRequestModel>
{
    public bool IsSatisfiedBy(ValidatedRevocationRequestModel model)
    {
        var tokenTypeHint = model.GetValue(RevocationRequest.TokenTypeHint);
        if (!string.IsNullOrWhiteSpace(tokenTypeHint))
        {
            if (tokenTypeHint == TokenType.RefreshToken || tokenTypeHint == TokenType.AccessToken)
            {
                model.TokenTypeHint = tokenTypeHint;
                return true;
            }

            return false;
        }

        return true;
    }
}

internal class CheckTokenAndRevoke : ISpecification<ValidatedRevocationRequestModel>
{
    private readonly IRepository<SecurityTokens> securityTokenRepository;

    public CheckTokenAndRevoke(IRepository<SecurityTokens> securityTokenRepository)
    {
        this.securityTokenRepository = securityTokenRepository;
    }

    public bool IsSatisfiedBy(ValidatedRevocationRequestModel model)
    {
        var token = model.GetValue(RevocationRequest.Token);
        var tokenTypeHint = model.GetValue(RevocationRequest.TokenTypeHint);
        try
        {
            IList<SecurityTokens> tokenList = null;
            var refreshTokenHash = token.ComputeSha256Hash();
            if (tokenTypeHint == TokenType.RefreshToken)
                tokenList = securityTokenRepository
                    .GetAsync(entity => entity.Key == refreshTokenHash && entity.TokenType == TokenType.RefreshToken)
                    .GetAwaiter().GetResult();
            else if (tokenTypeHint == TokenType.AccessToken)
                tokenList = securityTokenRepository
                    .GetAsync(entity => entity.TokenValue == token && entity.TokenType == TokenType.AccessToken)
                    .GetAwaiter().GetResult();
            else
                tokenList = securityTokenRepository
                    .GetAsync(entity =>
                        (entity.TokenType == TokenType.RefreshToken && entity.Key == refreshTokenHash)
                        || (entity.TokenType == TokenType.AccessToken && entity.TokenValue == token))
                    .GetAwaiter().GetResult();

            if (!tokenList.ContainsAny())
                // RFC 7009: idempotent success for unknown tokens.
                return true;

            var ownedTokenList = tokenList
                .Where(x => x.ClientId == model.ClientId)
                .ToList();

            if (!ownedTokenList.ContainsAny())
                // RFC 7009: do not reveal whether token exists for another client.
                return true;

            securityTokenRepository.DeleteAsync(ownedTokenList).GetAwaiter().GetResult();
            securityTokenRepository.SaveChangesWithHardDeleteAsync().GetAwaiter().GetResult();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
