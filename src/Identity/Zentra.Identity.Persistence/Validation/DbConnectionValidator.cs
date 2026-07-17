using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using MySqlConnector;
using Npgsql;
using Zentra.Domain;
using Zentra.DomainServices.Infra;

namespace Zentra.Infrastructure.Data.Validation;

public class DbConnectionValidator : IDbConnectionValidator
{
    public Exception? Validate(DbTypes databaseType, string connectionString)
    {
        try
        {
            if (databaseType == DbTypes.SqlServer)
            {
                using var conn = new SqlConnection(connectionString);
                if (conn.State != ConnectionState.Open) conn.Open();
            }
            else if (databaseType == DbTypes.MySql)
            {
                using var conn = new MySqlConnection(connectionString);
                conn.Open();
            }
            else if (databaseType == DbTypes.PostgreSQL)
            {
                using var conn = new NpgsqlConnection(connectionString);
                conn.Open();
            }
            else if (databaseType == DbTypes.SQLite)
            {
                using var conn = new SqliteConnection(connectionString);
                conn.Open();
            }
        }
        catch (Exception ex)
        {
            return ex;
        }

        return null;
    }
}
