using Zentra.Domain.Models.Endpoint.Response;

namespace Zentra.Service.Interfaces.Interfaces.Endpoint;

public interface IJWKSService
{
    Task<IList<JsonWebKeyResponseModel>> ProcessJWKSInformations();
}
