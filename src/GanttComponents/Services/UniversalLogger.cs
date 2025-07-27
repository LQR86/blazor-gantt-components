using Serilog;
using Serilog.Events;

namespace GanttComponents.Services
{
    public static class LoggingConfiguration
    {
        public static void ConfigureLogging()
        {
            // Create logs directory if it doesn't exist
            var logsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "logs");
            Directory.CreateDirectory(logsDirectory);

            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}")
                .WriteTo.File(
                    path: Path.Combine(logsDirectory, "gantt-debug-.log"),
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}",
                    retainedFileCountLimit: 7) // Keep 7 days of logs
                .WriteTo.File(
                    path: Path.Combine(logsDirectory, "gantt-operations-.log"),
                    rollingInterval: RollingInterval.Day,
                    restrictedToMinimumLevel: LogEventLevel.Information,
                    outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}",
                    retainedFileCountLimit: 30) // Keep 30 days of operation logs
                .CreateLogger();
        }

        public static void CloseLogging()
        {
            Log.CloseAndFlush();
        }
    }

    public interface IUniversalLogger
    {
        // General logging methods
        void LogOperation(string category, string operation, object? data = null);
        void LogDebugInfo(string message, object? data = null);
        void LogError(string message, Exception? exception = null);
        void LogWarning(string message, object? data = null);
        void LogInfo(string message, object? data = null);
        
        // Gantt-specific logging methods
        void LogTaskGridOperation(string operation, object? data = null);
        void LogTimelineOperation(string operation, object? data = null);
        void LogRowAlignment(string operation, object? data = null);
        void LogWbsOperation(string operation, object? data = null);
        void LogDependencyOperation(string operation, object? data = null);
        
        // Day-level date operations (REQUIREMENT 1)
        void LogDateOperation(string taskName, DateOnly? date, string dateType);
        void LogDurationCalculation(string operation, int days, object? context = null);
        
        // Blazor-specific operations
        void LogComponentLifecycle(string componentName, string lifecycle, object? data = null);
        void LogStateChange(string component, string property, object? oldValue, object? newValue);
        
        // General operations (from original)
        void LogUserAction(string action, object? context = null);
        void LogPerformance(string operation, TimeSpan duration, object? metadata = null);
        void LogDatabaseOperation(string operation, object? data = null);
    }

    public class UniversalLogger : IUniversalLogger
    {
        private readonly ILogger<UniversalLogger> _logger;

        public UniversalLogger(ILogger<UniversalLogger> logger)
        {
            _logger = logger;
        }

        // General logging methods
        public void LogOperation(string category, string operation, object? data = null)
        {
            if (data != null)
            {
                _logger.LogInformation("🔧 {Category}: {Operation} | Data: {@Data}", category, operation, data);
            }
            else
            {
                _logger.LogInformation("🔧 {Category}: {Operation}", category, operation);
            }
        }

        public void LogDebugInfo(string message, object? data = null)
        {
            if (data != null)
            {
                _logger.LogDebug("🔍 DEBUG: {Message} | Data: {@Data}", message, data);
            }
            else
            {
                _logger.LogDebug("🔍 DEBUG: {Message}", message);
            }
        }

        public void LogError(string message, Exception? exception = null)
        {
            _logger.LogError(exception, "❌ ERROR: {Message}", message);
        }

        public void LogWarning(string message, object? data = null)
        {
            if (data != null)
            {
                _logger.LogWarning("⚠️ WARNING: {Message} | Data: {@Data}", message, data);
            }
            else
            {
                _logger.LogWarning("⚠️ WARNING: {Message}", message);
            }
        }

        public void LogInfo(string message, object? data = null)
        {
            if (data != null)
            {
                _logger.LogInformation("ℹ️ INFO: {Message} | Data: {@Data}", message, data);
            }
            else
            {
                _logger.LogInformation("ℹ️ INFO: {Message}", message);
            }
        }

        // Gantt-specific logging methods
        public void LogTaskGridOperation(string operation, object? data = null)
        {
            LogOperation("TASKGRID", operation, data);
        }

        public void LogTimelineOperation(string operation, object? data = null)
        {
            LogOperation("TIMELINE", operation, data);
        }

        public void LogRowAlignment(string operation, object? data = null)
        {
            LogOperation("ROW-ALIGNMENT", operation, data);
        }

        public void LogWbsOperation(string operation, object? data = null)
        {
            LogOperation("WBS", operation, data);
        }

        public void LogDependencyOperation(string operation, object? data = null)
        {
            LogOperation("DEPENDENCY", operation, data);
        }

        // Day-level date operations (REQUIREMENT 1)
        public void LogDateOperation(string taskName, DateOnly? date, string dateType)
        {
            var dateStr = date?.ToString("yyyy-MM-dd") ?? "NULL";
            _logger.LogInformation("📅 DATE: {TaskName} | {DateType}: {Date}", taskName, dateType, dateStr);
        }

        public void LogDurationCalculation(string operation, int days, object? context = null)
        {
            if (context != null)
            {
                _logger.LogInformation("⏱️ DURATION: {Operation} | Days: {Days} | Context: {@Context}", operation, days, context);
            }
            else
            {
                _logger.LogInformation("⏱️ DURATION: {Operation} | Days: {Days}", operation, days);
            }
        }

        // Blazor-specific operations
        public void LogComponentLifecycle(string componentName, string lifecycle, object? data = null)
        {
            if (data != null)
            {
                _logger.LogInformation("🔄 COMPONENT: {ComponentName} | {Lifecycle} | Data: {@Data}", componentName, lifecycle, data);
            }
            else
            {
                _logger.LogInformation("🔄 COMPONENT: {ComponentName} | {Lifecycle}", componentName, lifecycle);
            }
        }

        public void LogStateChange(string component, string property, object? oldValue, object? newValue)
        {
            _logger.LogInformation("🔀 STATE CHANGE: {Component}.{Property} | Old: {@OldValue} | New: {@NewValue}", 
                component, property, oldValue, newValue);
        }

        // General operations (from original)
        public void LogUserAction(string action, object? context = null)
        {
            if (context != null)
            {
                _logger.LogInformation("👤 USER ACTION: {Action} | Context: {@Context}", action, context);
            }
            else
            {
                _logger.LogInformation("👤 USER ACTION: {Action}", action);
            }
        }

        public void LogPerformance(string operation, TimeSpan duration, object? metadata = null)
        {
            if (metadata != null)
            {
                _logger.LogInformation("⚡ PERFORMANCE: {Operation} took {Duration}ms | Metadata: {@Metadata}", 
                    operation, duration.TotalMilliseconds, metadata);
            }
            else
            {
                _logger.LogInformation("⚡ PERFORMANCE: {Operation} took {Duration}ms", 
                    operation, duration.TotalMilliseconds);
            }
        }

        public void LogDatabaseOperation(string operation, object? data = null)
        {
            LogOperation("DATABASE", operation, data);
        }
    }
}
