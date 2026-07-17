using System.ComponentModel.DataAnnotations;

namespace Zentra.DemoClientMvc.Models;

public class SetupAuthenticatorViewModel
{
    public string SharedKey { get; set; }
    public string AuthenticatorUri { get; set; }

    [Required] public string VerificationCode { get; set; }
}
