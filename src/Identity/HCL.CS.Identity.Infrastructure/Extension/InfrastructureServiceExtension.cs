/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
        LogConfig logConfig)
    {
        var registration = services
            .Where(descriptor => descriptor.ServiceType == typeof(LoggerInstanceRegistration))
            .Select(descriptor => descriptor.ImplementationInstance)
            .OfType<LoggerInstanceRegistration>()
            .SingleOrDefault();

        if (registration == null)
        {
            registration = new LoggerInstanceRegistration();
            services.AddSingleton(registration);
            services.RemoveAll<ILoggerInstance>();
            services.AddSingleton<ILoggerInstance>(serviceProvider =>
            {
                var loggerInstance = new LoggerInstance(
                    serviceProvider.GetRequiredService<IResourceStringHandler>());
                loggerInstance.InitiateLoggerInstance(registration.Configurations.ToList());
                return loggerInstance;
            });
        }

        registration.Add(logConfig);

        return services;
    }

    private sealed class LoggerInstanceRegistration
    {
        private readonly List<LogConfig> configurations = new();

        public IReadOnlyList<LogConfig> Configurations => configurations;

        public void Add(LogConfig configuration)
        {
            if (configurations.All(existing =>
                    !string.Equals(existing.InstanceName, configuration.InstanceName, StringComparison.Ordinal)))
                configurations.Add(configuration);
        }
    }
}
