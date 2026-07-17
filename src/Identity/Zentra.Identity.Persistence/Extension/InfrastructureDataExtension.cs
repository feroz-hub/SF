using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Zentra.Domain;
using Zentra.Domain.Entities.Api;
using Zentra.Domain.Entities.Endpoint;
using Zentra.DomainServices;
using Zentra.DomainServices.Infra;
using Zentra.DomainServices.Repository.Api;
using Zentra.DomainServices.UnitOfWork.Api;
using Zentra.DomainServices.UnitOfWork.Endpoint;
using Zentra.DomainServices.Wrappers;
using Zentra.Infrastructure.Data.Repository.Api;
using Zentra.Infrastructure.Data.UnitOfWork.Api;
using Zentra.Infrastructure.Data.UnitOfWork.Endpoint;
using Zentra.Infrastructure.Data.Validation;
using Zentra.Infrastructure.Data.Wrappers;

namespace Zentra.Infrastructure.Data.Extension;

public static class InfrastructureDataExtension
{
    public static IServiceCollection AddIdentityConfiguration(this IServiceCollection services)
    {
        var configSettings = GetRegisteredConfiguration(services);

        if (configSettings.SystemSettings.DBConfig.Database == DbTypes.SqlServer)
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configSettings.SystemSettings.DBConfig.DBConnectionString));
        else if (configSettings.SystemSettings.DBConfig.Database == DbTypes.MySql)
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(configSettings.SystemSettings.DBConfig.DBConnectionString,
                    ServerVersion.AutoDetect(configSettings.SystemSettings.DBConfig.DBConnectionString)));
        else if (configSettings.SystemSettings.DBConfig.Database == DbTypes.PostgreSQL)
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(configSettings.SystemSettings.DBConfig.DBConnectionString));
        else if (configSettings.SystemSettings.DBConfig.Database == DbTypes.SQLite)
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(configSettings.SystemSettings.DBConfig.DBConnectionString));

        AddIdentityServices(services, configSettings);

        services.AddLogging();
        services.AddHttpContextAccessor();

        services.Configure<DataProtectionTokenProviderOptions>(opt =>
        {
            if (configSettings != null)
                opt.TokenLifespan =
                    TimeSpan.FromMinutes(configSettings.SystemSettings.UserConfig.UserTokenExpiry);
        });

        services.Configure<EmailConfirmationTokenProviderOptions>(opt =>
        {
            if (configSettings != null)
                opt.TokenLifespan =
                    TimeSpan.FromMinutes(configSettings.SystemSettings.UserConfig.EmailTokenExpiry);
        });

        services.Configure<ChangePhoneNumberTokenProviderOption>(opt =>
        {
            if (configSettings != null)
                opt.TokenLifespan =
                    TimeSpan.FromMinutes(configSettings.SystemSettings.UserConfig.OTPTokenExpiry);
        });

        services.Configure<PasswordResetTokenProviderOptions>(opt =>
        {
            if (configSettings != null)
                opt.TokenLifespan =
                    TimeSpan.FromMinutes(configSettings.SystemSettings.UserConfig.PasswordResetTokenExpiry);
        });

        services.AddTransient<IApplicationDbContext, ApplicationDbContext>();
        return services;
    }

    private static ZentraConfig GetRegisteredConfiguration(IServiceCollection services)
    {
        var configDescriptor = services.LastOrDefault(d => d.ServiceType == typeof(ZentraConfig));
        var configSettings = configDescriptor?.ImplementationInstance as ZentraConfig;
        if (configSettings == null)
            throw new InvalidOperationException("Zentra configuration has not been registered.");

        return configSettings;
    }

    public static IServiceCollection AddRepository(this IServiceCollection services)
    {
        services.AddTransient<IRepository<Notification>, BaseRepository<Notification>>();
        services.AddTransient<IRepository<NotificationProviderConfig>, BaseRepository<NotificationProviderConfig>>();
        services.AddTransient<IRepository<ExternalAuthProviderConfig>, BaseRepository<ExternalAuthProviderConfig>>();

        services.AddTransient<IAuditRepository, AuditRepository>();

        services.AddTransient<IRepository<Clients>, BaseRepository<Clients>>();
        services.AddTransient<IRepository<ClientRedirectUris>, BaseRepository<ClientRedirectUris>>();
        services
            .AddTransient<IRepository<ClientPostLogoutRedirectUris>, BaseRepository<ClientPostLogoutRedirectUris>>();

        services.AddTransient<IApiResourceRepository, ApiResourceRepository>();
        services.AddTransient<IRepository<ApiResources>, BaseRepository<ApiResources>>();
        services.AddTransient<IRepository<ApiResourceClaims>, BaseRepository<ApiResourceClaims>>();
        services.AddTransient<IRepository<ApiScopes>, BaseRepository<ApiScopes>>();
        services.AddTransient<IRepository<ApiScopeClaims>, BaseRepository<ApiScopeClaims>>();

        services.AddTransient<IIdentityResourceRepository, IdentityResourceRepository>();
        services.AddTransient<IRepository<IdentityResources>, BaseRepository<IdentityResources>>();
        services.AddTransient<IRepository<IdentityClaims>, BaseRepository<IdentityClaims>>();

        services.AddTransient<IRepository<SecurityQuestions>, BaseRepository<SecurityQuestions>>();
        services.AddTransient<IUserManagementUnitOfWork, UserManagementUnitOfWork>();
        services.AddTransient<IUserRepository, UserRepository>();
        services.AddTransient<IRepository<ExternalIdentities>, BaseRepository<ExternalIdentities>>();
        services.AddTransient<IRepository<PasswordHistory>, BaseRepository<PasswordHistory>>();
        services.AddTransient<IRepository<SecurityTokens>, BaseRepository<SecurityTokens>>();
        services.AddTransient<IRepository<UserSecurityQuestions>, BaseRepository<UserSecurityQuestions>>();

        services.AddTransient<IRoleRepository, RoleRepository>();
        services.AddTransient<ISecurityTokenCommandRepository, SecurityTokenCommandRepository>();
        services.AddTransient<IDbConnectionValidator, DbConnectionValidator>();
        services.AddTransient<IUserRoleRepository, UserRoleRepository>();
        services.AddTransient<IRoleClaimsRepository, RoleClaimsRepository>();
        services.AddTransient<IRoleManagementUnitOfWork, RoleManagementUnitOfWork>();
        services.AddTransient<IUserTokenRepository, UserTokenRepository>();
        services.AddTransient<ISecurityTokenRepository, SecurityTokenRepository>();

        services.AddTransient<IUserClaimRepository, UserClaimRepository>();
        services.AddTransient<IClientsUnitOfWork, ClientsUnitOfWork>();
        services.AddTransient<IResourceUnitOfWork, ResourceUnitOfWork>();
        return services;
    }

    private static IServiceCollection AddIdentityServices(IServiceCollection services, ZentraConfig config)
    {
        services.AddIdentity<Users, Roles>(options =>
            {
                options.Password.RequiredLength = config.SystemSettings.PasswordConfig.MinPasswordLength;
                options.Password.RequiredUniqueChars = config.SystemSettings.PasswordConfig.RequiredUniqueChars;
                options.Password.RequireDigit = config.SystemSettings.PasswordConfig.RequireDigit;
                options.Password.RequireLowercase = config.SystemSettings.PasswordConfig.RequireLowercase;
                options.Password.RequireNonAlphanumeric = config.SystemSettings.PasswordConfig.RequireSpecialChar;
                options.Password.RequireUppercase = config.SystemSettings.PasswordConfig.RequireUppercase;

                options.User.RequireUniqueEmail = config.SystemSettings.UserConfig.RequireUniqueEmail;

                options.SignIn.RequireConfirmedEmail = config.SystemSettings.UserConfig.RequireConfirmedEmail;
                options.SignIn.RequireConfirmedPhoneNumber =
                    config.SystemSettings.UserConfig.RequireConfirmedPhoneNumber;

                options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
                options.Tokens.ChangePhoneNumberTokenProvider = TokenOptions.DefaultPhoneProvider;
                options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;

                options.Lockout.AllowedForNewUsers = config.SystemSettings.UserConfig.LockOutAllowedForNewUsers;
                options.Lockout.DefaultLockoutTimeSpan =
                    TimeSpan.FromMinutes(config.SystemSettings.UserConfig.DefaultLockoutTimeSpanMin);
                options.Lockout.MaxFailedAccessAttempts = config.SystemSettings.UserConfig.MaxFailedAccessAttempts;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddUserStore<UserStoreWrapper>()
            .AddRoleStore<RoleStoreWrapper>()
            .AddUserManager<UserManagerWrapper<Users>>()
            .AddRoleManager<RoleManagerWrapper<Roles>>()
            .AddSignInManager<SignInManagerWrapper<Users>>()
            .AddTokenProvider<EmailConfirmationTokenProvider<Users>>(TokenOptions.DefaultEmailProvider)
            .AddTokenProvider<ChangePhoneNumberTokenProvider<Users>>(TokenOptions.DefaultPhoneProvider)
            .AddTokenProvider<PasswordResetTokenProvider<Users>>(TokenOptions.DefaultEmailProvider)
            .AddTokenProvider<UserTokenProvider<Users>>(TokenOptions.DefaultProvider)
            .AddDefaultTokenProviders();
        return services;
    }
}
