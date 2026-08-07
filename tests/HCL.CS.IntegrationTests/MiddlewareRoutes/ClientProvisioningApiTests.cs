using System.Net;
using System.Net.Http.Headers;
using System.Text;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;
using HCL.CS.Domain.Constants;
using HCL.CS.Domain.Constants.Endpoint;
using HCL.CS.Domain.Entities.Endpoint;
using HCL.CS.Domain.Enums;
using HCL.CS.Domain.Models.Endpoint;
using HCL.CS.Infrastructure.Data;
using HCL.CS.Service.Interfaces.Interfaces.Api;

namespace IntegrationTests.MiddlewareRoutes;

public class ClientProvisioningApiTests : HclCsFakeSetup
{
    [Fact]
    public async Task ValidClientProfiles_AreIdempotentAuditedAndDoNotRotateSecrets()
    {
        await AuthenticateManagementCallerAsync();
        var profiles = new[]
        {
            CreateSpa("provisioning-spa"),
            CreateWeb("provisioning-web"),
            CreateService("provisioning-m2m")
        };

        foreach (var profile in profiles)
        {
            var firstResponse = await PostProvisioningAsync(profile);
            firstResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var first = JsonConvert.DeserializeObject<ClientsModel>(
                await firstResponse.Content.ReadAsStringAsync());

            var secretAfterFirst = await QueryClientSecretAsync(profile.ClientId);
            var secondResponse = await PostProvisioningAsync(profile);
            secondResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var second = JsonConvert.DeserializeObject<ClientsModel>(
                await secondResponse.Content.ReadAsStringAsync());
            var secretAfterSecond = await QueryClientSecretAsync(profile.ClientId);

            (await CountClientsAsync(profile.ClientId)).Should().Be(1);
            (await CountAuditRecordsAsync(profile.ClientId)).Should().Be(1);
            (await CountDistinctRedirectUrisAsync(profile.ClientId))
                .Should().Be(profile.RedirectUris.Count);
            SplitValues((await GetClientAsync(profile.ClientId)).AllowedScopes)
                .Should().OnlyHaveUniqueItems();
            secretAfterSecond.Should().Be(secretAfterFirst);
            second.ClientSecret.Should().BeNull();

            if (profile.RequireClientSecret)
            {
                first.ClientSecret.Should().NotBeNullOrWhiteSpace();
                secretAfterFirst.Should().NotBe(first.ClientSecret);
            }
            else
            {
                first.ClientSecret.Should().BeNull();
                secretAfterFirst.Should().BeNull();
                (await GetClientAsync(profile.ClientId)).RequirePkce.Should().BeTrue();
            }

            var auditPayload = await GetAuditPayloadAsync(profile.ClientId);
            JObject.Parse(auditPayload).Properties()
                .Select(property => property.Name)
                .Should().NotContain(name =>
                    string.Equals(name, "ClientSecret", StringComparison.OrdinalIgnoreCase));
            auditPayload.Should().NotContain(first.ClientSecret ?? Guid.NewGuid().ToString());
        }
    }

    [Fact]
    public async Task UnauthenticatedCaller_IsRejected()
    {
        var response = await PostProvisioningAsync(CreateSpa("unauthenticated-spa"), authenticate: false);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        (await CountClientsAsync("unauthenticated-spa")).Should().Be(0);
    }

    [Theory]
    [MemberData(nameof(InvalidRequests))]
    public async Task InvalidProvisioningRequests_AreRejected(
        string expectedError,
        Func<ClientsModel> requestFactory)
    {
        await AuthenticateManagementCallerAsync();

        var response = await PostProvisioningAsync(requestFactory());
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        body.Should().Contain(expectedError);
    }

