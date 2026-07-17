namespace Zentra.Domain;

public class LogConfig
{
    public string InstanceName { get; set; }

    public WriteLogTo WriteLogTo { get; set; } = WriteLogTo.File;

    public LogFileConfig LogFileConfig { get; set; } = new();

    public LogDbConfig LogDbConfig { get; set; } = new();
}

public class LogFileConfig
{
    private const string Format =
        "[{Timestamp:yyyy-MM-dd HH:mm:ss}|UserId-{UserId}|MachineName-{MachineName}|{Level}|{Message}|" +
        "{MethodName}|{FileName}|{Exception}]{NewLine}";


    // private static string logOutputFormat = "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level}] [ThreadId - {ThreadId}] [EnvironmentUserName - {EnvironmentUserName}] " +
    //    "[ProcessId - {ProcessId}] [ProcessName - {ProcessName}] [MachineName - {MachineName}] [SourceContext - {SourceContext}] " +
    //    "[Logged UserId - {UserId}] {NewLine}{Message}{NewLine}in method {MemberName} at {FilePath}:{LineNumber}{NewLine}at {Caller}{NewLine}{Exception}";

    public string FilePath { get; set; }

    public RollingIntervalType RollingIntervalType { get; set; } = RollingIntervalType.Day;

    public Log RestrictedToMinimumLevel { get; set; }

    public Log MinimumConfiguration { get; set; } = Log.Error;

    public bool SetLogFileSize { get; set; } = true;

    public long FileSizeInBytes { get; set; } = 5242880;

    public string OutputFormat { get; set; } = Format;
}

public class LogDbConfig
{
    public DbTypes Database { get; set; }

    public string ConnectionString { get; set; }

    public Log RestrictedToMinimumLevel { get; set; }

    public Log MinimumConfiguration { get; set; } = Log.Error;
}
