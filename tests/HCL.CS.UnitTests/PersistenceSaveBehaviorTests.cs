using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using HCL.CS.Domain;
using HCL.CS.Domain.Entities.Endpoint;
using HCL.CS.Infrastructure.Data;
using Xunit;

namespace HCL.CS.UnitTests;

public class PersistenceSaveBehaviorTests
{
    [Fact]
    public async Task SaveChangesWithHardDelete_DeletesInsteadOfReenteringSoftDeleteSave()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<SqLiteApplicationDbContext>()
            .UseSqlite(connection)
            .Options;
        await using var context = new SqLiteApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();
        var token = new SecurityTokens
        {
            TokenType = "test",
            TokenValue = "test-value",
            CreationTime = DateTime.UtcNow,
            ExpiresAt = 60,
            CreatedOn = DateTime.UtcNow,
            CreatedBy = "Test"
        };

        await context.SecurityTokens.AddAsync(token);
        (await context.SaveChangesAsync()).Status.Should().Be(ResultStatus.Succeeded);
        context.SecurityTokens.Remove(token);

        (await context.SaveChangesWithHardDeleteAsync()).Status.Should().Be(ResultStatus.Succeeded);

        (await context.SecurityTokens.IgnoreQueryFilters().CountAsync()).Should().Be(0);
    }
}
