using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using FluentAssertions;
using HCL.CS.Domain.Constants;
using HCL.CS.Domain.Constants.Api;
using HCL.CS.Domain.Constants.Endpoint;
using HCL.CS.Domain.Entities.Api;
using HCL.CS.Domain.Enums;
using HCL.CS.Domain.Models.Api;
using HCL.CS.Infrastructure.Data;
using HCL.CS.Service.Interfaces.Interfaces.Api;
using IntegrationTests.ApiDomainModel;
using IntegrationTests.Endpoint.Setup;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using Xunit;

namespace IntegrationTests.Endpoint.FlowTests;

public sealed class Phase2SbomIdentityContractTests : HclCsFakeSetup
{
    private const string RedirectUri = "https://localhost:3000/auth/callback";
    private readonly MutableLdapAccountValidationService ldapAccountValidator = new();

    public Phase2SbomIdentityContractTests()
    {
        OnPostConfigureServices += services =>
        {
            services.RemoveAll<ILdapAccountValidationService>();
            services.AddSingleton<ILdapAccountValidationService>(ldapAccountValidator);
        };
        Initialize();
    }

    [Fact]
    [Trait("Category", "Phase2")]
    public async Task SbomRegistration_IsExactAndIdempotent()
    {
        await using var scope = ServiceProvider.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var clients = await db.Clients
            .Include(client => client.RedirectUris)
            .Include(client => client.PostLogoutRedirectUris)
            .Where(client => client.ClientId == SbomIdentityContract.ClientId && !client.IsDeleted)
            .ToListAsync();
        clients.Should().ContainSingle();

        var client = clients.Single();
        client.RequirePkce.Should().BeTrue();
        client.IsPkceTextPlain.Should().BeFalse();
        client.RequireClientSecret.Should().BeFalse();
        client.ClientSecret.Should().BeNull();
        client.SupportedGrantTypes.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Should().BeEquivalentTo("authorization_code", "refresh_token");
        client.AllowedScopes.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Should().BeEquivalentTo(SbomIdentityContract.RequiredScopes);
        client.PreferredAudience.Should().Be(SbomIdentityContract.ApiAudience);
        client.AllowedSigningAlgorithm.Should().Be(OpenIdConstants.Algorithms.RsaSha256);
        client.AuthorizationCodeExpiration.Should().Be(300);
        client.AccessTokenExpiration.Should().Be(3600);
        client.RefreshTokenExpiration.Should().Be(86400);
        client.RedirectUris.Select(uri => uri.RedirectUri).Should().Equal(RedirectUri);
        client.PostLogoutRedirectUris.Select(uri => uri.PostLogoutRedirectUri)
            .Should().Equal("https://localhost:3000");

        var resources = await db.ApiResources
            .Where(resource => resource.Name == SbomIdentityContract.ApiAudience && !resource.IsDeleted)
            .ToListAsync();
        resources.Should().ContainSingle();
        (await db.ApiResourceClaims
                .Where(claim => claim.ApiResourceId == resources.Single().Id && !claim.IsDeleted)
                .Select(claim => claim.Type)
                .ToListAsync())
            .Should().BeEquivalentTo(SbomIdentityContract.RequiredUserClaims);
    }

