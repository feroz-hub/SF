/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using HCL.CS.Domain;
using HCL.CS.Domain.Constants;
using HCL.CS.Domain.Constants.Api;
using HCL.CS.Domain.Entities.Api;
using HCL.CS.Domain.Enums;
using HCL.CS.Domain.ErrorCodes;
using HCL.CS.Domain.Models.Api;
using HCL.CS.Service.Implementation.Api.Extension;
using HCL.CS.Service.Implementation.Api.Utils;
using HCL.CS.Service.Interfaces.Interfaces.Api;

namespace HCL.CS.Service.Implementation.Api.Services;

public partial class UserAccountService : SecurityBase, IUserAccountService
{
    public virtual async Task<FrameworkResult> GenerateEmailConfirmationTokenAsync(string username)
    {
        try
        {
            var (token, error, user) = await GenerateEmailConfirmationToken(username);
            if (!string.IsNullOrWhiteSpace(error)) return frameworkResultService.Failed<FrameworkResult>(error);
            if (user.IdentityProviderType != IdentityProvider.Local
                || !string.Equals(user.AuthenticationSource, "LOCAL", StringComparison.OrdinalIgnoreCase))
                return frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.RestrictedApiForLdapUser);

            if (securityConfig.SystemSettings.EmailConfig.EmailNotificationType == EmailNotificationType.Link)
                token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            return await SendNotification(NotificationTypes.Email, user, NotificationConstants.EmailVerification,
                token);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Error while generating email confirmation token.");
            throw;
        }
    }

    public virtual async Task<FrameworkResult> VerifyEmailConfirmationTokenAsync(string username, string emailToken)
    {
        if (string.IsNullOrWhiteSpace(username))
            return frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.InvalidUsername);

        if (string.IsNullOrWhiteSpace(emailToken))
            return frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.InvalidEmailConfirmationTokenProvided);

        try
        {
            var user = await userManager.FindByNameAsync(username);
            if (user != null)
            {
                if (user.IdentityProviderType != IdentityProvider.Local
                    || !string.Equals(user.AuthenticationSource, "LOCAL", StringComparison.OrdinalIgnoreCase)
                    || user.EmailConfirmed)
                {
                    await AuditLocalEmailConfirmationAsync(
                        user,
                        SecurityAuditEventTypes.LocalEmailConfirmationRejected,
                        "REJECTED",
                        ApiErrorCodes.AuthLocalEmailConfirmationInvalid);
                    return frameworkResultService.Failed<FrameworkResult>(
                        ApiErrorCodes.AuthLocalEmailConfirmationInvalid);
                }

                if (securityConfig.SystemSettings.EmailConfig.EmailNotificationType == EmailNotificationType.Link)
                    emailToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(emailToken));

                var concurrencyStamp = user.ConcurrencyStamp;
                loggerService.WriteTo(Log.Debug,
                    "Entered in verify email confirmation token for user: " + user.UserName);
                var result = await userManager.ConfirmEmailAsync(user, emailToken);
                if (result.Succeeded)
                {
                    await userManagementUnitOfWork.SetModifiedStatusAsync(user, concurrencyStamp);
                    var saveResult = await userManagementUnitOfWork.SaveChangesAsync();
                    if (saveResult.Status == ResultStatus.Succeeded)
                        await AuditLocalEmailConfirmationAsync(
                            user,
                            SecurityAuditEventTypes.LocalEmailConfirmed,
                            "SUCCEEDED");
                    return saveResult;
                }

                await AuditLocalEmailConfirmationAsync(
                    user,
                    SecurityAuditEventTypes.LocalEmailConfirmationRejected,
                    "REJECTED",
                    ApiErrorCodes.AuthLocalEmailConfirmationInvalid);
                return frameworkResultService.Failed(result.ConstructIdentityErrorAsList());
            }

            return frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.InvalidUserId);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Error while verifying email confirmation token.");
            throw;
        }
    }

    public virtual async Task<FrameworkResult> GeneratePhoneNumberConfirmationTokenAsync(string username)
    {
        try
        {
            var (token, error, user) = await GeneratePhoneNumberConfirmationToken(username);
            if (!string.IsNullOrWhiteSpace(error)) return frameworkResultService.Failed<FrameworkResult>(error);

            return await SendNotification(NotificationTypes.SMS, user, NotificationConstants.PhoneNumberVerification,
                token);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Error while generating sms confirmation token.");
            throw;
        }
    }

    public virtual async Task<FrameworkResult> VerifyPhoneNumberConfirmationTokenAsync(string username, string smsToken)
    {
        if (string.IsNullOrWhiteSpace(username))
            return frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.InvalidUsername);

        if (string.IsNullOrWhiteSpace(smsToken))
            return frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.InvalidPhoneConfirmationTokenProvided);

        try
        {
            var user = await userManager.FindByNameAsync(username);
            if (user != null)
            {
                var concurrencyStamp = user.ConcurrencyStamp;
                loggerService.WriteTo(Log.Debug,
                    "Entered in verify phoneNumber confirmation token for user: " + user.UserName);
                var result = await userManager.ChangePhoneNumberAsync(user, user.PhoneNumber, smsToken);
                if (result.Succeeded)
                {
                    await userManagementUnitOfWork.SetModifiedStatusAsync(user, concurrencyStamp);
                    return await userManagementUnitOfWork.SaveChangesAsync();
                }

                return frameworkResultService.Failed(result.ConstructIdentityErrorAsList());
            }

            return frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.InvalidUserId);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Error while verifying phoneNumber confirmation token.");
            throw;
        }
    }

    public virtual async Task<FrameworkResult> GeneratePasswordResetTokenAsync(string username,
        NotificationTypes notificationType = NotificationTypes.Email)
    {
        if (string.IsNullOrWhiteSpace(username))
            return frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.InvalidUsername);

        try
        {
            var user = await userManager.FindByNameAsync(username);
            if (user != null)
            {
                if (user.IdentityProviderType == IdentityProvider.Ldap)
                    return frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.RestrictedApiForLdapUser);

                loggerService.WriteTo(Log.Debug, "Entered in generate password reset token for user: " + user.UserName);
                var token = await userManager.GeneratePasswordResetTokenAsync(user);
                if (!string.IsNullOrWhiteSpace(token))
                    return await SendNotification(notificationType, user, NotificationConstants.ResetPasswordUsingToken,
                        token);

                return frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.InvalidResetTokenGenerated);
            }

            return frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.InvalidUserId);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Error while generating password reset token.");
            throw;
        }
    }

    public virtual async Task<FrameworkResult> GenerateUserTokenAsync(string username, string purpose,
        string templateName, NotificationTypes notificationType = NotificationTypes.Email)
    {
        if (string.IsNullOrWhiteSpace(username))
            return frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.InvalidUsername);

        if (string.IsNullOrWhiteSpace(purpose))
            return frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.InvalidPurpose);

        try
        {
            var user = await userManager.FindByNameAsync(username);
            if (user != null)
            {
                loggerService.WriteTo(Log.Debug, "Entered in generate user token for user: " + user.UserName);
                var token =
                    await userManager.GenerateUserTokenAsync(user, TokenOptions.DefaultEmailProvider, purpose);
                if (!string.IsNullOrWhiteSpace(token))
                {
                    if (string.IsNullOrWhiteSpace(templateName)) templateName = NotificationConstants.DefaultTemplate;

                    return await SendNotification(notificationType, user, templateName, token);
                }

                return frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.InvalidUserTokenGenerated);
            }

            return frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.InvalidUserId);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Error while generating user token.");
            throw;
        }
    }

    public virtual async Task<FrameworkResult> VerifyUserTokenAsync(string username, string purpose, string token)
    {
        if (string.IsNullOrWhiteSpace(username))
            return frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.InvalidUsername);

        try
        {
            var user = await userManager.FindByNameAsync(username);
            if (user != null) return await VerifyUserTokenAsync(user, purpose, token);

            return frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.InvalidUserId);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Error while verifying user Token.");
            throw;
        }
    }

    public virtual async Task<FrameworkResult> GenerateEmailTwoFactorTokenAsync(string username)
    {
        return await GenerateTwoFactorTokenAsync(username, NotificationTypes.Email);
    }

    public virtual async Task<FrameworkResult> VerifyEmailTwoFactorTokenAsync(string username, string emailToken)
    {
        return await VerifyTwoFactorTokenAsync(username, emailToken, NotificationTypes.Email);
    }

    public virtual async Task<FrameworkResult> GenerateSmsTwoFactorTokenAsync(string username)
    {
        return await GenerateTwoFactorTokenAsync(username, NotificationTypes.SMS);
    }

    public virtual async Task<FrameworkResult> VerifySmsTwoFactorTokenAsync(string username, string smsToken)
    {
        return await VerifyTwoFactorTokenAsync(username, smsToken, NotificationTypes.SMS);
    }

    private async Task<FrameworkResult> VerifyUserTokenAsync(Users user, string purpose, string token)
    {
        if (string.IsNullOrWhiteSpace(purpose))
            return frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.InvalidPurpose);

        if (string.IsNullOrWhiteSpace(token))
            return frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.InvalidUserToken);

        try
        {
            var concurrencyStamp = user.ConcurrencyStamp;
            loggerService.WriteTo(Log.Debug, "Entered in verify user Token for user: " + user.UserName);
            var result =
                await userManager.VerifyUserTokenAsync(
                    user,
                    TokenOptions.DefaultEmailProvider,
                    purpose,
                    token);
            if (!result) return frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.InvalidUserToken);

            await userManagementUnitOfWork.SetModifiedStatusAsync(user, concurrencyStamp);
            return await userManagementUnitOfWork.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Error while verifying user Token.");
            throw;
        }
    }

    private async Task<FrameworkResult> GenerateTwoFactorTokenAsync(string username, NotificationTypes notificationType)
    {
        if (string.IsNullOrWhiteSpace(username))
            return frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.InvalidUsername);

        try
        {
            var user = await userManager.FindByNameAsync(username);
            if (user != null)
            {
                if (!user.TwoFactorEnabled)
                    return frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.TwoFactorNotEnabledForUser);

                var provider = string.Empty;
                if (notificationType == NotificationTypes.Email)
                    provider = Constants.DefaultEmailProvider;
                else if (notificationType == NotificationTypes.SMS) provider = Constants.DefaultPhoneProvider;

                loggerService.WriteTo(Log.Debug, "Entered in generate two factor token for user: " + user.UserName);
                var token = await userManager.GenerateTwoFactorTokenAsync(user, provider);
                if (!string.IsNullOrWhiteSpace(token))
                    return await SendNotification(notificationType, user, NotificationConstants.GenerateTwoFactorToken,
                        token);

                return frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.InvalidTwoFactorTokenGenerated);
            }

            return frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.InvalidUserId);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Error while generating two factor token.");
            throw;
        }
    }

    private async Task<FrameworkResult> VerifyTwoFactorTokenAsync(
        string username,
        string token,
        NotificationTypes notificationTypes)
    {
        if (string.IsNullOrWhiteSpace(username))
            return frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.InvalidUsername);

        if (string.IsNullOrWhiteSpace(token))
            return frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.InvalidTwoFactorTokenProvided);

        try
        {
            var user = await userManager.FindByNameAsync(username);
            if (user != null)
            {
                if (!user.TwoFactorEnabled)
                    return frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.TwoFactorNotEnabledForUser);

                loggerService.WriteTo(Log.Debug, "Entered in verify two factor token for user: " + user.UserName);

                var provider = string.Empty;
                if (notificationTypes == NotificationTypes.Email)
                    provider = Constants.DefaultEmailProvider;
                else if (notificationTypes == NotificationTypes.SMS) provider = Constants.DefaultPhoneProvider;

                var concurrencyStamp = user.ConcurrencyStamp;
                var result = await userManager.VerifyTwoFactorTokenAsync(user, provider, token);
                if (!result)
                    return frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.InvalidTwoFactorTokenProvided);

                await userManagementUnitOfWork.SetModifiedStatusAsync(user, concurrencyStamp);
                return await userManagementUnitOfWork.SaveChangesAsync();
            }

            return frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.InvalidUserId);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Error while verifying two factor token.");
            throw;
        }
    }

    private async Task<(string, string, Users)> GenerateEmailConfirmationToken(string username)
    {
        var token = string.Empty;
        if (string.IsNullOrWhiteSpace(username)) return (token, ApiErrorCodes.InvalidUsername, null);

        try
        {
            var user = await userManager.FindByNameAsync(username);
            if (user != null)
            {
                loggerService.WriteTo(Log.Debug,
                    "Entered in generate email confirmation token for user: " + user.UserName);
                token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                if (string.IsNullOrWhiteSpace(token))
                    return (token, ApiErrorCodes.InvalidEmailConfirmationTokenGenerated, null);

                return (token, string.Empty, user);
            }

            return (token, ApiErrorCodes.InvalidUserId, null);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Error while generating email confirmation token.");
            throw;
        }
    }

    private async Task<(string, string, Users)> GeneratePhoneNumberConfirmationToken(string username)
    {
        var token = string.Empty;
        if (string.IsNullOrWhiteSpace(username)) return (token, ApiErrorCodes.InvalidUsername, null);

        try
        {
            var user = await userManager.FindByNameAsync(username);
            if (user != null)
            {
                loggerService.WriteTo(Log.Debug,
                    "Entered in generate sms confirmation token for user: " + user.UserName);
                token = await userManager.GenerateChangePhoneNumberTokenAsync(user, user.PhoneNumber);
                if (string.IsNullOrWhiteSpace(token))
                    return (token, ApiErrorCodes.InvalidPhoneConfirmationTokenGenerated, null);

                return (token, string.Empty, user);
            }

            return (token, ApiErrorCodes.InvalidUserId, null);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Error while generating sms confirmation token.");
            throw;
        }
    }

    private async Task<FrameworkResult> SendNotification(NotificationTypes notificationType, Users user, string purpose,
        string token)
    {
        var notificationUtils =
            new NotificationUtil(emailSender, smsSender, frameworkResultService,
                securityConfig.SystemSettings.EmailConfig);

        FrameworkResult frameworkResult = null;
        if (notificationType == NotificationTypes.Email)
            return await notificationUtils.SendEmailAsync(user, purpose, token);

        if (notificationType == NotificationTypes.SMS)
            return await notificationUtils.SendSmsAsync(user, purpose, token);

        return frameworkResult;
    }

    private Task AuditLocalEmailConfirmationAsync(
        Users user,
        string eventType,
        string result,
        string? reasonCode = null)
    {
        return securityAuditService.WriteAsync(new SecurityAuditEventModel
        {
            EventType = eventType,
            ActorUserId = user.Id.ToString(),
            AuthenticationSource = "LOCAL",
            Result = result,
            ReasonCode = reasonCode
        });
    }
}
