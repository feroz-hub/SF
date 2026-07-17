using HCL.CS.Domain.Models.Endpoint.Request;

namespace HCL.CS.Service.Interfaces.Interfaces.Endpoint;

public interface IDiscoveryService
{
    Task<Dictionary<string, object>> GenerateDiscoveryMetaData(DiscoveryRequestModel request);
}
