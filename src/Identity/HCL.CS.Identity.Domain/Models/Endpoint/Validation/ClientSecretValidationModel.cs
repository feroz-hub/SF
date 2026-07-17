using HCL.CS.Domain.Models.Endpoint.Response;

namespace HCL.CS.Domain.Models.Endpoint.Validation;

public class ClientSecretValidationModel : ErrorResponseModel
{
    public ClientsModel Client { get; set; }

    public ParsedSecretModel Secret { get; set; }
}
