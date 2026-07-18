/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using System.Security.Claims;
using Newtonsoft.Json.Linq;
using HCL.CS.Domain.Constants;
using HCL.CS.Domain.Enums;
using HCL.CS.Domain.Models.Api;
using HCL.CS.ProxyService.Routes.Extension;
using HCL.CS.Service.Interfaces.Interfaces.Api.Wrapper;

namespace HCL.CS.ProxyService.Routes;

internal partial class ApiGateway : BaseApiServiceInstance, IApiGateway
{
    private async Task<bool> LockUser(string jsonContent)
    {
        var userId = jsonContent.JsonDeserialize<Guid>();
        var frameworkResult = await UserAccountService.LockUserAsync(userId);
        return await GenerateApiResults(frameworkResult);
    }

    private async Task<bool> LockUserWithEndDate(string jsonContent)
    {
        var jsonObjects = JObject.Parse(jsonContent);
        var userId = jsonObjects[ApiRouteParameterConstants.UserId].ToObject<Guid>();
        var enddate = jsonObjects[ApiRouteParameterConstants.EndDate].ToObject<DateTime>();

        var frameworkResult = await UserAccountService.LockUserAsync(userId, enddate);
        return await GenerateApiResults(frameworkResult);
    }

    private async Task<bool> AddAdminClaim(string jsonContent)
    {
        var userClaimModel = jsonContent.JsonDeserialize<UserClaimModel>();
        var frameworkResult = await UserAccountService.AddAdminClaimAsync(userClaimModel);
        return await GenerateApiResults(frameworkResult);
    }

    private async Task<bool> AddAdminClaimList(string jsonContent)
    {
        var roleModel = jsonContent.JsonDeserialize<IList<UserClaimModel>>();
        var frameworkResult = await UserAccountService.AddAdminClaimAsync(roleModel);
        return await GenerateApiResults(frameworkResult);
    }

    private async Task<bool> AddClaim(string jsonContent)
    {
        var userClaimmodel = jsonContent.JsonDeserialize<UserClaimModel>();
        var frameworkResult = await UserAccountService.AddClaimAsync(userClaimmodel);
        return await GenerateApiResults(frameworkResult);
    }

    private async Task<bool> AddClaimList(string jsonContent)
    {
        var roleModel = jsonContent.JsonDeserialize<IList<UserClaimModel>>();
        var frameworkResult = await UserAccountService.AddClaimAsync(roleModel);
        return await GenerateApiResults(frameworkResult);
    }

    private async Task<bool> AddSecurityQuestion(string jsonContent)
    {
        var securityQuestionModel = jsonContent.JsonDeserialize<SecurityQuestionModel>();
        var frameworkResult = await UserAccountService.AddSecurityQuestionAsync(securityQuestionModel);
        return await GenerateApiResults(frameworkResult);
    }

    private async Task<bool> AddUserRole(string jsonContent)
    {
        var userRoleModel = jsonContent.JsonDeserialize<UserRoleModel>();
        var frameworkResult = await UserAccountService.AddUserRoleAsync(userRoleModel);
        return await GenerateApiResults(frameworkResult);
    }

    private async Task<bool> AddUserRolesList(string jsonContent)
    {
        var userRoleModel = jsonContent.JsonDeserialize<IList<UserRoleModel>>();
        var frameworkResult = await UserAccountService.AddUserRolesAsync(userRoleModel);
        return await GenerateApiResults(frameworkResult);
    }

    private async Task<bool> AddUserSecurityQuestion(string jsonContent)
    {
        var userSecurityQuestionModel = jsonContent.JsonDeserialize<UserSecurityQuestionModel>();
        var frameworkResult = await UserAccountService.AddUserSecurityQuestionAsync(userSecurityQuestionModel);
        return await GenerateApiResults(frameworkResult);
    }

    private async Task<bool> AddUserSecurityQuestionList(string jsonContent)
    {
        var userSecurityQuestionModel = jsonContent.JsonDeserialize<IList<UserSecurityQuestionModel>>();
        var frameworkResult = await UserAccountService.AddUserSecurityQuestionAsync(userSecurityQuestionModel);
        return await GenerateApiResults(frameworkResult);
    }

