using System.Text.Json;
using AutoMapper;
using MailKit;
using MailKit.Security;
using MimeKit;
using Zentra.Domain;
using Zentra.Domain.Configurations.Api;
using Zentra.Domain.Constants;
using Zentra.Domain.Entities.Api;
using Zentra.Domain.Enums;
using Zentra.Domain.ErrorCodes;
using Zentra.Domain.Models.Api;
using Zentra.DomainServices;
using Zentra.DomainServices.Infra;
using Zentra.DomainServices.Wrappers;
using Zentra.Infrastructure.Services.Extension;
using Zentra.Infrastructure.Services.Implementation.Providers;
using Zentra.Infrastructure.Services.Wrapper;

namespace Zentra.Infrastructure.Services.Implementation;

internal class EmailService : IEmailService
{
    private readonly UserManagerWrapper<Users> csUserManager;
    private readonly EmailConfig emailConfig;
    private readonly IRepository<Notification> emailRepository;
    private readonly IRepository<NotificationProviderConfig> providerConfigRepository;
    private readonly NotificationProviderFactory providerFactory;
    private readonly IFrameworkResultService frameworkResultService;
    private readonly ILoggerService loggerService;
    private readonly IMapper mapper;
    private readonly NotificationTemplateSettings templateSettings;
    private Notification notification;

    public EmailService(
        UserManagerWrapper<Users> userManager,
        ZentraConfig configSettings,
        ILoggerInstance loggerInstance,
        IRepository<Notification> emailRepository,
        IRepository<NotificationProviderConfig> providerConfigRepository,
        NotificationProviderFactory providerFactory,
        IMapper mapper,
        IFrameworkResultService frameworkResultService)
    {
        this.emailRepository = emailRepository;
        this.providerConfigRepository = providerConfigRepository;
        this.providerFactory = providerFactory;
        this.mapper = mapper;

        loggerService = loggerInstance.GetLoggerInstance(LoggerKeyConstants.DefaultLoggerKey);
        templateSettings = configSettings.NotificationTemplateSettings;
        emailConfig = configSettings.SystemSettings.EmailConfig;
        this.frameworkResultService = frameworkResultService;
        csUserManager = userManager;
    }

    public async Task<FrameworkResult> SendEmailAsync(NotificationInfoModel notificationModel)
    {
        if (notificationModel == null)
            return frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.InvalidOrNullObject);

