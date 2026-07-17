using Zentra.Domain.Enums;
using Zentra.Domain.Models.Endpoint.Response;

namespace Zentra.Domain.Models.Endpoint;

public class ParsedSecretModel : ErrorResponseModel
{
    public string ClientId { get; set; }

    public object Credential { get; set; }

    public string Type { get; set; }

    public ParseMethods ParseMethod { get; set; }
}
