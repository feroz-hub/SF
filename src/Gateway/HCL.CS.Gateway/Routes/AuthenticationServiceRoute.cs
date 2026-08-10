/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using System.Security.Claims;
using Newtonsoft.Json.Linq;
using HCL.CS.Domain.Constants;
using HCL.CS.Domain.Models.Endpoint.Validation;
using HCL.CS.ProxyService.Routes.Extension;
using HCL.CS.Service.Interfaces.Interfaces.Api.Wrapper;

namespace HCL.CS.ProxyService.Routes;

internal partial class ApiGateway : BaseApiServiceInstance, IApiGateway
{
    private async Task<bool> GetAuthenticationAvailability(string jsonContent)
    {
        var availability = await AuthenticationService.GetAuthenticationAvailabilityAsync();
        await GenerateApiResults(availability);
        return true;
    }

    private async Task<bool> GenerateRecoveryCodes(string jsonContent)
    {
        var userId = jsonContent.JsonDeserialize<Guid>();

        var recoveryCodes = await AuthenticationService.GenerateRecoveryCodesAsync(userId);
        await GenerateApiResults(recoveryCodes);
        return true;
    }

    private async Task<bool> CountRecoveryCodesAsync(string jsonContent)
    {
        var userId = jsonContent.JsonDeserialize<Guid>();

        var recoveryCodesCount = await AuthenticationService.CountRecoveryCodesAsync(userId);
        await GenerateApiResults(recoveryCodesCount);
        return true;
    }

    private async Task<bool> IsUserSignedIn(string jsonContent)
    {
        var claimsPrincipal = jsonContent.JsonDeserialize<ClaimsPrincipal>();

        var frameworkResult = await AuthenticationService.IsUserSignedInAsync(claimsPrincipal);
        await GenerateApiResults(frameworkResult);
        return true;
    }

    private async Task<bool> PasswordSignIn(string jsonContent)
    {
        var jsonObjects = JObject.Parse(jsonContent);
        var userName = jsonObjects[ApiRouteParameterConstants.UserName].ToObject<string>();
        var password = jsonObjects[ApiRouteParameterConstants.Password].ToObject<string>();

        var frameworkResult = await AuthenticationService.PasswordSignInAsync(userName, password);
        await GenerateApiResults(frameworkResult);
        return true;
    }

    private async Task<bool> PasswordSignInByTwoFactorAuthenticatorToken(string jsonContent)
    {
        var jsonObjects = JObject.Parse(jsonContent);
        var userName = jsonObjects[ApiRouteParameterConstants.UserName].ToObject<string>();
        var password = jsonObjects[ApiRouteParameterConstants.Password].ToObject<string>();
        var twofactorToken = jsonObjects[ApiRouteParameterConstants.TwoFactorAuthenticatorToken].ToObject<string>();

        var frameworkResult = await AuthenticationService.PasswordSignInAsync(userName, password, twofactorToken);
        await GenerateApiResults(frameworkResult);
        return true;
    }

    private async Task<bool> ResetAuthenticatorApp(string jsonContent)
    {
        var userId = jsonContent.JsonDeserialize<Guid>();
        var frameworkResult = await AuthenticationService.ResetAuthenticatorAppAsync(userId);
        await GenerateApiResults(frameworkResult);
        return true;
    }

    private async Task<bool> RopValidateCredentials(string jsonContent)
    {
        var ropValidationModel = jsonContent.JsonDeserialize<RopValidationModel>();
        var frameworkResult = await AuthenticationService.RopValidateCredentialsAsync(ropValidationModel);
        await GenerateApiResults(frameworkResult);
        return true;
    }

    private async Task<bool> SetupAuthenticatorApp(string jsonContent)
    {
        var jsonObjects = JObject.Parse(jsonContent);
        var userId = jsonObjects[ApiRouteParameterConstants.UserId].ToObject<Guid>();
        var applicationName = jsonObjects[ApiRouteParameterConstants.ApplicationName].ToObject<string>();

        var frameworkResult = await AuthenticationService.SetupAuthenticatorAppAsync(userId, applicationName);
        await GenerateApiResults(frameworkResult);
        return true;
    }

    private async Task<bool> SignOut(string jsonContent)
    {
        var frameworkResult = await AuthenticationService.SignOutAsync();
        await GenerateApiResults(frameworkResult);
        return true;
    }

    private async Task<bool> TwoFactorAuthenticatorAppSignIn(string jsonContent)
    {
        var code = jsonContent.JsonDeserialize<string>();
        var frameworkResult = await AuthenticationService.TwoFactorAuthenticatorAppSignInAsync(code);
        await GenerateApiResults(frameworkResult);
        return true;
    }

    private async Task<bool> TwoFactorEmailSignIn(string jsonContent)
    {
        var code = jsonContent.JsonDeserialize<string>();
        var frameworkResult = await AuthenticationService.TwoFactorEmailSignInAsync(code);
        await GenerateApiResults(frameworkResult);
        return true;
    }

    private async Task<bool> TwoFactorRecoveryCodeSignIn(string jsonContent)
    {
        var code = jsonContent.JsonDeserialize<string>();
        var frameworkResult = await AuthenticationService.TwoFactorRecoveryCodeSignInAsync(code);
        await GenerateApiResults(frameworkResult);
        return true;
    }

    private async Task<bool> TwoFactorSmsSignInAsync(string jsonContent)
    {
        var code = jsonContent.JsonDeserialize<string>();
        var frameworkResult = await AuthenticationService.TwoFactorSmsSignInAsync(code);
        await GenerateApiResults(frameworkResult);
        return true;
    }

    private async Task<bool> VerifyAuthenticatorAppSetup(string jsonContent)
    {
        var jsonObjects = JObject.Parse(jsonContent);
        var userId = jsonObjects[ApiRouteParameterConstants.UserId].ToObject<Guid>();
        var token = jsonObjects[ApiRouteParameterConstants.UserToken].ToObject<string>();
        var frameworkResult = await AuthenticationService.VerifyAuthenticatorAppSetupAsync(userId, token);
        await GenerateApiResults(frameworkResult);
        return true;
    }
}
