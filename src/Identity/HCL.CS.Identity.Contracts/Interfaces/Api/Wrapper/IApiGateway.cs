using Microsoft.AspNetCore.Http;

namespace HCL.CS.Service.Interfaces.Interfaces.Api.Wrapper;

public interface IApiGateway
{
    Task<bool> ProcessRequest(HttpContext httpContext);
}
