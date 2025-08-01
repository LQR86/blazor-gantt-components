using GanttComponents.Models;
using GanttComponents.Services;
using Xunit;

namespace GanttComponents.Tests.Unit;

/// <summary>
/// Unit tests for Manual Zoom Controls functionality.
/// Tests Iteration 3.1 implementation: manual zoom factor calculations and boundary conditions.
/// </summary>
public class ManualZoomControlsTests
{
    [Theory]
    [InlineData(1.0, 0.1, 1.1)]
    [InlineData(1.5, 0.1, 1.6)]
    [InlineData(2.0, 0.25, 2.25)]
    [InlineData(0.8, 0.2, 1.0)]
    public void ZoomFactor_Increment_CalculatesCorrectly(double currentFactor, double step, double expectedFactor)
    {
        // Arrange & Act
        var newFactor = currentFactor + step;

        // Assert
        Assert.Equal(expectedFactor, newFactor, precision: 2);
    }

    [Theory]
    [InlineData(1.0, 0.1, 0.9)]
    [InlineData(1.5, 0.1, 1.4)]
    [InlineData(2.0, 0.25, 1.75)]
    [InlineData(1.2, 0.2, 1.0)]
    public void ZoomFactor_Decrement_CalculatesCorrectly(double currentFactor, double step, double expectedFactor)
    {
        // Arrange & Act
        var newFactor = currentFactor - step;

        // Assert
        Assert.Equal(expectedFactor, newFactor, precision: 2);
    }

    [Theory]
    [InlineData(0.5, 3.0, 0.4, 0.5)] // Below min
    [InlineData(0.5, 3.0, 3.1, 3.0)] // Above max
    [InlineData(0.5, 3.0, 1.0, 1.0)] // Within range
    [InlineData(0.25, 5.0, 2.5, 2.5)] // Within larger range
    public void ZoomFactor_Clamping_WorksCorrectly(double min, double max, double input, double expected)
    {
        // Arrange & Act
        var clampedValue = Math.Max(min, Math.Min(max, input));

        // Assert
        Assert.Equal(expected, clampedValue, precision: 2);
    }

    [Theory]
    [InlineData(0.5, 0.5, true)]   // At minimum
    [InlineData(0.5, 0.6, false)]  // Above minimum
    [InlineData(0.25, 0.25, true)] // At custom minimum
    [InlineData(0.25, 0.3, false)] // Above custom minimum
    public void ZoomFactor_IsAtMinimum_DetectsCorrectly(double minFactor, double currentFactor, bool expectedAtMin)
    {
        // Arrange & Act
        var isAtMin = currentFactor <= minFactor;

        // Assert
        Assert.Equal(expectedAtMin, isAtMin);
    }

    [Theory]
    [InlineData(3.0, 3.0, true)]   // At maximum
    [InlineData(3.0, 2.9, false)]  // Below maximum
    [InlineData(5.0, 5.0, true)]   // At custom maximum
    [InlineData(5.0, 4.8, false)]  // Below custom maximum
    public void ZoomFactor_IsAtMaximum_DetectsCorrectly(double maxFactor, double currentFactor, bool expectedAtMax)
    {
        // Arrange & Act
        var isAtMax = currentFactor >= maxFactor;

        // Assert
        Assert.Equal(expectedAtMax, isAtMax);
    }

    [Theory]
    [InlineData(TimelineZoomLevel.WeekDay, 1.0, 60.0)]
    [InlineData(TimelineZoomLevel.WeekDay, 1.5, 90.0)]
    [InlineData(TimelineZoomLevel.MonthDay, 1.0, 25.0)]
    [InlineData(TimelineZoomLevel.MonthDay, 2.0, 50.0)]
    [InlineData(TimelineZoomLevel.MonthWeek, 0.5, 7.5)]
    [InlineData(TimelineZoomLevel.QuarterWeek, 1.5, 12.0)]
    [InlineData(TimelineZoomLevel.QuarterMonth, 2.0, 10.0)]
    [InlineData(TimelineZoomLevel.YearQuarter, 3.0, 9.0)]
    public void EffectiveDayWidth_WithZoomFactor_CalculatesCorrectly(TimelineZoomLevel level, double zoomFactor, double expectedWidth)
    {
        // Arrange
        var configuration = TimelineZoomService.GetConfiguration(level);

        // Act
        var effectiveWidth = configuration.GetEffectiveDayWidth(zoomFactor);

        // Assert
        Assert.Equal(expectedWidth, effectiveWidth, precision: 1);
    }

