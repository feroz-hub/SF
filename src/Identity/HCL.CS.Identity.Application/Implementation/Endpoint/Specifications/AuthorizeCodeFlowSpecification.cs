/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using System.Security.Claims;
using DomainValidation.Interfaces.Specification;
using DomainValidation.Validation;
using HCL.CS.Domain.Constants.Endpoint;
using HCL.CS.Domain.Entities.Api;
using HCL.CS.Domain.ErrorCodes;
using HCL.CS.Domain.Models.Endpoint.Request;
using HCL.CS.DomainServices.Wrappers;
using HCL.CS.Service.Extension;
using HCL.CS.Service.Implementation.Endpoint.Extensions;
using HCL.CS.Service.Implementation.Endpoint.Validators;
using HCL.CS.Service.Interfaces.Interfaces.Endpoint;

namespace HCL.CS.Service.Implementation.Endpoint.Specifications;

internal sealed class AuthorizationCodeFlowSpecification : BaseRequestModelValidator<ValidatedTokenRequestModel>
{
    internal AuthorizationCodeFlowSpecification(IAuthorizationService authorizationService,
        UserManagerWrapper<Users> userManager)
    {
        Add("CheckClientAuthorizedForGrantType", new Rule<ValidatedTokenRequestModel>(
            new CheckClientAuthorizedForGrantType<ValidatedTokenRequestModel>(new List<string>
            {
                AuthenticationConstants.GrantType.AuthorizationCode
            }),
            OpenIdConstants.Errors.UnauthorizedClient,
            EndpointErrorCodes.ClientNotAuthorizedForGrantType));

        Add("CheckAuthorizationCodeNull", new Rule<ValidatedTokenRequestModel>(
            new IsRequestNull<ValidatedTokenRequestModel>(request =>
                request.GetValue(OpenIdConstants.TokenRequest.Code)),
            OpenIdConstants.Errors.InvalidGrant,
            EndpointErrorCodes.AuthorizationCodeMissing));

        Add("CheckAuthorizationCodeLength", new Rule<ValidatedTokenRequestModel>(
            new CheckLengthRestrictions<ValidatedTokenRequestModel>(
                request => request.GetValue(OpenIdConstants.TokenRequest.Code),
                request => request.TokenConfigOptions.InputLengthRestrictionsConfig.AuthorizationCode,
                request => ">"),
            OpenIdConstants.Errors.InvalidGrant,
            EndpointErrorCodes.AuthorizationCodeTooLong));

        Add("CheckAuthorizationCode", new Rule<ValidatedTokenRequestModel>(
            new CheckAuthorizationCode(authorizationService),
            OpenIdConstants.Errors.InvalidGrant,
            EndpointErrorCodes.InvalidAuthorizationCode));

        Add("CheckClientBinding", new Rule<ValidatedTokenRequestModel>(
            new CheckClientBinding(),
            OpenIdConstants.Errors.InvalidGrant,
            EndpointErrorCodes.InvalidClientBinding));

        Add("CheckAuthorizationCodeExpiry", new Rule<ValidatedTokenRequestModel>(
            new CheckAuthorizationCodeExpiry(),
            OpenIdConstants.Errors.InvalidGrant,
            EndpointErrorCodes.AuthorizationCodeExpired));

        Add("CheckRedirectUri", new Rule<ValidatedTokenRequestModel>(
            new IsRequestNull<ValidatedTokenRequestModel>(request =>
                request.GetValue(OpenIdConstants.TokenRequest.RedirectUri)),
            OpenIdConstants.Errors.UnauthorizedClient,
            EndpointErrorCodes.RedirectUriMissing));

        Add("CompareRedirectUriFromAuthCode", new Rule<ValidatedTokenRequestModel>(
            new CompareRedirectUriFromAuthCode(),
            OpenIdConstants.Errors.InvalidGrant,
            EndpointErrorCodes.InvalidRedirectUri));

        Add("CheckScopesPresence", new Rule<ValidatedTokenRequestModel>(
            new CheckScopesPresence(),
            OpenIdConstants.Errors.InvalidRequest,
            EndpointErrorCodes.AuthorizationCodeScopeError));

        Add("CheckCodeVerifier", new Rule<ValidatedTokenRequestModel>(
            new CheckCodeVerifier(),
            OpenIdConstants.Errors.InvalidGrant,
            EndpointErrorCodes.InvalidCodeVerifier));

        Add("CheckUserExists", new Rule<ValidatedTokenRequestModel>(
            new CheckUserExistsByClaimsPrincipal(userManager),
            OpenIdConstants.Errors.InvalidGrant,
            EndpointErrorCodes.UserDoesNotExist));

        Add("CheckUserRoleMapped", new Rule<ValidatedTokenRequestModel>(
            new CheckUserRoleMapped(userManager),
            OpenIdConstants.Errors.InvalidRequest,
            EndpointErrorCodes.NoUserRoleMapped));
    }
}

internal class CheckAuthorizationCode : ISpecification<ValidatedTokenRequestModel>
{
    private readonly IAuthorizationService authorizationService;

    internal CheckAuthorizationCode(IAuthorizationService authorizationService)
    {
        this.authorizationService = authorizationService;
    }

