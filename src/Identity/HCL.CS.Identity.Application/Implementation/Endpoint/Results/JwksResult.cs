using Microsoft.AspNetCore.Http;
using HCL.CS.Domain.Constants.Endpoint;
using HCL.CS.Domain.Models.Endpoint.Response;
using HCL.CS.Service.Implementation.Endpoint.Extensions;
using HCL.CS.Service.Interfaces.Interfaces.Endpoint.Results;

namespace HCL.CS.Service.Implementation.Endpoint.Results;

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
