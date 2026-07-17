using HCL.CS.Domain.Models.Endpoint.Request;

namespace HCL.CS.Service.Interfaces.Interfaces.Endpoint;

public interface IUserInfoServices
{
    Task<Dictionary<string, object>> ProcessUserInfoAsync(ValidatedUserInfoRequestModel userInfoRequestValidation);
}
