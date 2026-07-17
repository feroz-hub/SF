using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Zentra.Domain;
using Zentra.Domain.Constants;
using Zentra.Domain.Constants.Endpoint;
using Zentra.Domain.Entities.Api;
using Zentra.Domain.Entities.Endpoint;
using Zentra.Domain.ErrorCodes;
using Zentra.Domain.Models.Endpoint;
using Zentra.Domain.Models.Endpoint.Request;
using Zentra.Domain.Models.Endpoint.Response;
using Zentra.DomainServices.Infra;
using Zentra.DomainServices.Repository.Api;
using Zentra.DomainServices.UnitOfWork.Endpoint;
using Zentra.DomainServices.Wrappers;
using Zentra.Service.Extension;
using Zentra.Service.Implementation.Endpoint.Extensions;
using Zentra.Service.Interfaces.Interfaces.Endpoint;
using static Zentra.Domain.Constants.Endpoint.OpenIdConstants;

namespace Zentra.Service.Implementation.Endpoint.Services;

internal class AuthorizationService : SecurityBase, IAuthorizationService
{
    private readonly TokenSettings configSettings;
    private readonly IFrameworkResultService frameworkResultService;
    private readonly ILoggerService loggerService;
    private readonly IResourceStringHandler resourceStringHandler;
    private readonly ISecurityTokenCommandRepository securityTokenCommandRepository;
    private readonly ITokenGenerationService tokenGenerationService;
    private readonly IClientsUnitOfWork unitOfWork;
    private readonly UserManagerWrapper<Users> userManager;

    public AuthorizationService(
        ILoggerInstance instance,
        UserManagerWrapper<Users> userManager,
        ITokenGenerationService tokenGenerationService,
        IClientsUnitOfWork unitOfWork,
        ISecurityTokenCommandRepository securityTokenCommandRepository,
        ZentraConfig tokenSettings,
        IFrameworkResultService frameworkResultService,
        IResourceStringHandler resourceStringHandler)
    {
        this.frameworkResultService = frameworkResultService;
        this.userManager = userManager;
        this.tokenGenerationService = tokenGenerationService;
        this.unitOfWork = unitOfWork;
        this.securityTokenCommandRepository = securityTokenCommandRepository;
        configSettings = tokenSettings.TokenSettings;
        loggerService = instance.GetLoggerInstance(LoggerKeyConstants.DefaultLoggerKey);
        this.resourceStringHandler = resourceStringHandler;
    }

    public async Task<string> SaveAuthorizationCodeAsync(AuthorizationCodeModel authCodeRequest)
    {
        var returnValue = string.Empty;
        try
        {
            if (authCodeRequest == null) frameworkResultService.Throw(EndpointErrorCodes.ArgumentNullError);

            loggerService.WriteTo(Log.Debug,
                "Entered into save authorization code for client : " + authCodeRequest.ClientId);
            var securityToken = new SecurityTokens
            {
                CreationTime = DateTime.UtcNow,
                ExpiresAt = Math.Min(authCodeRequest.Lifetime, 600),
                TokenType = TokenType.AuthorizationCode,
                ClientId = authCodeRequest.ClientId,
                TokenValue = authCodeRequest.JsonClaimSerialize(),
                SessionId = authCodeRequest.SessionId ?? string.Empty
            };

            var codeHandle = AuthenticationConstants.KeySize32.RandomString();
            var codeHandleHash = codeHandle.ComputeSha256Hash();
            securityToken.Key = codeHandleHash;
            securityToken.SubjectId = codeHandleHash;
            returnValue = codeHandle;
            await unitOfWork.SecurityTokensRepository.InsertAsync(securityToken);
            var result = await unitOfWork.SecurityTokensRepository.SaveChangesAsync();
            if (result.Status == ResultStatus.Failed) return string.Empty;
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, ex.Message);
            throw;
        }

