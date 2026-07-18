/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using HCL.CS.Domain;
using HCL.CS.Domain.Configurations.Api;
using HCL.CS.Domain.Constants;
using HCL.CS.Domain.Entities.Api;
using HCL.CS.Domain.Enums;
using HCL.CS.Domain.ErrorCodes;
using HCL.CS.Domain.Models.Api;
using HCL.CS.DomainServices;
using HCL.CS.DomainServices.Infra;
using HCL.CS.DomainServices.Wrappers;
using HCL.CS.Infrastructure.Services.Extension;
using HCL.CS.Infrastructure.Services.Implementation.Providers;

namespace HCL.CS.Infrastructure.Services.Implementation;

internal class SmsService : ISmsService
{
    private readonly UserManagerWrapper<Users> csUserManager;
    private readonly IFrameworkResultService frameworkResultService;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly ILoggerService loggerService;
    private readonly IMapper mapper;
    private readonly SMSConfig smsConfig;
    private readonly IRepository<Notification> smsRepository;
    private readonly IRepository<NotificationProviderConfig> providerConfigRepository;
    private readonly NotificationProviderFactory providerFactory;
    private readonly NotificationTemplateSettings templateSettings;

    public SmsService(
        UserManagerWrapper<Users> userManager,
        HclCsConfig configSettings,
        ILoggerInstance loggerInstance,
        IRepository<Notification> smsRepository,
        IRepository<NotificationProviderConfig> providerConfigRepository,
        NotificationProviderFactory providerFactory,
        IMapper mapper,
        IFrameworkResultService frameworkResultService,
        IHttpContextAccessor httpContextAccessor)
    {
        this.smsRepository = smsRepository;
        this.providerConfigRepository = providerConfigRepository;
        this.providerFactory = providerFactory;
        this.mapper = mapper;
        this.httpContextAccessor = httpContextAccessor;
        templateSettings = configSettings.NotificationTemplateSettings;
        loggerService = loggerInstance.GetLoggerInstance(LoggerKeyConstants.DefaultLoggerKey);
        smsConfig = configSettings.SystemSettings.SMSConfig;
        this.frameworkResultService = frameworkResultService;
        csUserManager = userManager;
    }

    public async Task<FrameworkResult> SendSmsAsync(NotificationInfoModel notificationModel)
    {
        if (notificationModel == null)
            return frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.InvalidOrNullObject);

