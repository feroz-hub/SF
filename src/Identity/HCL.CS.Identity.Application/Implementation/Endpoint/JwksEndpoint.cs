using System.Net;
using Microsoft.AspNetCore.Http;
using HCL.CS.Domain;
using HCL.CS.Domain.Constants;
using HCL.CS.Domain.Models.Endpoint.Response;
using HCL.CS.DomainServices.Infra;
using HCL.CS.Service.Implementation.Endpoint.Results;
using HCL.CS.Service.Interfaces.Interfaces.Endpoint;
using HCL.CS.Service.Interfaces.Interfaces.Endpoint.Results;

namespace HCL.CS.Service.Implementation.Endpoint;

internal class JwksEndpoint : IEndpoint
{
    private readonly IJWKSService jwksService;
    private readonly ILoggerService loggerService;
    private readonly TokenSettings tokenSettings;

    public JwksEndpoint(
        ILoggerInstance instance,
        IJWKSService jwksService,
        HclCsConfig settings)
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
