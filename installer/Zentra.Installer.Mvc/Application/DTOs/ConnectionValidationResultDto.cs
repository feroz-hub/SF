namespace ZentraInstallerMVC.Application.DTOs;

public sealed class ConnectionValidationResultDto
{
    public bool Succeeded { get; init; }

    public string? ErrorMessage { get; init; }
}
