using Microsoft.AspNetCore.Http;
using Zentra.Domain.Models.Endpoint.Validation;

namespace Zentra.Service.Interfaces.Interfaces.Endpoint.Validators;

public interface IClientSecretValidator
{
    Task<ClientSecretValidationModel> ValidateClientSecretAsync(HttpContext context);
}
