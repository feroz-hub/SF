using Npgsql;
using HclCsInstallerMVC.Application.Abstractions;
using HclCsInstallerMVC.Application.DTOs;

namespace HclCsInstallerMVC.Infrastructure.Services.DatabaseProvisioning;

public sealed class PostgreSqlProvisioner : IDatabaseProvisioner
{
    private readonly bool _allowDatabaseCreation;
    private readonly ILogger<PostgreSqlProvisioner> _logger;

    public PostgreSqlProvisioner(ILogger<PostgreSqlProvisioner> logger, bool allowDatabaseCreation)
    {
        _logger = logger;
        _allowDatabaseCreation = allowDatabaseCreation;
    }

    public async Task EnsureDatabaseExistsAsync(DatabaseConfigurationDto configuration,
        CancellationToken cancellationToken)
    {
        var targetBuilder = new NpgsqlConnectionStringBuilder(configuration.ConnectionString);
        if (string.IsNullOrWhiteSpace(targetBuilder.Database))
            throw new InvalidOperationException("Database name is required in PostgreSQL connection string.");

        var targetDatabase = targetBuilder.Database;
        var adminBuilder = new NpgsqlConnectionStringBuilder(configuration.ConnectionString)
        {
            Database = "postgres"
        };

        _logger.LogInformation(
            "Ensuring PostgreSQL database exists. Database: {Database}. AllowDatabaseCreation: {AllowDatabaseCreation}",
            targetDatabase,
            _allowDatabaseCreation);

        await using var connection = new NpgsqlConnection(adminBuilder.ConnectionString);
        await connection.OpenAsync(cancellationToken);

        await using var existsCommand = new NpgsqlCommand(
            "SELECT 1 FROM pg_database WHERE datname = @databaseName;",
            connection);
        existsCommand.Parameters.AddWithValue("databaseName", targetDatabase);

        var exists = await existsCommand.ExecuteScalarAsync(cancellationToken) is not null;
        if (exists)
        {
            _logger.LogInformation("PostgreSQL database already exists. Database: {Database}", targetDatabase);
            return;
        }

        if (!_allowDatabaseCreation)
            throw new InvalidOperationException(
                $"Database '{targetDatabase}' does not exist and automatic creation is disabled.");

        var escapedIdentifier = targetDatabase.Replace("\"", "\"\"", StringComparison.Ordinal);
        await using var createCommand = new NpgsqlCommand($"CREATE DATABASE \"{escapedIdentifier}\";", connection);
        await createCommand.ExecuteNonQueryAsync(cancellationToken);

        _logger.LogInformation("Created PostgreSQL database. Database: {Database}", targetDatabase);
    }
}
