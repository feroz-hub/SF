using Zentra.Domain.Models.Endpoint;

namespace Zentra.Service.Interfaces.Interfaces.Endpoint.Validators;

public interface ISecretValidator
{
    Task<bool> ValidateSecretAsync(ClientsModel client, ParsedSecretModel parsedSecret);
}
