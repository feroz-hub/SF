using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using HCL.CS.Domain;
using HCL.CS.DomainServices.Infra;
using HCL.CS.Infrastructure.Resources.Extension;
using HCL.CS.Infrastructure.Services.Extension;
using Xunit;

namespace HCL.CS.UnitTests;

public class LoggerRegistrationTests
{
    [Fact]
    public void MultipleLoggerConfigurations_AreComposedIntoOneLoggerInstance()
    {
        var directory = Path.Combine(Path.GetTempPath(), $"hcl-cs-logger-test-{Guid.NewGuid():N}");
        Directory.CreateDirectory(directory);

        try
        {
            var services = new ServiceCollection();
            services.AddInfrastructureResources();
            services.AddSecurityLoggerInstance(CreateConfiguration(directory, "framework"));
            services.AddSecurityLoggerInstance(CreateConfiguration(directory, "authentication"));

            using var provider = services.BuildServiceProvider();
            var loggerInstance = provider.GetRequiredService<ILoggerInstance>();

            loggerInstance.GetLoggerInstance("framework").Should().NotBeNull();
            loggerInstance.GetLoggerInstance("authentication").Should().NotBeNull();
        }
        finally
        {
            if (Directory.Exists(directory)) Directory.Delete(directory, recursive: true);
        }
    }

    private static LogConfig CreateConfiguration(string directory, string instanceName)
    {
        return new LogConfig
        {
            InstanceName = instanceName,
            WriteLogTo = WriteLogTo.File,
            LogFileConfig = new LogFileConfig
            {
                FilePath = Path.Combine(directory, $"{instanceName}.log")
            }
        };
    }
}
