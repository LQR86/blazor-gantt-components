using GanttComponents.Models;

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
    /// <returns>Header configuration based on preset template</returns>
    public static TimelineHeaderConfiguration GetHeaderConfigurationFromTemplate(TimelineZoomLevel zoomLevel)
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
    public static TimelineHeaderConfiguration GetHeaderConfiguration(TimelineZoomLevel zoomLevel, double effectiveDayWidth)
    {
        return zoomLevel switch
        {
            TimelineZoomLevel.WeekDay => new TimelineHeaderConfiguration
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

            TimelineZoomLevel.WeekDayMedium => new TimelineHeaderConfiguration
            {
                PrimaryUnit = TimelineHeaderUnit.Month,
                PrimaryFormat = "date.month-year",
                SecondaryUnit = TimelineHeaderUnit.Day,
                SecondaryFormat = "date.day-number",
                ShowPrimary = true,
                ShowSecondary = true,
                MinPrimaryWidth = 120, // Same as WeekDay
                MinSecondaryWidth = 20
            },

            TimelineZoomLevel.WeekDayLow => new TimelineHeaderConfiguration
            {
                PrimaryUnit = TimelineHeaderUnit.Month,
                PrimaryFormat = "date.month-year",
                SecondaryUnit = TimelineHeaderUnit.Day,
                SecondaryFormat = "date.day-number",
                ShowPrimary = true,
                ShowSecondary = true,
                MinPrimaryWidth = 120, // Same as WeekDay
                MinSecondaryWidth = 20
            },

            TimelineZoomLevel.MonthDay => new TimelineHeaderConfiguration
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

            TimelineZoomLevel.MonthDayMedium => new TimelineHeaderConfiguration
            {
                PrimaryUnit = TimelineHeaderUnit.Month,
                PrimaryFormat = "date.month-year",
                SecondaryUnit = TimelineHeaderUnit.Day,
                SecondaryFormat = "date.day-number",
                ShowPrimary = true,
                ShowSecondary = true,
                MinPrimaryWidth = 100, // Same as MonthDay
                MinSecondaryWidth = 15
            },

            TimelineZoomLevel.MonthWeek => new TimelineHeaderConfiguration
            {
                PrimaryUnit = TimelineHeaderUnit.Quarter,
                PrimaryFormat = "date.quarter-year",
                SecondaryUnit = TimelineHeaderUnit.Month,
                SecondaryFormat = "date.month-short",
                ShowPrimary = true,
                ShowSecondary = true,
                MinPrimaryWidth = 180, // Quarters need space for "Q1 2025"
                MinSecondaryWidth = 45  // Abbreviated months "Jan"
            },

            TimelineZoomLevel.MonthWeekMedium => new TimelineHeaderConfiguration
            {
                PrimaryUnit = TimelineHeaderUnit.Quarter,
                PrimaryFormat = "date.quarter-year",
                SecondaryUnit = TimelineHeaderUnit.Month,
                SecondaryFormat = "date.month-short",
                ShowPrimary = true,
                ShowSecondary = true,
                MinPrimaryWidth = 180, // Same as MonthWeek
                MinSecondaryWidth = 45
            },

            TimelineZoomLevel.MonthWeekLow => new TimelineHeaderConfiguration
            {
                PrimaryUnit = TimelineHeaderUnit.Quarter,
                PrimaryFormat = "date.quarter-year",
                SecondaryUnit = TimelineHeaderUnit.Month,
                SecondaryFormat = "date.month-short",
                ShowPrimary = true,
                ShowSecondary = true,
                MinPrimaryWidth = 180, // Same as MonthWeek
                MinSecondaryWidth = 45
            },

            TimelineZoomLevel.QuarterWeek => new TimelineHeaderConfiguration
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

            TimelineZoomLevel.QuarterWeekMedium => new TimelineHeaderConfiguration
            {
                PrimaryUnit = TimelineHeaderUnit.Year,
                PrimaryFormat = "date.year",
                SecondaryUnit = TimelineHeaderUnit.Quarter,
                SecondaryFormat = "date.quarter-short",
                ShowPrimary = true,
                ShowSecondary = true,
                MinPrimaryWidth = 200, // Same as QuarterWeek
                MinSecondaryWidth = 30
            },

            TimelineZoomLevel.QuarterMonth => new TimelineHeaderConfiguration
            {
                PrimaryUnit = TimelineHeaderUnit.Year,
                PrimaryFormat = "date.year",
                SecondaryUnit = TimelineHeaderUnit.Quarter,
                SecondaryFormat = "date.quarter-short",
                ShowPrimary = true,
                ShowSecondary = true,
                MinPrimaryWidth = 150,
                MinSecondaryWidth = 25
            },

            TimelineZoomLevel.QuarterMonthMedium => new TimelineHeaderConfiguration
            {
                PrimaryUnit = TimelineHeaderUnit.Year,
                PrimaryFormat = "date.year",
                SecondaryUnit = TimelineHeaderUnit.Quarter,
                SecondaryFormat = "date.quarter-short",
                ShowPrimary = true,
                ShowSecondary = true,
                MinPrimaryWidth = 150, // Same as QuarterMonth
                MinSecondaryWidth = 25
            },

            TimelineZoomLevel.YearQuarter => new TimelineHeaderConfiguration
            {
                PrimaryUnit = TimelineHeaderUnit.Decade,
                PrimaryFormat = "date.decade",
                SecondaryUnit = TimelineHeaderUnit.Year,
                SecondaryFormat = "date.year",
                ShowPrimary = true,
                ShowSecondary = true,
                MinPrimaryWidth = 300, // "2020-2029"
                MinSecondaryWidth = 40   // "2025"
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
    /// <returns>Start of the period containing the date</returns>
    public static DateTime GetPeriodStart(DateTime date, TimelineHeaderUnit unit)
    {
        return unit switch
        {
            TimelineHeaderUnit.Day => date.Date,
            TimelineHeaderUnit.Week => date.Date.AddDays(-(int)date.DayOfWeek), // Start of week (Sunday)
            TimelineHeaderUnit.Month => new DateTime(date.Year, date.Month, 1),
            TimelineHeaderUnit.Quarter => GetQuarterStart(date),
            TimelineHeaderUnit.Year => new DateTime(date.Year, 1, 1),
            TimelineHeaderUnit.Decade => new DateTime((date.Year / 10) * 10, 1, 1),
            _ => throw new ArgumentOutOfRangeException(nameof(unit))
        };
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

    private static double GetEstimatedPrimaryUnits(TimelineHeaderUnit unit, int timeSpanDays)
    {
        return unit switch
        {
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
            // Ultra-wide levels need space for verbose formats
            TimelineZoomLevel.WeekDay => 120,           // "January 2025"
            TimelineZoomLevel.WeekDayMedium => 100,     // "Jan 2025"
            TimelineZoomLevel.WeekDayLow => 60,         // "Jan"

            // Wide levels use standard formats
            TimelineZoomLevel.MonthDay => 60,           // "Jan"
            TimelineZoomLevel.MonthDayMedium => 40,     // "Q1"

            // Medium levels use quarterly/yearly formats
            TimelineZoomLevel.MonthWeek => 80,          // "Q1 2025"
            TimelineZoomLevel.MonthWeekMedium => 40,    // "Q1"
            TimelineZoomLevel.MonthWeekLow => 60,       // "2025"

            // Narrow levels use yearly/decade formats
            TimelineZoomLevel.QuarterWeek => 60,        // "2025"
            TimelineZoomLevel.QuarterWeekMedium => 100, // "2020-2029"
            TimelineZoomLevel.QuarterMonth => 60,       // "20s"
            TimelineZoomLevel.QuarterMonthMedium => 60, // "20s"

            // Ultra-narrow level
            TimelineZoomLevel.YearQuarter => 40,        // "20", "30" (minimal)

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
            // Day-level secondary headers
            TimelineZoomLevel.WeekDay => 20,            // "1", "2", "3"
            TimelineZoomLevel.WeekDayMedium => 20,      // "1", "2", "3"
            TimelineZoomLevel.WeekDayLow => 20,         // "1", "2", "3"
            TimelineZoomLevel.MonthDay => 20,           // "1", "2", "3"
            TimelineZoomLevel.MonthDayMedium => 25,     // "M", "T", "W" (day abbrev)

            // Month-level secondary headers
            TimelineZoomLevel.MonthWeek => 25,          // "J", "F", "M"
            TimelineZoomLevel.MonthWeekMedium => 25,    // "J", "F", "M"
            TimelineZoomLevel.MonthWeekLow => 15,       // "1", "2", "3", "4"

            // Quarter/year-level secondary headers
            TimelineZoomLevel.QuarterWeek => 15,        // "1", "2", "3", "4"
            TimelineZoomLevel.QuarterWeekMedium => 25,  // "25", "26", "27"
            TimelineZoomLevel.QuarterMonth => 25,       // "25", "26", "27"
            TimelineZoomLevel.QuarterMonthMedium => 15, // "5", "6", "7"

            // Minimal secondary headers
            TimelineZoomLevel.YearQuarter => 15,        // "5", "6", "7"

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
/// </summary>
public enum TimelineHeaderUnit
{
    Day,
    Week,
    Month,
    Quarter,
    Year,
    Decade
}
