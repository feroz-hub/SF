using Zentra.Domain.Models.Endpoint;
using Zentra.Domain.Models.Endpoint.Request;

namespace Zentra.Service.Interfaces.Interfaces.Endpoint.Validators;

public interface ITokenRevocationRequestValidator
{
    Task<ValidatedRevocationRequestModel> ValidateRevocationRequestAsync(
        Dictionary<string, string> requestCollection,
        ClientsModel client);
}
