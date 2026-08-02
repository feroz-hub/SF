/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using System.Net;
using FluentAssertions;
using IntegrationTests.ApiDomainModel;
using IntegrationTests.Endpoint.Setup;
using Xunit;
using HCL.CS.Domain.Constants.Endpoint;
using HCL.CS.Service.Implementation.Endpoint.Extensions;

namespace IntegrationTests.Endpoint.FlowTests;

public class RevocationTestsFlow : HclCsFakeSetup
{
    private const string RedirectUri = "https://127.0.0.1:63562/";

    [Fact]
    public async Task TokenRevocation_ValidInput_ReturnSuccess()
    {
        var (client, tokens) = await IssueTokensAsync();
        var request = CreateRevocationRequest(
            client.ClientId,
            client.ClientSecret,
            tokens.refresh_token,
            OpenIdConstants.TokenType.RefreshToken);

        var response = await BackChannelClient.PostAsync(
            RevocationEndpoint,
            new FormUrlEncodedContent(request));

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var introspectionRequest = CreateIntroSpecRequest(
            clientId: client.ClientId,
            clientSecret: client.ClientSecret,
            token: tokens.refresh_token,
            tokenTypeHint: OpenIdConstants.TokenType.RefreshToken);
        var introspectionResponse = await BackChannelClient.PostAsync(
            IntrospectionEndpoint,
            new FormUrlEncodedContent(introspectionRequest));
        var introspection = await introspectionResponse.ParseIntrospectionResponse();
        introspection.Active.Should().BeFalse();
    }

    [Fact]
    public async Task Token_Revocation_MissingToken_ReturnInvalidRequest()
    {
        var (client, _) = await IssueTokensAsync();
        var request = CreateRevocationRequest(
            client.ClientId,
            client.ClientSecret,
            tokentypehint: OpenIdConstants.TokenType.RefreshToken);

        var response = await BackChannelClient.PostAsync(
            RevocationEndpoint,
            new FormUrlEncodedContent(request));
        var error = await response.ParseIntrospectionErrorResponse();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        error.ErrorCode.Should().Be(OpenIdConstants.Errors.InvalidRequest);
    }

    [Fact]
    public async Task Token_Revocation_UnknownToken_IsIdempotent()
    {
        var (client, tokens) = await IssueTokensAsync();
        var request = CreateRevocationRequest(
            client.ClientId,
            client.ClientSecret,
            tokens.refresh_token + "unknown",
            OpenIdConstants.TokenType.RefreshToken);

        var response = await BackChannelClient.PostAsync(
            RevocationEndpoint,
            new FormUrlEncodedContent(request));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Token_Revocation_InvalidTokenHinttype_ReturnErrorUnsupportedTokenType()
    {
        var (client, tokens) = await IssueTokensAsync();
        var request = CreateRevocationRequest(
            client.ClientId,
            client.ClientSecret,
            tokens.refresh_token,
            OpenIdConstants.TokenType.RefreshToken + "invalid");

        var response = await BackChannelClient.PostAsync(
            RevocationEndpoint,
            new FormUrlEncodedContent(request));
        var error = await response.ParseIntrospectionErrorResponse();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        error.ErrorCode.Should().Be(OpenIdConstants.Errors.UnsupportedTokenType);
    }

    private async Task<(ClientsModel Client, TokenResponseResultModel Tokens)> IssueTokensAsync()
    {
        await LoginAsync(User);
        var client = await FetchClientDetails("HCL.CS S256 Client");
        client.Should().NotBeNull();

        var codeVerifier = GeneratePkceCodeVerifier();
        FrontChannelClient.AllowAutoRedirect = false;
        var authorizeRequest = CreateAuthorizeRequestUrl(
            client.ClientId,
            "code",
            "openid email profile offline_access phone",
            responseMode: "query",
            prompt: "none",
            codeChallenge: codeVerifier.GenerateCodeChallenge(),
            codeChallengeMethod: OpenIdConstants.CodeChallengeMethods.Sha256,
            maxAge: "60",
            redirectUri: RedirectUri,
            nonce: Guid.NewGuid().ToString("N"));
        var authorizeResponse = await FrontChannelClient.GetAsync(authorizeRequest);
        var authorization = authorizeResponse.Headers.Location.ToString().ParseQueryString();
        authorization.Code.Should().NotBeNullOrWhiteSpace();

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
        tokens.refresh_token.Should().NotBeNullOrWhiteSpace();
        return (client, tokens);
    }
}
