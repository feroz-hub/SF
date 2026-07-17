using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using HCL.CS.Domain.Models.Endpoint;

namespace HCL.CS.Service.Interfaces.Interfaces.Endpoint;

public interface ISessionManagementService
{
    Task AddClientAsync(string clientId);

    Task CreateAndBindSessionCookieAsync(ClaimsPrincipal principal, AuthenticationProperties properties);

    Task<IList<string>> GetClientListAsync();

    Task<string> GetSessionId();

    Task<AuthenticationPropertiesModel> GetAuthenticationAsync();

    Task<string> GetAuthenticationSchemeAsync();

    Task<ClaimsPrincipal> GetUserPrincipalFromContextAsync();

    Task<AuthenticationProperties> GetPropertiesFromContextAsync();
}
