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
    /// Predefined header templates for all zoom levels.
    /// Each template is optimized for its specific day width and use case.
    /// Based on GanttResources.resx specifications with integral pixel widths.
    /// Updated for optimal cell size approach with revolutionary 30-70px cell density.
    /// </summary>
    private static readonly Dictionary<TimelineZoomLevel, TimelineHeaderTemplate> _templates =
        new Dictionary<TimelineZoomLevel, TimelineHeaderTemplate>
        {
            // ========================================
            // OPTIMAL TEMPLATES (Revolutionary Cell Density)
            // ========================================

            // YearQuarter Pattern (4 levels): 30px → 40px → 50px → 70px quarter cells
            [TimelineZoomLevel.YearQuarterOptimal30px] = TimelineHeaderTemplate.Create(
                TimelineHeaderUnit.Year,
                "date.year-full",           // "2024", "2025" - top tier: full year
                TimelineHeaderUnit.Quarter,
                "date.quarter-full",        // "Q1", "Q2", "Q3", "Q4" - bottom tier: quarters
                "Strategic view with optimal 30px quarter cells",
                showPrimary: true,
                showSecondary: true
            ),

            [TimelineZoomLevel.YearQuarterOptimal40px] = TimelineHeaderTemplate.Create(
                TimelineHeaderUnit.Year,
                "date.year-full",           // "2024", "2025" - top tier: full year
                TimelineHeaderUnit.Quarter,
                "date.quarter-full",        // "Q1", "Q2", "Q3", "Q4" - bottom tier: quarters
                "Strategic view with optimal 40px quarter cells",
                showPrimary: true,
                showSecondary: true
            ),

            [TimelineZoomLevel.YearQuarterOptimal50px] = TimelineHeaderTemplate.Create(
                TimelineHeaderUnit.Year,
                "date.year-full",           // "2024", "2025" - top tier: full year
                TimelineHeaderUnit.Quarter,
                "date.quarter-full",        // "Q1", "Q2", "Q3", "Q4" - bottom tier: quarters
                "Strategic view with optimal 50px quarter cells",
                showPrimary: true,
                showSecondary: true
            ),

            [TimelineZoomLevel.YearQuarterOptimal70px] = TimelineHeaderTemplate.Create(
                TimelineHeaderUnit.Year,
                "date.year-full",           // "2024", "2025" - top tier: full year
                TimelineHeaderUnit.Quarter,
                "date.quarter-full",        // "Q1", "Q2", "Q3", "Q4" - bottom tier: quarters
                "Strategic view with optimal 70px quarter cells",
                showPrimary: true,
                showSecondary: true
            ),

            // QuarterMonth Pattern (5 levels): 30px → 40px → 50px → 60px → 70px month cells
            [TimelineZoomLevel.QuarterMonthOptimal30px] = TimelineHeaderTemplate.Create(
                TimelineHeaderUnit.Quarter,
                "date.quarter-year",        // "Q1 2024", "Q2 2024" - top tier: quarter with year
                TimelineHeaderUnit.Month,
                "date.month-short",         // "Jan", "Feb", "Mar" - bottom tier: month abbreviations
                "Quarterly planning with optimal 30px month cells",
                showPrimary: true,
                showSecondary: true
            ),

            [TimelineZoomLevel.QuarterMonthOptimal40px] = TimelineHeaderTemplate.Create(
                TimelineHeaderUnit.Quarter,
                "date.quarter-year",        // "Q1 2024", "Q2 2024" - top tier: quarter with year
                TimelineHeaderUnit.Month,
                "date.month-short",         // "Jan", "Feb", "Mar" - bottom tier: month abbreviations
                "Quarterly planning with optimal 40px month cells",
                showPrimary: true,
                showSecondary: true
            ),

            [TimelineZoomLevel.QuarterMonthOptimal50px] = TimelineHeaderTemplate.Create(
                TimelineHeaderUnit.Quarter,
                "date.quarter-year",        // "Q1 2024", "Q2 2024" - top tier: quarter with year
                TimelineHeaderUnit.Month,
                "date.month-short",         // "Jan", "Feb", "Mar" - bottom tier: month abbreviations
                "Quarterly planning with optimal 50px month cells",
                showPrimary: true,
                showSecondary: true
            ),

            [TimelineZoomLevel.QuarterMonthOptimal60px] = TimelineHeaderTemplate.Create(
                TimelineHeaderUnit.Quarter,
                "date.quarter-year",        // "Q1 2024", "Q2 2024" - top tier: quarter with year
                TimelineHeaderUnit.Month,
                "date.month-short",         // "Jan", "Feb", "Mar" - bottom tier: month abbreviations
                "Quarterly planning with optimal 60px month cells",
                showPrimary: true,
                showSecondary: true
            ),

            [TimelineZoomLevel.QuarterMonthOptimal60px] = TimelineHeaderTemplate.Create(
                TimelineHeaderUnit.Quarter,
                "date.quarter-year",        // "Q1 2024", "Q2 2024" - top tier: quarter with year
                TimelineHeaderUnit.Month,
                "date.month-short",         // "Jan", "Feb", "Mar" - bottom tier: month abbreviations
                "Quarterly planning with optimal 60px month cells",
                showPrimary: true,
                showSecondary: true
            ),

            // MonthWeek Pattern (4 levels): 30px → 40px → 50px → 60px week cells
            [TimelineZoomLevel.MonthWeekOptimal30px] = TimelineHeaderTemplate.Create(
                TimelineHeaderUnit.Month,
                "date.month-year",          // "Jan 2024", "Feb 2024" - top tier: month with year
                TimelineHeaderUnit.Week,
                "date.week-number",         // "W1", "W2", "W3" - bottom tier: week numbers
                "Monthly planning with optimal 30px week cells",
                showPrimary: true,
                showSecondary: true
            ),

            [TimelineZoomLevel.MonthWeekOptimal40px] = TimelineHeaderTemplate.Create(
                TimelineHeaderUnit.Month,
                "date.month-year",          // "Jan 2024", "Feb 2024" - top tier: month with year
                TimelineHeaderUnit.Week,
                "date.week-number",         // "W1", "W2", "W3" - bottom tier: week numbers
                "Monthly planning with optimal 40px week cells",
                showPrimary: true,
                showSecondary: true
            ),

            [TimelineZoomLevel.MonthWeekOptimal50px] = TimelineHeaderTemplate.Create(
                TimelineHeaderUnit.Month,
                "date.month-year",          // "Jan 2024", "Feb 2024" - top tier: month with year
                TimelineHeaderUnit.Week,
                "date.week-number",         // "W1", "W2", "W3" - bottom tier: week numbers
                "Monthly planning with optimal 50px week cells",
                showPrimary: true,
                showSecondary: true
            ),

            [TimelineZoomLevel.MonthWeekOptimal60px] = TimelineHeaderTemplate.Create(
                TimelineHeaderUnit.Month,
                "date.month-year",          // "Jan 2024", "Feb 2024" - top tier: month with year
                TimelineHeaderUnit.Week,
                "date.week-number",         // "W1", "W2", "W3" - bottom tier: week numbers
                "Monthly planning with optimal 60px week cells",
                showPrimary: true,
                showSecondary: true
            ),

            // WeekDay Pattern (4 levels): 30px → 40px → 50px → 60px day cells
            [TimelineZoomLevel.WeekDayOptimal30px] = TimelineHeaderTemplate.Create(
                TimelineHeaderUnit.Week,
                "date.week-range",          // "MMM %d, yyyy" / "yyyy-MM-%d" - top tier: full week start date
                TimelineHeaderUnit.Day,
                "date.day-number",          // "1", "2", "3" - bottom tier: day numbers
                "Daily scheduling with optimal 30px day cells",
                showPrimary: true,
                showSecondary: true
            ),

            [TimelineZoomLevel.WeekDayOptimal40px] = TimelineHeaderTemplate.Create(
                TimelineHeaderUnit.Week,
                "date.week-range",          // "MMM %d, yyyy" / "yyyy-MM-%d" - top tier: full week start date
                TimelineHeaderUnit.Day,
                "date.day-number",          // "1", "2", "3" - bottom tier: day numbers
                "Daily scheduling with optimal 40px day cells",
                showPrimary: true,
                showSecondary: true
            ),

            [TimelineZoomLevel.WeekDayOptimal50px] = TimelineHeaderTemplate.Create(
                TimelineHeaderUnit.Week,
                "date.week-range",          // "MMM %d, yyyy" / "yyyy-MM-%d" - top tier: full week start date
                TimelineHeaderUnit.Day,
                "date.day-number",          // "1", "2", "3" - bottom tier: day numbers
                "Daily scheduling with optimal 50px day cells",
                showPrimary: true,
                showSecondary: true
            ),

            [TimelineZoomLevel.WeekDayOptimal60px] = TimelineHeaderTemplate.Create(
                TimelineHeaderUnit.Week,
                "date.week-range",          // "MMM %d, yyyy" / "yyyy年MM月%d日" - top tier: full week start date
                TimelineHeaderUnit.Day,
                "date.day-number",          // "1", "2", "3" - bottom tier: day numbers
                "Daily scheduling with optimal 60px day cells",
                showPrimary: true,
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
