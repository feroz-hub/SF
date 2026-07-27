using System.Net;
using System.Security.Cryptography;
using System.Data;
using FluentAssertions;
using IntegrationTests.ApiDomainModel;
using IntegrationTests.Endpoint.Setup;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Xunit;
using HCL.CS.Domain.Constants.Endpoint;
using HCL.CS.Infrastructure.Data;
using HCL.CS.Service.Implementation.Endpoint.Extensions;

namespace IntegrationTests.Endpoint.FlowTests;

public class Phase1LdapDisabledCompatibilityTests : HclCsFakeSetup
{
    private const string ClientName = "HCL.CS S256 Client";
    private const string RedirectUri = "https://127.0.0.1:63562/";

    [Fact]
    public async Task LdapDisabled_LocalPasswordLogin_Succeeds()
    {
        HCL.CS.Domain.GlobalConfiguration.IsLdapConfigurationValid.Should().BeFalse();

        var act = () => LoginAsync(User);

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task LdapDisabled_AuthorizationCodeWithS256Pkce_IssuesTokens()
    {
        var (_, token) = await GetAuthorizationCodeTokenAsync();

        token.access_token.Should().NotBeNullOrWhiteSpace();
        token.id_token.Should().NotBeNullOrWhiteSpace();
        token.refresh_token.Should().NotBeNullOrWhiteSpace();
        token.token_type.Should().Be(OpenIdConstants.TokenResponseType.BearerTokenType);
    }

    [Fact]
    public async Task LdapDisabled_Discovery_RemainsAvailable()
    {
        var response = await BackChannelClient.GetAsync(DiscoveryEndpoint);
        var payload = JObject.Parse(await response.Content.ReadAsStringAsync());

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        payload.Value<string>("issuer").Should().Be("security.hcl-cs.com");
        payload.Value<string>("authorization_endpoint").Should().NotBeNullOrWhiteSpace();
        payload.Value<string>("token_endpoint").Should().NotBeNullOrWhiteSpace();
        payload.Value<string>("jwks_uri").Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task LdapDisabled_Jwks_ExposesValidAsymmetricKeys()
    {
        var response = await BackChannelClient.GetAsync(DiscoveryKeysEndpoint);
        var payload = JObject.Parse(await response.Content.ReadAsStringAsync());
        var keys = payload["keys"] as JArray;

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        keys.Should().NotBeNullOrEmpty();
        keys.Should().OnlyContain(key =>
            !string.IsNullOrWhiteSpace(key.Value<string>("kty")) &&
            !string.IsNullOrWhiteSpace(key.Value<string>("kid")) &&
            !string.IsNullOrWhiteSpace(key.Value<string>("alg")));

        var rsaKeys = keys.Where(key => key.Value<string>("kty") == "RSA").ToList();
        rsaKeys.Should().NotBeEmpty();
        rsaKeys.Should().OnlyContain(key =>
            !string.IsNullOrWhiteSpace(key.Value<string>("n")) &&
            !string.IsNullOrWhiteSpace(key.Value<string>("e")));

        var ecKeys = keys.Where(key => key.Value<string>("kty") == "EC").ToList();
        ecKeys.Should().NotBeEmpty();
        ecKeys.Should().OnlyContain(key =>
            !string.IsNullOrWhiteSpace(key.Value<string>("crv")) &&
            !string.IsNullOrWhiteSpace(key.Value<string>("x")) &&
            !string.IsNullOrWhiteSpace(key.Value<string>("y")));
    }

    [Fact]
    public async Task LdapDisabled_EndSession_ReachesExistingLogoutWorkflow()
    {
        var (_, token) = await GetAuthorizationCodeTokenAsync();
        FrontChannelClient.AllowAutoRedirect = false;
        var endSessionRequest = CreateEndSessionRequest(
            token.id_token,
            "https://localhost:5002/signout-callback-oidc",
            Guid.NewGuid().ToString("N"));

        var endSessionResponse = await FrontChannelClient.PostAsync(
            EndSessionEndpoint,
            new FormUrlEncodedContent(endSessionRequest));

        endSessionResponse.StatusCode.Should().Be(HttpStatusCode.Found);
        endSessionResponse.Headers.Location.Should().NotBeNull();
        endSessionResponse.Headers.Location.ToString().Should().Contain(LogoutUrl);

        await FrontChannelClient.GetAsync(endSessionResponse.Headers.Location);
        LogoutPageCalled.Should().BeTrue();
    }

    [Fact]
    public async Task SqliteSeedSchema_CoversCurrentEfModel()
    {
        using var scope = ServiceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var connection = dbContext.Database.GetDbConnection();
        var shouldClose = connection.State != ConnectionState.Open;
        if (shouldClose) await connection.OpenAsync();

        try
        {
            var mappedTables = dbContext.Model.GetEntityTypes()
                .Where(entityType => entityType.GetTableName() is not null)
                .GroupBy(entityType => entityType.GetTableName())
                .ToDictionary(group => group.Key, group => group.ToList());

            mappedTables.Keys.Should().Contain(new[]
            {
                "HclCs_Clients",
                "HclCs_ApiResources",
                "HclCs_ApiScopes",
                "HclCs_ClientRedirectUris",
                "HclCs_ClientPostLogoutRedirectUris",
                "HclCs_SecurityTokens",
                "HclCs_Users",
                "HclCs_Roles",
                "HclCs_UserRoles"
            });

            var mismatches = new List<string>();
            foreach (var (tableName, entityTypes) in mappedTables)
            {
                await using var command = connection.CreateCommand();
                command.CommandText = $"PRAGMA table_info(\"{tableName.Replace("\"", "\"\"")}\")";
                await using var reader = await command.ExecuteReaderAsync();
                var actualColumns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                while (await reader.ReadAsync()) actualColumns.Add(reader.GetString(1));

                if (actualColumns.Count == 0)
                {
                    mismatches.Add($"missing table {tableName}");
                    continue;
                }

                var storeObject = StoreObjectIdentifier.Table(tableName, null);
                var expectedColumns = entityTypes
                    .SelectMany(entityType => entityType.GetProperties())
                    .Select(property => property.GetColumnName(storeObject))
                    .Where(columnName => columnName is not null)
                    .Distinct(StringComparer.OrdinalIgnoreCase);

                var missingColumns = expectedColumns
                    .Where(expectedColumn => !actualColumns.Contains(expectedColumn))
                    .ToList();
                if (missingColumns.Count > 0)
                    mismatches.Add($"{tableName}: missing columns {string.Join(", ", missingColumns)}");
            }

            mismatches.Should().BeEmpty("the SQLite seed schema must match the current EF model");
        }
        finally
        {
            if (shouldClose) await connection.CloseAsync();
        }
    }

