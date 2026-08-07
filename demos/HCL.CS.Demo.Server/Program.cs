/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using HCL.CS.DemoServerApp.Constants;
using HCL.CS.DemoServerApp.Options;
using HCL.CS.DemoServerApp.Services.ExternalAuth;
using HCL.CS.Domain;
using HCL.CS.Domain.Configurations.Api;
using HCL.CS.Domain.Enums;
using HCL.CS.Domain.Models.Endpoint;
using HCL.CS.DomainServices;
using HCL.CS.Hosting.Extensions;
using HCL.CS.Infrastructure.Data.Validation;
using HCL.CS.Infrastructure.Resources;

var applicationRootPath = ResolveApplicationRoot();
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = applicationRootPath,
    WebRootPath = Path.Combine(applicationRootPath, "wwwroot")
});

var (systemSettings, tokenSettings, notificationSettings) =
    LoadHclCsConfiguration(builder.Environment.ContentRootPath);
var allowInsecureHttpDev = builder.Environment.IsDevelopment()
                           && string.Equals(
                               Environment.GetEnvironmentVariable("HCL_CS_ALLOW_INSECURE_HTTP_DEV"),
                               "true",
                               StringComparison.OrdinalIgnoreCase);
var trustProxyHeaders = string.Equals(
    Environment.GetEnvironmentVariable("HCL_CS_TRUST_PROXY_HEADERS"),
    "true",
    StringComparison.OrdinalIgnoreCase);
var railwayPort = Environment.GetEnvironmentVariable("PORT");
var disableHttpsRedirection = allowInsecureHttpDev || (trustProxyHeaders && !string.IsNullOrWhiteSpace(railwayPort));

if (!string.IsNullOrWhiteSpace(railwayPort))
    builder.WebHost.UseUrls($"http://+:{railwayPort}");

if (trustProxyHeaders)
{
    builder.Services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        options.KnownNetworks.Clear();
        options.KnownProxies.Clear();
    });
}

var logConfig = new LogConfig
{
    LogFileConfig = new LogFileConfig(),
    InstanceName = LogKeyConstants.Authentication,
    WriteLogTo = WriteLogTo.File
};
logConfig.LogFileConfig.FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Authentication.txt");

builder.Services.AddHclCs(systemSettings, tokenSettings, notificationSettings)
    .AddAsymmetricKeystore(LoadAsymmetricCertificate())
    .AddLoggerInstance(logConfig);

builder.Services.AddOptions<GoogleOidcOptions>()
    .Bind(builder.Configuration.GetSection(GoogleOidcOptions.SectionName))
    .Validate(
        options => !options.Enabled || (!string.IsNullOrWhiteSpace(options.ClientId)
                                        && !string.IsNullOrWhiteSpace(options.ClientSecret)
                                        && !string.IsNullOrWhiteSpace(options.Authority)
                                        && !string.IsNullOrWhiteSpace(options.MetadataAddress)
                                        && !string.IsNullOrWhiteSpace(options.CallbackPath)),
        "Authentication:Google settings must be configured when Google login is enabled.")
    .ValidateOnStart();

builder.Services.AddOptions<ExternalAccountOptions>()
    .Bind(builder.Configuration.GetSection(ExternalAccountOptions.SectionName));

builder.Services.AddSingleton<IExternalAuthProvider, GoogleExternalAuthProvider>();
builder.Services.AddScoped<IExternalAuthService, ExternalAuthService>();

var googleOidc = builder.Configuration.GetSection(GoogleOidcOptions.SectionName).Get<GoogleOidcOptions>() ??
                 new GoogleOidcOptions();