    [Fact]
    [Trait("Category", "Phase2")]
    public async Task AuthorizationCodeWithS256_IssuesValidSbomIdentityContract()
    {
        var (token, nonce) = await IssueTokensAsync();
        var accessToken = new JwtSecurityTokenHandler().ReadJwtToken(token.access_token);
        var identityToken = new JwtSecurityTokenHandler().ReadJwtToken(token.id_token);

        accessToken.Issuer.Should().Be("security.hcl-cs.com");
        accessToken.Audiences.Should().Equal(SbomIdentityContract.ApiAudience);
        accessToken.Subject.Should().NotBeNullOrWhiteSpace();
        accessToken.Claims.Single(claim => claim.Type == "email").Value
            .Should().Be("checktest@hcltech.com");
        accessToken.Claims.Single(claim => claim.Type == "name").Value.Should().Be("Check Test");
        accessToken.Claims.Single(claim => claim.Type == "preferred_username").Value
            .Should().Be("checktest@hcltech.com");
        accessToken.Claims.Single(claim => claim.Type == "employee_id").Value.Should().Be("EMP-CHECKTEST");
        accessToken.Claims.Single(claim => claim.Type == "department").Value.Should().Be("Engineering");
        accessToken.Claims.Should().NotContain(claim =>
            claim.Type == "tenant_id" ||
            claim.Type == "sbom_role" ||
            claim.Type == "ldap_password" ||
            claim.Type == "userAccountControl");

        identityToken.Audiences.Should().Equal(SbomIdentityContract.ClientId);
        identityToken.Subject.Should().Be(accessToken.Subject);
        identityToken.Claims.Single(claim => claim.Type == "nonce").Value.Should().Be(nonce);
        identityToken.Claims.Should().Contain(claim => claim.Type == "email");
        identityToken.Claims.Should().Contain(claim => claim.Type == "name");
        identityToken.Claims.Should().Contain(claim => claim.Type == "preferred_username");

        var validated = await ValidateAgainstJwksAsync(token.access_token);
        validated.Should().NotBeNull();

        await using var scope = ServiceProvider.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var auditNames = await db.AuditTrail
            .Where(audit => audit.TableName == "Authentication")
            .Select(audit => audit.ActionName)
            .ToListAsync();
        auditNames.Should().Contain(SecurityAuditEventTypes.AuthenticationSucceeded);
        auditNames.Should().Contain(SecurityAuditEventTypes.LocalAuthenticationSucceeded);
        auditNames.Should().Contain(SecurityAuditEventTypes.AuthorizationCodeIssued);
        auditNames.Should().Contain(SecurityAuditEventTypes.TokenIssued);
        (await db.AuditTrail.Where(audit => audit.TableName == "Authentication")
                .Select(audit => audit.NewValue).ToListAsync())
            .Should().OnlyContain(value =>
                !value.Contains("Test@123456789", StringComparison.Ordinal) &&
                !value.Contains("BindPassword", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    [Trait("Category", "Phase2")]
    public async Task PlainPkce_IsRejectedForSbomClient()
    {
        await LoginAsync(User);
        FrontChannelClient.AllowAutoRedirect = false;
        var request = CreateAuthorizeRequestUrl(
            SbomIdentityContract.ClientId,
            "code",
            string.Join(' ', SbomIdentityContract.RequiredScopes),
            RedirectUri,
            nonce: Guid.NewGuid().ToString("N"),
            prompt: "none",
            maxAge: "60",
            responseMode: "query",
            codeChallenge: GenerateCodeVerifier(),
            codeChallengeMethod: "plain");

        var response = await FrontChannelClient.GetAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Found);
        var error = response.Headers.Location?.ToString().ParseErrorQueryStringAsync();
        error.Should().NotBeNull();
        error!.Result.ErrorCode.Should().Be(OpenIdConstants.Errors.InvalidRequest);
        response.Headers.Location!.ToString().ParseQueryString().Code.Should().BeNullOrWhiteSpace();
    }

    [Fact]
    [Trait("Category", "Phase2")]
    public async Task FailedLogin_PersistsGenericSecretFreeAudit()
    {
        const string rejectedPassword = "DoNotPersist-Phase2!";
        var response = await FrontChannelClient.PostAsync(
            BaseUrl + ApiRoutePathConstants.PasswordSignIn,
            new StringContent(
                $"{{\"user_name\":\"checktest\",\"password\":\"{rejectedPassword}\"}}",
                Encoding.UTF8,
                "application/json"));
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await using var scope = ServiceProvider.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var audits = await db.AuditTrail
            .Where(audit => audit.TableName == "Authentication")
            .ToListAsync();
        audits.Select(audit => audit.ActionName)
            .Should().Contain(SecurityAuditEventTypes.AuthenticationFailed);
        audits.Select(audit => audit.ActionName)
            .Should().Contain(SecurityAuditEventTypes.LocalAuthenticationFailed);
        audits.Select(audit => audit.NewValue)
            .Should().OnlyContain(value => !value.Contains(rejectedPassword, StringComparison.Ordinal));
    }

    [Fact]
    [Trait("Category", "Phase2")]
    public async Task ActiveLdapUser_RefreshesAndPersistsAudit()
    {
        var (token, _) = await IssueTokensAsync();
        await ConvertSeedUserToLdapAsync();
        ldapAccountValidator.Result = LdapAccountValidationResult.Active();

        var response = await RefreshAsync(token.refresh_token);
        var payload = await response.ParseTokenResponseResult();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        payload.access_token.Should().NotBeNullOrWhiteSpace();
        payload.refresh_token.Should().NotBeNullOrWhiteSpace();
        ldapAccountValidator.LastImmutableId.Should().NotBeNullOrWhiteSpace();
        (await GetSecurityAuditNamesAsync()).Should().Contain(SecurityAuditEventTypes.TokenRefreshSucceeded);
    }

    [Fact]
    [Trait("Category", "Phase2")]
    public async Task InactiveLdapUser_RefreshIsRejectedAndSessionFamilyIsRevoked()
    {
        var (token, _) = await IssueTokensAsync();
        await ConvertSeedUserToLdapAsync();
        ldapAccountValidator.Result = LdapAccountValidationResult.Failed(
            LdapFailureCodes.AccountDisabled,
            LdapAccountState.Disabled);

        var response = await RefreshAsync(token.refresh_token);
        var payload = await response.ParseTokenErrorResponse();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        payload.ErrorCode.Should().Be(OpenIdConstants.Errors.InvalidGrant);
        var auditNames = await GetSecurityAuditNamesAsync();
        auditNames.Should().Contain(SecurityAuditEventTypes.LdapAccountRevalidationFailed);
        auditNames.Should().Contain(SecurityAuditEventTypes.TokenRefreshRejected);

        await using var scope = ServiceProvider.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        (await db.SecurityTokens
                .Where(item => item.TokenType == OpenIdConstants.TokenType.RefreshToken)
                .ToListAsync())
            .Should().OnlyContain(item => item.ConsumedAt.HasValue && item.TokenReuseDetected);
    }

    [Fact]
    [Trait("Category", "Phase2")]
    public async Task LocalUser_RefreshDoesNotContactLdap()
    {
        var (token, _) = await IssueTokensAsync();

        var response = await RefreshAsync(token.refresh_token);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        ldapAccountValidator.CallCount.Should().Be(0);
    }

    [Fact]
    [Trait("Category", "Phase2")]
    public async Task UserInfo_UsesGrantedProtocolClaimsWithoutHclRoles()
    {
        var (token, _) = await IssueTokensAsync();
        using var request = new HttpRequestMessage(HttpMethod.Get, UserInfoEndpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.access_token);

        var response = await BackChannelClient.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        var payload = JObject.Parse(content);
        payload.Value<string>("sub").Should().NotBeNullOrWhiteSpace();
        payload.Value<string>("email").Should().Be("checktest@hcltech.com");
        payload.Value<string>("name").Should().Be("Check Test");
        payload.Value<string>("preferred_username").Should().Be("checktest@hcltech.com");
        payload.Value<string>("employee_id").Should().Be("EMP-CHECKTEST");
        payload.Value<string>("department").Should().Be("Engineering");
        payload["role"].Should().BeNull();
        payload["tenant_id"].Should().BeNull();
    }

    [Fact]
    [Trait("Category", "Phase2")]
    public async Task Logout_PersistsStructuredSessionAudit()
    {
        await IssueTokensAsync();
        var response = await FrontChannelClient.PostAsync(
            BaseUrl + ApiRoutePathConstants.SignOut,
            new StringContent("{}", Encoding.UTF8, "application/json"));
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        (await GetSecurityAuditNamesAsync()).Should().Contain(SecurityAuditEventTypes.SessionLogout);
    }

    [Fact]
    [Trait("Category", "Phase2")]
    public async Task ProfileAndDirectoryChanges_DoNotChangeStableSubject()
    {
        var (before, _) = await IssueTokensAsync();
        var beforeSubject = new JwtSecurityTokenHandler().ReadJwtToken(before.access_token).Subject;

        await using (var scope = ServiceProvider.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var user = await db.Users.SingleAsync(item => item.UserName == "checktest");
            user.DirectoryImmutableId = Guid.NewGuid().ToString("D");
            user.Email = "changed@hcl-cs.local";
            user.NormalizedEmail = "CHANGED@HCL-CS.LOCAL";
            user.DisplayName = "Changed Display";
            user.Department = "Security";
            user.UserPrincipalName = "changed.upn@hcl-cs.local";
            await db.SaveChangesAsync();
        }

        var (after, _) = await IssueTokensAsync();
        var afterToken = new JwtSecurityTokenHandler().ReadJwtToken(after.access_token);

        afterToken.Subject.Should().Be(beforeSubject);
        afterToken.Claims.Single(claim => claim.Type == "email").Value.Should().Be("changed@hcl-cs.local");
        afterToken.Claims.Single(claim => claim.Type == "name").Value.Should().Be("Changed Display");
        afterToken.Claims.Single(claim => claim.Type == "preferred_username").Value
            .Should().Be("changed.upn@hcl-cs.local");
        afterToken.Claims.Single(claim => claim.Type == "department").Value.Should().Be("Security");
    }

    private async Task<(TokenResponseResultModel Token, string Nonce)> IssueTokensAsync()
    {
        await LoginAsync(User);
        var nonce = Guid.NewGuid().ToString("N");
        var verifier = GenerateCodeVerifier();
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

        var response = await BackChannelClient.PostAsync(
            TokenEndpoint,
            new FormUrlEncodedContent(CreateTokenRequest(
                SbomIdentityContract.ClientId,
                code: authorization.Code,
                redirectUri: RedirectUri,
                grantType: OpenIdConstants.GrantTypes.AuthorizationCode,
                codeVerifier: verifier)));
        var token = await response.ParseTokenResponseResult();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        token.access_token.Should().NotBeNullOrWhiteSpace();
        token.id_token.Should().NotBeNullOrWhiteSpace();
        token.refresh_token.Should().NotBeNullOrWhiteSpace();
        return (token, nonce);
    }

    private Task<HttpResponseMessage> RefreshAsync(string refreshToken)
    {
        return BackChannelClient.PostAsync(
            TokenEndpoint,
            new FormUrlEncodedContent(CreateTokenRequest(
                SbomIdentityContract.ClientId,
                grantType: OpenIdConstants.GrantTypes.RefreshToken,
                refreshToken: refreshToken)));
    }

    private async Task ConvertSeedUserToLdapAsync()
    {
        await using var scope = ServiceProvider.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var user = await db.Users.SingleAsync(item => item.UserName == "checktest");
        user.IdentityProviderType = HCL.CS.Domain.Enums.IdentityProvider.Ldap;
        user.AuthenticationSource = "LDAP";
        user.DirectoryImmutableId = Guid.NewGuid().ToString("D");
        await db.SaveChangesAsync();
    }

    private async Task<List<string>> GetSecurityAuditNamesAsync()
    {
        await using var scope = ServiceProvider.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        return await db.AuditTrail
            .Where(audit => audit.TableName == "Authentication")
            .Select(audit => audit.ActionName)
            .ToListAsync();
    }

    private async Task<SecurityToken?> ValidateAgainstJwksAsync(string encodedToken)
    {
        var response = await BackChannelClient.GetAsync(DiscoveryKeysEndpoint);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var jwks = JObject.Parse(await response.Content.ReadAsStringAsync());
        var keys = jwks["keys"]!
            .Select(item => new JsonWebKey(item.ToString()))
            .Cast<SecurityKey>()
            .ToList();
        keys.Should().OnlyContain(key => !(key is SymmetricSecurityKey));

        new JwtSecurityTokenHandler().ValidateToken(
            encodedToken,
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = "security.hcl-cs.com",
                ValidateAudience = true,
                ValidAudience = SbomIdentityContract.ApiAudience,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKeys = keys
            },
            out var validated);
        return validated;
    }

    private static string GenerateCodeVerifier()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    private sealed class MutableLdapAccountValidationService : ILdapAccountValidationService
    {
        public LdapAccountValidationResult Result { get; set; } = LdapAccountValidationResult.Active();
        public int CallCount { get; private set; }
        public string? LastImmutableId { get; private set; }

        public Task<LdapAccountValidationResult> ValidateAccountAsync(
            string directoryImmutableId,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            CallCount++;
            LastImmutableId = directoryImmutableId;
            return Task.FromResult(Result);
        }
    }
}
