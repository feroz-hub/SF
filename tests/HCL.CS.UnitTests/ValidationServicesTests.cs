/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using System.Threading.Tasks;
using FluentAssertions;
using HCL.CS.Infrastructure.Data;
using HCL.CS.Infrastructure.Data.Validation;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace HCL.CS.UnitTests;

public class ValidationServicesTests
{
    [Fact]
    public async Task DuplicateEmailPreflightService_WhenPhase2BNotPending_ReturnsPassed()
    {
        var options = new DbContextOptionsBuilder<SqLiteApplicationDbContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;

        using var dbContext = new SqLiteApplicationDbContext(options);
        await dbContext.Database.OpenConnectionAsync();
        await dbContext.Database.EnsureCreatedAsync();

        var service = new DuplicateEmailPreflightService(dbContext);
        var result = await service.CheckPreflightAsync();

        result.Should().NotBeNull();
        result.Pass.Should().BeTrue();
        result.DuplicatesFound.Should().BeFalse();
    }

    [Fact]
    public async Task RuntimeSchemaCompatibilityService_CanQueryCompatibility()
    {
        var options = new DbContextOptionsBuilder<SqLiteApplicationDbContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;

        using var dbContext = new SqLiteApplicationDbContext(options);
        await dbContext.Database.OpenConnectionAsync();
        await dbContext.Database.EnsureCreatedAsync();

        var service = new RuntimeSchemaCompatibilityService(dbContext);
        var report = await service.ValidateCompatibilityAsync();

        report.Should().NotBeNull();
        report.ConnectivityStatus.Should().BeTrue();
        report.Provider.Should().Contain("Sqlite");
    }

    [Fact]
    public async Task DatabaseReconciliationService_DryRun_Succeeds()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;

        using var dbContext = new ApplicationDbContext(options);
        await dbContext.Database.OpenConnectionAsync();
        await dbContext.Database.EnsureCreatedAsync();

        var service = new DatabaseReconciliationService(dbContext);
        var result = await service.ReconcileMigrationHistoryAsync(dryRun: true);

        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.VerificationLogs.Should().Contain(l => l.Contains("Provider"));
    }
}