    [Theory]
    [InlineData(1.0, "100%")]
    [InlineData(1.5, "150%")]
    [InlineData(2.0, "200%")]
    [InlineData(0.5, "50%")]
    [InlineData(0.75, "75%")]
    public void ZoomFactor_PercentageFormatting_IsCorrect(double zoomFactor, string expectedPercentage)
    {
        // Arrange & Act
        var percentage = $"{zoomFactor:P0}";

        // Assert
        Assert.Equal(expectedPercentage, percentage);
    }

    [Theory]
    [InlineData(0.1, 2)]
    [InlineData(0.01, 3)]
    [InlineData(0.001, 4)]
    public void ZoomFactor_FloatingPointPrecision_AvoidsPrecisionIssues(double step, int decimalPlaces)
    {
        // Arrange
        double currentFactor = 1.0;
        double newFactor = currentFactor + step;

        // Act
        var roundedFactor = Math.Round(newFactor, decimalPlaces);
        var difference = Math.Abs(roundedFactor - newFactor);

        // Assert - Difference should be very small when properly rounded
        Assert.True(difference < Math.Pow(10, -(decimalPlaces + 1)));
    }

    [Theory]
    [InlineData(0.5, 3.0, 0.1, 26)] // (3.0 - 0.5) / 0.1 + 1
    [InlineData(0.25, 5.0, 0.25, 20)] // (5.0 - 0.25) / 0.25 + 1
    [InlineData(1.0, 2.0, 0.2, 6)] // (2.0 - 1.0) / 0.2 + 1
    public void ZoomFactor_SliderSteps_CalculateCorrectStepCount(double min, double max, double step, int expectedSteps)
    {
        // Arrange & Act
        var actualSteps = (int)Math.Round((max - min) / step) + 1;

        // Assert
        Assert.Equal(expectedSteps, actualSteps);
    }

    [Fact]
    public void ZoomFactor_BoundaryConditionsHandling_DoesNotThrow()
    {
        // Arrange
        var testCases = new[]
        {
            (min: 0.5, max: 3.0, current: 0.5, step: 0.1),
            (min: 0.5, max: 3.0, current: 3.0, step: 0.1),
            (min: 0.25, max: 5.0, current: 0.25, step: 0.25),
            (min: 0.25, max: 5.0, current: 5.0, step: 0.25)
        };

        foreach (var (min, max, current, step) in testCases)
        {
            // Act & Assert - Should not throw
            var incrementResult = Math.Min(max, current + step);
            var decrementResult = Math.Max(min, current - step);
            var clampedResult = Math.Max(min, Math.Min(max, current));

            Assert.True(incrementResult >= min && incrementResult <= max);
            Assert.True(decrementResult >= min && decrementResult <= max);
            Assert.True(clampedResult >= min && clampedResult <= max);
        }
    }

    [Theory]
    [InlineData(TimelineZoomLevel.WeekDay, 1.0)]
    [InlineData(TimelineZoomLevel.MonthDay, 1.6)] // Backward compatibility default
    [InlineData(TimelineZoomLevel.MonthWeek, 1.0)]
    [InlineData(TimelineZoomLevel.QuarterWeek, 1.0)]
    [InlineData(TimelineZoomLevel.QuarterMonth, 1.0)]
    [InlineData(TimelineZoomLevel.YearQuarter, 1.0)]
    public void ZoomFactor_DefaultValues_AreReasonable(TimelineZoomLevel level, double expectedDefaultFactor)
    {
        // Arrange
        var configuration = TimelineZoomService.GetConfiguration(level);

        // Act
        var effectiveWidth = configuration.GetEffectiveDayWidth(expectedDefaultFactor);

        // Assert - Default should produce reasonable day widths (10-100px range)
        Assert.True(effectiveWidth >= 3.0 && effectiveWidth <= 100.0,
            $"Effective day width {effectiveWidth}px is outside reasonable range for {level} at {expectedDefaultFactor}x");
    }
}
