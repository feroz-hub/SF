/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using AutoMapper;
using HCL.CS.Domain;
using HCL.CS.Domain.Entities.Api;
using HCL.CS.Domain.Models.Api;
using HCL.CS.DomainServices;
using HCL.CS.DomainServices.Infra;
using HCL.CS.DomainServices.Repository.Api;
using HCL.CS.Service.Implementation.Api.Services;
using HCL.CS.Service.Interfaces.Interfaces.Api;
using HCL.CS.Service.Interfaces.Interfaces.Api.Wrapper;

namespace HCL.CS.ProxyService.Proxy;

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
