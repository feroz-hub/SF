using System.Text.Json;
using Zentra.Domain;
using Zentra.Domain.Configurations.Api;
using Zentra.Domain.Constants;
using Zentra.Domain.Entities.Api;
using Zentra.Domain.Enums;
using Zentra.Domain.ErrorCodes;
using Zentra.Domain.Models.Api;
using Zentra.DomainServices;
using Zentra.DomainServices.Infra;
using Zentra.Service.Interfaces.Interfaces.Api;

namespace Zentra.Service.Implementation.Api.Services;

public class NotificationManagementService : SecurityBase, INotificationManagementService
{
    private readonly IRepository<Notification> _notificationRepository;
    private readonly IRepository<NotificationProviderConfig> _providerConfigRepository;
    private readonly ZentraConfig _zentraConfig;
    private readonly IFrameworkResultService _frameworkResult;
    private readonly ILoggerService _loggerService;
    private readonly Dictionary<string, IEmailProvider> _emailProviders;

    public NotificationManagementService(
        IRepository<Notification> notificationRepository,
        IRepository<NotificationProviderConfig> providerConfigRepository,
        ZentraConfig zentraConfig,
        IFrameworkResultService frameworkResult,
        ILoggerInstance loggerInstance,
        IEnumerable<IEmailProvider> emailProviders)
    {
        _notificationRepository = notificationRepository;
        _providerConfigRepository = providerConfigRepository;
        _zentraConfig = zentraConfig;
        _frameworkResult = frameworkResult;
        _loggerService = loggerInstance.GetLoggerInstance(LoggerKeyConstants.DefaultLoggerKey);
        _emailProviders = emailProviders.ToDictionary(p => p.ProviderName, StringComparer.OrdinalIgnoreCase);
    }

    public virtual async Task<NotificationLogResponseModel> GetNotificationLogsAsync(NotificationSearchRequestModel request)
    {
        try
        {
            if (request.Page == null)
                _frameworkResult.Throw(ApiErrorCodes.InvalidOrNullObject);

            var response = new NotificationLogResponseModel();

            var query = await _notificationRepository.GetAllForQueryAsync();

            if (request.Type.HasValue)
                query = query.Where(n => (int)n.Type == request.Type.Value);

            if (request.Status.HasValue)
                query = query.Where(n => (int)n.Status == request.Status.Value);

            if (!string.IsNullOrWhiteSpace(request.FromDate) && DateTime.TryParse(request.FromDate, out var fromDate))
                query = query.Where(n => n.CreatedOn >= fromDate);

            if (!string.IsNullOrWhiteSpace(request.ToDate) && DateTime.TryParse(request.ToDate, out var toDate))
                query = query.Where(n => n.CreatedOn <= toDate.AddDays(1));

            if (!string.IsNullOrWhiteSpace(request.SearchValue))
            {
                var search = request.SearchValue.ToLower();
                query = query.Where(n =>
                    (n.Activity != null && n.Activity.ToLower().Contains(search)) ||
                    (n.Recipient != null && n.Recipient.ToLower().Contains(search)) ||
                    (n.Sender != null && n.Sender.ToLower().Contains(search)) ||
                    (n.MessageId != null && n.MessageId.ToLower().Contains(search)));
            }

            request.Page.TotalItems = query.Count();
            response.PageInfo = request.Page;

            var skip = (request.Page.CurrentPage - 1) * request.Page.ItemsPerPage;
            var notifications = query
                .OrderByDescending(n => n.CreatedOn)
                .Skip(skip)
                .Take(request.Page.ItemsPerPage)
                .Select(n => new NotificationLogModel
                {
                    Id = n.Id,
                    UserId = n.UserId,
                    MessageId = n.MessageId,
                    Type = (int)n.Type,
                    Activity = n.Activity,
                    Status = (int)n.Status,
                    Sender = n.Sender,
                    Recipient = n.Recipient,
                    CreatedOn = n.CreatedOn
                })
                .ToList();

            response.Notifications = notifications;
            return response;
        }
        catch (Exception ex)
        {
            _loggerService.WriteToWithCaller(Log.Error, ex, "Failed to retrieve notification logs.");
            throw;
        }
    }

    public virtual Task<NotificationTemplateResponseModel> GetNotificationTemplatesAsync()
    {
        var response = new NotificationTemplateResponseModel
        {
            EmailTemplates = _zentraConfig.NotificationTemplateSettings?.EmailTemplateCollection ?? new List<EmailTemplate>(),
            SmsTemplates = _zentraConfig.NotificationTemplateSettings?.SMSTemplateCollection ?? new List<SMSTemplate>()
        };

        return Task.FromResult(response);
    }

