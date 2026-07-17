using Microsoft.AspNetCore.Http;
using HCL.CS.Domain;
using HCL.CS.Domain.Constants;
using HCL.CS.Domain.Constants.Endpoint;
using HCL.CS.DomainServices.Infra;
using HCL.CS.Service.Implementation.Endpoint.Extensions;
using HCL.CS.Service.Interfaces.Interfaces.Endpoint.Parsers;

namespace HCL.CS.Service.Implementation.Endpoint.Parsers;

internal class TokenParser : ITokenParser
{
    private readonly ILoggerService loggerService;

    public TokenParser(ILoggerInstance instance)
    {
        loggerService = instance.GetLoggerInstance(LoggerKeyConstants.DefaultLoggerKey);
    }

    public async Task<string> ParseAsync(HttpContext context)
    {
        var result = ParseFromAuthorizationHeader(context);
        loggerService.WriteTo(Log.Debug, "Entered into token parser.");
        if (!string.IsNullOrWhiteSpace(result)) return result;

        if (HttpMethods.IsPost(context.Request.Method) && context.Request.CheckHeaderContentType())
        {
            result = await ParseFromPostBodyAsync(context);
            if (!string.IsNullOrWhiteSpace(result)) return result;
        }

        loggerService.WriteTo(Log.Debug, "Bearer token not found.");
        return null;
    }

    private string ParseFromAuthorizationHeader(HttpContext context)
    {
        var authorizationHeader = context.Request.Headers["Authorization"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(authorizationHeader))
        {
            loggerService.WriteTo(Log.Debug, "Entered into parse From authorization header.");
            var header = authorizationHeader.Trim();
            if (header.StartsWith(OpenIdConstants.AuthenticationSchemes.AuthorizationHeaderBearer))
            {
                var value = header[OpenIdConstants.AuthenticationSchemes.AuthorizationHeaderBearer.Length..].Trim();
                if (!string.IsNullOrWhiteSpace(value)) return value;
            }
            else
            {
                loggerService.WriteTo(Log.Debug, "Unexpected header format.");
            }
        }

        return null;
    }

    private async Task<string> ParseFromPostBodyAsync(HttpContext context)
    {
        var token = (await context.Request.ReadFormAsync())["access_token"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(token))
        {
            loggerService.WriteTo(Log.Debug, "Entered into parse from post body.");
            return token;
        }

        return null;
    }
}
