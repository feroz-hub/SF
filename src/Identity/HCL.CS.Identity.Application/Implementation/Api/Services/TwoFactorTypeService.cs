/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using HCL.CS.Domain;
using HCL.CS.Domain.Enums;
using HCL.CS.Domain.ErrorCodes;
using HCL.CS.Service.Extension;
using HCL.CS.Service.Implementation.Api.Extension;
using HCL.CS.Service.Interfaces.Interfaces.Api;

namespace HCL.CS.Service.Implementation.Api.Services;

public partial class UserAccountService : SecurityBase, IUserAccountService
{
    public virtual async Task<IList<string>> GetAllTwoFactorTypeAsync()
    {
        try
        {
            var twoFactorTypes = Enum.GetValues(typeof(TwoFactorType)).Cast<string>().ToList();
            return await Task.FromResult(twoFactorTypes);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Error while getting all two factor type.");
            throw;
        }
    }

    public virtual async Task<FrameworkResult> SetTwoFactorEnabledAsync(Guid userId, bool enabled)
    {
        if (!userId.IsValid()) return frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.InvalidUserId);

        try
        {
            var user = await FindByIdAsync(userId);
            if (user != null)
            {
                var concurrencyStamp = user.ConcurrencyStamp;
                loggerService.WriteTo(Log.Debug,
                    "Entered into set two factor authentication for specified user : " + user.UserName);
                var result = await userManager.SetTwoFactorEnabledAsync(user, enabled);
                if (!result.Succeeded) return frameworkResultService.Failed(result.ConstructIdentityErrorAsList());

                await userManagementUnitOfWork.SetModifiedStatusAsync(user, concurrencyStamp);
                return await userManagementUnitOfWork.SaveChangesAsync();
            }

            return frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.InvalidUserId);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Error while Setting two Factor enabled.");
            throw;
        }
    }

    public virtual async Task<FrameworkResult> UpdateUserTwoFactorTypeAsync(Guid userId, TwoFactorType twoFactorType)
    {
        if (!userId.IsValid()) return frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.InvalidUserId);

        try
        {
            var existingUser = await FindByIdAsync(userId);
            if (existingUser != null)
            {
                var concurrencyStamp = existingUser.ConcurrencyStamp;
                loggerService.WriteTo(Log.Debug,
                    "Entered in update user two factor type for user: " + existingUser.UserName);

                existingUser.TwoFactorType = twoFactorType;
                existingUser.TwoFactorEnabled = true;
                if (twoFactorType == TwoFactorType.None) existingUser.TwoFactorEnabled = false;

                var result = await userManager.UpdateAsync(existingUser);
                if (!result.Succeeded) return frameworkResultService.Failed(result.ConstructIdentityErrorAsList());

                await userManagementUnitOfWork.SetModifiedStatusAsync(existingUser, concurrencyStamp);
                return await userManagementUnitOfWork.SaveChangesAsync();
            }

            return frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.InvalidUserId);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Error while update user two factor type.");
            throw;
        }
    }
}