    private async Task<bool> ChangePassword(string jsonContent)
    {
        var jsonObjects = JObject.Parse(jsonContent);
        var userId = jsonObjects[ApiRouteParameterConstants.UserId].ToObject<Guid>();
        var currentPassword = jsonObjects[ApiRouteParameterConstants.CurrentPassword].ToObject<string>();
        var newPassword = jsonObjects[ApiRouteParameterConstants.NewPassword].ToObject<string>();

        var frameworkResult = await UserAccountService.ChangePasswordAsync(userId, currentPassword, newPassword);
        return await GenerateApiResults(frameworkResult);
    }

    private async Task<bool> DeleteSecurityQuestion(string jsonContent)
    {
        var securityQuestionId = jsonContent.JsonDeserialize<Guid>();

        var frameworkResult = await UserAccountService.DeleteSecurityQuestionAsync(securityQuestionId);
        return await GenerateApiResults(frameworkResult);
    }

    private async Task<bool> DeleteUserById(string jsonContent)
    {
        var userId = jsonContent.JsonDeserialize<Guid>();

        var frameworkResult = await UserAccountService.DeleteUserAsync(userId);
        return await GenerateApiResults(frameworkResult);
    }

    private async Task<bool> DeleteUserByName(string jsonContent)
    {
        var userName = jsonContent.JsonDeserialize<string>();

        var frameworkResult = await UserAccountService.DeleteUserAsync(userName);
        return await GenerateApiResults(frameworkResult);
    }

    private async Task<bool> DeleteUserSecurityQuestion(string jsonContent)
    {
        var userSecurityQuestionModel = jsonContent.JsonDeserialize<UserSecurityQuestionModel>();

        var frameworkResult = await UserAccountService.DeleteUserSecurityQuestionAsync(userSecurityQuestionModel);
        return await GenerateApiResults(frameworkResult);
    }

    private async Task<bool> DeleteUserSecurityQuestionList(string jsonContent)
    {
        var userSecurityQuestionModel = jsonContent.JsonDeserialize<List<UserSecurityQuestionModel>>();

        var frameworkResult = await UserAccountService.DeleteUserSecurityQuestionAsync(userSecurityQuestionModel);
        return await GenerateApiResults(frameworkResult);
    }

    private async Task<bool> GenerateEmailConfirmationToken(string jsonContent)
    {
        var username = jsonContent.JsonDeserialize<string>();
        var frameworkResult = await UserAccountService.GenerateEmailConfirmationTokenAsync(username);
        return await GenerateApiResults(frameworkResult);
    }

    private async Task<bool> GeneratePhoneNumberConfirmationToken(string jsonContent)
    {
        var username = jsonContent.JsonDeserialize<string>();
        var frameworkResult = await UserAccountService.GeneratePhoneNumberConfirmationTokenAsync(username);
        return await GenerateApiResults(frameworkResult);
    }

    private async Task<bool> GenerateEmailTwoFactorToken(string jsonContent)
    {
        var username = jsonContent.JsonDeserialize<string>();
        var frameworkResult = await UserAccountService.GenerateEmailTwoFactorTokenAsync(username);
        return await GenerateApiResults(frameworkResult);
    }

    private async Task<bool> GeneratePasswordResetToken(string jsonContent)
    {
        var jsonObjects = JObject.Parse(jsonContent);
        var username = jsonObjects[ApiRouteParameterConstants.UserName].ToObject<string>();
        var notificatype = jsonObjects[ApiRouteParameterConstants.NotificationType].ToObject<NotificationTypes>();

        var frameworkResult = await UserAccountService.GeneratePasswordResetTokenAsync(username, notificatype);
        return await GenerateApiResults(frameworkResult);
    }

    private async Task<bool> GenerateSmsTwoFactorToken(string jsonContent)
    {
        var username = jsonContent.JsonDeserialize<string>();
        var frameworkResult = await UserAccountService.GenerateSmsTwoFactorTokenAsync(username);
        return await GenerateApiResults(frameworkResult);
    }

