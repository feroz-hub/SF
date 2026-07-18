/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using Microsoft.EntityFrameworkCore;
using HclCsInstallerMVC.Application.DTOs;
using HclCsInstallerMVC.Infrastructure.Persistence.Data;

namespace HclCsInstallerMVC.Infrastructure.Services;

internal static class DatabaseProviderUtilities
{
    public static DbContextOptions<ApplicationDbContext> BuildApplicationOptions(DatabaseConfigurationDto configuration)
    {
        var builder = new DbContextOptionsBuilder<ApplicationDbContext>();

        switch (configuration.Provider)
        {
            case DatabaseProviderType.SqlServer:
                builder.UseSqlServer(configuration.ConnectionString);
                break;
            case DatabaseProviderType.MySql:
                builder.UseMySql(configuration.ConnectionString,
                    ServerVersion.AutoDetect(configuration.ConnectionString));
                break;
            case DatabaseProviderType.PostgreSql:
                builder.UseNpgsql(configuration.ConnectionString);
                break;
            case DatabaseProviderType.Sqlite:
                builder.UseSqlite(configuration.ConnectionString);
                break;
            default:
                throw new InvalidOperationException($"Unsupported database provider: {configuration.Provider}");
        }

        return builder.Options;
    }

    public static DbContext CreateMigrationDbContext(DatabaseConfigurationDto configuration)
    {
        return configuration.Provider switch
        {
            DatabaseProviderType.SqlServer => CreateSqlServerMigrationDbContext(configuration.ConnectionString),
            DatabaseProviderType.MySql => CreateMySqlMigrationDbContext(configuration.ConnectionString),
            DatabaseProviderType.PostgreSql => CreatePostgreSqlMigrationDbContext(configuration.ConnectionString),
            DatabaseProviderType.Sqlite => CreateSqliteMigrationDbContext(configuration.ConnectionString),
            _ => throw new InvalidOperationException($"Unsupported database provider: {configuration.Provider}")
        };
    }

    private static SqlServerApplicationDbContext CreateSqlServerMigrationDbContext(string connectionString)
    {
        var builder = new DbContextOptionsBuilder<SqlServerApplicationDbContext>();
        builder.UseSqlServer(connectionString);
        return new SqlServerApplicationDbContext(builder.Options);
    }

    private static MySqlApplicationDbContext CreateMySqlMigrationDbContext(string connectionString)
    {
        var builder = new DbContextOptionsBuilder<MySqlApplicationDbContext>();
        builder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        return new MySqlApplicationDbContext(builder.Options);
    }

    private static PostgreSqlApplicationDbcontext CreatePostgreSqlMigrationDbContext(string connectionString)
    {
        var builder = new DbContextOptionsBuilder<PostgreSqlApplicationDbcontext>();
        builder.UseNpgsql(connectionString);
        return new PostgreSqlApplicationDbcontext(builder.Options);
    }

    private static SqLiteApplicationDbContext CreateSqliteMigrationDbContext(string connectionString)
    {
        var builder = new DbContextOptionsBuilder<SqLiteApplicationDbContext>();
        builder.UseSqlite(connectionString);
        return new SqLiteApplicationDbContext(builder.Options);
    }
}
