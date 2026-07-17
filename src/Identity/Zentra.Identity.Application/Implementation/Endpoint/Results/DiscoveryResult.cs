using Microsoft.AspNetCore.Http;
using Zentra.Domain.Constants.Endpoint;
using Zentra.Service.Implementation.Endpoint.Extensions;
using Zentra.Service.Interfaces.Interfaces.Endpoint.Results;

namespace Zentra.Service.Implementation.Endpoint.Results;

internal class DiscoveryResult : IEndpointResult
{
    public DiscoveryResult(Dictionary<string, object> metadataValues, int cacheExpiry)
    {
        DiscoveryMetadata = metadataValues;
        CacheExpiry = cacheExpiry;
    }

    internal Dictionary<string, object> DiscoveryMetadata { get; set; }

    internal int CacheExpiry { get; set; }

    public async Task ConstructResponseAsync(HttpContext context)
    {
        context.Response.SetResponseCache(CacheExpiry);
        context.Response.StatusCode = OpenIdConstants.HTTPStatusCodes.success;
        await context.Response.WriteResponseJsonAsync(DiscoveryMetadata);
    }
}
