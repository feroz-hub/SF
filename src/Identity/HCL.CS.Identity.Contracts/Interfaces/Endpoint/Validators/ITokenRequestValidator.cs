using HCL.CS.Domain.Models.Endpoint.Request;
using HCL.CS.Domain.Models.Endpoint.Validation;

namespace HCL.CS.Service.Interfaces.Interfaces.Endpoint.Validators;

public interface ITokenRequestValidator
{
    Task<ValidatedTokenRequestModel> ValidateTokenRequestAsync(Dictionary<string, string> requestCollection,
        ClientSecretValidationModel clientValidationModel);
}
