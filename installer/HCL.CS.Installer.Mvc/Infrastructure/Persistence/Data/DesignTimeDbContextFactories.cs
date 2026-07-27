/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace HclCsInstallerMVC.Infrastructure.Persistence.Data;

public sealed class PostgreSqlDesignTimeDbContextFactory
    : IDesignTimeDbContextFactory<PostgreSqlApplicationDbcontext>
{
    public PostgreSqlApplicationDbcontext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable("HCL_CS_MIGRATION_POSTGRES_CONNECTION")
            ?? "Host=localhost;Database=hcl-cs-design;Username=hcl-cs;Password=not-used";
        var options = new DbContextOptionsBuilder<PostgreSqlApplicationDbcontext>()
            .UseNpgsql(connectionString)
            .Options;
        return new PostgreSqlApplicationDbcontext(options);
    }
}

public sealed class SqliteDesignTimeDbContextFactory
    : IDesignTimeDbContextFactory<SqLiteApplicationDbContext>
{
    public SqLiteApplicationDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable("HCL_CS_MIGRATION_SQLITE_CONNECTION")
            ?? "Data Source=hcl-cs-design.db";
        var options = new DbContextOptionsBuilder<SqLiteApplicationDbContext>()
            .UseSqlite(connectionString)
            .Options;
        return new SqLiteApplicationDbContext(options);
    }
}
