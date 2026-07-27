/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Cryptography;
using FluentAssertions;
using HCL.CS.Domain;
using HCL.CS.Domain.Constants;
using HCL.CS.Domain.Constants.Endpoint;
using HCL.CS.Domain.Entities.Api;
using HCL.CS.Domain.Enums;
using HCL.CS.Domain.Models.Api;
using HCL.CS.DomainServices.Infra;
using HCL.CS.DomainServices.Wrappers;
using HCL.CS.Infrastructure.Data;
using HCL.CS.Service.Implementation.Endpoint.Extensions;
using HCL.CS.Service.Interfaces.Interfaces.Api;
using IntegrationTests.Endpoint.Setup;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

namespace IntegrationTests.Endpoint.FlowTests;

public sealed class Phase2BLocalAuthenticationTests : HclCsFakeSetup
{
    private const string RedirectUri = "https://localhost:3000/auth/callback";
    private readonly RecordingEmailService emailService = new();

    public Phase2BLocalAuthenticationTests()
    {
        OnPostConfigureServices += services =>
        {
            services.RemoveAll<IEmailService>();
            services.AddSingleton<IEmailService>(emailService);
            HCL.CS.Domain.GlobalConfiguration.IsEmailConfigurationValid = true;
        };
        Initialize();
    }

    [Fact]
    [Trait("Category", "Phase2B")]
    public async Task LocalRegistration_NormalizesAndCreatesUnconfirmedPasswordUser()
    {
        await using var scope = ServiceProvider.CreateAsyncScope();
        var accounts = scope.ServiceProvider.GetRequiredService<IUserAccountService>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManagerWrapper<Users>>();
        var request = await CreateRegistrationAsync(accounts, "LocalUserA", "  LOCAL.USERA@HCLTECH.COM  ");

        var result = await accounts.RegisterUserAsync(request);

        result.Status.Should().Be(ResultStatus.Succeeded);
        var user = await userManager.FindByNameAsync(request.UserName);
        user.Should().NotBeNull();
        user!.Email.Should().Be("local.usera@hcltech.com");
        user.NormalizedEmail.Should().Be("LOCAL.USERA@HCLTECH.COM");
        user.IdentityProviderType.Should().Be(IdentityProvider.Local);
        user.AuthenticationSource.Should().Be("LOCAL");
        user.DirectoryImmutableId.Should().BeNull();
        user.DirectoryLastValidatedAt.Should().BeNull();
        user.EmailConfirmed.Should().BeFalse();
        user.LockoutEnabled.Should().BeTrue();
        user.PasswordHash.Should().NotBe(request.Password);
        (await userManager.CheckPasswordAsync(user, request.Password)).Should().BeTrue();
    }

    [Fact]
    [Trait("Category", "Phase2B")]
    public async Task LocalRegistration_DuplicateEmailIsRejectedWithoutSecondIdentity()
    {
        await using var scope = ServiceProvider.CreateAsyncScope();
        var accounts = scope.ServiceProvider.GetRequiredService<IUserAccountService>();
        var first = await CreateRegistrationAsync(accounts, "DuplicateA", "duplicate@hcltech.com");
        var second = await CreateRegistrationAsync(accounts, "DuplicateB", "DUPLICATE@HCLTECH.COM");

        (await accounts.RegisterUserAsync(first)).Status.Should().Be(ResultStatus.Succeeded);
        var duplicate = await accounts.RegisterUserAsync(second);

        duplicate.Status.Should().Be(ResultStatus.Failed);
        duplicate.Errors.Single().Code.Should().Be("AUTH_LOCAL_USER_ALREADY_EXISTS");
        await using var verificationScope = ServiceProvider.CreateAsyncScope();
        var database = verificationScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        (await database.Users.IgnoreQueryFilters()
                .CountAsync(user => user.NormalizedEmail == "DUPLICATE@HCLTECH.COM"))
            .Should().Be(1);
    }

