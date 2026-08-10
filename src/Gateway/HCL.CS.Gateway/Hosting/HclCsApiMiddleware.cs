/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using Microsoft.AspNetCore.Http;
using HCL.CS.Domain;
using HCL.CS.Domain.Constants;
using HCL.CS.Domain.Constants.Endpoint;
using HCL.CS.DomainServices.Infra;
using HCL.CS.ProxyService.Routes.Extension;
using HCL.CS.Service.Interfaces.Interfaces.Api.Wrapper;

namespace HCL.CS.ProxyService.Hosting;

public class HclCsApiMiddleware
{
    private readonly ILoggerService loggerService;
    private readonly RequestDelegate next;

    public HclCsApiMiddleware(
        RequestDelegate next,
        ILoggerInstance instance)
    {
        this.next = next;
        loggerService = instance.GetLoggerInstance(LoggerKeyConstants.DefaultLoggerKey);
    }

    public async Task InvokeAsync(HttpContext httpContext, IApiGateway apiRoutewrapper)
    {
        try
        {
            if (HttpMethods.IsPost(httpContext.Request.Method))
            {
                var path = httpContext.Request.Path != null ? httpContext.Request.Path.Value : null;
                if (!string.IsNullOrWhiteSpace(path) &&
                    path.StartsWith(ApiRoutePathConstants.BasePath, StringComparison.OrdinalIgnoreCase))
                {
                    await apiRoutewrapper.ProcessRequest(httpContext);
                    return;
                }
            }
        }
        catch (Exception ex)
        {
            var errorDescription = ResolveApiErrorDescription(ex);
            httpContext.Response.StatusCode = OpenIdConstants.HTTPStatusCodes.bad_request;
            await httpContext.Response.WriteResponseJsonAsync(new
            {
                error = OpenIdConstants.Errors.ServerError,
                error_description = errorDescription
            });
            loggerService.WriteToWithCaller(Log.Error, ex, "Exception while processing API middleware request.");
            return;
        }

        await next(httpContext);
    }

    private static string ResolveApiErrorDescription(Exception exception)
    {
        var current = exception;
        while (current.InnerException != null && !string.IsNullOrWhiteSpace(current.InnerException.Message))
        {
            current = current.InnerException;
        }

        return string.IsNullOrWhiteSpace(current.Message)
            ? "Request processing failed."
            : current.Message;
    }
}
