using GanttComponents.Models;
using GanttComponents.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GanttComponents.Services;

/// <summary>
/// Provides zoom-level-appropriate header configurations for timeline views.
/// Now supports both legacy configurations and new preset templates.
/// </summary>
public static class TimelineHeaderAdapter
{
    /// <summary>
    /// Gets the optimal header configuration using preset templates (recommended approach).
    /// Uses predefined templates for predictable, testable behavior.
    /// </summary>
    /// <param name="zoomLevel">Current zoom level</param>
    /// <param name="logger">Optional logger for debugging (can be null)</param>
    /// <returns>Header configuration based on preset template</returns>
    public static TimelineHeaderConfiguration GetHeaderConfigurationFromTemplate(TimelineZoomLevel zoomLevel, IUniversalLogger? logger = null)
    {
        // Get preset template for this zoom level
        var template = TimelineHeaderTemplateService.GetTemplate(zoomLevel);

        // Convert template to configuration structure
        return new TimelineHeaderConfiguration
        {
            PrimaryUnit = template.PrimaryUnit,
            PrimaryFormat = template.PrimaryFormat,
            SecondaryUnit = template.SecondaryUnit,
            SecondaryFormat = template.SecondaryFormat,
            ShowPrimary = template.ShowPrimary,
            ShowSecondary = template.ShowSecondary,
            // Set reasonable minimum widths based on zoom level characteristics
            MinPrimaryWidth = GetMinimumPrimaryWidth(zoomLevel),
            MinSecondaryWidth = GetMinimumSecondaryWidth(zoomLevel)
        };
    }

