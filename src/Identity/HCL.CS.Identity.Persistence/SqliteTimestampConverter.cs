using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace HCL.CS.Infrastructure.Data;

public sealed class SqliteTimestampConverter : ValueConverter<byte[], byte[]>
{
    public SqliteTimestampConverter()
        : base(
            value => value,
            value => value)
    {
    }
}
