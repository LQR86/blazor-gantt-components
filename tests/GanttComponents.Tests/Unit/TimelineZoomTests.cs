using GanttComponents.Models;
using GanttComponents.Services;
using Xunit;

namespace GanttComponents.Tests.Unit;

/// <summary>
/// Unit tests for timeline zoom infrastructure.
/// Validates zoom level calculations, configurations, and edge cases.
/// </summary>
public class TimelineZoomTests
{
    [Theory]
    [InlineData(TimelineZoomLevel.WeekDay, 60.0)]
    [InlineData(TimelineZoomLevel.MonthDay, 25.0)]
    [InlineData(TimelineZoomLevel.MonthWeek, 15.0)]
    [InlineData(TimelineZoomLevel.QuarterWeek, 8.0)]
    [InlineData(TimelineZoomLevel.QuarterMonth, 5.0)]
    [InlineData(TimelineZoomLevel.YearQuarter, 3.0)]
    public void ZoomLevelConfiguration_HasCorrectBaseDayWidth(TimelineZoomLevel level, double expectedDayWidth)
    {
        // Arrange & Act
        var config = TimelineZoomService.GetConfiguration(level);

        // Assert
        Assert.Equal(expectedDayWidth, config.BaseDayWidth);
        Assert.Equal(level, config.Level);
    }

    [Theory]
    [InlineData(1.0, 25.0)] // 1.0x factor = base width
    [InlineData(1.6, 40.0)] // Default factor for backward compatibility
    [InlineData(0.5, 12.5)] // Minimum factor
    [InlineData(3.0, 75.0)] // Maximum factor
    [InlineData(2.0, 50.0)] // 2x factor
    public void ZoomLevelConfiguration_CalculatesEffectiveDayWidth(double zoomFactor, double expectedWidth)
    {
        // Arrange
        var config = TimelineZoomService.GetConfiguration(TimelineZoomLevel.MonthDay);

        // Act
        var effectiveWidth = config.GetEffectiveDayWidth(zoomFactor);

        // Assert
        Assert.Equal(expectedWidth, effectiveWidth, precision: 1);
    }

    [Theory]
    [InlineData(-0.5, 0.5)] // Below minimum gets clamped
    [InlineData(0.3, 0.5)]  // Below minimum gets clamped
    [InlineData(0.5, 0.5)]  // Minimum value
    [InlineData(1.0, 1.0)]  // Normal value
    [InlineData(3.0, 3.0)]  // Maximum value
    [InlineData(3.5, 3.0)]  // Above maximum gets clamped
    [InlineData(10.0, 3.0)] // Way above maximum gets clamped
    public void ZoomLevelConfiguration_ClampsZoomFactorToValidRange(double input, double expected)
    {
        // Arrange
        var config = TimelineZoomService.GetConfiguration(TimelineZoomLevel.MonthDay);

        // Act
        var clampedWidth = config.GetEffectiveDayWidth(input);
        var expectedWidth = config.BaseDayWidth * expected;

        // Assert
        Assert.Equal(expectedWidth, clampedWidth, precision: 1);
    }

    [Fact]
    public void TimelineZoomService_ReturnsAllSixZoomLevels()
    {
        // Arrange & Act
        var configurations = TimelineZoomService.GetAllConfigurations();

        // Assert
        Assert.Equal(6, configurations.Count);
        Assert.Contains(TimelineZoomLevel.WeekDay, configurations.Keys);
        Assert.Contains(TimelineZoomLevel.MonthDay, configurations.Keys);
        Assert.Contains(TimelineZoomLevel.MonthWeek, configurations.Keys);
        Assert.Contains(TimelineZoomLevel.QuarterWeek, configurations.Keys);
        Assert.Contains(TimelineZoomLevel.QuarterMonth, configurations.Keys);
        Assert.Contains(TimelineZoomLevel.YearQuarter, configurations.Keys);
    }

    [Fact]
    public void TimelineZoomService_BackwardCompatibilitySettings()
    {
        // Arrange & Act
        var (level, factor) = TimelineZoomService.GetDefaultZoomSettings();
        var dayWidth = TimelineZoomService.CalculateEffectiveDayWidth(level, factor);

        // Assert
        Assert.Equal(TimelineZoomLevel.MonthDay, level);
        Assert.Equal(1.6, factor);
        Assert.Equal(40.0, dayWidth, precision: 1); // Current 40px day width preserved
    }

