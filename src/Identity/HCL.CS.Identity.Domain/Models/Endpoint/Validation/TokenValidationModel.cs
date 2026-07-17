using HCL.CS.Domain.Models.Endpoint.Response;

namespace HCL.CS.Domain.Models.Endpoint.Validation;

public class TokenValidationModel : ErrorResponseModel
{
    public SecurityTokensModel RefreshToken { get; set; }

    public ClientsModel Client { get; set; }
}
