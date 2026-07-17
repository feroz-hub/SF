using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HCL.CS.DemoClientMvc.Models;

namespace HCL.CS.DemoClientMvc.Controllers;

public class ErrorController : Controller
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error(string errorCode, string errorMessage)
    {
        var normalizedErrorCode = string.IsNullOrWhiteSpace(errorCode) ? "UnhandledError" : errorCode;

        var safeMessage = normalizedErrorCode switch
        {
            "OidcRemoteFailure" => "Remote authentication failure.",
            "OidcAuthenticationFailed" => "OpenID Connect authentication failed.",
            "UnhandledError" => "The request could not be completed due to an unexpected error.",
            _ => "The request could not be completed due to an unexpected error."
        };

        var errorViewModel = new ErrorViewModel
            { ErrorCode = normalizedErrorCode, ErrorMessage = safeMessage };
        errorViewModel.RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        return View(errorViewModel);
    }
}