if (googleOidc.Enabled)
{
    builder.Services.AddAuthentication()
        .AddOpenIdConnect(GoogleExternalAuthProvider.Scheme, options =>
        {
            options.SignInScheme = IdentityConstants.ExternalScheme;
            options.Authority = googleOidc.Authority;
            options.MetadataAddress = googleOidc.MetadataAddress;
            options.ClientId = googleOidc.ClientId;
            options.ClientSecret = googleOidc.ClientSecret;
            options.CallbackPath = googleOidc.CallbackPath;
            options.ResponseType = OpenIdConnectResponseType.Code;
            options.UsePkce = true;
            options.SaveTokens = false;
            options.GetClaimsFromUserInfoEndpoint = true;
            options.RequireHttpsMetadata = true;

            options.Scope.Clear();
            options.Scope.Add("openid");
            options.Scope.Add("profile");
            options.Scope.Add("email");

            options.NonceCookie.Name = "__Host.HCL.CS.Google.Nonce";
            options.NonceCookie.HttpOnly = true;
            options.NonceCookie.SecurePolicy =
                allowInsecureHttpDev ? CookieSecurePolicy.SameAsRequest : CookieSecurePolicy.Always;
            options.NonceCookie.SameSite = SameSiteMode.None;
            options.CorrelationCookie.Name = "__Host.HCL.CS.Google.Correlation";
            options.CorrelationCookie.HttpOnly = true;
            options.CorrelationCookie.SecurePolicy =
                allowInsecureHttpDev ? CookieSecurePolicy.SameAsRequest : CookieSecurePolicy.Always;
            options.CorrelationCookie.SameSite = SameSiteMode.None;

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuers = new[]
                {
                    "https://accounts.google.com",
                    "accounts.google.com"
                },
                ValidateAudience = true,
                ValidAudience = googleOidc.ClientId,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true
            };

            options.ProtocolValidator.RequireNonce = true;
            options.ProtocolValidator.RequireState = true;

            options.Events = new OpenIdConnectEvents
            {
                OnTokenValidated = context =>
                {
                    var issuer = context.Principal?.FindFirst("iss")?.Value;
                    var subject = context.Principal?.FindFirst("sub")?.Value;
                    if (string.IsNullOrWhiteSpace(issuer) || string.IsNullOrWhiteSpace(subject))
                        context.Fail("Google identity payload is missing required claims.");
                    return Task.CompletedTask;
                }
            };
        });
}

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "__Host.HCL.CS.DemoServer.Auth";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy =
        allowInsecureHttpDev ? CookieSecurePolicy.SameAsRequest : CookieSecurePolicy.Always;
    options.Cookie.IsEssential = true;
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromHours(10);
});

// Persist Data Protection keys so session/auth cookies survive app restarts (avoids "Error unprotecting the session cookie")
var keysPath = ResolveDataProtectionKeysPath(builder.Environment.ContentRootPath);

Directory.CreateDirectory(keysPath);
builder.Services.AddDataProtection()
    .SetApplicationName("HCL.CS.Demo.Server")
    .PersistKeysToFileSystem(new DirectoryInfo(keysPath));

builder.Services.AddSession(options =>
{
    options.Cookie.Name = "__Host.HCL.CS.DemoServer.Session";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy =
        allowInsecureHttpDev ? CookieSecurePolicy.SameAsRequest : CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Lax;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("HclCsStrictCors", policy =>
    {
        var allowedOrigins = builder.Configuration
            .GetSection("Security:Cors:AllowedOrigins")
            .Get<string[]>()?
            .Where(origin => !string.IsNullOrWhiteSpace(origin))
            .ToArray() ?? Array.Empty<string>();

        if (allowedOrigins.Length == 0)
            policy.WithOrigins("https://localhost:5002", "https://localhost:5001");
        else
            policy.WithOrigins(allowedOrigins);

        policy.WithMethods("GET", "POST")
            .WithHeaders("Authorization", "Content-Type", "X-Correlation-ID");
    });
});

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.OnRejected = (context, token) =>
    {
        context.HttpContext.Response.Headers.RetryAfter = "60";
        return ValueTask.CompletedTask;
    };

    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
    {
        var path = httpContext.Request.Path.Value?.ToLowerInvariant() ?? string.Empty;
        var isCriticalEndpoint = path.StartsWith("/security/token", StringComparison.Ordinal)
                                 || path.StartsWith("/security/introspect", StringComparison.Ordinal)
                                 || path.StartsWith("/security/revocation", StringComparison.Ordinal)
                                 || path.StartsWith("/account/login", StringComparison.Ordinal)
                                 || path.StartsWith("/auth/external/google/start", StringComparison.Ordinal)
                                 || path.StartsWith("/auth/external/google/callback", StringComparison.Ordinal);

        var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var clientIdentifier =
            GetClientIdentifierFromAuthorizationHeader(httpContext.Request.Headers.Authorization.ToString());
        var partitionKey = $"{ipAddress}:{clientIdentifier}:{(isCriticalEndpoint ? "critical" : "default")}";

        return isCriticalEndpoint
            ? RateLimitPartition.GetFixedWindowLimiter(
                partitionKey,
                _ => new FixedWindowRateLimiterOptions
                {
                    AutoReplenishment = true,
                    PermitLimit = 20,
                    QueueLimit = 0,
                    Window = TimeSpan.FromMinutes(1)
                })
            : RateLimitPartition.GetFixedWindowLimiter(
                partitionKey,
                _ => new FixedWindowRateLimiterOptions
                {
                    AutoReplenishment = true,
                    PermitLimit = 120,
                    QueueLimit = 0,
                    Window = TimeSpan.FromMinutes(1)
                });
    });
});

