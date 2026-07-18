/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using HCL.CS.Domain;
using HCL.CS.Domain.Entities.Api;
using HCL.CS.Domain.Models.Api;
using HCL.CS.DomainServices;
using HCL.CS.DomainServices.Infra;
using HCL.CS.Service.Implementation.Api.Services;
using HCL.CS.Service.Interfaces.Interfaces.Api;
using HCL.CS.Service.Interfaces.Interfaces.Api.Wrapper;

namespace HCL.CS.ProxyService.Proxy;

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
