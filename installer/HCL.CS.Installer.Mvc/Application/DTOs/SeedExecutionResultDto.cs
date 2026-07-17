namespace HclCsInstallerMVC.Application.DTOs;

public sealed class SeedExecutionResultDto
{
    public bool Succeeded { get; init; }

    public string? ErrorMessage { get; init; }

    public string? GeneratedClientId { get; init; }

    public string? GeneratedClientSecret { get; init; }
}