builder.Logging.AddJsonConsole();
// In Development, keys are persisted unencrypted so session survives restarts; suppress the XML encryptor warning
if (builder.Environment.IsDevelopment())
    builder.Logging.AddFilter("Microsoft.AspNetCore.DataProtection.KeyManagement.XmlKeyManager", LogLevel.Error);
IdentityModelEventSource.ShowPII = false;

// Build form-action CSP from CORS allowed origins so that login form POST → authorize
// callback → redirect to client origin is not blocked by the browser.
var cspFormActionOrigins = builder.Configuration
    .GetSection("Security:Cors:AllowedOrigins")
    .Get<string[]>()?
    .Where(origin => !string.IsNullOrWhiteSpace(origin))
    .Select(origin => origin.Trim())
    .ToArray() ?? Array.Empty<string>();
var formActionDirective = cspFormActionOrigins.Length > 0
    ? $"'self' {string.Join(" ", cspFormActionOrigins)}"
    : "'self'";

var app = builder.Build();

await ValidateRuntimeDatabaseCompatibilityAsync(app.Services, app.Logger);

app.Logger.LogInformation("Data Protection keys path: {KeysPath}", keysPath);
if (ShouldWarnAboutEphemeralDataProtectionKeys(keysPath))
{
    app.Logger.LogWarning(
        "Data Protection keys path is not explicitly configured for a container deployment. Mount persistent storage and set HCL_CS_DATA_PROTECTION_KEYS_PATH to a stable location such as /data/keys.");
}

if (trustProxyHeaders)
    app.UseForwardedHeaders();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

if (!disableHttpsRedirection)
{
    app.UseHttpsRedirection();
}
else if (trustProxyHeaders && !string.IsNullOrWhiteSpace(railwayPort))
{
    app.Logger.LogInformation(
        "Skipping HTTPS redirection because Railway terminates TLS at the edge and this instance is bound to internal HTTP on port {Port}.",
        railwayPort);
}
app.UseStaticFiles();
app.UseHclCsCorrelationId();
app.UseHclCsRequestObservability();

app.UseSession();
app.UseRouting();
app.UseCors("HclCsStrictCors");
app.UseRateLimiter();
app.Use(async (context, next) =>
{
    context.Response.Headers["X-Frame-Options"] = "DENY";
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["Referrer-Policy"] = "no-referrer";
    context.Response.Headers["Content-Security-Policy"] =
        "default-src 'self'; script-src 'self' 'unsafe-inline'; style-src 'self' 'unsafe-inline'; " +
        $"img-src 'self' data:; object-src 'none'; base-uri 'self'; frame-ancestors 'none'; form-action {formActionDirective}";
    await next();
});

app.UseAuthentication();
app.UseAuthorization();

