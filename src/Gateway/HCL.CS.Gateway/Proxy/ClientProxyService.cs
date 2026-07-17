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
        IApiValidator apiValidator)
        : base(
            instance,
            mapper,
            frameworkResult,
            unitOfWork,
            securityConfig,
            apiResourceRepository,
            apiScopeRepository,
            identityResourceRepository)
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
}