        if (string.IsNullOrWhiteSpace(notificationModel.TemplateName))
            return frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.InvalidTemplateName);

        try
        {
            var message = mapper.Map<NotificationInfoModel, SMSMessage>(notificationModel);
            string errorCode;
            (message, errorCode) = await ResolveSmsTemplate(message);
            if (!string.IsNullOrWhiteSpace(errorCode)) return frameworkResultService.Failed<FrameworkResult>(errorCode);

            var result = ValidateSmsMessage(message);
            if (result.Status == ResultStatus.Failed) return result;

            // Check for active DB-configured provider
            var activeProviderConfigs = await providerConfigRepository.GetAsync(
                x => x.ChannelType == 2 && x.IsActive);

            if (activeProviderConfigs != null && activeProviderConfigs.Any())
            {
                var providerConfig = activeProviderConfigs[0];
                await SendSmsViaProvider(message, providerConfig);
            }
            else
            {
                // Fallback to Twilio config from appsettings
                result = ValidateSmsConfiguration();
                if (result.Status == ResultStatus.Failed) return result;

                var accountSid = smsConfig.SMSAccountIdentification;
                var authToken = smsConfig.SMSAccountPassword;

                TwilioClient.Init(accountSid, authToken);
                var sendToNumber = message.To;
                var messageResource = await MessageResource.CreateAsync(
                    new PhoneNumber(sendToNumber),
                    from: new PhoneNumber(smsConfig.SMSAccountFrom),
                    body: message.Content);

                await SaveSmsNotification(message, messageResource);
            }

            loggerService.WriteTo(Log.Debug, "SMS sent");
            return frameworkResultService.Succeeded();
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "SMS delivery failed");
            throw;
        }
    }

    private async Task SendSmsViaProvider(SMSMessage message, NotificationProviderConfig providerConfig)
    {
        var config = JsonSerializer.Deserialize<Dictionary<string, string>>(providerConfig.ConfigJson)
                     ?? new Dictionary<string, string>();

        var smsMessage = new SmsMessage
        {
            From = config.GetValueOrDefault("FromNumber") ?? config.GetValueOrDefault("SenderName") ?? config.GetValueOrDefault("Originator"),
            To = message.To,
            Body = message.Content,
            CallbackUrl = config.GetValueOrDefault("StatusCallbackUrl")
        };

        // Save notification record
        var notification = mapper.Map<SMSMessage, Notification>(message);
        notification.MessageId = Guid.NewGuid().ToString();
        notification.Type = NotificationTypes.SMS;
        notification.Status = NotificationStatus.Initiated;
        notification.Sender = smsMessage.From;
        await smsRepository.InsertAsync(notification);
        await smsRepository.SaveChangesAsync();

        var provider = providerFactory.GetSmsProvider(providerConfig.ProviderName);
        loggerService.WriteTo(Log.Debug, $"Sending SMS via {provider.ProviderName}");

        var sendResult = await provider.SendAsync(smsMessage, config);

        if (sendResult.Success)
        {
            notification.MessageId = sendResult.MessageId ?? notification.MessageId;
            notification.Status = NotificationStatus.Delivered;
        }
        else
        {
            notification.Status = NotificationStatus.Failed;
            loggerService.WriteTo(Log.Error, $"Provider {provider.ProviderName} failed: {sendResult.ErrorMessage}");
        }

        await smsRepository.UpdateAsync(notification);
        await smsRepository.SaveChangesAsync();

        if (!sendResult.Success)
            throw new InvalidOperationException($"SMS delivery failed via {provider.ProviderName}: {sendResult.ErrorMessage}");
    }

    // TODO: Testing is pending, need deployment environment to get call back from Twilio.

    public async Task<FrameworkResult> UpdateSmsStatusAsync()
    {
        try
        {
            var request = httpContextAccessor.HttpContext.Request;
            if (request != null)
            {
                var smsSid = request.Form["MessageSid"];
                var messageStatus = request.Form["MessageStatus"];
                var notification = await smsRepository.GetAsync(x => x.MessageId == smsSid);
                if (notification != null && notification.Any())
                {
                    var message = notification[0];
                    // TODO - At testing need to find the message status format to update.
                    // message.Status = messageStatus;
                    await smsRepository.UpdateAsync(message);
                    return await smsRepository.SaveChangesAsync();
                }
            }

            return frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.InvalidOrNullObject);
        }
        catch (Exception ex)
        {
            loggerService.WriteTo(Log.Error, "SMS status update failed", ex.Message);
            throw;
        }
    }

    private async Task<(SMSMessage, string)> ResolveSmsTemplate(SMSMessage message)
    {
        var smsTemplate = templateSettings.SMSTemplateCollection.Find(x => x.Name == message.TemplateName);
        if (smsTemplate != null)
        {
            var templateFormat = smsTemplate.TemplateFormat.Clone().ToString();

            var users = await csUserManager.FindByIdAsync(message.UserId.ToString());
            if (users != null)
                templateFormat = !string.IsNullOrWhiteSpace(templateFormat)
                    ? templateFormat.UpdateNotificationTemplatePlaceholder(users, message.Parameters)
                    : null;
            else
                return (message, ApiErrorCodes.InvalidUserId);

            message.Content = templateFormat;
            return (message, string.Empty);
        }

        return (message, ApiErrorCodes.TemplateDoesNotExists);
    }

    private async Task SaveSmsNotification(SMSMessage message, MessageResource messageResource)
    {
        try
        {
            var notification = mapper.Map<SMSMessage, Notification>(message);
            notification.MessageId = messageResource.Sid;
            notification.Type = NotificationTypes.SMS;
            notification.Status = GetStatus(messageResource);
            notification.Sender = smsConfig.SMSAccountFrom;
            await smsRepository.InsertAsync(notification);
            await smsRepository.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            loggerService.WriteTo(Log.Error, "SMS delivery failed", ex.Message);
            throw;
        }
    }

    private FrameworkResult ValidateSmsMessage(SMSMessage message)
    {
        loggerService.WriteTo(Log.Debug, "Validating SMS message");
        if (string.IsNullOrWhiteSpace(message.To))
            return frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.ToNumberNotConfiguredForSMS);

        if (string.IsNullOrWhiteSpace(message.Content))
            return frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.NoContentSpecifiedForSMS);

        if (!string.IsNullOrWhiteSpace(message.Activity) && message.Activity.Length > Constants.ColumnLength255)
            return frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.ActivityTooLong);

        return frameworkResultService.Succeeded();
    }

    private FrameworkResult ValidateSmsConfiguration()
    {
        loggerService.WriteTo(Log.Debug, "Validating SMS configuration");
        if (string.IsNullOrWhiteSpace(smsConfig.SMSAccountIdentification))
            return frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.UserNameNotConfiguredForSMS);

        if (string.IsNullOrWhiteSpace(smsConfig.SMSAccountPassword))
            return frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.PasswordNotConfiguredForSMS);

        if (string.IsNullOrWhiteSpace(smsConfig.SMSAccountFrom))
            return frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.FromNumberNotConfiguredForSMS);

        if (string.IsNullOrWhiteSpace(smsConfig.SMSStatusCallbackURL))
            return frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.SMSCallbackURLMissing);

        return frameworkResultService.Succeeded();
    }

    private NotificationStatus GetStatus(MessageResource messageResource)
    {
        return (NotificationStatus)Enum.Parse(typeof(NotificationStatus), messageResource.Status.ToString());
    }
}