app.UseHclCsEndpoint();
app.UseHclCsApi();

app.MapHclCsHealthChecks("/health/live", "/health/ready");
app.MapControllers();
app.MapDefaultControllerRoute();

app.Run();
return;

static async Task ValidateRuntimeDatabaseCompatibilityAsync(
    IServiceProvider services,
    ILogger logger)
{
    using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(30));
    using var scope = services.CreateScope();
    var applicationDbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
    var compatibilityService = new RuntimeSchemaCompatibilityService((DbContext)applicationDbContext);
    var report = await compatibilityService.ValidateCompatibilityAsync(timeout.Token);

    if (!report.ConnectivityStatus || !report.SchemaCompatibilityStatus || !report.BootstrapReadinessStatus)
    {
        throw new InvalidOperationException(
            $"HCL.CS runtime database compatibility validation failed for provider '{report.Provider}'. " +
            $"{report.ActionRequired} Runtime startup does not apply migrations or seed data; " +
            "run the HCL.CS Installer or an approved migration command.");
    }

    logger.LogInformation(
        "Runtime database compatibility validation succeeded for provider {Provider}; applied migrations: {AppliedMigrationCount}.",
        report.Provider,
        report.AppliedMigrations.Count);
}

static (SystemSettings SystemSettings, TokenSettings TokenSettings, NotificationTemplateSettings NotificationSettings)
    LoadHclCsConfiguration(string contentRootPath)
{
    var configurationBasePath = ResolveConfigurationBasePath(contentRootPath);
    var config = new ConfigurationBuilder()
        .SetBasePath(configurationBasePath)
        .AddJsonFile("./Configurations/SystemSettings.json")
        .AddJsonFile("./Configurations/TokenSettings.json")
        .AddJsonFile("./Configurations/NotificationTemplateSettings.json")
        .AddEnvironmentVariables()
        .Build();

    var systemSettings = new SystemSettings();
    config.GetSection("SystemSettings").Bind(systemSettings);

    var tokenSettings = new TokenSettings();
    config.GetSection("TokenSettings").Bind(tokenSettings);

    var notificationSettings = new NotificationTemplateSettings();
    config.GetSection("NotificationTemplateSettings").Bind(notificationSettings);

    var workspaceRoot = ResolveWorkspaceRoot(configurationBasePath);
    var dbConnectionOverride = Environment.GetEnvironmentVariable("HCL_CS_DB_CONNECTION_STRING");
    var dbConnectionString = !string.IsNullOrWhiteSpace(dbConnectionOverride)
        ? dbConnectionOverride
        : ResolveSecretPlaceholders(systemSettings.DBConfig.DBConnectionString);
    systemSettings.DBConfig.DBConnectionString = ResolveSqliteConnectionString(
        systemSettings.DBConfig.Database,
        dbConnectionString,
        workspaceRoot,
        "hclCs_identity.db");

    systemSettings.EmailConfig.SmtpServer = ResolveSecretPlaceholders(systemSettings.EmailConfig.SmtpServer);
    systemSettings.EmailConfig.UserName = ResolveSecretPlaceholders(systemSettings.EmailConfig.UserName);
    systemSettings.EmailConfig.Password = ResolveSecretPlaceholders(systemSettings.EmailConfig.Password);
    systemSettings.SMSConfig.SMSAccountIdentification =
        ResolveSecretPlaceholders(systemSettings.SMSConfig.SMSAccountIdentification);
    systemSettings.SMSConfig.SMSAccountPassword =
        ResolveSecretPlaceholders(systemSettings.SMSConfig.SMSAccountPassword);
    systemSettings.SMSConfig.SMSAccountFrom = ResolveSecretPlaceholders(systemSettings.SMSConfig.SMSAccountFrom);
    systemSettings.SMSConfig.SMSStatusCallbackURL =
        ResolveSecretPlaceholders(systemSettings.SMSConfig.SMSStatusCallbackURL);
    ApplyLdapEnvironmentOverrides(systemSettings.LdapConfig);
    ApplyLocalAuthenticationEnvironmentOverrides(systemSettings.LocalAuthenticationConfig);
    systemSettings.LdapConfig.BindPassword =
        ResolveSecretPlaceholders(systemSettings.LdapConfig.BindPassword);
    systemSettings.LogConfig.LogDbConfig.ConnectionString = ResolveSqliteConnectionString(
        systemSettings.LogConfig.LogDbConfig.Database,
        ResolveSecretPlaceholders(systemSettings.LogConfig.LogDbConfig.ConnectionString),
        workspaceRoot,
        "hclCs_log.db");

    tokenSettings.TokenConfig.IssuerUri = ResolveSecretPlaceholders(tokenSettings.TokenConfig.IssuerUri, true);
    tokenSettings.TokenConfig.ApiIdentifier = ResolveSecretPlaceholders(tokenSettings.TokenConfig.ApiIdentifier, true);

    return (systemSettings, tokenSettings, notificationSettings);
}

