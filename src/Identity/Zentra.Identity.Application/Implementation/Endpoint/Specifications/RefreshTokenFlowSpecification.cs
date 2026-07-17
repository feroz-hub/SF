using DomainValidation.Interfaces.Specification;
using DomainValidation.Validation;
using Zentra.Domain.Constants.Endpoint;
using Zentra.Domain.ErrorCodes;
using Zentra.Domain.Models.Endpoint.Request;
using Zentra.Service.Implementation.Endpoint.Extensions;
using Zentra.Service.Implementation.Endpoint.Validators;
using Zentra.Service.Interfaces.Interfaces.Endpoint;

namespace Zentra.Service.Implementation.Endpoint.Specifications;

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
