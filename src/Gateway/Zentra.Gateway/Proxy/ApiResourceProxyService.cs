using AutoMapper;
using Zentra.Domain;
using Zentra.Domain.Entities.Api;
using Zentra.Domain.Models.Api;
using Zentra.DomainServices;
using Zentra.DomainServices.Infra;
using Zentra.DomainServices.Repository.Api;
using Zentra.Service.Implementation.Api.Services;
using Zentra.Service.Interfaces.Interfaces.Api;
using Zentra.Service.Interfaces.Interfaces.Api.Wrapper;

namespace Zentra.ProxyService.Proxy;

public sealed class ApiResourceProxyService : ApiResourceService, IApiResourceService
{
    private readonly IApiValidator apiValidator;
    private readonly IFrameworkResultService frameworkResult;

    public ApiResourceProxyService(
        IApiValidator apiValidator,
        ILoggerInstance instance,
        IMapper mapper,
        IFrameworkResultService frameworkResult,
        IApiResourceRepository apiResourceRepository,
        IRepository<ApiResourceClaims> apiResourceClaimRepository,
        IRepository<ApiScopes> apiScopeRepository,
        IRepository<ApiScopeClaims> apiScopeClaimRepository)
        : base(
            instance,
            mapper,
            frameworkResult,
            apiResourceRepository,
            apiResourceClaimRepository,
            apiScopeRepository,
            apiScopeClaimRepository)
    {
        this.apiValidator = apiValidator;
        this.frameworkResult = frameworkResult;
    }

    public override async Task<FrameworkResult> AddApiResourceAsync(ApiResourcesModel apiResourceModel)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.AddApiResourceAsync(apiResourceModel);
    }

    public override async Task<FrameworkResult> UpdateApiResourceAsync(ApiResourcesModel apiResourceModel)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.UpdateApiResourceAsync(apiResourceModel);
    }

    public override async Task<FrameworkResult> DeleteApiResourceAsync(Guid apiResourceId)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.DeleteApiResourceAsync(apiResourceId);
    }

    public override async Task<FrameworkResult> DeleteApiResourceAsync(string apiResourceName)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.DeleteApiResourceAsync(apiResourceName);
    }

    public override async Task<ApiResourcesModel> GetApiResourceAsync(Guid apiResourceId)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed)
            frameworkResult.ThrowCustomMessage(result.Errors.FirstOrDefault().Description);

        return await base.GetApiResourceAsync(apiResourceId);
    }

    public override async Task<ApiResourcesModel> GetApiResourceAsync(string apiResourceName)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed)
            frameworkResult.ThrowCustomMessage(result.Errors.FirstOrDefault().Description);

        return await base.GetApiResourceAsync(apiResourceName);
    }

    public override async Task<IList<ApiResourcesModel>> GetAllApiResourcesAsync()
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed)
            frameworkResult.ThrowCustomMessage(result.Errors.FirstOrDefault().Description);

        return await base.GetAllApiResourcesAsync();
    }

    public override async Task<IList<ApiResourcesByScopesModel>> GetAllApiResourcesByScopesAsync(
        IList<string> requestedScopes)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed)
            frameworkResult.ThrowCustomMessage(result.Errors.FirstOrDefault().Description);

        return await base.GetAllApiResourcesByScopesAsync(requestedScopes);
    }

    public override async Task<FrameworkResult> AddApiResourceClaimAsync(ApiResourceClaimsModel apiResourceClaimModel)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.AddApiResourceClaimAsync(apiResourceClaimModel);
    }

    public override async Task<FrameworkResult> DeleteApiResourceClaimByResourceIdAsync(Guid apiResourceId)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.DeleteApiResourceClaimByResourceIdAsync(apiResourceId);
    }

    public override async Task<FrameworkResult> DeleteApiResourceClaimByIdAsync(Guid apiResourceClaimId)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.DeleteApiResourceClaimByIdAsync(apiResourceClaimId);
    }

    public override async Task<FrameworkResult> DeleteApiResourceClaimAsync(
        ApiResourceClaimsModel apiResourceClaimModel)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.DeleteApiResourceClaimAsync(apiResourceClaimModel);
    }

    public override async Task<IList<ApiResourceClaimsModel>> GetApiResourceClaimsAsync(Guid apiResourceId)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed)
            frameworkResult.ThrowCustomMessage(result.Errors.FirstOrDefault().Description);

        return await base.GetApiResourceClaimsAsync(apiResourceId);
    }

    public override async Task<FrameworkResult> AddApiScopeAsync(ApiScopesModel apiScopesModel)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.AddApiScopeAsync(apiScopesModel);
    }

    public override async Task<FrameworkResult> UpdateApiScopeAsync(ApiScopesModel apiScopesModel)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.UpdateApiScopeAsync(apiScopesModel);
    }

    public override async Task<FrameworkResult> DeleteApiScopeAsync(Guid apiScopeId)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.DeleteApiScopeAsync(apiScopeId);
    }

    public override async Task<FrameworkResult> DeleteApiScopeAsync(string apiScopeName)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.DeleteApiScopeAsync(apiScopeName);
    }

    public override async Task<ApiScopesModel> GetApiScopeAsync(Guid apiScopeId)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed)
            frameworkResult.ThrowCustomMessage(result.Errors.FirstOrDefault().Description);

        return await base.GetApiScopeAsync(apiScopeId);
    }

    public override async Task<ApiScopesModel> GetApiScopeAsync(string apiScopeName)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed)
            frameworkResult.ThrowCustomMessage(result.Errors.FirstOrDefault().Description);

        return await base.GetApiScopeAsync(apiScopeName);
    }

    public override async Task<IList<ApiScopesModel>> GetAllApiScopesAsync()
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed)
            frameworkResult.ThrowCustomMessage(result.Errors.FirstOrDefault().Description);

        return await base.GetAllApiScopesAsync();
    }

    public override async Task<FrameworkResult> AddApiScopeClaimAsync(ApiScopeClaimsModel apiScopeClaimModel)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.AddApiScopeClaimAsync(apiScopeClaimModel);
    }

    public override async Task<FrameworkResult> DeleteApiScopeClaimByScopeIdAsync(Guid apiScopeId)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.DeleteApiScopeClaimByScopeIdAsync(apiScopeId);
    }

    public override async Task<FrameworkResult> DeleteApiScopeClaimByIdAsync(Guid apiScopeClaimId)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.DeleteApiScopeClaimByIdAsync(apiScopeClaimId);
    }

    public override async Task<FrameworkResult> DeleteApiScopeClaimAsync(ApiScopeClaimsModel apiScopeClaimModel)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.DeleteApiScopeClaimAsync(apiScopeClaimModel);
    }

    public override async Task<IList<ApiScopeClaimsModel>> GetApiScopeClaimsAsync(Guid apiScopeId)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed)
            frameworkResult.ThrowCustomMessage(result.Errors.FirstOrDefault().Description);

        return await base.GetApiScopeClaimsAsync(apiScopeId);
    }
}
