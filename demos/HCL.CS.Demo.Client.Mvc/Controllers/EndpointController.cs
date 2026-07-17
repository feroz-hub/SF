using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HCL.CS.DemoClientMvc.Interface;
using HCL.CS.DemoClientMvc.Services;
using HCL.CS.DemoClientMvc.ViewModels.Endpoint;
using HCL.CS.DemoClientMvc.ViewModels.Shared;

namespace HCL.CS.DemoClientMvc.Controllers;

public class EndpointController(IEndpointFlowService endpointFlowService) : Controller
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> AuthorizeEndpoint(string returnUrl = null)
    {
        var authentication = await HttpContext.AuthenticateAsync();
        var model = new AuthorizeEndpointViewModel
        {
            Claims = User.Claims
                .Select(claim => new ClaimItemViewModel
                {
                    Type = claim.Type,
                    Value = claim.Value
                })
                .OrderBy(claim => claim.Type)
                .ToList(),
            AuthenticationProperties = authentication.Properties?.Items
                .Where(property => !property.Key.Contains("token", StringComparison.OrdinalIgnoreCase))
                .Select(property => new ClaimItemViewModel
                {
                    Type = property.Key,
                    Value = property.Value
                })
                .OrderBy(property => property.Type)
                .ToList() ?? new List<ClaimItemViewModel>()
        };

        return View(model);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ResourceOwnerPassword(string returnUrl = null)
    {
        return View(new ResourceOwnerPasswordPageViewModel());
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResourceOwnerPassword(ResourceOwnerPasswordPageViewModel model,
        string returnUrl = null)
    {
        if (!ModelState.IsValid) return View(model);

        var tokenResult = await endpointFlowService.ExecuteResourceOwnerPasswordFlowAsync(
            model.UserName,
            model.Password,
            HttpContext.RequestAborted);

        model.Result = MapTokenResult(tokenResult);
        if (!model.Result.Succeeded)
            ModelState.AddModelError(model.Result.ErrorCode ?? "token_error",
                model.Result.ErrorDescription ?? "Token request failed.");

        return View(model);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ResourceOwnerEndpoint(string returnUrl = null)
    {
        return RedirectToAction(nameof(ResourceOwnerPassword));
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> ClientCredentailsEndpoint(string returnUrl = null)
    {
        var tokenResult = await endpointFlowService.ExecuteClientCredentialsFlowAsync(HttpContext.RequestAborted);
        var model = MapTokenResult(tokenResult);
        if (!model.Succeeded)
            ModelState.AddModelError(model.ErrorCode ?? "token_error",
                model.ErrorDescription ?? "Token request failed.");

        return View(model);
    }

    private static EndpointTokenFlowViewModel MapTokenResult(EndpointTokenFlowResult result)
    {
        return new EndpointTokenFlowViewModel
        {
            Succeeded = result.Succeeded,
            ExpiresIn = result.ExpiresIn,
            TokenType = result.TokenType,
            ErrorCode = result.ErrorCode,
            ErrorDescription = result.ErrorDescription,
            Claims = result.Claims
                .Select(claim => new ClaimItemViewModel
                {
                    Type = claim.Key,
                    Value = claim.Value
                })
                .OrderBy(claim => claim.Type)
                .ToList()
        };
    }
}
