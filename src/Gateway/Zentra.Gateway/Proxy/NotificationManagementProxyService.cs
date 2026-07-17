using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Zentra.Domain;
using Zentra.Domain.Constants;
using Zentra.Domain.Constants.Endpoint;
using Zentra.Domain.Entities.Api;
using Zentra.Domain.Models.Api;
using Zentra.DomainServices;
using Zentra.DomainServices.Infra;
using Zentra.Service.Implementation.Api.Services;
using Zentra.Service.Interfaces.Interfaces.Api;
using Zentra.Service.Interfaces.Interfaces.Api.Wrapper;

namespace Zentra.ProxyService.Proxy;

public sealed class NotificationManagementProxyService : NotificationManagementService, INotificationManagementService
{
    private readonly IApiValidator apiValidator;
    private readonly IFrameworkResultService frameworkResult;
    private readonly IHttpContextAccessor httpContextAccessor;

    public NotificationManagementProxyService(
        IRepository<Notification> notificationRepository,
        IRepository<NotificationProviderConfig> providerConfigRepository,
        ZentraConfig zentraConfig,
        IFrameworkResultService frameworkResult,
        ILoggerInstance loggerInstance,
        IEnumerable<IEmailProvider> emailProviders,
        IApiValidator apiValidator,
        IHttpContextAccessor httpContextAccessor)
        : base(
            notificationRepository,
            providerConfigRepository,
            zentraConfig,
            frameworkResult,
            loggerInstance,
            emailProviders)
    {
        this.apiValidator = apiValidator;
        this.frameworkResult = frameworkResult;
        this.httpContextAccessor = httpContextAccessor;
    }

    public override async Task<NotificationLogResponseModel> GetNotificationLogsAsync(NotificationSearchRequestModel request)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed)
            frameworkResult.ThrowCustomMessage(result.Errors.FirstOrDefault().Description);

        return await base.GetNotificationLogsAsync(request);
    }

    public override async Task<NotificationTemplateResponseModel> GetNotificationTemplatesAsync()
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed)
            frameworkResult.ThrowCustomMessage(result.Errors.FirstOrDefault().Description);

        return await base.GetNotificationTemplatesAsync();
    }

    public override async Task<ProviderConfigModel> GetProviderConfigAsync(Guid id)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed)
            frameworkResult.ThrowCustomMessage(result.Errors.FirstOrDefault().Description);

        return await base.GetProviderConfigAsync(id);
    }

    public override async Task<List<ProviderConfigModel>> GetAllProviderConfigsAsync()
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed)
            frameworkResult.ThrowCustomMessage(result.Errors.FirstOrDefault().Description);

        return await base.GetAllProviderConfigsAsync();
    }

    public override async Task<FrameworkResult> SaveProviderConfigAsync(SaveProviderConfigRequest request)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.SaveProviderConfigAsync(request);
    }

    public override async Task<FrameworkResult> SetActiveProviderAsync(SetActiveProviderRequest request)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.SetActiveProviderAsync(request);
    }

    public override async Task<FrameworkResult> DeleteProviderConfigAsync(DeleteProviderConfigRequest request)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.DeleteProviderConfigAsync(request);
    }

    public override async Task<ProviderFieldDefinitionsResponse> GetProviderFieldDefinitionsAsync()
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed)
            frameworkResult.ThrowCustomMessage(result.Errors.FirstOrDefault().Description);

        return await base.GetProviderFieldDefinitionsAsync();
    }

    public override async Task<FrameworkResult> SendTestNotificationAsync(SendTestNotificationRequest request)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        // Extract current user ID from JWT for notification logging (FK constraint)
        var authorization = httpContextAccessor.HttpContext?.Request?.Headers[HeaderNames.Authorization].ToString();
        if (!string.IsNullOrWhiteSpace(authorization) &&
            AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
        {
            var token = new JwtSecurityToken(headerValue.Parameter);
            var subClaim = token.Claims.FirstOrDefault(c => c.Type == OpenIdConstants.ClaimTypes.Sub);
            if (subClaim != null && Guid.TryParse(subClaim.Value, out var userId))
                request.UserId = userId;
        }

        return await base.SendTestNotificationAsync(request);
    }
}
