using GanttComponents.Models;
using GanttComponents.Services;

namespace GanttComponents.Components.TimelineView.Renderers;

/// <summary>
/// MonthWeek level renderer for TimelineView composition architecture.
/// Handles month-week pattern with calculated day width validation.
/// Primary Header: Month-Year ("February 2025", "March 2025")
/// Secondary Header: Week start dates ("2/17", "2/24", "3/3") - Monday dates
/// Optimized for medium-range planning with weekly breakdown by month.
/// Includes union expansion for complete header rendering at timeline edges.
/// </summary>
public class MonthWeekRenderer : BaseTimelineRenderer
{
    /// <summary>
    /// Constructor for MonthWeek template renderer with dependency injection.
    /// Uses template-based approach: 18px per week with 3.0x max zoom.
    /// Template unit: 1 week (7 days) = 18px base width.
    /// </summary>
    public MonthWeekRenderer(
        IUniversalLogger logger,
        DateFormatHelper dateFormatter,
        DateTime startDate,
        DateTime endDate,
        TimelineZoomLevel zoomLevel,
        double zoomFactor,
        int headerMonthHeight,
        int headerDayHeight)
        : base(logger, dateFormatter, startDate, endDate,
               zoomLevel, zoomFactor, headerMonthHeight, headerDayHeight)
    {
    }

    /// <summary>
    /// Renders the primary header with month-year displays.
    /// Uses template-unit padding from base class.
    /// </summary>
    /// <returns>SVG markup for primary header</returns>
    protected override string RenderPrimaryHeader()
    {
        // Use expanded boundaries calculated by base class template-unit padding
        return RenderMonthHeader(StartDate, EndDate);
    }

    /// <summary>
    /// Renders the secondary header with week start dates.
    /// Uses template-unit padding from base class.
    /// </summary>
    /// <returns>SVG markup for secondary header</returns>
    protected override string RenderSecondaryHeader()
    {
        // Use expanded boundaries calculated by base class template-unit padding
        return RenderWeekHeader(StartDate, EndDate);
    }

    /// <summary>
    /// Gets a human-readable description of this renderer for logging and debugging.
    /// </summary>
    /// <returns>Renderer description</returns>
    protected override string GetRendererDescription()
    {
        return "MonthWeek 50px level with month-year and week starts - ABC Composition";
    }

    /// <summary>
    /// Gets the CSS class prefix for this renderer's styling.
    /// </summary>
    /// <returns>CSS class prefix</returns>
    protected override string GetCSSClass() => "monthweek-50px";

    /// <summary>
    /// Calculates logical unit boundaries for MonthWeek pattern.
    /// Uses union of month and week boundaries to ensure both complete months and complete weeks.
    /// </summary>
    /// <param name="startDate">Original timeline start date</param>
    /// <param name="endDate">Original timeline end date</param>
    /// <returns>Union boundaries that guarantee complete months and weeks</returns>
    protected override (DateTime start, DateTime end) GetLogicalUnitBoundaries(DateTime startDate, DateTime endDate)
    {
        // MonthWeek pattern: Union of month and week boundaries
        var monthBounds = BoundaryCalculationHelpers.GetMonthBoundaries(startDate, endDate);
        var weekBounds = BoundaryCalculationHelpers.GetWeekBoundaries(startDate, endDate);

        // Take the widest span (earliest start, latest end)
        var unionStart = monthBounds.start < weekBounds.start ? monthBounds.start : weekBounds.start;
        var unionEnd = monthBounds.end > weekBounds.end ? monthBounds.end : weekBounds.end;

        return (unionStart, unionEnd);
    }

    // === HEADER RENDERING METHODS ===

    /// <summary>
    /// Renders the primary header with month-year displays.
    /// </summary>
    /// <param name="start">Expanded start date</param>
    /// <param name="end">Expanded end date</param>
    /// <returns>SVG markup for month header</returns>
    private string RenderMonthHeader(DateTime start, DateTime end)
    {
        var svg = new System.Text.StringBuilder();
        var currentDate = start;

        while (currentDate <= end)
        {
            var monthBounds = BoundaryCalculationHelpers.GetMonthBoundaries(currentDate, currentDate);
            var monthStart = monthBounds.start;
            var monthEnd = monthBounds.end;

            // Month display: "February 2025"
            var monthText = $"{monthStart:MMMM yyyy}";

            // SoC BENEFIT: Renderer focuses on WHAT to show, base class handles HOW to position
            svg.Append(CreateValidatedHeaderCell(
                monthStart, monthEnd, 0, HeaderMonthHeight,
                monthText, GetCSSClass() + "-cell-primary", GetCSSClass() + "-primary-text"));

            // Move to next month
            currentDate = monthEnd.AddDays(1);
        }

        return svg.ToString();
    }

    /// <summary>
    /// Renders the secondary header with week start dates (Monday dates).
    /// </summary>
    /// <param name="start">Expanded start date</param>
    /// <param name="end">Expanded end date</param>
    /// <returns>SVG markup for week header</returns>
    private string RenderWeekHeader(DateTime start, DateTime end)
    {
        var svg = new System.Text.StringBuilder();
        var weekBounds = BoundaryCalculationHelpers.GetWeekBoundaries(start, end);
        var currentDate = weekBounds.start; // Start from Monday of first week

        while (currentDate <= end)
        {
            var weekStart = currentDate;
            var weekEnd = currentDate.AddDays(6); // Sunday

            // Week display: "2/17" (Monday date)
            var weekText = $"{weekStart.Month}/{weekStart.Day}";

            // SoC BENEFIT: Clean, focused code - no coordinate calculations cluttering business logic
            svg.Append(CreateValidatedHeaderCell(
                weekStart, weekEnd, HeaderMonthHeight, HeaderDayHeight,
                weekText, GetCSSClass() + "-cell-secondary", GetCSSClass() + "-secondary-text"));

            currentDate = currentDate.AddDays(7); // Next Monday
        }

        return svg.ToString();
    }
}
