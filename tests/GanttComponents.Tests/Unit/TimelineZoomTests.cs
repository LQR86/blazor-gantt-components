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
    // WeekDay Pattern Levels (Week → Day) - 97px, 68px, 48px  
    [InlineData(TimelineZoomLevel.WeekDay97px, 97.0)]         // Integral pixel design
    [InlineData(TimelineZoomLevel.WeekDay68px, 68.0)]         // Integral pixel design 
    [InlineData(TimelineZoomLevel.MonthDay48px, 48.0)]        // Integral pixel design
    // MonthWeek Pattern Levels (Month → Week) - 34px, 24px
    [InlineData(TimelineZoomLevel.MonthDay34px, 34.0)]        // Integral pixel design
    [InlineData(TimelineZoomLevel.QuarterMonth24px, 24.0)]    // Integral pixel design
    // QuarterMonth Pattern Levels (Quarter → Month) - 17px, 12px
    [InlineData(TimelineZoomLevel.QuarterMonth17px, 17.0)]    // Integral pixel design
    [InlineData(TimelineZoomLevel.Month12px, 12.0)]           // Integral pixel design
    // YearQuarter Pattern Levels (Year → Quarter) - 8px, 6px, 4px, 3px
    [InlineData(TimelineZoomLevel.Month8px, 8.0)]             // Integral pixel design
    [InlineData(TimelineZoomLevel.YearQuarter6px, 6.0)]       // Integral pixel design
    [InlineData(TimelineZoomLevel.YearQuarter4px, 4.0)]       // Integral pixel design
    [InlineData(TimelineZoomLevel.YearQuarter3px, 3.0)]       // 3px minimum day width constraint
    public void ZoomLevelConfiguration_HasCorrectBaseDayWidth(TimelineZoomLevel level, double expectedDayWidth)
    {
        // Arrange & Act
        var config = TimelineZoomService.GetConfiguration(level);

        // Assert
        Assert.Equal(expectedDayWidth, config.BaseDayWidth);
        Assert.Equal(level, config.Level);
    }

    [Theory]
    [InlineData(1.0, 48.0)] // Preset-only: factor always 1.0, base width is 48.0 for MonthDay
    [InlineData(1.6, 48.0)] // Preset-only: factor clamped to 1.0, base width is 48.0
    [InlineData(0.5, 48.0)] // Preset-only: factor clamped to 1.0, base width is 48.0
    [InlineData(3.0, 48.0)] // Preset-only: factor clamped to 1.0, base width is 48.0
    [InlineData(2.0, 48.0)] // Preset-only: factor clamped to 1.0, base width is 48.0
    public void ZoomLevelConfiguration_CalculatesEffectiveDayWidth(double zoomFactor, double expectedWidth)
    {
        // Arrange
        var config = TimelineZoomService.GetConfiguration(TimelineZoomLevel.MonthDay48px);

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
        var config = TimelineZoomService.GetConfiguration(TimelineZoomLevel.MonthDay48px);

        // Act - In preset-only system, all factors are clamped to 1.0
        var clampedWidth = config.GetEffectiveDayWidth(input);
        var expectedWidth = config.BaseDayWidth * expected; // Always 48.0 * 1.0 = 48.0

        // Assert - Effective day width is always the base day width in preset-only system
        Assert.Equal(expectedWidth, clampedWidth, precision: 1);
    }

    [Fact]
    public void TimelineZoomService_ReturnsAllThirteenZoomLevels()
    {
        // Arrange & Act
        var configurations = TimelineZoomService.GetAllConfigurations();

        // Assert - Preset-only system now supports 11 fine-grained levels
        Assert.Equal(11, configurations.Count);

        // Check key zoom levels exist
        Assert.Contains(TimelineZoomLevel.WeekDay97px, configurations.Keys);
        Assert.Contains(TimelineZoomLevel.WeekDay68px, configurations.Keys);
        Assert.Contains(TimelineZoomLevel.MonthDay48px, configurations.Keys);
        Assert.Contains(TimelineZoomLevel.MonthDay34px, configurations.Keys);
        Assert.Contains(TimelineZoomLevel.QuarterMonth24px, configurations.Keys);
        Assert.Contains(TimelineZoomLevel.QuarterMonth17px, configurations.Keys);

        // Check intermediate levels added for finer granularity
        Assert.Contains(TimelineZoomLevel.Month12px, configurations.Keys);
        Assert.Contains(TimelineZoomLevel.Month8px, configurations.Keys);
        Assert.Contains(TimelineZoomLevel.YearQuarter6px, configurations.Keys);
        Assert.Contains(TimelineZoomLevel.YearQuarter4px, configurations.Keys);
        Assert.Contains(TimelineZoomLevel.YearQuarter3px, configurations.Keys);
    }

    [Fact]
    public void TimelineZoomService_BackwardCompatibilitySettings()
    {
        // Arrange & Act
        var (level, factor) = TimelineZoomService.GetDefaultZoomSettings();
        var dayWidth = TimelineZoomService.CalculateEffectiveDayWidth(level, factor);

        // Assert - In 11-level system, QuarterMonth24px is the default
        Assert.Equal(TimelineZoomLevel.QuarterMonth24px, level);
        Assert.Equal(1.0, factor); // Preset-only: factors are always 1.0
        Assert.Equal(24.0, dayWidth, precision: 1); // Day width at 24px for QuarterMonth24px
    }

    [Theory]
    [InlineData(1.0, TimelineZoomLevel.WeekDay97px, 1.0, true)]      // 97px wide = visible 
    [InlineData(0.1, TimelineZoomLevel.WeekDay97px, 1.0, false)]     // 9.7px wide = hidden 
    [InlineData(1.0, TimelineZoomLevel.Month8px, 1.0, false)]        // 8px wide = hidden (below 12px threshold) 
    [InlineData(0.8, TimelineZoomLevel.YearQuarter3px, 1.0, false)]  // 2.4px wide = hidden
    [InlineData(5.0, TimelineZoomLevel.YearQuarter3px, 1.0, true)]   // 15px wide = visible (3*5)
    [InlineData(0.2, TimelineZoomLevel.MonthDay48px, 1.6, false)]    // 8px wide = hidden (40px * 0.2)
    [InlineData(0.5, TimelineZoomLevel.MonthDay48px, 1.6, true)]     // 20px wide = visible (40px * 0.5)
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
        var config = TimelineZoomService.GetConfiguration(TimelineZoomLevel.MonthDay48px);

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

        // Assert - Should return default configuration (QuarterMonth24px in 11-level system)
        Assert.Equal(TaskDisplayConstants.DEFAULT_ZOOM_LEVEL, config.Level);
        Assert.Equal(24.0, config.BaseDayWidth); // QuarterMonth24px integral pixel base width
    }

    [Theory]
    [InlineData(TimelineZoomLevel.WeekDay97px, 97.0)]          // WeekDay: integral 97px
    [InlineData(TimelineZoomLevel.MonthDay48px, 48.0)]         // MonthDay: integral 48px
    [InlineData(TimelineZoomLevel.Month8px, 8.0)]              // Month: integral 8px
    [InlineData(TimelineZoomLevel.YearQuarter3px, 3.0)]        // YearQuarter: 3px minimum day width constraint
    [InlineData(TimelineZoomLevel.QuarterMonth24px, 24.0)]     // QuarterMonth: integral 24px
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
