using HCL.CS.Domain.Models.Endpoint;
using HCL.CS.Domain.Models.Endpoint.Request;

namespace HCL.CS.Service.Interfaces.Interfaces.Endpoint.Validators;

public interface IIntrospectionRequestValidator
{
    Task<ValidatedIntrospectionRequestModel> ValidateIntrospectionRequestAsync(
        Dictionary<string, string> requestCollection, ClientsModel client);
}
