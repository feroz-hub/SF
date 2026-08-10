/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using HCL.CS.Domain;
using HCL.CS.Domain.Enums;

namespace HCL.CS.Service.Interfaces.Interfaces.Api;

public partial interface IUserAccountService
{
    Task<FrameworkResult> GenerateEmailConfirmationTokenAsync(string username);

    Task<FrameworkResult> VerifyEmailConfirmationTokenAsync(string username, string emailToken);

    Task<FrameworkResult> GeneratePhoneNumberConfirmationTokenAsync(string username);

    Task<FrameworkResult> VerifyPhoneNumberConfirmationTokenAsync(string username, string smsToken);

    Task<FrameworkResult> GeneratePasswordResetTokenAsync(string username,
        NotificationTypes notificationType = NotificationTypes.Email);

    Task<FrameworkResult> GenerateUserTokenAsync(string username, string purpose, string templateName,
        NotificationTypes notificationType = NotificationTypes.Email);

    Task<FrameworkResult> VerifyUserTokenAsync(string username, string purpose, string token);

    Task<FrameworkResult> GenerateEmailTwoFactorTokenAsync(string username);

    Task<FrameworkResult> VerifyEmailTwoFactorTokenAsync(string username, string emailToken);

    Task<FrameworkResult> GenerateSmsTwoFactorTokenAsync(string username);

    Task<FrameworkResult> VerifySmsTwoFactorTokenAsync(string username, string smsToken);
}
