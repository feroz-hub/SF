/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using MySql.Data.MySqlClient;
using HclCsInstallerMVC.Application.Abstractions;
using HclCsInstallerMVC.Application.DTOs;

namespace HclCsInstallerMVC.Infrastructure.Services.DatabaseProvisioning;

public sealed class MySqlProvisioner : IDatabaseProvisioner
{
    private readonly bool _allowDatabaseCreation;
    private readonly ILogger<MySqlProvisioner> _logger;

    public MySqlProvisioner(ILogger<MySqlProvisioner> logger, bool allowDatabaseCreation)
    {
        _logger = logger;
        _allowDatabaseCreation = allowDatabaseCreation;
    }

    public async Task EnsureDatabaseExistsAsync(DatabaseConfigurationDto configuration,
        CancellationToken cancellationToken)
    {
        var targetBuilder = new MySqlConnectionStringBuilder(configuration.ConnectionString);
        if (string.IsNullOrWhiteSpace(targetBuilder.Database))
            throw new InvalidOperationException("Database name is required in MySQL connection string.");

        var targetDatabase = targetBuilder.Database;
        var adminBuilder = new MySqlConnectionStringBuilder(configuration.ConnectionString)
        {
            Database = string.Empty
        };

        _logger.LogInformation(
            "Ensuring MySQL database exists. Database: {Database}. AllowDatabaseCreation: {AllowDatabaseCreation}",
            targetDatabase,
            _allowDatabaseCreation);

        await using var connection = new MySqlConnection(adminBuilder.ConnectionString);
        await connection.OpenAsync(cancellationToken);

        await using var existsCommand = connection.CreateCommand();
        existsCommand.CommandText = """
                                    SELECT SCHEMA_NAME
                                    FROM INFORMATION_SCHEMA.SCHEMATA
                                    WHERE SCHEMA_NAME = @databaseName
                                    LIMIT 1;
                                    """;
        existsCommand.Parameters.AddWithValue("@databaseName", targetDatabase);

        var exists = await existsCommand.ExecuteScalarAsync(cancellationToken) is not null;
        if (exists)
        {
            _logger.LogInformation("MySQL database already exists. Database: {Database}", targetDatabase);
            return;
        }

        if (!_allowDatabaseCreation)
            throw new InvalidOperationException(
                $"Database '{targetDatabase}' does not exist and automatic creation is disabled.");

        var escapedIdentifier = targetDatabase.Replace("`", "``", StringComparison.Ordinal);
        await using var createCommand = connection.CreateCommand();
        createCommand.CommandText = $"CREATE DATABASE IF NOT EXISTS `{escapedIdentifier}`;";
        await createCommand.ExecuteNonQueryAsync(cancellationToken);

        _logger.LogInformation("Created MySQL database. Database: {Database}", targetDatabase);
    }
}