    [Fact]
    public async Task ConflictingReprovisioning_IsRejected()
    {
        await AuthenticateManagementCallerAsync();
        var request = CreateSpa("conflicting-client");
        (await PostProvisioningAsync(request)).StatusCode.Should().Be(HttpStatusCode.OK);
        request.ClientName = "Changed Name";

        var response = await PostProvisioningAsync(request);
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        body.Should().Contain("CLIENT_DEFINITION_CONFLICT");
        (await CountClientsAsync(request.ClientId)).Should().Be(1);
    }

    [Fact]
    public async Task FailureAfterPersistence_RollsBackClientAndAudit()
    {
        OnPostConfigureServices += services =>
        {
            services.RemoveAll<IClientProvisioningTransactionHook>();
            services.AddTransient<IClientProvisioningTransactionHook, ThrowingProvisioningHook>();
        };
        Initialize();
        await AuthenticateManagementCallerAsync();
        var request = CreateService("rollback-client");

        var response = await PostProvisioningAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        (await CountClientsAsync(request.ClientId)).Should().Be(0);
        (await CountAuditRecordsAsync(request.ClientId)).Should().Be(0);
    }

    public static IEnumerable<object[]> InvalidRequests()
    {
        yield return new object[]
        {
            "UNKNOWN_SCOPE",
            () =>
            {
                var request = CreateSpa("unknown-scope");
                request.AllowedScopes = new List<string> { "scope.that.does.not.exist" };
                return request;
            }
        };
        yield return new object[]
        {
            "UNKNOWN_AUDIENCE",
            () =>
            {
                var request = CreateSpa("unknown-audience");
                request.PreferredAudience = "audience.that.does.not.exist";
                return request;
            }
        };
        yield return new object[]
        {
            "INVALID_REDIRECT_URI",
            () =>
            {
                var request = CreateSpa("wildcard-redirect");
                request.RedirectUris[0].RedirectUri = "https://*.example.test/callback";
                return request;
            }
        };
        yield return new object[]
        {
            "INVALID_REDIRECT_URI",
            () =>
            {
                var request = CreateWeb("http-redirect");
                request.RedirectUris[0].RedirectUri = "http://example.test/callback";
                return request;
            }
        };
        yield return new object[]
        {
            "PUBLIC_CLIENT_PKCE_REQUIRED",
            () =>
            {
                var request = CreateSpa("no-pkce");
                request.RequirePkce = false;
                return request;
            }
        };
        yield return new object[]
        {
            "PUBLIC_CLIENT_SECRET_FORBIDDEN",
            () =>
            {
                var request = CreateSpa("public-secret");
                request.ClientSecret = "test-only-public-secret";
                return request;
            }
        };
        yield return new object[]
        {
            "UNSUPPORTED_GRANT_TYPE",
            () =>
            {
                var request = CreateWeb("unsupported-grant");
                request.SupportedGrantTypes = new List<string> { "password" };
                return request;
            }
        };
    }

