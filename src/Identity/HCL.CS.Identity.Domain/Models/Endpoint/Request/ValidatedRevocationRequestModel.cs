using HCL.CS.Domain.Models.Endpoint.Validation;

namespace HCL.CS.Domain.Models.Endpoint.Request;

public class ValidatedRevocationRequestModel : ValidatedBaseModel
{
    public string TokenTypeHint { get; set; }

    public string Token { get; set; }
}
