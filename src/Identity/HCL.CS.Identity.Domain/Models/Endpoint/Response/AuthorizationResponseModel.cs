using System.ComponentModel;
using HCL.CS.Domain.Models.Endpoint.Request;

namespace HCL.CS.Domain.Models.Endpoint.Response;

public class AuthorizationResponseModel : ErrorResponseModel
{
    public ValidatedAuthorizeRequestModel Request { get; set; }

    [DisplayName("redirect_uri")] public string RedirectUri => Request?.RedirectUri;

    [DisplayName("state")] public string State => Request?.State;

    [DisplayName("scope")] public string Scope { get; set; }

    [DisplayName("id_token")] public string IdentityToken { get; set; }

    [DisplayName("access_token")] public string AccessToken { get; set; }

    [DisplayName("refresh_token")] public string RefreshToken { get; set; }

    public int AccessTokenLifetime { get; set; }

    [DisplayName("code")] public string Code { get; set; }

    [DisplayName("session_state")] public string SessionState { get; set; }
}
