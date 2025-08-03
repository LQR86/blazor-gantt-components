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
    /// Predefined header templates for all 11 zoom levels.
    /// Each template is optimized for its specific day width and use case.
    /// Based on GanttResources.resx specifications with integral pixel widths.
    /// </summary>
    private static readonly Dictionary<TimelineZoomLevel, TimelineHeaderTemplate> _templates =
        new Dictionary<TimelineZoomLevel, TimelineHeaderTemplate>
        {
            // Week→Day Pattern Levels (Week → Day) - 97px, 68px
            [TimelineZoomLevel.WeekDay97px] = TimelineHeaderTemplate.Create(
                TimelineHeaderUnit.Week,
                "date.week-range",          // "Feb 17-23, 2025" - top tier: week range with year
                TimelineHeaderUnit.Day,
                "date.day-name-short",      // "Mon", "Tue", "Wed" - bottom tier: day abbreviations
                "Daily sprint planning with weekly context"
            ),

            [TimelineZoomLevel.WeekDay68px] = TimelineHeaderTemplate.Create(
                TimelineHeaderUnit.Week,
                "date.week-range",          // "Feb 17-23, 2025" - top tier: week range with year
                TimelineHeaderUnit.Day,
                "date.day-number",          // "17", "18", "19" - bottom tier: day numbers
                "Weekly view with daily granularity"
            ),

            // Month→Day Pattern Levels (Month → Day) - 48px, 34px
            [TimelineZoomLevel.MonthDay48px] = TimelineHeaderTemplate.Create(
                TimelineHeaderUnit.Month,
                "date.month-year",          // "February 2025" - top tier: month with year
                TimelineHeaderUnit.Day,
                "date.day-number",          // "17", "18", "19", "20", "21" - bottom tier: day numbers
                "Monthly overview with daily breakdown"
            ),

            [TimelineZoomLevel.MonthDay34px] = TimelineHeaderTemplate.Create(
                TimelineHeaderUnit.Month,
                "date.month-abbrev",        // "Feb", "Mar" - top tier: month abbreviations
                TimelineHeaderUnit.Day,
                "date.day-number",          // "17", "18", "19" - bottom tier: day numbers
                "Compact monthly view with daily tracking"
            ),

            // Quarter→Month Pattern Levels (Quarter → Month) - 24px, 17px
            [TimelineZoomLevel.QuarterMonth24px] = TimelineHeaderTemplate.Create(
                TimelineHeaderUnit.Quarter,
                "date.quarter-year",        // "Q1 2025" - top tier: quarter with year
                TimelineHeaderUnit.Month,
                "date.month-abbrev",        // "Jan", "Feb", "Mar" - bottom tier: month abbreviations
                "Quarterly overview with monthly breakdown"
            ),

            [TimelineZoomLevel.QuarterMonth17px] = TimelineHeaderTemplate.Create(
                TimelineHeaderUnit.Quarter,
                "date.quarter-short",       // "Q1", "Q2" - top tier: quarter abbreviations  
                TimelineHeaderUnit.Month,
                "date.month-abbrev",        // "Jan", "Feb", "Mar" - bottom tier: month abbreviations
                "Compact quarterly view with monthly periods"
            ),

            // Month-only Pattern Levels (Month-only) - 12px, 8px
            [TimelineZoomLevel.Month12px] = TimelineHeaderTemplate.Create(
                TimelineHeaderUnit.Year,
                "date.year",                // "2025" - top tier: full years
                TimelineHeaderUnit.Month,
                "date.month-abbrev",        // "Jan", "Feb", "Mar", "Apr", "May" - bottom tier: month abbreviations
                "Annual overview with monthly breakdown"
            ),

            [TimelineZoomLevel.Month8px] = TimelineHeaderTemplate.Create(
                TimelineHeaderUnit.Year,
                "date.year",                // "2025" - top tier: full years
                TimelineHeaderUnit.Month,
                "date.month-abbrev",        // "Jan", "Feb", "Mar", "Apr", "May" - bottom tier: month abbreviations
                "Compact annual view with monthly periods"
            ),

            // Year→Quarter Pattern Levels (Year → Quarter) - 6px, 4px, 3px
            [TimelineZoomLevel.YearQuarter6px] = TimelineHeaderTemplate.Create(
                TimelineHeaderUnit.Year,
                "date.year",                // "2025", "2026" - top tier: full years
                TimelineHeaderUnit.Quarter,
                "date.quarter-short",       // "Q1", "Q2", "Q3", "Q4" - bottom tier: quarter abbreviations
                "Multi-year overview with quarterly breakdown"
            ),

            [TimelineZoomLevel.YearQuarter4px] = TimelineHeaderTemplate.Create(
                TimelineHeaderUnit.Year,
                "date.year",                // "2025", "2026" - top tier: full years
                TimelineHeaderUnit.Quarter,
                "date.quarter-minimal",     // "1", "2", "3", "4" - bottom tier: quarter numbers
                "Extended multi-year view with quarterly periods"
            ),

            [TimelineZoomLevel.YearQuarter3px] = TimelineHeaderTemplate.Create(
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
