using System.DirectoryServices.Protocols;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Zentra.Domain;
using Zentra.Domain.Constants;
using Zentra.Domain.Models.Endpoint;
using Zentra.DomainServices.Infra;
using Zentra.Hosting.Health;
using Zentra.Infrastructure.Data.Extension;
using Zentra.Infrastructure.Data.Validation;
using Zentra.Infrastructure.Resources.Extension;
using Zentra.Infrastructure.Services.Extension;
using Zentra.ProxyService.Extension;
using Zentra.Service.Extension;

namespace Zentra.Hosting.Extensions;

public static class ZentraExtension
{
    private static readonly List<Exception> ErrorList = new();

    public static IServiceCollection AddZentra(
        this IServiceCollection services,
        SystemSettings systemSettings,
        TokenSettings tokenSettings,
        NotificationTemplateSettings templateSettings)
    {
        var ZentraConfig = new ZentraConfig
        {
            SystemSettings = systemSettings,
            NotificationTemplateSettings = templateSettings,
            TokenSettings = tokenSettings
        };
        return services.AddSecurityExtensions(ZentraConfig);
    }

    public static IServiceCollection AddZentra(
        this IServiceCollection services,
        string systemSettingsJsonPath,
        string tokenConfigSettingsJsonPath,
        string notificationTemplateSettingsJsonPath)
    {
        var ZentraConfig = DeserializeConfiguration(systemSettingsJsonPath, notificationTemplateSettingsJsonPath,
            tokenConfigSettingsJsonPath);
        return services.AddSecurityExtensions(ZentraConfig);
    }

    public static IServiceCollection AddAsymmetricKeystore(this IServiceCollection services,
        IEnumerable<AsymmetricKeyInfoModel> securityKeys)
    {
        if (securityKeys == null) throw new AggregateException("Security keys is null or invalid");

        services.AddSecurityAsymmetricKeystore(securityKeys);
        return services;
    }

    public static IServiceCollection AddLoggerInstance(this IServiceCollection services, LogConfig logConfig)
    {
        ErrorList.Clear();
        var dbConnectionValidator = new DbConnectionValidator();
        ValidateLogConfiguration(logConfig, false, dbConnectionValidator);

        if (ErrorList.Count > 0) throw new AggregateException("Multiple Errors Occurred", ErrorList);

        using var serviceProvider = services.BuildServiceProvider();
        services.AddSecurityLoggerInstance(serviceProvider, logConfig);
        return services;
    }

    private static IServiceCollection AddSecurityExtensions(this IServiceCollection services, ZentraConfig ZentraConfig)
    {
        ValidateConfiguration(ZentraConfig);

        // Setting Zentra instance name.
        ZentraConfig.SystemSettings.LogConfig.InstanceName = LoggerKeyConstants.DefaultLoggerKey;

        // Dont change the below order (application will misbehave).
        services.AddConfiguration(ZentraConfig)
            .AddAutoMapper()
            .AddInfrastructureResources()
            .AddUtilityServices()
            .AddDistributedCacheFromConfig()
            .AddIdentityConfiguration()
            .AddRepository()
            .AddInfrastructureServices()
            .AddCoreServices()
            .AddWrappers()
            .AddDefaultEndpoints()
            .AddProxyServices()
            .AddProxyValidator()
            .AddProxyRoutes();

        services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy(), tags: new[] { "live" })
            .AddCheck<DatabaseDependencyHealthCheck>("database", tags: new[] { "ready" })
            .AddCheck<CacheDependencyHealthCheck>("cache", tags: new[] { "ready" });

        using var serviceProvider = services.BuildServiceProvider();
        services.AddSecurityLoggerInstance(serviceProvider, ZentraConfig.SystemSettings.LogConfig);

