using HCL.CS.Domain;
using HCL.CS.Domain.ErrorCodes;
using HCL.CS.DomainServices.Infra;
using HCL.CS.Infrastructure.Services.Implementation;

namespace HCL.CS.Infrastructure.Services.Extension;

public class LoggerInstance : ILoggerInstance
{
    private readonly Dictionary<string, ILoggerService> logInstanceCollection = new();

    private readonly IResourceStringHandler resourceStringHandler;

    public LoggerInstance(IResourceStringHandler resourceStringHandler)
    {
        this.resourceStringHandler = resourceStringHandler;
    }

    public void InitiateLoggerInstance(List<LogConfig> logConfig)
    {
        if (logConfig == null || logConfig.Count <= 0)
        {
            var errorMessage = resourceStringHandler.GetResourceString(ApiErrorCodes.LoggerConfigurationIsNull);
            throw new Exception(errorMessage);
        }

        foreach (var logOption in logConfig) InitiateLoggerInstance(logOption);
    }

    public void InitiateLoggerInstance(LogConfig logConfig)
    {
        if (logConfig == null)
        {
            var errorMessage = resourceStringHandler.GetResourceString(ApiErrorCodes.LoggerConfigurationIsNull);
            throw new Exception(errorMessage);
        }

        var loggerService = new LogService(resourceStringHandler);
        loggerService.InitializeConfiguration(logConfig);
        Register(logConfig.InstanceName, loggerService);
    }

    public ILoggerService GetLoggerInstance(string instanceName)
    {
        if (instanceName != null && logInstanceCollection != null &&
            logInstanceCollection.ContainsKey(instanceName))
            return logInstanceCollection[instanceName];

        return null;
    }

    private void Register(string name, LogService logService)
    {
        if (logInstanceCollection != null && !logInstanceCollection.ContainsKey(name))
            logInstanceCollection.Add(name, logService);
    }
}
