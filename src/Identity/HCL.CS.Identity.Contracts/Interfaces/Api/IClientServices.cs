using HCL.CS.Domain;
using HCL.CS.Domain.Models.Endpoint;

namespace HCL.CS.Service.Interfaces.Interfaces.Api;

public interface IClientServices
{
    Task<ClientsModel> RegisterClientAsync(ClientsModel clientsModel);

    Task<ClientsModel> UpdateClientAsync(ClientsModel clientsModel);

    Task<FrameworkResult> DeleteClientAsync(string clientId);

    Task<ClientsModel> GenerateClientSecret(string clientId);

    Task<ClientsModel> GetClientAsync(string clientId);

    Task<Dictionary<string, string>> GetAllClientAsync();
}
