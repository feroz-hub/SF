using Zentra.Domain.Models.Endpoint.Request;

namespace Zentra.Service.Interfaces.Interfaces.Endpoint;

public interface IUserInfoServices
{
    Task<Dictionary<string, object>> ProcessUserInfoAsync(ValidatedUserInfoRequestModel userInfoRequestValidation);
}