    /// <summary>
    /// Gets the optimal header configuration for a given zoom level and day width.
    /// Legacy method - kept for backward compatibility. Consider using GetHeaderConfigurationFromTemplate instead.
    /// </summary>
    /// <param name="zoomLevel">Current zoom level</param>
    /// <param name="effectiveDayWidth">Current effective day width in pixels</param>
    /// <returns>Header configuration with timescale and format recommendations</returns>
#pragma warning disable CS0618 // Type or member is obsolete - Legacy zoom levels preserved for backward compatibility
    public static TimelineHeaderConfiguration GetHeaderConfiguration(TimelineZoomLevel zoomLevel, double effectiveDayWidth)
    {
        return zoomLevel switch
        {
            // ========================================
            // OPTIMAL ZOOM LEVELS (Cell-size-first approach)
            // ========================================

            // Year-Quarter Optimal Pattern (4 levels): 30px-70px quarter cells
            // All levels use same pattern but optimize for specific cell size

            TimelineZoomLevel.YearQuarterOptimal30px => new TimelineHeaderConfiguration
            {
                PrimaryUnit = TimelineHeaderUnit.Year,
                PrimaryFormat = "date.year",
                SecondaryUnit = TimelineHeaderUnit.Quarter,
                SecondaryFormat = "date.quarter-short",
                ShowPrimary = true,
                ShowSecondary = true,
                MinPrimaryWidth = 120,  // Optimized for 30px quarters (4 quarters = 120px)
                MinSecondaryWidth = 30  // Perfect 30px quarter cells
            },

            TimelineZoomLevel.YearQuarterOptimal40px => new TimelineHeaderConfiguration
            {
                PrimaryUnit = TimelineHeaderUnit.Year,
                PrimaryFormat = "date.year",
                SecondaryUnit = TimelineHeaderUnit.Quarter,
                SecondaryFormat = "date.quarter-short",
                ShowPrimary = true,
                ShowSecondary = true,
                MinPrimaryWidth = 160,  // Optimized for 40px quarters (4 quarters = 160px)
                MinSecondaryWidth = 40  // Perfect 40px quarter cells
            },

            TimelineZoomLevel.YearQuarterOptimal50px => new TimelineHeaderConfiguration
            {
                PrimaryUnit = TimelineHeaderUnit.Year,
                PrimaryFormat = "date.year",
                SecondaryUnit = TimelineHeaderUnit.Quarter,
                SecondaryFormat = "date.quarter-short",
                ShowPrimary = true,
                ShowSecondary = true,
                MinPrimaryWidth = 200,  // Optimized for 50px quarters (4 quarters = 200px)
                MinSecondaryWidth = 50  // Perfect 50px quarter cells
            },

            TimelineZoomLevel.YearQuarterOptimal70px => new TimelineHeaderConfiguration
            {
                PrimaryUnit = TimelineHeaderUnit.Year,
                PrimaryFormat = "date.year",
                SecondaryUnit = TimelineHeaderUnit.Quarter,
                SecondaryFormat = "date.quarter-short",
                ShowPrimary = true,
                ShowSecondary = true,
                MinPrimaryWidth = 280,  // Optimized for 70px quarters (4 quarters = 280px)
                MinSecondaryWidth = 70  // Perfect 70px quarter cells
            },

            // Quarter-Month Optimal Pattern (5 levels): 30px-70px month cells

            TimelineZoomLevel.QuarterMonthOptimal30px => new TimelineHeaderConfiguration
            {
                PrimaryUnit = TimelineHeaderUnit.Quarter,
                PrimaryFormat = "date.quarter-year",
                SecondaryUnit = TimelineHeaderUnit.Month,
                SecondaryFormat = "date.month-short",
                ShowPrimary = true,
                ShowSecondary = true,
                MinPrimaryWidth = 90,   // Optimized for 30px months (3 months = 90px)
                MinSecondaryWidth = 30  // Perfect 30px month cells
            },

            TimelineZoomLevel.QuarterMonthOptimal40px => new TimelineHeaderConfiguration
            {
                PrimaryUnit = TimelineHeaderUnit.Quarter,
                PrimaryFormat = "date.quarter-year",
                SecondaryUnit = TimelineHeaderUnit.Month,
                SecondaryFormat = "date.month-short",
                ShowPrimary = true,
                ShowSecondary = true,
                MinPrimaryWidth = 120,  // Optimized for 40px months (3 months = 120px)
                MinSecondaryWidth = 40  // Perfect 40px month cells
            },

            TimelineZoomLevel.QuarterMonthOptimal50px => new TimelineHeaderConfiguration
            {
                PrimaryUnit = TimelineHeaderUnit.Quarter,
                PrimaryFormat = "date.quarter-year",
                SecondaryUnit = TimelineHeaderUnit.Month,
                SecondaryFormat = "date.month-short",
                ShowPrimary = true,
                ShowSecondary = true,
                MinPrimaryWidth = 150,  // Optimized for 50px months (3 months = 150px)
                MinSecondaryWidth = 50  // Perfect 50px month cells
            },

            TimelineZoomLevel.QuarterMonthOptimal60px => new TimelineHeaderConfiguration
            {
                PrimaryUnit = TimelineHeaderUnit.Quarter,
                PrimaryFormat = "date.quarter-year",
                SecondaryUnit = TimelineHeaderUnit.Month,
                SecondaryFormat = "date.month-short",
                ShowPrimary = true,
                ShowSecondary = true,
                MinPrimaryWidth = 180,  // Optimized for 60px months (3 months = 180px)
                MinSecondaryWidth = 60  // Perfect 60px month cells
            },

            // Month-Week Optimal Pattern (4 levels): 30px-60px week cells

            TimelineZoomLevel.MonthWeekOptimal30px => new TimelineHeaderConfiguration
            {
                PrimaryUnit = TimelineHeaderUnit.Month,
                PrimaryFormat = "date.month-year",
                SecondaryUnit = TimelineHeaderUnit.Week,
                SecondaryFormat = "date.week-short",
                ShowPrimary = true,
                ShowSecondary = true,
                MinPrimaryWidth = 120,  // Optimized for 30px weeks (~4 weeks = 120px)
                MinSecondaryWidth = 30  // Perfect 30px week cells
            },

            TimelineZoomLevel.MonthWeekOptimal40px => new TimelineHeaderConfiguration
            {
                PrimaryUnit = TimelineHeaderUnit.Month,
                PrimaryFormat = "date.month-year",
                SecondaryUnit = TimelineHeaderUnit.Week,
                SecondaryFormat = "date.week-short",
                ShowPrimary = true,
                ShowSecondary = true,
                MinPrimaryWidth = 160,  // Optimized for 40px weeks (~4 weeks = 160px)
                MinSecondaryWidth = 40  // Perfect 40px week cells
            },

            TimelineZoomLevel.MonthWeekOptimal50px => new TimelineHeaderConfiguration
            {
                PrimaryUnit = TimelineHeaderUnit.Month,
                PrimaryFormat = "date.month-year",
                SecondaryUnit = TimelineHeaderUnit.Week,
                SecondaryFormat = "date.week-short",
                ShowPrimary = true,
                ShowSecondary = true,
                MinPrimaryWidth = 200,  // Optimized for 50px weeks (~4 weeks = 200px)
                MinSecondaryWidth = 50  // Perfect 50px week cells
            },

            TimelineZoomLevel.MonthWeekOptimal60px => new TimelineHeaderConfiguration
            {
                PrimaryUnit = TimelineHeaderUnit.Month,
                PrimaryFormat = "date.month-year",
                SecondaryUnit = TimelineHeaderUnit.Week,
                SecondaryFormat = "date.week-short",
                ShowPrimary = true,
                ShowSecondary = true,
                MinPrimaryWidth = 240,  // Optimized for 60px weeks (~4 weeks = 240px)
                MinSecondaryWidth = 60  // Perfect 60px week cells
            },

            // Week-Day Optimal Pattern (4 levels): 30px-60px day cells

            TimelineZoomLevel.WeekDayOptimal30px => new TimelineHeaderConfiguration
            {
                PrimaryUnit = TimelineHeaderUnit.Week,
                PrimaryFormat = "date.week-range",  // Fixed: Use "date.week-range" for full week format
                SecondaryUnit = TimelineHeaderUnit.Day,
                SecondaryFormat = "date.day-number",
                ShowPrimary = true,
                ShowSecondary = true,
                MinPrimaryWidth = 210,  // Optimized for 30px days (7 days = 210px)
                MinSecondaryWidth = 30  // Perfect 30px day cells
            },

            TimelineZoomLevel.WeekDayOptimal40px => new TimelineHeaderConfiguration
            {
                PrimaryUnit = TimelineHeaderUnit.Week,
                PrimaryFormat = "date.week-range",  // Fixed: Use "date.week-range" for full week format
                SecondaryUnit = TimelineHeaderUnit.Day,
                SecondaryFormat = "date.day-number",
                ShowPrimary = true,
                ShowSecondary = true,
                MinPrimaryWidth = 280,  // Optimized for 40px days (7 days = 280px)
                MinSecondaryWidth = 40  // Perfect 40px day cells
            },

            TimelineZoomLevel.WeekDayOptimal50px => new TimelineHeaderConfiguration
            {
                PrimaryUnit = TimelineHeaderUnit.Week,
                PrimaryFormat = "date.week-range",  // Fixed: Use "date.week-range" for full week format
                SecondaryUnit = TimelineHeaderUnit.Day,
                SecondaryFormat = "date.day-number",
                ShowPrimary = true,
                ShowSecondary = true,
                MinPrimaryWidth = 350,  // Optimized for 50px days (7 days = 350px)
                MinSecondaryWidth = 50  // Perfect 50px day cells
            },

            TimelineZoomLevel.WeekDayOptimal60px => new TimelineHeaderConfiguration
            {
                PrimaryUnit = TimelineHeaderUnit.Week,
                PrimaryFormat = "date.week-range",  // Fixed: Use "date.week-range" like template
                SecondaryUnit = TimelineHeaderUnit.Day,
                SecondaryFormat = "date.day-number",
                ShowPrimary = true,
                ShowSecondary = true,
                MinPrimaryWidth = 420,  // Optimized for 60px days (7 days = 420px)
                MinSecondaryWidth = 60  // Perfect 60px day cells
            },

            _ => throw new ArgumentOutOfRangeException(nameof(zoomLevel))
        };
    }
#pragma warning restore CS0618 // Type or member is obsolete

