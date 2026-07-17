using System.Security.Claims;
using HCL.CS.Domain.Models.Endpoint.Request;

namespace HCL.CS.Service.Interfaces.Interfaces.Endpoint.Validators;

public interface IEndSessionRequestValidator
{
    Task<ValidatedEndSessionRequestModel> ValidateRequestAsync(Dictionary<string, string> requestCollection,
        ClaimsPrincipal user);

    Task<ValidatedEndSessionCallbackRequestModel> ValidateCallbackRequestAsync(
        Dictionary<string, string> requestCollection);
}
