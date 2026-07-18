/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

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
