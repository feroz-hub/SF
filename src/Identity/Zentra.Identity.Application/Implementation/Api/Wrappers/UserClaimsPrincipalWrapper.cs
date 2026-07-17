using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Zentra.Domain.Constants.Endpoint;
using Zentra.Domain.Entities.Api;
using Zentra.DomainServices.Wrappers;

namespace Zentra.Service.Implementation.Api.Wrappers;

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
