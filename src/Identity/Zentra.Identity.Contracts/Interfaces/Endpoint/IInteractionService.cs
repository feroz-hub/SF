using Zentra.Domain.Models.Endpoint.Request;
using Zentra.Domain.Models.Endpoint.Response;

namespace Zentra.Service.Interfaces.Interfaces.Endpoint;

public interface IInteractionService
{
    Task<string> ConstructUserVerificationCode(string returnUrl, string userVerficationCode);

    Task<LogoutRequestModel> GetLogoutContextAsync(string logoutId);

    Task<ErrorResponseModel> GetErrorContextAsync(string errorId);
}
