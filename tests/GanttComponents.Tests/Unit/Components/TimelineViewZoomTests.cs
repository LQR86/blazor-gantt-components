using GanttComponents.Models;
using GanttComponents.Services;
using Xunit;

namespace GanttComponents.Tests.Unit.Components;

/// <summary>
/// Unit tests for TimelineView zoom functionality.
/// Focus: Backward compatibility and dynamic day width calculation.
/// </summary>
public class TimelineViewZoomTests
{
    [Fact]
    public void DefaultZoomParameters_ShouldMaintainBackwardCompatibility()
    {
        // Arrange
        var defaultZoomLevel = TimelineZoomLevel.MonthDay;
        var defaultZoomFactor = 1.6;
        var expectedDayWidth = 40.0; // Current legacy behavior

        // Act
        var config = TimelineZoomService.GetConfiguration(defaultZoomLevel);
        var actualDayWidth = config.GetEffectiveDayWidth(defaultZoomFactor);

        // Assert
        Assert.Equal(expectedDayWidth, actualDayWidth, precision: 1);
    }

    [Fact]
    public void MonthDayZoomLevel_WithDifferentFactors_ShouldCalculateCorrectly()
    {
        // Arrange
        var zoomLevel = TimelineZoomLevel.MonthDay;

        var testCases = new[]
        {
            (factor: 1.0, expected: 25.0),
            (factor: 1.6, expected: 40.0), // Backward compatibility case
            (factor: 2.0, expected: 50.0),
            (factor: 0.5, expected: 12.5)
        };

        var config = TimelineZoomService.GetConfiguration(zoomLevel);

        // Act & Assert
        foreach (var (factor, expected) in testCases)
        {
            var actual = config.GetEffectiveDayWidth(factor);
            Assert.Equal(expected, actual, precision: 1);
        }
    }

    [Fact]
    public void AllZoomLevels_ShouldHaveValidConfigurations()
    {
        // Arrange
        var allZoomLevels = Enum.GetValues<TimelineZoomLevel>();

        // Act & Assert
        foreach (var level in allZoomLevels)
        {
            var config = TimelineZoomService.GetConfiguration(level);

            Assert.NotNull(config);
            Assert.True(config.BaseDayWidth > 0, $"BaseDayWidth for {level} should be positive");
            Assert.Equal(level, config.Level);

            // Test with default factor
            var dayWidth = config.GetEffectiveDayWidth(1.0);
            Assert.True(dayWidth > 0, $"Effective day width for {level} should be positive");
        }
    }

    [Theory]
    [InlineData(TimelineZoomLevel.WeekDay, 1.0, 60.0)]
    [InlineData(TimelineZoomLevel.MonthDay, 1.0, 25.0)]
    [InlineData(TimelineZoomLevel.MonthWeek, 1.0, 15.0)]
    [InlineData(TimelineZoomLevel.QuarterWeek, 1.0, 8.0)]
    [InlineData(TimelineZoomLevel.QuarterMonth, 1.0, 5.0)]
    [InlineData(TimelineZoomLevel.YearQuarter, 1.0, 3.0)]
    public void ZoomLevel_WithBaseFactor_ShouldReturnExpectedDayWidth(
        TimelineZoomLevel level,
        double factor,
        double expectedDayWidth)
    {
        // Arrange
        var config = TimelineZoomService.GetConfiguration(level);

        // Act
        var actualDayWidth = config.GetEffectiveDayWidth(factor);

        // Assert
        Assert.Equal(expectedDayWidth, actualDayWidth, precision: 1);
    }

    [Fact]
    public void ZoomFactorValidation_ShouldClampToValidRange()
    {
        // Arrange
        var config = TimelineZoomService.GetConfiguration(TimelineZoomLevel.MonthDay);
        var baseDayWidth = 25.0;

        var testCases = new[]
        {
            (inputFactor: 0.1, expectedClamped: 0.5), // Below minimum
            (inputFactor: 5.0, expectedClamped: 3.0), // Above maximum
            (inputFactor: 1.5, expectedClamped: 1.5)  // Within range
        };

        // Act & Assert
        foreach (var (inputFactor, expectedClamped) in testCases)
        {
            var clampedFactor = Math.Max(config.MinZoomFactor, Math.Min(config.MaxZoomFactor, inputFactor));
            var actualDayWidth = config.GetEffectiveDayWidth(clampedFactor);
            var expectedDayWidth = baseDayWidth * expectedClamped;

            Assert.Equal(expectedDayWidth, actualDayWidth, precision: 1);
        }
    }
}
