using Microsoft.AspNetCore.Http;

namespace Zentra.Service.Interfaces.Interfaces.Endpoint.Results;

public interface IEndpoint
{
    Task<IEndpointResult> ProcessAsync(HttpContext context);
}
