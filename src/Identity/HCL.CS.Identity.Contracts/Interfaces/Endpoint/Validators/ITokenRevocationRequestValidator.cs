using HCL.CS.Domain.Models.Endpoint;
using HCL.CS.Domain.Models.Endpoint.Request;

namespace HCL.CS.Service.Interfaces.Interfaces.Endpoint.Validators;

public interface ITokenRevocationRequestValidator
{
    Task<ValidatedRevocationRequestModel> ValidateRevocationRequestAsync(
        Dictionary<string, string> requestCollection,
        ClientsModel client);
}
