using System.Net;
using Microsoft.AspNetCore.Http;
using Zentra.Service.Interfaces.Interfaces.Endpoint.Results;

namespace Zentra.Service.Implementation.Endpoint.Results;

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
