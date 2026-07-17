using Zentra.Domain.Models.Endpoint;
using Zentra.Domain.Models.Endpoint.Request;

namespace Zentra.Service.Interfaces.Interfaces.Endpoint.Validators;

public interface IIntrospectionRequestValidator
{
    Task<ValidatedIntrospectionRequestModel> ValidateIntrospectionRequestAsync(
        Dictionary<string, string> requestCollection, ClientsModel client);
}
