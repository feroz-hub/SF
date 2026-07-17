using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Zentra.Domain;
using Zentra.Domain.Constants.Endpoint;
using Zentra.Domain.Entities.Api;
using Zentra.Domain.Entities.Endpoint;
using Zentra.Domain.Models.Endpoint.Request;
using Zentra.DomainServices;
using Zentra.DomainServices.Infra;
using Zentra.DomainServices.Wrappers;
using Zentra.Service.Implementation.Endpoint.Extensions;
using Zentra.Service.Implementation.Endpoint.Results;
using Zentra.Service.Interfaces.Interfaces.Endpoint;
using Zentra.Service.Interfaces.Interfaces.Endpoint.Results;
using Zentra.Service.Interfaces.Interfaces.Endpoint.Validators;

namespace Zentra.Service.Implementation.Endpoint;

internal class AuthorizeCallBackEndpoint : AuthorizationCodeBase, IEndpoint
{
    public AuthorizeCallBackEndpoint(
        ILoggerInstance instance,
        IResourceScopeValidator resourceScopeValidator,
        IAuthorizationService authorizationService,
        IRepository<Clients> clientRepository,
        IMapper mapper,
        ISessionManagementService session,
        ZentraConfig tokenSettings,
        SignInManagerWrapper<Users> csSignInManager,
        IFrameworkResultService frameworkResultService)
        : base(
            instance,
            resourceScopeValidator,
            authorizationService,
            clientRepository,
            mapper,
            session,
            tokenSettings,
            csSignInManager,
            frameworkResultService)
    {
    }

    public async Task<IEndpointResult> ProcessAsync(HttpContext context)
    {
        if (!HttpMethods.IsGet(context.Request.Method))
        {
            LoggerService.WriteTo(Log.Error, "Invalid HTTP method for authorize endpoint.");
            return new StatusCodeResult(HttpStatusCode.MethodNotAllowed);
        }

        var requestCollection = context.Request.Query.ConvertCollection();
        var redirectUrlId =
            requestCollection.GetValueFromDictionary(AuthenticationConstants.AuthCodeStore.ReturnUrlCode);
        var savedUrlCollection = await AuthorizationService.ValidateReturnUrlAsync(redirectUrlId);
        if (!string.IsNullOrWhiteSpace(redirectUrlId) && savedUrlCollection == null)
        {
            LoggerService.WriteTo(Log.Warning,
                "Authorize callback could not restore the saved authorize request. " +
                $"returnUrlId={redirectUrlId}");
            var invalidRequest = new ValidatedAuthorizeRequestModel
            {
                RequestRawData = requestCollection
            };
            return invalidRequest.Error(
                SessionService,
                OpenIdConstants.Errors.InvalidRequest,
                "Authorization request context expired or is invalid. Start sign-in again.");
        }

        var shouldDeleteReturnUrl = savedUrlCollection != null;
        if (savedUrlCollection != null)
        {
            requestCollection = savedUrlCollection;
            var actualUser = await SessionService.GetUserPrincipalFromContextAsync();
            var actualUserName = actualUser?.Identity?.Name;
            if (string.IsNullOrWhiteSpace(actualUserName))
                LoggerService.WriteTo(Log.Warning,
                    "Unable to resolve authenticated user in authorize callback. Falling back to normal authorize navigation.");

            var verificationCode = context.Request.Query
                .ConvertCollection()
                .GetValueFromDictionary(AuthenticationConstants.AuthCodeStore.UserVerificationCode);
            if (!string.IsNullOrWhiteSpace(actualUserName) && !string.IsNullOrWhiteSpace(verificationCode))
            {
                var securityToken = await AuthorizationService.ValidateVerificationCodeAsync(verificationCode);
                if (securityToken != null
                    && string.Equals(securityToken.TokenValue, actualUserName, StringComparison.OrdinalIgnoreCase)
                    && securityToken.CreationTime.AddSeconds(securityToken.ExpiresAt) >= DateTime.UtcNow)
                    await AuthorizationService.DeleteSecurityTokenByTokenValueAsync(verificationCode);
                else
                    LoggerService.WriteTo(Log.Warning,
                        "User verification code mismatch/expired. Continuing with authenticated session.");
            }
            else
            {
                LoggerService.WriteTo(Log.Warning,
                    "User verification code missing or user session unavailable. Continuing with authorize navigation.");
            }
        }

        try
        {
            var result = await ProcessAuthorizeRequestAsync(context, requestCollection);
            LoggerService.WriteTo(Log.Debug, "End Authorize Request");
            return result;
        }
        finally
        {
            if (shouldDeleteReturnUrl && Guid.TryParse(redirectUrlId, out var returnUrlTokenId))
                try
                {
                    await AuthorizationService.DeleteSecurityTokenByIdAsync(returnUrlTokenId);
                }
                catch (Exception ex)
                {
                    LoggerService.WriteToWithCaller(Log.Warning, ex,
                        "Failed to delete return-url token in authorize callback cleanup.");
                }
        }
    }
}
