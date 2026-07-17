using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using HCL.CS.Domain;
using HCL.CS.DomainServices.Infra;
using HCL.CS.Infrastructure.Services.Implementation;
using HCL.CS.Infrastructure.Services.Implementation.Providers;
using HCL.CS.Infrastructure.Services.Implementation.Providers.Email;
using HCL.CS.Infrastructure.Services.Implementation.Providers.Sms;

namespace HCL.CS.Infrastructure.Services.Extension;

public static class InfrastructureServiceExtension
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddSingleton<IFrameworkResultService, FrameworkResultService>();
        services.AddSingleton<ILoggerInstance, LoggerInstance>();
        services.AddScoped<ITenantContext, HttpTenantContext>();
        services.AddTransient<IEmailService, EmailService>();
        services.AddTransient<ISmsService, SmsService>();

        // Email Providers
        services.AddTransient<IEmailProvider, SmtpEmailProvider>();
        services.AddTransient<IEmailProvider, SendGridEmailProvider>();
        services.AddTransient<IEmailProvider, BrevoEmailProvider>();
        services.AddTransient<IEmailProvider, ResendEmailProvider>();
        services.AddTransient<IEmailProvider, AmazonSesEmailProvider>();
        services.AddTransient<IEmailProvider, MailgunEmailProvider>();
        services.AddTransient<IEmailProvider, PostmarkEmailProvider>();

        // SMS Providers
        services.AddTransient<ISmsProvider, TwilioSmsProvider>();
        services.AddTransient<ISmsProvider, BrevoSmsProvider>();
        services.AddTransient<ISmsProvider, VonageSmsProvider>();
        services.AddTransient<ISmsProvider, AmazonSnsSmsProvider>();
        services.AddTransient<ISmsProvider, MessageBirdSmsProvider>();
        services.AddTransient<ISmsProvider, PlivoSmsProvider>();

        // Provider Factory
        services.AddTransient<NotificationProviderFactory>();

        return services;
    }

    public static IServiceCollection AddSecurityLoggerInstance(this IServiceCollection services,
        IServiceProvider serviceProvider, LogConfig logConfig)
    {
        var loggerInstance = serviceProvider.GetService<ILoggerInstance>();
        if (loggerInstance != null)
        {
            loggerInstance.InitiateLoggerInstance(logConfig);
            services.AddSingleton(loggerInstance);
        }

        return services;
    }
}
