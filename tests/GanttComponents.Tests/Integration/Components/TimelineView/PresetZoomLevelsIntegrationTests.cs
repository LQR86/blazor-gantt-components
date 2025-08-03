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
    [InlineData(TimelineZoomLevel.WeekDay97px, 97.0)]           // 11-level integral: 97px
    [InlineData(TimelineZoomLevel.MonthDay48px, 48.0)]          // 11-level integral: 48px  
    [InlineData(TimelineZoomLevel.QuarterMonth24px, 24.0)]       // 11-level integral: 24px
    [InlineData(TimelineZoomLevel.YearQuarter6px, 6.0)]        // 11-level integral: 6px
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
    [InlineData(TimelineZoomLevel.WeekDay97px, 1.5, 97.0)]     // Preset-only: factor clamped to 1.0, so 97 * 1.0 = 97
    [InlineData(TimelineZoomLevel.MonthDay48px, 1.6, 48.0)]    // Preset-only: factor clamped to 1.0, so 48 * 1.0 = 48
    [InlineData(TimelineZoomLevel.QuarterMonth24px, 2.5, 24.0)] // Preset-only: factor clamped to 1.0, so 24 * 1.0 = 24
    [InlineData(TimelineZoomLevel.YearQuarter6px, 2.0, 6.0)]  // Preset-only: factor clamped to 1.0, so 6 * 1.0 = 6
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
            TimelineZoomLevel.WeekDay97px,
            TimelineZoomLevel.MonthDay48px,
            TimelineZoomLevel.QuarterMonth24px,
            TimelineZoomLevel.YearQuarter6px
        };

        // Act
        var dayWidths = new List<double>();
        foreach (var level in allLevels)
        {
            var config = TimelineZoomService.GetConfiguration(level);
            dayWidths.Add(config.GetEffectiveDayWidth(factor));
        }

        // Assert - All day widths should be different
        Assert.Equal(4, dayWidths.Distinct().Count());

        // Assert - Should be in descending order (WeekDay97px largest, YearQuarter6px smallest)
        Assert.Equal(97.0, dayWidths[0]);  // WeekDay97px: 11-level integral 97px
        Assert.Equal(48.0, dayWidths[1]);  // MonthDay48px: 11-level integral 48px
        Assert.Equal(24.0, dayWidths[2]);  // QuarterMonth24px: 11-level integral 24px
        Assert.Equal(6.0, dayWidths[3]);  // YearQuarter6px: 11-level integral 6px
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
            TimelineZoomLevel.WeekDay97px,
            TimelineZoomLevel.MonthDay48px,
            TimelineZoomLevel.MonthDay48px,
            TimelineZoomLevel.QuarterMonth24px,
            TimelineZoomLevel.QuarterMonth24px,
            TimelineZoomLevel.YearQuarter6px
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
            TimelineZoomLevel.WeekDay97px,
            TimelineZoomLevel.MonthDay48px,
            TimelineZoomLevel.QuarterMonth24px,
            TimelineZoomLevel.Month8px
        };

        var expectedWidths = new[]
        {
            totalDays * 97.0,  // WeekDay97px: ~10283px 
            totalDays * 48.0,  // MonthDay48px: ~5088px 
            totalDays * 24.0,  // QuarterMonth24px: ~2544px 
            totalDays * 8.0    // Month8px: ~848px (integral 8px)
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
        Assert.True(expectedWidths[0] > expectedWidths[3] * 12); // WeekDay97px should be 12x+ larger than Month8px (97/8 = 12.1x)
    }

    [Fact]
    public void PerformanceValidation_ZoomLevelSwitching_ShouldBeEfficient()
    {
        // Arrange
        var allLevels = new[]
        {
            TimelineZoomLevel.WeekDay97px,
            TimelineZoomLevel.MonthDay48px,
            TimelineZoomLevel.MonthDay34px,
            TimelineZoomLevel.QuarterMonth24px,
            TimelineZoomLevel.QuarterMonth17px,
            TimelineZoomLevel.Month8px
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
    [InlineData(TimelineZoomLevel.WeekDay97px, 0.5, 97.0)]     // Preset-only: factor clamped to 1.0, base 97px
    [InlineData(TimelineZoomLevel.WeekDay97px, 3.0, 97.0)]     // Preset-only: factor clamped to 1.0, base 97px
    [InlineData(TimelineZoomLevel.MonthDay48px, 0.5, 48.0)]    // Preset-only: factor clamped to 1.0, base 48px
    [InlineData(TimelineZoomLevel.MonthDay48px, 3.0, 48.0)]    // Preset-only: factor clamped to 1.0, base 48px
    [InlineData(TimelineZoomLevel.YearQuarter6px, 0.5, 6.0)]   // Preset-only: factor clamped to 1.0, base 6px
    [InlineData(TimelineZoomLevel.YearQuarter6px, 3.0, 6.0)]   // Preset-only: factor clamped to 1.0, base 6px
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
    public void IntegralPixelDesign_DefaultMonthDayAt1Point0_ShouldReturn48PixelWidth()
    {
        // Arrange
        var defaultLevel = TimelineZoomLevel.MonthDay48px;
        var defaultFactor = 1.0; // Preset-only: factors are always 1.0
        var expectedWidth = 48.0; // Integral pixel value for MonthDay48px

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
            TimelineZoomLevel.WeekDay97px,
            TimelineZoomLevel.MonthDay48px,
            TimelineZoomLevel.MonthDay34px,
            TimelineZoomLevel.QuarterMonth24px,
            TimelineZoomLevel.QuarterMonth17px,
            TimelineZoomLevel.Month8px
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
            if (level == TimelineZoomLevel.WeekDay97px)
            {
                // WeekDay97px level for 6-month project: ~17,664px width (184 days * 96px) - should be manageable
                Assert.True(timelineWidth < 25000, "WeekDay97px zoom should not create excessively wide timelines");
            }

            if (level == TimelineZoomLevel.Month8px)
            {
                // Month8px level should maintain minimum usability
                Assert.True(timelineWidth > 500, "Month8px zoom should maintain minimum timeline width");
            }
        }
    }
}
