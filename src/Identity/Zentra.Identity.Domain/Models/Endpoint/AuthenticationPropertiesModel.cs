using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Zentra.Domain.Models.Endpoint.Response;

namespace Zentra.Domain.Models.Endpoint;

public class AuthenticationPropertiesModel : ErrorResponseModel
{
    public ClaimsPrincipal Principal { get; set; }

    public AuthenticationProperties Properties { get; set; }
}
