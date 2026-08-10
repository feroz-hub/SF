/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using Microsoft.AspNetCore.Identity;

namespace HCL.CS.Service.Interfaces.Interfaces.Api.Wrapper;

public interface IWindowsSignInManagerWrapper<TUser>
    where TUser : class
{
    Task<SignInResult> PasswordSignInAsync(TUser user, string password, bool lockoutOnFailure);

    Task<SignInResult> TwoFactorSignInAsync(Guid userId, string provider, string code);

    Task<SignInResult> TwoFactorAuthenticatorSignInAsync(Guid userId, string code);

    Task<SignInResult> TwoFactorRecoveryCodeSignInAsync(Guid userId, string recoveryCode);
}
