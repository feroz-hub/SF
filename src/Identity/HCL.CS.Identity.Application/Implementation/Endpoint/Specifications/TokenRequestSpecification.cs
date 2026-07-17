using DomainValidation.Interfaces.Specification;
using DomainValidation.Validation;
using HCL.CS.Domain.Constants.Endpoint;
using HCL.CS.Domain.ErrorCodes;
using HCL.CS.Domain.Models.Endpoint.Request;
using HCL.CS.Service.Implementation.Endpoint.Extensions;
using HCL.CS.Service.Implementation.Endpoint.Validators;

namespace HCL.CS.Service.Implementation.Endpoint.Specifications;

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
