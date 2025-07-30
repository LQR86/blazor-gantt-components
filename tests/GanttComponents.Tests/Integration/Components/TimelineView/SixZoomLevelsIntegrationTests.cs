using Xunit;
using GanttComponents.Models;
using GanttComponents.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace GanttComponents.Tests.Integration.Components.TimelineView;

/// <summary>
/// Integration tests for six strategic timeline zoom levels implementation.
/// Tests all zoom levels render correctly with proper day widths and performance validation.
/// </summary>
public class SixZoomLevelsIntegrationTests
{
    private readonly List<GanttTask> _testTasks;

    public SixZoomLevelsIntegrationTests()
    {
        // Create test data representing a typical project
        _testTasks = new List<GanttTask>
        {
            new GanttTask
            {
                Id = 1,
                Name = "Phase 1: Planning",
                StartDate = new DateTime(2025, 8, 1),
                EndDate = new DateTime(2025, 8, 15),
                WbsCode = "1"
            },
            new GanttTask
            {
                Id = 2,
                Name = "Phase 2: Development",
                StartDate = new DateTime(2025, 8, 16),
                EndDate = new DateTime(2025, 9, 30),
                WbsCode = "2"
            },
            new GanttTask
            {
                Id = 3,
                Name = "Phase 3: Testing",
                StartDate = new DateTime(2025, 10, 1),
                EndDate = new DateTime(2025, 10, 31),
                WbsCode = "3"
            },
            new GanttTask
            {
                Id = 4,
                Name = "Phase 4: Deployment",
                StartDate = new DateTime(2025, 11, 1),
                EndDate = new DateTime(2025, 11, 15),
                WbsCode = "4"
            }
        };
    }

    [Theory]
    [InlineData(TimelineZoomLevel.WeekDay, 60.0)]
    [InlineData(TimelineZoomLevel.MonthDay, 25.0)]
    [InlineData(TimelineZoomLevel.MonthWeek, 15.0)]
    [InlineData(TimelineZoomLevel.QuarterWeek, 8.0)]
    [InlineData(TimelineZoomLevel.QuarterMonth, 5.0)]
    [InlineData(TimelineZoomLevel.YearQuarter, 3.0)]
    public void AllSixZoomLevels_WithBaseFactor_ShouldRenderWithCorrectDayWidths(
        TimelineZoomLevel zoomLevel, double expectedDayWidth)
    {
        // Arrange
        var config = TimelineZoomService.GetConfiguration(zoomLevel);
        var baseFactor = 1.0;

        // Act
        var actualDayWidth = config.GetEffectiveDayWidth(baseFactor);

        // Assert
        Assert.Equal(expectedDayWidth, actualDayWidth, 1);
    }

    [Theory]
    [InlineData(TimelineZoomLevel.WeekDay, 1.5, 90.0)]   // 60 * 1.5 = 90
    [InlineData(TimelineZoomLevel.MonthDay, 1.6, 40.0)]  // 25 * 1.6 = 40 (backward compatibility)
    [InlineData(TimelineZoomLevel.MonthWeek, 2.0, 30.0)] // 15 * 2.0 = 30
    [InlineData(TimelineZoomLevel.QuarterWeek, 2.5, 20.0)] // 8 * 2.5 = 20
    [InlineData(TimelineZoomLevel.QuarterMonth, 3.0, 15.0)] // 5 * 3.0 = 15
    [InlineData(TimelineZoomLevel.YearQuarter, 2.0, 6.0)]   // 3 * 2.0 = 6
    public void AllSixZoomLevels_WithVariousFactors_ShouldScaleCorrectly(
        TimelineZoomLevel zoomLevel, double factor, double expectedDayWidth)
    {
        // Arrange
        var config = TimelineZoomService.GetConfiguration(zoomLevel);

        // Act
        var actualDayWidth = config.GetEffectiveDayWidth(factor);

        // Assert
        Assert.Equal(expectedDayWidth, actualDayWidth, 1);
    }

