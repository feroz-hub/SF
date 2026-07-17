namespace HclCsInstallerMVC.Application.DTOs;

public sealed class MigrationExecutionResultDto
{
    public bool Succeeded { get; init; }

    public string? ErrorMessage { get; init; }
}
