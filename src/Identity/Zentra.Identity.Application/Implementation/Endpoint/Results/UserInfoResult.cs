using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Zentra.Domain.Constants.Endpoint;
using Zentra.Domain.Models.Endpoint.Response;
using Zentra.Service.Implementation.Endpoint.Extensions;
using Zentra.Service.Interfaces.Interfaces.Endpoint.Results;

namespace Zentra.Service.Implementation.Endpoint.Results;

internal class UserInfoResult : IEndpointResult
{
    private readonly Dictionary<string, object> claims;
    private readonly ErrorResponseModel errorResponse;

    public UserInfoResult(Dictionary<string, object> claims, ErrorResponseModel errorResponse)
    {
        this.claims = claims;
        this.errorResponse = errorResponse;
    }

    public async Task ConstructResponseAsync(HttpContext context)
    {
        if (errorResponse != null && errorResponse.IsError)
            await ProcessErrorResponse(context);
        else
            await ProcessSuccessResponse(context);
    }

    private async Task ProcessSuccessResponse(HttpContext context)
    {
        context.Response.SetResponseNoCache();
        context.Response.StatusCode = OpenIdConstants.HTTPStatusCodes.success;
        await context.Response.WriteResponseJsonAsync(claims);
    }

    private async Task ProcessErrorResponse(HttpContext context)
    {
        context.Response.SetResponseNoCache();
        var challenge = $"Bearer error=\"{EscapeHeaderValue(errorResponse.ErrorCode)}\"";
        if (!string.IsNullOrWhiteSpace(errorResponse.ErrorDescription))
            challenge = $"{challenge}, error_description=\"{EscapeHeaderValue(errorResponse.ErrorDescription)}\"";

        context.Response.Headers[HeaderNames.WWWAuthenticate] = challenge;

        context.Response.StatusCode = OpenIdConstants.HTTPStatusCodes.GetHttpStatusCode(errorResponse.ErrorCode);
        await Task.CompletedTask;
    }

    private static string EscapeHeaderValue(string input)
    {
        return string.IsNullOrWhiteSpace(input)
            ? string.Empty
            : input.Replace("\\", "\\\\").Replace("\"", "\\\"");
    }
}