        return services;
    }

    private static IServiceCollection AddDistributedCacheFromConfig(this IServiceCollection services)
    {
        var redisConnection = Environment.GetEnvironmentVariable("ZENTRA_REDIS_CONNECTION_STRING");
        if (!string.IsNullOrWhiteSpace(redisConnection))
        {
            var instanceName = Environment.GetEnvironmentVariable("ZENTRA_REDIS_INSTANCE_NAME") ?? "zentra:";
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnection;
                options.InstanceName = instanceName;
            });
        }
        else
        {
            services.AddDistributedMemoryCache();
        }

        return services;
    }

    private static ZentraConfig DeserializeConfiguration(
        string systemSettingsJsonPath,
        string notificationTemplateSettingsJsonPath,
        string tokenConfigSettingsJsonPath)
    {
        if (string.IsNullOrWhiteSpace(systemSettingsJsonPath) || !File.Exists(systemSettingsJsonPath.Trim()))
            throw new ArgumentException("System Setting Configuration file not found");

        if (string.IsNullOrWhiteSpace(notificationTemplateSettingsJsonPath) ||
            !File.Exists(notificationTemplateSettingsJsonPath.Trim()))
            throw new ArgumentException("Notification template Configuration file not found");

        if (string.IsNullOrWhiteSpace(tokenConfigSettingsJsonPath) || !File.Exists(tokenConfigSettingsJsonPath.Trim()))
            throw new ArgumentException("Token Configuration file not found");

        IConfiguration configuration = new ConfigurationBuilder()
            .AddJsonFile(systemSettingsJsonPath)
            .AddJsonFile(notificationTemplateSettingsJsonPath)
            .AddJsonFile(tokenConfigSettingsJsonPath)
            .Build();
        var systemSettings = new SystemSettings();
        configuration.GetSection("SystemSettings").Bind(systemSettings);

        var templateSettings = new NotificationTemplateSettings();
        configuration.GetSection("NotificationTemplateSettings").Bind(templateSettings);

        var tokenSettings = new TokenSettings();
        configuration.GetSection("TokenSettings").Bind(tokenSettings);

        var ZentraConfig = new ZentraConfig
        {
            SystemSettings = systemSettings,
            NotificationTemplateSettings = templateSettings,
            TokenSettings = tokenSettings
        };
        return ZentraConfig;
    }

    private static void ValidateConfiguration(ZentraConfig ZentraConfig)
    {
        ErrorList.Clear();
        var dbConnectionValidator = new DbConnectionValidator();
        ValidateDatabaseConfiguration(ZentraConfig, dbConnectionValidator);
        ValidateLdapConfiguration(ZentraConfig);
        ValidateEmailConfiguration(ZentraConfig);
        ValidateSmsConfiguration(ZentraConfig);
        ValidateTokenConfiguration(ZentraConfig);
        ValidateLogConfiguration(ZentraConfig.SystemSettings.LogConfig, true, dbConnectionValidator);

        if (ErrorList.Count > 0) throw new AggregateException("Multiple Errors Occurred", ErrorList);
    }

    private static void ValidateDatabaseConfiguration(ZentraConfig ZentraConfig, IDbConnectionValidator dbConnectionValidator)
    {
        if (ZentraConfig.SystemSettings.DBConfig.Database <= 0)
            ErrorList.Add(new Exception("Database name is null or invalid"));

        if (string.IsNullOrWhiteSpace(ZentraConfig.SystemSettings.DBConfig.DBConnectionString))
            ErrorList.Add(new Exception("Database connection string is not configured"));
        else
        {
            var exception = dbConnectionValidator.Validate(
                ZentraConfig.SystemSettings.DBConfig.Database,
                ZentraConfig.SystemSettings.DBConfig.DBConnectionString);
            if (exception != null) ErrorList.Add(exception);
        }
    }

    private static void ValidateLdapConfiguration(ZentraConfig ZentraConfig)
    {
        if (!string.IsNullOrWhiteSpace(ZentraConfig.SystemSettings.LdapConfig.LdapHostName) &&
            ZentraConfig.SystemSettings.LdapConfig.LdapPort > 0)
            try
            {
                if (string.IsNullOrWhiteSpace(ZentraConfig.SystemSettings.LdapConfig.LdapDomainName))
                    ErrorList.Add(new Exception("Ldap domain name is not configured"));

                var serverId = new LdapDirectoryIdentifier(
                    ZentraConfig.SystemSettings.LdapConfig.LdapHostName,
                    ZentraConfig.SystemSettings.LdapConfig.LdapPort);
                using var ldapConnection = new LdapConnection(serverId);
                ldapConnection.SessionOptions.ProtocolVersion = 3;
                if (ZentraConfig.SystemSettings.LdapConfig.IsSecureConnection)
                {
                    ldapConnection.AuthType = AuthType.External;
                    ldapConnection.SessionOptions.SecureSocketLayer = true;
                }
                else
                {
                    ldapConnection.AuthType = AuthType.Basic;
                    ldapConnection.SessionOptions.SecureSocketLayer = false;
                }

                ldapConnection.Bind();

                GlobalConfiguration.IsLdapConfigurationValid = true;
                if (ldapConnection != null) ldapConnection.Dispose();
            }
            catch (Exception ex)
            {
                ErrorList.Add(ex);
            }
    }

    private static void ValidateEmailConfiguration(ZentraConfig ZentraConfig)
    {
        if (!string.IsNullOrWhiteSpace(ZentraConfig.SystemSettings.EmailConfig.SmtpServer) &&
            !string.IsNullOrWhiteSpace(ZentraConfig.SystemSettings.EmailConfig.UserName) &&
            !string.IsNullOrWhiteSpace(ZentraConfig.SystemSettings.EmailConfig.Password) &&
            ZentraConfig.SystemSettings.EmailConfig.Port > 0)
            try
            {
                using var client = new SmtpClient();
                if (ZentraConfig.SystemSettings.EmailConfig.SecureSocketOptions)
                    client.ConnectAsync(
                        ZentraConfig.SystemSettings.EmailConfig.SmtpServer,
                        ZentraConfig.SystemSettings.EmailConfig.Port,
                        true).GetAwaiter().GetResult();
                else
                    client.ConnectAsync(
                        ZentraConfig.SystemSettings.EmailConfig.SmtpServer,
                        ZentraConfig.SystemSettings.EmailConfig.Port,
                        SecureSocketOptions.StartTls).GetAwaiter().GetResult();

                GlobalConfiguration.IsEmailConfigurationValid = true;

                if (client.IsConnected)
                {
                    client.DisconnectAsync(true).GetAwaiter().GetResult();
                }
            }
            catch (Exception ex)
            {
                ErrorList.Add(ex);
            }
    }

    private static void ValidateSmsConfiguration(ZentraConfig ZentraConfig)
    {
        if (!string.IsNullOrWhiteSpace(ZentraConfig.SystemSettings.SMSConfig.SMSAccountIdentification) &&
            !string.IsNullOrWhiteSpace(ZentraConfig.SystemSettings.SMSConfig.SMSAccountPassword) &&
            !string.IsNullOrWhiteSpace(ZentraConfig.SystemSettings.SMSConfig.SMSAccountFrom) &&
            !string.IsNullOrWhiteSpace(ZentraConfig.SystemSettings.SMSConfig.SMSStatusCallbackURL))
            try
            {
                GlobalConfiguration.IsSmsConfigurationValid = true;
            }
            catch (Exception ex)
            {
                ErrorList.Add(ex);
            }
    }

    private static void ValidateTokenConfiguration(ZentraConfig ZentraConfig)
    {
        if (string.IsNullOrWhiteSpace(ZentraConfig.TokenSettings.TokenConfig.IssuerUri))
            ErrorList.Add(new Exception("IssuerUri is not configured"));
        else if (IsProductionEnvironment() && LooksLikeLocalIssuer(ZentraConfig.TokenSettings.TokenConfig.IssuerUri))
            ErrorList.Add(new Exception(
                $"IssuerUri '{ZentraConfig.TokenSettings.TokenConfig.IssuerUri}' is not valid for Production. Configure TokenSettings__TokenConfig__IssuerUri with the public Railway domain."));

        if (string.IsNullOrWhiteSpace(ZentraConfig.TokenSettings.UserInteractionConfig.LoginUrl))
            ErrorList.Add(new Exception("LoginUrl is not configured"));

        if (string.IsNullOrWhiteSpace(ZentraConfig.TokenSettings.UserInteractionConfig.LogoutUrl))
            ErrorList.Add(new Exception("LogoutUrl is not configured"));

        if (string.IsNullOrWhiteSpace(ZentraConfig.TokenSettings.UserInteractionConfig.ErrorUrl))
            ErrorList.Add(new Exception("ErrorUrl is not configured"));
    }

    private static bool IsProductionEnvironment()
    {
        var environment =
            Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
            ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");

        return string.Equals(environment, "Production", StringComparison.OrdinalIgnoreCase);
    }

    private static bool LooksLikeLocalIssuer(string issuerUri)
    {
        if (!Uri.TryCreate(issuerUri, UriKind.Absolute, out var issuer))
        {
            return false;
        }

        return issuer.IsLoopback
            || string.Equals(issuer.Host, "localhost", StringComparison.OrdinalIgnoreCase)
            || string.Equals(issuer.Host, "127.0.0.1", StringComparison.OrdinalIgnoreCase)
            || string.Equals(issuer.Host, "::1", StringComparison.OrdinalIgnoreCase);
    }

    private static void ValidateLogConfiguration(LogConfig logConfig, bool isZentraInstance,
        IDbConnectionValidator dbConnectionValidator)
    {
        if (logConfig.WriteLogTo == WriteLogTo.DataBase)
        {
            ValidateLoggerDatabaseConfiguration(logConfig, dbConnectionValidator);
        }
        else if (logConfig.WriteLogTo == WriteLogTo.File)
        {
            if (isZentraInstance && string.IsNullOrWhiteSpace(logConfig.LogFileConfig.FilePath))
                logConfig.LogFileConfig.FilePath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "Logs\\Zentra.txt");

            ValidateLoggerFileConfiguration(logConfig);
        }
    }

    private static void ValidateLoggerDatabaseConfiguration(LogConfig logConfig,
        IDbConnectionValidator dbConnectionValidator)
    {
        if (logConfig.LogDbConfig.Database <= 0)
            ErrorList.Add(new Exception("Database name specified in logconfig is null or invalid"));

        if (string.IsNullOrWhiteSpace(logConfig.LogDbConfig.ConnectionString))
            ErrorList.Add(new Exception("Database connection string specified in logconfig is null or invalid"));
        else
        {
            var exception = dbConnectionValidator.Validate(logConfig.LogDbConfig.Database,
                logConfig.LogDbConfig.ConnectionString);
            if (exception != null) ErrorList.Add(exception);
        }
    }

    private static void ValidateLoggerFileConfiguration(LogConfig logConfig)
    {
        if (string.IsNullOrWhiteSpace(logConfig.LogFileConfig.FilePath))
        {
            ErrorList.Add(new Exception("File path specified in logconfig is null or invalid"));
        }
        else
        {
            var directoryName = Path.GetDirectoryName(logConfig.LogFileConfig.FilePath);
            if (!Directory.Exists(directoryName))
                ErrorList.Add(new Exception("File path specified in logconfig is not exists"));
        }
    }
}
