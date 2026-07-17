using Microsoft.AspNetCore.Http;
using HCL.CS.Domain.Constants.Endpoint;
using HCL.CS.Domain.Models.Endpoint.Response;
using HCL.CS.Service.Implementation.Endpoint.Extensions;
using HCL.CS.Service.Interfaces.Interfaces.Endpoint.Results;

namespace HCL.CS.Service.Implementation.Endpoint.Results;

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
