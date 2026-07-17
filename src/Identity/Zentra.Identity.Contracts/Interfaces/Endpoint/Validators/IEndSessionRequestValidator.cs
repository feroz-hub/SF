using System.Security.Claims;
using Zentra.Domain.Models.Endpoint.Request;

namespace Zentra.Service.Interfaces.Interfaces.Endpoint.Validators;

public interface IEndSessionRequestValidator
{
    Task<ValidatedEndSessionRequestModel> ValidateRequestAsync(Dictionary<string, string> requestCollection,
        ClaimsPrincipal user);

    Task<ValidatedEndSessionCallbackRequestModel> ValidateCallbackRequestAsync(
        Dictionary<string, string> requestCollection);
}
