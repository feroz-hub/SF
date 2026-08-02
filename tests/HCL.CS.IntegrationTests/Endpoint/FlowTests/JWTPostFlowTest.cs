/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using FluentAssertions;
using IntegrationTests.ApiDomainModel;
using IntegrationTests.Endpoint.Setup;
using Microsoft.IdentityModel.Tokens;
using Xunit;
using HCL.CS.Domain.Constants.Endpoint;
using HCL.CS.Service.Implementation.Endpoint.Extensions;
using static HCL.CS.Domain.Constants.Endpoint.OpenIdConstants;

namespace IntegrationTests.Endpoint.FlowTests;

public class JWTPostFlowTest : HclCsFakeSetup
{
    private const string PositiveCaseClientName = "HCL.CS Plain PKCE Client";
    private const string RedirectUri = "https://127.0.0.1:63562/";

    public static IEnumerable<object[]> RejectedClientAssertionScenarios()
    {
        foreach (var scenario in Enum.GetValues<ClientAssertionScenario>()) yield return new object[] { scenario };
    }

    [Theory]
    [MemberData(nameof(RejectedClientAssertionScenarios))]
    public async Task ConfidentialClient_ClientSecretJwt_IsRejectedBeforeGrantValidation(
        ClientAssertionScenario scenario)
    {
        await LoginAsync(User);
        var client = await FetchClientDetails(PositiveCaseClientName);
        client.Should().NotBeNull();

        var codeVerifier = GeneratePkceCodeVerifier();
        FrontChannelClient.AllowAutoRedirect = false;
        var authorizeRequest = CreateAuthorizeRequestUrl(
            client.ClientId,
            ResponseTypes.Code,
            "openid email profile phone",
            responseMode: ResponseModes.Query,
            prompt: "none",
            codeChallenge: codeVerifier.GenerateCodeChallenge(),
            codeChallengeMethod: "S256",
            maxAge: "60",
            redirectUri: RedirectUri,
            nonce: Guid.NewGuid().ToString());
        var authorizeResponse = await FrontChannelClient.GetAsync(authorizeRequest);
        var authorizeResult = authorizeResponse.Headers.Location!.ToString().ParseQueryString();
        authorizeResult.Code.Should().NotBeNullOrWhiteSpace();

        var assertionClient = new ClientsModel
        {
            ClientId = client.ClientId,
            ClientSecret = client.ClientSecret,
            AccessTokenExpiration = client.AccessTokenExpiration
        };
        if (scenario == ClientAssertionScenario.InvalidClientId)
            assertionClient.ClientId += "-unknown";
        else if (scenario == ClientAssertionScenario.InvalidClientSecret)
            assertionClient.ClientSecret += "-invalid";

        var tokenRequest = CreateTokenRequest(
            code: authorizeResult.Code,
            redirectUri: RedirectUri,
            grantType: GrantTypes.AuthorizationCode,
            codeVerifier: codeVerifier,
            clientAssertionType: ClientAssertionTypes.JwtBearer,
            clientAssertion: GenerateJwtSecretRequest(assertionClient));

        ApplyScenario(tokenRequest, scenario);

        var tokenResponse = await BackChannelClient.PostAsync(
            TokenEndpoint,
            new FormUrlEncodedContent(tokenRequest));
        var tokenError = await tokenResponse.ParseTokenErrorResponse();

        tokenResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        tokenError.ErrorCode.Should().Be(Errors.InvalidClient);
    }

    private static string GenerateJwtSecretRequest(ClientsModel client)
    {
        var now = DateTime.UtcNow;
        var securityKey = Encoding.ASCII.GetBytes(client.ClientSecret);
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(securityKey),
            Algorithms.HmacSha256);
        var claims = new List<Claim>
        {
            new("sub", client.ClientId),
            new("iat", now.ToUnixTime().ToString()),
            new("jti", AuthenticationConstants.KeySize32.RandomString())
        };
        var jwt = new JwtSecurityToken(
            client.ClientId,
            TokenEndpoint,
            claims,
            now,
            now.AddMinutes(5),
            credentials);
        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }

    private static void ApplyScenario(
        IDictionary<string, string> tokenRequest,
        ClientAssertionScenario scenario)
    {
        switch (scenario)
        {
            case ClientAssertionScenario.MissingCode:
                tokenRequest.Remove(TokenRequest.Code);
                break;
            case ClientAssertionScenario.InvalidCode:
                tokenRequest[TokenRequest.Code] += "-invalid";
                break;
            case ClientAssertionScenario.MissingRedirectUri:
                tokenRequest.Remove(TokenRequest.RedirectUri);
                break;
            case ClientAssertionScenario.InvalidRedirectUri:
                tokenRequest[TokenRequest.RedirectUri] += "invalid";
                break;
            case ClientAssertionScenario.MissingGrantType:
                tokenRequest.Remove(TokenRequest.GrantType);
                break;
            case ClientAssertionScenario.InvalidGrantType:
                tokenRequest[TokenRequest.GrantType] = "unsupported_grant";
                break;
            case ClientAssertionScenario.MissingCodeVerifier:
                tokenRequest.Remove(TokenRequest.CodeVerifier);
                break;
            case ClientAssertionScenario.InvalidCodeVerifier:
                tokenRequest[TokenRequest.CodeVerifier] += "-invalid";
                break;
            case ClientAssertionScenario.MissingAssertionType:
                tokenRequest.Remove(TokenRequest.ClientAssertionType);
                break;
            case ClientAssertionScenario.InvalidAssertionType:
                tokenRequest[TokenRequest.ClientAssertionType] = "unsupported_assertion_type";
                break;
        }
    }

    public enum ClientAssertionScenario
    {
        ValidAssertion,
        MissingCode,
        InvalidCode,
        MissingRedirectUri,
        InvalidRedirectUri,
        MissingGrantType,
        InvalidGrantType,
        MissingCodeVerifier,
        InvalidCodeVerifier,
        MissingAssertionType,
        InvalidAssertionType,
        InvalidClientId,
        InvalidClientSecret
    }
}
