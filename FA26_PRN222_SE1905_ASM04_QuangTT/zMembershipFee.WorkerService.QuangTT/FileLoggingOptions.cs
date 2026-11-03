using Microsoft.Extensions.Logging;

namespace zMembershipFee.WorkerService.QuangTT;

/// <summary>
/// Tùy chọn ghi log vào file txt.
/// Đường dẫn file = Path.Combine(logRoot, LogDirectoryName, LogFileName).
/// Khi chạy Development, logRoot là thư mục project Worker Service; sau publish,
/// logRoot là thư mục chứa file executable.
/// </summary>
public class FileLoggingOptions
{
    public const string SectionName = "FileLogging";

    /// <summary>Thư mục chứa file log, tính tương đối so với thư mục chạy.</summary>
    public string LogDirectoryName { get; set; } = "Logs";

    /// <summary>Tên file log (bao gồm phần mở rộng .txt).</summary>
    public string LogFileName { get; set; } = "QuangTT_SE1905_WS.txt";

    /// <summary>Mức log tối thiểu được ghi vào file.</summary>
    public LogLevel MinimumLogLevel { get; set; } = LogLevel.Information;

    /// <summary>Đường dẫn đầy đủ của file log.</summary>
    public string GetFullPath(string logRoot)
        => Path.Combine(logRoot, LogDirectoryName, LogFileName);
}
