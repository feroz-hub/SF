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
using IntegrationTests.Endpoint.Helper;
using IntegrationTests.Endpoint.Setup;
using Microsoft.IdentityModel.Tokens;
using Xunit;
using HCL.CS.Domain.Constants.Endpoint;
using HCL.CS.Service.Implementation.Endpoint.Extensions;

namespace IntegrationTests.Endpoint.FlowTests;

public class JwksFlowTest : HclCsFakeSetup
{
    private const string Category = "Introspection endpoint";
    private readonly string audience = "hcl-cs.api";

    private readonly string hCLCSEarlyTokenExpireClient = "HCL.CS Early Token Expire Client";
    private readonly string hclCSES256AlgorithmClient = "HCL.CS ES256";
    private readonly string hclCSES512AlgorithmClient = "HCL.CS ES512";

    private readonly string hclCSHS256AlgorithmClient = "HCL.CS HS256";
    private readonly string hclCSHS512AlgorithmClient = "HCL.CS HS512";

    private readonly string hclCSPS256AlgorithmClient = "HCL.CS PS256";
    private readonly string hclCSPS512AlgorithmClient = "HCL.CS PS512";
    private readonly string hclCSRS256AlgorithmClient = "HCL.CS RS256";

    private readonly string hclCSRS512AlgorithmClient = "HCL.CS RS512";
    private readonly string issuer = "security.hcl-cs.com";
    private readonly JwksTestHelper JwksTestHelper = new();

    private readonly string redirectUri = "https://127.0.0.1:63562/";
    private ClientsModel clientModel;
    private string positiveCaseClientName = "HCL.CS Plain PKCE Client";


    [Fact]
    [Trait("Category", Category)]
    public async Task JwksAsymmetricRS256_PassingValidAccessToken_ReturnSuccess()
    {
        string clientName = hclCSRS256AlgorithmClient,
            responseType = "code",
            scopes = "openid email profile phone offline_access",
            codeChallengeMethod = "S256";
        var tokenResult = await TokenGenerationFlow(clientName, responseType, scopes, codeChallengeMethod);
        new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(tokenResult.access_token)
            .Audiences.Should().Contain(audience);
        var result = JwksTestHelper.ValidateToken(tokenResult.access_token, issuer, audience).GetAwaiter().GetResult();
        result.Should().NotBeNull();
    }

    [Fact]
    [Trait("Category", Category)]
    public async Task JwksAsymmetricRS256_PassingValidIssueandAudienceAccessToken_ReturnSuccess()
    {
        string clientName = hclCSRS256AlgorithmClient,
            responseType = "code",
            scopes = "openid email profile phone offline_access",
            codeChallengeMethod = "S256";
        var tokenResult = await TokenGenerationFlow(clientName, responseType, scopes, codeChallengeMethod);
        var result = JwksTestHelper.ValidateToken(tokenResult.access_token, issuer, audience).GetAwaiter().GetResult();
        result.Should().NotBeNull();
    }

    // RS 512

    [Fact]
    [Trait("Category", Category)]
    public async Task JwksAsymmetricRS512_IsRejected()
    {
        await AssertUnsupportedSigningAlgorithmRejectedAsync(hclCSRS512AlgorithmClient);
    }

    // ES 256

    [Fact]
    [Trait("Category", Category)]
    public async Task JwksAsymmetricES256_PassingValidIssueandAudienceAccessToken_ReturnSuccess()
    {
        string clientName = hclCSES256AlgorithmClient,
            responseType = "code",
            scopes = "openid email profile phone offline_access",
            codeChallengeMethod = "S256";
        var tokenResult = await TokenGenerationFlow(clientName, responseType, scopes, codeChallengeMethod);
        var result = JwksTestHelper.ValidateToken(tokenResult.access_token, issuer, audience, clientModel.ClientSecret)
            .GetAwaiter().GetResult();
        result.Should().NotBeNull();
    }

    // ES 512

    [Fact]
    [Trait("Category", Category)]
    public async Task JwksAsymmetricES512_IsRejected()
    {
        await AssertUnsupportedSigningAlgorithmRejectedAsync(hclCSES512AlgorithmClient);
    }

    // PS 256

    [Fact]
    [Trait("Category", Category)]
    public async Task JwksAsymmetricPS256_IsRejected()
    {
        await AssertUnsupportedSigningAlgorithmRejectedAsync(hclCSPS256AlgorithmClient);
    }

    // PS 512

    [Fact]
    [Trait("Category", Category)]
    public async Task JwksAsymmetricPS512_IsRejected()
    {
        await AssertUnsupportedSigningAlgorithmRejectedAsync(hclCSPS512AlgorithmClient);
    }

