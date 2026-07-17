namespace Zentra.Domain.Models.Api.Response;

public class AuthenticatorAppResponseModel
{
    public bool Succeeded { get; set; } = false;

    public string Message { get; set; }

    public IEnumerable<string> RecoveryCodes { get; set; }
}