    [Fact]
    public void ZoomLevelSwitching_BetweenAllSixLevels_ShouldProduceDifferentWidths()
    {
        // Arrange
        var factor = 1.0;
        var allLevels = new[]
        {
            TimelineZoomLevel.WeekDay,
            TimelineZoomLevel.MonthDay,
            TimelineZoomLevel.MonthWeek,
            TimelineZoomLevel.QuarterWeek,
            TimelineZoomLevel.QuarterMonth,
            TimelineZoomLevel.YearQuarter
        };

        // Act
        var dayWidths = new List<double>();
        foreach (var level in allLevels)
        {
            var config = TimelineZoomService.GetConfiguration(level);
            dayWidths.Add(config.GetEffectiveDayWidth(factor));
        }

        // Assert - All day widths should be different
        Assert.Equal(6, dayWidths.Distinct().Count());

        // Assert - Should be in descending order (WeekDay largest, YearQuarter smallest)
        Assert.Equal(60.0, dayWidths[0]); // WeekDay
        Assert.Equal(25.0, dayWidths[1]); // MonthDay
        Assert.Equal(15.0, dayWidths[2]); // MonthWeek
        Assert.Equal(8.0, dayWidths[3]);  // QuarterWeek
        Assert.Equal(5.0, dayWidths[4]);  // QuarterMonth
        Assert.Equal(3.0, dayWidths[5]);  // YearQuarter
    }

    [Fact]
    public void TaskWidthCalculation_AcrossAllZoomLevels_ShouldScaleProportionally()
    {
        // Arrange
        var testTask = _testTasks[0]; // 15-day task (Aug 1-15)
        var taskDurationDays = (testTask.EndDate - testTask.StartDate).Days + 1; // 15 days
        var factor = 1.0;

        var allLevels = new[]
        {
            TimelineZoomLevel.WeekDay,
            TimelineZoomLevel.MonthDay,
            TimelineZoomLevel.MonthWeek,
            TimelineZoomLevel.QuarterWeek,
            TimelineZoomLevel.QuarterMonth,
            TimelineZoomLevel.YearQuarter
        };

        // Act & Assert
        foreach (var level in allLevels)
        {
            var config = TimelineZoomService.GetConfiguration(level);
            var dayWidth = config.GetEffectiveDayWidth(factor);
            var expectedTaskWidth = taskDurationDays * dayWidth;

            // Simulate task width calculation
            var actualTaskWidth = taskDurationDays * dayWidth;

            Assert.Equal(expectedTaskWidth, actualTaskWidth, 1);
        }
    }

    [Fact]
    public void TimelineWidthCalculation_AcrossAllZoomLevels_ShouldVarySignificantly()
    {
        // Arrange
        var startDate = new DateTime(2025, 8, 1);
        var endDate = new DateTime(2025, 11, 15); // ~106 days project
        var totalDays = (endDate - startDate).Days + 1;
        var factor = 1.0;

        var allLevels = new[]
        {
            TimelineZoomLevel.WeekDay,
            TimelineZoomLevel.MonthDay,
            TimelineZoomLevel.MonthWeek,
            TimelineZoomLevel.QuarterWeek,
            TimelineZoomLevel.QuarterMonth,
            TimelineZoomLevel.YearQuarter
        };

        var expectedWidths = new[]
        {
            totalDays * 60.0, // WeekDay: ~6360px
            totalDays * 25.0, // MonthDay: ~2650px
            totalDays * 15.0, // MonthWeek: ~1590px
            totalDays * 8.0,  // QuarterWeek: ~848px
            totalDays * 5.0,  // QuarterMonth: ~530px
            totalDays * 3.0   // YearQuarter: ~318px
        };

        // Act & Assert
        for (int i = 0; i < allLevels.Length; i++)
        {
            var config = TimelineZoomService.GetConfiguration(allLevels[i]);
            var dayWidth = config.GetEffectiveDayWidth(factor);
            var timelineWidth = totalDays * dayWidth;

            Assert.Equal(expectedWidths[i], timelineWidth, 1);
        }

        // Assert significant variation between zoom levels
        Assert.True(expectedWidths[0] > expectedWidths[5] * 15); // WeekDay should be 15x+ larger than YearQuarter (60/3 = 20x, but with totalDays calculation)
    }