    private async Task AuthenticateManagementCallerAsync()
    {
        var token = await GetAccessToken();
        FrontChannelClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token.access_token);
    }

    private async Task<HttpResponseMessage> PostProvisioningAsync(
        ClientsModel request,
        bool authenticate = true)
    {
        if (!authenticate)
            FrontChannelClient.DefaultRequestHeaders.Authorization = null;
        return await FrontChannelClient.PostAsync(
            BaseUrl + ApiRoutePathConstants.ProvisionClient,
            new StringContent(
                JsonConvert.SerializeObject(request),
                Encoding.UTF8,
                "application/json"));
    }

    private async Task<Clients> GetClientAsync(string clientId)
    {
        using var scope = ServiceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        return await dbContext.Clients
            .AsNoTracking()
            .Include(client => client.RedirectUris)
            .SingleAsync(client => client.ClientId == clientId);
    }

    private async Task<int> CountClientsAsync(string clientId)
    {
        using var scope = ServiceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        return await dbContext.Clients
            .IgnoreQueryFilters()
            .CountAsync(client => client.ClientId == clientId);
    }

    private async Task<string> QueryClientSecretAsync(string clientId)
    {
        using var scope = ServiceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        return await dbContext.Clients
            .Where(client => client.ClientId == clientId)
            .Select(client => client.ClientSecret)
            .SingleOrDefaultAsync();
    }

    private async Task<int> CountDistinctRedirectUrisAsync(string clientId)
    {
        using var scope = ServiceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var clientKey = await dbContext.Clients
            .Where(client => client.ClientId == clientId)
            .Select(client => client.Id)
            .SingleAsync();
        return await dbContext.RedirectUris
            .Where(uri => uri.ClientId == clientKey)
            .Select(uri => uri.RedirectUri)
            .Distinct()
            .CountAsync();
    }

    private async Task<int> CountAuditRecordsAsync(string clientId)
    {
        using var scope = ServiceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        return await dbContext.AuditTrail
            .CountAsync(audit => audit.ActionName == "ProvisionClient"
                                 && audit.NewValue.Contains(clientId));
    }

    private async Task<string> GetAuditPayloadAsync(string clientId)
    {
        using var scope = ServiceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        return await dbContext.AuditTrail
            .Where(audit => audit.ActionName == "ProvisionClient"
                            && audit.NewValue.Contains(clientId))
            .Select(audit => audit.NewValue)
            .SingleAsync();
    }

    private static ClientsModel CreateSpa(string clientId)
    {
        return CreateAuthorizationCodeClient(
            clientId,
            ApplicationType.SinglePageApp,
            requireClientSecret: false);
    }

    private static ClientsModel CreateWeb(string clientId)
    {
        return CreateAuthorizationCodeClient(
            clientId,
            ApplicationType.RegularWeb,
            requireClientSecret: true);
    }

    private static ClientsModel CreateAuthorizationCodeClient(
        string clientId,
        ApplicationType applicationType,
        bool requireClientSecret)
    {
        return new ClientsModel
        {
            ClientId = clientId,
            ClientName = $"{clientId} client",
            ApplicationType = applicationType,
            RequireClientSecret = requireClientSecret,
            RequirePkce = true,
            AllowOfflineAccess = true,
            PreferredAudience = "hcl-cs.client",
            AllowedSigningAlgorithm = OpenIdConstants.Algorithms.RsaSha256,
            AllowedScopes = new List<string> { "openid", "hcl-cs.client", "offline_access" },
            SupportedGrantTypes = new List<string>
            {
                OpenIdConstants.GrantTypes.AuthorizationCode,
                OpenIdConstants.GrantTypes.RefreshToken
            },
            SupportedResponseTypes = new List<string> { OpenIdConstants.ResponseTypes.Code },
            RedirectUris = new List<ClientRedirectUrisModel>
            {
                new() { RedirectUri = $"https://{clientId}.example.test/callback" }
            },
            PostLogoutRedirectUris = new List<ClientPostLogoutRedirectUrisModel>()
        };
    }

    private static ClientsModel CreateService(string clientId)
    {
        return new ClientsModel
        {
            ClientId = clientId,
            ClientName = $"{clientId} client",
            ApplicationType = ApplicationType.Service,
            RequireClientSecret = true,
            RequirePkce = false,
            PreferredAudience = "hcl-cs.client",
            AllowedSigningAlgorithm = OpenIdConstants.Algorithms.RsaSha256,
            AllowedScopes = new List<string> { "hcl-cs.client" },
            SupportedGrantTypes = new List<string> { OpenIdConstants.GrantTypes.ClientCredentials },
            SupportedResponseTypes = new List<string>(),
            RedirectUris = new List<ClientRedirectUrisModel>(),
            PostLogoutRedirectUris = new List<ClientPostLogoutRedirectUrisModel>()
        };
    }

    private static string[] SplitValues(string values)
    {
        return values.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }

    private sealed class ThrowingProvisioningHook : IClientProvisioningTransactionHook
    {
        public Task BeforeCommitAsync(string clientId, CancellationToken cancellationToken = default)
        {
            throw new InvalidOperationException("Forced provisioning failure before commit.");
        }
    }
}
