namespace HCL.CS.DemoClientMvc.ViewModels.Home;

public sealed class HomeIndexViewModel
{
    public bool IsAuthenticated { get; set; }

    public string UserName { get; set; } = string.Empty;

    public IReadOnlyCollection<string> Roles { get; set; } = new List<string>();
}