    private async Task<bool> GenerateUserTokenAsync(string jsonContent)
    {
        var jsonObjects = JObject.Parse(jsonContent);
        var username = jsonObjects[ApiRouteParameterConstants.UserName].ToObject<string>();
        var purpose = jsonObjects[ApiRouteParameterConstants.TokenPurpose].ToObject<string>();
        var template = jsonObjects[ApiRouteParameterConstants.NotificationTemplate].ToObject<string>();
        var notificatype = jsonObjects[ApiRouteParameterConstants.NotificationType].ToObject<NotificationTypes>();

        var frameworkResult =
            await UserAccountService.GenerateUserTokenAsync(username, purpose, template, notificatype);
        return await GenerateApiResults(frameworkResult);
    }

    private async Task<bool> GetAllSecurityQuestions(string jsonContent)
    {
        var frameworkResult = await UserAccountService.GetAllSecurityQuestionsAsync();
        await GenerateApiResults(frameworkResult);
        return true;
    }

    private async Task<bool> GetAllTwoFactorType(string jsonContent)
    {
        var frameworkResult = await UserAccountService.GetAllTwoFactorTypeAsync();
        await GenerateApiResults(frameworkResult);
        return true;
    }

    private async Task<bool> GetClaims(string jsonContent)
    {
        var userId = jsonContent.JsonDeserialize<Guid>();
        var frameworkResult = await UserAccountService.GetClaimsAsync(userId);
        await GenerateApiResults(frameworkResult);
        return true;
    }

    private async Task<bool> GetUserByEmail(string jsonContent)
    {
        var email = jsonContent.JsonDeserialize<string>();
        var frameworkResult = await UserAccountService.GetUserByEmailAsync(email);
        await GenerateApiResults(frameworkResult);
        return true;
    }

    private async Task<bool> GetAllUsers(string jsonContent)
    {
        var userList = await UserAccountService.GetAllUsersAsync();
        await GenerateApiResults(userList);
        return true;
    }

    private async Task<bool> GetUserById(string jsonContent)
    {
        var userId = jsonContent.JsonDeserialize<Guid>();
        var frameworkResult = await UserAccountService.GetUserByIdAsync(userId);
        await GenerateApiResults(frameworkResult);
        return true;
    }

    private async Task<bool> GetUserByName(string jsonContent)
    {
        var userName = jsonContent.JsonDeserialize<string>();
        var frameworkResult = await UserAccountService.GetUserByNameAsync(userName);
        await GenerateApiResults(frameworkResult);
        return true;
    }

    private async Task<bool> GetUserClaims(string jsonContent)
    {
        var userId = jsonContent.JsonDeserialize<Guid>();
        var frameworkResult = await UserAccountService.GetUserClaimsAsync(userId);
        await GenerateApiResults(frameworkResult);
        return true;
    }

    private async Task<bool> GetAdminUserClaims(string jsonContent)
    {
        var userId = jsonContent.JsonDeserialize<Guid>();
        var frameworkResult = await UserAccountService.GetAdminUserClaimsAsync(userId);
        await GenerateApiResults(frameworkResult);
        return true;
    }

    private async Task<bool> GetUserRoleClaimsById(string jsonContent)
    {
        var userId = jsonContent.JsonDeserialize<Guid>();
        var frameworkResult = await UserAccountService.GetUserRoleClaimsByIdAsync(userId);
        await GenerateApiResults(frameworkResult);
        return true;
    }

    private async Task<bool> GetUserRoleClaimsByName(string jsonContent)
    {
        var userName = jsonContent.JsonDeserialize<string>();
        var frameworkResult = await UserAccountService.GetUserRoleClaimsByNameAsync(userName);
        await GenerateApiResults(frameworkResult);
        return true;
    }

    private async Task<bool> GetUserRoles(string jsonContent)
    {
        var userId = jsonContent.JsonDeserialize<Guid>();
        var frameworkResult = await UserAccountService.GetUserRolesAsync(userId);
        await GenerateApiResults(frameworkResult);
        return true;
    }

