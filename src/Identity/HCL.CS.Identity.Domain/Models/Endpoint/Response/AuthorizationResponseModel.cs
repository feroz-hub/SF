/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using System.ComponentModel;
using HCL.CS.Domain.Models.Endpoint.Request;

namespace HCL.CS.Domain.Models.Endpoint.Response;

public class AuthorizationResponseModel : ErrorResponseModel
{
    public ValidatedAuthorizeRequestModel Request { get; set; }

    [DisplayName("redirect_uri")] public string RedirectUri => Request?.RedirectUri;

    [DisplayName("state")] public string State => Request?.State;

    [DisplayName("scope")] public string Scope { get; set; }

    [DisplayName("id_token")] public string IdentityToken { get; set; }

    [DisplayName("access_token")] public string AccessToken { get; set; }

    [DisplayName("refresh_token")] public string RefreshToken { get; set; }

    public int AccessTokenLifetime { get; set; }

    [DisplayName("code")] public string Code { get; set; }

    [DisplayName("session_state")] public string SessionState { get; set; }
}
