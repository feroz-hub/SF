using Zentra.Domain;
using Zentra.Domain.Enums;

namespace Zentra.Service.Interfaces.Interfaces.Api;

public partial interface IUserAccountService
{
    Task<FrameworkResult> GenerateEmailConfirmationTokenAsync(string username);

    Task<FrameworkResult> VerifyEmailConfirmationTokenAsync(string username, string emailToken);

    Task<FrameworkResult> GeneratePhoneNumberConfirmationTokenAsync(string username);

    Task<FrameworkResult> VerifyPhoneNumberConfirmationTokenAsync(string username, string smsToken);

    Task<FrameworkResult> GeneratePasswordResetTokenAsync(string username,
        NotificationTypes notificationType = NotificationTypes.Email);

    Task<FrameworkResult> GenerateUserTokenAsync(string username, string purpose, string templateName,
        NotificationTypes notificationType = NotificationTypes.Email);

    Task<FrameworkResult> VerifyUserTokenAsync(string username, string purpose, string token);

    Task<FrameworkResult> GenerateEmailTwoFactorTokenAsync(string username);

    Task<FrameworkResult> VerifyEmailTwoFactorTokenAsync(string username, string emailToken);

    Task<FrameworkResult> GenerateSmsTwoFactorTokenAsync(string username);

    Task<FrameworkResult> VerifySmsTwoFactorTokenAsync(string username, string smsToken);
}