    /// <summary>
    /// Determines if headers should be collapsed based on available space.
    /// </summary>
    /// <param name="config">Header configuration</param>
    /// <param name="effectiveDayWidth">Current effective day width</param>
    /// <param name="timeSpanDays">Number of days in the timeline</param>
    /// <returns>True if headers should be collapsed to single row</returns>
    public static bool ShouldCollapseHeaders(TimelineHeaderConfiguration config, double effectiveDayWidth, int timeSpanDays)
    {
        var totalWidth = timeSpanDays * effectiveDayWidth;

        // If primary units would be too cramped, collapse to secondary only
        var estimatedPrimaryUnits = GetEstimatedPrimaryUnits(config.PrimaryUnit, timeSpanDays);
        var averagePrimaryWidth = totalWidth / estimatedPrimaryUnits;

        return averagePrimaryWidth < config.MinPrimaryWidth;
    }

    /// <summary>
    /// Gets the appropriate date increment for iterating through the timeline.
    /// </summary>
    /// <param name="unit">Timeline unit</param>
    /// <returns>Date increment function</returns>
    public static Func<DateTime, DateTime> GetDateIncrement(TimelineHeaderUnit unit)
    {
        return unit switch
        {
            TimelineHeaderUnit.Day => date => date.AddDays(1),
            TimelineHeaderUnit.Week => date => date.AddDays(7),
            TimelineHeaderUnit.Month => date => date.AddMonths(1),
            TimelineHeaderUnit.Quarter => date => date.AddMonths(3),
            TimelineHeaderUnit.Year => date => date.AddYears(1),
            TimelineHeaderUnit.Decade => date => date.AddYears(10),
            _ => throw new ArgumentOutOfRangeException(nameof(unit))
        };
    }

