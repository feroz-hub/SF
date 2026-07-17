using System.Security.Claims;
using System.Text.Encodings.Web;
using Zentra.Domain;
using Zentra.Domain.Entities.Api;
using Zentra.Domain.Models.Api.Response;
using Zentra.Domain.Models.Endpoint.Validation;
using Zentra.DomainServices.Infra;
using Zentra.DomainServices.Repository.Api;
using Zentra.DomainServices.Wrappers;
using Zentra.Service.Implementation.Api.Services;
using Zentra.Service.Interfaces.Interfaces.Api;
using Zentra.Service.Interfaces.Interfaces.Api.Wrapper;
using Zentra.Service.Interfaces.Interfaces.Endpoint;

namespace Zentra.ProxyService.Proxy;

public sealed class AuthenticationProxyService : AuthenticationService, IAuthenticationService
{
    private readonly IApiValidator apiValidator;
    private readonly IFrameworkResultService frameworkResult;

    public AuthenticationProxyService(
        UserManagerWrapper<Users> userManager,
        SignInManagerWrapper<Users> signInManager,
        ILoggerInstance instance,
        IResourceStringHandler resourceStringHandler,
        IFrameworkResultService frameworkResultService,
        ZentraConfig frameworkConfig,
        UrlEncoder urlEncoder,
        IUserAccountService userAccountService,
        IAuthorizationService authorizationService,
        ITokenGenerationService tokenGenerationService,
        IUserRepository userRepository,
        ISessionManagementService session,
        IApiValidator apiValidator)
        : base(
            userManager,
            signInManager,
            instance,
            resourceStringHandler,
            frameworkResultService,
            frameworkConfig,
            urlEncoder,
            userAccountService,
            authorizationService,
            tokenGenerationService,
            userRepository,
            session)
    {
        this.apiValidator = apiValidator;
        frameworkResult = frameworkResultService;
    }

    public override async Task<IEnumerable<string>> GenerateRecoveryCodesAsync(Guid userId)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed)
            frameworkResult.ThrowCustomMessage(result.Errors.FirstOrDefault().Description);

        return await base.GenerateRecoveryCodesAsync(userId);
    }

    public override async Task<int> CountRecoveryCodesAsync(Guid userId)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed)
            frameworkResult.ThrowCustomMessage(result.Errors.FirstOrDefault().Description);

        return await base.CountRecoveryCodesAsync(userId);
    }

    public override async Task<bool> IsUserSignedInAsync(ClaimsPrincipal principal)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed)
            frameworkResult.ThrowCustomMessage(result.Errors.FirstOrDefault().Description);

        return await base.IsUserSignedInAsync(principal);
    }

    public override async Task<SignInResponseModel> PasswordSignInAsync(string username, string password)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed)
            frameworkResult.ThrowCustomMessage(result.Errors.FirstOrDefault().Description);

        return await base.PasswordSignInAsync(username, password);
    }

    public override async Task<SignInResponseModel> PasswordSignInAsync(string username, string password,
        string twoFactorAuthenticatorToken)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed)
            frameworkResult.ThrowCustomMessage(result.Errors.FirstOrDefault().Description);

        return await base.PasswordSignInAsync(username, password, twoFactorAuthenticatorToken);
    }

    public override async Task<FrameworkResult> ResetAuthenticatorAppAsync(Guid userId)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed)
            frameworkResult.ThrowCustomMessage(result.Errors.FirstOrDefault().Description);

        return await base.ResetAuthenticatorAppAsync(userId);
    }

    public override async Task<RopValidationModel> RopValidateCredentialsAsync(RopValidationModel validationModel)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed)
            frameworkResult.ThrowCustomMessage(result.Errors.FirstOrDefault().Description);

        return await base.RopValidateCredentialsAsync(validationModel);
    }

    public override async Task<AuthenticatorAppSetupResponseModel> SetupAuthenticatorAppAsync(Guid userId,
        string applicationName)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed)
            frameworkResult.ThrowCustomMessage(result.Errors.FirstOrDefault().Description);

        return await base.SetupAuthenticatorAppAsync(userId, applicationName);
    }

    public override async Task<FrameworkResult> SignOutAsync()
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed)
            frameworkResult.ThrowCustomMessage(result.Errors.FirstOrDefault().Description);

        return await base.SignOutAsync();
    }

    public override async Task<SignInResponseModel> TwoFactorAuthenticatorAppSignInAsync(string code)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed)
            frameworkResult.ThrowCustomMessage(result.Errors.FirstOrDefault().Description);

        return await base.TwoFactorAuthenticatorAppSignInAsync(code);
    }

    public override async Task<SignInResponseModel> TwoFactorEmailSignInAsync(string code)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed)
            frameworkResult.ThrowCustomMessage(result.Errors.FirstOrDefault().Description);

        return await base.TwoFactorEmailSignInAsync(code);
    }

    public override async Task<SignInResponseModel> TwoFactorRecoveryCodeSignInAsync(string recoveryCode)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed)
            frameworkResult.ThrowCustomMessage(result.Errors.FirstOrDefault().Description);

        return await base.TwoFactorRecoveryCodeSignInAsync(recoveryCode);
    }

    public override async Task<SignInResponseModel> TwoFactorSmsSignInAsync(string code)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed)
            frameworkResult.ThrowCustomMessage(result.Errors.FirstOrDefault().Description);

        return await base.TwoFactorSmsSignInAsync(code);
    }

    public override async Task<AuthenticatorAppResponseModel> VerifyAuthenticatorAppSetupAsync(Guid userId,
        string token)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed)
            frameworkResult.ThrowCustomMessage(result.Errors.FirstOrDefault().Description);

        return await base.VerifyAuthenticatorAppSetupAsync(userId, token);
    }
}
