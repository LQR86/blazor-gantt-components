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
        var defaultZoomLevel = TimelineZoomLevel.MonthDay48px;
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
        var zoomLevel = TimelineZoomLevel.MonthDay48px;

        var testCases = new[]
        {
            (factor: 1.0, expected: 40.0),  // Preset-only: base day width is 40px (25 * 1.6)
            (factor: 1.6, expected: 40.0), // Preset-only: factor clamped to 1.0, so 40 * 1.0 = 40
            (factor: 2.0, expected: 40.0), // Preset-only: factor clamped to 1.0, so 40 * 1.0 = 40
            (factor: 0.5, expected: 40.0)  // Preset-only: factor clamped to 1.0, so 40 * 1.0 = 40
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
    [InlineData(TimelineZoomLevel.WeekDay97px, 1.0, 97.0)]        // 11-level integral: 97px
    [InlineData(TimelineZoomLevel.MonthDay48px, 1.0, 48.0)]       // 11-level integral: 48px
    [InlineData(TimelineZoomLevel.QuarterMonth24px, 1.0, 24.0)]    // 11-level integral: 24px
    [InlineData(TimelineZoomLevel.YearQuarter6px, 1.0, 6.0)]     // 11-level integral: 6px
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
        var config = TimelineZoomService.GetConfiguration(TimelineZoomLevel.MonthDay48px);
        var baseDayWidth = 40.0; // Updated for preset-only: 25 * 1.6 = 40px

        var testCases = new[]
        {
            (inputFactor: 0.1, expectedClamped: 1.0), // Preset-only: all factors clamped to 1.0
            (inputFactor: 5.0, expectedClamped: 1.0), // Preset-only: all factors clamped to 1.0
            (inputFactor: 1.5, expectedClamped: 1.0)  // Preset-only: all factors clamped to 1.0
        };

        // Act & Assert
        foreach (var (inputFactor, expectedClamped) in testCases)
        {
            var clampedFactor = Math.Max(config.MinZoomFactor, Math.Min(config.MaxZoomFactor, inputFactor));
            var actualDayWidth = config.GetEffectiveDayWidth(clampedFactor);
            var expectedDayWidth = baseDayWidth * expectedClamped; // Always 40 * 1.0 = 40

            Assert.Equal(expectedDayWidth, actualDayWidth, precision: 1);
        }
    }
}