    /// <summary>
    /// Gets the start of period for a given date and unit.
    /// </summary>
    /// <param name="date">Input date</param>
    /// <param name="unit">Timeline unit</param>
    /// <param name="logger">Optional logger for debugging (can be null)</param>
    /// <returns>Start of the period containing the date</returns>
    public static DateTime GetPeriodStart(DateTime date, TimelineHeaderUnit unit, IUniversalLogger? logger = null)
    {
        var result = unit switch
        {
            TimelineHeaderUnit.Day => date.Date,
            TimelineHeaderUnit.Week => GetMondayWeekStart(date, logger), // Start of week (Monday)
            TimelineHeaderUnit.Month => new DateTime(date.Year, date.Month, 1),
            TimelineHeaderUnit.Quarter => GetQuarterStart(date),
            TimelineHeaderUnit.Year => new DateTime(date.Year, 1, 1),
            TimelineHeaderUnit.Decade => new DateTime((date.Year / 10) * 10, 1, 1),
            _ => throw new ArgumentOutOfRangeException(nameof(unit))
        };

        return result;
    }

    /// <summary>
    /// Calculates the width in pixels for a time period at a given zoom level.
    /// </summary>
    /// <param name="periodStart">Start of the period</param>
    /// <param name="unit">Timeline unit</param>
    /// <param name="effectiveDayWidth">Current effective day width</param>
    /// <returns>Width in pixels for the period</returns>
    public static double GetPeriodWidth(DateTime periodStart, TimelineHeaderUnit unit, double effectiveDayWidth)
    {
        var periodEnd = GetDateIncrement(unit)(periodStart);
        var days = (periodEnd - periodStart).Days;
        return days * effectiveDayWidth;
    }

    private static DateTime GetQuarterStart(DateTime date)
    {
        var quarterStartMonth = ((date.Month - 1) / 3) * 3 + 1;
        return new DateTime(date.Year, quarterStartMonth, 1);
    }

    private static DateTime GetMondayWeekStart(DateTime date, IUniversalLogger? logger = null)
    {
        // Calculate days to subtract to get to Monday
        // DayOfWeek: Sunday=0, Monday=1, Tuesday=2, ..., Saturday=6
        int daysFromMonday = ((int)date.DayOfWeek + 6) % 7; // Convert to Monday-based week
        var result = date.Date.AddDays(-daysFromMonday);

        return result;
    }
    private static double GetEstimatedPrimaryUnits(TimelineHeaderUnit unit, int timeSpanDays)
    {
        return unit switch
        {
            TimelineHeaderUnit.Week => timeSpanDays / 7.0,
            TimelineHeaderUnit.Month => timeSpanDays / 30.0,
            TimelineHeaderUnit.Quarter => timeSpanDays / 90.0,
            TimelineHeaderUnit.Year => timeSpanDays / 365.0,
            TimelineHeaderUnit.Decade => timeSpanDays / 3650.0,
            _ => timeSpanDays
        };
    }

    /// <summary>
    /// Gets the minimum primary header width for a zoom level.
    /// Based on typical text content for each level.
    /// </summary>
#pragma warning disable CS0618 // Type or member is obsolete - Legacy zoom levels preserved for backward compatibility
    private static double GetMinimumPrimaryWidth(TimelineZoomLevel zoomLevel)
    {
        return zoomLevel switch
        {
            // OPTIMAL ZOOM LEVELS - Cell-size-first approach

            // Year-Quarter Optimal: Calculated based on target quarter cell sizes
            TimelineZoomLevel.YearQuarterOptimal30px => 120,  // 4 quarters × 30px = 120px
            TimelineZoomLevel.YearQuarterOptimal40px => 160,  // 4 quarters × 40px = 160px  
            TimelineZoomLevel.YearQuarterOptimal50px => 200,  // 4 quarters × 50px = 200px
            TimelineZoomLevel.YearQuarterOptimal70px => 280,  // 4 quarters × 70px = 280px

            // Quarter-Month Optimal: Calculated based on target month cell sizes
            TimelineZoomLevel.QuarterMonthOptimal30px => 90,   // 3 months × 30px = 90px
            TimelineZoomLevel.QuarterMonthOptimal40px => 120,  // 3 months × 40px = 120px
            TimelineZoomLevel.QuarterMonthOptimal50px => 150,  // 3 months × 50px = 150px
            TimelineZoomLevel.QuarterMonthOptimal60px => 180,  // 3 months × 60px = 180px

            // Month-Week Optimal: Calculated based on target week cell sizes
            TimelineZoomLevel.MonthWeekOptimal30px => 120,  // ~4 weeks × 30px = 120px
            TimelineZoomLevel.MonthWeekOptimal40px => 160,  // ~4 weeks × 40px = 160px
            TimelineZoomLevel.MonthWeekOptimal50px => 200,  // ~4 weeks × 50px = 200px
            TimelineZoomLevel.MonthWeekOptimal60px => 240,  // ~4 weeks × 60px = 240px

            // Week-Day Optimal: Calculated based on target day cell sizes
            TimelineZoomLevel.WeekDayOptimal30px => 210,  // 7 days × 30px = 210px
            TimelineZoomLevel.WeekDayOptimal40px => 280,  // 7 days × 40px = 280px
            TimelineZoomLevel.WeekDayOptimal50px => 350,  // 7 days × 50px = 350px
            TimelineZoomLevel.WeekDayOptimal60px => 420,  // 7 days × 60px = 420px

            _ => 60 // Default fallback
        };
    }
#pragma warning restore CS0618 // Type or member is obsolete

