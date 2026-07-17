using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zentra.DemoClientMvc.ViewModels.Profile;
using Zentra.DemoClientMvc.ViewModels.Shared;
using Zentra.Domain.Constants.Endpoint;

namespace Zentra.DemoClientMvc.Controllers;

[Authorize]
public class ProfileController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        var claims = User.Claims
            .Select(claim => new ClaimItemViewModel
            {
                Type = claim.Type,
                Value = claim.Value
            })
            .OrderBy(claim => claim.Type)
            .ToList();

        var model = new ProfileViewModel
        {
            UserName = User.Identity?.Name ?? string.Empty,
            Email = User.Claims.FirstOrDefault(claim => claim.Type == OpenIdConstants.ClaimTypes.Email)?.Value ??
                    string.Empty,
            SubjectId = User.Claims.FirstOrDefault(claim => claim.Type == OpenIdConstants.ClaimTypes.Sub)?.Value ??
                        string.Empty,
            Roles = User.Claims
                .Where(claim => claim.Type == OpenIdConstants.ClaimTypes.Role || claim.Type == ClaimTypes.Role)
                .Select(claim => claim.Value)
                .Distinct()
                .ToList(),
            Claims = claims
        };

        return View(model);
    }
}
