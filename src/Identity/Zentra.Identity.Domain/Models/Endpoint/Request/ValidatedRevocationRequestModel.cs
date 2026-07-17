using Zentra.Domain.Models.Endpoint.Validation;

namespace Zentra.Domain.Models.Endpoint.Request;

public class ValidatedRevocationRequestModel : ValidatedBaseModel
{
    public string TokenTypeHint { get; set; }

    public string Token { get; set; }
}
