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
    private readonly string positiveCaseClientName = "HCL.CS Plain PKCE Client";
    private readonly string redirectUri = "http://127.0.0.1:63562/";
    private ClientsModel clientModel;

    [Fact]
    [Trait("Category", Category)]
    public async Task UserInfoEndpoint_ValidInput_Success()
    {
        await LoginAsync(User);
        clientModel = await FetchClientDetails(positiveCaseClientName);
        clientModel.Should().NotBeNull();
        var nonce = Guid.NewGuid().ToString();
        var codeVerifier = 32.RandomString();
        FrontChannelClient.AllowAutoRedirect = false;
        var authcodeRequest = CreateAuthorizeRequestUrl(
            clientModel.ClientId,
            "code id_token token",
            "openid email profile offline_access phone",
            responseMode: "form_post",
            prompt: "none",
            codeChallenge: codeVerifier,
            codeChallengeMethod: "plain",
            maxAge: "60",
            redirectUri: redirectUri,
            nonce: nonce,
            state: "TestState");
        var returnQuery = await FrontChannelClient.GetAsync(authcodeRequest);
        returnQuery.StatusCode.Should().Be(HttpStatusCode.OK);
        var response = await returnQuery.ParseAuthorizeResponse();
        FrontChannelClient.SetAccessTokenAuthorizationHeader(response.AccessToken);
        var userInfoResponse = await FrontChannelClient.GetAsync(UserInfoEndpoint);
        var userInfoResult = await userInfoResponse.ParseUserInfoResponse();
        userInfoResult.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    [Trait("Category", Category)]
    public async Task UserInfoEndpoint_InvalidAccessToken_ReturnTokenIsNullOrInvalidError()
    {
        await LoginAsync(User);
        clientModel = await FetchClientDetails(positiveCaseClientName);
        clientModel.Should().NotBeNull();
        var nonce = Guid.NewGuid().ToString();
        var codeVerifier = 32.RandomString();
        FrontChannelClient.AllowAutoRedirect = false;
        var authcodeRequest = CreateAuthorizeRequestUrl(
            clientModel.ClientId,
            "code id_token token",
            "openid email profile offline_access phone",
            responseMode: "form_post",
            prompt: "none",
            codeChallenge: codeVerifier,
            codeChallengeMethod: "plain",
            maxAge: "60",
            redirectUri: redirectUri,
            nonce: nonce,
            state: "TestState");

        var returnQuery = await FrontChannelClient.GetAsync(authcodeRequest);
        returnQuery.StatusCode.Should().Be(HttpStatusCode.OK);

        var response = await returnQuery.ParseAuthorizeResponse();

        FrontChannelClient.SetAccessTokenAuthorizationHeader(response.AccessToken + "test001");
        var userInfoResponse = await FrontChannelClient.GetAsync(UserInfoEndpoint);
        userInfoResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var errorResponse = userInfoResponse.ParseUserInfoErrorResponse();
        errorResponse.IsError = true;
        errorResponse.ErrorDescription.Should()
            .Be(ResourceStringHandler.GetResourceString(EndpointErrorCodes.TokenRevoked));
    }

    [Fact]
    [Trait("Category", Category)]
    public async Task UserInfoEndpoint_PassingRefreeshToken_ReturnInvalidTokenFormatError()
    {
        await LoginAsync(User);
        clientModel = await FetchClientDetails(positiveCaseClientName);
        clientModel.Should().NotBeNull();
        var nonce = Guid.NewGuid().ToString();
        var codeVerifier = 32.RandomString();
        FrontChannelClient.AllowAutoRedirect = false;
        var authcodeRequest = CreateAuthorizeRequestUrl(
            clientModel.ClientId,
            "code id_token token",
            "openid email profile offline_access phone",
            responseMode: "form_post",
            prompt: "none",
            codeChallenge: codeVerifier,
            codeChallengeMethod: "plain",
            maxAge: "60",
            redirectUri: redirectUri,
            nonce: nonce,
            state: "TestState");

        var returnQuery = await FrontChannelClient.GetAsync(authcodeRequest);
        returnQuery.StatusCode.Should().Be(HttpStatusCode.OK);
        var response = await returnQuery.ParseAuthorizeResponse();
        FrontChannelClient.SetAccessTokenAuthorizationHeader(response.RefreshToken);
        var userInfoResponse = await FrontChannelClient.GetAsync(UserInfoEndpoint);
        userInfoResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var errorResponse = userInfoResponse.ParseUserInfoErrorResponse();
        errorResponse.IsError = true;
        errorResponse.ErrorCode.Should().Be(OpenIdConstants.Errors.InvalidFormat.ToLower());
        errorResponse.ErrorDescription.Should()
            .Be(ResourceStringHandler.GetResourceString(EndpointErrorCodes.InvalidTokenFormat));
    }

    [Fact]
    [Trait("Category", Category)]
    public async Task UserInfoEndpoint_PassingIdentityToken_ReturnInvalidTokenFormatError()
    {
        await LoginAsync(User);
        clientModel = await FetchClientDetails(positiveCaseClientName);
        clientModel.Should().NotBeNull();
        var nonce = Guid.NewGuid().ToString();
        var codeVerifier = 32.RandomString();
        FrontChannelClient.AllowAutoRedirect = false;
        var authcodeRequest = CreateAuthorizeRequestUrl(
            clientModel.ClientId,
            "code id_token token",
            "openid email profile offline_access phone",
            responseMode: "form_post",
            prompt: "none",
            codeChallenge: codeVerifier,
            codeChallengeMethod: "plain",
            maxAge: "60",
            redirectUri: redirectUri,
            nonce: nonce,
            state: "TestState");

        var returnQuery = await FrontChannelClient.GetAsync(authcodeRequest);
        var response = await returnQuery.ParseAuthorizeResponse();
        FrontChannelClient.SetAccessTokenAuthorizationHeader(response.IdentityToken);
        var userInfoResponse = await FrontChannelClient.GetAsync(UserInfoEndpoint);
        userInfoResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var errorResponse = userInfoResponse.ParseUserInfoErrorResponse();
        errorResponse.IsError = true;
        errorResponse.ErrorDescription.Should()
            .Be(ResourceStringHandler.GetResourceString(EndpointErrorCodes.TokenRevoked));
    }

    [Fact]
    [Trait("Category", Category)]
    public async Task UserInfoEndpoint_MissingTokenAuthorizationHeader_ReturnInvalidUserClaimsError()
    {
        await LoginAsync(User);
        clientModel = await FetchClientDetails(positiveCaseClientName);
        clientModel.Should().NotBeNull();
        var nonce = Guid.NewGuid().ToString();
        var codeVerifier = 32.RandomString();
        FrontChannelClient.AllowAutoRedirect = false;
        var authcodeRequest = CreateAuthorizeRequestUrl(
            clientModel.ClientId,
            "code id_token token",
            "openid email profile offline_access phone",
            responseMode: "form_post",
            prompt: "none",
            codeChallenge: codeVerifier,
            codeChallengeMethod: "plain",
            maxAge: "60",
            redirectUri: redirectUri,
            nonce: nonce,
            state: "TestState");

        var returnQuery = await FrontChannelClient.GetAsync(authcodeRequest);
        var userInfoResponse = await FrontChannelClient.GetAsync(UserInfoEndpoint);
        userInfoResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var errorResponse = userInfoResponse.ParseUserInfoErrorResponse();
        errorResponse.IsError = true;
        errorResponse.ErrorCode.Should().Be(OpenIdConstants.Errors.InvalidToken.ToLower());
    }
}
