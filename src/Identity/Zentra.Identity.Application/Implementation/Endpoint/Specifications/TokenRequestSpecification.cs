using DomainValidation.Interfaces.Specification;
using DomainValidation.Validation;
using Zentra.Domain.Constants.Endpoint;
using Zentra.Domain.ErrorCodes;
using Zentra.Domain.Models.Endpoint.Request;
using Zentra.Service.Implementation.Endpoint.Extensions;
using Zentra.Service.Implementation.Endpoint.Validators;

namespace Zentra.Service.Implementation.Endpoint.Specifications;

internal sealed class TokenRequestSpecification : BaseRequestModelValidator<ValidatedTokenRequestModel>
{
    internal TokenRequestSpecification()
    {
        Add("CheckGrantType", new Rule<ValidatedTokenRequestModel>(
            new CheckGrantType(),
            OpenIdConstants.Errors.UnsupportedGrantType,
            EndpointErrorCodes.GrantTypeIsMissing));

        Add("CheckLengthRestrictions", new Rule<ValidatedTokenRequestModel>(
            new CheckLengthRestrictions<ValidatedTokenRequestModel>(
                request => request.GetValue(OpenIdConstants.TokenRequest.GrantType),
                request => request.TokenConfigOptions.InputLengthRestrictionsConfig.GrantType,
                request => ">"),
            OpenIdConstants.Errors.UnsupportedGrantType,
            EndpointErrorCodes.GrantTypeTooLong));

        Add("CheckAlgorithm", new Rule<ValidatedTokenRequestModel>(
            new CheckAlgorithm(),
            OpenIdConstants.Errors.UnsupportedAlgorithm,
            EndpointErrorCodes.SigningAlgorithmIsInvalid));
    }
}

internal class CheckGrantType : ISpecification<ValidatedTokenRequestModel>
{
    public bool IsSatisfiedBy(ValidatedTokenRequestModel model)
    {
        var grantType = model.GetValue(OpenIdConstants.TokenRequest.GrantType);

        if (string.IsNullOrWhiteSpace(grantType)) return false;

        var allowedGrantTypes = typeof(OpenIdConstants.GrantTypes).GetArray().ToList().ConvertAll(x => x.ToLower());
        if (!allowedGrantTypes.Contains(grantType.ToLower())) return false;

        return true;
    }
}

internal class CheckAlgorithm : ISpecification<ValidatedTokenRequestModel>
{
    public bool IsSatisfiedBy(ValidatedTokenRequestModel model)
    {
        var algorithm = model.Client.AllowedSigningAlgorithm;
        if (string.IsNullOrWhiteSpace(algorithm)) return false;

        var allowedSigningAlgorithms = new[]
        {
            OpenIdConstants.Algorithms.RsaSha256,
            OpenIdConstants.Algorithms.EcdsaSha256
        };

        if (!allowedSigningAlgorithms.Contains(algorithm, StringComparer.Ordinal)) return false;

        return true;
    }
}
