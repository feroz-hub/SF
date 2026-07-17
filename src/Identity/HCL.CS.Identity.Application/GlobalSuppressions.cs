// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly:
    SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private",
        Justification = "<Pending>", Scope = "member",
        Target = "~F:HCL.CS.Service.Implementation.AuthorizationCodeBase.LoggerService")]
[assembly:
    SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private",
        Justification = "<Pending>", Scope = "member",
        Target = "~F:HCL.CS.Service.Implementation.AuthorizationCodeBase.AuthorizationService")]
[assembly:
    SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private",
        Justification = "<Pending>", Scope = "member",
        Target = "~F:HCL.CS.Service.Implementation.AuthorizationCodeBase.CsSignInManager")]
[assembly:
    SuppressMessage("Major Bug", "S2259:Null pointers should not be dereferenced", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:HCL.CS.Service.Implementation.UserAccountService.RegisterUserAsync(HCL.CS.Domain.Models.UserModel)~System.Threading.Tasks.Task{HCL.CS.Domain.FrameworkResult}")]
[assembly:
    SuppressMessage("Major Bug", "S2259:Null pointers should not be dereferenced", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:HCL.CS.Service.Implementation.UserAccountService.DeleteAccountAsync(HCL.CS.Domain.Models.UserModel)~System.Threading.Tasks.Task{HCL.CS.Domain.FrameworkResult}")]
[assembly:
    SuppressMessage("Minor Code Smell", "S1075:URIs should not be hardcoded", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:HCL.CS.Service.Implementation.AuthenticationService.GenerateQrCodeUri(System.String,System.String,System.String,System.Text.Encodings.Web.UrlEncoder)~System.String")]
[assembly:
    SuppressMessage("Major Bug", "S2259:Null pointers should not be dereferenced", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:HCL.CS.Service.Implementation.AuthenticationService.TwoFactorRecoveryCodeSignInAsync(System.String,System.String)~System.Threading.Tasks.Task{HCL.CS.Domain.Models.ConstructSignInResponseModel}")]
[assembly:
    SuppressMessage("Major Bug", "S2259:Null pointers should not be dereferenced", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:HCL.CS.Service.Implementation.AuthenticationService.TwoFactorAuthenticatorSignInAsync(System.String,System.String)~System.Threading.Tasks.Task{HCL.CS.Domain.Models.ConstructSignInResponseModel}")]
[assembly:
    SuppressMessage("Major Bug", "S2259:Null pointers should not be dereferenced", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:HCL.CS.Service.Implementation.UserManagementValidator.ValidateAsync(System.String,System.String,System.String,System.Boolean,HCL.CS.Domain.SystemSettings,HCL.CS.Service.Implementation.UserManagerWrapper{HCL.CS.Domain.Entities.Users},HCL.CS.DomainServices.Infra.IFrameworkResultService,HCL.CS.DomainServices.Infra.IResourceStringHandler,HCL.CS.DomainServices.Infra.ILoggerService)~System.Threading.Tasks.Task{HCL.CS.Domain.Models.ConstructSignInResponseModel}")]
[assembly:
    SuppressMessage("Major Bug", "S2259:Null pointers should not be dereferenced", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:HCL.CS.Service.Implementation.AuthenticationService.TwoFactorSignInAsync(System.String,System.String,HCL.CS.Domain.Enums.NotificationTypes)~System.Threading.Tasks.Task{HCL.CS.Domain.Models.ConstructSignInResponseModel}")]
[assembly:
    SuppressMessage("Major Bug", "S2259:Null pointers should not be dereferenced", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:HCL.CS.Service.Implementation.WinFormsAuthenticationService.TwoFactorAuthenticatorSignInAsync(System.String,System.String)~System.Threading.Tasks.Task{HCL.CS.Domain.Models.ConstructSignInResponseModel}")]
[assembly:
    SuppressMessage("Major Bug", "S2259:Null pointers should not be dereferenced", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:HCL.CS.Service.Implementation.WinFormsAuthenticationService.TwoFactorRecoveryCodeSignInAsync(System.String,System.String)~System.Threading.Tasks.Task{HCL.CS.Domain.Models.ConstructSignInResponseModel}")]
[assembly:
    SuppressMessage("Major Bug", "S2259:Null pointers should not be dereferenced", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:HCL.CS.Service.Implementation.WinFormsAuthenticationService.TwoFactorSignInAsync(System.String,System.String,HCL.CS.Domain.Enums.NotificationTypes)~System.Threading.Tasks.Task{HCL.CS.Domain.Models.ConstructSignInResponseModel}")]
[assembly:
    SuppressMessage("Major Bug", "S2259:Null pointers should not be dereferenced", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:HCL.CS.Service.Implementation.AuthenticationService.VerifyAuthenticatorSetupAsync(System.Guid,System.String)~System.Threading.Tasks.Task{HCL.CS.Domain.Models.AuthenticatorAppResponseModel}")]
[assembly:
    SuppressMessage("Major Code Smell", "S4457:Parameter validation in \"async\"/\"await\" methods should be wrapped",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:HCL.CS.Service.Implementation.UserManagerWrapper`1.VerifyTwoFactorTokenAsync(System.Guid,System.String)~System.Threading.Tasks.Task{System.Boolean}")]
[assembly:
    SuppressMessage("Major Code Smell", "S4457:Parameter validation in \"async\"/\"await\" methods should be wrapped",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:HCL.CS.Service.Implementation.WinFormsSignInWrapper`1.PasswordSignInAsync(`0,System.String,System.Boolean)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Identity.SignInResult}")]
[assembly:
    SuppressMessage("Major Code Smell", "S4457:Parameter validation in \"async\"/\"await\" methods should be wrapped",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:HCL.CS.Service.Implementation.WinFormsSignInWrapper`1.CheckPasswordSignInAsync(`0,System.String,System.Boolean)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Identity.SignInResult}")]
[assembly:
    SuppressMessage("Major Bug", "S2259:Null pointers should not be dereferenced", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:HCL.CS.Service.Implementation.UserInfoServices.ProcessUserInfoAsync(HCL.CS.Domain.Models.ValidatedUserInfoRequestModel)~System.Threading.Tasks.Task{System.Collections.Generic.Dictionary{System.String,System.Object}}")]
[assembly:
    SuppressMessage("Major Bug", "S2259:Null pointers should not be dereferenced", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:HCL.CS.Service.Implementation.IdentityResourceService.AddIdentityResourceAsync(HCL.CS.Domain.Models.IdentityResourcesModel)~System.Threading.Tasks.Task{HCL.CS.Domain.FrameworkResult}")]
[assembly:
    SuppressMessage("Major Bug", "S2259:Null pointers should not be dereferenced", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:HCL.CS.Service.Implementation.RoleService.CreateRoleAsync(HCL.CS.Domain.Models.RoleModel)~System.Threading.Tasks.Task{HCL.CS.Domain.FrameworkResult}")]
[assembly:
    SuppressMessage("Major Bug", "S2259:Null pointers should not be dereferenced", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:HCL.CS.Service.Implementation.RoleService.UpdateRoleAsync(HCL.CS.Domain.Models.RoleModel)~System.Threading.Tasks.Task{HCL.CS.Domain.FrameworkResult}")]
[assembly:
    SuppressMessage("Major Bug", "S2259:Null pointers should not be dereferenced", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:HCL.CS.Service.Implementation.RoleService.RemoveRoleAsync(HCL.CS.Domain.Models.RoleModel)~System.Threading.Tasks.Task{HCL.CS.Domain.FrameworkResult}")]
[assembly:
    SuppressMessage("Major Bug", "S2259:Null pointers should not be dereferenced", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:HCL.CS.Service.Implementation.RoleService.UpdateUserRoleAsync(HCL.CS.Domain.Models.UserRoleModel,System.Guid)~System.Threading.Tasks.Task{HCL.CS.Domain.FrameworkResult}")]
[assembly:
    SuppressMessage("Major Bug", "S2259:Null pointers should not be dereferenced", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:HCL.CS.Service.Implementation.RoleService.AddRoleClaimAsync(HCL.CS.Domain.Models.RoleClaimModel)~System.Threading.Tasks.Task{HCL.CS.Domain.FrameworkResult}")]
[assembly:
    SuppressMessage("Major Bug", "S2259:Null pointers should not be dereferenced", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:HCL.CS.Service.Implementation.RoleService.RemoveRoleClaimAsync(HCL.CS.Domain.Models.RoleClaimModel)~System.Threading.Tasks.Task{HCL.CS.Domain.FrameworkResult}")]
[assembly:
    SuppressMessage("Major Bug", "S2259:Null pointers should not be dereferenced", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:HCL.CS.Service.Implementation.RoleService.DeleteUserRole(HCL.CS.Domain.Models.UserRoleModel,System.Boolean)~System.Threading.Tasks.Task{HCL.CS.Domain.FrameworkResult}")]
[assembly:
    SuppressMessage("Major Bug", "S2259:Null pointers should not be dereferenced", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:HCL.CS.Service.Implementation.SignInManagerWrapper`1.GetAuthenticationAsync~System.Threading.Tasks.Task{HCL.CS.Domain.Models.AuthenticationPropertiesModel}")]
[assembly:
    SuppressMessage("Major Code Smell",
        "S3928:Parameter names used into ArgumentException constructors should match an existing one ",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:HCL.CS.Service.Implementation.TokenExtension.GetAsymmetricCertificateHash(System.Collections.Generic.Dictionary{System.String,HCL.CS.Domain.Models.AsymmetricKeyInfoModel},System.String)~System.String")]
[assembly:
    SuppressMessage("Major Bug", "S2259:Null pointers should not be dereferenced", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:HCL.CS.Service.Implementation.AuthorizationService.SaveReturnUrlAsync(HCL.CS.Domain.Models.ValidatedAuthorizeRequestModel)~System.Threading.Tasks.Task{System.Guid}")]
[assembly:
    SuppressMessage("Major Bug", "S2259:Null pointers should not be dereferenced", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:HCL.CS.Service.Implementation.AuthenticationService.RopValidateCredentialsAsync(HCL.CS.Domain.Models.RopValidationModel)~System.Threading.Tasks.Task{HCL.CS.Domain.Models.RopValidationModel}")]
[assembly:
    SuppressMessage("Major Bug", "S2259:Null pointers should not be dereferenced", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:HCL.CS.Service.Implementation.UserAccountService.GetUsersForClaimAsync(System.Security.Claims.Claim)~System.Threading.Tasks.Task{System.Collections.Generic.IList{HCL.CS.Domain.Models.UserModel}}")]
