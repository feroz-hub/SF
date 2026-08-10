/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using System.Security.Claims;
using HCL.CS.Domain;
using HCL.CS.Domain.Models.Api;
using HCL.CS.Domain.Models.Api.Response;
using HCL.CS.Domain.Models.Endpoint.Validation;

namespace HCL.CS.Service.Interfaces.Interfaces.Api;

public interface IAuthenticationService
{
    Task<AuthenticationAvailabilityModel> GetAuthenticationAvailabilityAsync();

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
