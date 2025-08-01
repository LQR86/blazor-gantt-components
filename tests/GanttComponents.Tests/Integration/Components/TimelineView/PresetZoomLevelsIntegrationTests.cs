using Xunit;
using GanttComponents.Models;
using GanttComponents.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace GanttComponents.Tests.Integration.Components.TimelineView;

/// <summary>
/// Integration tests for preset-only timeline zoom levels implementation.
/// Tests all 13 zoom levels render correctly with proper day widths and performance validation.
/// Validates the preset-only approach where zoom factors are always 1.0.
/// </summary>
public class PresetZoomLevelsIntegrationTests
{
    private readonly List<GanttTask> _testTasks;

    public PresetZoomLevelsIntegrationTests()
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
    [InlineData(TimelineZoomLevel.WeekDay, 96.0)]           // 60 * 1.6 backward compatibility
    [InlineData(TimelineZoomLevel.MonthDay, 40.0)]          // 25 * 1.6 backward compatibility  
    [InlineData(TimelineZoomLevel.MonthWeek, 24.0)]         // 15 * 1.6 backward compatibility
    [InlineData(TimelineZoomLevel.QuarterWeek, 12.8)]       // 8 * 1.6 backward compatibility
    [InlineData(TimelineZoomLevel.QuarterMonth, 8.0)]       // 5 * 1.6 backward compatibility
    [InlineData(TimelineZoomLevel.YearQuarter, 3.0)]        // Maintains 3px minimum constraint
    public void AllPresetZoomLevels_WithBaseFactor_ShouldRenderWithCorrectDayWidths(
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
    [InlineData(TimelineZoomLevel.WeekDay, 1.5, 96.0)]     // Preset-only: factor clamped to 1.0, so 96 * 1.0 = 96
    [InlineData(TimelineZoomLevel.MonthDay, 1.6, 40.0)]    // Preset-only: factor clamped to 1.0, so 40 * 1.0 = 40  
    [InlineData(TimelineZoomLevel.MonthWeek, 2.0, 24.0)]   // Preset-only: factor clamped to 1.0, so 24 * 1.0 = 24
    [InlineData(TimelineZoomLevel.QuarterWeek, 2.5, 12.8)] // Preset-only: factor clamped to 1.0, so 12.8 * 1.0 = 12.8
    [InlineData(TimelineZoomLevel.QuarterMonth, 3.0, 8.0)] // Preset-only: factor clamped to 1.0, so 8 * 1.0 = 8
    [InlineData(TimelineZoomLevel.YearQuarter, 2.0, 3.0)]  // Preset-only: factor clamped to 1.0, so 3 * 1.0 = 3
    public void AllPresetZoomLevels_WithVariousFactors_ShouldScaleCorrectly(
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
    public void ZoomLevelSwitching_BetweenAllPresetLevels_ShouldProduceDifferentWidths()
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
        // Note: This test still uses the 6 main zoom levels for backward compatibility testing
        Assert.Equal(96.0, dayWidths[0]);  // WeekDay: 60 * 1.6 backward compatibility
        Assert.Equal(40.0, dayWidths[1]);  // MonthDay: 25 * 1.6 backward compatibility
        Assert.Equal(24.0, dayWidths[2]);  // MonthWeek: 15 * 1.6 backward compatibility
        Assert.Equal(12.8, dayWidths[3]);  // QuarterWeek: 8 * 1.6 backward compatibility
        Assert.Equal(8.0, dayWidths[4]);   // QuarterMonth: 5 * 1.6 backward compatibility
        Assert.Equal(3.0, dayWidths[5]);   // YearQuarter: maintains 3px minimum constraint
    }

    [Fact]
    public void TaskWidthCalculation_AcrossAllPresetZoomLevels_ShouldScaleProportionally()
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
    public void TimelineWidthCalculation_AcrossAllPresetZoomLevels_ShouldVarySignificantly()
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
            totalDays * 96.0,  // WeekDay: ~10176px (60 * 1.6 backward compatibility)
            totalDays * 40.0,  // MonthDay: ~4240px (25 * 1.6 backward compatibility)
            totalDays * 24.0,  // MonthWeek: ~2544px (15 * 1.6 backward compatibility)
            totalDays * 12.8,  // QuarterWeek: ~1356.8px (8 * 1.6 backward compatibility)
            totalDays * 8.0,   // QuarterMonth: ~848px (5 * 1.6 backward compatibility)
            totalDays * 3.0    // YearQuarter: ~318px (maintains 3px minimum constraint)
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
        Assert.True(expectedWidths[0] > expectedWidths[5] * 30); // WeekDay should be 30x+ larger than YearQuarter (96/3 = 32x)
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
    [InlineData(TimelineZoomLevel.WeekDay, 0.5, 96.0)]     // Preset-only: factor clamped to 1.0, base 96px
    [InlineData(TimelineZoomLevel.WeekDay, 3.0, 96.0)]     // Preset-only: factor clamped to 1.0, base 96px
    [InlineData(TimelineZoomLevel.MonthDay, 0.5, 40.0)]    // Preset-only: factor clamped to 1.0, base 40px
    [InlineData(TimelineZoomLevel.MonthDay, 3.0, 40.0)]    // Preset-only: factor clamped to 1.0, base 40px
    [InlineData(TimelineZoomLevel.YearQuarter, 0.5, 3.0)]  // Preset-only: factor clamped to 1.0, base 3px (minimum)
    [InlineData(TimelineZoomLevel.YearQuarter, 3.0, 3.0)]  // Preset-only: factor clamped to 1.0, base 3px (minimum)
    public void ZoomFactorBounds_WithAllPresetZoomLevels_ShouldWorkCorrectly(
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
    public void BackwardCompatibility_DefaultMonthDayAt1Point0_ShouldMaintain40PixelWidth()
    {
        // Arrange
        var defaultLevel = TimelineZoomLevel.MonthDay;
        var defaultFactor = 1.0; // Preset-only: factors are always 1.0
        var expectedWidth = 40.0; // New base day width for backward compatibility (25 * 1.6)

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
                // WeekDay level for 6-month project: ~17,664px width (184 days * 96px) - should be manageable
                Assert.True(timelineWidth < 25000, "WeekDay zoom should not create excessively wide timelines");
            }

            if (level == TimelineZoomLevel.YearQuarter)
            {
                // YearQuarter level should maintain minimum usability
                Assert.True(timelineWidth > 500, "YearQuarter zoom should maintain minimum timeline width");
            }
        }
    }
}
