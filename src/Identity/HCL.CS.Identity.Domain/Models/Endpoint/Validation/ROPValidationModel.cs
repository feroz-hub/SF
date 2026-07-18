/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using System.Security.Claims;
using HCL.CS.Domain.Constants.Endpoint;
using HCL.CS.Domain.Models.Endpoint.Request;
using HCL.CS.Domain.Models.Endpoint.Response;

namespace HCL.CS.Domain.Models.Endpoint.Validation;

public class RopValidationModel : ErrorResponseModel
{
    public RopValidationModel()
    {
    }

    public RopValidationModel(ClaimsPrincipal principal)
    {
        IsError = false;
        if (principal.Identities.Count() != 1) return;

        if (principal.FindFirst(OpenIdConstants.ClaimTypes.Sub) == null) return;

        if (principal.FindFirst(OpenIdConstants.ClaimTypes.IdentityProvider) == null) return;

        if (principal.FindFirst(OpenIdConstants.ClaimTypes.AuthenticationMethod) == null) return;

        if (principal.FindFirst(OpenIdConstants.ClaimTypes.AuthenticationTime) == null) return;

        Subject = principal;
    }

    public RopValidationModel(
        string subject,
        string authenticationMethod,
        DateTime authTime,
        IEnumerable<Claim> claims = null,
        string identityProvider = AuthenticationConstants.LocalIdentityProvider)
    {
        IsError = false;

        var resultClaims = new List<Claim>
        {
            new(OpenIdConstants.ClaimTypes.Sub, subject),
            new(OpenIdConstants.ClaimTypes.AuthenticationMethod, authenticationMethod),
            new(OpenIdConstants.ClaimTypes.IdentityProvider, identityProvider),
            new(OpenIdConstants.ClaimTypes.AuthenticationTime,
                new DateTimeOffset(authTime).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        if (claims != null && claims.Any()) resultClaims.AddRange(claims);

        var id = new ClaimsIdentity(authenticationMethod);
        id.AddClaims(resultClaims.Distinct());

        Subject = new ClaimsPrincipal(id);
    }

    public string UserName { get; set; }

    public string Password { get; set; }

    public ValidatedTokenRequestModel Request { get; set; }

    public ClaimsPrincipal Subject { get; set; }
}