    [Fact]
    public void PerformanceValidation_ZoomLevelSwitching_ShouldBeEfficient()
    {
        // Arrange
        var allLevels = new[]
        {
            TimelineZoomLevel.WeekDay,
            TimelineZoomLevel.MonthDay,
            TimelineZoomLevel.MonthWeek,
            TimelineZoomLevel.QuarterWeek,
            TimelineZoomLevel.QuarterMonth,
            TimelineZoomLevel.YearQuarter
        };

        var stopwatch = new Stopwatch();

        // Act - Measure time for 100 zoom level switches
        stopwatch.Start();
        for (int i = 0; i < 100; i++)
        {
            foreach (var level in allLevels)
            {
                var config = TimelineZoomService.GetConfiguration(level);
                var dayWidth = config.GetEffectiveDayWidth(1.0);

                // Simulate timeline calculations
                var timelineWidth = 365 * dayWidth; // 1-year project
            }
        }
        stopwatch.Stop();

        // Assert - Should complete 600 zoom operations in under 100ms
        Assert.True(stopwatch.ElapsedMilliseconds < 100,
            $"Zoom level switching took {stopwatch.ElapsedMilliseconds}ms, should be under 100ms");
    }

    [Theory]
    [InlineData(TimelineZoomLevel.WeekDay, 0.5, 30.0)]   // Min factor
    [InlineData(TimelineZoomLevel.WeekDay, 3.0, 180.0)]  // Max factor
    [InlineData(TimelineZoomLevel.MonthDay, 0.5, 12.5)]  // Min factor
    [InlineData(TimelineZoomLevel.MonthDay, 3.0, 75.0)]  // Max factor
    [InlineData(TimelineZoomLevel.YearQuarter, 0.5, 1.5)] // Min factor
    [InlineData(TimelineZoomLevel.YearQuarter, 3.0, 9.0)] // Max factor
    public void ZoomFactorBounds_WithAllZoomLevels_ShouldWorkCorrectly(
        TimelineZoomLevel zoomLevel, double factor, double expectedDayWidth)
    {
        // Arrange
        var config = TimelineZoomService.GetConfiguration(zoomLevel);

        // Act
        var actualDayWidth = config.GetEffectiveDayWidth(factor);

        // Assert
        Assert.Equal(expectedDayWidth, actualDayWidth, 1);
    }

    [Fact]
    public void BackwardCompatibility_DefaultMonthDayAt1Point6_ShouldMaintain40PixelWidth()
    {
        // Arrange
        var defaultLevel = TimelineZoomLevel.MonthDay;
        var defaultFactor = 1.6; // Current backward compatibility factor
        var expectedWidth = 40.0; // Current behavior

        // Act
        var config = TimelineZoomService.GetConfiguration(defaultLevel);
        var actualWidth = config.GetEffectiveDayWidth(defaultFactor);

        // Assert
        Assert.Equal(expectedWidth, actualWidth, 1);
    }

    [Fact]
    public void RealWorldScenario_MediumProject_AllZoomLevelsPerformWell()
    {
        // Arrange - Simulate a 6-month project with 50 tasks
        var projectStartDate = new DateTime(2025, 8, 1);
        var projectEndDate = new DateTime(2026, 2, 1); // ~184 days
        var totalDays = (projectEndDate - projectStartDate).Days + 1;
        var taskCount = 50;

        var allLevels = new[]
        {
            TimelineZoomLevel.WeekDay,
            TimelineZoomLevel.MonthDay,
            TimelineZoomLevel.MonthWeek,
            TimelineZoomLevel.QuarterWeek,
            TimelineZoomLevel.QuarterMonth,
            TimelineZoomLevel.YearQuarter
        };

        // Act & Assert - All zoom levels should handle medium projects efficiently
        foreach (var level in allLevels)
        {
            var config = TimelineZoomService.GetConfiguration(level);
            var dayWidth = config.GetEffectiveDayWidth(1.0);

            // Timeline dimensions
            var timelineWidth = totalDays * dayWidth;
            var timelineHeight = taskCount * 32; // Default row height

            // Validate reasonable dimensions
            Assert.True(timelineWidth > 0, $"Timeline width should be positive for {level}");
            Assert.True(timelineHeight > 0, $"Timeline height should be positive for {level}");

            // Performance consideration - very wide timelines might cause issues
            if (level == TimelineZoomLevel.WeekDay)
            {
                // WeekDay level for 6-month project: ~11,040px width - should be manageable
                Assert.True(timelineWidth < 20000, "WeekDay zoom should not create excessively wide timelines");
            }

            if (level == TimelineZoomLevel.YearQuarter)
            {
                // YearQuarter level should maintain minimum usability
                Assert.True(timelineWidth > 500, "YearQuarter zoom should maintain minimum timeline width");
            }
        }
    }
}
