/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using HCL.CS.Infrastructure.Data;
using HclCsInstallerMVC.Application.Abstractions;
using HclCsInstallerMVC.Application.DTOs;
using HclCsInstallerMVC.Infrastructure.Configuration;
using HclCsInstallerMVC.Infrastructure.Services.DatabaseProvisioning;

namespace HclCsInstallerMVC.Infrastructure.Services;

public sealed class DatabaseMigrationService : IDatabaseMigrationService
{
    private readonly ILogger<DatabaseMigrationService> _logger;
    private readonly ILoggerFactory _loggerFactory;
    private readonly DatabaseProvisioningOptions _provisioningOptions;

    public DatabaseMigrationService(
        ILogger<DatabaseMigrationService> logger,
        ILoggerFactory _loggerFactory,
        IOptions<DatabaseProvisioningOptions> provisioningOptions)
    {
        _logger = logger;
        this._loggerFactory = _loggerFactory;
        _provisioningOptions = provisioningOptions.Value;
    }

    public async Task<ConnectionValidationResultDto> ValidateConnectionAsync(
        DatabaseConfigurationDto configuration,
        CancellationToken cancellationToken)
    {
        try
        {
            var provisioner = CreateProvisioner(configuration);
            await provisioner.EnsureDatabaseExistsAsync(configuration, cancellationToken);

            var options = DatabaseProviderUtilities.BuildApplicationOptions(configuration);
            await using var dbContext = new ApplicationDbContext(options);

            await dbContext.Database.OpenConnectionAsync(cancellationToken);
            await dbContext.Database.CloseConnectionAsync();

            _logger.LogInformation(
                "Database connectivity validated for provider {Provider}.",
                configuration.Provider);

            return new ConnectionValidationResultDto { Succeeded = true };
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Database connection validation failed for provider {Provider}. AllowDatabaseCreation: {AllowDatabaseCreation}.",
                configuration.Provider,
                _provisioningOptions.AllowDatabaseCreation);
            return new ConnectionValidationResultDto
            {
                Succeeded = false,
                ErrorMessage = GetFailureMessage(ex,
                    "Invalid connection string, missing database, or database endpoint is unavailable.")
            };
        }
    }

    public async Task<MigrationExecutionResultDto> RunMigrationsAsync(
        DatabaseConfigurationDto configuration,
        CancellationToken cancellationToken)
    {
        try
        {
            var provisioner = CreateProvisioner(configuration);
            await provisioner.EnsureDatabaseExistsAsync(configuration, cancellationToken);

            await using var migrationContext = DatabaseProviderUtilities.CreateMigrationDbContext(configuration);
            migrationContext.Database.SetCommandTimeout(TimeSpan.FromMinutes(5));

            var preflightService = new HCL.CS.Infrastructure.Data.Validation.DuplicateEmailPreflightService(migrationContext);
            var preflightResult = await preflightService.CheckPreflightAsync(cancellationToken);
            if (!preflightResult.Pass)
            {
                _logger.LogError("Migration preflight failed: {Message}", preflightResult.Message);
                return new MigrationExecutionResultDto
                {
                    Succeeded = false,
                    ErrorMessage = preflightResult.Message
                };
            }

            await migrationContext.Database.MigrateAsync(cancellationToken);

            _logger.LogInformation("Database migrations completed successfully for provider {Provider}.",
                configuration.Provider);

            return new MigrationExecutionResultDto { Succeeded = true };
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Migration execution failed for provider {Provider}. AllowDatabaseCreation: {AllowDatabaseCreation}.",
                configuration.Provider,
                _provisioningOptions.AllowDatabaseCreation);
            return new MigrationExecutionResultDto
            {
                Succeeded = false,
                ErrorMessage = GetFailureMessage(ex,
                    "Migration execution failed. Verify credentials, provider configuration, and database permissions.")
            };
        }
    }

    private IDatabaseProvisioner CreateProvisioner(DatabaseConfigurationDto configuration)
    {
        return DatabaseProvisionerFactory.Create(
            configuration.Provider.ToString(),
            _loggerFactory,
            _provisioningOptions.AllowDatabaseCreation);
    }

    private static string GetFailureMessage(Exception exception, string defaultMessage)
    {
        return exception switch
        {
            NotSupportedException => exception.Message,
            InvalidOperationException => exception.Message,
            _ => defaultMessage
        };
    }
}
