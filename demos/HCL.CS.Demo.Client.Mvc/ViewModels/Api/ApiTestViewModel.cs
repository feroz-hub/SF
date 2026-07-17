namespace HCL.CS.DemoClientMvc.ViewModels.Api;

public sealed class ApiTestViewModel
{
    public string RelativePath { get; set; } = "/health/live";

    public int? StatusCode { get; set; }

    public string? ResponseBody { get; set; }

    public string? ErrorMessage { get; set; }
}
