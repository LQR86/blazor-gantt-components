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

        // Debug logging
        logger?.LogDebugInfo($"TEMPLATE DEBUG: ZoomLevel: {zoomLevel}, PrimaryUnit: {template.PrimaryUnit}, SecondaryUnit: {template.SecondaryUnit}");
        logger?.LogDebugInfo($"TEMPLATE FORMATS: Primary: {template.PrimaryFormat}, Secondary: {template.SecondaryFormat}");

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
    public static TimelineHeaderConfiguration GetHeaderConfiguration(TimelineZoomLevel zoomLevel, double effectiveDayWidth)
    {
        return zoomLevel switch
        {
            // Week→Day patterns (levels 10-11: 68px, 97px)
            TimelineZoomLevel.WeekDay68px => new TimelineHeaderConfiguration
            {
                PrimaryUnit = TimelineHeaderUnit.Month,
                PrimaryFormat = "date.month-year",
                SecondaryUnit = TimelineHeaderUnit.Day,
                SecondaryFormat = "date.day-number",
                ShowPrimary = true,
                ShowSecondary = true,
                MinPrimaryWidth = 120, // Months need space for "January 2025"
                MinSecondaryWidth = 20  // Days just show numbers
            },

            TimelineZoomLevel.WeekDay97px => new TimelineHeaderConfiguration
            {
                PrimaryUnit = TimelineHeaderUnit.Month,
                PrimaryFormat = "date.month-year",
                SecondaryUnit = TimelineHeaderUnit.Day,
                SecondaryFormat = "date.day-number",
                ShowPrimary = true,
                ShowSecondary = true,
                MinPrimaryWidth = 120, // Same as WeekDay68px
                MinSecondaryWidth = 20
            },

            // Month→Day patterns (levels 8-9: 34px, 48px)
            TimelineZoomLevel.MonthDay34px => new TimelineHeaderConfiguration
            {
                PrimaryUnit = TimelineHeaderUnit.Month,
                PrimaryFormat = "date.month-year",
                SecondaryUnit = TimelineHeaderUnit.Day,
                SecondaryFormat = "date.day-number",
                ShowPrimary = true,
                ShowSecondary = true,
                MinPrimaryWidth = 100,
                MinSecondaryWidth = 15
            },

            TimelineZoomLevel.MonthDay48px => new TimelineHeaderConfiguration
            {
                PrimaryUnit = TimelineHeaderUnit.Month,
                PrimaryFormat = "date.month-year",
                SecondaryUnit = TimelineHeaderUnit.Day,
                SecondaryFormat = "date.day-number",
                ShowPrimary = true,
                ShowSecondary = true,
                MinPrimaryWidth = 100, // Same as MonthDay34px
                MinSecondaryWidth = 15
            },

            // Quarter→Month patterns (levels 6-7: 17px, 24px)
            TimelineZoomLevel.QuarterMonth17px => new TimelineHeaderConfiguration
            {
                PrimaryUnit = TimelineHeaderUnit.Quarter,
                PrimaryFormat = "date.quarter-year",
                SecondaryUnit = TimelineHeaderUnit.Month,
                SecondaryFormat = "date.month-short",
                ShowPrimary = true,
                ShowSecondary = true,
                MinPrimaryWidth = 180,
                MinSecondaryWidth = 45
            },

            TimelineZoomLevel.QuarterMonth24px => new TimelineHeaderConfiguration
            {
                PrimaryUnit = TimelineHeaderUnit.Quarter,
                PrimaryFormat = "date.quarter-year",
                SecondaryUnit = TimelineHeaderUnit.Month,
                SecondaryFormat = "date.month-short",
                ShowPrimary = true,
                ShowSecondary = true,
                MinPrimaryWidth = 180, // Same as QuarterMonth17px
                MinSecondaryWidth = 45
            },

            // Month-only patterns (levels 4-5: 8px, 12px)
            TimelineZoomLevel.Month8px => new TimelineHeaderConfiguration
            {
                PrimaryUnit = TimelineHeaderUnit.Year,
                PrimaryFormat = "date.year",
                SecondaryUnit = TimelineHeaderUnit.Month,
                SecondaryFormat = "date.month-short",
                ShowPrimary = true,
                ShowSecondary = true,
                MinPrimaryWidth = 150,
                MinSecondaryWidth = 25
            },

            TimelineZoomLevel.Month12px => new TimelineHeaderConfiguration
            {
                PrimaryUnit = TimelineHeaderUnit.Year,
                PrimaryFormat = "date.year",
                SecondaryUnit = TimelineHeaderUnit.Month,
                SecondaryFormat = "date.month-short",
                ShowPrimary = true,
                ShowSecondary = true,
                MinPrimaryWidth = 150, // Same as Month8px
                MinSecondaryWidth = 25
            },

            // Year→Quarter patterns (levels 1-3: 3px, 4px, 6px)
            TimelineZoomLevel.YearQuarter3px => new TimelineHeaderConfiguration
            {
                PrimaryUnit = TimelineHeaderUnit.Year,
                PrimaryFormat = "date.year",
                SecondaryUnit = TimelineHeaderUnit.Quarter,
                SecondaryFormat = "date.quarter-short",
                ShowPrimary = true,
                ShowSecondary = true,
                MinPrimaryWidth = 200, // Years need space
                MinSecondaryWidth = 30  // "Q1", "Q2", etc.
            },

            TimelineZoomLevel.YearQuarter4px => new TimelineHeaderConfiguration
            {
                PrimaryUnit = TimelineHeaderUnit.Year,
                PrimaryFormat = "date.year",
                SecondaryUnit = TimelineHeaderUnit.Quarter,
                SecondaryFormat = "date.quarter-short",
                ShowPrimary = true,
                ShowSecondary = true,
                MinPrimaryWidth = 200, // Same as YearQuarter3px
                MinSecondaryWidth = 30
            },

            TimelineZoomLevel.YearQuarter6px => new TimelineHeaderConfiguration
            {
                PrimaryUnit = TimelineHeaderUnit.Year,
                PrimaryFormat = "date.year",
                SecondaryUnit = TimelineHeaderUnit.Quarter,
                SecondaryFormat = "date.quarter-short",
                ShowPrimary = true,
                ShowSecondary = true,
                MinPrimaryWidth = 200, // Same as YearQuarter3px
                MinSecondaryWidth = 30
            },

            _ => throw new ArgumentOutOfRangeException(nameof(zoomLevel))
        };
    }

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

        // Debug logging for week calculations
        if (unit == TimelineHeaderUnit.Week)
        {
            logger?.LogDebugInfo($"GetPeriodStart DEBUG: Input date: {date:yyyy-MM-dd (ddd)}, Week start: {result:yyyy-MM-dd (ddd)}");
        }

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

        logger?.LogDebugInfo($"GetMondayWeekStart DEBUG: Input: {date:yyyy-MM-dd (ddd)}, DaysFromMonday: {daysFromMonday}, Result: {result:yyyy-MM-dd (ddd)}");

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
    private static double GetMinimumPrimaryWidth(TimelineZoomLevel zoomLevel)
    {
        return zoomLevel switch
        {
            // Week→Day patterns (levels 10-11: 68px, 97px) need space for verbose formats
            TimelineZoomLevel.WeekDay68px => 120,           // "January 2025"
            TimelineZoomLevel.WeekDay97px => 100,     // "Jan 2025"

            // Month→Day patterns (levels 8-9: 34px, 48px) use standard formats
            TimelineZoomLevel.MonthDay34px => 60,           // "Jan"
            TimelineZoomLevel.MonthDay48px => 40,     // "Q1"

            // Quarter→Month patterns (levels 6-7: 17px, 24px) use quarterly/yearly formats
            TimelineZoomLevel.QuarterMonth17px => 80,          // "Q1 2025"
            TimelineZoomLevel.QuarterMonth24px => 40,    // "Q1"

            // Month-only patterns (levels 4-5: 8px, 12px) use yearly formats
            TimelineZoomLevel.Month8px => 60,       // "2025"
            TimelineZoomLevel.Month12px => 60,        // "2025"

            // Year→Quarter patterns (levels 1-3: 3px, 4px, 6px) use yearly/decade formats
            TimelineZoomLevel.YearQuarter3px => 100, // "2020-2029"
            TimelineZoomLevel.YearQuarter4px => 60,       // "20s"
            TimelineZoomLevel.YearQuarter6px => 40,        // "20", "30" (minimal)

            _ => 60 // Default fallback
        };
    }

    /// <summary>
    /// Gets the minimum secondary header width for a zoom level.
    /// Based on typical text content for each level.
    /// </summary>
    private static double GetMinimumSecondaryWidth(TimelineZoomLevel zoomLevel)
    {
        return zoomLevel switch
        {
            // Week→Day patterns (levels 10-11: 68px, 97px) - Day-level secondary headers
            TimelineZoomLevel.WeekDay68px => 20,            // "1", "2", "3"
            TimelineZoomLevel.WeekDay97px => 20,      // "1", "2", "3"

            // Month→Day patterns (levels 8-9: 34px, 48px) - Day-level secondary headers
            TimelineZoomLevel.MonthDay34px => 20,         // "1", "2", "3"
            TimelineZoomLevel.MonthDay48px => 20,           // "1", "2", "3"

            // Quarter→Month patterns (levels 6-7: 17px, 24px) - Month-level secondary headers
            TimelineZoomLevel.QuarterMonth17px => 25,     // "M", "T", "W" (day abbrev)
            TimelineZoomLevel.QuarterMonth24px => 25,          // "J", "F", "M"

            // Month-only patterns (levels 4-5: 8px, 12px) - Month-level secondary headers
            TimelineZoomLevel.Month8px => 25,    // "J", "F", "M"
            TimelineZoomLevel.Month12px => 15,       // "1", "2", "3", "4"

            // Year→Quarter patterns (levels 1-3: 3px, 4px, 6px) - Quarter/year-level secondary headers
            TimelineZoomLevel.YearQuarter3px => 15,        // "1", "2", "3", "4"
            TimelineZoomLevel.YearQuarter4px => 25,  // "25", "26", "27"
            TimelineZoomLevel.YearQuarter6px => 25,       // "25", "26", "27"

            _ => 20 // Default fallback
        };
    }
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
