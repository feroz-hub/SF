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