static void ApplyLdapEnvironmentOverrides(LdapConfig ldapConfig)
{
    OverrideString("HCL_CS_LDAP__HOST", value => ldapConfig.LdapHostName = value);
    OverrideInt("HCL_CS_LDAP__PORT", value => ldapConfig.LdapPort = value);
    OverrideBoolean("HCL_CS_LDAP__ENABLED", value => ldapConfig.Enabled = value);
    OverrideBoolean("HCL_CS_LDAP__USESSL", value => ldapConfig.UseSsl = value);
    OverrideBoolean("HCL_CS_LDAP__USESTARTTLS", value => ldapConfig.UseStartTls = value);
    OverrideBoolean(
        "HCL_CS_LDAP__ALLOWUNENCRYPTEDFORDEVELOPMENT",
        value => ldapConfig.AllowUnencryptedForDevelopment = value);
    OverrideString("HCL_CS_LDAP__BASEDN", value => ldapConfig.LdapDomainName = value);
    OverrideString("HCL_CS_LDAP__USERSEARCHBASE", value => ldapConfig.UserSearchBase = value);
    OverrideString("HCL_CS_LDAP__USERSEARCHFILTER", value => ldapConfig.UserSearchFilter = value);
    OverrideString("HCL_CS_LDAP__BINDDN", value => ldapConfig.BindDn = value);
    OverrideString("HCL_CS_LDAP__BINDPASSWORD", value => ldapConfig.BindPassword = value);
    OverrideInt(
        "HCL_CS_LDAP__CONNECTTIMEOUTSECONDS",
        value => ldapConfig.ConnectTimeoutSeconds = value);
    OverrideInt(
        "HCL_CS_LDAP__SEARCHTIMEOUTSECONDS",
        value => ldapConfig.SearchTimeoutSeconds = value);
    OverrideBoolean(
        "HCL_CS_LDAP__REQUIREEMPLOYEEID",
        value => ldapConfig.RequireEmployeeId = value);
    OverrideBoolean(
        "HCL_CS_LDAP__REQUIREDEPARTMENT",
        value => ldapConfig.RequireDepartment = value);
    OverrideBoolean(
        "HCL_CS_LDAP__REQUIREUSERPRINCIPALNAME",
        value => ldapConfig.RequireUserPrincipalName = value);
    OverrideString(
        "HCL_CS_LDAP__ATTRIBUTES__IMMUTABLEID",
        value => ldapConfig.Attributes.ImmutableId = value);
    OverrideString(
        "HCL_CS_LDAP__ATTRIBUTES__EMPLOYEEID",
        value => ldapConfig.Attributes.EmployeeId = value);
    OverrideString(
        "HCL_CS_LDAP__ATTRIBUTES__USERPRINCIPALNAME",
        value => ldapConfig.Attributes.UserPrincipalName = value);
    OverrideString(
        "HCL_CS_LDAP__ATTRIBUTES__EMAIL",
        value => ldapConfig.Attributes.Email = value);
    OverrideString(
        "HCL_CS_LDAP__ATTRIBUTES__DISPLAYNAME",
        value => ldapConfig.Attributes.DisplayName = value);
    OverrideString(
        "HCL_CS_LDAP__ATTRIBUTES__DEPARTMENT",
        value => ldapConfig.Attributes.Department = value);
    OverrideString(
        "HCL_CS_LDAP__ATTRIBUTES__ACCOUNTSTATUS",
        value => ldapConfig.Attributes.AccountStatus = value);
}