    [Theory]
    [InlineData(1.0, TimelineZoomLevel.WeekDay, 1.0, true)]   // 60px wide = visible
    [InlineData(0.1, TimelineZoomLevel.WeekDay, 1.0, false)]  // 6px wide = hidden
    [InlineData(1.0, TimelineZoomLevel.YearQuarter, 1.0, false)] // 3px wide = hidden
    [InlineData(5.0, TimelineZoomLevel.YearQuarter, 1.0, true)]  // 15px wide = visible
    [InlineData(0.2, TimelineZoomLevel.MonthDay, 1.6, false)] // 8px wide = hidden (40px * 0.2)
    [InlineData(0.5, TimelineZoomLevel.MonthDay, 1.6, true)]  // 20px wide = visible (40px * 0.5)
    public void TimelineZoomService_CorrectlyDeterminesTaskVisibility(
        double taskDurationDays,
        TimelineZoomLevel level,
        double zoomFactor,
        bool expectedVisible)
    {
        // Arrange & Act
        var isVisible = TimelineZoomService.IsTaskVisible(taskDurationDays, level, zoomFactor);

        // Assert
        Assert.Equal(expectedVisible, isVisible);
    }

    [Theory]
    [InlineData(12.0, true)]  // Exactly minimum = visible
    [InlineData(12.1, true)]  // Above minimum = visible
    [InlineData(11.9, false)] // Below minimum = hidden
    [InlineData(0.0, false)]  // Zero width = hidden
    [InlineData(100.0, true)] // Large width = visible
    public void TaskDisplayConstants_CorrectlyDeterminesWidthVisibility(double width, bool expectedVisible)
    {
        // Arrange & Act
        var isVisible = TaskDisplayConstants.IsTaskWidthVisible(width);

        // Assert
        Assert.Equal(expectedVisible, isVisible);
    }

    [Theory]
    [InlineData(5.0, 12.0)]  // Below minimum gets adjusted
    [InlineData(12.0, 12.0)] // Exactly minimum stays same
    [InlineData(20.0, 20.0)] // Above minimum stays same
    [InlineData(0.0, 12.0)]  // Zero gets adjusted
    public void TaskDisplayConstants_CalculatesEffectiveTaskWidth(double input, double expected)
    {
        // Arrange & Act
        var effectiveWidth = TaskDisplayConstants.GetEffectiveTaskWidth(input);

        // Assert
        Assert.Equal(expected, effectiveWidth);
    }

    [Fact]
    public void ZoomLevelConfiguration_ValidatesZoomFactorCorrectly()
    {
        // Arrange
        var config = TimelineZoomService.GetConfiguration(TimelineZoomLevel.MonthDay);

        // Act & Assert
        Assert.True(config.IsValidZoomFactor(0.5));  // Min valid
        Assert.True(config.IsValidZoomFactor(1.0));  // Normal
        Assert.True(config.IsValidZoomFactor(3.0));  // Max valid
        Assert.False(config.IsValidZoomFactor(0.4)); // Below min
        Assert.False(config.IsValidZoomFactor(3.1)); // Above max
        Assert.False(config.IsValidZoomFactor(-1.0)); // Negative
    }

    [Fact]
    public void TimelineZoomService_HandlesInvalidZoomLevel()
    {
        // Arrange - Use invalid enum value
        var invalidLevel = (TimelineZoomLevel)999;

        // Act
        var config = TimelineZoomService.GetConfiguration(invalidLevel);

        // Assert - Should return default configuration
        Assert.Equal(TaskDisplayConstants.DEFAULT_ZOOM_LEVEL, config.Level);
        Assert.Equal(25.0, config.BaseDayWidth); // MonthDay base width
    }

    [Theory]
    [InlineData(TimelineZoomLevel.WeekDay, 0.1, 0.5)]    // Clamp to minimum
    [InlineData(TimelineZoomLevel.MonthDay, 1.6, 1.6)]   // Valid factor unchanged
    [InlineData(TimelineZoomLevel.YearQuarter, 5.0, 3.0)] // Clamp to maximum
    [InlineData(TimelineZoomLevel.QuarterWeek, -1.0, 0.5)] // Negative clamped to minimum
    public void TimelineZoomService_ClampsZoomFactorCorrectly(
        TimelineZoomLevel level,
        double input,
        double expected)
    {
        // Arrange & Act
        var clamped = TimelineZoomService.ClampZoomFactor(level, input);

        // Assert
        Assert.Equal(expected, clamped);
    }
}