    // Access Token - Symmetric -Positive Flows.
    // HS 256

    [Fact]
    [Trait("Category", Category)]
    public async Task JwksSymmetricHS256_IsRejected()
    {
        await AssertUnsupportedSigningAlgorithmRejectedAsync(hclCSHS256AlgorithmClient);
    }

    // HS 512

    [Fact]
    [Trait("Category", Category)]
    public async Task JwksSymmetricHS512_IsRejected()
    {
        await AssertUnsupportedSigningAlgorithmRejectedAsync(hclCSHS512AlgorithmClient);
    }

    // Identity Token - Asymmetric -Positive Flows.

    // RS 256

    [Fact]
    [Trait("Category", Category)]
    public async Task JwksAsymmetricRS256_PassingValidIdentityToken_ReturnSuccess()
    {
        string clientName = hclCSRS256AlgorithmClient,
            responseType = "code",
            scopes = "openid email profile phone offline_access",
            codeChallengeMethod = "S256";
        var tokenResult = await TokenGenerationFlow(clientName, responseType, scopes, codeChallengeMethod);
        tokenResult.Should().NotBeNull();
        var result = JwksTestHelper.ValidateToken(tokenResult.id_token).GetAwaiter().GetResult();
        result.Id.Should().NotBeNull();
        result.SigningKey.Should().NotBeNull();
        result.Should().NotBeNull();
    }

    [Fact]
    [Trait("Category", Category)]
    public async Task JwksAsymmetricRS256_PassingValidIssueandAudienceIdentityToken_ReturnSuccess()
    {
        string clientName = hclCSRS256AlgorithmClient,
            responseType = "code",
            scopes = "openid email profile phone offline_access",
            codeChallengeMethod = "S256";
        var tokenResult = await TokenGenerationFlow(clientName, responseType, scopes, codeChallengeMethod);
        var result = JwksTestHelper.ValidateToken(tokenResult.id_token, issuer, clientModel.ClientId).GetAwaiter()
            .GetResult();
        result.Should().NotBeNull();
    }

    // RS 512

    [Fact]
    [Trait("Category", Category)]
    public async Task JwksAsymmetricRS512Identity_IsRejected()
    {
        await AssertUnsupportedSigningAlgorithmRejectedAsync(hclCSRS512AlgorithmClient);
    }

    // ES 256

    [Fact]
    [Trait("Category", Category)]
    public async Task JwksAsymmetricES256_PassingValidIssueandAudienceIdentityToken_ReturnSuccess()
    {
        string clientName = hclCSES256AlgorithmClient,
            responseType = "code",
            scopes = "openid email profile phone offline_access",
            codeChallengeMethod = "S256";
        var tokenResult = await TokenGenerationFlow(clientName, responseType, scopes, codeChallengeMethod);
        var result = JwksTestHelper
            .ValidateToken(tokenResult.id_token, issuer, clientModel.ClientId, clientModel.ClientSecret).GetAwaiter()
            .GetResult();
        result.Should().NotBeNull();
    }

    // ES 512

    [Fact]
    [Trait("Category", Category)]
    public async Task JwksAsymmetricES512Identity_IsRejected()
    {
        await AssertUnsupportedSigningAlgorithmRejectedAsync(hclCSES512AlgorithmClient);
    }

    // PS 256

    [Fact]
    [Trait("Category", Category)]
    public async Task JwksAsymmetricPS256Identity_IsRejected()
    {
        await AssertUnsupportedSigningAlgorithmRejectedAsync(hclCSPS256AlgorithmClient);
    }

    // PS 512

    [Fact]
    [Trait("Category", Category)]
    public async Task JwksAsymmetricPS512Identity_IsRejected()
    {
        await AssertUnsupportedSigningAlgorithmRejectedAsync(hclCSPS512AlgorithmClient);
    }

    // Identity Token - Symmetric -Positive Flows.
    // HS 256

    [Fact]
    [Trait("Category", Category)]
    public async Task JwksSymmetricHS256Identity_IsRejected()
    {
        await AssertUnsupportedSigningAlgorithmRejectedAsync(hclCSHS256AlgorithmClient);
    }

    // HS 512

    [Fact]
    [Trait("Category", Category)]
    public async Task JwksSymmetricHS512Identity_IsRejected()
    {
        await AssertUnsupportedSigningAlgorithmRejectedAsync(hclCSHS512AlgorithmClient);
    }

    // Negative Scenarios

