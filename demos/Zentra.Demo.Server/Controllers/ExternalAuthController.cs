using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Zentra.DemoServerApp.Options;
using Zentra.DemoServerApp.Services.ExternalAuth;

namespace Zentra.DemoServerApp.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
public class ExternalAuthController(
    IExternalAuthService externalAuthService,
    IOptions<GoogleOidcOptions> googleOptions)
    : Controller
{
    private readonly GoogleOidcOptions google = googleOptions.Value;

    [AllowAnonymous]
    [HttpGet("/auth/external/google/start")]
    public IActionResult GoogleStart(string? returnUrl = null, string? tenantId = null)
    {
        if (!google.Enabled)
        {
            return StatusCode(
                StatusCodes.Status503ServiceUnavailable,
                "Google sign-in is not enabled. Set Authentication:Google:Enabled to true and configure ClientId and ClientSecret.");
        }

        var properties = externalAuthService.BuildChallengeProperties(
            GoogleExternalAuthProvider.ProviderName,
            returnUrl,
            tenantId,
            false);

        return Challenge(properties, GoogleExternalAuthProvider.Scheme);
    }

    [AllowAnonymous]
    [HttpGet("/auth/external/google/callback")]
    public async Task<IActionResult> GoogleCallback()
    {
        if (!google.Enabled)
        {
            return StatusCode(
                StatusCodes.Status503ServiceUnavailable,
                "Google sign-in is not enabled.");
        }

        var result = await externalAuthService.CompleteGoogleCallbackAsync(HttpContext);
        if (result.RequiresTwoFactor) return Redirect(result.RedirectUrl);

        if (!result.Succeeded)
        {
            TempData["ExternalAuthError"] = result.Message;
            return RedirectToAction("Login", "Account", new { returnUrl = result.RedirectUrl });
        }

        return RedirectSafely(result.RedirectUrl);
    }

    [Authorize]
    [ValidateAntiForgeryToken]
    [HttpPost("/auth/external/link/google")]
    public IActionResult LinkGoogle(string? returnUrl = null)
    {
        if (!google.Enabled) return NotFound();

        var properties = externalAuthService.BuildChallengeProperties(
            GoogleExternalAuthProvider.ProviderName,
            returnUrl,
            null,
            true);

        return Challenge(properties, GoogleExternalAuthProvider.Scheme);
    }

    [Authorize]
    [ValidateAntiForgeryToken]
    [HttpPost("/auth/external/unlink/google")]
    public async Task<IActionResult> UnlinkGoogle()
    {
        var result = await externalAuthService.UnlinkGoogleAsync(HttpContext);
        TempData["ExternalAuthMessage"] = result.Message;
        return RedirectSafely(result.RedirectUrl);
    }

    private IActionResult RedirectSafely(string redirectUrl)
    {
        return Url.IsLocalUrl(redirectUrl) ? LocalRedirect(redirectUrl) : Redirect(redirectUrl);
    }
}
