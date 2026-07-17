using Microsoft.AspNetCore.Http;
using Zentra.Domain;
using Zentra.Domain.Constants;
using Zentra.Domain.Constants.Endpoint;
using Zentra.Domain.Models.Endpoint;
using Zentra.Domain.Models.Endpoint.Request;
using Zentra.Domain.Models.Endpoint.Response;
using Zentra.DomainServices.Infra;
using Zentra.Service.Implementation.Endpoint.Extensions;
using Zentra.Service.Interfaces.Interfaces.Endpoint;
using static Zentra.Domain.Constants.Endpoint.AuthenticationConstants;

namespace Zentra.Service.Implementation.Endpoint.Services;

internal class InteractionService : SecurityBase, IInteractionService
{
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly ILoggerService loggerService;
    private readonly ISessionManagementService session;

    public InteractionService(
        ILoggerInstance instance,
        IHttpContextAccessor httpContextAccessor,
        ISessionManagementService session)
    {
        this.httpContextAccessor = httpContextAccessor;
        this.session = session;
        loggerService = instance.GetLoggerInstance(LoggerKeyConstants.DefaultLoggerKey);
    }

    public async Task<ErrorResponseModel> GetErrorContextAsync(string errorId)
    {
        if (!string.IsNullOrWhiteSpace(errorId))
        {
            var result = await errorId.UnProtectDataAsync<ErrorResponseModel>();
            return result;
        }

        return null;
    }

    public async Task<LogoutRequestModel> GetLogoutContextAsync(string logoutId)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(logoutId))
            {
                loggerService.WriteTo(Log.Debug, "Entered into get logout context.");
                var logoutMessageModel = await logoutId.UnProtectDataAsync<LogoutMessageModel>();
                var endSessionCallBackUrl =
                    await GetEndSessionCallbackUrlAsync(httpContextAccessor.HttpContext, logoutMessageModel);
                return new LogoutRequestModel(endSessionCallBackUrl, logoutMessageModel);
            }
            return null;
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, ex.Message);
            throw;
        }
    }

    public Task<string> ConstructUserVerificationCode(string returnUrl, string userVerficationCode)
    {
        if (!string.IsNullOrWhiteSpace(returnUrl))
        {
            if (!string.IsNullOrWhiteSpace(userVerficationCode))
            {
                loggerService.WriteTo(Log.Debug, "Entered into construct user verification code");
                return Task.FromResult(returnUrl.AddQueryString(AuthCodeStore.UserVerificationCode, userVerficationCode));
            }

            return Task.FromResult(returnUrl);
        }

        return Task.FromResult("/");
    }

    private async Task<string> GetEndSessionCallbackUrlAsync(HttpContext context,
        LogoutMessageModel logoutMessage = null)
    {
        var user = await session.GetUserPrincipalFromContextAsync();
        var contextSubjectId = user?.GetSubjectId();
        LogoutMessageModel messageModel = null;

        if (logoutMessage?.ClientIdCollection?.ContainsAny() == true)
        {
            var clientIdCollection = logoutMessage?.ClientIdCollection;
            if (contextSubjectId == logoutMessage?.SubjectId)
            {
                var clientList = await session.GetClientListAsync();
                clientIdCollection = clientIdCollection.Union(clientList);
                clientIdCollection = clientIdCollection.Distinct();
            }

            messageModel = new LogoutMessageModel
            {
                SubjectId = logoutMessage.SubjectId,
                SessionId = logoutMessage.SessionId,
                ClientIdCollection = clientIdCollection
            };
        }
        else if (contextSubjectId != null)
        {
            var clientIdCollection = await session.GetClientListAsync();
            if (clientIdCollection.ContainsAny())
                messageModel = new LogoutMessageModel
                {
                    SubjectId = logoutMessage?.SubjectId,
                    SessionId = await session.GetSessionId(),
                    ClientIdCollection = clientIdCollection
                };
        }

        if (messageModel != null)
        {
            var logoutId = await messageModel.ProtectDataAsync();
            var endSessionCallbackUrl = context.GetZentraBaseUrl().IncludeEndSlash() +
                                        OpenIdConstants.EndpointRoutePaths.EndSessionCallback;
            endSessionCallbackUrl =
                endSessionCallbackUrl.AddQueryString(ApplicationUIConstants.DefaultRoutePathParams.EndSessionCallback,
                    logoutId);
            return endSessionCallbackUrl;
        }

        return null;
    }
}
