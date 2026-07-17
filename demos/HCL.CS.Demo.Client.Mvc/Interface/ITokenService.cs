using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace HCL.CS.DemoClientMvc.Interface;

public interface ITokenService
{
    Task<string?> GetAccessTokenAsync(CancellationToken cancellationToken = default);

    Task<string?> GetRefreshTokenAsync(CancellationToken cancellationToken = default);

    Task<string?> GetIdTokenAsync(CancellationToken cancellationToken = default);

    Task<DateTimeOffset?> GetAccessTokenExpiresAtAsync(CancellationToken cancellationToken = default);

    Task<bool> RefreshTokenAsync(CancellationToken cancellationToken = default);

    Task<bool> RefreshTokenAsync(
        AuthenticationProperties properties,
        ClaimsPrincipal principal,
        bool persistCookie,
        CancellationToken cancellationToken = default);

    Task<bool> RevokeTokenAsync(CancellationToken cancellationToken = default);
}
