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
    /// </summary>
    private static readonly Dictionary<TimelineZoomLevel, TimelineHeaderTemplate> _templates =
        new Dictionary<TimelineZoomLevel, TimelineHeaderTemplate>
        {
            // Ultra-wide zoom levels (96px-56px) - Week/Day patterns (true Syncfusion alignment)
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
                "date.week-start-day",      // "15 Feb" - top tier: week start day with month
                TimelineHeaderUnit.Day,
                "date.day-number",          // "1", "2", "3" - bottom tier: day numbers (wider)
                "Compact weekly view with daily tracking"
            ),

            // Wide zoom levels (40px-32px) - Month/Week patterns (true Syncfusion alignment)
            [TimelineZoomLevel.MonthDay] = TimelineHeaderTemplate.Create(
                TimelineHeaderUnit.Month,
                "date.month-year",          // "Feb, 2025" - top tier: month with year
                TimelineHeaderUnit.Week,
                "date.week-start-day",      // Week start day - bottom tier
                "Monthly overview with weekly breakdown"
            ),

            [TimelineZoomLevel.MonthDayMedium] = TimelineHeaderTemplate.Create(
                TimelineHeaderUnit.Month,
                "date.month-year",          // "Feb, 2025" - top tier: month with year
                TimelineHeaderUnit.Week,
                "date.day-number",          // Day numbers for week starts - bottom tier
                "Medium monthly view with weekly periods"
            ),

            // Medium zoom levels (24px-16px) - Year/Quarter patterns (true Syncfusion alignment)
            [TimelineZoomLevel.MonthWeek] = TimelineHeaderTemplate.Create(
                TimelineHeaderUnit.Year,
                "date.year",                // "2025" - top tier: year
                TimelineHeaderUnit.Quarter,
                "date.quarter-short",       // "Q1", "Q2", "Q3", "Q4" - bottom tier: quarters (wider)
                "Annual overview with quarterly breakdown"
            ),

            [TimelineZoomLevel.MonthWeekMedium] = TimelineHeaderTemplate.Create(
                TimelineHeaderUnit.Year,
                "date.year",                // "2025" - top tier: year
                TimelineHeaderUnit.Quarter,
                "date.quarter-short",       // "Q1", "Q2", "Q3", "Q4" - bottom tier: quarters
                "Medium annual view with quarterly periods"
            ),

            [TimelineZoomLevel.MonthWeekLow] = TimelineHeaderTemplate.Create(
                TimelineHeaderUnit.Year,
                "date.year",                // "2025" - top tier: year
                TimelineHeaderUnit.Quarter,
                "date.quarter-minimal",     // "1", "2", "3", "4" - bottom tier: quarter numbers
                "Compact annual view with quarterly markers"
            ),

            // Narrow zoom levels (12.8px-8px) - Multi-year patterns (true Syncfusion alignment)
            [TimelineZoomLevel.QuarterWeek] = TimelineHeaderTemplate.Create(
                TimelineHeaderUnit.Decade,
                "date.decade",              // "2020-2029" - top tier: decade range
                TimelineHeaderUnit.Year,
                "date.year",                // "2025", "2026", "2027" - bottom tier: full years
                "Decade overview with annual breakdown"
            ),

            [TimelineZoomLevel.QuarterWeekMedium] = TimelineHeaderTemplate.Create(
                TimelineHeaderUnit.Decade,
                "date.decade",              // "2020-2029" - top tier: decade range
                TimelineHeaderUnit.Year,
                "date.year-short",          // "25", "26", "27" - bottom tier: year abbreviations
                "Multi-year strategic planning view"
            ),

            [TimelineZoomLevel.QuarterMonth] = TimelineHeaderTemplate.Create(
                TimelineHeaderUnit.Decade,
                "date.decade-short",        // "20s" - top tier: decade abbreviation
                TimelineHeaderUnit.Year,
                "date.year-short",          // "25", "26", "27" - bottom tier: year abbreviations
                "Strategic long-term planning"
            ),

            [TimelineZoomLevel.QuarterMonthMedium] = TimelineHeaderTemplate.Create(
                TimelineHeaderUnit.Decade,
                "date.decade-short",        // "20s" - top tier: decade abbreviation
                TimelineHeaderUnit.Year,
                "date.year-minimal",        // "5", "6", "7" (last digit) - bottom tier: minimal year
                "Ultra-compact strategic overview"
            ),

            // Ultra-narrow zoom level (3px) - Decade/Year granularity (Syncfusion aligned)
            [TimelineZoomLevel.YearQuarter] = TimelineHeaderTemplate.Create(
                TimelineHeaderUnit.Decade,
                "date.decade-minimal",      // "20", "30" - top tier: minimal decade display
                TimelineHeaderUnit.Year,
                "date.year-minimal",        // "5", "6", "7" (last digit) - bottom tier: minimal year markers
                "Enterprise portfolio and decade planning",
                showPrimary: false,         // Too cramped for primary header
                showSecondary: true         // Only minimal year markers
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
