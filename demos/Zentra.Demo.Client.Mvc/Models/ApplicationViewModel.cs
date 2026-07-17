using System.ComponentModel.DataAnnotations;

namespace Zentra.DemoClientMvc.Models;

public class ApplicationViewModel
{
}

public class RopViewModel(string userName)
{
    [Required]
    [Display(Name = "UserName")]
    public string UserName { get; set; } = userName;

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; }
}
