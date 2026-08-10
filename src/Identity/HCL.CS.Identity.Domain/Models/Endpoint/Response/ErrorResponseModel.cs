/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using System.ComponentModel;

namespace HCL.CS.Domain.Models.Endpoint.Response;

public class ErrorResponseModel
{
    public bool IsError { get; set; } = true;

    [DisplayName("error")] public string ErrorCode { get; set; }

    [DisplayName("error_description")] public string ErrorDescription { get; set; }
}

public class ErrorResponseResultModel
{
    public string error { get; set; }

    public string error_description { get; set; }
}
