using Microsoft.AspNetCore.Http;
using HCL.CS.Domain.Models.Endpoint.Validation;

namespace HCL.CS.Service.Interfaces.Interfaces.Endpoint.Validators;

public interface IClientSecretValidator
{
    Task<ClientSecretValidationModel> ValidateClientSecretAsync(HttpContext context);
}