static void ApplyLocalAuthenticationEnvironmentOverrides(LocalAuthenticationConfig localAuthenticationConfig)
{
    OverrideBoolean(
        "HCL_CS_LOCALAUTHENTICATION__ENABLEDWHENLDAPDISABLEDORUNCONFIGURED",
        value => localAuthenticationConfig.EnabledWhenLdapDisabledOrUnconfigured = value);
    OverrideBoolean(
        "HCL_CS_LOCALAUTHENTICATION__REQUIREEMAILCONFIRMATION",
        value => localAuthenticationConfig.RequireEmailConfirmation = value);
    OverrideBoolean(
        "HCL_CS_LOCALAUTHENTICATION__ALLOWSELFREGISTRATION",
        value => localAuthenticationConfig.AllowSelfRegistration = value);
    OverrideBoolean(
        "HCL_CS_LOCALAUTHENTICATION__ALLOWADMINISTRATORCREATION",
        value => localAuthenticationConfig.AllowAdministratorCreation = value);

    var domains = Environment.GetEnvironmentVariable("HCL_CS_LOCALAUTHENTICATION__ALLOWEDEMAILDOMAINS");
    if (domains is not null)
        localAuthenticationConfig.AllowedEmailDomains = domains
            .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .ToList();
}

static void OverrideString(string environmentVariable, Action<string> apply)
{
    var value = Environment.GetEnvironmentVariable(environmentVariable);
    if (value is not null) apply(value);
}

static void OverrideInt(string environmentVariable, Action<int> apply)
{
    var value = Environment.GetEnvironmentVariable(environmentVariable);
    if (value is null) return;
    if (!int.TryParse(value, out var parsed))
        throw new InvalidOperationException($"{environmentVariable} must be an integer.");
    apply(parsed);
}

static void OverrideBoolean(string environmentVariable, Action<bool> apply)
{
    var value = Environment.GetEnvironmentVariable(environmentVariable);
    if (value is null) return;
    if (!bool.TryParse(value, out var parsed))
        throw new InvalidOperationException($"{environmentVariable} must be true or false.");
    apply(parsed);
}

static string ResolveApplicationRoot()
{
    var candidateRoots = new[]
    {
        Directory.GetCurrentDirectory(),
        AppContext.BaseDirectory
    };

    foreach (var candidateRoot in candidateRoots
                 .Where(path => !string.IsNullOrWhiteSpace(path))
                 .Distinct(StringComparer.OrdinalIgnoreCase))
    {
        var directory = new DirectoryInfo(Path.GetFullPath(candidateRoot));
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "HCL.CS.DemoServerApp.csproj")))
                return directory.FullName;

            directory = directory.Parent;
        }
    }

    return Path.GetFullPath(Directory.GetCurrentDirectory());
}

static string ResolveDataProtectionKeysPath(string contentRootPath)
{
    var configuredPath = Environment.GetEnvironmentVariable("HCL_CS_DATA_PROTECTION_KEYS_PATH");
    if (!string.IsNullOrWhiteSpace(configuredPath))
        return Path.GetFullPath(configuredPath);

    var contentRootKeysPath = Path.Combine(contentRootPath, "DataProtection-Keys");
    if (ContainsDataProtectionKeys(contentRootKeysPath))
        return contentRootKeysPath;

    const string containerDataRoot = "/data";
    var containerKeysPath = Path.Combine(containerDataRoot, "keys");
    if (ContainsDataProtectionKeys(containerKeysPath))
        return containerKeysPath;

    if (Directory.Exists(containerDataRoot))
        return containerKeysPath;

    var localApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
    if (!string.IsNullOrWhiteSpace(localApplicationData))
    {
        var localApplicationKeysPath = Path.Combine(localApplicationData, "HCL.CS", "DataProtection-Keys");
        if (ContainsDataProtectionKeys(localApplicationKeysPath))
            return localApplicationKeysPath;

        return localApplicationKeysPath;
    }

    return contentRootKeysPath;
}

