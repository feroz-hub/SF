using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using HCL.CS.Domain.Models.Endpoint.Response;

namespace HCL.CS.Domain.Models.Endpoint;

public class AuthenticationPropertiesModel : ErrorResponseModel
{
    public ClaimsPrincipal Principal { get; set; }

    public AuthenticationProperties Properties { get; set; }
}
