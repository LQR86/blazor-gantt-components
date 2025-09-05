using GanttComponents.Models;
using GanttComponents.Services;

namespace GanttComponents.Components.TimelineView.Renderers;

/// <summary>
/// WeekDay 50px level renderer for TimelineView composition architecture.
/// Handles week-day pattern with integral day width validation.
/// Primary Header: Week ranges ("February 17-23, 2025")
/// Secondary Header: Day names with numbers ("Mon 17", "Tue 18")
/// Cell Width: 50px day cells with 50px integral day width
/// Optimized for detailed weekly planning with daily breakdown.
/// Includes union expansion for complete header rendering at timeline edges.
/// </summary>
public class WeekDay50pxRenderer : BaseTimelineRenderer
{
    /// <summary>
    /// Constructor for WeekDay template renderer with dependency injection.
    /// Uses template-based approach: 12px per day with 2.5x max zoom.
    /// Template unit: 1 day = 12px base width.
    /// </summary>
    public WeekDay50pxRenderer(
        IUniversalLogger logger,
        IGanttI18N i18n,
        DateFormatHelper dateFormatter,
        DateTime startDate,
        DateTime endDate,
        TimelineZoomLevel zoomLevel,
        double zoomFactor,
        int headerMonthHeight,
        int headerDayHeight)
        : base(logger, i18n, dateFormatter, startDate, endDate,
               zoomLevel, zoomFactor, headerMonthHeight, headerDayHeight)
    {
    }

    /// <summary>
    /// Renders the primary header with week ranges.
    /// Uses template-unit padding from base class.
    /// </summary>
    /// <returns>SVG markup for primary header</returns>
    protected override string RenderPrimaryHeader()
    {
        // Use expanded boundaries calculated by base class template-unit padding
        return RenderWeekHeader(StartDate, EndDate);
    }

    /// <summary>
    /// Renders the secondary header with day names and numbers.
    /// Uses template-unit padding from base class.
    /// </summary>
    /// <returns>SVG markup for secondary header</returns>
    protected override string RenderSecondaryHeader()
    {
        // Use expanded boundaries calculated by base class union logic
        return RenderDayHeader(StartDate, EndDate);
    }

    /// <summary>
    /// Gets a human-readable description of this renderer for logging and debugging.
    /// </summary>
    /// <returns>Renderer description</returns>
    protected override string GetRendererDescription()
    {
        return "WeekDay 50px level with week ranges and day names - ABC Composition";
    }

    /// <summary>
    /// Gets the CSS class prefix for this renderer's styling.
    /// </summary>
    /// <returns>CSS class prefix</returns>
    protected override string GetCSSClass() => "weekday-50px";

    // === HEADER RENDERING METHODS ===

    /// <summary>
    /// Renders the primary header with week ranges.
    /// </summary>
    /// <param name="start">Expanded start date</param>
    /// <param name="end">Expanded end date</param>
    /// <returns>SVG markup for week header</returns>
    private string RenderWeekHeader(DateTime start, DateTime end)
    {
        var svg = new System.Text.StringBuilder();
        var currentDate = start;

        while (currentDate <= end)
        {
            var weekBounds = BoundaryCalculationHelpers.GetWeekBoundaries(currentDate, currentDate);
            var weekStart = weekBounds.start;
            var weekEnd = weekBounds.end;

            // Week display: "February 17-23, 2025"
            var weekText = FormatWeekRange(weekStart, weekEnd);

            // SoC BENEFIT: Renderer focuses on WHAT to show, base class handles HOW to position
            svg.Append(CreateValidatedHeaderCell(
                weekStart, weekEnd, 0, HeaderMonthHeight,
                weekText, GetCSSClass() + "-cell-primary", GetCSSClass() + "-primary-text"));

            // Move to next week
            currentDate = weekEnd.AddDays(1);
        }

        return svg.ToString();
    }

    /// <summary>
    /// Renders the secondary header with day names and numbers.
    /// </summary>
    /// <param name="start">Expanded start date</param>
    /// <param name="end">Expanded end date</param>
    /// <returns>SVG markup for day header</returns>
    private string RenderDayHeader(DateTime start, DateTime end)
    {
        var svg = new System.Text.StringBuilder();
        var currentDate = start;

        while (currentDate <= end)
        {
            // Day display: "Mon 17", "Tue 18", etc.
            var dayText = $"{currentDate:ddd} {currentDate.Day}";

            // SoC BENEFIT: Single day cells use individual date for both start and end
            svg.Append(CreateValidatedHeaderCell(
                currentDate, currentDate, HeaderMonthHeight, HeaderDayHeight,
                dayText, GetCSSClass() + "-cell-secondary", GetCSSClass() + "-secondary-text"));

            // Move to next day
            currentDate = currentDate.AddDays(1);
        }

        return svg.ToString();
    }

    /// <summary>
    /// Formats a week range for display.
    /// </summary>
    /// <param name="weekStart">Monday of the week</param>
    /// <param name="weekEnd">Sunday of the week</param>
    /// <returns>Formatted week range string</returns>
    private string FormatWeekRange(DateTime weekStart, DateTime weekEnd)
    {
        try
        {
            // Premium format for 350px cells (50px day width) - maximum space available
            if (weekStart.Month == weekEnd.Month && weekStart.Year == weekEnd.Year)
            {
                // Same month: "February 17-23, 2025" (full month name)
                return $"{weekStart:MMMM} {weekStart.Day}-{weekEnd.Day}, {weekStart:yyyy}";
            }
            else if (weekStart.Year == weekEnd.Year)
            {
                // Different months: "February 28 - March 6, 2025" (full month names)
                return $"{weekStart:MMMM d} - {weekEnd:MMMM d}, {weekStart:yyyy}";
            }
            else
            {
                // Different years: "December 30, 2024 - January 5, 2025" (full month names)
                return $"{weekStart:MMMM d, yyyy} - {weekEnd:MMMM d, yyyy}";
            }
        }
        catch (Exception ex)
        {
            Logger.LogError($"Error formatting week range {weekStart:yyyy-MM-dd} to {weekEnd:yyyy-MM-dd}: {ex.Message}");
            return $"{weekStart:MMM d} - {weekEnd:MMM d}, {weekStart:yyyy}";
        }
    }
}
