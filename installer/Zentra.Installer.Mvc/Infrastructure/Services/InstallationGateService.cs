using System.Text.Json;
using Microsoft.Extensions.Options;
using ZentraInstallerMVC.Application.Abstractions;
using ZentraInstallerMVC.Application.DTOs;
using ZentraInstallerMVC.Infrastructure.Configuration;

namespace ZentraInstallerMVC.Infrastructure.Services;

public sealed class InstallationGateService : IInstallationGateService
{
    private readonly IWebHostEnvironment _environment;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly InstallerLockOptions _options;

    public InstallationGateService(IWebHostEnvironment environment, IOptions<InstallerLockOptions> options)
    {
        _environment = environment;
        _options = options.Value;
        _jsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            WriteIndented = true
        };
    }

    public Task<bool> IsInstallationCompletedAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(File.Exists(GetMarkerPath()));
    }

    public async Task<InstallationCompletionMetadataDto?> GetCompletionMetadataAsync(
        CancellationToken cancellationToken)
    {
        var markerPath = GetMarkerPath();
        if (!File.Exists(markerPath)) return null;

        await using var stream = File.OpenRead(markerPath);
        return await JsonSerializer.DeserializeAsync<InstallationCompletionMetadataDto>(stream, _jsonSerializerOptions,
            cancellationToken);
    }

    public async Task MarkInstallationCompletedAsync(InstallationCompletionMetadataDto metadata,
        CancellationToken cancellationToken)
    {
        var markerPath = GetMarkerPath();
        var directoryPath = Path.GetDirectoryName(markerPath);
        if (!string.IsNullOrWhiteSpace(directoryPath)) Directory.CreateDirectory(directoryPath);

        await using var stream = File.Create(markerPath);
        await JsonSerializer.SerializeAsync(stream, metadata, _jsonSerializerOptions, cancellationToken);
        await stream.FlushAsync(cancellationToken);
    }

    private string GetMarkerPath()
    {
        return Path.IsPathRooted(_options.MarkerFilePath)
            ? _options.MarkerFilePath
            : Path.Combine(_environment.ContentRootPath, _options.MarkerFilePath);
    }
}
