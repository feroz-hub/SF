/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using HCL.CS.Domain.Enums;

namespace HCL.CS.Domain.Models.Api.Response;

public class SignInResponseModel
{
    public bool Succeeded { get; set; } = false;

    public bool IsLockedOut { get; set; } = false;

    public bool IsNotAllowed { get; set; } = false;

    public bool RequiresTwoFactor { get; set; } = false;

    public TwoFactorType TwoFactorVerificationMode { get; set; }

    public bool TwoFactorVerificationCodeSent { get; set; } = false;

    public string Message { get; set; }

    public string ErrorCode { get; set; }

    public string UserVerificationCode { get; set; }
}
