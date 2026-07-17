using Microsoft.AspNetCore.Identity;

namespace HCL.CS.Service.Interfaces.Interfaces.Api.Wrapper;

public interface IWindowsSignInManagerWrapper<TUser>
    where TUser : class
{
    Task<SignInResult> PasswordSignInAsync(TUser user, string password, bool lockoutOnFailure);

    Task<SignInResult> TwoFactorSignInAsync(Guid userId, string provider, string code);

    Task<SignInResult> TwoFactorAuthenticatorSignInAsync(Guid userId, string code);

    Task<SignInResult> TwoFactorRecoveryCodeSignInAsync(Guid userId, string recoveryCode);
}
