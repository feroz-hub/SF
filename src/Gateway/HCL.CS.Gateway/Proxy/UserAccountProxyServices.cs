/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using HCL.CS.Domain;
using HCL.CS.Domain.Entities.Api;
using HCL.CS.Domain.Entities.Endpoint;
using HCL.CS.Domain.Enums;
using HCL.CS.Domain.Models.Api;
using HCL.CS.Domain.Models.Api.Response;
using HCL.CS.DomainServices;
using HCL.CS.DomainServices.Infra;
using HCL.CS.DomainServices.UnitOfWork.Api;
using HCL.CS.DomainServices.Wrappers;
using HCL.CS.Service.Implementation.Api.Services;
using HCL.CS.Service.Interfaces.Interfaces.Api;
using HCL.CS.Service.Interfaces.Interfaces.Api.Wrapper;

namespace HCL.CS.ProxyService.Proxy;

public sealed class UserAccountProxyServices(
    UserManagerWrapper<Users> userManager,
    ILoggerInstance instance,
    IResourceStringHandler resourceStringHandler,
    IUserManagementUnitOfWork userManagementUnitOfWork,
    IMapper mapper,
    HclCsConfig securityConfig,
    IEmailService emailService,
    ISmsService smsService,
    IFrameworkResultService frameworkResultService,
    IRepository<UserSecurityQuestions> userSecurityQuestionsRepository,
    IPasswordHasher<Users> passwordHasher,
    IRoleService roleService,
    IRepository<SecurityTokens> securityTokenRepository,
    IApiValidator apiValidator,
    RoleManagerWrapper<Roles> roleManager)
    : UserAccountService(userManager,
        instance,
        resourceStringHandler,
        userManagementUnitOfWork,
        mapper,
        securityConfig,
        emailService,
        smsService,
        frameworkResultService,
        userSecurityQuestionsRepository,
        passwordHasher,
        roleService,
        securityTokenRepository,
        roleManager), IUserAccountService
{
    private readonly IFrameworkResultService frameworkResult = frameworkResultService;

    public override async Task<FrameworkResult> AddAdminClaimAsync(IList<UserClaimModel> userClaimModels)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.AddAdminClaimAsync(userClaimModels);
    }

    public override async Task<FrameworkResult> AddAdminClaimAsync(UserClaimModel userClaimModel)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.AddAdminClaimAsync(userClaimModel);
    }

    public override async Task<FrameworkResult> AddClaimAsync(UserClaimModel userClaimModel)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.AddClaimAsync(userClaimModel);
    }

    public override async Task<FrameworkResult> AddClaimAsync(IList<UserClaimModel> userClaimModels)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.AddClaimAsync(userClaimModels);
    }

    public override async Task<FrameworkResult> AddSecurityQuestionAsync(SecurityQuestionModel securityQuestionModel)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.AddSecurityQuestionAsync(securityQuestionModel);
    }

    public override async Task<FrameworkResult> AddUserRoleAsync(UserRoleModel userRoleModel)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.AddUserRoleAsync(userRoleModel);
    }

    public override async Task<FrameworkResult> AddUserRolesAsync(IList<UserRoleModel> modelList)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.AddUserRolesAsync(modelList);
    }

    public override async Task<FrameworkResult> AddUserSecurityQuestionAsync(
        UserSecurityQuestionModel userSecurityQuestionModel)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.AddUserSecurityQuestionAsync(userSecurityQuestionModel);
    }

    public override async Task<FrameworkResult> AddUserSecurityQuestionAsync(
        IList<UserSecurityQuestionModel> userSecurityQuestionModels)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.AddUserSecurityQuestionAsync(userSecurityQuestionModels);
    }

    public override async Task<FrameworkResult> ChangePasswordAsync(Guid userId, string currentPassword,
        string newPassword)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.ChangePasswordAsync(userId, currentPassword, newPassword);
    }

    public override async Task<FrameworkResult> DeleteSecurityQuestionAsync(Guid securityQuestionId)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.DeleteSecurityQuestionAsync(securityQuestionId);
    }

    public override async Task<FrameworkResult> DeleteUserAsync(string username)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.DeleteUserAsync(username);
    }

    public override async Task<FrameworkResult> DeleteUserAsync(Guid userId)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.DeleteUserAsync(userId);
    }

    public override async Task<FrameworkResult> DeleteUserSecurityQuestionAsync(
        UserSecurityQuestionModel userSecurityQuestionModel)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.DeleteUserSecurityQuestionAsync(userSecurityQuestionModel);
    }

    public override async Task<FrameworkResult> DeleteUserSecurityQuestionAsync(
        IList<UserSecurityQuestionModel> userSecurityQuestionModels)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.DeleteUserSecurityQuestionAsync(userSecurityQuestionModels);
    }

    public override async Task<FrameworkResult> GenerateEmailConfirmationTokenAsync(string username)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.GenerateEmailConfirmationTokenAsync(username);
    }

    public override async Task<FrameworkResult> GeneratePhoneNumberConfirmationTokenAsync(string username)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.GeneratePhoneNumberConfirmationTokenAsync(username);
    }

    public override async Task<FrameworkResult> GenerateEmailTwoFactorTokenAsync(string username)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.GenerateEmailTwoFactorTokenAsync(username);
    }

    public override async Task<FrameworkResult> GeneratePasswordResetTokenAsync(string username,
        NotificationTypes notificationType = NotificationTypes.Email)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.GeneratePasswordResetTokenAsync(username, notificationType);
    }

    public override async Task<FrameworkResult> GenerateSmsTwoFactorTokenAsync(string username)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.GenerateSmsTwoFactorTokenAsync(username);
    }

    public override async Task<FrameworkResult> GenerateUserTokenAsync(string username, string purpose,
        string templateName, NotificationTypes notificationType = NotificationTypes.Email)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.GenerateUserTokenAsync(username, purpose, templateName, notificationType);
    }

    public override async Task<IList<SecurityQuestionModel>> GetAllSecurityQuestionsAsync()
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed)
            frameworkResult.ThrowCustomMessage(result.Errors.FirstOrDefault().Description);

        return await base.GetAllSecurityQuestionsAsync();
    }

    public override async Task<IList<string>> GetAllTwoFactorTypeAsync()
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed)
            frameworkResult.ThrowCustomMessage(result.Errors.FirstOrDefault().Description);

        return await base.GetAllTwoFactorTypeAsync();
    }

    public override async Task<IList<Claim>> GetClaimsAsync(Guid userId)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed)
            frameworkResult.ThrowCustomMessage(result.Errors.FirstOrDefault().Description);

        return await base.GetClaimsAsync(userId);
    }

    public override async Task<UserModel> GetUserByEmailAsync(string email)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed)
            frameworkResult.ThrowCustomMessage(result.Errors.FirstOrDefault().Description);

        return await base.GetUserByEmailAsync(email);
    }

    public override async Task<UserModel> GetUserByIdAsync(Guid userId)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed)
            frameworkResult.ThrowCustomMessage(result.Errors.FirstOrDefault().Description);

        return await base.GetUserByIdAsync(userId);
    }

    public override async Task<UserModel> GetUserByNameAsync(string userName)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed)
            frameworkResult.ThrowCustomMessage(result.Errors.FirstOrDefault().Description);

        return await base.GetUserByNameAsync(userName);
    }

    public override async Task<IList<UserClaimModel>> GetUserClaimsAsync(Guid userId)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed)
            frameworkResult.ThrowCustomMessage(result.Errors.FirstOrDefault().Description);

        return await base.GetUserClaimsAsync(userId);
    }

    public override async Task<IList<UserClaimModel>> GetAdminUserClaimsAsync(Guid userId)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed)
            frameworkResult.ThrowCustomMessage(result.Errors.FirstOrDefault().Description);

        return await base.GetAdminUserClaimsAsync(userId);
    }

    public override async Task<UserPermissionsResponseModel> GetUserRoleClaimsByIdAsync(Guid userId)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed)
            frameworkResult.ThrowCustomMessage(result.Errors.FirstOrDefault().Description);

        return await base.GetUserRoleClaimsByIdAsync(userId);
    }

    public override async Task<UserPermissionsResponseModel> GetUserRoleClaimsByNameAsync(string userName)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed)
            frameworkResult.ThrowCustomMessage(result.Errors.FirstOrDefault().Description);

        return await base.GetUserRoleClaimsByNameAsync(userName);
    }

    public override async Task<IList<string>> GetUserRolesAsync(Guid userId)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed)
            frameworkResult.ThrowCustomMessage(result.Errors.FirstOrDefault().Description);

        return await base.GetUserRolesAsync(userId);
    }

    public override async Task<IList<UserSecurityQuestionModel>> GetUserSecurityQuestionsAsync(Guid userId)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed)
            frameworkResult.ThrowCustomMessage(result.Errors.FirstOrDefault().Description);

        return await base.GetUserSecurityQuestionsAsync(userId);
    }

    public override async Task<IList<UserModel>> GetUsersForClaimAsync(string claimType, string claimValue)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed)
            frameworkResult.ThrowCustomMessage(result.Errors.FirstOrDefault().Description);

        return await base.GetUsersForClaimAsync(claimType, claimValue);
    }

    public override async Task<IList<UserModel>> GetUsersInRoleAsync(string roleName)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed)
            frameworkResult.ThrowCustomMessage(result.Errors.FirstOrDefault().Description);

        return await base.GetUsersInRoleAsync(roleName);
    }

    public override async Task<bool> IsUserExistsAsync(ClaimsPrincipal claimsPrincipal)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed)
            frameworkResult.ThrowCustomMessage(result.Errors.FirstOrDefault().Description);

        return await base.IsUserExistsAsync(claimsPrincipal);
    }

    public override async Task<bool> IsUserExistsAsync(Guid userId)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed)
            frameworkResult.ThrowCustomMessage(result.Errors.FirstOrDefault().Description);

        return await base.IsUserExistsAsync(userId);
    }

    public override async Task<bool> IsUserExistsAsync(string userName)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed)
            frameworkResult.ThrowCustomMessage(result.Errors.FirstOrDefault().Description);

        return await base.IsUserExistsAsync(userName);
    }

    public override async Task<FrameworkResult> LockUserAsync(Guid userId)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed)
            frameworkResult.ThrowCustomMessage(result.Errors.FirstOrDefault().Description);

        return await base.LockUserAsync(userId);
    }

    public override async Task<FrameworkResult> LockUserAsync(Guid userId, DateTime? dateTime)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed)
            frameworkResult.ThrowCustomMessage(result.Errors.FirstOrDefault().Description);

        return await base.LockUserAsync(userId, dateTime);
    }

    public override async Task<FrameworkResult> RegisterUserAsync(UserModel user)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.RegisterUserAsync(user);
    }

    public override async Task<FrameworkResult> RemoveAdminClaimAsync(UserClaimModel userClaimModel)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.RemoveAdminClaimAsync(userClaimModel);
    }

    public override async Task<FrameworkResult> RemoveAdminClaimAsync(IList<UserClaimModel> userClaimModel)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.RemoveAdminClaimAsync(userClaimModel);
    }

    public override async Task<FrameworkResult> RemoveClaimAsync(UserClaimModel userClaimModel)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.RemoveClaimAsync(userClaimModel);
    }

    public override async Task<FrameworkResult> RemoveClaimAsync(IList<UserClaimModel> userClaimModel)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.RemoveClaimAsync(userClaimModel);
    }

    public override async Task<FrameworkResult> RemoveUserRoleAsync(UserRoleModel userRoleModel)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.RemoveUserRoleAsync(userRoleModel);
    }

    public override async Task<FrameworkResult> RemoveUserRolesAsync(IList<UserRoleModel> modelList)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.RemoveUserRolesAsync(modelList);
    }

    public override async Task<FrameworkResult> ReplaceClaimAsync(UserClaimModel existingUserClaimModel,
        UserClaimModel newUserClaimModel)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.ReplaceClaimAsync(existingUserClaimModel, newUserClaimModel);
    }

    public override async Task<FrameworkResult> ResetPasswordAsync(string username, string passwordResetToken,
        string newPassword)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.ResetPasswordAsync(username, passwordResetToken, newPassword);
    }

    public override async Task<FrameworkResult> SetTwoFactorEnabledAsync(Guid userId, bool enabled)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.SetTwoFactorEnabledAsync(userId, enabled);
    }

    public override async Task<FrameworkResult> UnLockUserAsync(string username)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.UnLockUserAsync(username);
    }

    public override async Task<FrameworkResult> UnLockUserAsync(string username, string token, string purpose)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.UnLockUserAsync(username, token, purpose);
    }

    public override async Task<FrameworkResult> UnlockUserAsync(string username,
        IList<UserSecurityQuestionModel> userSecurityQuestions)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.UnlockUserAsync(username, userSecurityQuestions);
    }

    public override async Task<FrameworkResult> UpdateSecurityQuestionAsync(SecurityQuestionModel securityQuestionModel)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.UpdateSecurityQuestionAsync(securityQuestionModel);
    }

    public override async Task<FrameworkResult> UpdateUserAsync(UserModel userModel)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.UpdateUserAsync(userModel);
    }

    public override async Task<IList<UserDisplayModel>> GetAllUsersAsync()
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed)
            frameworkResult.ThrowCustomMessage(result.Errors.FirstOrDefault().Description);

        return await base.GetAllUsersAsync();
    }

    public override async Task<FrameworkResult> UpdateUserSecurityQuestionAsync(
        UserSecurityQuestionModel userSecurityQuestionModel)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.UpdateUserSecurityQuestionAsync(userSecurityQuestionModel);
    }

    public override async Task<FrameworkResult> UpdateUserTwoFactorTypeAsync(Guid userId, TwoFactorType twoFactorType)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.UpdateUserTwoFactorTypeAsync(userId, twoFactorType);
    }

    public override async Task<FrameworkResult> VerifyEmailConfirmationTokenAsync(string username, string emailToken)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.VerifyEmailConfirmationTokenAsync(username, emailToken);
    }

    public override async Task<FrameworkResult> VerifyEmailTwoFactorTokenAsync(string username, string emailToken)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.VerifyEmailTwoFactorTokenAsync(username, emailToken);
    }

    public override async Task<FrameworkResult> VerifyPhoneNumberConfirmationTokenAsync(string username,
        string smsToken)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.VerifyPhoneNumberConfirmationTokenAsync(username, smsToken);
    }

    public override async Task<FrameworkResult> VerifySmsTwoFactorTokenAsync(string username, string smsToken)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.VerifySmsTwoFactorTokenAsync(username, smsToken);
    }

    public override async Task<FrameworkResult> VerifyUserTokenAsync(string username, string purpose, string token)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.VerifyUserTokenAsync(username, purpose, token);
    }
}
