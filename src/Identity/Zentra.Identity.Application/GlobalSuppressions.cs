// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly:
    SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private",
        Justification = "<Pending>", Scope = "member",
        Target = "~F:Zentra.Service.Implementation.AuthorizationCodeBase.LoggerService")]
[assembly:
    SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private",
        Justification = "<Pending>", Scope = "member",
        Target = "~F:Zentra.Service.Implementation.AuthorizationCodeBase.AuthorizationService")]
[assembly:
    SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private",
        Justification = "<Pending>", Scope = "member",
        Target = "~F:Zentra.Service.Implementation.AuthorizationCodeBase.CsSignInManager")]
[assembly:
    SuppressMessage("Major Bug", "S2259:Null pointers should not be dereferenced", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:Zentra.Service.Implementation.UserAccountService.RegisterUserAsync(Zentra.Domain.Models.UserModel)~System.Threading.Tasks.Task{Zentra.Domain.FrameworkResult}")]
[assembly:
    SuppressMessage("Major Bug", "S2259:Null pointers should not be dereferenced", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:Zentra.Service.Implementation.UserAccountService.DeleteAccountAsync(Zentra.Domain.Models.UserModel)~System.Threading.Tasks.Task{Zentra.Domain.FrameworkResult}")]
[assembly:
    SuppressMessage("Minor Code Smell", "S1075:URIs should not be hardcoded", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:Zentra.Service.Implementation.AuthenticationService.GenerateQrCodeUri(System.String,System.String,System.String,System.Text.Encodings.Web.UrlEncoder)~System.String")]
[assembly:
    SuppressMessage("Major Bug", "S2259:Null pointers should not be dereferenced", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:Zentra.Service.Implementation.AuthenticationService.TwoFactorRecoveryCodeSignInAsync(System.String,System.String)~System.Threading.Tasks.Task{Zentra.Domain.Models.ConstructSignInResponseModel}")]
[assembly:
    SuppressMessage("Major Bug", "S2259:Null pointers should not be dereferenced", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:Zentra.Service.Implementation.AuthenticationService.TwoFactorAuthenticatorSignInAsync(System.String,System.String)~System.Threading.Tasks.Task{Zentra.Domain.Models.ConstructSignInResponseModel}")]
[assembly:
    SuppressMessage("Major Bug", "S2259:Null pointers should not be dereferenced", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:Zentra.Service.Implementation.UserManagementValidator.ValidateAsync(System.String,System.String,System.String,System.Boolean,Zentra.Domain.SystemSettings,Zentra.Service.Implementation.UserManagerWrapper{Zentra.Domain.Entities.Users},Zentra.DomainServices.Infra.IFrameworkResultService,Zentra.DomainServices.Infra.IResourceStringHandler,Zentra.DomainServices.Infra.ILoggerService)~System.Threading.Tasks.Task{Zentra.Domain.Models.ConstructSignInResponseModel}")]
[assembly:
    SuppressMessage("Major Bug", "S2259:Null pointers should not be dereferenced", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:Zentra.Service.Implementation.AuthenticationService.TwoFactorSignInAsync(System.String,System.String,Zentra.Domain.Enums.NotificationTypes)~System.Threading.Tasks.Task{Zentra.Domain.Models.ConstructSignInResponseModel}")]
[assembly:
    SuppressMessage("Major Bug", "S2259:Null pointers should not be dereferenced", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:Zentra.Service.Implementation.WinFormsAuthenticationService.TwoFactorAuthenticatorSignInAsync(System.String,System.String)~System.Threading.Tasks.Task{Zentra.Domain.Models.ConstructSignInResponseModel}")]
[assembly:
    SuppressMessage("Major Bug", "S2259:Null pointers should not be dereferenced", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:Zentra.Service.Implementation.WinFormsAuthenticationService.TwoFactorRecoveryCodeSignInAsync(System.String,System.String)~System.Threading.Tasks.Task{Zentra.Domain.Models.ConstructSignInResponseModel}")]
[assembly:
    SuppressMessage("Major Bug", "S2259:Null pointers should not be dereferenced", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:Zentra.Service.Implementation.WinFormsAuthenticationService.TwoFactorSignInAsync(System.String,System.String,Zentra.Domain.Enums.NotificationTypes)~System.Threading.Tasks.Task{Zentra.Domain.Models.ConstructSignInResponseModel}")]
[assembly:
    SuppressMessage("Major Bug", "S2259:Null pointers should not be dereferenced", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:Zentra.Service.Implementation.AuthenticationService.VerifyAuthenticatorSetupAsync(System.Guid,System.String)~System.Threading.Tasks.Task{Zentra.Domain.Models.AuthenticatorAppResponseModel}")]
[assembly:
    SuppressMessage("Major Code Smell", "S4457:Parameter validation in \"async\"/\"await\" methods should be wrapped",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:Zentra.Service.Implementation.UserManagerWrapper`1.VerifyTwoFactorTokenAsync(System.Guid,System.String)~System.Threading.Tasks.Task{System.Boolean}")]
[assembly:
    SuppressMessage("Major Code Smell", "S4457:Parameter validation in \"async\"/\"await\" methods should be wrapped",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:Zentra.Service.Implementation.WinFormsSignInWrapper`1.PasswordSignInAsync(`0,System.String,System.Boolean)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Identity.SignInResult}")]
[assembly:
    SuppressMessage("Major Code Smell", "S4457:Parameter validation in \"async\"/\"await\" methods should be wrapped",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:Zentra.Service.Implementation.WinFormsSignInWrapper`1.CheckPasswordSignInAsync(`0,System.String,System.Boolean)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Identity.SignInResult}")]
[assembly:
    SuppressMessage("Major Bug", "S2259:Null pointers should not be dereferenced", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:Zentra.Service.Implementation.UserInfoServices.ProcessUserInfoAsync(Zentra.Domain.Models.ValidatedUserInfoRequestModel)~System.Threading.Tasks.Task{System.Collections.Generic.Dictionary{System.String,System.Object}}")]
[assembly:
    SuppressMessage("Major Bug", "S2259:Null pointers should not be dereferenced", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:Zentra.Service.Implementation.IdentityResourceService.AddIdentityResourceAsync(Zentra.Domain.Models.IdentityResourcesModel)~System.Threading.Tasks.Task{Zentra.Domain.FrameworkResult}")]
[assembly:
    SuppressMessage("Major Bug", "S2259:Null pointers should not be dereferenced", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:Zentra.Service.Implementation.RoleService.CreateRoleAsync(Zentra.Domain.Models.RoleModel)~System.Threading.Tasks.Task{Zentra.Domain.FrameworkResult}")]
[assembly:
    SuppressMessage("Major Bug", "S2259:Null pointers should not be dereferenced", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:Zentra.Service.Implementation.RoleService.UpdateRoleAsync(Zentra.Domain.Models.RoleModel)~System.Threading.Tasks.Task{Zentra.Domain.FrameworkResult}")]
[assembly:
    SuppressMessage("Major Bug", "S2259:Null pointers should not be dereferenced", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:Zentra.Service.Implementation.RoleService.RemoveRoleAsync(Zentra.Domain.Models.RoleModel)~System.Threading.Tasks.Task{Zentra.Domain.FrameworkResult}")]
[assembly:
    SuppressMessage("Major Bug", "S2259:Null pointers should not be dereferenced", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:Zentra.Service.Implementation.RoleService.UpdateUserRoleAsync(Zentra.Domain.Models.UserRoleModel,System.Guid)~System.Threading.Tasks.Task{Zentra.Domain.FrameworkResult}")]
[assembly:
    SuppressMessage("Major Bug", "S2259:Null pointers should not be dereferenced", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:Zentra.Service.Implementation.RoleService.AddRoleClaimAsync(Zentra.Domain.Models.RoleClaimModel)~System.Threading.Tasks.Task{Zentra.Domain.FrameworkResult}")]
[assembly:
    SuppressMessage("Major Bug", "S2259:Null pointers should not be dereferenced", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:Zentra.Service.Implementation.RoleService.RemoveRoleClaimAsync(Zentra.Domain.Models.RoleClaimModel)~System.Threading.Tasks.Task{Zentra.Domain.FrameworkResult}")]
[assembly:
    SuppressMessage("Major Bug", "S2259:Null pointers should not be dereferenced", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:Zentra.Service.Implementation.RoleService.DeleteUserRole(Zentra.Domain.Models.UserRoleModel,System.Boolean)~System.Threading.Tasks.Task{Zentra.Domain.FrameworkResult}")]
[assembly:
    SuppressMessage("Major Bug", "S2259:Null pointers should not be dereferenced", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:Zentra.Service.Implementation.SignInManagerWrapper`1.GetAuthenticationAsync~System.Threading.Tasks.Task{Zentra.Domain.Models.AuthenticationPropertiesModel}")]
[assembly:
    SuppressMessage("Major Code Smell",
        "S3928:Parameter names used into ArgumentException constructors should match an existing one ",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:Zentra.Service.Implementation.TokenExtension.GetAsymmetricCertificateHash(System.Collections.Generic.Dictionary{System.String,Zentra.Domain.Models.AsymmetricKeyInfoModel},System.String)~System.String")]
[assembly:
    SuppressMessage("Major Bug", "S2259:Null pointers should not be dereferenced", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:Zentra.Service.Implementation.AuthorizationService.SaveReturnUrlAsync(Zentra.Domain.Models.ValidatedAuthorizeRequestModel)~System.Threading.Tasks.Task{System.Guid}")]
[assembly:
    SuppressMessage("Major Bug", "S2259:Null pointers should not be dereferenced", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:Zentra.Service.Implementation.AuthenticationService.RopValidateCredentialsAsync(Zentra.Domain.Models.RopValidationModel)~System.Threading.Tasks.Task{Zentra.Domain.Models.RopValidationModel}")]
[assembly:
    SuppressMessage("Major Bug", "S2259:Null pointers should not be dereferenced", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:Zentra.Service.Implementation.UserAccountService.GetUsersForClaimAsync(System.Security.Claims.Claim)~System.Threading.Tasks.Task{System.Collections.Generic.IList{Zentra.Domain.Models.UserModel}}")]
