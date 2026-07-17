using HCL.CS.Domain;

namespace HCL.CS.DomainServices.Infra;

public interface IDbConnectionValidator
{
    Exception? Validate(DbTypes databaseType, string connectionString);
}
