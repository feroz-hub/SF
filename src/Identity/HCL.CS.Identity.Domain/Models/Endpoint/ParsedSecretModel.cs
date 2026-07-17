using HCL.CS.Domain.Enums;
using HCL.CS.Domain.Models.Endpoint.Response;

namespace HCL.CS.Domain.Models.Endpoint;

public class ParsedSecretModel : ErrorResponseModel
{
    public string ClientId { get; set; }

    public object Credential { get; set; }

    public string Type { get; set; }

    public ParseMethods ParseMethod { get; set; }
}
