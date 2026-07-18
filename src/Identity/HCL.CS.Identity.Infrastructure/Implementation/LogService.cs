/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using System.Collections.ObjectModel;
using System.Data;
using System.Runtime.CompilerServices;
using NpgsqlTypes;
using Serilog;
using Serilog.Context;
using Serilog.Events;
using Serilog.Sinks.MariaDB.Extensions;
using Serilog.Sinks.MSSqlServer;
using Serilog.Sinks.PostgreSQL;
using HCL.CS.Domain;
using HCL.CS.Domain.ErrorCodes;
using HCL.CS.DomainServices.Infra;
using ColumnOptions = Serilog.Sinks.MSSqlServer.ColumnOptions;
using Log = HCL.CS.Domain.Log;

namespace HCL.CS.Infrastructure.Services.Implementation;

public class LogService : ILoggerService
{
    private readonly IResourceStringHandler resourceStringHandler;
    private ILogger logger;

    public LogService(IResourceStringHandler resourceStringHandler)
    {
        this.resourceStringHandler = resourceStringHandler;
    }

    public void SetLoggedUserName(string userName)
    {
        LogContext.PushProperty("UserId", userName);
    }

    public void WriteTo(Log loggingOption, string message, params object[] propertyValues)
    {
        switch (loggingOption)
        {
            case Log.Debug:
                logger.Debug(message, propertyValues);
                break;
            case Log.Error:
                logger.Error(message, propertyValues);
                break;
            case Log.Information:
                logger.Information(message, propertyValues);
                break;
            case Log.Warning:
                logger.Warning(message, propertyValues);
                break;
            case Log.Fatal:
                logger.Fatal(message, propertyValues);
                break;
            case Log.Verbose:
                logger.Verbose(message, propertyValues);
                break;
        }
    }

    public void WriteTo(Log loggingOption, Exception exception, string message, params object[] propertyValues)
    {
        switch (loggingOption)
        {
            case Log.Debug:
                logger.Debug(exception, message, propertyValues);
                break;
            case Log.Error:
                logger.Error(exception, message, propertyValues);
                break;
            case Log.Information:
                logger.Information(exception, message, propertyValues);
                break;
            case Log.Warning:
                logger.Warning(exception, message, propertyValues);
                break;
            case Log.Fatal:
                logger.Fatal(exception, message, propertyValues);
                break;
            case Log.Verbose:
                logger.Verbose(exception, message, propertyValues);
                break;
        }
    }

    public void WriteToWithCaller(
        Log loggingOption,
        string message,
        object[] propertyValues = null,
        [CallerMemberName] string callerMemberName = null,
        [CallerFilePath] string sourceFilePath = null)
    {
        var fileName = Path.GetFileName(sourceFilePath);
        switch (loggingOption)
        {
            case Log.Debug:
                logger.ForContext("MethodName", callerMemberName).ForContext("FileName", fileName)
                    .Debug(message, propertyValues);
                break;
            case Log.Error:
                logger.ForContext("MethodName", callerMemberName).ForContext("FileName", fileName)
                    .Error(message, propertyValues);
                break;
            case Log.Information:
                logger.ForContext("MethodName", callerMemberName).ForContext("FileName", fileName)
                    .Information(message, propertyValues);
                break;
            case Log.Warning:
                logger.ForContext("MethodName", callerMemberName).ForContext("FileName", fileName)
                    .Warning(message, propertyValues);
                break;
            case Log.Fatal:
                logger.ForContext("MethodName", callerMemberName).ForContext("FileName", fileName)
                    .Fatal(message, propertyValues);
                break;
            case Log.Verbose:
                logger.ForContext("MethodName", callerMemberName).ForContext("FileName", fileName)
                    .Verbose(message, propertyValues);
                break;
        }
    }

    public void WriteToWithCaller(
        Log loggingOption,
        Exception exception,
        string message,
        object[] propertyValues = null,
        [CallerMemberName] string callerMemberName = null,
        [CallerFilePath] string sourceFilePath = null)
    {
        var fileName = Path.GetFileName(sourceFilePath);
        switch (loggingOption)
        {
            case Log.Debug:
                logger.ForContext("MethodName", callerMemberName).ForContext("FileName", fileName)
                    .Debug(exception, message, propertyValues);
                break;
            case Log.Error:
                logger.ForContext("MethodName", callerMemberName).ForContext("FileName", fileName)
                    .Error(exception, message, propertyValues);
                break;
            case Log.Information:
                logger.ForContext("MethodName", callerMemberName).ForContext("FileName", fileName)
                    .Information(exception, message, propertyValues);
                break;
            case Log.Warning:
                logger.ForContext("MethodName", callerMemberName).ForContext("FileName", fileName)
                    .Warning(exception, message, propertyValues);
                break;
            case Log.Fatal:
                logger.ForContext("MethodName", callerMemberName).ForContext("FileName", fileName)
                    .Fatal(exception, message, propertyValues);
                break;
            case Log.Verbose:
                logger.ForContext("MethodName", callerMemberName).ForContext("FileName", fileName)
                    .Verbose(exception, message, propertyValues);
                break;
        }
    }

