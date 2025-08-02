using GanttComponents.Models;
using GanttComponents.Services;
using Xunit;

namespace GanttComponents.Tests.Unit.Services;

/// <summary>
/// Unit tests specifically for preset-only zoom system behavior.
/// Validates that manual zoom factors are properly clamped in preset-only mode.
/// </summary>
public class PresetOnlyZoomValidationTests
{
    [Theory]
    [InlineData(0.1, 1.0)] // Sub-minimum factor clamped to 1.0
    [InlineData(0.5, 1.0)] // Minimum factor clamped to 1.0
    [InlineData(1.0, 1.0)] // Default factor remains 1.0
    [InlineData(1.5, 1.0)] // Manual factor clamped to 1.0
    [InlineData(2.0, 1.0)] // Manual factor clamped to 1.0
    [InlineData(3.0, 1.0)] // Maximum factor clamped to 1.0
    [InlineData(5.0, 1.0)] // Super-maximum factor clamped to 1.0
    public void PresetOnlySystem_AllZoomFactors_ClampedToOne(double inputFactor, double expectedFactor)
    {
        // Arrange - Test with WeekDay level (should behave same for all levels in preset-only)
        var level = TimelineZoomLevel.WeekDay;

        // Act - Clamp the zoom factor using preset-only system
        var actualFactor = TimelineZoomService.ClampZoomFactor(level, inputFactor);

        // Assert - All factors should be clamped to 1.0 in preset-only system
        Assert.Equal(expectedFactor, actualFactor, 3); // 3 decimal places precision
    }

    [Theory]
    [InlineData(TimelineZoomLevel.WeekDay, 96.0)]        // 60 * 1.6 backward compatibility
    [InlineData(TimelineZoomLevel.MonthDay, 40.0)]       // 25 * 1.6 backward compatibility  
    [InlineData(TimelineZoomLevel.MonthWeek, 24.0)]      // 15 * 1.6 backward compatibility
    [InlineData(TimelineZoomLevel.QuarterMonth, 8.0)]    // 5 * 1.6 backward compatibility
    [InlineData(TimelineZoomLevel.YearQuarter, 3.0)]     // 3px minimum constraint
    public void PresetOnlySystem_AllLevels_ConsistentDayWidthRegardlessOfFactor(TimelineZoomLevel level, double expectedDayWidth)
    {
        // Arrange - Test various zoom factors that should all produce same result
        var testFactors = new[] { 0.5, 1.0, 1.5, 2.0, 3.0 };

        foreach (var factor in testFactors)
        {
            // Act - Calculate day width with preset-only system
            var actualDayWidth = TimelineZoomService.CalculateEffectiveDayWidth(level, factor);

            // Assert - All factors should produce same day width for each level (preset-only behavior)
            Assert.Equal(expectedDayWidth, actualDayWidth, 1); // 1 decimal place tolerance
        }
    }

    [Fact]
    public void PresetOnlySystem_CannotZoomOut_FactorAlwaysOne()
    {
        // Arrange - Test all zoom levels
        var allLevels = Enum.GetValues<TimelineZoomLevel>();

        foreach (var level in allLevels)
        {
            // Act - Check if zoom out is possible (should always be false in preset-only)
            var canZoomOut = TimelineZoomService.CanZoomOut(level, 1.0, 0.1);

            // Assert - Preset-only system should never allow zoom factor changes
            Assert.False(canZoomOut, $"Level {level} should not allow zoom out in preset-only system");
        }
    }

    [Fact]
    public void PresetOnlySystem_DefaultSettings_UseMonthDayAtFactorOne()
    {
        // Act - Get default zoom settings for preset-only approach
        var (defaultLevel, defaultFactor) = TimelineZoomService.GetDefaultZoomSettings();

        // Assert - Default should be MonthDay level at 1.0x factor
        Assert.Equal(TaskDisplayConstants.DEFAULT_ZOOM_LEVEL, defaultLevel);
        Assert.Equal(TaskDisplayConstants.DEFAULT_ZOOM_FACTOR, defaultFactor);
        Assert.Equal(1.0, defaultFactor); // Explicitly verify factor is 1.0 for preset-only
    }

    [Fact]
    public void PresetOnlySystem_AllLevels_MaintainMinimumDayWidth()
    {
        // Arrange - All zoom levels should respect the 3px minimum
        var allLevels = Enum.GetValues<TimelineZoomLevel>();

        foreach (var level in allLevels)
        {
            // Act - Calculate day width at 1.0x factor (preset-only)
            var dayWidth = TimelineZoomService.CalculateEffectiveDayWidth(level, 1.0);

            // Assert - All levels should have day width >= 3px minimum
            Assert.True(dayWidth >= TaskDisplayConstants.MIN_EFFECTIVE_DAY_WIDTH,
                $"Level {level} has day width {dayWidth}px, should be >= {TaskDisplayConstants.MIN_EFFECTIVE_DAY_WIDTH}px");
        }
    }
}
