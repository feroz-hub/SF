/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

namespace HCL.CS.Domain.Constants;

public class ProxyConstants
{
    public static readonly List<string> AnonymousApis = new()
    {
        "AddAuditTrailAsync", "RegisterUserAsync", "ResetPasswordAsync", "UnLockUserAsync", "IsUserExistsAsync",
        "AddClaimAsync", "GetAllSecurityQuestionsAsync", "AddUserSecurityQuestionAsync",
        "GenerateEmailConfirmationTokenAsync", "VerifyEmailConfirmationTokenAsync",
        "GeneratePhoneNumberConfirmationTokenAsync", "VerifyPhoneNumberConfirmationTokenAsync",
        "GeneratePasswordResetTokenAsync", "GenerateUserTokenAsync", "VerifyUserTokenAsync",
        "GenerateEmailTwoFactorTokenAsync", "VerifyEmailTwoFactorTokenAsync", "GenerateSmsTwoFactorTokenAsync",
        "VerifySmsTwoFactorTokenAsync", "GetAllTwoFactorTypeAsync", "PasswordSignInAsync", "TwoFactorEmailSignInAsync",
        "TwoFactorSmsSignInAsync", "TwoFactorAuthenticatorAppSignInAsync", "TwoFactorRecoveryCodeSignInAsync",
        "SignOutAsync", "IsUserSignedInAsync", "SetupAuthenticatorAppAsync", "VerifyAuthenticatorAppSetupAsync",
        "ResetAuthenticatorAppAsync", "GenerateRecoveryCodesAsync", "RopValidateCredentialsAsync"
    };

    public static readonly List<string> SecureApis = new()
    {
        "GetApiResourceAsync", "GetAllApiResourcesByScopesAsync", "GetAllApiResourcesAsync", "GetAllApiScopesAsync",
        "GetApiResourceClaimsAsync", "GetApiScopeAsync", "GetApiScopeClaimsAsync", "GetIdentityResourceAsync",
        "GetAllIdentityResourcesAsync", "GetAllIdentityResourcesByScopesAsync", "GetIdentityResourceClaimsAsync",
        "AddApiResourceAsync", "UpdateApiResourceAsync", "DeleteApiResourceAsync", "AddApiResourceClaimAsync",
        "DeleteApiResourceClaimByResourceIdAsync", "DeleteApiResourceClaimByIdAsync", "AddApiScopeAsync",
        "UpdateApiScopeAsync", "DeleteApiScopeAsync", "AddApiScopeClaimAsync", "DeleteApiScopeClaimByScopeIdAsync",
        "DeleteApiScopeClaimByIdAsync", "AddIdentityResourceAsync", "UpdateIdentityResourceAsync",
        "DeleteIdentityResourceAsync", "AddIdentityResourceClaimAsync", "DeleteIdentityResourceClaimByResourceIdAsync",
        "RegisterClientAsync", "UpdateClientAsync", "DeleteClientAsync", "GenerateClientSecret", "GetClientAsync",
        "GetAllClientAsync", "GetClientsActiveTokensAsync", "GetAuditDetailsAsync", "CreateRoleAsync",
        "UpdateRoleAsync", "DeleteRoleAsync", "GetRoleAsync", "AddRoleClaimAsync", "AddRoleClaimsAsync",
        "RemoveRoleClaimAsync", "RemoveRoleClaimsAsync", "GetRoleClaimAsync", "UpdateUserAsync", "DeleteUserAsync",
        "LockUserAsync", "GetUserByNameAsync", "GetUserByEmailAsync", "GetUserByIdAsync", "GetUsersForClaimAsync",
        "RemoveClaimAsync", "ReplaceClaimAsync", "GetClaimsAsync", "GetUserClaimsAsync", "RemoveAdminClaimAsync",
        "AddAdminClaimAsync", "AddUserRoleAsync", "AddUserRolesAsync", "RemoveUserRoleAsync", "RemoveUserRolesAsync",
        "GetUserRolesAsync", "GetUsersInRoleAsync", "GetUserRoleClaimsByIdAsync", "GetUserRoleClaimsByNameAsync",
        "AddSecurityQuestionAsync", "UpdateSecurityQuestionAsync", "DeleteSecurityQuestionAsync",
        "UpdateUserSecurityQuestionAsync", "DeleteUserSecurityQuestionAsync", "GetUserSecurityQuestionsAsync",
        "SetTwoFactorEnabledAsync", "UpdateUserTwoFactorTypeAsync", "CountRecoveryCodesAsync", "ChangePasswordAsync",
        "GetAllRolesAsync", "GetAllUsersAsync"
    };
}
