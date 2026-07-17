using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using HCL.CS.Domain.Models.Endpoint.Validation;

namespace HCL.CS.Domain.Models.Endpoint.Request;

public class ValidatedUserInfoRequestModel : ValidatedBaseModel
{
    public List<Claim> Claims { get; set; }

    public string Token { get; set; }

    public SecurityKey Key { get; set; }

    public JwtSecurityToken DecodedToken { get; set; }
}
