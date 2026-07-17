using Microsoft.Data.Sqlite;
using HclCsInstallerMVC.Application.Abstractions;
using HclCsInstallerMVC.Application.DTOs;

namespace HclCsInstallerMVC.Infrastructure.Services.DatabaseProvisioning;

public sealed class SqliteProvisioner : IDatabaseProvisioner
{
    private readonly ILogger<SqliteProvisioner> _logger;

    public SqliteProvisioner(ILogger<SqliteProvisioner> logger)
    {
        _logger = logger;
    }

    public Task EnsureDatabaseExistsAsync(DatabaseConfigurationDto configuration, CancellationToken cancellationToken)
    {
        var builder = new SqliteConnectionStringBuilder(configuration.ConnectionString);
        _logger.LogInformation(
            "SQLite provisioning does not require explicit database creation. DataSource: {DataSource}",
            builder.DataSource);

        return Task.CompletedTask;
    }
}
