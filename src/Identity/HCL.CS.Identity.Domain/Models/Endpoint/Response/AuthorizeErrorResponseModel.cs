/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using System.ComponentModel;

namespace HCL.CS.Domain.Models.Endpoint.Response;

public class AuthorizeErrorResponseModel : ErrorResponseModel
{
    [DisplayName("error_uri")] public virtual string ErrorUri { get; set; }

    [DisplayName("state")] public virtual string State { get; set; }

    // TODO Update the below 4 values where it is applicable.

    [DisplayName("trace_id")] public string TraceId { get; set; }

    [DisplayName("client_id")] public string ClientId { get; set; }

    [DisplayName("redirect_uri")] public string RedirectUri { get; set; }

    [DisplayName("response_mode")] public string ResponseMode { get; set; }
}
