/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

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
