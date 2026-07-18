/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

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
