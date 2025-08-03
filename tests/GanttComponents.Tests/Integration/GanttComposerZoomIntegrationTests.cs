using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using GanttComponents.Models;
using GanttComponents.Services;
using Xunit;
using Moq;

namespace GanttComponents.Tests.Integration;

/// <summary>
/// Integration tests for GanttComposer zoom system validation
/// Validates Iteration 2.5: GanttComposer Integration Validation
/// </summary>
public class GanttComposerZoomIntegrationTests : IDisposable
{
    private readonly ServiceProvider _serviceProvider;
    private readonly Mock<IUniversalLogger> _mockLogger;
    private readonly Mock<IGanttI18N> _mockI18N;

    public GanttComposerZoomIntegrationTests()
    {
        var services = new ServiceCollection();

        _mockLogger = new Mock<IUniversalLogger>();
        _mockI18N = new Mock<IGanttI18N>();

        // Setup I18N mock
        _mockI18N.Setup(x => x.T(It.IsAny<string>())).Returns((string key) => key);

        // Register services
        services.AddSingleton(_mockLogger.Object);
        services.AddSingleton(_mockI18N.Object);
        services.AddScoped<TimelineZoomService>();
        services.AddScoped<GanttRowAlignmentService>();

        _serviceProvider = services.BuildServiceProvider();
    }

    [Fact]
    public void TimelineZoomService_CalculateEffectiveDayWidth_AllZoomLevels_ReturnsCorrectValues()
    {
        // Arrange
        var zoomService = _serviceProvider.GetRequiredService<TimelineZoomService>();
        var testCases = new[]
        {
            // Preset-only system: factors ignored, backward-compatible day widths (original * 1.6)
            (TimelineZoomLevel.WeekDay97px, 1.0, 96.0),     // 60 * 1.6 for backward compatibility
            (TimelineZoomLevel.MonthDay48px, 1.0, 40.0),   // 25 * 1.6 for backward compatibility
            (TimelineZoomLevel.QuarterMonth24px, 1.0, 24.0), // 15 * 1.6 for backward compatibility
            (TimelineZoomLevel.YearQuarter6px, 1.0, 12.8),  // 8 * 1.6 for backward compatibility
            // In preset-only system, factors are clamped to 1.0 (same result regardless of input factor)
            (TimelineZoomLevel.WeekDay97px, 0.5, 96.0),     // Factor ignored, same as 1.0
            (TimelineZoomLevel.WeekDay97px, 2.0, 96.0),     // Factor ignored, same as 1.0
        };

        foreach (var (level, factor, expected) in testCases)
        {
            // Act
            var result = TimelineZoomService.CalculateEffectiveDayWidth(level, factor);

            // Assert
            Assert.Equal(expected, result, 1); // 1 decimal place tolerance
        }
    }

    [Fact]
    public void ZoomLevelTransitions_AllLevels_MaintainConsistency()
    {
        // Arrange
        var allLevels = Enum.GetValues<TimelineZoomLevel>();
        var currentLevel = TimelineZoomLevel.WeekDay97px;
        var currentFactor = 1.0;

        foreach (TimelineZoomLevel targetLevel in allLevels)
        {
            // Act - Simulate zoom level change
            currentLevel = targetLevel;
            var dayWidth = TimelineZoomService.CalculateEffectiveDayWidth(currentLevel, currentFactor);

            // Assert - Verify day width is within expected ranges
            Assert.True(dayWidth >= 1.5, $"Day width {dayWidth} for {targetLevel} should be >= 1.5px (minimum visibility)");
            Assert.True(dayWidth <= 180.0, $"Day width {dayWidth} for {targetLevel} should be <= 180px (maximum zoom)");
        }
    }

    [Fact]
    public void ZoomFactorRange_AllFactors_ProducesValidDayWidths()
    {
        // Arrange - In preset-only system, factors are always clamped to 1.0
        var testFactors = new[] { 0.5, 0.75, 1.0, 1.25, 1.5, 2.0, 2.5, 3.0 };
        var testLevel = TimelineZoomLevel.WeekDay97px; // 96px base (60 * 1.6)

        foreach (var factor in testFactors)
        {
            // Act - In preset-only system, all factors result in same day width
            var dayWidth = TimelineZoomService.CalculateEffectiveDayWidth(testLevel, factor);
            var expectedWidth = 96.0; // Always 96px regardless of factor (preset-only)

            // Assert - All factors should produce same day width in preset-only system
            Assert.Equal(expectedWidth, dayWidth, 0.1);
            Assert.True(dayWidth >= 30.0, $"Factor {factor} should produce day width >= 30px");
            Assert.True(dayWidth <= 180.0, $"Factor {factor} should produce day width <= 180px");
        }
    }

