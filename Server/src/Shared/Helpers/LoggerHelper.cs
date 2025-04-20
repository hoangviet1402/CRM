using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;

namespace Shared.Helpers;

public static class LoggerHelper
{
    private static ILogger? _logger;
    private static readonly object _lock = new object();

    public static void Initialize(IConfiguration configuration)
    {
        if (_logger != null) return;

        lock (_lock)
        {
            if (_logger != null) return;

            var logConfig = configuration.GetSection("Serilog");
            var logPath = logConfig["LogFilePath"] ?? "D:\\BANCA\\Logs\\log-.txt";
            var minimumLevel = logConfig["MinimumLevel"] ?? "Debug";
            var fileSizeLimit = int.Parse(logConfig["FileSizeLimitBytes"] ?? "5242880");
            var retainedFileCount = int.Parse(logConfig["RetainedFileCountLimit"] ?? "31");

            var loggerConfig = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File(
                    logPath,
                    rollingInterval: RollingInterval.Day,
                    rollOnFileSizeLimit: true,
                    fileSizeLimitBytes: fileSizeLimit,
                    retainedFileCountLimit: retainedFileCount,
                    outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff}] [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}");

            _logger = loggerConfig.CreateLogger();
            Log.Logger = _logger;

            Information("Khởi tạo Logger thành công. Đường dẫn log file: {LogPath}", logPath);
        }
    }

    public static void Debug(string message, params object[] args)
    {
        EnsureInitialized();
        _logger?.Debug(message, args);
    }

    public static void Information(string message, params object[] args)
    {
        EnsureInitialized();
        _logger?.Information(message, args);
    }

    public static void Warning(string message, params object[] args)
    {
        EnsureInitialized();
        _logger?.Warning(message, args);
    }

    public static void Error(string message, Exception? exception = null, params object[] args)
    {
        EnsureInitialized();
        if (exception != null)
            _logger?.Error(exception, message, args);
        else
            _logger?.Error(message, args);
    }

    public static void Critical(string message, Exception? exception = null, params object[] args)
    {
        EnsureInitialized();
        if (exception != null)
            _logger?.Fatal(exception, message, args);
        else
            _logger?.Fatal(message, args);
    }

    private static void EnsureInitialized()
    {
        if (_logger == null)
        {
            throw new InvalidOperationException("Logger chưa được khởi tạo. Vui lòng gọi Initialize() trước khi sử dụng.");
        }
    }

    public static void CloseAndFlush()
    {
        Log.CloseAndFlush();
    }
} 