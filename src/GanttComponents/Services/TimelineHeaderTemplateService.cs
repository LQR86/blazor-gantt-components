using GanttComponents.Models;

namespace GanttComponents.Services;

/// <summary>
/// Service for managing predefined timeline header templates.
/// Follows the preset-only approach - each zoom level has a fixed header configuration
/// for predictable, testable behavior.
/// 
/// Templates follow TRUE Syncfusion Gantt alignment patterns:
/// - Week/Day: Week start (Feb 15, 2025) → Day names/numbers (ultra-wide zoom levels)
/// - Month/Week: Month-Year (Feb, 2025) → Week start dates (wide zoom levels)  
/// - Year/Quarter: Year (2025) → Quarter markers (Q1, Q2, Q3, Q4) (medium zoom levels)
/// - Decade/Year: Decade range (2020-2029) → Year numbers (narrow zoom levels)
/// This matches the exact progression used in Syncfusion Gantt charts.
/// </summary>
public static class TimelineHeaderTemplateService
{
    /// <summary>
    /// Predefined header templates for all 13 zoom levels.
    /// Each template is optimized for its specific day width and use case.
    /// Based on GanttResources.resx specifications.
    /// </summary>
    private static readonly Dictionary<TimelineZoomLevel, TimelineHeaderTemplate> _templates =
        new Dictionary<TimelineZoomLevel, TimelineHeaderTemplate>
        {
            // WeekDay Pattern Levels (Week → Day) - 60px, 45px, 35px
            [TimelineZoomLevel.WeekDay] = TimelineHeaderTemplate.Create(
                TimelineHeaderUnit.Week,
                "date.week-range",          // "Feb 15, 2025" - top tier: week start with month-year
                TimelineHeaderUnit.Day,
                "date.day-name-short",      // "Mon", "Tue", "Wed" - bottom tier: day abbreviations
                "Daily sprint planning with weekly context"
            ),

            [TimelineZoomLevel.WeekDayMedium] = TimelineHeaderTemplate.Create(
                TimelineHeaderUnit.Week,
                "date.week-range",          // "Feb 15, 2025" - top tier: week start with month-year
                TimelineHeaderUnit.Day,
                "date.day-number",          // "1", "2", "3" - bottom tier: day numbers
                "Medium weekly view with daily granularity"
            ),

            [TimelineZoomLevel.WeekDayLow] = TimelineHeaderTemplate.Create(
                TimelineHeaderUnit.Week,
                "date.week-range",          // "Dec 30, 2024" - top tier: week start with month-year
                TimelineHeaderUnit.Day,
                "date.day-number",          // "1", "2", "3" - bottom tier: day numbers
                "Compact weekly view with daily tracking"
            ),

            // MonthWeek Pattern Levels (Month → Week) - 25px, 20px, 18px
            [TimelineZoomLevel.MonthWeek] = TimelineHeaderTemplate.Create(
                TimelineHeaderUnit.Month,
                "date.month-year",          // "Feb 2025" - top tier: month with year
                TimelineHeaderUnit.Week,
                "date.week-start-day",      // "1 Feb", "8 Feb" - bottom tier: week start dates
                "Monthly overview with weekly breakdown"
            ),

            [TimelineZoomLevel.MonthWeekMedium] = TimelineHeaderTemplate.Create(
                TimelineHeaderUnit.Month,
                "date.month-abbrev",        // "Feb", "Mar" - top tier: month abbreviations
                TimelineHeaderUnit.Week,
                "date.week-start",          // "1", "8", "15" - bottom tier: week start day numbers
                "Medium monthly view with weekly periods"
            ),

            [TimelineZoomLevel.MonthWeekLow] = TimelineHeaderTemplate.Create(
                TimelineHeaderUnit.Month,
                "date.month-abbrev",        // "Feb", "Mar" - top tier: month abbreviations
                TimelineHeaderUnit.Week,
                "date.week-start",          // "1", "8", "15" - bottom tier: week start day numbers
                "Compact monthly view with weekly periods"
            ),

            // QuarterMonth Pattern Levels (Quarter → Month) - 15px, 12px, 10px
            [TimelineZoomLevel.QuarterMonth] = TimelineHeaderTemplate.Create(
                TimelineHeaderUnit.Quarter,
                "date.quarter-year",        // "Q1 2025" - top tier: quarter with year
                TimelineHeaderUnit.Month,
                "date.month-abbrev",        // "Jan", "Feb", "Mar" - bottom tier: month abbreviations
                "Quarterly overview with monthly breakdown"
            ),

            [TimelineZoomLevel.QuarterMonthMedium] = TimelineHeaderTemplate.Create(
                TimelineHeaderUnit.Quarter,
                "date.quarter-short",       // "Q1", "Q2" - top tier: quarter abbreviations  
                TimelineHeaderUnit.Month,
                "date.month-abbrev",        // "Jan", "Feb", "Mar" - bottom tier: month abbreviations
                "Medium quarterly view with monthly periods"
            ),

            [TimelineZoomLevel.QuarterMonthLow] = TimelineHeaderTemplate.Create(
                TimelineHeaderUnit.Quarter,
                "date.quarter-short",       // "Q1", "Q2" - top tier: quarter abbreviations
                TimelineHeaderUnit.Month,
                "date.month-abbrev",        // "Jan", "Feb", "Mar" - bottom tier: month abbreviations
                "Compact quarterly view with monthly markers"
            ),

            // YearQuarter Pattern Levels (Year → Quarter) - 8px, 6.5px, 5px, 3px
            [TimelineZoomLevel.YearQuarter] = TimelineHeaderTemplate.Create(
                TimelineHeaderUnit.Year,
                "date.year",                // "2025", "2026" - top tier: full years
                TimelineHeaderUnit.Quarter,
                "date.quarter-short",       // "Q1", "Q2", "Q3", "Q4" - bottom tier: quarter abbreviations
                "Annual overview with quarterly breakdown"
            ),

            [TimelineZoomLevel.YearQuarterMedium] = TimelineHeaderTemplate.Create(
                TimelineHeaderUnit.Year,
                "date.year",                // "2025", "2026" - top tier: full years
                TimelineHeaderUnit.Quarter,
                "date.quarter-minimal",     // "1", "2", "3", "4" - bottom tier: quarter numbers
                "Medium annual view with quarterly periods"
            ),

            [TimelineZoomLevel.YearQuarterLow] = TimelineHeaderTemplate.Create(
                TimelineHeaderUnit.Year,
                "date.year-short",          // "25", "26" - top tier: year abbreviations
                TimelineHeaderUnit.Quarter,
                "date.quarter-minimal",     // "1", "2", "3", "4" - bottom tier: quarter numbers
                "Extended multi-year planning with quarterly markers"
            ),

            [TimelineZoomLevel.YearQuarterMin] = TimelineHeaderTemplate.Create(
                TimelineHeaderUnit.Year,
                "date.year-short",          // "25", "26" - top tier: year abbreviations
                TimelineHeaderUnit.Quarter,
                "date.quarter-minimal",     // "1", "2", "3", "4" - bottom tier: quarter numbers
                "Long-term strategic view with minimum day width",
                showPrimary: true,          // Keep both headers for minimal visibility
                showSecondary: true
            )
        };

