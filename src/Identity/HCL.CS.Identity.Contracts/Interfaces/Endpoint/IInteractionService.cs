using HCL.CS.Domain.Models.Endpoint.Request;
using HCL.CS.Domain.Models.Endpoint.Response;

namespace HCL.CS.Service.Interfaces.Interfaces.Endpoint;

public interface IInteractionService
{
    Task<string> ConstructUserVerificationCode(string returnUrl, string userVerficationCode);

    Task<LogoutRequestModel> GetLogoutContextAsync(string logoutId);

    Task<ErrorResponseModel> GetErrorContextAsync(string errorId);
}
