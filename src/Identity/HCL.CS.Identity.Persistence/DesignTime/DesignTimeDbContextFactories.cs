/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace HCL.CS.Infrastructure.Data.DesignTime;

public sealed class PostgreSqlApplicationDbcontextFactory
    : IDesignTimeDbContextFactory<PostgreSqlApplicationDbcontext>
{
    public PostgreSqlApplicationDbcontext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<PostgreSqlApplicationDbcontext>();
        var connectionString =
            Environment.GetEnvironmentVariable("HCL_CS_DESIGN_TIME_CONNECTION")
            ?? Environment.GetEnvironmentVariable("HCL_CS_MIGRATION_POSTGRES_CONNECTION")
            ?? "Host=localhost;Port=5432;Database=hcl_cs_design_time;Username=hcl_cs;Password=design-time-only";

        optionsBuilder.UseNpgsql(
            connectionString,
            provider => provider.MigrationsAssembly(typeof(PostgreSqlApplicationDbcontext).Assembly.FullName));

        return new PostgreSqlApplicationDbcontext(optionsBuilder.Options);
    }
}

public sealed class SqLiteApplicationDbContextFactory
    : IDesignTimeDbContextFactory<SqLiteApplicationDbContext>
{
    public SqLiteApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<SqLiteApplicationDbContext>();
        var connectionString =
            Environment.GetEnvironmentVariable("HCL_CS_MIGRATION_SQLITE_CONNECTION")
            ?? "Data Source=hcl_cs_design_time.db";

        optionsBuilder.UseSqlite(
            connectionString,
            provider => provider.MigrationsAssembly(typeof(SqLiteApplicationDbContext).Assembly.FullName));

        return new SqLiteApplicationDbContext(optionsBuilder.Options);
    }
}

public sealed class MySqlApplicationDbContextFactory
    : IDesignTimeDbContextFactory<MySqlApplicationDbContext>
{
    public MySqlApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<MySqlApplicationDbContext>();
        var connectionString =
            Environment.GetEnvironmentVariable("HCL_CS_MIGRATION_MYSQL_CONNECTION")
            ?? "Server=localhost;Database=hcl_cs_design_time;Uid=root;Pwd=not-used";

        optionsBuilder.UseMySql(
            connectionString,
            new MySqlServerVersion(new Version(8, 0, 30)),
            provider => provider.MigrationsAssembly(typeof(MySqlApplicationDbContext).Assembly.FullName));

        return new MySqlApplicationDbContext(optionsBuilder.Options);
    }
}

public sealed class SqlServerApplicationDbContextFactory
    : IDesignTimeDbContextFactory<SqlServerApplicationDbContext>
{
    public SqlServerApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<SqlServerApplicationDbContext>();
        var connectionString =
            Environment.GetEnvironmentVariable("HCL_CS_MIGRATION_SQLSERVER_CONNECTION")
            ?? "Server=localhost;Database=hcl_cs_design_time;User Id=sa;Password=not-used;Encrypt=False";

        optionsBuilder.UseSqlServer(
            connectionString,
            provider => provider.MigrationsAssembly(typeof(SqlServerApplicationDbContext).Assembly.FullName));

        return new SqlServerApplicationDbContext(optionsBuilder.Options);
    }
}