    [Fact]
    [Trait("Category", "Phase2B")]
    public async Task LocalRegistration_LdapShadowEmailConflictIsRejected()
    {
        await using var scope = ServiceProvider.CreateAsyncScope();
        var accounts = scope.ServiceProvider.GetRequiredService<IUserAccountService>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManagerWrapper<Users>>();
        var ldapUser = new Users
        {
            Id = Guid.NewGuid(),
            UserName = "ldap-shadow",
            Email = "source.conflict@hcltech.com",
            FirstName = "Directory",
            LastName = "User",
            IdentityProviderType = IdentityProvider.Ldap,
            AuthenticationSource = "LDAP",
            DirectoryImmutableId = Guid.NewGuid().ToString("D"),
            PasswordHash = "!LDAP_EXTERNAL_CREDENTIAL_ONLY!",
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            LockoutEnabled = true,
            CreatedBy = "Test",
            CreatedOn = DateTime.UtcNow,
            IsDeleted = false
        };
        (await userManager.CreateAsync(ldapUser)).Succeeded.Should().BeTrue();
        await scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().SaveChangesAsync();
        var request = await CreateRegistrationAsync(accounts, "ConflictA", ldapUser.Email);

        var result = await accounts.RegisterUserAsync(request);

        result.Status.Should().Be(ResultStatus.Failed);
        result.Errors.Single().Code.Should().Be("AUTH_IDENTITY_SOURCE_CONFLICT");
    }

    [Fact]
    [Trait("Category", "Phase2B")]
    public async Task LocalEmailConfirmation_IsRequiredAndReplayIsRejected()
    {
        await using var scope = ServiceProvider.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext =
            new DefaultHttpContext { RequestServices = scope.ServiceProvider };
        var accounts = scope.ServiceProvider.GetRequiredService<IUserAccountService>();
        var authentication = scope.ServiceProvider.GetRequiredService<IAuthenticationService>();
        var request = await CreateRegistrationAsync(accounts, "ConfirmUser", "confirm.user@hcltech.com");
        (await accounts.RegisterUserAsync(request)).Status.Should().Be(ResultStatus.Succeeded);

        var beforeConfirmation = await authentication.PasswordSignInAsync(request.UserName, request.Password);
        beforeConfirmation.Succeeded.Should().BeFalse();
        beforeConfirmation.ErrorCode.Should().Be("AUTH_LOCAL_EMAIL_CONFIRMATION_REQUIRED");

        (await accounts.GenerateEmailConfirmationTokenAsync(request.UserName))
            .Status.Should().Be(ResultStatus.Succeeded);
        emailService.LastToken.Should().NotBeNullOrWhiteSpace();
        var confirmation = await accounts.VerifyEmailConfirmationTokenAsync(
            request.UserName,
            emailService.LastToken!);
        confirmation.Status.Should().Be(
            ResultStatus.Succeeded,
            string.Join(", ", (confirmation.Errors ?? []).Select(error => $"{error.Code}: {error.Description}")));
        (await accounts.VerifyEmailConfirmationTokenAsync(request.UserName, emailService.LastToken!))
            .Errors.Single().Code.Should().Be("AUTH_LOCAL_EMAIL_CONFIRMATION_INVALID");

        var afterConfirmation = await authentication.PasswordSignInAsync(request.UserName, request.Password);
        afterConfirmation.Succeeded.Should().BeTrue();
    }

    [Fact]
    [Trait("Category", "Phase2B")]
    public async Task LocalLogin_WrongPasswordTriggersConfiguredLockout()
    {
        await using var scope = ServiceProvider.CreateAsyncScope();
        var accounts = scope.ServiceProvider.GetRequiredService<IUserAccountService>();
        var authentication = scope.ServiceProvider.GetRequiredService<IAuthenticationService>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManagerWrapper<Users>>();
        var request = await CreateRegistrationAsync(accounts, "LockUserA", "lock.user@hcltech.com");
        (await accounts.RegisterUserAsync(request)).Status.Should().Be(ResultStatus.Succeeded);
        (await accounts.GenerateEmailConfirmationTokenAsync(request.UserName))
            .Status.Should().Be(ResultStatus.Succeeded);
        var confirmation = await accounts.VerifyEmailConfirmationTokenAsync(
            request.UserName,
            emailService.LastToken!);
        confirmation.Status.Should().Be(
            ResultStatus.Succeeded,
            string.Join(", ", (confirmation.Errors ?? []).Select(error => $"{error.Code}: {error.Description}")));

        for (var attempt = 0; attempt < 3; attempt++)
            (await authentication.PasswordSignInAsync(request.UserName, "WrongPassword!1"))
                .Succeeded.Should().BeFalse();

        var user = await userManager.FindByNameAsync(request.UserName);
        user!.LockoutEnd.Should().BeAfter(DateTimeOffset.UtcNow);
        (await authentication.PasswordSignInAsync(request.UserName, request.Password))
            .Succeeded.Should().BeFalse();
    }

