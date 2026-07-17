using System.Security.Claims;
using Zentra.Domain;
using Zentra.Domain.Models.Api.Response;
using Zentra.Domain.Models.Endpoint.Validation;

namespace Zentra.Service.Interfaces.Interfaces.Api;

public interface IAuthenticationService
{
    Task<SignInResponseModel> PasswordSignInAsync(string username, string password);

    Task<SignInResponseModel> PasswordSignInAsync(string username, string password, string twoFactorAuthenticatorToken);

    Task<SignInResponseModel> TwoFactorEmailSignInAsync(string code);

    Task<SignInResponseModel> TwoFactorSmsSignInAsync(string code);

    Task<SignInResponseModel> TwoFactorAuthenticatorAppSignInAsync(string code);

    Task<SignInResponseModel> TwoFactorRecoveryCodeSignInAsync(string recoveryCode);

    Task<FrameworkResult> SignOutAsync();

    Task<bool> IsUserSignedInAsync(ClaimsPrincipal principal);

    Task<AuthenticatorAppSetupResponseModel> SetupAuthenticatorAppAsync(Guid userId, string applicationName);

    Task<AuthenticatorAppResponseModel> VerifyAuthenticatorAppSetupAsync(Guid userId, string token);

    Task<FrameworkResult> ResetAuthenticatorAppAsync(Guid userId);

    Task<IEnumerable<string>> GenerateRecoveryCodesAsync(Guid userId);

    Task<int> CountRecoveryCodesAsync(Guid userId);
    // ! @cond

    Task<RopValidationModel> RopValidateCredentialsAsync(RopValidationModel validationModel);

    // ! @endcond
}
