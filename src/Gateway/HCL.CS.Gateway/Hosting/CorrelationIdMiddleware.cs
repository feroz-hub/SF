/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using Microsoft.AspNetCore.Http;

namespace HCL.CS.ProxyService.Hosting;

public class CorrelationIdMiddleware
{
    private readonly RequestDelegate next;

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var inboundCorrelationId = context.Request.Headers[ObservabilityConstants.CorrelationIdHeaderName].ToString();
        var correlationId = ResolveCorrelationId(inboundCorrelationId);

        context.TraceIdentifier = correlationId;
        context.Items[ObservabilityConstants.CorrelationIdItemKey] = correlationId;
        context.Response.Headers[ObservabilityConstants.CorrelationIdHeaderName] = correlationId;

        await next(context);
    }

    private static string ResolveCorrelationId(string? correlationId)
    {
        if (string.IsNullOrWhiteSpace(correlationId)) return CreateCorrelationId();

        var value = correlationId.Trim();
        if (!IsValidCorrelationId(value)) return CreateCorrelationId();

        return value;
    }

    private static string CreateCorrelationId()
    {
        return Guid.NewGuid().ToString("N");
    }

    private static bool IsValidCorrelationId(string correlationId)
    {
        if (correlationId.Length is <= 0 or > 128) return false;

        foreach (var character in correlationId)
        {
            if (char.IsLetterOrDigit(character)) continue;
            if (character is '-' or '_' or '.') continue;
            return false;
        }

        return true;
    }
}