    [Fact]
    [Trait("Category", "Phase2B")]
    public async Task ConcurrentLocalRegistration_CreatesOneNormalizedIdentity()
    {
        const string email = "concurrent.user@hcltech.com";
        var first = RegisterInIndependentScopeAsync("ConcurrentA", email);
        var second = RegisterInIndependentScopeAsync("ConcurrentB", email);

        var results = await Task.WhenAll(first, second);

        results.Count(result => result.Status == ResultStatus.Succeeded).Should().Be(1);
        await using var scope = ServiceProvider.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        (await database.Users.IgnoreQueryFilters()
                .CountAsync(user => user.NormalizedEmail == "CONCURRENT.USER@HCLTECH.COM"))
            .Should().Be(1);
    }

    [Fact]
    [Trait("Category", "Phase2B")]
    public async Task AuthenticationAvailability_LocalModeExposesOnlySafeHint()
    {
        await using var scope = ServiceProvider.CreateAsyncScope();
        var authentication = scope.ServiceProvider.GetRequiredService<IAuthenticationService>();

        var availability = await authentication.GetAuthenticationAvailabilityAsync();

        availability.AuthenticationMode.Should().Be("LOCAL");
        availability.LocalRegistrationAvailable.Should().BeTrue();
        availability.AllowedEmailDomainHint.Should().Be("@hcltech.com");
    }