    public virtual async Task<ProviderConfigModel> GetProviderConfigAsync(Guid id)
    {
        try
        {
            var config = await _providerConfigRepository.GetAsync(id);
            if (config == null)
                _frameworkResult.Throw(ApiErrorCodes.InvalidOrNullObject);

            return MapToProviderConfigModel(config, maskSecrets: true);
        }
        catch (Exception ex)
        {
            _loggerService.WriteToWithCaller(Log.Error, ex, "Failed to retrieve provider config.");
            throw;
        }
    }

    public virtual async Task<List<ProviderConfigModel>> GetAllProviderConfigsAsync()
    {
        try
        {
            var configs = await _providerConfigRepository.GetAllAsync();
            return configs.Select(c => MapToProviderConfigModel(c, maskSecrets: true)).ToList();
        }
        catch (Exception ex)
        {
            _loggerService.WriteToWithCaller(Log.Error, ex, "Failed to retrieve provider configs.");
            throw;
        }
    }

    public virtual async Task<FrameworkResult> SaveProviderConfigAsync(SaveProviderConfigRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.ProviderName))
                return _frameworkResult.Failed<FrameworkResult>(ApiErrorCodes.InvalidOrNullObject);

            if (request.Settings == null || request.Settings.Count == 0)
                return _frameworkResult.Failed<FrameworkResult>(ApiErrorCodes.InvalidOrNullObject);

            var configJson = JsonSerializer.Serialize(request.Settings);

            if (request.Id.HasValue && request.Id.Value != Guid.Empty)
            {
                var existing = await _providerConfigRepository.GetAsync(request.Id.Value);
                if (existing == null)
                    return _frameworkResult.Failed<FrameworkResult>(ApiErrorCodes.InvalidOrNullObject);

                existing.ProviderName = request.ProviderName;
                existing.ChannelType = request.ChannelType;
                existing.ConfigJson = configJson;

                if (request.IsActive && !existing.IsActive)
                    await DeactivateOtherProviders(request.ChannelType, existing.Id);

                existing.IsActive = request.IsActive;
                await _providerConfigRepository.UpdateAsync(existing);
            }
            else
            {
                var newConfig = new NotificationProviderConfig
                {
                    Id = Guid.NewGuid(),
                    ProviderName = request.ProviderName,
                    ChannelType = request.ChannelType,
                    IsActive = request.IsActive,
                    ConfigJson = configJson,
                    CreatedBy = "Admin"
                };

                if (request.IsActive)
                    await DeactivateOtherProviders(request.ChannelType, newConfig.Id);

                await _providerConfigRepository.InsertAsync(newConfig);
            }

            return await _providerConfigRepository.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _loggerService.WriteToWithCaller(Log.Error, ex, "Failed to save provider config.");
            throw;
        }
    }

    public virtual async Task<FrameworkResult> SetActiveProviderAsync(SetActiveProviderRequest request)
    {
        try
        {
            var config = await _providerConfigRepository.GetAsync(request.Id);
            if (config == null)
                return _frameworkResult.Failed<FrameworkResult>(ApiErrorCodes.InvalidOrNullObject);

            await DeactivateOtherProviders(config.ChannelType, config.Id);
            config.IsActive = true;
            await _providerConfigRepository.UpdateAsync(config);
            return await _providerConfigRepository.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _loggerService.WriteToWithCaller(Log.Error, ex, "Failed to set active provider.");
            throw;
        }
    }

    public virtual async Task<FrameworkResult> DeleteProviderConfigAsync(DeleteProviderConfigRequest request)
    {
        try
        {
            var config = await _providerConfigRepository.GetAsync(request.Id);
            if (config == null)
                return _frameworkResult.Failed<FrameworkResult>(ApiErrorCodes.InvalidOrNullObject);

            if (config.IsActive)
                return _frameworkResult.ConstructFailed(ApiErrorCodes.InvalidOrNullObject, "Cannot delete the active provider. Set another provider as active first.");

            await _providerConfigRepository.DeleteAsync(config);
            return await _providerConfigRepository.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _loggerService.WriteToWithCaller(Log.Error, ex, "Failed to delete provider config.");
            throw;
        }
    }

    public virtual Task<ProviderFieldDefinitionsResponse> GetProviderFieldDefinitionsAsync()
    {
        var response = new ProviderFieldDefinitionsResponse
        {
            EmailProviders = NotificationProviderConstants.EmailProviderFields,
            SmsProviders = NotificationProviderConstants.SmsProviderFields
        };

        return Task.FromResult(response);
    }

    public virtual async Task<FrameworkResult> SendTestNotificationAsync(SendTestNotificationRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Recipient))
                return _frameworkResult.Failed<FrameworkResult>(ApiErrorCodes.InvalidOrNullObject);

            // Resolve the provider config: use specific provider if given, otherwise active
            NotificationProviderConfig providerConfig = null;

            if (request.ProviderConfigId.HasValue && request.ProviderConfigId.Value != Guid.Empty)
            {
                providerConfig = await _providerConfigRepository.GetAsync(request.ProviderConfigId.Value);
            }
            else
            {
                var activeConfigs = await _providerConfigRepository.GetAsync(
                    x => x.ChannelType == request.Type && x.IsActive);
                providerConfig = activeConfigs?.FirstOrDefault();
            }

            if (providerConfig == null)
                return _frameworkResult.ConstructFailed(ApiErrorCodes.InvalidOrNullObject, "No provider configured. Please add and activate a provider first.");

            // Deserialize provider settings
            var config = JsonSerializer.Deserialize<Dictionary<string, string>>(providerConfig.ConfigJson)
                         ?? new Dictionary<string, string>();

            // Resolve the email provider by name
            if (!_emailProviders.TryGetValue(providerConfig.ProviderName, out var provider))
                return _frameworkResult.ConstructFailed(ApiErrorCodes.InvalidOrNullObject, $"Email provider '{providerConfig.ProviderName}' is not registered.");

            // Build and send a test email directly through the provider
            var fromAddress = config.GetValueOrDefault("FromAddress", "noreply@example.com");
            var fromName = config.GetValueOrDefault("FromName", "Zentra");

            var sentAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
            var testMessage = new EmailMessage
            {
                From = fromAddress,
                FromName = fromName,
                To = request.Recipient,
                Subject = "Test Email \u2014 Zentra",
                HtmlBody = "<!DOCTYPE html><html><head><meta charset='utf-8'><meta name='viewport' content='width=device-width,initial-scale=1'></head><body style='margin:0;padding:0;background:#020617;font-family:-apple-system,BlinkMacSystemFont,\"Segoe UI\",Roboto,\"Helvetica Neue\",Arial,sans-serif;-webkit-font-smoothing:antialiased'><div style='height:4px;background:#2563eb'></div><div style='max-width:520px;margin:0 auto;padding:40px 20px'><table role='presentation' style='width:100%;border-collapse:collapse'><tr><td style='text-align:center;padding:28px 0 32px'><div style='display:inline-block;background:#2563eb;width:44px;height:44px;border-radius:10px;text-align:center;line-height:44px;font-size:20px;font-weight:800;color:#fff'>Z</div><div style='margin-top:10px;font-size:14px;font-weight:700;letter-spacing:4px;color:#94a3b8'>ZENTRA</div></td></tr></table><div style='background:#ffffff;border-radius:16px;box-shadow:0 8px 32px rgba(0,0,0,0.4);overflow:hidden'><div style='padding:40px 36px'><h1 style='margin:0 0 8px;font-size:22px;font-weight:700;color:#0f172a'>Email configuration test</h1><div style='width:40px;height:3px;background:#2563eb;border-radius:2px;margin:0 0 20px'></div><p style='margin:0 0 20px;font-size:15px;line-height:1.7;color:#334155'>This is a test email sent from the <strong>Zentra Admin</strong> panel. If you are reading this, your email provider is configured correctly.</p><div style='background:#f0fdf4;border:1px solid #bbf7d0;border-radius:8px;padding:16px 20px;margin:0 0 16px'><p style='margin:0;font-size:14px;line-height:1.6;color:#166534'><strong>Provider:</strong> " + providerConfig.ProviderName + "<br/><strong>Status:</strong> Delivered successfully</p></div><p style='margin:0;font-size:13px;color:#64748b;text-align:center'>Sent at " + sentAt + " UTC</p></div><div style='background:#f8fafc;border-top:1px solid #e2e8f0;padding:14px 36px'><p style='margin:0;font-size:13px;color:#64748b;line-height:1.5'>This is an automated test from Zentra Admin. No action is required.</p></div></div><table role='presentation' style='width:100%;border-collapse:collapse'><tr><td style='text-align:center;padding:28px 0 0'><p style='margin:0 0 6px;font-size:11px;color:#475569'>Secured by Zentra Identity Platform</p><p style='margin:0 0 6px;font-size:11px;color:#475569'>A product of <a href='https://futurebeyondtech.com' style='color:#3b82f6;text-decoration:none;font-weight:600'>FBT &mdash; Future Beyond Tech</a></p><p style='margin:0;font-size:11px;color:#334155'>&copy; 2025 FBT Future Beyond Tech. All rights reserved.</p></td></tr></table></div></body></html>"
            };

            _loggerService.WriteTo(Log.Debug, $"Sending test email via {provider.ProviderName} to {request.Recipient}");

            var sendResult = await provider.SendAsync(testMessage, config);

            // Update provider test status
            providerConfig.LastTestedOn = DateTime.UtcNow;
            providerConfig.LastTestSuccess = sendResult.Success;
            await _providerConfigRepository.UpdateAsync(providerConfig);
            await _providerConfigRepository.SaveChangesAsync();

            // Save notification log so the test email appears in Delivery Logs
            // Only save if we have a valid user ID (required by FK constraint on Users table)
            if (request.UserId.HasValue && request.UserId.Value != Guid.Empty)
            {
                var notification = new Notification
                {
                    Id = Guid.NewGuid(),
                    UserId = request.UserId.Value,
                    MessageId = sendResult.MessageId ?? $"test-{Guid.NewGuid():N}",
                    Type = NotificationTypes.Email,
                    Activity = "Test Notification",
                    Status = sendResult.Success ? NotificationStatus.Delivered : NotificationStatus.Failed,
                    Sender = fromAddress,
                    Recipient = request.Recipient,
                    CreatedOn = DateTime.UtcNow
                };
                await _notificationRepository.InsertAsync(notification);
                await _notificationRepository.SaveChangesAsync();
            }

            if (sendResult.Success)
            {
                _loggerService.WriteTo(Log.Debug, $"Test email sent successfully. MessageId: {sendResult.MessageId}");
                return _frameworkResult.Succeeded();
            }

            _loggerService.WriteTo(Log.Error, $"Test email failed via {provider.ProviderName}: {sendResult.ErrorMessage}");
            return _frameworkResult.ConstructFailed(ApiErrorCodes.InvalidOrNullObject, $"Email delivery failed: {sendResult.ErrorMessage}");
        }
        catch (Exception ex)
        {
            _loggerService.WriteToWithCaller(Log.Error, ex, "Failed to send test notification.");
            throw;
        }
    }

    private async Task DeactivateOtherProviders(int channelType, Guid excludeId)
    {
        var activeConfigs = await _providerConfigRepository.GetAsync(
            c => c.ChannelType == channelType && c.IsActive && c.Id != excludeId);

        if (activeConfigs != null)
        {
            foreach (var config in activeConfigs)
            {
                config.IsActive = false;
                await _providerConfigRepository.UpdateAsync(config);
            }
        }
    }

    private static ProviderConfigModel MapToProviderConfigModel(NotificationProviderConfig config, bool maskSecrets)
    {
        var settings = new Dictionary<string, string>();

        if (!string.IsNullOrWhiteSpace(config.ConfigJson))
        {
            try
            {
                settings = JsonSerializer.Deserialize<Dictionary<string, string>>(config.ConfigJson)
                    ?? new Dictionary<string, string>();
            }
            catch
            {
                settings = new Dictionary<string, string>();
            }
        }

        if (maskSecrets)
        {
            // Get field definitions to know which fields are passwords
            Dictionary<string, ProviderFieldDefinition[]> fieldDefs;
            if (config.ChannelType == (int)NotificationTypes.Email)
                fieldDefs = NotificationProviderConstants.EmailProviderFields;
            else
                fieldDefs = NotificationProviderConstants.SmsProviderFields;

            if (fieldDefs.TryGetValue(config.ProviderName, out var fields))
            {
                foreach (var field in fields)
                {
                    if (field.InputType == "password" && settings.ContainsKey(field.Key))
                    {
                        var value = settings[field.Key];
                        if (!string.IsNullOrEmpty(value) && value.Length > 4)
                            settings[field.Key] = value[..4] + "****";
                        else if (!string.IsNullOrEmpty(value))
                            settings[field.Key] = "****";
                    }
                }
            }
        }

        return new ProviderConfigModel
        {
            Id = config.Id,
            ProviderName = config.ProviderName,
            ChannelType = config.ChannelType,
            IsActive = config.IsActive,
            Settings = settings,
            LastTestedOn = config.LastTestedOn,
            LastTestSuccess = config.LastTestSuccess
        };
    }
}