        if (string.IsNullOrWhiteSpace(notificationModel.TemplateName))
            return frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.InvalidTemplateName);

        try
        {
            var message = mapper.Map<NotificationInfoModel, EmailMessageModel>(notificationModel);
            string errorCode;
            (message, errorCode) = await ResolveEmailTemplate(message);
            if (!string.IsNullOrWhiteSpace(errorCode)) return frameworkResultService.Failed<FrameworkResult>(errorCode);

            var result = ValidateIncomingMessage(message);
            if (result.Status == ResultStatus.Failed) return result;

            // Check for active DB-configured provider
            var activeProviderConfigs = await providerConfigRepository.GetAsync(
                x => x.ChannelType == 1 && x.IsActive);

            if (activeProviderConfigs != null && activeProviderConfigs.Any())
            {
                var providerConfig = activeProviderConfigs[0];
                await SendViaProvider(message, providerConfig);
            }
            else
            {
                // Fallback to SMTP config from appsettings
                result = ValidateEmailConfiguration();
                if (result.Status == ResultStatus.Failed) return result;

                var mailMessage = CreateEmailMessage(message);
                await SendEmailAsync(mailMessage, message);
            }

            return frameworkResultService.Succeeded();
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Email delivery failed");
            throw;
        }
    }

    private async Task SendViaProvider(EmailMessageModel message, NotificationProviderConfig providerConfig)
    {
        var config = JsonSerializer.Deserialize<Dictionary<string, string>>(providerConfig.ConfigJson)
                     ?? new Dictionary<string, string>();

        var emailMessage = new EmailMessage
        {
            From = message.FromAddress,
            FromName = message.FromName,
            To = message.ToAddress,
            Subject = message.Subject,
            HtmlBody = message.Content,
            CC = message.CC
        };

        await SaveEmailNotification(message, Guid.NewGuid().ToString());

        var provider = providerFactory.GetEmailProvider(providerConfig.ProviderName);
        loggerService.WriteTo(Log.Debug, $"Sending email via {provider.ProviderName}");

        var sendResult = await provider.SendAsync(emailMessage, config);

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

        await emailRepository.UpdateAsync(notification);
        await emailRepository.SaveChangesAsync();

        if (!sendResult.Success)
            throw new InvalidOperationException($"Email delivery failed via {provider.ProviderName}: {sendResult.ErrorMessage}");
    }

    private async Task<(EmailMessageModel, string)> ResolveEmailTemplate(EmailMessageModel message)
    {
        var emailTemplate = templateSettings.EmailTemplateCollection.Find(x => x.Name == message.TemplateName);
        var temlateFormat = emailTemplate.TemplateFormat;
        if (emailTemplate != null)
        {
            var users = await csUserManager.FindByIdAsync(message.UserId.ToString());
            if (users != null)
                temlateFormat = !string.IsNullOrWhiteSpace(temlateFormat)
                    ? temlateFormat.UpdateNotificationTemplatePlaceholder(users, message.Parameters)
                    : null;
            else
                return (message, ApiErrorCodes.InvalidUserId);

            message.Content = temlateFormat;
            message.Subject = emailTemplate.Subject;
            message.FromAddress = emailTemplate.FromAddress;
            message.FromName = emailTemplate.FromName;
            message.CC = emailTemplate.CC;
            return (message, string.Empty);
        }

        return (message, ApiErrorCodes.TemplateDoesNotExists);
    }

    private MimeMessage CreateEmailMessage(EmailMessageModel message)
    {
        loggerService.WriteTo(Log.Debug, "Constructing Email message");

        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress(message.FromName, message.FromAddress));
        emailMessage.To.Add(new MailboxAddress(message.ToName, message.ToAddress));
        emailMessage.Subject = message.Subject;

        var bodyBuilder = new BodyBuilder { HtmlBody = message.Content };
        emailMessage.Body = bodyBuilder.ToMessageBody();
        return emailMessage;
    }

    private async Task SendEmailAsync(MimeMessage mailMessage, EmailMessageModel message)
    {
        using var smtpClient = new SmtpClientWrapper();
        try
        {
            await SaveEmailNotification(message, mailMessage.MessageId);

            smtpClient.MessageSent += OnMessageSent;
            loggerService.WriteTo(Log.Debug, "Sending email");

            if (emailConfig.SecureSocketOptions)
                await smtpClient.ConnectAsync(emailConfig.SmtpServer, emailConfig.Port, true);
            else
                await smtpClient.ConnectAsync(emailConfig.SmtpServer, emailConfig.Port, SecureSocketOptions.StartTls);

            smtpClient.AuthenticationMechanisms.Remove("XOAUTH2");
            if (emailConfig.UserName != null && emailConfig.Password != null)
                try
                {
                    await smtpClient.AuthenticateAsync(emailConfig.UserName, emailConfig.Password);
                }
                catch (Exception ex)
                {
                    loggerService.WriteTo(Log.Error, "Email Authentication Failure:" + ex.Message);
                }

            await smtpClient.SendAsync(mailMessage);
        }
        catch (Exception ex)
        {
            notification.Status = NotificationStatus.Failed;
            await emailRepository.UpdateAsync(notification);
            await emailRepository.SaveChangesAsync();
            loggerService.WriteToWithCaller(Log.Error, ex, "Email delivery failed");
            throw;
        }
        finally
        {
            await smtpClient.DisconnectAsync(true);
            smtpClient.Dispose();
            loggerService.WriteTo(Log.Debug, "Email client disconnected");
        }
    }

    private void OnMessageSent(object sender, MessageSentEventArgs e)
    {
        notification.Status = NotificationStatus.Delivered;
        emailRepository.UpdateAsync(notification);
        emailRepository.SaveChangesAsync();
    }

    private async Task SaveEmailNotification(EmailMessageModel message, string messageId)
    {
        try
        {
            notification = mapper.Map<EmailMessageModel, Notification>(message);
            notification.MessageId = messageId;
            notification.Type = NotificationTypes.Email;
            notification.Status = NotificationStatus.Initiated;
            await emailRepository.InsertAsync(notification);
            await emailRepository.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Error while saving email notification");
            throw;
        }
    }

    private FrameworkResult ValidateIncomingMessage(EmailMessageModel message)
    {
        loggerService.WriteTo(Log.Debug, "Validating Email message");
        if (string.IsNullOrWhiteSpace(message.ToAddress))
            return frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.InvalidToAddress);

        if (string.IsNullOrWhiteSpace(message.Content))
            return frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.NoContentSpecifiedForEmail);

        if (string.IsNullOrWhiteSpace(message.FromAddress))
            return frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.InvalidFromAddress);

        if (string.IsNullOrWhiteSpace(message.Subject))
            return frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.InvalidSubject);

        if (!string.IsNullOrWhiteSpace(message.Activity) && message.Activity.Length > Constants.ColumnLength255)
            return frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.ActivityTooLong);

        return frameworkResultService.Succeeded();
    }

    private FrameworkResult ValidateEmailConfiguration()
    {
        loggerService.WriteTo(Log.Debug, "Validating Email configuration");
        if (string.IsNullOrWhiteSpace(emailConfig.SmtpServer))
            return frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.SmtpServerNotConfiguredForEmail);

        if (emailConfig.Port <= 0)
            return frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.PortNotConfiguredForEmail);

        if (string.IsNullOrWhiteSpace(emailConfig.UserName))
            return frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.UserNameNotConfiguredForEmail);

        if (string.IsNullOrWhiteSpace(emailConfig.Password))
            return frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.PasswordNotConfiguredForEmail);

        return frameworkResultService.Succeeded();
    }
}
