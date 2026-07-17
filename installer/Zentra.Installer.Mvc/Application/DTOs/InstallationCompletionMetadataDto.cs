namespace ZentraInstallerMVC.Application.DTOs;

public sealed class InstallationCompletionMetadataDto
{
    public DateTimeOffset CompletedOnUtc { get; init; }

    public string? DatabaseProvider { get; init; }

    public string? ClientId { get; init; }

    public string? ClientSecret { get; init; }
}
