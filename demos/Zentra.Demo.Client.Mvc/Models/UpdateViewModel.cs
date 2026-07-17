using System.ComponentModel.DataAnnotations;

namespace Zentra.DemoClientMvc.Models;

public class UpdateViewModel
{
    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; }

    [Required]
    [Phone]
    [Display(Name = "PhoneNumber")]
    public string PhoneNumber { get; set; }

    [Required]
    [Display(Name = "FirstName")]
    public string FirstName { get; set; }

    [Display(Name = "LastName")] public string LastName { get; set; }

    public string ReturnUrl { get; set; }
}
