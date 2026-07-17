namespace Zentra.Domain.Models.Api.Response;

public class AuthenticatorAppSetupResponseModel
{
    public string SharedKey { get; set; }

    public string AuthenticatorUri { get; set; }

    public string VerificationCode { get; set; }
}
