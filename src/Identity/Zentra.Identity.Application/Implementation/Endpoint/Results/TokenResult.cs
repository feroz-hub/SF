using Microsoft.AspNetCore.Http;
using Zentra.Domain.Constants.Endpoint;
using Zentra.Domain.Models.Endpoint.Response;
using Zentra.Service.Implementation.Endpoint.Extensions;
using Zentra.Service.Interfaces.Interfaces.Endpoint.Results;

namespace Zentra.Service.Implementation.Endpoint.Results;

public class TokenResult : IEndpointResult
{
    public TokenResult(TokenResponseModel response)
    {
        Response = response;
    }

    internal TokenResponseModel Response { get; set; }

    public async Task ConstructResponseAsync(HttpContext context)
    {
        context.Response.SetResponseNoCache();

        var tokenResponseResultModel = new TokenResponseResultModel
        {
            id_token = Response.IdentityToken,
            access_token = Response.AccessToken,
            refresh_token = Response.RefreshToken,
            expires_in = Response.AccessTokenExpiresIn,
            token_type = OpenIdConstants.TokenResponseType.BearerTokenType,
            scope = Response.Scope
        };
        //context.Response.StatusCode = OpenIdConstants.HTTPStatusCodes.success;
        await context.Response.WriteResponseJsonAsync(tokenResponseResultModel);
    }
}
