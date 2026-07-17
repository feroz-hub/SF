using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Zentra.Infrastructure.Data;

public sealed class SqliteTimestampConverter : ValueConverter<byte[], byte[]>
{
    public SqliteTimestampConverter()
        : base(
            value => value,
            value => value)
    {
    }
}
