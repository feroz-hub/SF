using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Zentra.Domain.Models.Endpoint.Validation;

namespace Zentra.Domain.Models.Endpoint.Request;

public class ValidatedIntrospectionRequestModel : ValidatedBaseModel
{
    public bool Active { get; set; } = true;

    public string Scopes { get; set; }

    public SecurityKey Key { get; set; }

    public JwtSecurityToken DecodedToken { get; set; }

    public string TokenType { get; set; }

    public string UserId { get; set; }

    public long? ExpiresAt { get; set; }

    public long? IssuedAt { get; set; }
}
