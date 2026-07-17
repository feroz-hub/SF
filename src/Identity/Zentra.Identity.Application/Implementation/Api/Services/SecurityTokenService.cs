using Zentra.Domain;
using Zentra.Domain.Constants;
using Zentra.Domain.Enums;
using Zentra.Domain.ErrorCodes;
using Zentra.Domain.Models.Api;
using Zentra.Domain.Models.Endpoint;
using Zentra.DomainServices.Infra;
using Zentra.DomainServices.Repository.Api;
using Zentra.Service.Implementation.Endpoint.Extensions;
using Zentra.Service.Interfaces.Interfaces.Api;

namespace Zentra.Service.Implementation.Api.Services;

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
