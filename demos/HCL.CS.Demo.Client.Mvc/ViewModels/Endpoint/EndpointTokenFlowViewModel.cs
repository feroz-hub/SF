using HCL.CS.DemoClientMvc.ViewModels.Shared;

namespace HCL.CS.DemoClientMvc.ViewModels.Endpoint;

public sealed class EndpointTokenFlowViewModel
{
    public bool Succeeded { get; set; }

    public int ExpiresIn { get; set; }

    public string? TokenType { get; set; }

    public string? ErrorCode { get; set; }

    public string? ErrorDescription { get; set; }

    public IReadOnlyCollection<ClaimItemViewModel> Claims { get; set; } = new List<ClaimItemViewModel>();
}