        return returnValue;
    }

    public async Task<AuthorizationCodeModel> GetAuthorizationCodeAsync(string authorizationCode)
    {
        if (string.IsNullOrWhiteSpace(authorizationCode))
            frameworkResultService.Throw(EndpointErrorCodes.ArgumentNullError);

        var authorizationCodeHash = authorizationCode.ComputeSha256Hash();
        var securityToken = await unitOfWork.SecurityTokensRepository.GetAsync(
            x => x.SubjectId == authorizationCodeHash && x.TokenType == TokenType.AuthorizationCode,
            x => new
            {
                x.Id,
                x.TokenValue,
                x.SessionId,
                x.Key,
                x.ConsumedAt,
                x.ConsumedTime
            });
        if (securityToken.ContainsAny() && !string.IsNullOrWhiteSpace(securityToken[0].TokenValue))
        {
            var token = securityToken.FirstOrDefault();
            loggerService.WriteTo(Log.Debug, "Entered into get authorization code with session id: " + token.SessionId);

            if (token.Key == authorizationCodeHash)
            {
                if (token.ConsumedAt.HasValue || token.ConsumedTime.HasValue)
                    return frameworkResultService.Failed<AuthorizationCodeModel>(EndpointErrorCodes
                        .InvalidAuthorizationCode);

                var updatedRows = await securityTokenCommandRepository
                    .ConsumeAuthorizationCodeAsync(token.Id, DateTime.UtcNow);

                if (updatedRows != 1)
                    return frameworkResultService.Failed<AuthorizationCodeModel>(EndpointErrorCodes
                        .InvalidAuthorizationCode);

                var tokenValue = token.TokenValue.JsonClaimDeserialize<AuthorizationCodeModel>();
                return tokenValue;
            }

            return frameworkResultService.Failed<AuthorizationCodeModel>(EndpointErrorCodes.InvalidAuthorizationCode);
        }

        return frameworkResultService.Failed<AuthorizationCodeModel>(EndpointErrorCodes.ArgumentNullError);
    }

    public async Task<FrameworkResult> DeleteAuthorizationCodeAsync(string authorizationCode)
    {
        if (string.IsNullOrWhiteSpace(authorizationCode))
            return frameworkResultService.Failed<FrameworkResult>(EndpointErrorCodes.ArgumentNullError);

        var authorizationCodeHash = authorizationCode.ComputeSha256Hash();
        var securityToken = await unitOfWork.SecurityTokensRepository.GetAsync(x =>
            x.SubjectId == authorizationCodeHash && x.TokenType == TokenType.AuthorizationCode);
        if (securityToken.ContainsAny() && !string.IsNullOrWhiteSpace(securityToken[0].TokenValue))
        {
            var token = securityToken.FirstOrDefault();
            loggerService.WriteTo(Log.Debug,
                "Entered into delete authorization code with session id: " + token.SessionId);
            await unitOfWork.SecurityTokensRepository.DeleteAsync(token);
            return await unitOfWork.SecurityTokensRepository.SaveChangesWithHardDeleteAsync();
        }

        return frameworkResultService.Failed<FrameworkResult>(EndpointErrorCodes.InvalidAuthorizationCode);
    }

    public async Task<Guid> SaveReturnUrlAsync(ValidatedAuthorizeRequestModel authCodeRequest)
    {
        if (authCodeRequest == null) frameworkResultService.Throw(EndpointErrorCodes.ArgumentNullError);

        try
        {
            var securityToken = new SecurityTokens
            {
                CreationTime = DateTime.UtcNow,
                ExpiresAt = 120,
                TokenType = TokenType.RequestParameter,
                TokenValue = HttpContextExtension.JsonSerialize(authCodeRequest.RequestRawData),
                ClientId = authCodeRequest.ClientId,
                SessionId = authCodeRequest.SessionId ?? string.Empty
            };

            await unitOfWork.SecurityTokensRepository.InsertAsync(securityToken);
            var result = await unitOfWork.SecurityTokensRepository.SaveChangesAsync();
            if (result.Status == ResultStatus.Succeeded)
            {
                loggerService.WriteTo(Log.Debug, "Saved return url with session id: " + securityToken.SessionId);
                return securityToken.Id;
            }

            var persistenceError = result.Errors?.FirstOrDefault()?.Description;
            if (!string.IsNullOrWhiteSpace(persistenceError))
                frameworkResultService.ThrowCustomMessage(
                    $"{EndpointErrorCodes.FailedToStoreReturnUrl}: {persistenceError}");

            frameworkResultService.Throw(EndpointErrorCodes.FailedToStoreReturnUrl);

            return securityToken.Id;
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, ex.Message);
            throw;
        }
    }

    public async Task<Dictionary<string, string>> ValidateReturnUrlAsync(string requestId)
    {
        if (string.IsNullOrWhiteSpace(requestId)) return null;

        if (!Guid.TryParse(requestId, out var parsedRequestId))
        {
            loggerService.WriteTo(Log.Warning,
                "Invalid returnUrlId supplied to authorize callback: " + requestId);
            return frameworkResultService.Failed<Dictionary<string, string>>(EndpointErrorCodes.InvalidRequest);
        }

        var securityToken = await unitOfWork.SecurityTokensRepository.GetAsync(
            x => x.Id == parsedRequestId,
            x => new { x.TokenValue, x.SessionId });
        if (securityToken.ContainsAny() && !string.IsNullOrWhiteSpace(securityToken[0].TokenValue))
        {
            var token = securityToken.FirstOrDefault();
            loggerService.WriteTo(Log.Debug, "Entered into validate return url with session id: " + token.SessionId);
            var requestCollection = token.TokenValue.JsonDeserialize<Dictionary<string, string>>();
            return requestCollection;
        }

        return frameworkResultService.Failed<Dictionary<string, string>>(EndpointErrorCodes.InvalidRequest);
    }

    public async Task<FrameworkResult> DeleteSecurityTokenByIdAsync(Guid id)
    {
        if (id.IsValid())
        {
            await unitOfWork.SecurityTokensRepository.DeleteAsync(id);
            loggerService.WriteTo(Log.Debug, "Entered into delete security token by id.");
            return await unitOfWork.SecurityTokensRepository.SaveChangesWithHardDeleteAsync();
        }

        return frameworkResultService.Failed<FrameworkResult>(EndpointErrorCodes.InvalidSecurityTokenId);
    }

    public async Task<string> SaveVerificationCodeAsync(string name)
    {
        // TODO - To analyze on the scenario for saving the verification code while logging in the User with credentials.
        if (string.IsNullOrWhiteSpace(name)) frameworkResultService.Throw(EndpointErrorCodes.ArgumentNullError);

        try
        {
            var securityToken = new SecurityTokens
            {
                CreationTime = DateTime.UtcNow,
                ExpiresAt = AuthenticationConstants.DefaultVerificationCodeExpiryDuration,
                TokenType = TokenType.VerificationCode,
                TokenValue = name.AesEncrypt(EncryptionKeyConstants.VerificationEncryptedKey,
                    AuthenticationConstants.KeySize32),
                SessionId = string.Empty
            };
            loggerService.WriteTo(Log.Debug,
                "Entered into save verification code with session id : " + securityToken.SessionId);
            await unitOfWork.SecurityTokensRepository.InsertAsync(securityToken);
            var result = await unitOfWork.SecurityTokensRepository.SaveChangesAsync();
            if (result.Status == ResultStatus.Failed) return string.Empty;

            return securityToken.TokenValue;
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, ex.Message);
            throw;
        }
    }

    public async Task<SecurityTokens> ValidateVerificationCodeAsync(string tokenValue)
    {
        // TODO - To analyze on the scenario for saving the verification code while logging in the User with credentials.
        if (string.IsNullOrWhiteSpace(tokenValue)) frameworkResultService.Throw(EndpointErrorCodes.ArgumentNullError);

        var securityToken = await unitOfWork.SecurityTokensRepository.GetAsync(x => x.TokenValue == tokenValue);
        if (securityToken.ContainsAny() && !string.IsNullOrWhiteSpace(securityToken[0].TokenValue))
        {
            var token = securityToken.FirstOrDefault();
            loggerService.WriteTo(Log.Debug,
                "Entered into validate verification code with session id : " + token.SessionId);
            token.TokenValue = token.TokenValue.AesDecrypt(EncryptionKeyConstants.VerificationEncryptedKey,
                AuthenticationConstants.KeySize32);
            return token;
        }

        return null;
    }

    public async Task<FrameworkResult> DeleteSecurityTokenByTokenValueAsync(string tokenValue)
    {
        if (!string.IsNullOrWhiteSpace(tokenValue))
        {
            var securityToken = await unitOfWork.SecurityTokensRepository.GetAsync(x => x.TokenValue == tokenValue);
            if (securityToken.ContainsAny() && !string.IsNullOrWhiteSpace(securityToken[0].TokenValue))
            {
                var token = securityToken.FirstOrDefault();
                loggerService.WriteTo(Log.Debug,
                    "Entered into delete security token by token value with session id : " + token.SessionId);
                await unitOfWork.SecurityTokensRepository.DeleteAsync(token);
                return await unitOfWork.SecurityTokensRepository.SaveChangesWithHardDeleteAsync();
            }
        }

        return null;
    }

    public async Task<NavigationModel> CheckNavigationAsync(ValidatedAuthorizeRequestModel requestValidationModel)
    {
        if (requestValidationModel == null) frameworkResultService.Throw(EndpointErrorCodes.ArgumentNullError);

        var result = await IsLoginRequiredAsync(requestValidationModel);
        if (result.IsLogin && requestValidationModel.PromptModes.ContainsAny() &&
            requestValidationModel.PromptModes.Contains(PromptModes.None))
        {
            // UI restricted prompt=none
            loggerService.WriteTo(Log.Debug, "Changing response to login required : prompt=none was requested.");
            result = new NavigationModel
            {
                ErrorCode = Errors.LoginRequired,
                ErrorDescription =
                    resourceStringHandler.GetResourceString(EndpointErrorCodes.LoginRequiredAndPromptNone)
            };
        }

        return result;
    }

    public async Task<AuthorizationResponseModel> ProcessAuthorizationCodeAsync(
        ValidatedAuthorizeRequestModel requestValidationModel)
    {
        if (requestValidationModel == null) frameworkResultService.Throw(EndpointErrorCodes.ArgumentNullError);

        if (requestValidationModel.GrantType == AuthenticationConstants.GrantType.AuthorizationCode)
            return await ProcessCodeFlowResponseAsync(requestValidationModel);

        return new AuthorizationResponseModel
        {
            IsError = true,
            ErrorCode = Errors.InvalidRequest,
            ErrorDescription = EndpointErrorCodes.InvalidGrantTypeForEndpoint
        };
    }

    private string GenerateSessionStateValue(ValidatedAuthorizeRequestModel request)
    {
        if (request == null) return frameworkResultService.Failed<string>(EndpointErrorCodes.ArgumentNullError);

        if (!configSettings.EndpointsConfig.FrontchannelLogoutSupported ||
            !configSettings.EndpointsConfig.FrontchannelLogoutSessionRequired)
            return null;

        if (!request.IsOpenIdRequest || request.SessionId == null || string.IsNullOrWhiteSpace(request.ClientId) ||
            string.IsNullOrWhiteSpace(request.RedirectUri))
            return frameworkResultService.Failed<string>(EndpointErrorCodes.InvalidRequest);

        var clientId = request.ClientId;
        var sessionId = request.SessionId;
        var salt = AuthenticationConstants.KeySize16.RandomString();

        var uri = new Uri(request.RedirectUri);
        var origin = uri.Scheme + "://" + uri.Host;
        if (!uri.IsDefaultPort) origin += ":" + uri.Port;

        var bytes = Encoding.UTF8.GetBytes(clientId + origin + sessionId + salt);
        byte[] hash;

        using (var sha = SHA256.Create())
        {
            hash = sha.ComputeHash(bytes);
        }

        return hash.Encode() + "." + salt;
    }

    private async Task<AuthorizationCodeModel> BuildAuthorizationCodeRequestAsync(
        ValidatedAuthorizeRequestModel requestValidationModel)
    {
        var code = new AuthorizationCodeModel
        {
            CreationTime = DateTimeOffset.UtcNow.UtcDateTime,
            ClientId = requestValidationModel.Client.ClientId,
            Lifetime = Math.Min(requestValidationModel.Client.AuthorizationCodeExpiration, 600),
            Subject = requestValidationModel.Subject,
            SessionId = requestValidationModel.SessionId,
            CodeChallenge = requestValidationModel.CodeChallenge,
            CodeChallengeMethod = requestValidationModel.CodeChallengeMethod,

            IsOpenId = requestValidationModel.IsOpenIdRequest,
            RedirectUri = requestValidationModel.RedirectUri,
            Nonce = requestValidationModel.Nonce,
            RequestedScopes = requestValidationModel.RequestedScopes,
            AllowedScopesParserModel = requestValidationModel.AllowedScopesParserModel,
            TokenDetails = requestValidationModel.AllowedScopesParserModel.TokenDetails
        };
        return await Task.FromResult(code);
    }

    private async Task<NavigationModel> IsLoginRequiredAsync(ValidatedAuthorizeRequestModel requestValidationModel)
    {
        var requiredLogin = new NavigationModel
            { IsLogin = true, IsError = false }; // RedirectUrl = requestValidationModel.RedirectUri
        if (requestValidationModel.PromptModes.Contains(PromptModes.Login) ||
            requestValidationModel.PromptModes.Contains(PromptModes.SelectAccount))
        {
            // remove prompt so when we redirect back in from login page
            // we won't think we need to force a prompt again
            requestValidationModel.PromptModes = null;
            requestValidationModel.RequestRawData.Remove(AuthorizeRequest.Prompt);

            return requiredLogin;
        }

        // unauthenticated user
        var isAuthenticated = requestValidationModel.Subject != null &&
                              requestValidationModel.Subject.Identity != null &&
                              requestValidationModel.Subject.Identity.IsAuthenticated;
        if (isAuthenticated)
        {
            var isUserExists = await IsUserExistsAsync(requestValidationModel.Subject);
            if (!isUserExists)
            {
                loggerService.WriteToWithCaller(Log.Debug, "User is not active.");
                return requiredLogin;
            }
        }
        else
        {
            loggerService.WriteToWithCaller(Log.Debug, "User is not authenticated.");
            return requiredLogin;
        }

        // check authentication freshness
        if (requestValidationModel.MaxAge.HasValue)
        {
            var authTime = requestValidationModel.Subject.GetAuthenticationTimeFromIdentity();
            if (authTime != null)
                if (DateTime.UtcNow > authTime.Value.AddSeconds(requestValidationModel.MaxAge.Value))
                    return requiredLogin;
        }

        return new NavigationModel { IsLogin = false, IsError = false };
    }

    private async Task<AuthorizationResponseModel> ProcessHybridFlowResponseAsync(
        ValidatedAuthorizeRequestModel requestValidationModel)
    {
        if (requestValidationModel == null) frameworkResultService.Throw(EndpointErrorCodes.ArgumentNullError);

        var authCode = await BuildAuthorizationCodeRequestAsync(requestValidationModel);
        var decryptedCode = await SaveAuthorizationCodeAsync(authCode);

        var response = await ProcessTokenFlowResponseAsync(requestValidationModel, decryptedCode);
        response.Code = decryptedCode;

        return response;
    }

    private async Task<AuthorizationResponseModel> ProcessCodeFlowResponseAsync(
        ValidatedAuthorizeRequestModel requestValidationModel)
    {
        var authCode = await BuildAuthorizationCodeRequestAsync(requestValidationModel);
        var decryptedCode = await SaveAuthorizationCodeAsync(authCode);

        var response = new AuthorizationResponseModel
        {
            IsError = false,
            Request = requestValidationModel,
            Code = decryptedCode,
            SessionState = GenerateSessionStateValue(requestValidationModel),
            Scope = requestValidationModel.AllowedScopesParserModel.ParsedAllowedScopes.ConvertSpaceSeparatedString()
        };

        return response;
    }

    // TO-DO Check how State Hash is used.
    private async Task<AuthorizationResponseModel> ProcessTokenFlowResponseAsync(
        ValidatedAuthorizeRequestModel requestValidationModel, string authorizationCode = null)
    {
        loggerService.WriteToWithCaller(Log.Debug,
            "Processing Token Flow response." + "Client Id = " + requestValidationModel.ClientId);
        var responseTypes = requestValidationModel.ResponseType.SplitBySpace();
        var tokenRequest = new ValidatedTokenRequestModel
        {
            Issuer = configSettings.TokenConfig.IssuerUri,
            Client = requestValidationModel.Client,
            ClientId = requestValidationModel.ClientId,
            GrantType = requestValidationModel.GrantType,
            Nonce = requestValidationModel.Nonce,
            State = requestValidationModel.State,
            AllowedScopesParserModel = requestValidationModel.AllowedScopesParserModel,
            TokenDetails = requestValidationModel.AllowedScopesParserModel.TokenDetails,
            IsRequestFromAuthorizationEndpoint = true,
            ResponseTypes = responseTypes.ToList(),
            Subject = requestValidationModel.Subject,
            AuthorizationCodeToHash = authorizationCode
        };

        var result = await tokenGenerationService.ProcessTokenAsync(tokenRequest);

        var response = new AuthorizationResponseModel
        {
            Request = requestValidationModel,
            AccessTokenLifetime = result.AccessTokenExpiresIn,
            SessionState = GenerateSessionStateValue(requestValidationModel)
        };

        if (result.AccessToken != null) response.AccessToken = result.AccessToken;

        if (result.IdentityToken != null) response.IdentityToken = result.IdentityToken;

        if (result.RefreshToken != null) response.RefreshToken = result.RefreshToken;

        if (result.Scope != null) response.Scope = result.Scope;

        return response;
    }

    private async Task DeleteReturnUrlAsync(Guid id)
    {
        if (id.IsValid())
        {
            loggerService.WriteTo(Log.Debug, "Entering into Delete Return Url. Guid =" + id);
            await unitOfWork.SecurityTokensRepository.DeleteAsync(id);
            await unitOfWork.SecurityTokensRepository.SaveChangesWithHardDeleteAsync();
        }
        else
        {
            frameworkResultService.Throw(EndpointErrorCodes.InvalidSecurityTokenId);
        }
    }

    private async Task<bool> IsUserExistsAsync(ClaimsPrincipal claimsPrincipal)
    {
        var id = claimsPrincipal.Identity as ClaimsIdentity;
        if (id != null)
        {
            var claim = id.FindFirst(OpenIdConstants.ClaimTypes.Sub);
            if (claim != null)
            {
                Users user = null;
                if (claim.Value.IsGuid())
                {
                    user = await userManager.FindByIdAsync(claim.Value);
                    if (user != null)
                    {
                        loggerService.WriteTo(Log.Debug, "Entered into Is User Exists for user :" + user.UserName);
                        return true;
                    }
                }

                user = await userManager.FindByNameAsync(claim.Value);
                if (user != null)
                {
                    loggerService.WriteTo(Log.Debug, "Entered into IsUser exists : " + user.UserName);
                    return true;
                }
            }
        }

        return false;
    }
}
