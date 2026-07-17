using System.ComponentModel.DataAnnotations;

namespace HCL.CS.DemoServerApp.Models;

public class TwoFactorViewModel
{
    [Required]
    [DataType(DataType.Text)]
    [Display(Name = "Code")]
    public string TwoFactorCode { get; set; }

    public bool RememberMe { get; set; }
    public string? ReturnUrl { get; set; }
}
