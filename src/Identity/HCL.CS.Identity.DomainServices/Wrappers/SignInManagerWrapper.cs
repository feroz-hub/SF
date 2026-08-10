/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HCL.CS.DomainServices.Wrappers;

public class SignInManagerWrapper<TUser> : SignInManager<TUser>
    where TUser : class
{
    public SignInManagerWrapper(
        UserManagerWrapper<TUser> userManager,
        IHttpContextAccessor contextAccessor,
        IUserClaimsPrincipalFactory<TUser> claimsFactory,
        IOptions<IdentityOptions> optionsAccessor,
        ILogger<SignInManagerWrapper<TUser>> logger,
        IAuthenticationSchemeProvider schemes,
        IUserConfirmation<TUser> confirmation)
        : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
    {
    }

    public async Task<SignInResult> ExternalCredentialSignInAsync(TUser user, bool isPersistent)
    {
        ArgumentNullException.ThrowIfNull(user);

        var preSignInResult = await PreSignInCheck(user);
        if (preSignInResult is not null) return preSignInResult;

        return await SignInOrTwoFactorAsync(user, isPersistent);
    }

    public async Task<SignInResult> LocalCredentialSignInAsync(
        TUser user,
        string password,
        bool isPersistent,
        bool lockoutOnFailure)
    {
        ArgumentNullException.ThrowIfNull(user);

        if (await UserManager.IsLockedOutAsync(user)) return SignInResult.LockedOut;

        if (!await UserManager.CheckPasswordAsync(user, password))
        {
            if (lockoutOnFailure && UserManager.SupportsUserLockout)
            {
                await UserManager.AccessFailedAsync(user);
                if (await UserManager.IsLockedOutAsync(user)) return SignInResult.LockedOut;
            }

            return SignInResult.Failed;
        }

        if (UserManager.SupportsUserLockout) await UserManager.ResetAccessFailedCountAsync(user);
        return await SignInOrTwoFactorAsync(user, isPersistent);
    }
}
