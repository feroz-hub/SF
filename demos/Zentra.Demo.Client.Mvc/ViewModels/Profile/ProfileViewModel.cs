using Zentra.DemoClientMvc.ViewModels.Shared;

namespace Zentra.DemoClientMvc.ViewModels.Profile;

public sealed class ProfileViewModel
{
    public string UserName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string SubjectId { get; set; } = string.Empty;

    public IReadOnlyCollection<string> Roles { get; set; } = new List<string>();

    public IReadOnlyCollection<ClaimItemViewModel> Claims { get; set; } = new List<ClaimItemViewModel>();
}
