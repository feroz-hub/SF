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
using HCL.CS.Domain.ErrorCodes;
using HCL.CS.Service.Implementation.Endpoint.Extensions;

namespace IntegrationTests.Endpoint.UserInfo;

public class UserInfoEndpointTests : HclCsFakeSetup
{
    private const string Category = "UserInfoEndpointTests";
    private const string RedirectUri = "https://127.0.0.1:63562/";

    [Fact]
    [Trait("Category", Category)]
    public async Task UserInfoEndpoint_ValidInput_Success()
    {
        var tokens = await IssueTokensAsync();
        FrontChannelClient.SetAccessTokenAuthorizationHeader(tokens.access_token);

        var response = await FrontChannelClient.GetAsync(UserInfoEndpoint);
        var claims = await response.ParseUserInfoResponse();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        claims.Should().NotBeEmpty();
    }

    [Fact]
    [Trait("Category", Category)]
    public async Task UserInfoEndpoint_InvalidAccessToken_ReturnTokenIsNullOrInvalidError()
    {
        var tokens = await IssueTokensAsync();
        FrontChannelClient.SetAccessTokenAuthorizationHeader(tokens.access_token + "invalid");

        var response = await FrontChannelClient.GetAsync(UserInfoEndpoint);
        var error = response.ParseUserInfoErrorResponse();

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        error.ErrorDescription.Should()
            .Be(ResourceStringHandler.GetResourceString(EndpointErrorCodes.TokenRevoked));
    }

    [Fact]
    [Trait("Category", Category)]
    public async Task UserInfoEndpoint_PassingRefreshToken_ReturnInvalidTokenFormatError()
    {
        var tokens = await IssueTokensAsync();
        FrontChannelClient.SetAccessTokenAuthorizationHeader(tokens.refresh_token);

        var response = await FrontChannelClient.GetAsync(UserInfoEndpoint);
        var error = response.ParseUserInfoErrorResponse();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        error.ErrorCode.Should().Be(OpenIdConstants.Errors.InvalidFormat.ToLowerInvariant());
        error.ErrorDescription.Should()
            .Be(ResourceStringHandler.GetResourceString(EndpointErrorCodes.InvalidTokenFormat));
    }

    [Fact]
    [Trait("Category", Category)]
    public async Task UserInfoEndpoint_PassingIdentityToken_ReturnInvalidTokenFormatError()
    {
        var tokens = await IssueTokensAsync();
        FrontChannelClient.SetAccessTokenAuthorizationHeader(tokens.id_token);

        var response = await FrontChannelClient.GetAsync(UserInfoEndpoint);
        var error = response.ParseUserInfoErrorResponse();

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        error.ErrorDescription.Should()
            .Be(ResourceStringHandler.GetResourceString(EndpointErrorCodes.TokenRevoked));
    }

    [Fact]
    [Trait("Category", Category)]
    public async Task UserInfoEndpoint_MissingTokenAuthorizationHeader_ReturnsUnauthorized()
    {
        var response = await FrontChannelClient.GetAsync(UserInfoEndpoint);
        var error = response.ParseUserInfoErrorResponse();

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        error.ErrorCode.Should().Be(OpenIdConstants.Errors.InvalidToken.ToLowerInvariant());
    }

    private async Task<TokenResponseResultModel> IssueTokensAsync()
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
        authorizeResponse.StatusCode.Should().Be(HttpStatusCode.Found);
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
        tokens.access_token.Should().NotBeNullOrWhiteSpace();
        tokens.id_token.Should().NotBeNullOrWhiteSpace();
        tokens.refresh_token.Should().NotBeNullOrWhiteSpace();
        return tokens;
    }
}
