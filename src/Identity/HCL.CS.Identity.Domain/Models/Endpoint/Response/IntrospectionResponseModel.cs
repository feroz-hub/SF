using System.ComponentModel;

namespace HCL.CS.Domain.Models.Endpoint.Response;

public class IntrospectionResponseModel
{
    [DisplayName("active")] public bool Active { get; set; }

    [DisplayName("client_id")] public string ClientId { get; set; }

    [DisplayName("username")] public string UserName { get; set; }

    [DisplayName("scope")] public string Scope { get; set; }

    [DisplayName("sub")] public string SubjectId { get; set; }

    [DisplayName("aud")] public string Audience { get; set; }

    [DisplayName("iss")] public string Issuer { get; set; }

    [DisplayName("exp")] public string ExpiresAt { get; set; }

    [DisplayName("iat")] public string IssuedAt { get; set; }
}
