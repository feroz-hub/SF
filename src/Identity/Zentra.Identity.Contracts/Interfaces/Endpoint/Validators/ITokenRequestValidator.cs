using Zentra.Domain.Models.Endpoint.Request;
using Zentra.Domain.Models.Endpoint.Validation;

namespace Zentra.Service.Interfaces.Interfaces.Endpoint.Validators;

public interface ITokenRequestValidator
{
    Task<ValidatedTokenRequestModel> ValidateTokenRequestAsync(Dictionary<string, string> requestCollection,
        ClientSecretValidationModel clientValidationModel);
}
