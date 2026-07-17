using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using HCL.CS.Domain.Constants.Endpoint;
using HCL.CS.Domain.Models.Endpoint.Response;
using HCL.CS.Service.Implementation.Endpoint.Extensions;
using HCL.CS.Service.Interfaces.Interfaces.Endpoint.Results;

namespace HCL.CS.Service.Implementation.Endpoint.Results;

public class ErrorResult : IEndpointResult
{
    public ErrorResult(ErrorResponseModel errorResponse)
    {
        ErrorResponse = errorResponse;
    }

    internal ErrorResponseModel ErrorResponse { get; set; }

    public async Task ConstructResponseAsync(HttpContext context)
    {
        context.Response.SetResponseNoCache();

        if (string.Equals(ErrorResponse.ErrorCode, OpenIdConstants.Errors.InvalidClient, StringComparison.Ordinal))
            context.Response.Headers[HeaderNames.WWWAuthenticate] = "Basic realm=\"token\"";

        var error = new ErrorResponseResultModel
        {
            error = ErrorResponse.ErrorCode,
            error_description = ErrorResponse.ErrorDescription
        };
        context.Response.StatusCode = OpenIdConstants.HTTPStatusCodes.GetHttpStatusCode(ErrorResponse.ErrorCode);
        await context.Response.WriteResponseJsonAsync(error);
    }
}
