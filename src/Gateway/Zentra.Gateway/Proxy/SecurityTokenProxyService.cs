using Zentra.Domain;
using Zentra.Domain.Models.Api;
using Zentra.Domain.Models.Endpoint;
using Zentra.DomainServices.Infra;
using Zentra.DomainServices.Repository.Api;
using Zentra.Service.Implementation.Api.Services;
using Zentra.Service.Interfaces.Interfaces.Api;
using Zentra.Service.Interfaces.Interfaces.Api.Wrapper;

namespace Zentra.ProxyService.Proxy;

public sealed class SecurityTokenProxyService : SecurityTokenService, ISecurityTokenService
{
    private readonly IApiValidator apiValidator;
    private readonly IFrameworkResultService frameworkResult;

    public SecurityTokenProxyService(
        IApiValidator apiValidator,
        ILoggerInstance instance,
        IFrameworkResultService frameworkResult,
        ISecurityTokenRepository securityTokenRepository)
        : base(
            instance,
            frameworkResult,
            securityTokenRepository)
    {
        this.apiValidator = apiValidator;
        this.frameworkResult = frameworkResult;
    }

    public override async Task<IList<TokenModel>> GetClientsActiveSecurityTokensAsync(IList<string> clientIds,
        PagingModel page = null)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed)
            frameworkResult.ThrowCustomMessage(result.Errors.FirstOrDefault().Description);

        return await base.GetClientsActiveSecurityTokensAsync(clientIds, page);
    }

    public override async Task<IList<TokenModel>> GetUsersActiveSecurityTokensAsync(IList<string> userIds,
        PagingModel page = null)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed)
            frameworkResult.ThrowCustomMessage(result.Errors.FirstOrDefault().Description);

        return await base.GetUsersActiveSecurityTokensAsync(userIds, page);
    }

    public override async Task<IList<TokenModel>> GetActiveSecurityTokensAsync(DateTime fromdate, DateTime todate,
        PagingModel page = null)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed)
            frameworkResult.ThrowCustomMessage(result.Errors.FirstOrDefault().Description);

        return await base.GetActiveSecurityTokensAsync(fromdate, todate, page);
    }

    public override async Task<IList<TokenModel>> GetAllSecurityTokensAsync(DateTime fromdate, DateTime todate,
        PagingModel page = null)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed)
            frameworkResult.ThrowCustomMessage(result.Errors.FirstOrDefault().Description);

        return await base.GetAllSecurityTokensAsync(fromdate, todate, page);
    }
}
