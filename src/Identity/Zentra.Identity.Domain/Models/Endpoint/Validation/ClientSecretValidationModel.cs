using Zentra.Domain.Models.Endpoint.Response;

namespace Zentra.Domain.Models.Endpoint.Validation;

public class ClientSecretValidationModel : ErrorResponseModel
{
    public ClientsModel Client { get; set; }

    public ParsedSecretModel Secret { get; set; }
}
