using System.Text;
using Microsoft.Extensions.Logging;

namespace zMembershipFee.WorkerService.QuangTT;

/// <summary>
/// ILoggerProvider ghi log vào một file txt duy nhất (append). Provider giữ mở StreamWriter
/// và dispose khi host dừng (khi service Stop trong services.msc).
/// </summary>
public sealed class FileLoggerProvider : ILoggerProvider
{
    private readonly Encoding _encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
    private readonly FileLoggingOptions _options;
    private readonly string _logRoot;
    private readonly object _sync = new();
    private StreamWriter? _writer;

    public FileLoggerProvider(FileLoggingOptions options, string logRoot)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _logRoot = logRoot ?? throw new ArgumentNullException(nameof(logRoot));
    }

    internal FileLoggingOptions Options => _options;

    private TextWriter GetWriter()
    {
        if (_writer is null)
        {
            var fullPath = _options.GetFullPath(_logRoot);
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
            _writer = new StreamWriter(fullPath, append: true, _encoding) { AutoFlush = false };
        }

        return _writer;
    }

    public ILogger CreateLogger(string categoryName)
        => new FileLogger(this, categoryName);

    internal void Write(string message)
    {
        lock (_sync)
        {
            var writer = GetWriter();
            writer.Write(message);
            writer.Flush();
        }
    }

    public void Dispose()
    {
        lock (_sync)
        {
            _writer?.Flush();
            _writer?.Dispose();
            _writer = null;
        }

        GC.SuppressFinalize(this);
    }
}
