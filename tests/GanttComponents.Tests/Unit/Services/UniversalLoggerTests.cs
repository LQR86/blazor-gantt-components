using Microsoft.Extensions.Logging;
using GanttComponents.Services;
using Moq;
using Xunit;

namespace GanttComponents.Tests.Unit.Services;

public class UniversalLoggerTests
{
    private readonly Mock<ILogger<UniversalLogger>> _mockLogger;
    private readonly UniversalLogger _universalLogger;

    public UniversalLoggerTests()
    {
        _mockLogger = new Mock<ILogger<UniversalLogger>>();
        _universalLogger = new UniversalLogger(_mockLogger.Object);
    }

    [Fact]
    public void LogOperation_WithData_LogsCorrectly()
    {
        // Arrange
        var category = "TEST";
        var operation = "TestOperation";
        var data = new { Property = "Value", Number = 42 };

        // Act
        _universalLogger.LogOperation(category, operation, data);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("🔧 TEST: TestOperation")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void LogOperation_WithoutData_LogsCorrectly()
    {
        // Arrange
        var category = "TEST";
        var operation = "TestOperation";

        // Act
        _universalLogger.LogOperation(category, operation);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("🔧 TEST: TestOperation")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void LogError_WithException_LogsCorrectly()
    {
        // Arrange
        var message = "Test error message";
        var exception = new InvalidOperationException("Test exception");

        // Act
        _universalLogger.LogError(message, exception);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("❌ ERROR: Test error message")),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void LogError_WithoutException_LogsCorrectly()
    {
        // Arrange
        var message = "Test error message";

        // Act
        _universalLogger.LogError(message);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("❌ ERROR: Test error message")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void LogDebugInfo_WithData_LogsCorrectly()
    {
        // Arrange
        var message = "Debug message";
        var data = new { DebugProperty = "DebugValue" };

        // Act
        _universalLogger.LogDebugInfo(message, data);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Debug,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("🔍 DEBUG: Debug message")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void LogWarning_WithData_LogsCorrectly()
    {
        // Arrange
        var message = "Warning message";
        var data = new { WarningProperty = "WarningValue" };

        // Act
        _universalLogger.LogWarning(message, data);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("⚠️ WARNING: Warning message")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void LogInfo_WithData_LogsCorrectly()
    {
        // Arrange
        var message = "Info message";
        var data = new { InfoProperty = "InfoValue" };

        // Act
        _universalLogger.LogInfo(message, data);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("ℹ️ INFO: Info message")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Theory]
    [InlineData("CreateTask")]
    [InlineData("UpdateTask")]
    [InlineData("DeleteTask")]
    public void LogTaskGridOperation_LogsWithCorrectCategory(string operation)
    {
        // Act
        _universalLogger.LogTaskGridOperation(operation);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("🔧 TASKGRID:") && v.ToString()!.Contains(operation)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Theory]
    [InlineData("RenderTimeline")]
    [InlineData("UpdateScale")]
    [InlineData("DrawTaskBars")]
    public void LogTimelineOperation_LogsWithCorrectCategory(string operation)
    {
        // Act
        _universalLogger.LogTimelineOperation(operation);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("🔧 TIMELINE:") && v.ToString()!.Contains(operation)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Theory]
    [InlineData("AlignRows")]
    [InlineData("SynchronizeScroll")]
    public void LogRowAlignment_LogsWithCorrectCategory(string operation)
    {
        // Act
        _universalLogger.LogRowAlignment(operation);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("🔧 ROW-ALIGNMENT:") && v.ToString()!.Contains(operation)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Theory]
    [InlineData("GenerateWbs")]
    [InlineData("ValidateWbs")]
    public void LogWbsOperation_LogsWithCorrectCategory(string operation)
    {
        // Act
        _universalLogger.LogWbsOperation(operation);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("🔧 WBS:") && v.ToString()!.Contains(operation)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Theory]
    [InlineData("CreateDependency")]
    [InlineData("DeleteDependency")]
    public void LogDependencyOperation_LogsWithCorrectCategory(string operation)
    {
        // Act
        _universalLogger.LogDependencyOperation(operation);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("🔧 DEPENDENCY:") && v.ToString()!.Contains(operation)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void LogDateOperation_WithValidDate_LogsCorrectly()
    {
        // Arrange
        var taskName = "Task 1";
        var date = new DateOnly(2025, 7, 27);
        var dateType = "StartDate";

        // Act
        _universalLogger.LogDateOperation(taskName, date, dateType);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("📅 DATE: Task 1") 
                    && v.ToString()!.Contains("StartDate: 2025-07-27")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void LogDateOperation_WithNullDate_LogsNullCorrectly()
    {
        // Arrange
        var taskName = "Task 1";
        DateOnly? date = null;
        var dateType = "EndDate";

        // Act
        _universalLogger.LogDateOperation(taskName, date, dateType);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("📅 DATE: Task 1") 
                    && v.ToString()!.Contains("EndDate: NULL")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void LogDurationCalculation_WithContext_LogsCorrectly()
    {
        // Arrange
        var operation = "CalculateProjectDuration";
        var days = 15;
        var context = new { ProjectId = 123, TaskCount = 5 };

        // Act
        _universalLogger.LogDurationCalculation(operation, days, context);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("⏱️ DURATION: CalculateProjectDuration") 
                    && v.ToString()!.Contains("Days: 15")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void LogComponentLifecycle_LogsCorrectly()
    {
        // Arrange
        var componentName = "TaskGrid";
        var lifecycle = "OnInitialized";
        var data = new { TaskCount = 10 };

        // Act
        _universalLogger.LogComponentLifecycle(componentName, lifecycle, data);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("🔄 COMPONENT: TaskGrid") 
                    && v.ToString()!.Contains("OnInitialized")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void LogStateChange_LogsCorrectly()
    {
        // Arrange
        var component = "TaskGrid";
        var property = "SelectedTaskId";
        var oldValue = 1;
        var newValue = 2;

        // Act
        _universalLogger.LogStateChange(component, property, oldValue, newValue);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("🔀 STATE CHANGE: TaskGrid.SelectedTaskId")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void LogUserAction_WithContext_LogsCorrectly()
    {
        // Arrange
        var action = "SelectTask";
        var context = new { TaskId = 123, UserId = "user@example.com" };

        // Act
        _universalLogger.LogUserAction(action, context);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("👤 USER ACTION: SelectTask")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void LogPerformance_WithMetadata_LogsCorrectly()
    {
        // Arrange
        var operation = "LoadTasks";
        var duration = TimeSpan.FromMilliseconds(250);
        var metadata = new { TaskCount = 100, CacheHit = true };

        // Act
        _universalLogger.LogPerformance(operation, duration, metadata);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("⚡ PERFORMANCE: LoadTasks took 250ms")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void LogDatabaseOperation_LogsWithCorrectCategory()
    {
        // Arrange
        var operation = "SaveTask";
        var data = new { TaskId = 123, Success = true };

        // Act
        _universalLogger.LogDatabaseOperation(operation, data);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("🔧 DATABASE: SaveTask")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
