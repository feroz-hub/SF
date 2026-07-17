using System.Data;
using Microsoft.Data.SqlClient;
using HclCsInstallerMVC.Application.Abstractions;
using HclCsInstallerMVC.Application.DTOs;

namespace HclCsInstallerMVC.Infrastructure.Services.DatabaseProvisioning;

public sealed class SqlServerProvisioner : IDatabaseProvisioner
{
    private readonly bool _allowDatabaseCreation;
    private readonly ILogger<SqlServerProvisioner> _logger;

    public SqlServerProvisioner(ILogger<SqlServerProvisioner> logger, bool allowDatabaseCreation)
    {
        _logger = logger;
        _allowDatabaseCreation = allowDatabaseCreation;
    }

    public async Task EnsureDatabaseExistsAsync(DatabaseConfigurationDto configuration,
        CancellationToken cancellationToken)
    {
        var targetBuilder = new SqlConnectionStringBuilder(configuration.ConnectionString);
        if (string.IsNullOrWhiteSpace(targetBuilder.InitialCatalog))
            throw new InvalidOperationException("Database name is required in SQL Server connection string.");

        var targetDatabase = targetBuilder.InitialCatalog;
        var adminBuilder = new SqlConnectionStringBuilder(configuration.ConnectionString)
        {
            InitialCatalog = "master"
        };

        _logger.LogInformation(
            "Ensuring SQL Server database exists. Database: {Database}. AllowDatabaseCreation: {AllowDatabaseCreation}",
            targetDatabase,
            _allowDatabaseCreation);

        await using var connection = new SqlConnection(adminBuilder.ConnectionString);
        await connection.OpenAsync(cancellationToken);

        await using var existsCommand = connection.CreateCommand();
        existsCommand.CommandText = "SELECT CASE WHEN DB_ID(@databaseName) IS NULL THEN 0 ELSE 1 END;";
        existsCommand.Parameters.Add(new SqlParameter("@databaseName", SqlDbType.NVarChar, 128)
            { Value = targetDatabase });

        var existsResult = await existsCommand.ExecuteScalarAsync(cancellationToken);
        var exists = existsResult is int existsValue && existsValue == 1;
        if (exists)
        {
            _logger.LogInformation("SQL Server database already exists. Database: {Database}", targetDatabase);
            return;
        }

        if (!_allowDatabaseCreation)
            throw new InvalidOperationException(
                $"Database '{targetDatabase}' does not exist and automatic creation is disabled.");

        await using var createCommand = connection.CreateCommand();
        createCommand.CommandText = """
                                    IF DB_ID(@databaseName) IS NULL
                                    BEGIN
                                        DECLARE @sql nvarchar(max) = N'CREATE DATABASE ' + QUOTENAME(@databaseName);
                                        EXEC (@sql);
                                    END
                                    """;
        createCommand.Parameters.Add(new SqlParameter("@databaseName", SqlDbType.NVarChar, 128)
            { Value = targetDatabase });
        await createCommand.ExecuteNonQueryAsync(cancellationToken);

        _logger.LogInformation("Created SQL Server database. Database: {Database}", targetDatabase);
    }
}
