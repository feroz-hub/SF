using System.Runtime.CompilerServices;
using HCL.CS.Domain;

namespace HCL.CS.DomainServices.Infra;

public interface ILoggerInstance
{
    void InitiateLoggerInstance(List<LogConfig> logConfig);

    void InitiateLoggerInstance(LogConfig logConfig);

    ILoggerService GetLoggerInstance(string instanceName);
}

public interface ILoggerService
{
    void SetLoggedUserName(string userName);

    void WriteTo(Log loggingOption, string message, params object[] propertyValues);

    void WriteTo(Log loggingOption, Exception exception, string message, params object[] propertyValues);

    void WriteToWithCaller(Log loggingOption, string message, object[] propertyValues = null,
        [CallerMemberName] string callerMemberName = null, [CallerFilePath] string sourceFilePath = null);

    void WriteToWithCaller(Log loggingOption, Exception exception, string message, object[] propertyValues = null,
        [CallerMemberName] string callerMemberName = null, [CallerFilePath] string sourceFilePath = null);
}
