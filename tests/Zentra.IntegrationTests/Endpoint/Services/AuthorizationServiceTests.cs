using System.Security.Claims;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Zentra.Domain.Models.Endpoint.Request;
using Zentra.Service.Interfaces.Interfaces.Api;
using Zentra.Service.Interfaces.Interfaces.Endpoint;

namespace IntegrationTests.Endpoint.Services;

public class AuthorizeServiceTests : ZentraFakeSetup
{
    private readonly IApiResourceService apiResourceService;
    private readonly IAuthorizationService authorizationService;
    private readonly IIdentityResourceService identityResourceService;

    public AuthorizeServiceTests()
    {
        authorizationService = ServiceProvider.GetService<IAuthorizationService>();
        apiResourceService = ServiceProvider.GetService<IApiResourceService>();
        identityResourceService = ServiceProvider.GetService<IIdentityResourceService>();
    }

    private ClaimsPrincipal GetClaimsPrincipal()
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, "username"),
            new(ClaimTypes.NameIdentifier, "userId"),
            new("name", "John Doe")
        };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        return new ClaimsPrincipal(identity);
    }

    private string ToAssertableString(Dictionary<string, string> dictionary)
    {
        var pairStrings = dictionary.OrderBy(p => p.Key)
            .Select(p => p.Key + ": " + string.Join(", ", p.Value));
        return string.Join("; ", pairStrings);
    }


    public Dictionary<string, string> GetRequestRawData()
    {
        var requestRawData = new Dictionary<string, string>
        {
            { "request_uri", "http://testuri.com/test.aspx" },
            { "request", "" },
            { "client_id", "VALID_CLIENT" },
            { "redirect_uri", "https://localhost:44300/index.html" },
            { "response_type", "token" },
            {
                "code_challenge",
                "test_codechallenge_which_is_greater_than_the_minimum_number_of_characters_required_that_is_43"
            },
            { "code_challenge_method", "S256" },
            { "response_mode", "form_post" },
            { "scope", "client user" },
            { "nonce", "testNonce_valid" },
            { "prompt", "login" },
            { "max_age", "20" }
        };
        return requestRawData;
    }

    [Fact]
    public async Task SaveReturnUrlAsync_SavesRequest_ReturnsANewGuid()
    {
        try
        {
            //Arrange
            var requestModel = new ValidatedAuthorizeRequestModel
            {
                User = GetClaimsPrincipal(),
                PromptModes = new List<string> { "login" },
                RequestRawData = new Dictionary<string, string>
                {
                    { "request_uri", "http://testuri.com/test.aspx" },
                    { "request", "" },
                    { "client_id", "VALID_CLIENT" },
                    { "redirect_uri", "https://localhost:44300/index.html" },
                    { "response_type", "token" },
                    {
                        "code_challenge",
                        "test_codechallenge_which_is_greater_than_the_minimum_number_of_characters_required_that_is_43"
                    },
                    { "code_challenge_method", "S256" },
                    { "response_mode", "form_post" },
                    { "scope", "client user" },
                    { "nonce", "testNonce_valid" },
                    { "prompt", "login" },
                    { "max_age", "20" }
                }
            };

            //Act.
            var result = await authorizationService.SaveReturnUrlAsync(requestModel);
            var dict = await authorizationService.ValidateReturnUrlAsync(result.ToString());

            //Assert
            result.Should().NotBeEmpty();
            dict.Should().NotBeEmpty();
            ToAssertableString(requestModel.RequestRawData).Should().BeEquivalentTo(ToAssertableString(dict));
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task SaveVerificationCodeAsync_ReturnsSuccess()
    {
        try
        {
            //Arrange
            var code = "TEST_VERIFICATION_CODE";

            //Act
            var verificationCode = await authorizationService.SaveVerificationCodeAsync(code);
            var token = await authorizationService.ValidateVerificationCodeAsync(verificationCode);

            //Assert
            verificationCode.Should().NotBeNull();
            token.TokenValue.Should().Be(code);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}