static bool ContainsDataProtectionKeys(string path)
{
    return Directory.Exists(path) && Directory.EnumerateFiles(path, "key-*.xml").Any();
}

static bool ShouldWarnAboutEphemeralDataProtectionKeys(string keysPath)
{
    var configuredPath = Environment.GetEnvironmentVariable("HCL_CS_DATA_PROTECTION_KEYS_PATH");
    if (!string.IsNullOrWhiteSpace(configuredPath))
        return false;

    var runningInContainer = string.Equals(
        Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"),
        "true",
        StringComparison.OrdinalIgnoreCase);

    if (!runningInContainer)
        return false;

    return !keysPath.Equals("/data/keys", StringComparison.Ordinal)
           && !keysPath.StartsWith("/data/keys/", StringComparison.Ordinal);
}

static string ResolveConfigurationBasePath(string contentRootPath)
{
    var candidateRoots = new[]
    {
        contentRootPath,
        AppContext.BaseDirectory
    };

    foreach (var candidateRoot in candidateRoots
                 .Where(path => !string.IsNullOrWhiteSpace(path))
                 .Distinct(StringComparer.OrdinalIgnoreCase))
    {
        var absolutePath = Path.GetFullPath(candidateRoot);
        if (File.Exists(Path.Combine(absolutePath, "Configurations", "SystemSettings.json")))
            return absolutePath;
    }

    return Path.GetFullPath(contentRootPath);
}

static string ResolveWorkspaceRoot(string contentRootPath)
{
    var candidateRoots = new[]
    {
        contentRootPath,
        AppContext.BaseDirectory
    };

    foreach (var candidateRoot in candidateRoots
                 .Where(path => !string.IsNullOrWhiteSpace(path))
                 .Distinct(StringComparer.OrdinalIgnoreCase))
    {
        var directory = new DirectoryInfo(Path.GetFullPath(candidateRoot));
        while (directory is not null)
        {
            if (Directory.Exists(Path.Combine(directory.FullName, ".data"))
                || File.Exists(Path.Combine(directory.FullName, "HCL.CS.sln"))
                || Directory.Exists(Path.Combine(directory.FullName, ".git")))
                return directory.FullName;

            directory = directory.Parent;
        }
    }

    return Path.GetFullPath(contentRootPath);
}

static string ResolveSqliteConnectionString(
    DbTypes databaseType,
    string? connectionString,
    string workspaceRoot,
    string defaultFileName)
{
    if (databaseType != DbTypes.SQLite) return connectionString ?? string.Empty;

    var builder = string.IsNullOrWhiteSpace(connectionString)
        ? new SqliteConnectionStringBuilder()
        : new SqliteConnectionStringBuilder(connectionString);

    if (string.IsNullOrWhiteSpace(builder.DataSource))
    {
        builder.DataSource = Path.Combine(workspaceRoot, ".data", defaultFileName);
        builder.Mode = SqliteOpenMode.ReadWriteCreate;
        builder.Cache = SqliteCacheMode.Shared;
    }
    else if (!IsSpecialSqliteDataSource(builder.DataSource) && !Path.IsPathRooted(builder.DataSource))
    {
        builder.DataSource = Path.GetFullPath(Path.Combine(workspaceRoot, builder.DataSource));
    }

    if (!IsSpecialSqliteDataSource(builder.DataSource))
    {
        var directoryPath = Path.GetDirectoryName(builder.DataSource);
        if (!string.IsNullOrWhiteSpace(directoryPath))
            Directory.CreateDirectory(directoryPath);
    }

    return builder.ToString();
}

