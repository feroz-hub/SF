/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

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