    [Fact]
    [Trait("Category", "Phase2B")]
    public async Task ConfirmedLocalUser_CompletesSbomAuthorizationCodePkceAndRefreshWithoutDirectoryClaims()
    {
        UserModel request;
        Guid userId;
        await using (var scope = ServiceProvider.CreateAsyncScope())
        {
            var accounts = scope.ServiceProvider.GetRequiredService<IUserAccountService>();
            request = await CreateRegistrationAsync(accounts, "LocalOidcA", "local.oidc@hcltech.com");
            (await accounts.RegisterUserAsync(request)).Status.Should().Be(ResultStatus.Succeeded);
            (await accounts.GenerateEmailConfirmationTokenAsync(request.UserName))
                .Status.Should().Be(ResultStatus.Succeeded);
            (await accounts.VerifyEmailConfirmationTokenAsync(request.UserName, emailService.LastToken!))
                .Status.Should().Be(ResultStatus.Succeeded);
            var userManager = scope.ServiceProvider.GetRequiredService<UserManagerWrapper<Users>>();
            var localUser = (await userManager.FindByNameAsync(request.UserName))!;
            userId = localUser.Id;
            (await userManager.GetRolesAsync(localUser)).Should().BeEmpty();
        }

        await LoginAsync(request);
        var verifier = GenerateCodeVerifier();
        var nonce = Guid.NewGuid().ToString("N");
        FrontChannelClient.AllowAutoRedirect = false;
        var authorizeRequest = CreateAuthorizeRequestUrl(
            SbomIdentityContract.ClientId,
            "code",
            string.Join(' ', SbomIdentityContract.RequiredScopes),
            RedirectUri,
            state: Guid.NewGuid().ToString("N"),
            nonce: nonce,
            prompt: "none",
            maxAge: "60",
            responseMode: "query",
            codeChallenge: verifier.GenerateCodeChallenge(),
            codeChallengeMethod: OpenIdConstants.CodeChallengeMethods.Sha256);

        var authorizeResponse = await FrontChannelClient.GetAsync(authorizeRequest);
        authorizeResponse.StatusCode.Should().Be(HttpStatusCode.Found);
        var authorization = authorizeResponse.Headers.Location!.ToString().ParseQueryString();
        authorization.Code.Should().NotBeNullOrWhiteSpace();

        var tokenResponse = await BackChannelClient.PostAsync(
            TokenEndpoint,
            new FormUrlEncodedContent(CreateTokenRequest(
                SbomIdentityContract.ClientId,
                code: authorization.Code,
                redirectUri: RedirectUri,
                grantType: OpenIdConstants.GrantTypes.AuthorizationCode,
                codeVerifier: verifier)));
        var tokens = await tokenResponse.ParseTokenResponseResult();
        tokenResponse.StatusCode.Should().Be(
            HttpStatusCode.OK,
            await tokenResponse.Content.ReadAsStringAsync());
        var accessToken = new JwtSecurityTokenHandler().ReadJwtToken(tokens.access_token);
        accessToken.Issuer.Should().Be("security.hcl-cs.com");
        accessToken.Audiences.Should().Equal(SbomIdentityContract.ApiAudience);
        accessToken.Subject.Should().Be(userId.ToString());
        accessToken.Claims.Single(claim => claim.Type == "email").Value
            .Should().Be("local.oidc@hcltech.com");
        accessToken.Claims.Single(claim => claim.Type == "name").Value.Should().Be("Local User");
        accessToken.Claims.Single(claim => claim.Type == "preferred_username").Value
            .Should().Be(request.UserName);
        accessToken.Claims.Should().NotContain(claim =>
            claim.Type == "employee_id" ||
            claim.Type == "department" ||
            claim.Type == "tenant_id" ||
            claim.Type == "role");

        var refreshResponse = await BackChannelClient.PostAsync(
            TokenEndpoint,
            new FormUrlEncodedContent(CreateTokenRequest(
                SbomIdentityContract.ClientId,
                grantType: OpenIdConstants.GrantTypes.RefreshToken,
                refreshToken: tokens.refresh_token)));
        refreshResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        (await refreshResponse.ParseTokenResponseResult()).access_token.Should().NotBeNullOrWhiteSpace();
    }

    private async Task<FrameworkResult> RegisterInIndependentScopeAsync(string username, string email)
    {
        await using var scope = ServiceProvider.CreateAsyncScope();
        var accounts = scope.ServiceProvider.GetRequiredService<IUserAccountService>();
        var request = await CreateRegistrationAsync(accounts, username, email);
        return await accounts.RegisterUserAsync(request);
    }

    private static string GenerateCodeVerifier()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    private static async Task<UserModel> CreateRegistrationAsync(
        IUserAccountService accounts,
        string username,
        string email)
    {
        var questions = await accounts.GetAllSecurityQuestionsAsync();
        return new UserModel
        {
            UserName = username,
            Email = email,
            PhoneNumber = "+12055550199",
            Password = "LocalUser@12345",
            FirstName = "Local",
            LastName = "User",
            DateOfBirth = new DateTime(1990, 1, 1),
            CreatedBy = "self-registration",
            ModifiedBy = "self-registration",
            IdentityProviderType = IdentityProvider.Ldap,
            AuthenticationSource = "LDAP",
            EmailConfirmed = true,
            LockoutEnabled = false,
            UserSecurityQuestion =
            [
                new UserSecurityQuestionModel
                {
                    SecurityQuestionId = questions.First().Id,
                    Answer = "safe answer",
                    CreatedBy = "self-registration"
                }
            ],
            UserClaims = []
        };
    }

    private sealed class RecordingEmailService : IEmailService
    {
        public string? LastToken { get; private set; }

        public Task<FrameworkResult> SendEmailAsync(NotificationInfoModel message)
        {
            LastToken = message.Parameters.TryGetValue("{TOKEN}", out var token) ? token : null;
            return Task.FromResult(new FrameworkResult { Status = ResultStatus.Succeeded });
        }
    }
}
