using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Zentra.DemoServerApp.Constants;
using Zentra.DemoServerApp.Options;
using Zentra.DemoServerApp.Services.ExternalAuth;
using Zentra.Domain;
using Zentra.Domain.Enums;
using Zentra.Domain.Models.Endpoint;
using Zentra.Hosting.Extensions;

var applicationRootPath = ResolveApplicationRoot();
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = applicationRootPath,
    WebRootPath = Path.Combine(applicationRootPath, "wwwroot")
});

var (systemSettings, tokenSettings, notificationSettings) =
    LoadZentraConfiguration(builder.Environment.ContentRootPath);
var allowInsecureHttpDev = builder.Environment.IsDevelopment()
                           && string.Equals(
                               Environment.GetEnvironmentVariable("ZENTRA_ALLOW_INSECURE_HTTP_DEV"),
                               "true",
                               StringComparison.OrdinalIgnoreCase);
var trustProxyHeaders = string.Equals(
    Environment.GetEnvironmentVariable("ZENTRA_TRUST_PROXY_HEADERS"),
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

builder.Services.AddZentra(systemSettings, tokenSettings, notificationSettings)
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

            options.NonceCookie.Name = "__Host.Zentra.Google.Nonce";
            options.NonceCookie.HttpOnly = true;
            options.NonceCookie.SecurePolicy =
                allowInsecureHttpDev ? CookieSecurePolicy.SameAsRequest : CookieSecurePolicy.Always;
            options.NonceCookie.SameSite = SameSiteMode.None;
            options.CorrelationCookie.Name = "__Host.Zentra.Google.Correlation";
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
    options.Cookie.Name = "__Host.Zentra.DemoServer.Auth";
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
    .SetApplicationName("Zentra.Demo.Server")
    .PersistKeysToFileSystem(new DirectoryInfo(keysPath));

builder.Services.AddSession(options =>
{
    options.Cookie.Name = "__Host.Zentra.DemoServer.Session";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy =
        allowInsecureHttpDev ? CookieSecurePolicy.SameAsRequest : CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Lax;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("ZentraStrictCors", policy =>
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

app.Logger.LogInformation("Data Protection keys path: {KeysPath}", keysPath);
if (ShouldWarnAboutEphemeralDataProtectionKeys(keysPath))
{
    app.Logger.LogWarning(
        "Data Protection keys path is not explicitly configured for a container deployment. Mount persistent storage and set ZENTRA_DATA_PROTECTION_KEYS_PATH to a stable location such as /data/keys.");
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
app.UseZentraCorrelationId();
app.UseZentraRequestObservability();

app.UseSession();
app.UseRouting();
app.UseCors("ZentraStrictCors");
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

app.UseZentraEndpoint();
app.UseZentraApi();

app.MapZentraHealthChecks("/health/live", "/health/ready");
app.MapControllers();
app.MapDefaultControllerRoute();

app.Run();
return;

static (SystemSettings SystemSettings, TokenSettings TokenSettings, NotificationTemplateSettings NotificationSettings)
    LoadZentraConfiguration(string contentRootPath)
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
    var dbConnectionOverride = Environment.GetEnvironmentVariable("ZENTRA_DB_CONNECTION_STRING");
    var dbConnectionString = !string.IsNullOrWhiteSpace(dbConnectionOverride)
        ? dbConnectionOverride
        : ResolveSecretPlaceholders(systemSettings.DBConfig.DBConnectionString);
    systemSettings.DBConfig.DBConnectionString = ResolveSqliteConnectionString(
        systemSettings.DBConfig.Database,
        dbConnectionString,
        workspaceRoot,
        "zentra_identity.db");

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
    systemSettings.LogConfig.LogDbConfig.ConnectionString = ResolveSqliteConnectionString(
        systemSettings.LogConfig.LogDbConfig.Database,
        ResolveSecretPlaceholders(systemSettings.LogConfig.LogDbConfig.ConnectionString),
        workspaceRoot,
        "zentra_log.db");

    tokenSettings.TokenConfig.IssuerUri = ResolveSecretPlaceholders(tokenSettings.TokenConfig.IssuerUri, true);
    tokenSettings.TokenConfig.ApiIdentifier = ResolveSecretPlaceholders(tokenSettings.TokenConfig.ApiIdentifier, true);

    return (systemSettings, tokenSettings, notificationSettings);
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
            if (File.Exists(Path.Combine(directory.FullName, "Zentra.DemoServerApp.csproj")))
                return directory.FullName;

            directory = directory.Parent;
        }
    }

    return Path.GetFullPath(Directory.GetCurrentDirectory());
}

static string ResolveDataProtectionKeysPath(string contentRootPath)
{
    var configuredPath = Environment.GetEnvironmentVariable("ZENTRA_DATA_PROTECTION_KEYS_PATH");
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
        var localApplicationKeysPath = Path.Combine(localApplicationData, "Zentra", "DataProtection-Keys");
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
    var configuredPath = Environment.GetEnvironmentVariable("ZENTRA_DATA_PROTECTION_KEYS_PATH");
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
                || File.Exists(Path.Combine(directory.FullName, "Zentra.sln"))
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

    return Zentra.DemoServerApp.Program.MyRegex().Replace(value, match =>
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
    var certificatePassword = Environment.GetEnvironmentVariable("ZENTRA_SIGNING_CERT_PASSWORD");
    var keyInfos = new List<AsymmetricKeyInfoModel>();

    var rsaCertificate = LoadCertificateFromEnvironment(
        "ZENTRA_RSA_SIGNING_CERT_BASE64",
        "ZENTRA_RSA_SIGNING_CERT_PATH",
        certificatePassword,
        SigningAlgorithm.RS256,
        "CN=Zentra Demo RSA");

    var ecdsaCertificate = LoadCertificateFromEnvironment(
        "ZENTRA_ECDSA_SIGNING_CERT_BASE64",
        "ZENTRA_ECDSA_SIGNING_CERT_PATH",
        certificatePassword,
        SigningAlgorithm.ES256,
        "CN=Zentra Demo ECDSA");

    keyInfos.Add(new AsymmetricKeyInfoModel
    {
        Certificate = rsaCertificate,
        Algorithm = SigningAlgorithm.RS256,
        KeyId = Environment.GetEnvironmentVariable("ZENTRA_RSA_SIGNING_KID") ?? "zentra-rsa-current"
    });

    keyInfos.Add(new AsymmetricKeyInfoModel
    {
        Certificate = ecdsaCertificate,
        Algorithm = SigningAlgorithm.ES256,
        KeyId = Environment.GetEnvironmentVariable("ZENTRA_ECDSA_SIGNING_KID") ?? "zentra-ecdsa-current"
    });

    return keyInfos;
}

static X509Certificate2 LoadCertificateFromEnvironment(
    string base64EnvKey,
    string pathEnvKey,
    string? password,
    SigningAlgorithm algorithm,
    string subjectName)
{
    const X509KeyStorageFlags storageFlags = X509KeyStorageFlags.Exportable | X509KeyStorageFlags.EphemeralKeySet;
    var certificateBase64 = Environment.GetEnvironmentVariable(base64EnvKey);
    if (!string.IsNullOrWhiteSpace(certificateBase64))
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new InvalidOperationException(
                $"'{base64EnvKey}' is set but 'ZENTRA_SIGNING_CERT_PASSWORD' is missing.");

        var rawBytes = Convert.FromBase64String(certificateBase64);
        var certificate = new X509Certificate2(rawBytes, password, storageFlags);
        return IsCertificateUsable(certificate, algorithm)
            ? certificate
            : throw new InvalidOperationException(
                $"Certificate loaded from '{base64EnvKey}' is not valid for {algorithm}.");
    }

    var certificatePath = Environment.GetEnvironmentVariable(pathEnvKey);
    if (string.IsNullOrWhiteSpace(certificatePath) || !File.Exists(certificatePath))
        return CreateSelfSignedCertificate(algorithm, subjectName);
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new InvalidOperationException(
                $"'{pathEnvKey}' is set but 'ZENTRA_SIGNING_CERT_PASSWORD' is missing.");

        var certificate = new X509Certificate2(certificatePath, password, storageFlags);
        return IsCertificateUsable(certificate, algorithm)
            ? certificate
            : throw new InvalidOperationException(
                $"Certificate loaded from '{pathEnvKey}' is not valid for {algorithm}.");
    }
}

static bool IsCertificateUsable(X509Certificate2 certificate, SigningAlgorithm algorithm)
{
    if (!certificate.HasPrivateKey || certificate.NotAfter <= DateTime.UtcNow) return false;

    var algorithmName = Enum.GetName(typeof(SigningAlgorithm), algorithm);
    if (algorithmName is null) return false;

    if (algorithmName.StartsWith("RS", StringComparison.OrdinalIgnoreCase)
        || algorithmName.StartsWith("PS", StringComparison.OrdinalIgnoreCase))
    {
        using var privateKey = certificate.GetRSAPrivateKey();
        return privateKey != null;
    }

    if (!algorithmName.StartsWith("ES", StringComparison.OrdinalIgnoreCase)) return false;
    {
        using var privateKey = certificate.GetECDsaPrivateKey();
        return privateKey != null;
    }
}

static X509Certificate2 CreateSelfSignedCertificate(SigningAlgorithm algorithm, string subjectName)
{
    var notBefore = DateTimeOffset.UtcNow.AddMinutes(-5);
    var notAfter = DateTimeOffset.UtcNow.AddYears(1);
    var algorithmName = Enum.GetName(typeof(SigningAlgorithm), algorithm);
    if (algorithmName is null) throw new InvalidOperationException("Unsupported signing algorithm.");

    if (algorithmName.StartsWith("ES", StringComparison.OrdinalIgnoreCase))
    {
        using var ecdsa = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        var request = new CertificateRequest(subjectName, ecdsa, HashAlgorithmName.SHA256);
        request.CertificateExtensions.Add(new X509BasicConstraintsExtension(false, false, 0, false));
        request.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature, false));
        request.CertificateExtensions.Add(new X509SubjectKeyIdentifierExtension(request.PublicKey, false));
        return request.CreateSelfSigned(notBefore, notAfter);
    }

    if (!algorithmName.StartsWith("RS", StringComparison.OrdinalIgnoreCase)
        && !algorithmName.StartsWith("PS", StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException("Unsupported signing algorithm.");
    {
        using var rsa = RSA.Create(2048);
        var request = new CertificateRequest(subjectName, rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        request.CertificateExtensions.Add(new X509BasicConstraintsExtension(false, false, 0, false));
        request.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature, false));
        request.CertificateExtensions.Add(new X509SubjectKeyIdentifierExtension(request.PublicKey, false));
        return request.CreateSelfSigned(notBefore, notAfter);
    }
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

namespace Zentra.DemoServerApp
{
    internal partial class Program
    {
        [GeneratedRegex(@"\$\{([A-Z0-9_]+)\}")]
        internal static partial Regex MyRegex();
    }
}
