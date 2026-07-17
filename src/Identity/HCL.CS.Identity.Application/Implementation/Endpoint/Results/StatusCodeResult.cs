using System.Net;
using Microsoft.AspNetCore.Http;
using HCL.CS.Service.Interfaces.Interfaces.Endpoint.Results;

namespace HCL.CS.Service.Implementation.Endpoint.Results;

internal class StatusCodeResult : IEndpointResult
{
    public StatusCodeResult(HttpStatusCode statusCode)
    {
        StatusCode = (int)statusCode;
    }

    internal StatusCodeResult(int statusCode)
    {
        StatusCode = statusCode;
    }

    internal int StatusCode { get; set; }

    public Task ConstructResponseAsync(HttpContext context)
    {
        context.Response.StatusCode = StatusCode;
        return Task.CompletedTask;
    }
}
