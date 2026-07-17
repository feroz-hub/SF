using HCL.CS.Domain.Models.Endpoint;

namespace HCL.CS.Service.Interfaces.Interfaces.Endpoint.Validators;

public interface ISecretValidator
{
    Task<bool> ValidateSecretAsync(ClientsModel client, ParsedSecretModel parsedSecret);
}
