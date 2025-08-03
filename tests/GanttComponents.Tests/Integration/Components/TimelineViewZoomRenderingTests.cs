using GanttComponents.Models;
using GanttComponents.Services;
using Xunit;

namespace GanttComponents.Tests.Integration.Components;

/// <summary>
/// Integration tests validating that zoom parameter changes affect actual TimelineView rendering.
/// Focus: Verify that width calculations respond to zoom parameter changes.
/// </summary>
public class TimelineViewZoomRenderingTests
{
    [Fact]
    public void ZoomParameterChanges_ShouldAffectHorizontalScaling()
    {
        // Arrange - Test different zoom configurations
        var testCases = new[]
        {
            // ZoomLevel, ZoomFactor, ExpectedDayWidth (preset-only system, factors clamped to 1.0)
            (TimelineZoomLevel.WeekDay97px, 1.0, 97.0),      // Week level: 97px base 
            (TimelineZoomLevel.MonthDay48px, 1.0, 48.0),     // Month level: 48px base 
            (TimelineZoomLevel.MonthDay48px, 1.6, 48.0),     // Preset-only: factor clamped to 1.0
            (TimelineZoomLevel.MonthDay48px, 2.0, 48.0),     // Preset-only: factor clamped to 1.0
            (TimelineZoomLevel.Month8px, 1.0, 8.0),          // Year level: 8px base 
        };

        foreach (var (zoomLevel, zoomFactor, expectedDayWidth) in testCases)
        {
            // Act - Calculate effective day width using the same logic as TimelineView
            var config = TimelineZoomService.GetConfiguration(zoomLevel);
            var actualDayWidth = config.GetEffectiveDayWidth(zoomFactor);

            // Assert - Zoom parameters should affect day width calculation
            Assert.Equal(expectedDayWidth, actualDayWidth, precision: 1);
        }
    }

    [Fact]
    public void DifferentZoomLevels_ShouldProduceDifferentWidthCalculations()
    {
        // Arrange - Sample task duration (5 days)
        var taskDurationDays = 5;

        var zoomConfigurations = new[]
        {
            (level: TimelineZoomLevel.WeekDay97px, factor: 1.0, baseDayWidth: 97.0),      // 97px integral
            (level: TimelineZoomLevel.MonthDay48px, factor: 1.0, baseDayWidth: 48.0),    // 48px integral  
            (level: TimelineZoomLevel.QuarterMonth24px, factor: 1.0, baseDayWidth: 24.0), // 24px integral
            (level: TimelineZoomLevel.Month8px, factor: 1.0, baseDayWidth: 8.0),         // 8px integral
        };

        var calculatedWidths = new List<double>();

        foreach (var (level, factor, expectedDayWidth) in zoomConfigurations)
        {
            // Act - Calculate task width using zoom configuration
            var config = TimelineZoomService.GetConfiguration(level);
            var dayWidth = config.GetEffectiveDayWidth(factor);
            var taskWidth = taskDurationDays * dayWidth;

            // Verify day width matches expected
            Assert.Equal(expectedDayWidth, dayWidth, precision: 1);

            calculatedWidths.Add(taskWidth);
        }

        // Assert - All zoom levels should produce different task widths
        var distinctWidths = calculatedWidths.Distinct().ToList();
        Assert.Equal(calculatedWidths.Count, distinctWidths.Count);

        // Assert - Widths should be in descending order (WeekDay > MonthDay > MonthWeek > YearQuarter)
        Assert.True(calculatedWidths[0] > calculatedWidths[1]); // WeekDay > MonthDay
        Assert.True(calculatedWidths[1] > calculatedWidths[2]); // MonthDay > MonthWeek  
        Assert.True(calculatedWidths[2] > calculatedWidths[3]); // MonthWeek > YearQuarter
    }

    [Fact]
    public void ZoomFactorRange_ShouldAffectWidthProportionally()
    {
        // Arrange - In preset-only system, factors are clamped to 1.0
        var zoomLevel = TimelineZoomLevel.MonthDay48px; // 48px base (integral pixel)
        var baseDayWidth = 48.0;
        var testFactors = new[] { 0.5, 1.0, 1.5, 2.0, 3.0 };

        var config = TimelineZoomService.GetConfiguration(zoomLevel);

        foreach (var factor in testFactors)
        {
            // Act - In preset-only system, all factors result in same width (clamped to 1.0)
            var actualDayWidth = config.GetEffectiveDayWidth(factor);
            var expectedDayWidth = baseDayWidth; // Always 48px regardless of factor

            // Assert - All factors should result in same day width in preset-only system
            Assert.Equal(expectedDayWidth, actualDayWidth, precision: 1);
        }
    }

