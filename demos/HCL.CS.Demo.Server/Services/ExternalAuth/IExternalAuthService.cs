using Microsoft.AspNetCore.Authentication;

namespace HCL.CS.DemoServerApp.Services.ExternalAuth;

public interface IExternalAuthService
{
    AuthenticationProperties BuildChallengeProperties(string provider, string? returnUrl, string? tenantId,
        bool isLinkRequest);

    Task<ExternalAuthResult> CompleteGoogleCallbackAsync(HttpContext httpContext);

    Task<ExternalAuthResult> UnlinkGoogleAsync(HttpContext httpContext);
}