    private async Task<(ClientsModel Client, TokenResponseResultModel Token)> GetAuthorizationCodeTokenAsync()
    {
        await LoginAsync(User);
        var client = await FetchClientDetails(ClientName);
        client.Should().NotBeNull();

        var codeVerifier = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
        var codeChallenge = codeVerifier.GenerateCodeChallenge();
        FrontChannelClient.AllowAutoRedirect = false;
        var authorizeRequest = CreateAuthorizeRequestUrl(
            client.ClientId,
            "code",
            "openid email profile offline_access hcl-cs.client",
            responseMode: "query",
            prompt: "none",
            codeChallenge: codeChallenge,
            codeChallengeMethod: OpenIdConstants.CodeChallengeMethods.Sha256,
            maxAge: "60",
            redirectUri: RedirectUri,
            state: Guid.NewGuid().ToString("N"),
            nonce: Guid.NewGuid().ToString("N"));

        var authorizeResponse = await FrontChannelClient.GetAsync(authorizeRequest);
        authorizeResponse.StatusCode.Should().Be(HttpStatusCode.Found);
        authorizeResponse.Headers.Location.Should().NotBeNull();
        var authorizePayload = authorizeResponse.Headers.Location.ToString().ParseQueryString();
        authorizePayload.Code.Should().NotBeNullOrWhiteSpace();

        var tokenRequest = CreateTokenRequest(
            client.ClientId,
            client.ClientSecret,
            authorizePayload.Code,
            RedirectUri,
            OpenIdConstants.GrantTypes.AuthorizationCode,
            codeVerifier);
        var tokenResponse = await BackChannelClient.PostAsync(
            TokenEndpoint,
            new FormUrlEncodedContent(tokenRequest));
        var tokenPayload = await tokenResponse.ParseTokenResponseResult();

        tokenResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        return (client, tokenPayload);
    }
}
