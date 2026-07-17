using HCL.CS.DemoClientMvc.ViewModels.Shared;

namespace HCL.CS.DemoClientMvc.ViewModels.Endpoint;

public sealed class AuthorizeEndpointViewModel
{
    public IReadOnlyCollection<ClaimItemViewModel> Claims { get; set; } = new List<ClaimItemViewModel>();

    public IReadOnlyCollection<ClaimItemViewModel> AuthenticationProperties { get; set; } =
        new List<ClaimItemViewModel>();
}