    [Fact]
    [Trait("Category", Category)]
    public async Task JwksAsymmetricRS256_TruncatedAccessToken_IsRejected()
    {
        string clientName = hclCSRS256AlgorithmClient,
            responseType = "code",
            scopes = "openid email profile phone offline_access",
            codeChallengeMethod = "S256";
        var tokenResult = await TokenGenerationFlow(clientName, responseType, scopes, codeChallengeMethod);
        tokenResult.Should().NotBeNull();
        var accesstoken = tokenResult.access_token;
        var index = accesstoken.Length - 3;
        var resultAccessToken = accesstoken.Substring(2, index);
        Func<Task> validate = async () => await JwksTestHelper
            .ValidateToken(resultAccessToken, issuer, clientModel.ClientId, clientModel.ClientSecret);
        await validate.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    [Trait("Category", Category)]
    public async Task JwksAsymmetricRS256_TamperedAccessToken_IsRejected()
    {
        string clientName = hclCSRS256AlgorithmClient,
            responseType = "code",
            scopes = "openid email profile phone offline_access",
            codeChallengeMethod = "S256";
        var tokenResult = await TokenGenerationFlow(clientName, responseType, scopes, codeChallengeMethod);
        var accesstoken = tokenResult.access_token + "zxo";
        Func<Task> validate = async () => await JwksTestHelper.ValidateToken(accesstoken);
        await validate.Should().ThrowAsync<SecurityTokenInvalidSignatureException>();
    }

    [Fact]
    [Trait("Category", Category)]
    public async Task JwksAsymmetricRS256_AccessTokenBeforeExpiry_IsValid()
    {
        var tokenResult = await TokenGenerationFlow_EarlyExpirationFlow();
        var result = JwksTestHelper.ValidateToken(tokenResult.access_token).GetAwaiter().GetResult();
        result.Should().NotBeNull();
    }

    [Fact]
    [Trait("Category", Category)]
    public async Task JwksAsymmetric_InvalidAudienceAccessToken_IsRejected()
    {
        string clientName = hclCSES256AlgorithmClient,
            responseType = "code",
            scopes = "openid email profile phone offline_access",
            codeChallengeMethod = "S256";
        var tokenResult = await TokenGenerationFlow(clientName, responseType, scopes, codeChallengeMethod);
        Func<Task> validate = async () =>
            await JwksTestHelper.ValidateToken(tokenResult.access_token, issuer, "invalid-audience");
        await validate.Should().ThrowAsync<SecurityTokenInvalidAudienceException>();
    }

    [Fact]
    [Trait("Category", Category)]
    public async Task JwksAsymmetricRS256_IdentityTokenBeforeExpiry_IsValid()
    {
        var tokenResult = await TokenGenerationFlow_EarlyExpirationFlow();
        var result = JwksTestHelper.ValidateToken(tokenResult.id_token).GetAwaiter().GetResult();
        result.Should().NotBeNull();
    }

    [Fact]
    [Trait("Category", Category)]
    public async Task JwksAsymmetricRS256_TamperedIdentityToken_IsRejected()
    {
        string clientName = hclCSRS256AlgorithmClient,
            responseType = "code",
            scopes = "openid email profile phone offline_access",
            codeChallengeMethod = "S256";
        var tokenResult = await TokenGenerationFlow(clientName, responseType, scopes, codeChallengeMethod);
        Func<Task> validate = async () => await JwksTestHelper.ValidateToken(tokenResult.id_token + "zxe");
        await validate.Should().ThrowAsync<SecurityTokenInvalidSignatureException>();
    }

    [Fact]
    [Trait("Category", Category)]
    public async Task JwksAsymmetric_InvalidAudienceIdentityToken_IsRejected()
    {
        string clientName = hclCSES256AlgorithmClient,
            responseType = "code",
            scopes = "openid email profile phone offline_access",
            codeChallengeMethod = "S256";
        var tokenResult = await TokenGenerationFlow(clientName, responseType, scopes, codeChallengeMethod);
        Func<Task> validate = async () =>
            await JwksTestHelper.ValidateToken(tokenResult.id_token, issuer, "invalid-audience");
        await validate.Should().ThrowAsync<SecurityTokenInvalidAudienceException>();
    }

    private async Task AssertUnsupportedSigningAlgorithmRejectedAsync(string clientName)
    {
        await LoginAsync(User);
        clientModel = await FetchClientDetails(clientName);
        clientModel.Should().NotBeNull();
        var codeVerifier = GeneratePkceCodeVerifier();
        FrontChannelClient.AllowAutoRedirect = false;
        var request = CreateAuthorizeRequestUrl(
            clientModel.ClientId,
            "code",
            "openid email profile phone offline_access",
            responseMode: "query",
            prompt: "none",
            codeChallenge: codeVerifier.GenerateCodeChallenge(),
            codeChallengeMethod: "S256",
            maxAge: "60",
            redirectUri: redirectUri,
            nonce: Guid.NewGuid().ToString());

        var response = await FrontChannelClient.GetAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Found);
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.AbsolutePath.Should().Be("/home/error");
    }

