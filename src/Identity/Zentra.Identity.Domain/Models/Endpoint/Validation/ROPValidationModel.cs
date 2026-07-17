using System.Security.Claims;
using Zentra.Domain.Constants.Endpoint;
using Zentra.Domain.Models.Endpoint.Request;
using Zentra.Domain.Models.Endpoint.Response;

namespace Zentra.Domain.Models.Endpoint.Validation;

public class RopValidationModel : ErrorResponseModel
{
    public RopValidationModel()
    {
    }

    public RopValidationModel(ClaimsPrincipal principal)
    {
        IsError = false;
        if (principal.Identities.Count() != 1) return;

        if (principal.FindFirst(OpenIdConstants.ClaimTypes.Sub) == null) return;

        if (principal.FindFirst(OpenIdConstants.ClaimTypes.IdentityProvider) == null) return;

        if (principal.FindFirst(OpenIdConstants.ClaimTypes.AuthenticationMethod) == null) return;

        if (principal.FindFirst(OpenIdConstants.ClaimTypes.AuthenticationTime) == null) return;

        Subject = principal;
    }

    public RopValidationModel(
        string subject,
        string authenticationMethod,
        DateTime authTime,
        IEnumerable<Claim> claims = null,
        string identityProvider = AuthenticationConstants.LocalIdentityProvider)
    {
        IsError = false;

        var resultClaims = new List<Claim>
        {
            new(OpenIdConstants.ClaimTypes.Sub, subject),
            new(OpenIdConstants.ClaimTypes.AuthenticationMethod, authenticationMethod),
            new(OpenIdConstants.ClaimTypes.IdentityProvider, identityProvider),
            new(OpenIdConstants.ClaimTypes.AuthenticationTime,
                new DateTimeOffset(authTime).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        if (claims != null && claims.Any()) resultClaims.AddRange(claims);

        var id = new ClaimsIdentity(authenticationMethod);
        id.AddClaims(resultClaims.Distinct());

        Subject = new ClaimsPrincipal(id);
    }

    public string UserName { get; set; }

    public string Password { get; set; }

    public ValidatedTokenRequestModel Request { get; set; }

    public ClaimsPrincipal Subject { get; set; }
}