    [Fact]
    public void MonthWidthCalculation_ShouldScaleWithZoomParameters()
    {
        // Arrange - Test month with 30 days
        var daysInMonth = 30;
        var testCases = new[]
        {
            (level: TimelineZoomLevel.WeekDay97px, factor: 1.0, expectedDayWidth: 97.0),   // 11-level integral: 97px
            (level: TimelineZoomLevel.MonthDay48px, factor: 1.6, expectedDayWidth: 48.0), // 11-level integral: 48px (factor ignored)
            (level: TimelineZoomLevel.Month8px, factor: 1.0, expectedDayWidth: 8.0), // 11-level integral: 8px
        };

        foreach (var (level, factor, expectedDayWidth) in testCases)
        {
            // Act - Calculate month width using zoom configuration  
            var config = TimelineZoomService.GetConfiguration(level);
            var dayWidth = config.GetEffectiveDayWidth(factor);
            var monthWidth = daysInMonth * dayWidth;
            var expectedMonthWidth = daysInMonth * expectedDayWidth;

            // Assert - Month width should scale with zoom parameters
            Assert.Equal(expectedDayWidth, dayWidth, precision: 1);
            Assert.Equal(expectedMonthWidth, monthWidth, precision: 1);
        }
    }

    [Fact]
    public void TimelineWidthCalculation_ShouldScaleWithZoomParameters()
    {
        // Arrange - Test timeline spanning 90 days
        var totalDays = 90;
        var testCases = new[]
        {
            (level: TimelineZoomLevel.WeekDay97px, factor: 1.0, expectedDayWidth: 97.0),   // 11-level integral: 97px
            (level: TimelineZoomLevel.MonthDay48px, factor: 1.6, expectedDayWidth: 48.0), // 11-level integral: 48px (factor ignored)
            (level: TimelineZoomLevel.Month8px, factor: 1.0, expectedDayWidth: 8.0), // 11-level integral: 8px
        };

        foreach (var (level, factor, expectedDayWidth) in testCases)
        {
            // Act - Calculate total timeline width using zoom configuration
            var config = TimelineZoomService.GetConfiguration(level);
            var dayWidth = config.GetEffectiveDayWidth(factor);
            var timelineWidth = totalDays * dayWidth;
            var expectedTimelineWidth = totalDays * expectedDayWidth;

            // Assert - Timeline width should scale with zoom parameters
            Assert.Equal(expectedDayWidth, dayWidth, precision: 1);
            Assert.Equal(expectedTimelineWidth, timelineWidth, precision: 1);
        }
    }

    [Fact]
    public void BackwardCompatibility_ShouldMaintainExactWidth()
    {
        // Arrange - Default parameters that should maintain 40px day width
        var defaultZoomLevel = TimelineZoomLevel.MonthDay48px;
        var defaultZoomFactor = 1.6;
        var expectedLegacyDayWidth = 48.0; // Updated for 11-level system: MonthDay48px = 48px

        // Act
        var config = TimelineZoomService.GetConfiguration(defaultZoomLevel);
        var actualDayWidth = config.GetEffectiveDayWidth(defaultZoomFactor);

        // Assert - Must maintain exact backward compatibility
        Assert.Equal(expectedLegacyDayWidth, actualDayWidth, precision: 1);
    }

    [Theory]
    [InlineData(1, 48.0)]    // 1 day task: 1 * 48px = 48px
    [InlineData(7, 336.0)]   // 1 week task: 7 * 48px = 336px
    [InlineData(30, 1440.0)] // 1 month task: 30 * 48px = 1440px
    public void TaskWidthCalculation_WithDefaultZoom_ShouldMatchExpected(
        int durationDays,
        double expectedTaskWidth)
    {
        // Arrange - Using default zoom (MonthDay48px @ 1.6x = 48px in 11-level system)
        var config = TimelineZoomService.GetConfiguration(TimelineZoomLevel.MonthDay48px);
        var dayWidth = config.GetEffectiveDayWidth(1.6);

        // Act - Calculate task width
        var actualTaskWidth = durationDays * dayWidth;

        // Assert - Task width should match expected values
        Assert.Equal(48.0, dayWidth, precision: 1); // Verify day width first (11-level system: 48px)
        Assert.Equal(expectedTaskWidth, actualTaskWidth, precision: 1);
    }
}
