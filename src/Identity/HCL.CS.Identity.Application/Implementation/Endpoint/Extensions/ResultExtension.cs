using HCL.CS.Domain.Models.Endpoint.Request;
using HCL.CS.Domain.Models.Endpoint.Response;
using HCL.CS.Service.Implementation.Endpoint.Results;
using HCL.CS.Service.Interfaces.Interfaces.Endpoint;

namespace HCL.CS.Service.Implementation.Endpoint.Extensions;

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
