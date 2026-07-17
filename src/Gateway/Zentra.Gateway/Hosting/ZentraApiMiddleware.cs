using Microsoft.AspNetCore.Http;
using Zentra.Domain;
using Zentra.Domain.Constants;
using Zentra.Domain.Constants.Endpoint;
using Zentra.DomainServices.Infra;
using Zentra.ProxyService.Routes.Extension;
using Zentra.Service.Interfaces.Interfaces.Api.Wrapper;

namespace Zentra.ProxyService.Hosting;

public class ZentraApiMiddleware
{
    private readonly ILoggerService loggerService;
    private readonly RequestDelegate next;

    public ZentraApiMiddleware(
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