    public bool IsSatisfiedBy(ValidatedTokenRequestModel model)
    {
        var authorizationCode = model.GetValue(OpenIdConstants.TokenRequest.Code);
        var authCodeModel = authorizationService.GetAuthorizationCodeAsync(authorizationCode).GetAwaiter().GetResult();
        if (authCodeModel == null) return false;

        model.AuthorizationCode = authCodeModel;
        return true;
    }
}

internal class CheckClientBinding : ISpecification<ValidatedTokenRequestModel>
{
    public bool IsSatisfiedBy(ValidatedTokenRequestModel model)
    {
        if (model.AuthorizationCode.ClientId != model.Client.ClientId) return false;

        return true;
    }
}

internal class CheckAuthorizationCodeExpiry : ISpecification<ValidatedTokenRequestModel>
{
    public bool IsSatisfiedBy(ValidatedTokenRequestModel model)
    {
        if (model.AuthorizationCode.CreationTime.IsExpired(model.AuthorizationCode.Lifetime)) return false;

        // validate code expiration
        if (model.AuthorizationCode.CreationTime.IsExpired(model.Client.AuthorizationCodeExpiration)) return false;

        // populate Session id
        if (!string.IsNullOrWhiteSpace(model.AuthorizationCode.SessionId))
            model.SessionId = model.AuthorizationCode.SessionId;

        model.Subject = model.AuthorizationCode.Subject;
        model.Nonce = model.AuthorizationCode.Nonce;
        model.State = model.AuthorizationCode.State;
        return true;
    }
}

internal class CompareRedirectUriFromAuthCode : ISpecification<ValidatedTokenRequestModel>
{
    public bool IsSatisfiedBy(ValidatedTokenRequestModel model)
    {
        var redirectUri = model.GetValue(OpenIdConstants.TokenRequest.RedirectUri);
        if (redirectUri != null &&
            !redirectUri.Equals(model.AuthorizationCode.RedirectUri, StringComparison.Ordinal)) return false;

        return true;
    }
}

internal class CheckScopesPresence : ISpecification<ValidatedTokenRequestModel>
{
    public bool IsSatisfiedBy(ValidatedTokenRequestModel model)
    {
        // validate scopes are present
        if (model.AuthorizationCode.AllowedScopesParserModel == null) return false;

        model.AllowedScopesParserModel = model.AuthorizationCode.AllowedScopesParserModel;
        model.TokenDetails = model.AuthorizationCode.AllowedScopesParserModel.TokenDetails;
        return true;
    }
}

internal class CheckCodeVerifier : ISpecification<ValidatedTokenRequestModel>
{
    public bool IsSatisfiedBy(ValidatedTokenRequestModel model)
    {
        var codeVerifier = model.GetValue(OpenIdConstants.TokenRequest.CodeVerifier);
        if (model.Client.RequirePkce)
        {
            if (string.IsNullOrWhiteSpace(codeVerifier)) return false;

            model.CodeVerifier = model.GetValue(OpenIdConstants.TokenRequest.CodeVerifier);
        }

        return true;
    }
}

internal class CheckUserExistsByClaimsPrincipal : ISpecification<ValidatedTokenRequestModel>
{
    private readonly UserManagerWrapper<Users> userManager;

    internal CheckUserExistsByClaimsPrincipal(UserManagerWrapper<Users> userManager)
    {
        this.userManager = userManager;
    }

    public bool IsSatisfiedBy(ValidatedTokenRequestModel model)
    {
        // make sure user is enabled
        Users user = null;
        var id = model.AuthorizationCode.Subject.Identity as ClaimsIdentity;
        if (id != null)
        {
            var claim = id.FindFirst(OpenIdConstants.ClaimTypes.Sub);
            if (claim != null)
            {
                if (claim.Value.IsGuid()) user = userManager.FindByIdAsync(claim.Value).GetAwaiter().GetResult();

                if (user == null) user = userManager.FindByNameAsync(claim.Value).GetAwaiter().GetResult();
            }
        }

        if (user == null) return false;

        return true;
    }
}

internal class CheckUserRoleMapped : ISpecification<ValidatedTokenRequestModel>
{
    private readonly UserManagerWrapper<Users> userManager;

    internal CheckUserRoleMapped(UserManagerWrapper<Users> userManager)
    {
        this.userManager = userManager;
    }

    public bool IsSatisfiedBy(ValidatedTokenRequestModel model)
    {
        // SBOM authorization is intentionally owned by SBOM Analyzer. Requiring an
        // HCL.CS role here would prevent a newly confirmed local identity from
        // completing OIDC or would force an inappropriate HCL.CS role assignment.
        if (string.Equals(
                model.Client?.ClientId,
                SbomIdentityContract.ClientId,
                StringComparison.Ordinal))
            return true;

        var id = model.AuthorizationCode.Subject.Identity as ClaimsIdentity;
        var claim = id.FindFirst(OpenIdConstants.ClaimTypes.Sub);
        if (claim != null && claim.Value.IsGuid())
        {
            var user = userManager.FindByIdAsync(claim.Value).GetAwaiter().GetResult();
            if (user != null)
            {
                // make sure user is enabled
                var userRoles = (List<string>)userManager.GetRolesAsync(user).GetAwaiter().GetResult();
                if (!userRoles.ContainsAny()) return false;
            }
        }

        return true;
    }
}
