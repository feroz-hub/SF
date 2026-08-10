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
using HCL.CS.Domain.ErrorCodes;
using HCL.CS.Service.Implementation.Endpoint.Extensions;

namespace IntegrationTests.Endpoint.FlowTests;

/// <summary>
///     Hybrid and implicit response types are deliberately unsupported. HCL.CS exposes the
///     authorization-code flow with S256 PKCE instead of returning tokens on the front channel.
/// </summary>
public class HybridFlowTests : HclCsFakeSetup
{
    private const string StrictPkceClient = "HCL.CS S256 Client";
    private const string RedirectUri = "https://127.0.0.1:63562/";

    [Theory]
    [InlineData("code id_token", "query")]
    [InlineData("code id_token", "form_post")]
    [InlineData("code token", "query")]
    [InlineData("code token", "form_post")]
    [InlineData("code id_token token", "query")]
    [InlineData("code id_token token", "form_post")]
    public async Task HybridAndImplicitResponseTypes_AreRejected(string responseType, string responseMode)
    {
        await LoginAsync(User);
        var client = await FetchClientDetails(StrictPkceClient);
        client.Should().NotBeNull();

        var codeVerifier = GeneratePkceCodeVerifier();
        FrontChannelClient.AllowAutoRedirect = false;
        var request = CreateAuthorizeRequestUrl(
            client.ClientId,
            responseType,
            "openid email profile phone",
            responseMode: responseMode,
            prompt: "none",
            codeChallenge: codeVerifier.GenerateCodeChallenge(),
            codeChallengeMethod: OpenIdConstants.CodeChallengeMethods.Sha256,
            maxAge: "60",
            redirectUri: RedirectUri,
            nonce: Guid.NewGuid().ToString("N"));

        var response = await FrontChannelClient.GetAsync(request);
        var error = await response.Headers.Location.ToString().ParseErrorQueryStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.Found);
        error.IsError.Should().BeTrue();
        error.ErrorCode.Should().Be(OpenIdConstants.Errors.UnsupportedResponseType);
        error.ErrorDescription.Should().Be(
            ResourceStringHandler.GetResourceString(EndpointErrorCodes.ResponseTypeMissing));
        error.ClientId.Should().Be(client.ClientId);
        error.TraceId.Should().NotBeNullOrWhiteSpace();
    }
}
