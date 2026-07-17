using Microsoft.AspNetCore.Http;
using Zentra.Domain.Constants.Endpoint;
using Zentra.Domain.Models.Endpoint.Response;
using Zentra.Service.Implementation.Endpoint.Extensions;
using Zentra.Service.Interfaces.Interfaces.Endpoint.Results;

namespace Zentra.Service.Implementation.Endpoint.Results;

internal class JwksResult : IEndpointResult
{
    public JwksResult(List<JsonWebKeyResponseModel> webKeys, int expiry)
    {
        WebKeys = webKeys;
        Expiry = expiry;
    }

    internal List<JsonWebKeyResponseModel> WebKeys { get; }

    internal int Expiry { get; }

    public async Task ConstructResponseAsync(HttpContext context)
    {
        context.Response.SetResponseCache(Expiry);
        context.Response.StatusCode = OpenIdConstants.HTTPStatusCodes.success;
        await context.Response.WriteResponseJsonAsync(new { keys = WebKeys }, "application/json; charset=UTF-8");
    }
}
