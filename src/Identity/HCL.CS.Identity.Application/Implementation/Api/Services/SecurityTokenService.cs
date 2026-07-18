/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using HCL.CS.Domain;
using HCL.CS.Domain.Constants;
using HCL.CS.Domain.Enums;
using HCL.CS.Domain.ErrorCodes;
using HCL.CS.Domain.Models.Api;
using HCL.CS.Domain.Models.Endpoint;
using HCL.CS.DomainServices.Infra;
using HCL.CS.DomainServices.Repository.Api;
using HCL.CS.Service.Implementation.Endpoint.Extensions;
using HCL.CS.Service.Interfaces.Interfaces.Api;

namespace HCL.CS.Service.Implementation.Api.Services;

public class SecurityTokenService(
    ILoggerInstance instance,
    IFrameworkResultService frameworkResult,
    ISecurityTokenRepository securityTokenRepository)
    : SecurityBase, ISecurityTokenService
{
    private readonly ILoggerService loggerService = instance.GetLoggerInstance(LoggerKeyConstants.DefaultLoggerKey);

    public virtual async Task<IList<TokenModel>> GetClientsActiveSecurityTokensAsync(IList<string> clientIds,
        PagingModel page = null)
    {
        if (!clientIds.ContainsAny()) frameworkResult.Throw(EndpointErrorCodes.ClientIdIsRequired);

        try
        {
            loggerService.WriteTo(Log.Debug, "Entered into get active security tokens for clients");
            return await securityTokenRepository.GetSecurityTokenAsync(page, SecurityTokenOption.Client,
                clientIds: clientIds);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, ex.Message);
            throw;
        }
    }

    public virtual async Task<IList<TokenModel>> GetUsersActiveSecurityTokensAsync(IList<string> userIds,
        PagingModel page = null)
    {
        if (!userIds.ContainsAny()) frameworkResult.Throw(EndpointErrorCodes.InvalidUser);

        try
        {
            loggerService.WriteTo(Log.Debug, "Entered into get active security tokens for users");
            return await securityTokenRepository.GetSecurityTokenAsync(page, SecurityTokenOption.User,
                userIds: userIds);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, ex.Message);
            throw;
        }
    }

    public virtual async Task<IList<TokenModel>> GetActiveSecurityTokensAsync(DateTime fromdate, DateTime todate,
        PagingModel page = null)
    {
        if (fromdate > todate) frameworkResult.Throw(ApiErrorCodes.FromDateGreaterThanToDate);

        try
        {
            loggerService.WriteTo(Log.Debug, "Entered into get active security tokens based on given dates");
            return await securityTokenRepository.GetSecurityTokenAsync(page, SecurityTokenOption.BetweenDates, fromdate,
                todate);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, ex.Message);
            throw;
        }
    }

    public virtual async Task<IList<TokenModel>> GetAllSecurityTokensAsync(DateTime fromdate, DateTime todate,
        PagingModel page = null)
    {
        if (fromdate > todate) frameworkResult.Throw(ApiErrorCodes.FromDateGreaterThanToDate);

        try
        {
            loggerService.WriteTo(Log.Debug, "Entered into get active security tokens based on given dates");
            return await securityTokenRepository.GetSecurityTokenAsync(page, SecurityTokenOption.All, fromdate, todate);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, ex.Message);
            throw;
        }
    }
}
