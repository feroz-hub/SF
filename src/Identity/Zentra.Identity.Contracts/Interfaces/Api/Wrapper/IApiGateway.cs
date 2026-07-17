using Microsoft.AspNetCore.Http;

namespace Zentra.Service.Interfaces.Interfaces.Api.Wrapper;

public interface IApiGateway
{
    Task<bool> ProcessRequest(HttpContext httpContext);
}
