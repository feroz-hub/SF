using Microsoft.AspNetCore.Http;
using Zentra.Domain;
using Zentra.Domain.Constants;
using Zentra.Domain.Models.Endpoint.Request;
using Zentra.DomainServices.Infra;
using Zentra.Service.Implementation.Endpoint.Extensions;
using Zentra.Service.Implementation.Endpoint.Results;
using Zentra.Service.Interfaces.Interfaces.Endpoint;
using Zentra.Service.Interfaces.Interfaces.Endpoint.Results;

namespace Zentra.Service.Implementation.Endpoint;

internal class DiscoveryEndpoint : IEndpoint
{
    private readonly IDiscoveryService discoveryService;
    private readonly ILoggerService loggerService;
    private readonly TokenSettings tokenSettings;

    public DiscoveryEndpoint(
        ILoggerInstance instance,
        IDiscoveryService discoveryService,
        ZentraConfig tokenSettings)
    {
        this.discoveryService = discoveryService;
        this.tokenSettings = tokenSettings.TokenSettings;
        loggerService = instance.GetLoggerInstance(LoggerKeyConstants.DefaultLoggerKey);
    }

    public async Task<IEndpointResult> ProcessAsync(HttpContext context)
    {
        loggerService.WriteTo(Log.Debug, "Processing discovery request.");

        var request = new DiscoveryRequestModel
        {
            BaseUrl = context.GetZentraHost().IncludeEndSlash()
        };
        var result = await discoveryService.GenerateDiscoveryMetaData(request);

        loggerService.WriteTo(Log.Debug, "Discovery request successfully processed.");

        return new DiscoveryResult(result, tokenSettings.TokenConfig.CachingLifetime);
    }
}
