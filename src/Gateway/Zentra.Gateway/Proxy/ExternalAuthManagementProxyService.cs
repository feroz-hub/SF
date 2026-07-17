using Zentra.Domain;
using Zentra.Domain.Entities.Api;
using Zentra.Domain.Models.Api;
using Zentra.DomainServices;
using Zentra.DomainServices.Infra;
using Zentra.Service.Implementation.Api.Services;
using Zentra.Service.Interfaces.Interfaces.Api;
using Zentra.Service.Interfaces.Interfaces.Api.Wrapper;

namespace Zentra.ProxyService.Proxy;

public sealed class ExternalAuthManagementProxyService : ExternalAuthManagementService, IExternalAuthManagementService
{
    private readonly IApiValidator apiValidator;
    private readonly IFrameworkResultService frameworkResult;

    public ExternalAuthManagementProxyService(
        IRepository<ExternalAuthProviderConfig> providerConfigRepository,
        IFrameworkResultService frameworkResult,
        ILoggerInstance loggerInstance,
        IApiValidator apiValidator)
        : base(
            providerConfigRepository,
            frameworkResult,
            loggerInstance)
    {
        this.apiValidator = apiValidator;
        this.frameworkResult = frameworkResult;
    }

    public override async Task<List<ExternalAuthProviderConfigModel>> GetAllProvidersAsync()
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed)
            frameworkResult.ThrowCustomMessage(result.Errors.FirstOrDefault().Description);

        return await base.GetAllProvidersAsync();
    }

    public override async Task<ExternalAuthProviderConfigModel> GetProviderAsync(Guid id)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed)
            frameworkResult.ThrowCustomMessage(result.Errors.FirstOrDefault().Description);

        return await base.GetProviderAsync(id);
    }

    public override async Task<FrameworkResult> SaveProviderAsync(SaveExternalAuthProviderRequest request)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.SaveProviderAsync(request);
    }

    public override async Task<FrameworkResult> DeleteProviderAsync(DeleteExternalAuthProviderRequest request)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.DeleteProviderAsync(request);
    }

    public override async Task<FrameworkResult> TestProviderAsync(TestExternalAuthProviderRequest request)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.TestProviderAsync(request);
    }

    public override async Task<ExternalAuthFieldDefinitionsResponse> GetFieldDefinitionsAsync()
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed)
            frameworkResult.ThrowCustomMessage(result.Errors.FirstOrDefault().Description);

        return await base.GetFieldDefinitionsAsync();
    }
}
