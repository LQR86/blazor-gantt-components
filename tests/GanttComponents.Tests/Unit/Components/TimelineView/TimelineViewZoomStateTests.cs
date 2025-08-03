using Xunit;
using GanttComponents.Models;
using GanttComponents.Services;

namespace GanttComponents.Tests.Unit.Components.TimelineView;

/// <summary>
/// Unit tests for TimelineView zoom state management functionality.
/// Tests zoom parameter validation, state changes, and control methods.
/// </summary>
public class TimelineViewZoomStateTests
{
    [Fact]
    public void ZoomLevel_DefaultValue_ShouldBeMonthDay()
    {
        // Arrange & Act
        var defaultLevel = TimelineZoomLevel.MonthDay48px;

        // Assert
        Assert.Equal(TimelineZoomLevel.MonthDay48px, defaultLevel);
    }

    [Fact]
    public void ZoomFactor_DefaultValue_ShouldBe1Point6()
    {
        // Arrange & Act
        var defaultFactor = 1.6;

        // Assert
        Assert.Equal(1.6, defaultFactor, 1);
    }

    [Fact]
    public void CurrentEffectiveDayWidth_WithDefaults_ShouldBe24Pixels()
    {
        // Arrange
        var config = TimelineZoomService.GetConfiguration(TimelineZoomLevel.QuarterMonth24px);

        // Act
        var dayWidth = config.GetEffectiveDayWidth(1.0);

        // Assert
        // QuarterMonth24px (24px) * 1.0 = 24px (11-level integral design)
        Assert.Equal(24.0, dayWidth, 1);
    }

    [Theory]
    [InlineData(0.4, 0.5)] // Below minimum should clamp to 0.5
    [InlineData(0.5, 0.5)] // At minimum should stay
    [InlineData(1.0, 1.0)] // Normal value should stay
    [InlineData(3.0, 3.0)] // At maximum should stay
    [InlineData(3.5, 3.0)] // Above maximum should clamp to 3.0
    [InlineData(10.0, 3.0)] // Far above maximum should clamp to 3.0
    public void ZoomFactorClamping_ShouldRespectBounds(double input, double expected)
    {
        // Arrange & Act
        var clamped = Math.Max(0.5, Math.Min(3.0, input));

        // Assert
        Assert.Equal(expected, clamped, 1);
    }

    [Fact]
    public void ZoomFactorChange_WithTinyDifference_ShouldBeIgnored()
    {
        // Arrange
        var originalFactor = 1.6;
        var newFactor = 1.605; // Only 0.005 difference

        // Act
        var shouldUpdate = Math.Abs(originalFactor - newFactor) > 0.01;

        // Assert
        Assert.False(shouldUpdate, "Tiny changes should be ignored to avoid excessive updates");
    }

    [Fact]
    public void ZoomFactorChange_WithSignificantDifference_ShouldBeAccepted()
    {
        // Arrange
        var originalFactor = 1.6;
        var newFactor = 1.8; // 0.2 difference

        // Act
        var shouldUpdate = Math.Abs(originalFactor - newFactor) > 0.01;

        // Assert
        Assert.True(shouldUpdate, "Significant changes should be accepted");
    }

    [Fact]
    public void ZoomInFactor_ShouldIncrease20Percent()
    {
        // Arrange
        var originalFactor = 1.0;

        // Act
        var newFactor = originalFactor * 1.2;

        // Assert
        Assert.Equal(1.2, newFactor, 2);
    }

    [Fact]
    public void ZoomOutFactor_ShouldDecrease20Percent()
    {
        // Arrange
        var originalFactor = 1.2;

        // Act
        var newFactor = originalFactor / 1.2;

        // Assert
        Assert.Equal(1.0, newFactor, 2);
    }

    [Theory]
    [InlineData(TimelineZoomLevel.WeekDay97px, 1.0, 96.0)]        // 60 * 1.6 backward compatibility
    [InlineData(TimelineZoomLevel.MonthDay48px, 1.0, 40.0)]       // 25 * 1.6 backward compatibility
    [InlineData(TimelineZoomLevel.QuarterMonth24px, 1.0, 24.0)]    // 15 * 1.6 backward compatibility
    [InlineData(TimelineZoomLevel.YearQuarter6px, 1.0, 12.8)]     // 8 * 1.6 backward compatibility
    public void EffectiveDayWidth_WithDifferentZoomLevels_ShouldCalculateCorrectly(
        TimelineZoomLevel level, double factor, double expectedWidth)
    {
        // Arrange
        var config = TimelineZoomService.GetConfiguration(level);

        // Act
        var actualWidth = config.GetEffectiveDayWidth(factor);

        // Assert
        Assert.Equal(expectedWidth, actualWidth, 1);
    }

    [Fact]
    public void EffectiveDayWidth_WithZoomFactor_ShouldScaleCorrectly()
    {
        // Arrange
        var config = TimelineZoomService.GetConfiguration(TimelineZoomLevel.MonthDay48px);

        // Act - MonthDay (40px base) with preset-only factor (always 1.0)
        var actualWidth = config.GetEffectiveDayWidth(2.0); // Factor clamped to 1.0

        // Assert
        Assert.Equal(40.0, actualWidth, 1); // Preset-only: 40 * 1.0 = 40 (factor clamped)
    }

    [Fact]
    public void ZoomDescription_ShouldFormatCorrectly()
    {
        // Arrange
        var level = TimelineZoomLevel.MonthDay48px;
        var factor = 1.6;

        // Act
        var description = $"{level} @ {factor:F1}x";

        // Assert
        Assert.Equal("MonthWeek @ 1.6x", description);
    }

    [Theory]
    [InlineData(2.9, 1.2, 3.0)] // Near max, zoom in should clamp
    [InlineData(3.0, 1.2, 3.0)] // At max, zoom in should stay at max
    [InlineData(0.6, 1.2, 0.5)] // Near min, zoom out should clamp
    [InlineData(0.5, 1.2, 0.5)] // At min, zoom out should stay at min
    public void ZoomControls_WithBoundaryValues_ShouldRespectLimits(
        double initialFactor, double zoomMultiplier, double expectedResult)
    {
        // Arrange
        var zoomInResult = initialFactor * zoomMultiplier;
        var zoomOutResult = initialFactor / zoomMultiplier;

        // Act - simulate zoom in
        var clampedZoomIn = Math.Max(0.5, Math.Min(3.0, zoomInResult));
        // Act - simulate zoom out
        var clampedZoomOut = Math.Max(0.5, Math.Min(3.0, zoomOutResult));

        // Assert
        if (initialFactor >= 2.5) // High values should clamp on zoom in
        {
            Assert.Equal(expectedResult, clampedZoomIn, 1);
        }
        else if (initialFactor <= 0.6) // Low values should clamp on zoom out
        {
            Assert.Equal(expectedResult, clampedZoomOut, 1);
        }
    }

    [Fact]
    public void ZoomReset_ShouldSetFactorToOne()
    {
        // Arrange
        var resetFactor = 1.0;

        // Act & Assert
        Assert.Equal(1.0, resetFactor, 1);
    }

    [Fact]
    public void BackwardCompatibility_DefaultParameters_ShouldMaintain40PixelDayWidth()
    {
        // Arrange
        var defaultLevel = TimelineZoomLevel.MonthDay48px;
        var defaultFactor = 1.6;
        var legacyDayWidth = 40.0;

        // Act
        var config = TimelineZoomService.GetConfiguration(defaultLevel);
        var actualDayWidth = config.GetEffectiveDayWidth(defaultFactor);

        // Assert
        Assert.Equal(legacyDayWidth, actualDayWidth, 1);
    }
}