    /// <summary>
    /// Gets the predefined header template for a specific zoom level.
    /// </summary>
    /// <param name="zoomLevel">The timeline zoom level</param>
    /// <returns>Optimized header template for the zoom level</returns>
    public static TimelineHeaderTemplate GetTemplate(TimelineZoomLevel zoomLevel)
    {
        return _templates.TryGetValue(zoomLevel, out var template)
            ? template
            : _templates[TaskDisplayConstants.DEFAULT_ZOOM_LEVEL];
    }

    /// <summary>
    /// Gets all available header templates.
    /// </summary>
    /// <returns>All predefined header templates by zoom level</returns>
    public static IReadOnlyDictionary<TimelineZoomLevel, TimelineHeaderTemplate> GetAllTemplates()
    {
        return _templates;
    }

    /// <summary>
    /// Gets the header template description for a zoom level.
    /// </summary>
    /// <param name="zoomLevel">The timeline zoom level</param>
    /// <returns>Human-readable description of the template's use case</returns>
    public static string GetTemplateDescription(TimelineZoomLevel zoomLevel)
    {
        var template = GetTemplate(zoomLevel);
        return template.Description;
    }

    /// <summary>
    /// Validates that all zoom levels have defined templates.
    /// Used for testing and development verification.
    /// </summary>
    /// <returns>True if all zoom levels are covered</returns>
    public static bool ValidateAllTemplatesDefined()
    {
        var allZoomLevels = Enum.GetValues<TimelineZoomLevel>();
        return allZoomLevels.All(level => _templates.ContainsKey(level));
    }
}
