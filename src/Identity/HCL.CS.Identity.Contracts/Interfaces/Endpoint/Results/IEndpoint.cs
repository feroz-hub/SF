using Microsoft.AspNetCore.Http;

namespace HCL.CS.Service.Interfaces.Interfaces.Endpoint.Results;

public interface IEndpoint
{
    Task<IEndpointResult> ProcessAsync(HttpContext context);
}
