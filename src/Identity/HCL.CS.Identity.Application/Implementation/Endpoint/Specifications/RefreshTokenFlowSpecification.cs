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
using HCL.CS.Service.Interfaces.Interfaces.Endpoint;

namespace HCL.CS.Service.Implementation.Endpoint.Specifications;

internal sealed class RefreshTokenFlowSpecification : BaseRequestModelValidator<ValidatedTokenRequestModel>
{
    internal RefreshTokenFlowSpecification(ITokenGenerationService tokenGenerationService)
    {
        Add("CheckRefreshToken", new Rule<ValidatedTokenRequestModel>(
            new IsRequestNull<ValidatedTokenRequestModel>(request =>
                request.GetValue(OpenIdConstants.TokenRequest.RefreshToken)),
            OpenIdConstants.Errors.InvalidRequest,
            EndpointErrorCodes.RefreshTokenMissing));

        Add("CheckTokenLengthRestrictions", new Rule<ValidatedTokenRequestModel>(
            new CheckLengthRestrictions<ValidatedTokenRequestModel>(
                request => request.GetValue(OpenIdConstants.TokenRequest.RefreshToken),
                request => request.TokenConfigOptions.InputLengthRestrictionsConfig.RefreshToken,
                request => ">"),
            OpenIdConstants.Errors.InvalidGrant,
            EndpointErrorCodes.RefreshTokenTooLong));

        Add("ValidateRefreshToken", new Rule<ValidatedTokenRequestModel>(
            new ValidateRefreshToken(tokenGenerationService),
            OpenIdConstants.Errors.InvalidGrant,
            EndpointErrorCodes.RefreshTokenValidationFailed));
    }
}

internal class ValidateRefreshToken : ISpecification<ValidatedTokenRequestModel>
{
    private readonly ITokenGenerationService tokenGenerationService;

    internal ValidateRefreshToken(ITokenGenerationService tokenGenerationService)
    {
        this.tokenGenerationService = tokenGenerationService;
    }

    public bool IsSatisfiedBy(ValidatedTokenRequestModel model)
    {
        var refreshTokenHandle = model.GetValue(OpenIdConstants.TokenRequest.RefreshToken);
        var result = tokenGenerationService.ValidateRefreshTokenAsync(refreshTokenHandle, model.Client).GetAwaiter()
            .GetResult();

        if (result.IsError) return false;

        model.RequestedRefreshToken = refreshTokenHandle;
        return true;
    }
}
