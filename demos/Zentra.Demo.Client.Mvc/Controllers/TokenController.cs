using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zentra.DemoClientMvc.Interface;
using Zentra.DemoClientMvc.ViewModels.Shared;
using Zentra.DemoClientMvc.ViewModels.Token;

namespace Zentra.DemoClientMvc.Controllers;

[Authorize]
public class TokenController(ITokenService tokenService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var accessToken = await tokenService.GetAccessTokenAsync(cancellationToken);
        var refreshToken = await tokenService.GetRefreshTokenAsync(cancellationToken);
        var idToken = await tokenService.GetIdTokenAsync(cancellationToken);
        var expiresAt = await tokenService.GetAccessTokenExpiresAtAsync(cancellationToken);

        var model = new TokenViewModel
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            IdToken = idToken,
            ExpiresAtUtc = expiresAt?.UtcDateTime.ToString("u"),
            AccessTokenClaims = ParseAccessTokenClaims(accessToken)
        };

        return View(model);
    }

    private static IReadOnlyCollection<ClaimItemViewModel> ParseAccessTokenClaims(string? accessToken)
    {
        if (string.IsNullOrWhiteSpace(accessToken)) return Array.Empty<ClaimItemViewModel>();

        var handler = new JwtSecurityTokenHandler();
        if (!handler.CanReadToken(accessToken)) return Array.Empty<ClaimItemViewModel>();

        var token = handler.ReadJwtToken(accessToken);
        return token.Claims
            .GroupBy(claim => claim.Type)
            .Select(group => new ClaimItemViewModel
            {
                Type = group.Key,
                Value = string.Join(", ", group.Select(c => c.Value))
            })
            .OrderBy(claim => claim.Type, StringComparer.Ordinal)
            .ToList();
    }
}
