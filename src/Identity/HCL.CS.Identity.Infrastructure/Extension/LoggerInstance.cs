/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using HCL.CS.Domain;
using HCL.CS.Domain.ErrorCodes;
using HCL.CS.DomainServices.Infra;
using HCL.CS.Infrastructure.Services.Implementation;

namespace HCL.CS.Infrastructure.Services.Extension;

public class LoggerInstance : ILoggerInstance, IDisposable
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

    public void Dispose()
    {
        foreach (var loggerService in logInstanceCollection.Values.OfType<IDisposable>())
            loggerService.Dispose();

        logInstanceCollection.Clear();
        GC.SuppressFinalize(this);
    }
}
