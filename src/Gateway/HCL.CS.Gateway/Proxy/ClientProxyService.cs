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
using HCL.CS.Domain.Models.Endpoint;
using HCL.CS.DomainServices;
using HCL.CS.DomainServices.Infra;
using HCL.CS.DomainServices.Repository.Api;
using HCL.CS.DomainServices.UnitOfWork.Endpoint;
using HCL.CS.Service.Implementation.Api.Services;
using HCL.CS.Service.Interfaces.Interfaces.Api;
using HCL.CS.Service.Interfaces.Interfaces.Api.Wrapper;

namespace HCL.CS.ProxyService.Proxy;

public sealed class ClientProxyService : ClientService, IClientServices
{
    private readonly IApiValidator apiValidator;
    private readonly IFrameworkResultService frameworkResult;

    public ClientProxyService(
        ILoggerInstance instance,
        IMapper mapper,
        IFrameworkResultService frameworkResult,
        IClientsUnitOfWork unitOfWork,
        HclCsConfig securityConfig,
        IUserAccountService userAccountService,
        IApiResourceRepository apiResourceRepository,
        IRepository<ApiScopes> apiScopeRepository,
        IIdentityResourceRepository identityResourceRepository,
        IClientProvisioningTransactionHook provisioningTransactionHook,
        IApiValidator apiValidator)
        : base(
            instance,
            mapper,
            frameworkResult,
            unitOfWork,
            securityConfig,
            apiResourceRepository,
            apiScopeRepository,
            identityResourceRepository,
            provisioningTransactionHook)
    {
        this.apiValidator = apiValidator;
        this.frameworkResult = frameworkResult;
    }

    public override async Task<FrameworkResult> DeleteClientAsync(string clientId)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.DeleteClientAsync(clientId);
    }

    public override async Task<ClientsModel> GenerateClientSecret(string clientId)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed)
            frameworkResult.ThrowCustomMessage(result.Errors.FirstOrDefault().Description);

        return await base.GenerateClientSecret(clientId);
    }

    public override async Task<Dictionary<string, string>> GetAllClientAsync()
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed)
            frameworkResult.ThrowCustomMessage(result.Errors.FirstOrDefault().Description);

        return await base.GetAllClientAsync();
    }

    public override async Task<ClientsModel> GetClientAsync(string clientId)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed)
            frameworkResult.ThrowCustomMessage(result.Errors.FirstOrDefault().Description);

        return await base.GetClientAsync(clientId);
    }

    public override async Task<ClientsModel> RegisterClientAsync(ClientsModel clientsModel)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed)
            frameworkResult.ThrowCustomMessage(result.Errors.FirstOrDefault().Description);

        return await base.RegisterClientAsync(clientsModel);
    }

    public override async Task<ClientsModel> UpdateClientAsync(ClientsModel clientsModel)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed)
            frameworkResult.ThrowCustomMessage(result.Errors.FirstOrDefault().Description);

        return await base.UpdateClientAsync(clientsModel);
    }

    public override async Task<ClientsModel> ProvisionClientAsync(ClientsModel clientsModel)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed)
            frameworkResult.ThrowCustomMessage(result.Errors.FirstOrDefault().Description);

        return await base.ProvisionClientAsync(clientsModel);
    }
}