    private async Task<TokenResponseResultModel> TokenGenerationFlow_EarlyExpirationFlow()
    {
        await LoginAsync(User);
        positiveCaseClientName = hCLCSEarlyTokenExpireClient;
        clientModel = await FetchClientDetails(positiveCaseClientName);
        clientModel.Should().NotBeNull();
        var nonce = Guid.NewGuid().ToString();
        var codeVerifier = GeneratePkceCodeVerifier();
        FrontChannelClient.AllowAutoRedirect = false;
        var authcodeRequest = CreateAuthorizeRequestUrl(
            clientModel.ClientId,
            "code",
            "openid email profile phone offline_access",
            responseMode: "query",
            prompt: "none",
            codeChallenge: codeVerifier.GenerateCodeChallenge(),
            codeChallengeMethod: "S256",
            maxAge: "60",
            redirectUri: redirectUri,
            nonce: nonce);
        var returnQuery = await FrontChannelClient.GetAsync(authcodeRequest);
        var response = returnQuery.Headers.Location.ToString().ParseQueryString();
        response.Code.Should().NotBeNull(
            "the authorization request should succeed; error={0}, description={1}, location={2}",
            response.ErrorCode,
            response.ErrorDescription,
            returnQuery.Headers.Location);
        var code = response.Code;
        var tokenClient = BackChannelClient;

        var tokenRequest = CreateTokenRequest(
            clientModel.ClientId,
            clientModel.ClientSecret,
            code,
            redirectUri,
            OpenIdConstants.GrantTypes.AuthorizationCode,
            codeVerifier);
        var tokenResponse = await tokenClient.PostAsync(TokenEndpoint, new FormUrlEncodedContent(tokenRequest));
        var tokenResult = await tokenResponse.ParseTokenResponseResult();
        tokenResult.access_token.Should().NotBeNull();
        tokenResult.id_token.Should().NotBeNullOrEmpty();
        tokenResult.refresh_token.Should().NotBeNullOrEmpty();
        tokenResult.expires_in.Should().BeGreaterThan(0);
        tokenResult.token_type.Should().Be(OpenIdConstants.TokenResponseType.BearerTokenType);
        JwksTestHelper.BackChannelClient = BackChannelClient;
        return tokenResult;
    }

    private async Task<TokenResponseResultModel> TokenGenerationFlow(string clientName, string responseType,
        string scopes, string codeChallengeMethod)
    {
        await LoginAsync(User);
        clientModel = await FetchClientDetails(clientName);
        clientModel.Should().NotBeNull();
        var nonce = Guid.NewGuid().ToString();
        var codeVerifier = GeneratePkceCodeVerifier();
        FrontChannelClient.AllowAutoRedirect = false;

        var authcodeRequest = CreateAuthorizeRequestUrl(
            clientModel.ClientId,
            responseType,
            scopes,
            responseMode: "query",
            prompt: "none",
            codeChallenge: codeVerifier.GenerateCodeChallenge(),
            codeChallengeMethod: codeChallengeMethod,
            maxAge: "60",
            redirectUri: redirectUri,
            nonce: nonce);

        var returnQuery = await FrontChannelClient.GetAsync(authcodeRequest);
        returnQuery.StatusCode.Should().Be(HttpStatusCode.Found);
        var response = returnQuery.Headers.Location.ToString().ParseQueryString();
        response.Code.Should().NotBeNull(
            "the authorization request should succeed; error={0}, description={1}, location={2}",
            response.ErrorCode,
            response.ErrorDescription,
            returnQuery.Headers.Location);

        var code = response.Code;
        var tokenClient = BackChannelClient;

        var tokenRequest = CreateTokenRequest(
            clientModel.ClientId,
            clientModel.ClientSecret,
            code,
            redirectUri,
            OpenIdConstants.GrantTypes.AuthorizationCode,
            codeVerifier);

        var tokenResponse = await tokenClient.PostAsync(TokenEndpoint, new FormUrlEncodedContent(tokenRequest));
        var tokenResult = await tokenResponse.ParseTokenResponseResult();
        tokenResult.access_token.Should().NotBeNull();
        tokenResult.id_token.Should().NotBeNullOrEmpty();
        tokenResult.refresh_token.Should().NotBeNullOrEmpty();
        tokenResult.expires_in.Should().BeGreaterThan(0);
        tokenResult.token_type.Should().Be(OpenIdConstants.TokenResponseType.BearerTokenType);
        JwksTestHelper.BackChannelClient = BackChannelClient;
        return tokenResult;
    }
}
