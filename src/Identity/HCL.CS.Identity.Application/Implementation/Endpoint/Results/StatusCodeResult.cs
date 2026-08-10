/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

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
