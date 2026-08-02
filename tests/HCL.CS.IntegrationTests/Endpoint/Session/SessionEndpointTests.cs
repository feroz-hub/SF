/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using System.Net;
using FluentAssertions;
using IntegrationTests.Endpoint.Setup;
using Xunit;
using HCL.CS.Domain.Constants.Endpoint;
using HCL.CS.Service.Implementation.Endpoint.Extensions;

namespace IntegrationTests.Endpoint.Session;

public class SessionEndpointTests : HclCsFakeSetup
{
    private const string RedirectUri = "https://127.0.0.1:63562/";
    private const string PostLogoutRedirectUri = "https://localhost:5002/signout-oidc";

    [Theory]
    [InlineData("GET")]
    [InlineData("POST")]
    public async Task EndSessionEndpoint_ClearsSessionAndRunsCallback(string method)
    {
        var idToken = await IssueIdentityTokenAsync();
        FrontChannelClient.GetCookie(BaseUrl, ".AspNetCore.Identity.Application").Should().NotBeNull();

        var request = CreateEndSessionRequest(
            idToken,
            PostLogoutRedirectUri,
            Guid.NewGuid().ToString("N"));
        FrontChannelClient.AllowAutoRedirect = true;

        if (method == "GET")
            await FrontChannelClient.GetAsync(EndSessionEndpoint.AddQueryString(request));
        else
            await FrontChannelClient.PostAsync(EndSessionEndpoint, new FormUrlEncodedContent(request));

        LogoutPageCalled.Should().BeTrue();
        FrontChannelClient.GetCookie(BaseUrl, ".AspNetCore.Identity.Application").Should().BeNull();
    }

    private async Task<string> IssueIdentityTokenAsync()
    {
        await LoginAsync(User);
        var client = await FetchClientDetails("HCL.CS S256 Client");
        client.Should().NotBeNull();

        var codeVerifier = GeneratePkceCodeVerifier();
        FrontChannelClient.AllowAutoRedirect = false;
        var authorizeRequest = CreateAuthorizeRequestUrl(
            client.ClientId,
            "code",
            "openid email profile phone",
            responseMode: "query",
            prompt: "none",
            codeChallenge: codeVerifier.GenerateCodeChallenge(),
            codeChallengeMethod: OpenIdConstants.CodeChallengeMethods.Sha256,
            maxAge: "60",
            redirectUri: RedirectUri,
            nonce: Guid.NewGuid().ToString("N"));
        var authorizeResponse = await FrontChannelClient.GetAsync(authorizeRequest);
        authorizeResponse.StatusCode.Should().Be(HttpStatusCode.Found);
        var authorization = authorizeResponse.Headers.Location.ToString().ParseQueryString();

        var tokenRequest = CreateTokenRequest(
            client.ClientId,
            client.ClientSecret,
            authorization.Code,
            RedirectUri,
            OpenIdConstants.GrantTypes.AuthorizationCode,
            codeVerifier);
        var tokenResponse = await BackChannelClient.PostAsync(
            TokenEndpoint,
            new FormUrlEncodedContent(tokenRequest));
        var tokens = await tokenResponse.ParseTokenResponseResult();

        tokenResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        tokens.id_token.Should().NotBeNullOrWhiteSpace();
        return tokens.id_token;
    }
}
