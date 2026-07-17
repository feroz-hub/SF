using System.Security.Claims;
using DomainValidation.Interfaces.Specification;
using DomainValidation.Validation;
using HCL.CS.Domain.Constants.Endpoint;
using HCL.CS.Domain.Entities.Api;
using HCL.CS.Domain.ErrorCodes;
using HCL.CS.Domain.Models.Endpoint;
using HCL.CS.Domain.Models.Endpoint.Request;
using HCL.CS.Domain.Models.Endpoint.Validation;
using HCL.CS.DomainServices.Wrappers;
using HCL.CS.Service.Implementation.Endpoint.Extensions;
using HCL.CS.Service.Implementation.Endpoint.Validators;
using HCL.CS.Service.Interfaces.Interfaces.Api;
using HCL.CS.Service.Interfaces.Interfaces.Endpoint;
using HCL.CS.Service.Interfaces.Interfaces.Endpoint.Validators;
using SystemClaimTypes = System.Security.Claims.ClaimTypes;

namespace HCL.CS.Service.Implementation.Endpoint.Specifications;

internal sealed class UserCodeFlowSpecification : BaseRequestModelValidator<ValidatedTokenRequestModel>
{
    internal UserCodeFlowSpecification(
        IResourceScopeValidator resourceScopeValidator,
        IAuthorizationService authorizationService,
        UserManagerWrapper<Users> userManager)
    {
        Add("CheckClientAuthorizedForUserCodeGrant", new Rule<ValidatedTokenRequestModel>(
            new CheckClientAuthorizedForGrantType<ValidatedTokenRequestModel>(new List<string>
            {
                AuthenticationConstants.GrantType.UserCode
            }),
            OpenIdConstants.Errors.UnauthorizedClient,
            EndpointErrorCodes.ClientNotAuthorizedForGrantType));

        Add("CheckUserCodePresent", new Rule<ValidatedTokenRequestModel>(
            new IsRequestNull<ValidatedTokenRequestModel>(request =>
                request.GetValue(OpenIdConstants.TokenRequest.UserCode)),
            OpenIdConstants.Errors.InvalidGrant,
            EndpointErrorCodes.InvalidSecurityTokenId));

        Add("ValidateUserCodeAndResolveUser", new Rule<ValidatedTokenRequestModel>(
            new ValidateUserCodeAndResolveUser(authorizationService, userManager),
            OpenIdConstants.Errors.InvalidGrant,
            EndpointErrorCodes.InvalidSecurityTokenId));

        Add("ValidateRequestedUserCodeScopes", new Rule<ValidatedTokenRequestModel>(
            new ValidateRequestedRopScopes(resourceScopeValidator),
            OpenIdConstants.Errors.InvalidScope,
            EndpointErrorCodes.InvalidScopeOrNotAllowed));
    }
}

internal class ValidateUserCodeAndResolveUser : ISpecification<ValidatedTokenRequestModel>
{
    private readonly IAuthorizationService authorizationService;
    private readonly UserManagerWrapper<Users> userManager;

    internal ValidateUserCodeAndResolveUser(
        IAuthorizationService authorizationService,
        UserManagerWrapper<Users> userManager)
    {
        this.authorizationService = authorizationService;
        this.userManager = userManager;
    }

    public bool IsSatisfiedBy(ValidatedTokenRequestModel model)
    {
        var userCode = model.GetValue(OpenIdConstants.TokenRequest.UserCode);
        if (string.IsNullOrWhiteSpace(userCode)) return false;

        var securityToken = authorizationService.ValidateVerificationCodeAsync(userCode).GetAwaiter().GetResult();
        if (securityToken == null || string.IsNullOrWhiteSpace(securityToken.TokenValue))
            return false;

        var userName = securityToken.TokenValue;
        var user = userManager.FindByNameAsync(userName).GetAwaiter().GetResult();
        if (user == null)
            return false;

        _ = authorizationService.DeleteSecurityTokenByTokenValueAsync(userCode).GetAwaiter().GetResult();

        model.UserName = userName;
        if (!model.RequestRawData.ContainsKey(OpenIdConstants.TokenRequest.UserName))
            model.RequestRawData[OpenIdConstants.TokenRequest.UserName] = userName;

        var identity = new ClaimsIdentity(
            new[]
            {
                new Claim(SystemClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(SystemClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim(OpenIdConstants.ClaimTypes.AuthenticationTime, DateTime.UtcNow.ToUnixTime().ToString(), ClaimValueTypes.Integer64)
            },
            "user_code");
        model.Subject = new ClaimsPrincipal(identity);

        return true;
    }
}
