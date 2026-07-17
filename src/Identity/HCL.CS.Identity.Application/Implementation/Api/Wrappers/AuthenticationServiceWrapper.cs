using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using HCL.CS.Domain.Constants.Endpoint;
using HCL.CS.Service.Implementation.Endpoint.Extensions;
using HCL.CS.Service.Interfaces.Interfaces.Endpoint;

namespace HCL.CS.Service.Implementation.Api.Wrappers;

public class AuthenticationServiceWrapper(
    IAuthenticationSchemeProvider schemes,
    ISessionManagementService session,
    IBackChannelLogoutService backChannelLogoutService,
    IAuthenticationHandlerProvider handlers,
    IClaimsTransformation transform,
    IOptions<AuthenticationOptions> options)
    : AuthenticationService(schemes, handlers, transform, options)
{
    private readonly IAuthenticationSchemeProvider schemes = schemes;

    public override async Task SignInAsync(HttpContext context, string scheme, ClaimsPrincipal principal,
        AuthenticationProperties properties)
    {
        var defaultScheme = await schemes.GetDefaultSignInSchemeAsync();
        var cookieScheme = await session.GetAuthenticationSchemeAsync();

        if ((scheme == null && defaultScheme?.Name == cookieScheme) || scheme == cookieScheme)
        {
            AddClaimsToPrincipal(principal);

            properties ??= new AuthenticationProperties();
            await session.CreateAndBindSessionCookieAsync(principal, properties);
        }

        await base.SignInAsync(context, scheme, principal, properties);
    }

    public override async Task SignOutAsync(HttpContext context, string scheme, AuthenticationProperties properties)
    {
        var defaultScheme = await schemes.GetDefaultSignOutSchemeAsync();
        var cookieScheme = await session.GetAuthenticationSchemeAsync();

        if ((scheme == null && defaultScheme?.Name == cookieScheme) || scheme == cookieScheme)
        {
            await backChannelLogoutService.ProcessLogoutAsync();
            SetSignOutCalled(context);
        }

        await base.SignOutAsync(context, scheme, properties);
    }

    private void SetSignOutCalled(HttpContext context)
    {
        context.Items[AuthenticationConstants.EnvironmentPaths.SignOutCalled] = "true";
    }

    private void AddClaimsToPrincipal(ClaimsPrincipal principal)
    {
        if (principal.Identities.Count() != 1)
            throw new InvalidOperationException("User should allow to login with only one credentails");

        if (principal.FindFirst(OpenIdConstants.ClaimTypes.Sub) == null)
            throw new InvalidOperationException("No subject claim found");

        AddAdditionalAuthenticationClaims(principal, DateTime.UtcNow);
    }

    private void AddAdditionalAuthenticationClaims(ClaimsPrincipal principal, DateTime authTime)
    {
        var identity = principal.Identities.First();

        var amr = identity.FindFirst(OpenIdConstants.ClaimTypes.AuthenticationMethod);
        if (amr != null &&
            identity.FindFirst(OpenIdConstants.ClaimTypes.IdentityProvider) == null &&
            identity.FindFirst(OpenIdConstants.ClaimTypes.AuthenticationMethod) == null)
        {
            identity.RemoveClaim(amr);
            identity.AddClaim(new Claim(OpenIdConstants.ClaimTypes.IdentityProvider, amr.Value));
        }

        if (identity.FindFirst(OpenIdConstants.ClaimTypes.IdentityProvider) == null)
            // TODO: Add IdentityProvider value as external if user is from ldap
            identity.AddClaim(new Claim(OpenIdConstants.ClaimTypes.IdentityProvider,
                AuthenticationConstants.LocalIdentityProvider));

        if (identity.FindFirst(OpenIdConstants.ClaimTypes.AuthenticationMethod) == null &&
            identity.FindFirst(OpenIdConstants.ClaimTypes.IdentityProvider).Value ==
            AuthenticationConstants.LocalIdentityProvider)
            identity.AddClaim(new Claim(OpenIdConstants.ClaimTypes.AuthenticationMethod,
                OpenIdConstants.UserAuthenticationMethods.Password));

        if (identity.FindFirst(OpenIdConstants.ClaimTypes.AuthenticationTime) == null)
        {
            var time = authTime.ToUnixTime().ToString();
            identity.AddClaim(new Claim(OpenIdConstants.ClaimTypes.AuthenticationTime, time,
                ClaimValueTypes.Integer64));
        }
    }
}
