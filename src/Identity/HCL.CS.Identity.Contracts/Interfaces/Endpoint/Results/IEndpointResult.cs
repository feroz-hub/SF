using Microsoft.AspNetCore.Http;

namespace HCL.CS.Service.Interfaces.Interfaces.Endpoint.Results;

public interface IEndpointResult
{
    Task ConstructResponseAsync(HttpContext context);
}
