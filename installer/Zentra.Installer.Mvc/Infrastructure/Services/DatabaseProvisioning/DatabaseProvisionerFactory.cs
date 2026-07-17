using Microsoft.Extensions.Logging.Abstractions;
using ZentraInstallerMVC.Application.Abstractions;

namespace ZentraInstallerMVC.Infrastructure.Services.DatabaseProvisioning;

public static class DatabaseProvisionerFactory
{
    public static IDatabaseProvisioner Create(string provider)
    {
        return Create(provider, NullLoggerFactory.Instance, true);
    }

    public static IDatabaseProvisioner Create(string provider, ILoggerFactory loggerFactory, bool allowDatabaseCreation)
    {
        var normalizedProvider = (provider ?? string.Empty).Trim().ToUpperInvariant();

        return normalizedProvider switch
        {
            "POSTGRESQL" or "POSTGRESSQL" or "POSTGRES" =>
                new PostgreSqlProvisioner(loggerFactory.CreateLogger<PostgreSqlProvisioner>(), allowDatabaseCreation),
            "SQLSERVER" =>
                new SqlServerProvisioner(loggerFactory.CreateLogger<SqlServerProvisioner>(), allowDatabaseCreation),
            "MYSQL" =>
                new MySqlProvisioner(loggerFactory.CreateLogger<MySqlProvisioner>(), allowDatabaseCreation),
            "SQLITE" =>
                new SqliteProvisioner(loggerFactory.CreateLogger<SqliteProvisioner>()),
            _ => throw new NotSupportedException($"Database provider '{provider}' is not supported for provisioning.")
        };
    }
}
