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

public sealed class IdentityResourceProxyService : IdentityResourceService, IIdentityResourceService
{
    private readonly IApiValidator apiValidator;
    private readonly IFrameworkResultService frameworkResult;

    public IdentityResourceProxyService(
        ILoggerInstance instance,
        IMapper mapper,
        IFrameworkResultService frameworkResult,
        IIdentityResourceRepository identityResourceRepository,
        IRepository<IdentityClaims> identityClaimRepository,
        IApiValidator apiValidator)
        : base(
            instance,
            mapper,
            frameworkResult,
            identityResourceRepository,
            identityClaimRepository)
    {
        this.apiValidator = apiValidator;
        this.frameworkResult = frameworkResult;
    }

    public override async Task<FrameworkResult> AddIdentityResourceAsync(IdentityResourcesModel identityResourceModel)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.AddIdentityResourceAsync(identityResourceModel);
    }

    public override async Task<FrameworkResult> UpdateIdentityResourceAsync(
        IdentityResourcesModel identityResourceModel)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.UpdateIdentityResourceAsync(identityResourceModel);
    }

    public override async Task<FrameworkResult> DeleteIdentityResourceAsync(Guid identityResourceId)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.DeleteIdentityResourceAsync(identityResourceId);
    }

    public override async Task<FrameworkResult> DeleteIdentityResourceAsync(string identityResourceName)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.DeleteIdentityResourceAsync(identityResourceName);
    }

    public override async Task<IdentityResourcesModel> GetIdentityResourceAsync(Guid identityResourceId)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed)
            frameworkResult.ThrowCustomMessage(result.Errors.FirstOrDefault().Description);

        return await base.GetIdentityResourceAsync(identityResourceId);
    }

    public override async Task<IdentityResourcesModel> GetIdentityResourceAsync(string identityResourceName)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed)
            frameworkResult.ThrowCustomMessage(result.Errors.FirstOrDefault().Description);

        return await base.GetIdentityResourceAsync(identityResourceName);
    }

    public override async Task<IList<IdentityResourcesModel>> GetAllIdentityResourcesAsync()
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed)
            frameworkResult.ThrowCustomMessage(result.Errors.FirstOrDefault().Description);

        return await base.GetAllIdentityResourcesAsync();
    }

    public override async Task<FrameworkResult> AddIdentityResourceClaimAsync(IdentityClaimsModel identityClaimsModel)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.AddIdentityResourceClaimAsync(identityClaimsModel);
    }

    public override async Task<FrameworkResult> DeleteIdentityResourceClaimByResourceIdAsync(Guid identityResourceId)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.DeleteIdentityResourceClaimByResourceIdAsync(identityResourceId);
    }

    public override async Task<FrameworkResult> DeleteIdentityResourceClaimByIdAsync(Guid identityResourceClaimId)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.DeleteIdentityResourceClaimByIdAsync(identityResourceClaimId);
    }

    public override async Task<FrameworkResult> DeleteIdentityResourceClaimAsync(
        IdentityClaimsModel identityClaimsModel)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.DeleteIdentityResourceClaimAsync(identityClaimsModel);
    }

    public override async Task<IList<IdentityClaimsModel>> GetIdentityResourceClaimsAsync(Guid identityResourceId)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed)
            frameworkResult.ThrowCustomMessage(result.Errors.FirstOrDefault().Description);

        return await base.GetIdentityResourceClaimsAsync(identityResourceId);
    }
}
