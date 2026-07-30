using HclCsInstallerMVC.Application.DTOs;
using HclCsInstallerMVC.Application.Services;
using HclCsInstallerMVC.Infrastructure.Configuration;
using HclCsInstallerMVC.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using HCL.CS.Domain.Enums;
using HCL.CS.Infrastructure.Data;
using HCL.CS.Infrastructure.Data.Validation;

return await DatabaseAdminProgram.RunAsync(args);

internal static class DatabaseAdminProgram
{
    public static async Task<int> RunAsync(string[] args)
    {
        try
        {
            var arguments = OperatorArguments.Parse(args);
            var connectionString = Environment.GetEnvironmentVariable(arguments.ConnectionEnvironmentVariable);
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                Console.Error.WriteLine(
                    $"ERROR: Environment variable '{arguments.ConnectionEnvironmentVariable}' is not configured.");
                return 10;
            }

            return arguments.Command switch
            {
                "migrate" => await RunMigrationsAsync(arguments.Provider, connectionString),
                "reconcile" => await RunReconciliationAsync(arguments, connectionString),
                _ => throw new OperatorArgumentException(
                    "Command must be 'migrate' or 'reconcile'.")
            };
        }
        catch (OperatorArgumentException exception)
        {
            Console.Error.WriteLine($"ERROR: {exception.Message}");
            PrintUsage();
            return 2;
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine($"ERROR: {exception.GetType().Name}: {exception.Message}");
            return 1;
        }
    }

    private static async Task<int> RunMigrationsAsync(
        DatabaseProviderType provider,
        string connectionString)
    {
        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.SetMinimumLevel(LogLevel.Information);
            builder.AddSimpleConsole(options => options.SingleLine = true);
        });
        var service = new DatabaseMigrationService(
            loggerFactory.CreateLogger<DatabaseMigrationService>(),
            loggerFactory,
            Options.Create(new DatabaseProvisioningOptions
            {
                AllowDatabaseCreation = false
            }));
        var configuration = new DatabaseConfigurationDto
        {
            Provider = provider,
            ConnectionString = connectionString
        };

        var validation = await service.ValidateConnectionAsync(configuration, CancellationToken.None);
        Console.WriteLine($"CONNECTION_VALIDATION_SUCCEEDED={validation.Succeeded}");
        if (!validation.Succeeded)
        {
            Console.WriteLine($"MIGRATION_SUCCEEDED=false");
            Console.WriteLine($"MIGRATION_ERROR={validation.ErrorMessage}");
            return 10;
        }

        var migration = await service.RunMigrationsAsync(configuration, CancellationToken.None);
        Console.WriteLine($"MIGRATION_SUCCEEDED={migration.Succeeded}");
        if (!migration.Succeeded)
        {
            Console.WriteLine($"MIGRATION_ERROR={migration.ErrorMessage}");
            return 20;
        }

        return 0;
    }

    private static async Task<int> RunReconciliationAsync(
        OperatorArguments arguments,
        string connectionString)
    {
        await using var context = CreateContext(arguments.Provider, connectionString);
        var service = new DatabaseReconciliationService(context);
        var result = await service.ReconcileMigrationHistoryAsync(
            new ReconciliationRequest
            {
                DryRun = !arguments.Apply,
                Apply = arguments.Apply,
                ConfirmationToken = arguments.ConfirmationToken,
                MigrationIds = arguments.MigrationIds
            });

        Console.WriteLine($"RECONCILIATION_SUCCEEDED={result.Success}");
        Console.WriteLine($"DRY_RUN={result.DryRun}");
        Console.WriteLine($"PROVIDER={result.Provider}");
        Console.WriteLine($"ERROR_CODE={result.ErrorCode}");
        Console.WriteLine($"MISSING_HISTORY_COUNT={result.MissingHistoryMigrations.Count}");
        Console.WriteLine($"DIFFERENCE_COUNT={result.Differences.Count}");
        foreach (var difference in result.Differences)
        {
            Console.WriteLine(
                $"DIFFERENCE={difference.Kind}|{difference.ObjectName}|expected:{difference.Expected}|actual:{difference.Actual}");
        }
        if (!string.IsNullOrWhiteSpace(result.ConfirmationToken))
            Console.WriteLine($"CONFIRMATION_TOKEN={result.ConfirmationToken}");
        if (!string.IsNullOrWhiteSpace(result.ErrorMessage))
            Console.WriteLine($"RECONCILIATION_ERROR={result.ErrorMessage}");
        Console.WriteLine($"RECONCILED_COUNT={result.ReconciledMigrations.Count}");

        return result.Success ? 0 : 30;
    }

    private static DbContext CreateContext(
        DatabaseProviderType provider,
        string connectionString)
    {
        return provider switch
        {
            DatabaseProviderType.PostgreSql => CreatePostgreSqlContext(connectionString),
            DatabaseProviderType.Sqlite => CreateSqliteContext(connectionString),
            _ => throw new OperatorArgumentException(
                $"Provider '{provider}' is not approved for reconciliation writes.")
        };
    }

    private static PostgreSqlApplicationDbcontext CreatePostgreSqlContext(string connectionString)
    {
        var options = new DbContextOptionsBuilder<PostgreSqlApplicationDbcontext>()
            .UseNpgsql(
                connectionString,
                provider => provider.MigrationsAssembly(
                    typeof(PostgreSqlApplicationDbcontext).Assembly.FullName))
            .Options;
        return new PostgreSqlApplicationDbcontext(options);
    }

    private static SqLiteApplicationDbContext CreateSqliteContext(string connectionString)
    {
        var options = new DbContextOptionsBuilder<SqLiteApplicationDbContext>()
            .UseSqlite(
                connectionString,
                provider => provider.MigrationsAssembly(
                    typeof(SqLiteApplicationDbContext).Assembly.FullName))
            .Options;
        return new SqLiteApplicationDbContext(options);
    }

    private static void PrintUsage()
    {
        Console.Error.WriteLine(
            "Usage: HCL.CS.DatabaseAdmin <migrate|reconcile> --provider <PostgreSql|Sqlite|MySql|SqlServer> " +
            "--connection-env <ENVIRONMENT_VARIABLE> [--migration <ID>] [--apply --confirm <TOKEN>]");
    }
}

