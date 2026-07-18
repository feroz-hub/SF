/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using System.Text.Json;
using Microsoft.Extensions.Options;
using HclCsInstallerMVC.Application.Abstractions;
using HclCsInstallerMVC.Application.DTOs;
using HclCsInstallerMVC.Infrastructure.Configuration;

namespace HclCsInstallerMVC.Infrastructure.Services;

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
