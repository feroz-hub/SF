using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using HCL.CS.Domain;
using HCL.CS.Domain.Entities.Api;
using HCL.CS.Infrastructure.Data;

namespace IntegrationTests.ExternalAuth;

public class ExternalIdentityPersistenceTests : HclCsFakeSetup
{
    [Fact]
    public async Task ExternalIdentity_UniqueProviderIssuerSubject_MustEnforceUniqueness()
    {
        using var scope = ServiceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var firstUser = await dbContext.Users.FirstAsync();
        var secondUser = await dbContext.Users.OrderBy(user => user.Id).Skip(1).FirstAsync();
        var issuer = "https://accounts.google.com";
        var subject = "google-subject-duplicate";

        dbContext.ExternalIdentities.Add(new ExternalIdentities
        {
            Id = Guid.NewGuid(),
            UserId = firstUser.Id,
            Provider = "Google",
            Issuer = issuer,
            Subject = subject,
            Email = "one@example.com",
            EmailVerified = true,
            TenantId = "tenant-a",
            LinkedAt = DateTime.UtcNow,
            CreatedOn = DateTime.UtcNow,
            CreatedBy = "Test"
        });

        await dbContext.SaveChangesAsync();

        dbContext.ExternalIdentities.Add(new ExternalIdentities
        {
            Id = Guid.NewGuid(),
            UserId = secondUser.Id,
            Provider = "Google",
            Issuer = issuer,
            Subject = subject,
            Email = "two@example.com",
            EmailVerified = true,
            TenantId = "tenant-b",
            LinkedAt = DateTime.UtcNow,
            CreatedOn = DateTime.UtcNow,
            CreatedBy = "Test"
        });

        var result = await dbContext.SaveChangesAsync();

        result.Status.Should().Be(ResultStatus.Failed);
        result.Errors.Should().NotBeNull();

        var duplicateCount = dbContext.ExternalIdentities.Count(identity =>
            identity.Provider == "Google" &&
            identity.Issuer == issuer &&
            identity.Subject == subject);

        duplicateCount.Should().Be(1);
    }
}
