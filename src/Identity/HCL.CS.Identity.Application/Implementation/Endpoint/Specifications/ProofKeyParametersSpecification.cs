/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using System.Text;
using System.Text.RegularExpressions;
using DomainValidation.Interfaces.Specification;
using DomainValidation.Validation;
using HCL.CS.Domain.Constants.Endpoint;
using HCL.CS.Domain.ErrorCodes;
using HCL.CS.Domain.Models.Endpoint.Request;
using HCL.CS.Service.Implementation.Endpoint.Extensions;
using HCL.CS.Service.Implementation.Endpoint.Validators;

namespace HCL.CS.Service.Implementation.Endpoint.Specifications;

internal sealed class ProofKeyParametersSpecification : BaseRequestModelValidator<ValidatedTokenRequestModel>
{
    internal ProofKeyParametersSpecification()
    {
        Add("CheckCodeChallenge", new Rule<ValidatedTokenRequestModel>(
            new IsRequestNull<ValidatedTokenRequestModel>(request => request.AuthorizationCode.CodeChallenge),
            OpenIdConstants.Errors.InvalidGrant,
            EndpointErrorCodes.ClientMissingCodeChallenge));

        Add("CheckCodeChallengeMethod", new Rule<ValidatedTokenRequestModel>(
            new IsRequestNull<ValidatedTokenRequestModel>(request => request.AuthorizationCode.CodeChallengeMethod),
            OpenIdConstants.Errors.InvalidGrant,
            EndpointErrorCodes.ClientMissingCodeChallengeMethod));

        Add("CheckCodeVerifier", new Rule<ValidatedTokenRequestModel>(
            new IsRequestNull<ValidatedTokenRequestModel>(request =>
                request.GetValue(OpenIdConstants.TokenRequest.CodeVerifier)),
            OpenIdConstants.Errors.InvalidGrant,
            EndpointErrorCodes.CodeVerifierMissing));

        Add("CheckCodeVerifierLength<", new Rule<ValidatedTokenRequestModel>(
            new CheckLengthRestrictions<ValidatedTokenRequestModel>(
                request => request.GetValue(OpenIdConstants.TokenRequest.CodeVerifier),
                request => request.TokenConfigOptions.InputLengthRestrictionsConfig.CodeVerifierMinLength,
                request => "<"), OpenIdConstants.Errors.InvalidGrant,
            EndpointErrorCodes.CodeVerifierTooShort));
        Add("CheckCodeVerifierLength>", new Rule<ValidatedTokenRequestModel>(
            new CheckLengthRestrictions<ValidatedTokenRequestModel>(
                request => request.GetValue(OpenIdConstants.TokenRequest.CodeVerifier),
                request => request.TokenConfigOptions.InputLengthRestrictionsConfig.CodeVerifierMaxLength,
                request => ">"), OpenIdConstants.Errors.InvalidGrant,
            EndpointErrorCodes.CodeVerifierTooLong));

        Add("CheckCodeVerifierCharset", new Rule<ValidatedTokenRequestModel>(
            new CheckCodeVerifierCharset(),
            OpenIdConstants.Errors.InvalidGrant,
            EndpointErrorCodes.InvalidCodeVerifier));

        Add("CheckSupportedCodeChallengeMethod", new Rule<ValidatedTokenRequestModel>(
            new CheckSupportedCodeChallengeMethod(),
            OpenIdConstants.Errors.InvalidGrant,
            EndpointErrorCodes.UnsupportedCodeChallengeMethod));

        Add("CheckCodeChallengeFormat", new Rule<ValidatedTokenRequestModel>(
            new CheckCodeChallengeFormat(),
            OpenIdConstants.Errors.InvalidGrant,
            EndpointErrorCodes.InvalidCodeChallenge));

        Add("CheckCodeVerifierAgainstCodeChallenge", new Rule<ValidatedTokenRequestModel>(
            new CheckCodeVerifierAgainstCodeChallenge(),
            OpenIdConstants.Errors.InvalidGrant,
            EndpointErrorCodes.UnsupportedCodeChallengeMethod));
    }
}

internal class CheckSupportedCodeChallengeMethod : ISpecification<ValidatedTokenRequestModel>
{
    public bool IsSatisfiedBy(ValidatedTokenRequestModel model)
    {
        if (model.AuthorizationCode != null
            && model.AuthorizationCode.CodeChallengeMethod == OpenIdConstants.CodeChallengeMethods.Sha256)
            return true;

        return false;
    }
}

internal class CheckCodeChallengeFormat : ISpecification<ValidatedTokenRequestModel>
{
    private static readonly Regex Base64UrlPattern =
        new("^[A-Za-z0-9_-]+$", RegexOptions.CultureInvariant | RegexOptions.Compiled);

    public bool IsSatisfiedBy(ValidatedTokenRequestModel model)
    {
        var challenge = model.AuthorizationCode?.CodeChallenge;
        return !string.IsNullOrWhiteSpace(challenge) && Base64UrlPattern.IsMatch(challenge);
    }
}

internal class CheckCodeVerifierCharset : ISpecification<ValidatedTokenRequestModel>
{
    private static readonly Regex CodeVerifierPattern =
        new("^[A-Za-z0-9\\-._~]+$", RegexOptions.CultureInvariant | RegexOptions.Compiled);

    public bool IsSatisfiedBy(ValidatedTokenRequestModel model)
    {
        var verifier = model.GetValue(OpenIdConstants.TokenRequest.CodeVerifier);
        if (string.IsNullOrWhiteSpace(verifier)) return false;

        return CodeVerifierPattern.IsMatch(verifier);
    }
}

internal class CheckCodeVerifierAgainstCodeChallenge : ISpecification<ValidatedTokenRequestModel>
{
    public bool IsSatisfiedBy(ValidatedTokenRequestModel model)
    {
        if (model.AuthorizationCode != null)
        {
            var codeVerifierBytes = Encoding.ASCII.GetBytes(model.CodeVerifier);
            var hashedBytes = codeVerifierBytes.Sha256();
            var transformedCodeVerifier = hashedBytes.Encode();

            return transformedCodeVerifier.CompareStrings(model.AuthorizationCode.CodeChallenge);
        }

        return true;
    }
}
