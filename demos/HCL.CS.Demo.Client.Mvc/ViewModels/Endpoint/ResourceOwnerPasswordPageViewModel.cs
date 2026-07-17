using System.ComponentModel.DataAnnotations;

namespace HCL.CS.DemoClientMvc.ViewModels.Endpoint;

public sealed class ResourceOwnerPasswordPageViewModel
{
    [Required]
    [Display(Name = "Username")]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    public EndpointTokenFlowViewModel Result { get; set; } = new();
}
