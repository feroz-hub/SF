/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using System.Net.Http.Headers;
using System.Text;
using FluentAssertions;
using HCL.CS.Domain.Constants;
using IntegrationTests.ApiDomainModel;
using Newtonsoft.Json;
using TestApp.Helper.Endpoint;
using Xunit;

namespace IntegrationTests.MiddlewareRoutes;

/// <summary>
/// Regression tests for the client-update persistence pipeline. These reproduce the Admin
/// "edit HCL.CS Admin client, add a scope, Update" flow: load an existing client (with redirect and
/// post-logout child rows) and submit it back with one extra scope. The update must succeed, must not
/// duplicate or drop child rows, must keep the client id/secret, and must be idempotent.
/// </summary>
public class ClientUpdatePersistenceTests : HclCsFakeSetup
{
    private readonly Random random = new();

    private async Task<string> PostAsync(string path, object payload)
    {
        var response = await FrontChannelClient.PostAsync(
            BaseUrl + path,
            new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json"));
        return await response.Content.ReadAsStringAsync();
    }

    private async Task<ClientsModel> RegisterClientWithChildUrisAsync()
    {
        var model = ClientHelper.CreateClientsModel();
        model.ClientName = "ClientUpdatePersistence" + random.Next();
        model.ClientId = Guid.NewGuid().ToString();
        model.ClientSecret = Guid.NewGuid().ToString();

        var token = await GetAccessToken();
        FrontChannelClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token.access_token);
        await LoginAsync(User);

        var registered = JsonConvert.DeserializeObject<ClientsModel>(
            await PostAsync(ApiRoutePathConstants.RegisterClient, model));
        registered.Should().NotBeNull();
        registered.ClientId.Should().NotBeNullOrWhiteSpace();
        return registered;
    }

    private async Task<ClientsModel> GetClientAsync(string clientId)
    {
        return JsonConvert.DeserializeObject<ClientsModel>(
            await PostAsync(ApiRoutePathConstants.GetClient, clientId));
    }

    [Fact]
    public async Task UpdateClient_AddScope_Succeeds_PreservesChildrenAndSecret_AndIsIdempotent()
    {
        var registered = await RegisterClientWithChildUrisAsync();
        var clientId = registered.ClientId;

        var before = await GetClientAsync(clientId);
        var redirectCount = before.RedirectUris.Count;
        var postLogoutCount = before.PostLogoutRedirectUris.Count;
        var storedSecret = before.ClientSecret;
        redirectCount.Should().BeGreaterThan(0);
        postLogoutCount.Should().BeGreaterThan(0);

        // Edit exactly like the Admin UI: add a (valid, seeded) scope, leave lifetimes / URIs untouched.
        const string addedScope = "openid";
        before.AllowedScopes.Add(addedScope);
        var updateRaw = await PostAsync(ApiRoutePathConstants.UpdateClient, before);
        var updateResponse = JsonConvert.DeserializeObject<ClientsModel>(updateRaw);
        updateResponse.Should().NotBeNull();
        updateResponse.ClientId.Should().Be(clientId, "update should succeed. Server response: {0}", updateRaw);

        var after = await GetClientAsync(clientId);
        after.AllowedScopes.Should().Contain(addedScope);
        after.ClientId.Should().Be(clientId);
        after.ClientSecret.Should().Be(storedSecret, "an ordinary update must not rotate the client secret");
        after.RedirectUris.Count.Should().Be(redirectCount, "existing redirect URIs must not be duplicated or dropped");
        after.PostLogoutRedirectUris.Count.Should().Be(postLogoutCount,
            "existing post-logout URIs must not be duplicated or dropped");
        after.RedirectUris.Select(u => u.RedirectUri).Should()
            .BeEquivalentTo(before.RedirectUris.Select(u => u.RedirectUri));
        after.PostLogoutRedirectUris.Select(u => u.PostLogoutRedirectUri).Should()
            .BeEquivalentTo(before.PostLogoutRedirectUris.Select(u => u.PostLogoutRedirectUri));

        // Idempotency: submitting the same client again must not create duplicate child rows.
        var secondRaw = await PostAsync(ApiRoutePathConstants.UpdateClient, after);
        var second = JsonConvert.DeserializeObject<ClientsModel>(secondRaw);
        second.Should().NotBeNull();
        second.ClientId.Should().Be(clientId, "repeat update should succeed. Server response: {0}", secondRaw);

        var afterSecond = await GetClientAsync(clientId);
        afterSecond.RedirectUris.Count.Should().Be(redirectCount);
        afterSecond.PostLogoutRedirectUris.Count.Should().Be(postLogoutCount);
        afterSecond.AllowedScopes.Should().Contain(addedScope);
    }

    [Fact]
    public async Task UpdateClient_AddRedirectUri_AddsExactlyOneRow()
    {
        var registered = await RegisterClientWithChildUrisAsync();
        var clientId = registered.ClientId;

        var before = await GetClientAsync(clientId);
        var redirectCount = before.RedirectUris.Count;
        var newUri = "https://localhost:44300/added-" + random.Next() + ".html";
        before.RedirectUris.Add(new ClientRedirectUrisModel { ClientId = before.Id, RedirectUri = newUri });

        var updateRaw = await PostAsync(ApiRoutePathConstants.UpdateClient, before);
        var updateResponse = JsonConvert.DeserializeObject<ClientsModel>(updateRaw);
        updateResponse.ClientId.Should().Be(clientId, "update should succeed. Server response: {0}", updateRaw);

        var after = await GetClientAsync(clientId);
        after.RedirectUris.Count.Should().Be(redirectCount + 1);
        after.RedirectUris.Select(u => u.RedirectUri).Should().Contain(newUri);
    }

    [Fact]
    public async Task UpdateClient_RemoveRedirectUri_RemovesExactlyOneRow()
    {
        var registered = await RegisterClientWithChildUrisAsync();
        var clientId = registered.ClientId;

        var before = await GetClientAsync(clientId);
        before.RedirectUris.Count.Should().BeGreaterThan(1);
        var removedUri = before.RedirectUris.First().RedirectUri;
        before.RedirectUris = before.RedirectUris.Skip(1).ToList();

        var updateResponse = JsonConvert.DeserializeObject<ClientsModel>(
            await PostAsync(ApiRoutePathConstants.UpdateClient, before));
        updateResponse.ClientId.Should().Be(clientId);

        var after = await GetClientAsync(clientId);
        after.RedirectUris.Select(u => u.RedirectUri).Should().NotContain(removedUri);
        after.RedirectUris.Count.Should().Be(before.RedirectUris.Count);
    }
}
