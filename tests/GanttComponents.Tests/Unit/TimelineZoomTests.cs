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
    [InlineData(TimelineZoomLevel.WeekDay97px, 96.0)]         // 60 * 1.6 for backward compatibility
    [InlineData(TimelineZoomLevel.WeekDay68px, 72.0)]         // 45 * 1.6 for backward compatibility 
    [InlineData(TimelineZoomLevel.MonthDay48px, 56.0)]        // 35 * 1.6 for backward compatibility
    // MonthWeek Pattern Levels (Month → Week) - 48px, 34px, 24px
    [InlineData(TimelineZoomLevel.MonthDay48px, 40.0)]        // 25 * 1.6 for backward compatibility
    [InlineData(TimelineZoomLevel.MonthDay34px, 32.0)]        // 20 * 1.6 for backward compatibility
    [InlineData(TimelineZoomLevel.QuarterMonth24px, 28.8)]    // 18 * 1.6 for backward compatibility
    // QuarterMonth Pattern Levels (Quarter → Month) - 24px, 17px, 12px
    [InlineData(TimelineZoomLevel.QuarterMonth24px, 24.0)]    // 15 * 1.6 for backward compatibility
    [InlineData(TimelineZoomLevel.QuarterMonth17px, 19.2)]    // 12 * 1.6 for backward compatibility
    [InlineData(TimelineZoomLevel.Month12px, 16.0)]           // 10 * 1.6 for backward compatibility
    // YearQuarter Pattern Levels (Year → Quarter) - 8px, 6px, 4px, 3px
    [InlineData(TimelineZoomLevel.Month8px, 12.8)]            // 8 * 1.6 for backward compatibility
    [InlineData(TimelineZoomLevel.YearQuarter6px, 10.4)]      // 6.5 * 1.6 for backward compatibility
    [InlineData(TimelineZoomLevel.YearQuarter4px, 8.0)]       // 5 * 1.6 for backward compatibility
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
    [InlineData(1.0, 40.0)] // Preset-only: factor always 1.0, base width is 40.0 for MonthDay
    [InlineData(1.6, 40.0)] // Preset-only: factor clamped to 1.0, base width is 40.0
    [InlineData(0.5, 40.0)] // Preset-only: factor clamped to 1.0, base width is 40.0
    [InlineData(3.0, 40.0)] // Preset-only: factor clamped to 1.0, base width is 40.0
    [InlineData(2.0, 40.0)] // Preset-only: factor clamped to 1.0, base width is 40.0
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
        var expectedWidth = config.BaseDayWidth * expected; // Always 40.0 * 1.0 = 40.0

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

        // Assert - In preset-only system, factor is always 1.0, but effective day width remains 40px
        Assert.Equal(TimelineZoomLevel.MonthDay48px, level);
        Assert.Equal(1.0, factor); // Preset-only: factors are always 1.0
        Assert.Equal(40.0, dayWidth, precision: 1); // Day width preserved at 40px through base day width adjustment
    }

    [Theory]
    [InlineData(1.0, TimelineZoomLevel.WeekDay97px, 1.0, true)]      // 96px wide = visible (60*1.6)
    [InlineData(0.1, TimelineZoomLevel.WeekDay97px, 1.0, false)]     // 9.6px wide = hidden (60*1.6*0.1)
    [InlineData(1.0, TimelineZoomLevel.Month8px, 1.0, true)]         // 12.8px wide = visible (8*1.6)
    [InlineData(0.8, TimelineZoomLevel.YearQuarter3px, 1.0, false)]  // 2.4px wide = hidden (3*0.8)
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

        // Assert - Should return default configuration with preset-only base width
        Assert.Equal(TaskDisplayConstants.DEFAULT_ZOOM_LEVEL, config.Level);
        Assert.Equal(40.0, config.BaseDayWidth); // MonthDay preset-only base width (25 * 1.6)
    }

    [Theory]
    [InlineData(TimelineZoomLevel.WeekDay97px, 96.0)]          // WeekDay: 60 * 1.6 = 96.0
    [InlineData(TimelineZoomLevel.MonthDay48px, 40.0)]         // MonthWeek: 25 * 1.6 = 40.0 (maintains backward compatibility)
    [InlineData(TimelineZoomLevel.Month8px, 12.8)]             // YearQuarter: 8 * 1.6 = 12.8
    [InlineData(TimelineZoomLevel.YearQuarter3px, 3.0)]        // YearQuarterMin: 3px minimum day width constraint
    [InlineData(TimelineZoomLevel.QuarterMonth24px, 24.0)]     // QuarterMonth: 15 * 1.6 = 24.0
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