internal sealed class OperatorArguments
{
    public required string Command { get; init; }
    public required DatabaseProviderType Provider { get; init; }
    public required string ConnectionEnvironmentVariable { get; init; }
    public bool Apply { get; init; }
    public string ConfirmationToken { get; init; } = string.Empty;
    public IReadOnlyList<string> MigrationIds { get; init; } = Array.Empty<string>();

    public static OperatorArguments Parse(string[] args)
    {
        if (args.Length == 0) throw new OperatorArgumentException("A command is required.");

        string? provider = null;
        string? connectionEnvironmentVariable = null;
        string confirmationToken = string.Empty;
        var migrationIds = new List<string>();
        var apply = false;
        for (var index = 1; index < args.Length; index++)
        {
            switch (args[index])
            {
                case "--provider" when index + 1 < args.Length:
                    provider = args[++index];
                    break;
                case "--connection-env" when index + 1 < args.Length:
                    connectionEnvironmentVariable = args[++index];
                    break;
                case "--migration" when index + 1 < args.Length:
                    migrationIds.Add(args[++index]);
                    break;
                case "--apply":
                    apply = true;
                    break;
                case "--confirm" when index + 1 < args.Length:
                    confirmationToken = args[++index];
                    break;
                default:
                    throw new OperatorArgumentException($"Unknown or incomplete argument '{args[index]}'.");
            }
        }

        if (!Enum.TryParse<DatabaseProviderType>(provider, true, out var parsedProvider))
            throw new OperatorArgumentException("A valid --provider value is required.");
        if (string.IsNullOrWhiteSpace(connectionEnvironmentVariable))
            throw new OperatorArgumentException("--connection-env is required.");
        if (!System.Text.RegularExpressions.Regex.IsMatch(
                connectionEnvironmentVariable,
                "^[A-Za-z_][A-Za-z0-9_]*$"))
            throw new OperatorArgumentException("--connection-env must be an environment-variable name.");
        if (!apply && !string.IsNullOrWhiteSpace(confirmationToken))
            throw new OperatorArgumentException("--confirm is valid only with --apply.");
        if (apply && string.IsNullOrWhiteSpace(confirmationToken))
            throw new OperatorArgumentException("--apply requires --confirm.");

        return new OperatorArguments
        {
            Command = args[0].ToLowerInvariant(),
            Provider = parsedProvider,
            ConnectionEnvironmentVariable = connectionEnvironmentVariable,
            Apply = apply,
            ConfirmationToken = confirmationToken,
            MigrationIds = migrationIds
        };
    }
}

internal sealed class OperatorArgumentException(string message) : Exception(message);