static bool IsSpecialSqliteDataSource(string dataSource)
{
    return string.Equals(dataSource, ":memory:", StringComparison.OrdinalIgnoreCase)
           || dataSource.StartsWith("file:", StringComparison.OrdinalIgnoreCase);
}

static string ResolveSecretPlaceholders(string value, bool required = false)
{
    if (string.IsNullOrWhiteSpace(value)) return value;

    return HCL.CS.DemoServerApp.Program.MyRegex().Replace(value, match =>
    {
        var variableName = match.Groups[1].Value;
        var replacement = Environment.GetEnvironmentVariable(variableName);
        if (!string.IsNullOrWhiteSpace(replacement)) return replacement;
        return required
            ? throw new InvalidOperationException($"Missing required environment variable '{variableName}'.")
            : string.Empty;
    });
}

static List<AsymmetricKeyInfoModel> LoadAsymmetricCertificate()
{
    var certificatePassword = Environment.GetEnvironmentVariable("HCL_CS_SIGNING_CERT_PASSWORD");
    if (string.IsNullOrWhiteSpace(certificatePassword))
        throw new InvalidOperationException(
            "A password for persistent signing certificates is required in 'HCL_CS_SIGNING_CERT_PASSWORD'.");

    return new List<AsymmetricKeyInfoModel>
    {
        LoadCertificateFromEnvironment(
        "HCL_CS_RSA_SIGNING_CERT_BASE64",
        "HCL_CS_RSA_SIGNING_CERT_PATH",
        certificatePassword,
        SigningAlgorithm.RS256,
        Environment.GetEnvironmentVariable("HCL_CS_RSA_SIGNING_KID") ?? "hcl-cs-rsa-current"),
        LoadCertificateFromEnvironment(
        "HCL_CS_ECDSA_SIGNING_CERT_BASE64",
        "HCL_CS_ECDSA_SIGNING_CERT_PATH",
        certificatePassword,
        SigningAlgorithm.ES256,
        Environment.GetEnvironmentVariable("HCL_CS_ECDSA_SIGNING_KID") ?? "hcl-cs-ecdsa-current")
    };
}

static AsymmetricKeyInfoModel LoadCertificateFromEnvironment(
    string base64EnvKey,
    string pathEnvKey,
    string password,
    SigningAlgorithm algorithm,
    string keyId)
{
    var certificateBase64 = Environment.GetEnvironmentVariable(base64EnvKey);
    if (!string.IsNullOrWhiteSpace(certificateBase64))
    {
        var rawBytes = Convert.FromBase64String(certificateBase64);
        try
        {
            return SigningCertificateLoader.LoadFromBytes(rawBytes, password, algorithm, keyId);
        }
        finally
        {
            CryptographicOperations.ZeroMemory(rawBytes);
        }
    }

    var certificatePath = Environment.GetEnvironmentVariable(pathEnvKey);
    if (string.IsNullOrWhiteSpace(certificatePath))
        throw new InvalidOperationException(
            $"A persistent {algorithm} signing certificate is required. Configure '{base64EnvKey}' or '{pathEnvKey}'.");

    return SigningCertificateLoader.LoadFromFile(certificatePath, password, algorithm, keyId);
}

static string GetClientIdentifierFromAuthorizationHeader(string authorizationHeader)
{
    const string basicPrefix = "Basic ";
    if (string.IsNullOrWhiteSpace(authorizationHeader)
        || !authorizationHeader.StartsWith(basicPrefix, StringComparison.OrdinalIgnoreCase))
        return "anonymous";

    try
    {
        var encoded = authorizationHeader[basicPrefix.Length..];
        var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(encoded));
        var split = decoded.Split(':', 2);
        return split.Length > 0 && !string.IsNullOrWhiteSpace(split[0]) ? split[0] : "anonymous";
    }
    catch
    {
        return "anonymous";
    }
}

namespace HCL.CS.DemoServerApp
{
    internal partial class Program
    {
        [GeneratedRegex(@"\$\{([A-Z0-9_]+)\}")]
        internal static partial Regex MyRegex();
    }
}
