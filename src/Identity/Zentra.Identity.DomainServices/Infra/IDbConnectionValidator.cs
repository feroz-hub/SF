using Zentra.Domain;

namespace Zentra.DomainServices.Infra;

public interface IDbConnectionValidator
{
    Exception? Validate(DbTypes databaseType, string connectionString);
}