    private async Task<bool> GetUserSecurityQuestions(string jsonContent)
    {
        var userId = jsonContent.JsonDeserialize<Guid>();
        var frameworkResult = await UserAccountService.GetUserSecurityQuestionsAsync(userId);
        await GenerateApiResults(frameworkResult);
        return true;
    }

    private async Task<bool> GetUsersForClaim(string jsonContent)
    {
        var jsonObjects = JObject.Parse(jsonContent);
        var claimType = jsonObjects[ApiRouteParameterConstants.ClaimType].ToObject<string>();
        var claimValue = jsonObjects[ApiRouteParameterConstants.ClaimValue].ToObject<string>();
        var frameworkResult = await UserAccountService.GetUsersForClaimAsync(claimType, claimValue);
        await GenerateApiResults(frameworkResult);
        return true;
    }

    private async Task<bool> GetUsersInRole(string jsonContent)
    {
        var roleName = jsonContent.JsonDeserialize<string>();
        var frameworkResult = await UserAccountService.GetUsersInRoleAsync(roleName);
        await GenerateApiResults(frameworkResult);
        return true;
    }

    private async Task<bool> IsUserExistsByClaimPrincipal(string jsonContent)
    {
        var claimsPrincipal = jsonContent.JsonDeserialize<ClaimsPrincipal>();
        var frameworkResult = await UserAccountService.IsUserExistsAsync(claimsPrincipal);
        await GenerateApiResults(frameworkResult);
        return true;
    }

    private async Task<bool> IsUserExistsById(string jsonContent)
    {
        var userId = jsonContent.JsonDeserialize<Guid>();
        var frameworkResult = await UserAccountService.IsUserExistsAsync(userId);
        await GenerateApiResults(frameworkResult);
        return true;
    }

    private async Task<bool> IsUserExistsByName(string jsonContent)
    {
        var userName = jsonContent.JsonDeserialize<string>();
        var frameworkResult = await UserAccountService.IsUserExistsAsync(userName);
        await GenerateApiResults(frameworkResult);
        return true;
    }

    private async Task<bool> RegisterUser(string jsonContent)
    {
        var userModel = jsonContent.JsonDeserialize<UserModel>();
        var frameworkResult = await UserAccountService.RegisterUserAsync(userModel);
        await GenerateApiResults(frameworkResult);
        return true;
    }

    private async Task<bool> RemoveAdminClaim(string jsonContent)
    {
        var userClaimModel = jsonContent.JsonDeserialize<UserClaimModel>();
        var frameworkResult = await UserAccountService.RemoveAdminClaimAsync(userClaimModel);
        await GenerateApiResults(frameworkResult);
        return true;
    }

    private async Task<bool> RemoveAdminClaimList(string jsonContent)
    {
        var userClaimModel = jsonContent.JsonDeserialize<IList<UserClaimModel>>();
        var frameworkResult = await UserAccountService.RemoveAdminClaimAsync(userClaimModel);
        await GenerateApiResults(frameworkResult);
        return true;
    }

    private async Task<bool> RemoveClaim(string jsonContent)
    {
        var userClaimModel = jsonContent.JsonDeserialize<UserClaimModel>();
        var frameworkResult = await UserAccountService.RemoveClaimAsync(userClaimModel);
        await GenerateApiResults(frameworkResult);
        return true;
    }

    private async Task<bool> RemoveClaimList(string jsonContent)
    {
        var userClaimModel = jsonContent.JsonDeserialize<IList<UserClaimModel>>();
        var frameworkResult = await UserAccountService.RemoveClaimAsync(userClaimModel);
        await GenerateApiResults(frameworkResult);
        return true;
    }

    private async Task<bool> RemoveUserRole(string jsonContent)
    {
        var userRoleModel = jsonContent.JsonDeserialize<UserRoleModel>();
        var frameworkResult = await UserAccountService.RemoveUserRoleAsync(userRoleModel);
        await GenerateApiResults(frameworkResult);
        return true;
    }

