using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Zentra.DemoServerApp.Models;
using Zentra.Service.Interfaces.Interfaces.Endpoint;

namespace Zentra.DemoServerApp.Controllers;

public class HomeController(
    ILogger<HomeController> logger,
    IInteractionService interactionService)
    : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<IActionResult> Error(string? errorId = null, string? errorCode = null, string? errorMessage = null)
    {
        var errorViewModel = new ErrorViewModel
        {
            ErrorCode = errorCode,
            ErrorMessage = errorMessage
        };

        if (!string.IsNullOrWhiteSpace(errorId))
        {
            var errorContext = await interactionService.GetErrorContextAsync(errorId);
            if (errorContext != null)
            {
                errorViewModel.ErrorCode = string.IsNullOrWhiteSpace(errorContext.ErrorCode)
                    ? errorViewModel.ErrorCode
                    : errorContext.ErrorCode;
                errorViewModel.ErrorMessage = string.IsNullOrWhiteSpace(errorContext.ErrorDescription)
                    ? errorViewModel.ErrorMessage
                    : errorContext.ErrorDescription;
            }
        }

        errorViewModel.RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

        if (string.IsNullOrWhiteSpace(errorViewModel.ErrorCode))
            errorViewModel.ErrorCode = "UnhandledError";

        if (string.IsNullOrWhiteSpace(errorViewModel.ErrorMessage))
            errorViewModel.ErrorMessage = "The request could not be completed due to an unexpected error.";

        var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
        if (exceptionFeature?.Error != null)
            logger.LogError(
                exceptionFeature.Error,
                "Unhandled exception captured by exception handler for path {Path}. TraceIdentifier: {TraceIdentifier}",
                exceptionFeature.Path,
                HttpContext.TraceIdentifier);

        return View(errorViewModel);
    }
}
