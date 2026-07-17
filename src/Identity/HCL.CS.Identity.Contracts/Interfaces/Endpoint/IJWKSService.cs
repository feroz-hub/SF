using HCL.CS.Domain.Models.Endpoint.Response;

namespace HCL.CS.Service.Interfaces.Interfaces.Endpoint;

public interface IJWKSService
{
    Task<IList<JsonWebKeyResponseModel>> ProcessJWKSInformations();
}
