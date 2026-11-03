using Microsoft.Extensions.Logging;

namespace zMembershipFee.WorkerService.QuangTT;

/// <summary>
/// ILogger ghi một dòng vào file txt (UTF-8, append). An toàn cho việc ghi đa luồng nhờ lock ở provider.
/// </summary>
public sealed class FileLogger : ILogger
{
    private readonly FileLoggerProvider _provider;
    private readonly string _categoryName;

    public FileLogger(FileLoggerProvider provider, string categoryName)
    {
        _provider = provider;
        _categoryName = categoryName;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel)
        => logLevel != LogLevel.None && logLevel >= _provider.Options.MinimumLogLevel;

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        var line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] [{logLevel,-8}] [{_categoryName}] " +
                   formatter(state, exception);

        if (exception is not null)
        {
            line += Environment.NewLine + exception;
        }

        _provider.Write(line + Environment.NewLine);
    }
}