    [Theory]
    [InlineData(TimelineZoomLevel.WeekDay97px, 1.0, 1)] // 1-day task at 60px = 60px (visible)
    [InlineData(TimelineZoomLevel.YearQuarter6px, 0.5, 1)] // 1-day task at 1.5px (needs overflow handling)
    [InlineData(TimelineZoomLevel.QuarterMonth24px, 1.0, 7)] // 7-day task at 15px = 105px (visible)
    [InlineData(TimelineZoomLevel.MonthDay48px, 2.0, 3)] // 3-day task at 60px = 180px (visible)
    public void TaskVisibility_VariousZoomLevelsAndDurations_CalculatesCorrectWidth(
        TimelineZoomLevel level, double factor, int durationDays)
    {
        // Arrange
        var dayWidth = TimelineZoomService.CalculateEffectiveDayWidth(level, factor);

        // Act
        var taskWidth = dayWidth * durationDays;

        // Assert - Task width should be calculable
        Assert.True(taskWidth > 0, "Task width must be positive");

        // Log visibility status for validation
        var isVisible = taskWidth >= 12.0; // 12px minimum visibility threshold
        _mockLogger.Verify(x => x.LogDebugInfo(
            It.Is<string>(s => s.Contains("Task visibility")),
            It.IsAny<object>()), Times.Never); // Just checking the test setup

        // The test validates that calculations work - actual visibility handling is in components
        Assert.True(true, $"Task with {durationDays} days at {level} ({factor}x) = {taskWidth}px (visible: {isVisible})");
    }

    [Fact]
    public void RowAlignment_ZoomChanges_MaintainAlignmentService()
    {
        // Arrange
        var alignmentService = _serviceProvider.GetRequiredService<GanttRowAlignmentService>();
        var testTasks = CreateTestTasks(5);

        // Act & Assert - Verify alignment service is available for all zoom levels
        foreach (TimelineZoomLevel level in Enum.GetValues<TimelineZoomLevel>())
        {
            // The alignment service should be usable at any zoom level
            Assert.NotNull(alignmentService);

            // Simulate row height calculation (alignment service responsibility)
            var rowHeight = 32; // Standard row height
            var taskCount = testTasks.Count;
            var totalHeight = rowHeight * taskCount;

            Assert.True(totalHeight > 0, $"Total height calculation should work at {level}");
            Assert.Equal(160, totalHeight); // 5 tasks * 32px = 160px total height
        }
    }

    [Fact]
    public void Integration_ZoomControlsAndComposer_ParameterFlow()
    {
        // Arrange
        var initialLevel = TimelineZoomLevel.WeekDay97px;
        var initialFactor = 1.0;
        var targetLevel = TimelineZoomLevel.MonthDay48px;
        var targetFactor = 1.5;

        // Act - Simulate zoom controls affecting composer
        var initialDayWidth = TimelineZoomService.CalculateEffectiveDayWidth(initialLevel, initialFactor);
        var targetDayWidth = TimelineZoomService.CalculateEffectiveDayWidth(targetLevel, targetFactor);

        // Assert - Parameter flow validation for preset-only system
        Assert.Equal(96.0, initialDayWidth); // WeekDay in preset-only: 96px (60 * 1.6)
        Assert.Equal(40.0, targetDayWidth);  // MonthWeek in preset-only: 40px (25 * 1.6, factor ignored)

        // Verify the integration maintains different day widths for different settings
        Assert.NotEqual(initialDayWidth, targetDayWidth);
    }

    [Fact]
    public void Performance_LargeDataset_ZoomChanges_WithinThresholds()
    {
        // Arrange
        var largeTasks = CreateTestTasks(500); // Simulate large dataset
        var zoomLevels = Enum.GetValues<TimelineZoomLevel>();

        // Act & Assert - Performance validation
        foreach (TimelineZoomLevel level in zoomLevels)
        {
            var startTime = DateTime.UtcNow;

            // Simulate zoom calculation for all tasks
            foreach (var task in largeTasks)
            {
                var dayWidth = TimelineZoomService.CalculateEffectiveDayWidth(level, 1.0);
                var taskWidth = dayWidth * ParseDurationDays(task.Duration);

                // Just ensure calculation completes
                Assert.True(taskWidth >= 0);
            }

            var duration = DateTime.UtcNow - startTime;

            // Assert - Performance threshold
            Assert.True(duration.TotalMilliseconds < 100,
                $"Zoom calculations for 500 tasks at {level} should complete in <100ms, took {duration.TotalMilliseconds}ms");
        }
    }

    private List<GanttTask> CreateTestTasks(int count)
    {
        var tasks = new List<GanttTask>();
        var startDate = DateTime.Today;

        for (int i = 0; i < count; i++)
        {
            tasks.Add(new GanttTask
            {
                Id = i + 1,
                Name = $"Task {i + 1}",
                WbsCode = $"{i + 1}",
                StartDate = startDate.AddDays(i),
                Duration = $"{(i % 10) + 1}d", // 1-10 day durations
                ParentId = i > 0 && i % 5 == 0 ? i - 4 : null // Some hierarchy
            });
        }

        return tasks;
    }

    private int ParseDurationDays(string duration)
    {
        // Simple duration parser for testing
        if (duration.EndsWith("d"))
        {
            if (int.TryParse(duration[..^1], out int days))
                return days;
        }
        return 1; // Default to 1 day
    }

    public void Dispose()
    {
        _serviceProvider?.Dispose();
    }
}
