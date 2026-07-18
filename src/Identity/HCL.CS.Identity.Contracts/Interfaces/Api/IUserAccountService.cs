/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using System.Security.Claims;
using HCL.CS.Domain;
using HCL.CS.Domain.Models.Api;

namespace HCL.CS.Service.Interfaces.Interfaces.Api;

public partial interface IUserAccountService
{
    Task<FrameworkResult> RegisterUserAsync(UserModel user);

    Task<FrameworkResult> UpdateUserAsync(UserModel userModel);

    Task<FrameworkResult> DeleteUserAsync(string username);

    Task<FrameworkResult> DeleteUserAsync(Guid userId);

    Task<FrameworkResult> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword);

    Task<FrameworkResult> ResetPasswordAsync(string username, string passwordResetToken, string newPassword);

    Task<FrameworkResult> LockUserAsync(Guid userId);

    Task<FrameworkResult> LockUserAsync(Guid userId, DateTime? dateTime);

    Task<FrameworkResult> UnLockUserAsync(string username);

    Task<FrameworkResult> UnLockUserAsync(string username, string token, string purpose);

    Task<FrameworkResult> UnlockUserAsync(string username, IList<UserSecurityQuestionModel> userSecurityQuestions);

    Task<UserModel> GetUserByNameAsync(string userName);

    Task<UserModel> GetUserByEmailAsync(string email);

    Task<UserModel> GetUserByIdAsync(Guid userId);

    Task<IList<UserModel>> GetUsersForClaimAsync(string claimType, string claimValue);

    Task<bool> IsUserExistsAsync(ClaimsPrincipal claimsPrincipal);

    Task<bool> IsUserExistsAsync(Guid userId);

    Task<bool> IsUserExistsAsync(string userName);

    Task<IList<UserDisplayModel>> GetAllUsersAsync();
}
