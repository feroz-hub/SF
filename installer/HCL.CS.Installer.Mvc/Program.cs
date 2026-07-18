/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.HttpOverrides;
using Serilog;
using HclCsInstallerMVC.Application.Abstractions;
using HclCsInstallerMVC.Application.Services;
using HclCsInstallerMVC.Infrastructure.Configuration;
using HclCsInstallerMVC.Infrastructure.Persistence;
using HclCsInstallerMVC.Infrastructure.Services;
using HclCsInstallerMVC.Middleware;
using HclCsInstallerMVC.Services;
using HclCsInstallerMVC.ViewModels.Validators;

var builder = WebApplication.CreateBuilder(args);
var trustProxyHeaders = string.Equals(
    Environment.GetEnvironmentVariable("HCL_CS_TRUST_PROXY_HEADERS"),
    "true",
    StringComparison.OrdinalIgnoreCase);
var railwayPort = Environment.GetEnvironmentVariable("PORT");

if (!string.IsNullOrWhiteSpace(railwayPort))
    builder.WebHost.UseUrls($"http://+:{railwayPort}");

builder.Host.UseSerilog((context, services, loggerConfiguration) => loggerConfiguration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .WriteTo.Console());

builder.Services.Configure<InstallerLockOptions>(builder.Configuration.GetSection("InstallerLock"));
builder.Services.Configure<DatabaseProvisioningOptions>(builder.Configuration);

var dataProtectionKeysPath = Environment.GetEnvironmentVariable("HCL_CS_INSTALLER_DATA_PROTECTION_KEYS_PATH");
var dataProtectionBuilder = builder.Services.AddDataProtection();
if (!string.IsNullOrWhiteSpace(dataProtectionKeysPath))
{
    Directory.CreateDirectory(dataProtectionKeysPath);
    dataProtectionBuilder.PersistKeysToFileSystem(new DirectoryInfo(dataProtectionKeysPath));
}

if (trustProxyHeaders)
{
    builder.Services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        options.KnownNetworks.Clear();
        options.KnownProxies.Clear();
    });
}

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.Name = "HclCsInstaller.Session";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});

builder.Services
    .AddControllersWithViews(options => { options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()); });
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();

builder.Services.AddValidatorsFromAssemblyContaining<SetupProviderViewModelValidator>();

builder.Services.AddHealthChecks()
    .AddCheck<InstallationHealthCheck>("installation_state");
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IInstallerStateStore, ProtectedInstallerStateStore>();
builder.Services.AddScoped<IInstallationGateService, InstallationGateService>();
builder.Services.AddScoped<IDatabaseMigrationService, DatabaseMigrationService>();
builder.Services.AddScoped<ISeedDataService, SeedDataService>();
builder.Services.AddScoped<IInstallerService, InstallerService>();
builder.Services.AddScoped<IInstallerWorkflowService, InstallerWorkflowService>();

var app = builder.Build();

if (trustProxyHeaders)
    app.UseForwardedHeaders();

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseSerilogRequestLogging();

if (!app.Environment.IsDevelopment()) app.UseHsts();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseMiddleware<SetupRedirectMiddleware>();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapHealthChecks("/health");
app.MapControllers();

app.Run();