    private async Task<bool> RemoveUserRoleList(string jsonContent)
    {
        var userRoleModel = jsonContent.JsonDeserialize<IList<UserRoleModel>>();
        var frameworkResult = await UserAccountService.RemoveUserRolesAsync(userRoleModel);
        await GenerateApiResults(frameworkResult);
        return true;
    }

    private async Task<bool> ReplaceClaim(string jsonContent)
    {
        var existingUserClaimModel = jsonContent.JsonDeserialize<UserClaimModel>();
        var newUserClaimModel = jsonContent.JsonDeserialize<UserClaimModel>();
        var frameworkResult = await UserAccountService.ReplaceClaimAsync(existingUserClaimModel, newUserClaimModel);
        await GenerateApiResults(frameworkResult);
        return true;
    }

    private async Task<bool> ResetPassword(string jsonContent)
    {
        var jsonObjects = JObject.Parse(jsonContent);
        var username = jsonObjects[ApiRouteParameterConstants.UserName].ToObject<string>();
        var passwordResetToken = jsonObjects[ApiRouteParameterConstants.PasswordResetToken].ToObject<string>();
        var newPassword = jsonObjects[ApiRouteParameterConstants.NewPassword].ToObject<string>();
        var frameworkResult = await UserAccountService.ResetPasswordAsync(username, passwordResetToken, newPassword);
        await GenerateApiResults(frameworkResult);
        return true;
    }

    private async Task<bool> SetTwoFactorEnabled(string jsonContent)
    {
        var jsonObjects = JObject.Parse(jsonContent);
        var userId = jsonObjects[ApiRouteParameterConstants.UserId].ToObject<Guid>();
        var enabled = jsonObjects[ApiRouteParameterConstants.Enabled].ToObject<bool>();
        var frameworkResult = await UserAccountService.SetTwoFactorEnabledAsync(userId, enabled);
        await GenerateApiResults(frameworkResult);
        return true;
    }

    private async Task<bool> UnLockUserByToken(string jsonContent)
    {
        var jsonObjects = JObject.Parse(jsonContent);
        var username = jsonObjects[ApiRouteParameterConstants.UserName].ToObject<string>();
        var token = jsonObjects[ApiRouteParameterConstants.UserToken].ToObject<string>();
        var purpose = jsonObjects[ApiRouteParameterConstants.TokenPurpose].ToObject<string>();
        var frameworkResult = await UserAccountService.UnLockUserAsync(username, token, purpose);
        await GenerateApiResults(frameworkResult);
        return true;
    }

    private async Task<bool> UnLockUser(string jsonContent)
    {
        // Roshan Bashyam : Commented because not sending as an object.Commented Code to be removed after discussion
        //JObject jsonObjects = JObject.Parse(jsonContent);
        //var userId = jsonObjects[ApiRouteParameterConstants.UserId].ToObject<Guid>();
        var username = jsonContent.JsonDeserialize<string>();
        var frameworkResult = await UserAccountService.UnLockUserAsync(username);
        await GenerateApiResults(frameworkResult);
        return true;
    }

    private async Task<bool> UnLockUserByuserSecurityQuestions(string jsonContent)
    {
        var jsonObjects = JObject.Parse(jsonContent);
        var username = jsonObjects[ApiRouteParameterConstants.UserName].ToObject<string>();
        var securityQuestionModel = jsonObjects[ApiRouteParameterConstants.ListOfUserSecurityQuestions]
            .ToObject<List<UserSecurityQuestionModel>>();
        var frameworkResult = await UserAccountService.UnlockUserAsync(username, securityQuestionModel);
        await GenerateApiResults(frameworkResult);
        return true;
    }

    private async Task<bool> UpdateSecurityQuestion(string jsonContent)
    {
        var securityQuestionModel = jsonContent.JsonDeserialize<SecurityQuestionModel>();
        var frameworkResult = await UserAccountService.UpdateSecurityQuestionAsync(securityQuestionModel);
        await GenerateApiResults(frameworkResult);
        return true;
    }

