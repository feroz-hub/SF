using System.ComponentModel.DataAnnotations;

namespace HCL.CS.DemoClientMvc.ViewModels.Account;

public sealed class ResetPasswordViewModel
{
    [Required]
    [Display(Name = "Username")]
    public string UserName { get; set; } = string.Empty;

    [Display(Name = "Reset Token")] public string ResetToken { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Display(Name = "New Password")]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
    public string NewPassword { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Display(Name = "Confirm Password")]
    [Compare(nameof(NewPassword), ErrorMessage = "Password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;

    public bool TokenIssued { get; set; }
}