    internal void InitializeConfiguration(LogConfig logConfig)
    {
        var loggerConfiguration = new LoggerConfiguration();
        if (logConfig.WriteLogTo == WriteLogTo.DataBase)
        {
            SetMinimumLevel(loggerConfiguration, logConfig.LogDbConfig.MinimumConfiguration);
            if (string.IsNullOrWhiteSpace(logConfig.LogDbConfig.ConnectionString))
            {
                var errorMessage = resourceStringHandler.GetResourceString(ApiErrorCodes.ConnectionStringInvalid);
                throw new Exception(errorMessage);
            }

            LogDataBaseSetup(loggerConfiguration, logConfig.LogDbConfig);
        }
        else
        {
            SetMinimumLevel(loggerConfiguration, logConfig.LogFileConfig.MinimumConfiguration);
            if (string.IsNullOrWhiteSpace(logConfig.LogFileConfig.FilePath))
            {
                var errorMessage = resourceStringHandler.GetResourceString(ApiErrorCodes.InvalidLogFilePath);
                throw new Exception(errorMessage);
            }

            LogFileSetup(loggerConfiguration, logConfig.LogFileConfig);
        }

        logger = loggerConfiguration.CreateLogger();
    }

    private void LogFileSetup(LoggerConfiguration loggerConfiguration, LogFileConfig logConfig)
    {
        loggerConfiguration.WriteTo.Async(a =>
            a.File(
                logConfig.FilePath,
                rollingInterval: (RollingInterval)logConfig.RollingIntervalType,
                rollOnFileSizeLimit: logConfig.SetLogFileSize,
                fileSizeLimitBytes: logConfig.FileSizeInBytes,
                restrictedToMinimumLevel: (LogEventLevel)logConfig.RestrictedToMinimumLevel,
                outputTemplate: logConfig.OutputFormat,
                shared: true));
    }

    private void LogDataBaseSetup(LoggerConfiguration loggerConfiguration, LogDbConfig logConfig)
    {
        if (logConfig.Database == DbTypes.SqlServer)
            SetupMsSqlServer(loggerConfiguration, logConfig);
        else if (logConfig.Database == DbTypes.MySql)
            SetupMySql(loggerConfiguration, logConfig);
        else if (logConfig.Database == DbTypes.PostgreSQL) SetupPostGreSql(loggerConfiguration, logConfig);
    }

    private void SetupMsSqlServer(LoggerConfiguration loggerConfiguration, LogDbConfig logConfig)
    {
        var sinkOptions = new MSSqlServerSinkOptions
        {
            TableName = "HclCs_Logs",
            AutoCreateSqlTable = true
        };

        var columnOptions = new ColumnOptions
        {
            AdditionalColumns = new Collection<SqlColumn>
            {
                new()
                {
                    ColumnName = "UserId",
                    PropertyName = "UserId",
                    DataType = SqlDbType.NVarChar,
                    DataLength = 255
                }
            }
        };

        loggerConfiguration.AuditTo.MSSqlServer(
            logConfig.ConnectionString,
            restrictedToMinimumLevel: (LogEventLevel)logConfig.RestrictedToMinimumLevel,
            sinkOptions: sinkOptions,
            columnOptions: columnOptions);
    }

    private void SetupMySql(LoggerConfiguration loggerConfiguration, LogDbConfig logConfig)
    {
        loggerConfiguration.AuditTo.MariaDB(
            logConfig.ConnectionString,
            tableName: "HclCs_Logs",
            autoCreateTable: true,
            restrictedToMinimumLevel: (LogEventLevel)logConfig.RestrictedToMinimumLevel);
    }

    private void SetupPostGreSql(LoggerConfiguration loggerConfiguration, LogDbConfig logConfig)
    {
        var connectionstring = logConfig.ConnectionString;
        var tableName = "HclCs_Logs";
        IDictionary<string, ColumnWriterBase> columnWriters = new Dictionary<string, ColumnWriterBase>
        {
            { "Message", new RenderedMessageColumnWriter() },
            { "MessageTemplate", new MessageTemplateColumnWriter() },
            { "Level", new LevelColumnWriter(true, NpgsqlDbType.Varchar) },
            { "TimeStamp", new TimestampColumnWriter() },
            { "Exception", new ExceptionColumnWriter() },
            { "Properties", new LogEventSerializedColumnWriter() },
            {
                "UserId", new SinglePropertyColumnWriter("UserId", PropertyWriteMethod.ToString, NpgsqlDbType.Text, "l")
            },
            {
                "MachineName",
                new SinglePropertyColumnWriter("MachineName", PropertyWriteMethod.ToString, NpgsqlDbType.Text, "l")
            }
        };
        loggerConfiguration.WriteTo.PostgreSQL(
            connectionstring,
            tableName,
            columnWriters,
            needAutoCreateTable: true,
            restrictedToMinimumLevel: (LogEventLevel)logConfig.RestrictedToMinimumLevel);
    }

    private void SetMinimumLevel(LoggerConfiguration loggerConfiguration, Log minimumConfiguration)
    {
        switch (minimumConfiguration)
        {
            case Log.Debug:
                loggerConfiguration.MinimumLevel.Debug();
                break;
            case Log.Error:
                loggerConfiguration.MinimumLevel.Error();
                break;
            case Log.Fatal:
                loggerConfiguration.MinimumLevel.Fatal();
                break;
            case Log.Information:
                loggerConfiguration.MinimumLevel.Information();
                break;
            case Log.Verbose:
                loggerConfiguration.MinimumLevel.Verbose();
                break;
            case Log.Warning:
                loggerConfiguration.MinimumLevel.Warning();
                break;
        }

        loggerConfiguration.Enrich.WithMachineName();
        loggerConfiguration.Enrich.FromLogContext();
    }
}
