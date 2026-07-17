using Zentra.Domain.Models.Endpoint.Request;

namespace Zentra.Service.Interfaces.Interfaces.Endpoint;

public interface IDiscoveryService
{
    Task<Dictionary<string, object>> GenerateDiscoveryMetaData(DiscoveryRequestModel request);
}
