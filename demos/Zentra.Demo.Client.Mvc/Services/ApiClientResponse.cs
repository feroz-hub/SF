namespace Zentra.DemoClientMvc.Services;

public sealed class ApiClientResponse
{
    public bool Succeeded { get; init; }

    public int StatusCode { get; init; }

    public string? ResponseBody { get; init; }

    public string? ErrorMessage { get; init; }
}
