using Zentra.Domain;
using Zentra.Domain.Models.Endpoint;

namespace Zentra.Service.Interfaces.Interfaces.Api;

public interface IClientServices
{
    Task<ClientsModel> RegisterClientAsync(ClientsModel clientsModel);

    Task<ClientsModel> UpdateClientAsync(ClientsModel clientsModel);

    Task<FrameworkResult> DeleteClientAsync(string clientId);

    Task<ClientsModel> GenerateClientSecret(string clientId);

    Task<ClientsModel> GetClientAsync(string clientId);

    Task<Dictionary<string, string>> GetAllClientAsync();
}
