using System.ComponentModel.DataAnnotations;

namespace HCL.CS.DemoServerApp.Models;

public class ForgetPasswordViewModel
{
    [Required]
    [Display(Name = "UserName")]
    public string UserName { get; set; }

    public string? ReturnUrl { get; set; }
}

public class ResetPasswordViewModel
{
    [Required] public string Token { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
        MinimumLength = 6)]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }

    public string? ReturnUrl { get; set; }
}
