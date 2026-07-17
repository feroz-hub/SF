using System.ComponentModel;

namespace Zentra.Domain.Models.Endpoint.Response;

public class TokenResponseModel
{
    [DisplayName("token_type")] public string TokenType { get; set; }

    [DisplayName("id_token")] public string IdentityToken { get; set; }

    [DisplayName("access_token")] public string AccessToken { get; set; }

    [DisplayName("expires_in")] public int AccessTokenExpiresIn { get; set; }

    [DisplayName("refresh_token")] public string RefreshToken { get; set; }

    [DisplayName("scope")] public string Scope { get; set; }

    [DisplayName("state")] public string State { get; set; } = null;
}

public class TokenResponseResultModel
{
    public string id_token { get; set; }

    public string access_token { get; set; }

    public int expires_in { get; set; }

    public string token_type { get; set; }

    public string refresh_token { get; set; }

    public string scope { get; set; }
}