    private async Task<bool> UpdateUser(string jsonContent)
    {
        var userModel = jsonContent.JsonDeserialize<UserModel>();
        var frameworkResult = await UserAccountService.UpdateUserAsync(userModel);
        await GenerateApiResults(frameworkResult);
        return true;
    }

    private async Task<bool> UpdateUserSecurityQuestion(string jsonContent)
    {
        var userSecurityQuestionModel = jsonContent.JsonDeserialize<UserSecurityQuestionModel>();
        var frameworkResult = await UserAccountService.UpdateUserSecurityQuestionAsync(userSecurityQuestionModel);
        await GenerateApiResults(frameworkResult);
        return true;
    }

    private async Task<bool> UpdateUserTwoFactorType(string jsonContent)
    {
        var jsonObjects = JObject.Parse(jsonContent);
        var userId = jsonObjects[ApiRouteParameterConstants.UserId].ToObject<Guid>();
        var twoFactorType = jsonObjects[ApiRouteParameterConstants.TwoFactorType].ToObject<TwoFactorType>();

        var frameworkResult = await UserAccountService.UpdateUserTwoFactorTypeAsync(userId, twoFactorType);
        await GenerateApiResults(frameworkResult);
        return true;
    }

    private async Task<bool> VerifyEmailConfirmationToken(string jsonContent)
    {
        var jsonObjects = JObject.Parse(jsonContent);
        var username = jsonObjects[ApiRouteParameterConstants.UserName].ToObject<string>();
        var emailToken = jsonObjects[ApiRouteParameterConstants.EmailToken].ToObject<string>();
        var frameworkResult = await UserAccountService.VerifyEmailConfirmationTokenAsync(username, emailToken);
        await GenerateApiResults(frameworkResult);
        return true;
    }

    private async Task<bool> VerifyEmailTwoFactorToken(string jsonContent)
    {
        var jsonObjects = JObject.Parse(jsonContent);
        var username = jsonObjects[ApiRouteParameterConstants.UserName].ToObject<string>();
        var emailToken = jsonObjects[ApiRouteParameterConstants.EmailToken].ToObject<string>();
        var frameworkResult = await UserAccountService.VerifyEmailTwoFactorTokenAsync(username, emailToken);
        await GenerateApiResults(frameworkResult);
        return true;
    }

    private async Task<bool> VerifyPhoneNumberConfirmationToken(string jsonContent)
    {
        var jsonObjects = JObject.Parse(jsonContent);
        var username = jsonObjects[ApiRouteParameterConstants.UserName].ToObject<string>();
        var smsToken = jsonObjects[ApiRouteParameterConstants.SmsToken].ToObject<string>();
        var frameworkResult = await UserAccountService.VerifyPhoneNumberConfirmationTokenAsync(username, smsToken);
        await GenerateApiResults(frameworkResult);
        return true;
    }

    private async Task<bool> VerifySmsTwoFactorToken(string jsonContent)
    {
        var jsonObjects = JObject.Parse(jsonContent);
        var username = jsonObjects[ApiRouteParameterConstants.UserName].ToObject<string>();
        var smsToken = jsonObjects[ApiRouteParameterConstants.SmsToken].ToObject<string>();
        var frameworkResult = await UserAccountService.VerifySmsTwoFactorTokenAsync(username, smsToken);
        await GenerateApiResults(frameworkResult);
        return true;
    }

    private async Task<bool> VerifyUserToken(string jsonContent)
    {
        var jsonObjects = JObject.Parse(jsonContent);
        var username = jsonObjects[ApiRouteParameterConstants.UserName].ToObject<string>();
        var purpose = jsonObjects[ApiRouteParameterConstants.TokenPurpose].ToObject<string>();
        var token = jsonObjects[ApiRouteParameterConstants.UserToken].ToObject<string>();
        var frameworkResult = await UserAccountService.VerifyUserTokenAsync(username, purpose, token);
        await GenerateApiResults(frameworkResult);
        return true;
    }
}
