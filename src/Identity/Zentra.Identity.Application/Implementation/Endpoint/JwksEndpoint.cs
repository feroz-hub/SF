using System.Net;
using Microsoft.AspNetCore.Http;
using Zentra.Domain;
using Zentra.Domain.Constants;
using Zentra.Domain.Models.Endpoint.Response;
using Zentra.DomainServices.Infra;
using Zentra.Service.Implementation.Endpoint.Results;
using Zentra.Service.Interfaces.Interfaces.Endpoint;
using Zentra.Service.Interfaces.Interfaces.Endpoint.Results;

namespace Zentra.Service.Implementation.Endpoint;

internal class JwksEndpoint : IEndpoint
{
    private readonly IJWKSService jwksService;
    private readonly ILoggerService loggerService;
    private readonly TokenSettings tokenSettings;

    public JwksEndpoint(
        ILoggerInstance instance,
        IJWKSService jwksService,
        ZentraConfig settings)
    {
        tokenSettings = settings.TokenSettings;
        this.jwksService = jwksService;
        loggerService = instance.GetLoggerInstance(LoggerKeyConstants.DefaultLoggerKey);
    }

    public async Task<IEndpointResult> ProcessAsync(HttpContext context)
    {
        loggerService.WriteTo(Log.Debug, "Processing jwks endpoint request.");

        // validate HTTP
        if (!HttpMethods.IsGet(context.Request.Method))
        {
            loggerService.WriteTo(Log.Error, "Jwks endpoint supports only GET requests.");
            return new StatusCodeResult(HttpStatusCode.MethodNotAllowed);
        }

        if (!tokenSettings.TokenConfig.ShowKeySet)
        {
            loggerService.WriteTo(Log.Error, "Key discovery disabled.");
            return new StatusCodeResult(HttpStatusCode.NotFound);
        }

        // generate response
        var response = (List<JsonWebKeyResponseModel>)await jwksService.ProcessJWKSInformations();

        return new JwksResult(response, tokenSettings.TokenConfig.CachingLifetime);
    }
}
