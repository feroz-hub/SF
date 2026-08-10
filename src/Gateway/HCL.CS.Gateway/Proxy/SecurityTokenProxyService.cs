/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using HCL.CS.Domain;
using HCL.CS.Domain.Models.Api;
using HCL.CS.Domain.Models.Endpoint;
using HCL.CS.DomainServices.Infra;
using HCL.CS.DomainServices.Repository.Api;
using HCL.CS.Service.Implementation.Api.Services;
using HCL.CS.Service.Interfaces.Interfaces.Api;
using HCL.CS.Service.Interfaces.Interfaces.Api.Wrapper;

namespace HCL.CS.ProxyService.Proxy;

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