    /// <summary>
    /// Gets the minimum secondary header width for a zoom level.
    /// Based on typical text content for each level.
    /// </summary>
#pragma warning disable CS0618 // Type or member is obsolete - Legacy zoom levels preserved for backward compatibility
    private static double GetMinimumSecondaryWidth(TimelineZoomLevel zoomLevel)
    {
        return zoomLevel switch
        {
            // OPTIMAL ZOOM LEVELS - Perfect cell size approach

            // Year-Quarter Optimal: Quarter cells (perfect target sizes)
            TimelineZoomLevel.YearQuarterOptimal30px => 30,  // Perfect 30px quarter cells
            TimelineZoomLevel.YearQuarterOptimal40px => 40,  // Perfect 40px quarter cells
            TimelineZoomLevel.YearQuarterOptimal50px => 50,  // Perfect 50px quarter cells
            TimelineZoomLevel.YearQuarterOptimal70px => 70,  // Perfect 70px quarter cells

            // Quarter-Month Optimal: Month cells (perfect target sizes)
            TimelineZoomLevel.QuarterMonthOptimal30px => 30,  // Perfect 30px month cells
            TimelineZoomLevel.QuarterMonthOptimal40px => 40,  // Perfect 40px month cells
            TimelineZoomLevel.QuarterMonthOptimal50px => 50,  // Perfect 50px month cells
            TimelineZoomLevel.QuarterMonthOptimal60px => 60,  // Perfect 60px month cells

            // Month-Week Optimal: Week cells (perfect target sizes)
            TimelineZoomLevel.MonthWeekOptimal30px => 30,  // Perfect 30px week cells
            TimelineZoomLevel.MonthWeekOptimal40px => 40,  // Perfect 40px week cells
            TimelineZoomLevel.MonthWeekOptimal50px => 50,  // Perfect 50px week cells
            TimelineZoomLevel.MonthWeekOptimal60px => 60,  // Perfect 60px week cells

            // Week-Day Optimal: Day cells (perfect target sizes)
            TimelineZoomLevel.WeekDayOptimal30px => 30,  // Perfect 30px day cells
            TimelineZoomLevel.WeekDayOptimal40px => 40,  // Perfect 40px day cells
            TimelineZoomLevel.WeekDayOptimal50px => 50,  // Perfect 50px day cells
            TimelineZoomLevel.WeekDayOptimal60px => 60,  // Perfect 60px day cells

            _ => 20 // Default fallback
        };
    }
#pragma warning restore CS0618 // Type or member is obsolete
}

/// <summary>
/// Configuration for timeline header display at different zoom levels.
/// </summary>
public class TimelineHeaderConfiguration
{
    public TimelineHeaderUnit PrimaryUnit { get; set; }
    public string PrimaryFormat { get; set; } = string.Empty;
    public TimelineHeaderUnit SecondaryUnit { get; set; }
    public string SecondaryFormat { get; set; } = string.Empty;
    public bool ShowPrimary { get; set; }
    public bool ShowSecondary { get; set; }
    public double MinPrimaryWidth { get; set; }
    public double MinSecondaryWidth { get; set; }
}

/// <summary>
/// Timeline header units for different zoom levels.
/// Now includes Hour unit for ultra-wide zoom levels following Syncfusion patterns.
/// </summary>
public enum TimelineHeaderUnit
{
    Hour,
    Day,
    Week,
    Month,
    Quarter,
    Year,
    Decade
}
