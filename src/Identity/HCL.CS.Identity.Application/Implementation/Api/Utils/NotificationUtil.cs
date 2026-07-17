using HCL.CS.Domain;
using HCL.CS.Domain.Configurations.Api;
using HCL.CS.Domain.Constants;
using HCL.CS.Domain.Entities.Api;
using HCL.CS.Domain.Enums;
using HCL.CS.Domain.ErrorCodes;
using HCL.CS.Domain.Models.Api;
using HCL.CS.DomainServices.Infra;

namespace HCL.CS.Service.Implementation.Api.Utils;

internal class NotificationUtil
{
    private readonly EmailConfig config;
    private readonly IEmailService emailSender;
    private readonly IFrameworkResultService frameworkResult;
    private readonly ISmsService smsSender;

    internal NotificationUtil(
        IEmailService emailSender,
        ISmsService smsSender,
        IFrameworkResultService frameworkResult,
        EmailConfig config)
    {
        this.emailSender = emailSender;
        this.smsSender = smsSender;
        this.frameworkResult = frameworkResult;
        this.config = config;
    }

    internal async Task<FrameworkResult> TwoFactorNotificationAsync(Users user, string purpose, string token)
    {
        if (user.TwoFactorType == TwoFactorType.Email) return await SendEmailAsync(user, purpose, token);

        if (user.TwoFactorType == TwoFactorType.Sms) return await SendSmsAsync(user, purpose, token);

        return null;
    }

    internal async Task<FrameworkResult> SendEmailAsync(Users user, string purpose, string token)
    {
        if (!GlobalConfiguration.IsEmailConfigurationValid)
            return frameworkResult.Failed<FrameworkResult>(ApiErrorCodes.InvalidEmailConfiguration);

        var emailParameter = new Dictionary<string, string>();
        emailParameter.Add("{TOKEN}", token);

        var templateName = GetTemplateName(purpose);
        var notification = new NotificationInfoModel
        {
            UserId = user.Id,
            TemplateName = templateName,
            Activity = purpose,
            ToAddress = user.Email,
            Parameters = emailParameter
        };
        return await emailSender.SendEmailAsync(notification);
    }

    internal async Task<FrameworkResult> SendSmsAsync(Users user, string purpose, string token)
    {
        if (!GlobalConfiguration.IsSmsConfigurationValid)
            return frameworkResult.Failed<FrameworkResult>(ApiErrorCodes.InvalidSmsConfiguration);

        var smsParameter = new Dictionary<string, string>();
        smsParameter.Add("{TOKEN}", token);

        var templateName = GetTemplateName(purpose);
        var smsMessage = new NotificationInfoModel
        {
            UserId = user.Id,
            Activity = purpose,
            ToAddress = user.PhoneNumber,
            TemplateName = templateName,
            Parameters = smsParameter
        };
        return await smsSender.SendSmsAsync(smsMessage);
    }

    private string GetTemplateName(string purpose)
    {
        var templateName = string.Empty;
        switch (purpose)
        {
            case NotificationConstants.EmailVerification:
                if (config.EmailNotificationType == EmailNotificationType.Link)
                    templateName = NotificationConstants.EmailVerificationUsingLink;
                else if (config.EmailNotificationType == EmailNotificationType.Token)
                    templateName = NotificationConstants.EmailVerificationUsingToken;

                break;
            case NotificationConstants.PhoneNumberVerification:
                templateName = NotificationConstants.PhoneNumberVerificationToken;
                break;
            case NotificationConstants.GenerateTwoFactorToken:
                templateName = NotificationConstants.GenerateTwoFactorToken;
                break;
            case NotificationConstants.ResetPasswordUsingToken:
                templateName = NotificationConstants.ResetPasswordUsingToken;
                break;
            default:
                templateName = purpose;
                break;
        }

        return templateName;
    }
}
