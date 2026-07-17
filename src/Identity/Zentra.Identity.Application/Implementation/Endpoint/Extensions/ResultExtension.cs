using Zentra.Domain.Models.Endpoint.Request;
using Zentra.Domain.Models.Endpoint.Response;
using Zentra.Service.Implementation.Endpoint.Results;
using Zentra.Service.Interfaces.Interfaces.Endpoint;

namespace Zentra.Service.Implementation.Endpoint.Extensions;

internal static class ResultExtension
{
    internal static AuthorizeResult Error(this ValidatedAuthorizeRequestModel validationModel,
        ISessionManagementService session, string error, string errorDescription = null)
    {
        var validResponse = new AuthorizationResponseModel
        {
            Request = validationModel,
            IsError = true,
            ErrorCode = error,
            ErrorDescription = errorDescription
        };

        return new AuthorizeResult(validResponse, session);
    }

    internal static ErrorResult Error(this string error, string errorDescription = null)
    {
        var response = new ErrorResponseModel
        {
            ErrorCode = error,
            ErrorDescription = errorDescription
        };
        return new ErrorResult(response);
    }

    internal static UserInfoResult UserInfoError(this string error, string errorDescription = null)
    {
        var response = new ErrorResponseModel
        {
            ErrorCode = error,
            ErrorDescription = errorDescription
        };
        return new UserInfoResult(null, response);
    }
}
