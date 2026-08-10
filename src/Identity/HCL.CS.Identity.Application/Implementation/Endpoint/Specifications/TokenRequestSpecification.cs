/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

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
