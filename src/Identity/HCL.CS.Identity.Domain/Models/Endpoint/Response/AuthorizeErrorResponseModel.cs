using System.ComponentModel;

namespace HCL.CS.Domain.Models.Endpoint.Response;

public class AuthorizeErrorResponseModel : ErrorResponseModel
{
    [DisplayName("error_uri")] public virtual string ErrorUri { get; set; }

    [DisplayName("state")] public virtual string State { get; set; }

    // TODO Update the below 4 values where it is applicable.

    [DisplayName("trace_id")] public string TraceId { get; set; }

    [DisplayName("client_id")] public string ClientId { get; set; }

    [DisplayName("redirect_uri")] public string RedirectUri { get; set; }

    [DisplayName("response_mode")] public string ResponseMode { get; set; }
}
