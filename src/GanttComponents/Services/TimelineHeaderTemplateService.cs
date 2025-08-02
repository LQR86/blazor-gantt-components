using GanttComponents.Models;

namespace GanttComponents.Services;

/// <summary>
/// Service for managing predefined timeline header templates.
/// Follows the preset-only approach - each zoom level has a fixed header configuration
/// for predictable, testable behavior.
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
            // Ultra-wide zoom levels (96px-56px) - Full verbose formats
            [TimelineZoomLevel.WeekDay] = TimelineHeaderTemplate.Create(
                TimelineHeaderUnit.Month,
                "date.month-year-verbose",  // "January 2025"
                TimelineHeaderUnit.Day,
                "date.day-number",          // "1", "2", "3"
                "Daily sprint planning and detailed task management"
            ),

            [TimelineZoomLevel.WeekDayMedium] = TimelineHeaderTemplate.Create(
                TimelineHeaderUnit.Month,
                "date.month-year",          // "Jan 2025"
                TimelineHeaderUnit.Day,
                "date.day-number",          // "1", "2", "3"
                "Medium-detail daily view for project tracking"
            ),

            [TimelineZoomLevel.WeekDayLow] = TimelineHeaderTemplate.Create(
                TimelineHeaderUnit.Month,
                "date.month-short",         // "Jan"
                TimelineHeaderUnit.Day,
                "date.day-number",          // "1", "2", "3"
                "Compact daily view for quick overview"
            ),

            // Wide zoom levels (40px-32px) - Standard formats
            [TimelineZoomLevel.MonthDay] = TimelineHeaderTemplate.Create(
                TimelineHeaderUnit.Month,
                "date.month-short",         // "Jan"
                TimelineHeaderUnit.Day,
                "date.day-number",          // "1", "2", "3"
                "Standard project milestone and phase tracking"
            ),

            [TimelineZoomLevel.MonthDayMedium] = TimelineHeaderTemplate.Create(
                TimelineHeaderUnit.Quarter,
                "date.quarter-short",       // "Q1"
                TimelineHeaderUnit.Day,
                "date.day-abbrev",          // "M", "T", "W" (Mon/Tue/Wed)
                "Medium project overview with quarterly context"
            ),

            // Medium zoom levels (24px-16px) - Weekly granularity
            [TimelineZoomLevel.MonthWeek] = TimelineHeaderTemplate.Create(
                TimelineHeaderUnit.Quarter,
                "date.quarter-year",        // "Q1 2025"
                TimelineHeaderUnit.Month,
                "date.month-abbrev",        // "J", "F", "M"
                "Quarterly planning and resource scheduling"
            ),

            [TimelineZoomLevel.MonthWeekMedium] = TimelineHeaderTemplate.Create(
                TimelineHeaderUnit.Quarter,
                "date.quarter-short",       // "Q1"
                TimelineHeaderUnit.Month,
                "date.month-abbrev",        // "J", "F", "M"
                "Medium quarterly view for resource allocation"
            ),

            [TimelineZoomLevel.MonthWeekLow] = TimelineHeaderTemplate.Create(
                TimelineHeaderUnit.Year,
                "date.year",                // "2025"
                TimelineHeaderUnit.Quarter,
                "date.quarter-minimal",     // "1", "2", "3", "4"
                "Yearly overview with quarterly breakdown"
            ),

            // Narrow zoom levels (12.8px-8px) - Monthly granularity
            [TimelineZoomLevel.QuarterWeek] = TimelineHeaderTemplate.Create(
                TimelineHeaderUnit.Year,
                "date.year",                // "2025"
                TimelineHeaderUnit.Quarter,
                "date.quarter-minimal",     // "1", "2", "3", "4"
                "Annual planning with quarterly milestones"
            ),

            [TimelineZoomLevel.QuarterWeekMedium] = TimelineHeaderTemplate.Create(
                TimelineHeaderUnit.Decade,
                "date.decade",              // "2020-2029"
                TimelineHeaderUnit.Year,
                "date.year-short",          // "25", "26", "27"
                "Multi-year programs and strategic roadmaps"
            ),

            [TimelineZoomLevel.QuarterMonth] = TimelineHeaderTemplate.Create(
                TimelineHeaderUnit.Decade,
                "date.decade-short",        // "20s"
                TimelineHeaderUnit.Year,
                "date.year-short",          // "25", "26", "27"
                "Strategic multi-year planning"
            ),

            [TimelineZoomLevel.QuarterMonthMedium] = TimelineHeaderTemplate.Create(
                TimelineHeaderUnit.Decade,
                "date.decade-short",        // "20s"
                TimelineHeaderUnit.Year,
                "date.year-minimal",        // "5", "6", "7" (last digit)
                "Long-term strategic overview"
            ),

            // Ultra-narrow zoom level (3px) - Minimal display
            [TimelineZoomLevel.YearQuarter] = TimelineHeaderTemplate.Create(
                TimelineHeaderUnit.Decade,
                "date.decade-minimal",      // "20", "30"
                TimelineHeaderUnit.Year,
                "date.year-minimal",        // "5", "6", "7" (last digit)
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
