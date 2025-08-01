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
    [InlineData(TimelineZoomLevel.WeekDay, 96.0)]    // Preset-only: 60 * 1.6 for backward compatibility
    [InlineData(TimelineZoomLevel.MonthDay, 40.0)]   // Preset-only: 25 * 1.6 for backward compatibility
    [InlineData(TimelineZoomLevel.MonthWeek, 24.0)]  // Preset-only: 15 * 1.6 for backward compatibility
    [InlineData(TimelineZoomLevel.QuarterWeek, 12.8)] // Preset-only: 8 * 1.6 for backward compatibility
    [InlineData(TimelineZoomLevel.QuarterMonth, 8.0)] // Preset-only: 5 * 1.6 for backward compatibility
    [InlineData(TimelineZoomLevel.YearQuarter, 3.0)]  // Minimum day width constraint maintained
    public void ZoomLevelConfiguration_HasCorrectBaseDayWidth(TimelineZoomLevel level, double expectedDayWidth)
    {
        // Arrange & Act
        var config = TimelineZoomService.GetConfiguration(level);

        // Assert
        Assert.Equal(expectedDayWidth, config.BaseDayWidth);
        Assert.Equal(level, config.Level);
    }

    [Theory]
    [InlineData(1.0, 40.0)] // Preset-only: factor always 1.0, base width is 40.0 for MonthDay
    [InlineData(1.6, 40.0)] // Preset-only: factor clamped to 1.0, base width is 40.0
    [InlineData(0.5, 40.0)] // Preset-only: factor clamped to 1.0, base width is 40.0
    [InlineData(3.0, 40.0)] // Preset-only: factor clamped to 1.0, base width is 40.0
    [InlineData(2.0, 40.0)] // Preset-only: factor clamped to 1.0, base width is 40.0
    public void ZoomLevelConfiguration_CalculatesEffectiveDayWidth(double zoomFactor, double expectedWidth)
    {
        // Arrange
        var config = TimelineZoomService.GetConfiguration(TimelineZoomLevel.MonthDay);

        // Act - In preset-only system, all factors are clamped to 1.0
        var effectiveWidth = config.GetEffectiveDayWidth(zoomFactor);

        // Assert - Effective width always equals base day width in preset-only system
        Assert.Equal(expectedWidth, effectiveWidth, precision: 1);
    }

    [Theory]
    [InlineData(-0.5, 1.0)] // Preset-only: all inputs clamped to 1.0
    [InlineData(0.3, 1.0)]  // Preset-only: all inputs clamped to 1.0
    [InlineData(0.5, 1.0)]  // Preset-only: all inputs clamped to 1.0
    [InlineData(1.0, 1.0)]  // Preset-only: all inputs clamped to 1.0
    [InlineData(3.0, 1.0)]  // Preset-only: all inputs clamped to 1.0
    [InlineData(3.5, 1.0)]  // Preset-only: all inputs clamped to 1.0
    [InlineData(10.0, 1.0)] // Preset-only: all inputs clamped to 1.0
    public void ZoomLevelConfiguration_ClampsZoomFactorToValidRange(double input, double expected)
    {
        // Arrange
        var config = TimelineZoomService.GetConfiguration(TimelineZoomLevel.MonthDay);

        // Act - In preset-only system, all factors are clamped to 1.0
        var clampedWidth = config.GetEffectiveDayWidth(input);
        var expectedWidth = config.BaseDayWidth * expected; // Always 40.0 * 1.0 = 40.0

        // Assert - Effective day width is always the base day width in preset-only system
        Assert.Equal(expectedWidth, clampedWidth, precision: 1);
    }

    [Fact]
    public void TimelineZoomService_ReturnsAllThirteenZoomLevels()
    {
        // Arrange & Act
        var configurations = TimelineZoomService.GetAllConfigurations();

        // Assert - Preset-only system now supports 13 fine-grained levels
        Assert.Equal(13, configurations.Count);
        
        // Original 6 levels still exist
        Assert.Contains(TimelineZoomLevel.WeekDay, configurations.Keys);
        Assert.Contains(TimelineZoomLevel.MonthDay, configurations.Keys);
        Assert.Contains(TimelineZoomLevel.MonthWeek, configurations.Keys);
        Assert.Contains(TimelineZoomLevel.QuarterWeek, configurations.Keys);
        Assert.Contains(TimelineZoomLevel.QuarterMonth, configurations.Keys);
        Assert.Contains(TimelineZoomLevel.YearQuarter, configurations.Keys);
        
        // New intermediate levels added for finer granularity
        Assert.Contains(TimelineZoomLevel.WeekDayMedium, configurations.Keys);
        Assert.Contains(TimelineZoomLevel.WeekDayLow, configurations.Keys);
        Assert.Contains(TimelineZoomLevel.MonthDayMedium, configurations.Keys);
        Assert.Contains(TimelineZoomLevel.MonthWeekMedium, configurations.Keys);
        Assert.Contains(TimelineZoomLevel.MonthWeekLow, configurations.Keys);
        Assert.Contains(TimelineZoomLevel.QuarterWeekMedium, configurations.Keys);
        Assert.Contains(TimelineZoomLevel.QuarterMonthMedium, configurations.Keys);
    }

    [Fact]
    public void TimelineZoomService_BackwardCompatibilitySettings()
    {
        // Arrange & Act
        var (level, factor) = TimelineZoomService.GetDefaultZoomSettings();
        var dayWidth = TimelineZoomService.CalculateEffectiveDayWidth(level, factor);

        // Assert - In preset-only system, factor is always 1.0, but effective day width remains 40px
        Assert.Equal(TimelineZoomLevel.MonthDay, level);
        Assert.Equal(1.0, factor); // Preset-only: factors are always 1.0
        Assert.Equal(40.0, dayWidth, precision: 1); // Day width preserved at 40px through base day width adjustment
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

        // Act & Assert - In preset-only system, only factor 1.0 is valid
        Assert.True(config.IsValidZoomFactor(1.0));   // Only valid factor in preset-only system
        Assert.False(config.IsValidZoomFactor(0.5));  // Invalid in preset-only system
        Assert.False(config.IsValidZoomFactor(3.0));  // Invalid in preset-only system
        Assert.False(config.IsValidZoomFactor(0.4));  // Invalid in preset-only system
        Assert.False(config.IsValidZoomFactor(3.1));  // Invalid in preset-only system
        Assert.False(config.IsValidZoomFactor(-1.0)); // Invalid in preset-only system
    }

    [Fact]
    public void TimelineZoomService_HandlesInvalidZoomLevel()
    {
        // Arrange - Use invalid enum value
        var invalidLevel = (TimelineZoomLevel)999;

        // Act
        var config = TimelineZoomService.GetConfiguration(invalidLevel);

        // Assert - Should return default configuration with preset-only base width
        Assert.Equal(TaskDisplayConstants.DEFAULT_ZOOM_LEVEL, config.Level);
        Assert.Equal(40.0, config.BaseDayWidth); // MonthDay preset-only base width (25 * 1.6)
    }

    [Theory]
    [InlineData(TimelineZoomLevel.WeekDay, 96.0)]   // Preset-only always returns base day width
    [InlineData(TimelineZoomLevel.MonthDay, 40.0)]  // Maintains backward compatibility
    [InlineData(TimelineZoomLevel.YearQuarter, 3.0)] // Minimum day width constraint
    [InlineData(TimelineZoomLevel.QuarterWeek, 12.8)] // New intermediate level
    public void TimelineZoomService_PresetOnlyFactorsAlwaysOne(
        TimelineZoomLevel level,
        double expectedDayWidth)
    {
        // Arrange & Act - In preset-only system, factor is always 1.0
        var clamped = TimelineZoomService.ClampZoomFactor(level, 999.0); // Any input
        var dayWidth = TimelineZoomService.CalculateEffectiveDayWidth(level, clamped);

        // Assert - Factor always clamped to 1.0, day width comes from base configuration
        Assert.Equal(1.0, clamped);
        Assert.Equal(expectedDayWidth, dayWidth, precision: 1);
    }
}
