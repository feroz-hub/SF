using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zentra.DemoClientMvc.Interface;
using Zentra.DemoClientMvc.ViewModels.Api;

namespace Zentra.DemoClientMvc.Controllers;

[Authorize]
public class ApiController(IApiClientService apiClientService) : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View(new ApiTestViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Call(ApiTestViewModel model)
    {
        model ??= new ApiTestViewModel();

        if (Uri.TryCreate(model.RelativePath, UriKind.Absolute, out _))
        {
            model.ErrorMessage = "Absolute URLs are not allowed.";
            return View("Index", model);
        }

        var response = await apiClientService.GetAsync(model.RelativePath, HttpContext.RequestAborted);
        model.StatusCode = response.StatusCode;
        model.ResponseBody = response.ResponseBody;
        model.ErrorMessage = response.Succeeded ? null : response.ErrorMessage;

        return View("Index", model);
    }
}
