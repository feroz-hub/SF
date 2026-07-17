using Microsoft.AspNetCore.Http;
using Zentra.Domain.Constants.Endpoint;
using Zentra.Domain.Models.Endpoint.Response;
using Zentra.Service.Implementation.Endpoint.Extensions;
using Zentra.Service.Interfaces.Interfaces.Endpoint.Results;

namespace Zentra.Service.Implementation.Endpoint.Results;

internal class IntrospectionResult : IEndpointResult
{
    public IntrospectionResult(IntrospectionResponseModel response)
    {
        Response = response;
    }

    internal IntrospectionResponseModel Response { get; }

    public async Task ConstructResponseAsync(HttpContext context)
    {
        context.Response.SetResponseNoCache();
        context.Response.StatusCode = OpenIdConstants.HTTPStatusCodes.success;
        if (Response != null) await context.Response.WriteResponseJsonAsync(Response);
    }
}
