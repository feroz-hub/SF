/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using HCL.CS.Domain.Constants.Endpoint;
using HCL.CS.Domain.Models.Endpoint.Response;
using HCL.CS.Service.Implementation.Endpoint.Extensions;
using HCL.CS.Service.Interfaces.Interfaces.Endpoint.Results;

namespace HCL.CS.Service.Implementation.Endpoint.Results;

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
