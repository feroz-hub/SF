using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Zentra.Domain.Models.Endpoint.Validation;

namespace Zentra.Domain.Models.Endpoint.Request;

public class ValidatedEndSessionRequestModel : ValidatedBaseModel
{
    public SecurityKey Key { get; set; }

    public string SubjectId { get; set; }

    public JwtSecurityToken DecodedToken { get; set; }

    public long ExpiresAt { get; set; }

    public string PostLogOutUri { get; set; }

    public string State { get; set; }

    public IEnumerable<string> ClientIds { get; set; }
}
