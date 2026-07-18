/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using HCL.CS.Domain.Constants.Endpoint;
using HCL.CS.Domain.Entities.Api;
using HCL.CS.DomainServices.Wrappers;

namespace HCL.CS.Service.Implementation.Api.Wrappers;

public class UserClaimsPrincipalWrapper(
    UserManagerWrapper<Users> userManager,
    IOptions<IdentityOptions> optionsAccessor)
    : UserClaimsPrincipalFactory<Users>(userManager,
        optionsAccessor)
{
    public override async Task<ClaimsPrincipal> CreateAsync(Users user)
    {
        var principal = await base.CreateAsync(user);
        var identity = principal.Identities.First();

        var currentUser = await userManager.FindByNameAsync(user.UserName);

        if (!identity.HasClaim(x => x.Type == OpenIdConstants.ClaimTypes.Sub))
            identity.AddClaim(new Claim(OpenIdConstants.ClaimTypes.Sub, Convert.ToString(currentUser.Id)));

        var usernameClaim = identity.FindFirst(claim =>
            claim.Type == userManager.Options.ClaimsIdentity.UserNameClaimType && claim.Value == currentUser.UserName);
        if (usernameClaim != null)
            identity.AddClaim(new Claim(OpenIdConstants.ClaimTypes.PreferredUserName, currentUser.UserName));

        if (!identity.HasClaim(x => x.Type == OpenIdConstants.ClaimTypes.Name))
            identity.AddClaim(new Claim(OpenIdConstants.ClaimTypes.Name, currentUser.UserName));

        if (userManager.SupportsUserEmail && !string.IsNullOrWhiteSpace(currentUser.Email))
            identity.AddClaims(new[]
            {
                new Claim(OpenIdConstants.ClaimTypes.Email, currentUser.Email),
                new Claim(OpenIdConstants.ClaimTypes.EmailVerified,
                    await userManager.IsEmailConfirmedAsync(currentUser) ? "true" : "false", ClaimValueTypes.Boolean)
            });

        if (userManager.SupportsUserPhoneNumber && !string.IsNullOrWhiteSpace(currentUser.PhoneNumber))
            identity.AddClaims(new[]
            {
                new Claim(OpenIdConstants.ClaimTypes.PhoneNumber, currentUser.PhoneNumber),
                new Claim(OpenIdConstants.ClaimTypes.PhoneNumberVerified,
                    await userManager.IsPhoneNumberConfirmedAsync(currentUser) ? "true" : "false",
                    ClaimValueTypes.Boolean)
            });

        return principal;
    }
}
