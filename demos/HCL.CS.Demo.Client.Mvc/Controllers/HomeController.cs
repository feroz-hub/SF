using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using HCL.CS.DemoClientMvc.Constants;
using HCL.CS.DemoClientMvc.Interface;
using HCL.CS.DemoClientMvc.ViewModels.Home;
using HCL.CS.Domain.Constants;
using HCL.CS.Domain.Constants.Endpoint;
using HCL.CS.Domain.Models.Api;

namespace HCL.CS.DemoClientMvc.Controllers;

public class HomeController(IHttpService httpService) : Controller
{
    public async Task<IActionResult> Index()
    {
        try
        {
            if (User != null && User.Identity != null && User.Identity.IsAuthenticated)
            {
                var userid = HttpContext.Session.GetString(ApplicationConstants.SessionUserId);
                if (userid == null)
                {
                    var userModel = await httpService.PostSecureAsync<UserModel>(ApiRoutePathConstants.GetUserByName,
                        User.Identity.Name);
                    if (userModel != null)
                        HttpContext.Session.SetString(ApplicationConstants.SessionUserId, userModel.Id.ToString());
                }
            }

            var model = new HomeIndexViewModel
            {
                IsAuthenticated = User?.Identity?.IsAuthenticated == true,
                UserName = User?.Identity?.Name ?? string.Empty,
                Roles = User?.Claims
                    .Where(claim => claim.Type == OpenIdConstants.ClaimTypes.Role || claim.Type == ClaimTypes.Role)
                    .Select(claim => claim.Value)
                    .Distinct()
                    .ToList() ?? new List<string>()
            };

            return View(model);
        }
        catch (Exception ex)
        {
            return RedirectToAction("Error", "Error", new { errorCode = "", errorMessage = ex.Message });
        }
    }

    public IActionResult Privacy()
    {
        return View();
    }
}
