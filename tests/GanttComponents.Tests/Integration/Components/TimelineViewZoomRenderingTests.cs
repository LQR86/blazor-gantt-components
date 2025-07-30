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
            // ZoomLevel, ZoomFactor, ExpectedDayWidth
            (TimelineZoomLevel.WeekDay, 1.0, 60.0),      // Week level: 60px base
            (TimelineZoomLevel.MonthDay, 1.0, 25.0),     // Month level: 25px base
            (TimelineZoomLevel.MonthDay, 1.6, 40.0),     // Default backward compatibility
            (TimelineZoomLevel.MonthDay, 2.0, 50.0),     // 2x zoom factor
            (TimelineZoomLevel.YearQuarter, 1.0, 3.0),   // Year level: 3px base
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
            (level: TimelineZoomLevel.WeekDay, factor: 1.0, baseDayWidth: 60.0),
            (level: TimelineZoomLevel.MonthDay, factor: 1.0, baseDayWidth: 25.0),
            (level: TimelineZoomLevel.MonthWeek, factor: 1.0, baseDayWidth: 15.0),
            (level: TimelineZoomLevel.YearQuarter, factor: 1.0, baseDayWidth: 3.0),
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
        // Arrange
        var zoomLevel = TimelineZoomLevel.MonthDay; // 25px base
        var baseDayWidth = 25.0;
        var testFactors = new[] { 0.5, 1.0, 1.5, 2.0, 3.0 };

        var config = TimelineZoomService.GetConfiguration(zoomLevel);

        foreach (var factor in testFactors)
        {
            // Act
            var actualDayWidth = config.GetEffectiveDayWidth(factor);
            var expectedDayWidth = baseDayWidth * factor;

            // Assert - Width should scale proportionally with zoom factor
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
            (level: TimelineZoomLevel.WeekDay, factor: 1.0, expectedDayWidth: 60.0),
            (level: TimelineZoomLevel.MonthDay, factor: 1.6, expectedDayWidth: 40.0), // Backward compatibility
            (level: TimelineZoomLevel.YearQuarter, factor: 1.0, expectedDayWidth: 3.0),
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
            (level: TimelineZoomLevel.WeekDay, factor: 1.0, expectedDayWidth: 60.0),
            (level: TimelineZoomLevel.MonthDay, factor: 1.6, expectedDayWidth: 40.0), // Default
            (level: TimelineZoomLevel.YearQuarter, factor: 1.0, expectedDayWidth: 3.0),
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
        var defaultZoomLevel = TimelineZoomLevel.MonthDay;
        var defaultZoomFactor = 1.6;
        var expectedLegacyDayWidth = 40.0;

        // Act
        var config = TimelineZoomService.GetConfiguration(defaultZoomLevel);
        var actualDayWidth = config.GetEffectiveDayWidth(defaultZoomFactor);

        // Assert - Must maintain exact backward compatibility
        Assert.Equal(expectedLegacyDayWidth, actualDayWidth, precision: 1);
    }

    [Theory]
    [InlineData(1, 40.0)]   // 1 day task
    [InlineData(7, 280.0)]  // 1 week task  
    [InlineData(30, 1200.0)] // 1 month task
    public void TaskWidthCalculation_WithDefaultZoom_ShouldMatchExpected(
        int durationDays,
        double expectedTaskWidth)
    {
        // Arrange - Using default backward-compatible zoom (MonthDay @ 1.6x = 40px)
        var config = TimelineZoomService.GetConfiguration(TimelineZoomLevel.MonthDay);
        var dayWidth = config.GetEffectiveDayWidth(1.6);

        // Act - Calculate task width
        var actualTaskWidth = durationDays * dayWidth;

        // Assert - Task width should match expected values
        Assert.Equal(40.0, dayWidth, precision: 1); // Verify day width first
        Assert.Equal(expectedTaskWidth, actualTaskWidth, precision: 1);
    }
}
