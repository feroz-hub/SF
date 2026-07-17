using Zentra.Domain.Models.Endpoint.Response;

namespace Zentra.Domain.Models.Endpoint.Validation;

public class TokenValidationModel : ErrorResponseModel
{
    public SecurityTokensModel RefreshToken { get; set; }

    public ClientsModel Client { get; set; }
}
