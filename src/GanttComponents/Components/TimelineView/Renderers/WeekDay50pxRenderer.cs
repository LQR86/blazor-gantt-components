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
    /// Constructor for WeekDay 50px renderer with dependency injection.
    /// Uses integral 50px day width for perfect SVG coordinate calculations.
    /// Union expansion is handled automatically by the base class.
    /// </summary>
    public WeekDay50pxRenderer(
        IUniversalLogger logger,
        IGanttI18N i18n,
        DateFormatHelper dateFormatter,
        DateTime startDate,
        DateTime endDate,
        int headerMonthHeight,
        int headerDayHeight,
        TimelineZoomLevel zoomLevel,
        double zoomFactor)
        : base(logger, i18n, dateFormatter, startDate, endDate,
               50.0, // INTEGRAL DAY WIDTH: 50px day width = 350px week cells (50px × 7 days)
               headerMonthHeight, headerDayHeight, zoomLevel, zoomFactor)
    {
        Logger.LogDebugInfo($"WeekDay50pxRenderer initialized - StartDate: {startDate}, EndDate: {endDate}, DayWidth: {DayWidth}");
    }

    /// <summary>
    /// Calculates header boundaries with union expansion for complete week coverage.
    /// Extends timeline range to ensure week headers are not truncated at edges.
    /// WeekDay pattern: Extend to week boundaries (Monday to Sunday).
    /// </summary>
    /// <returns>Expanded start and end dates for complete header rendering</returns>
    protected override (DateTime expandedStart, DateTime expandedEnd) CalculateHeaderBoundaries()
    {
        // Extend to week boundaries for complete header coverage
        var expandedStart = GetWeekStart(StartDate);
        var expandedEnd = GetWeekEnd(EndDate);

        Logger.LogDebugInfo($"WeekDay50px union expansion - Original: {StartDate} to {EndDate}, Expanded: {expandedStart} to {expandedEnd}");

        return (expandedStart, expandedEnd);
    }

    /// <summary>
    /// Renders the primary header with week ranges.
    /// </summary>
    /// <returns>SVG markup for primary header</returns>
    protected override string RenderPrimaryHeader()
    {
        var (expandedStart, expandedEnd) = CalculateHeaderBoundaries();
        return RenderWeekHeader(expandedStart, expandedEnd);
    }

    /// <summary>
    /// Renders the secondary header with day names and numbers.
    /// </summary>
    /// <returns>SVG markup for secondary header</returns>
    protected override string RenderSecondaryHeader()
    {
        var (expandedStart, expandedEnd) = CalculateHeaderBoundaries();
        return RenderDayHeader(expandedStart, expandedEnd);
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

    // === WEEK BOUNDARY UTILITIES ===

    /// <summary>
    /// Gets the Monday of the week containing the given date.
    /// </summary>
    /// <param name="date">Date within the week</param>
    /// <returns>Monday of the week</returns>
    private DateTime GetWeekStart(DateTime date)
    {
        var current = date;
        while (current.DayOfWeek != DayOfWeek.Monday)
        {
            current = current.AddDays(-1);
        }
        return current;
    }

    /// <summary>
    /// Gets the Sunday of the week containing the given date.
    /// </summary>
    /// <param name="date">Date within the week</param>
    /// <returns>Sunday of the week</returns>
    private DateTime GetWeekEnd(DateTime date)
    {
        var monday = GetWeekStart(date);
        return monday.AddDays(6); // Sunday
    }

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
        double xPosition = 0;

        while (currentDate <= end)
        {
            var weekStart = GetWeekStart(currentDate);
            var weekEnd = GetWeekEnd(currentDate);

            // Calculate week width (7 days * 50px = 350px)
            var weekDays = (weekEnd - weekStart).Days + 1;
            var weekWidth = weekDays * DayWidth;

            // Week display: "February 17-23, 2025"
            var weekText = FormatWeekRange(weekStart, weekEnd);

            // Render week header cell
            svg.Append(CreateSVGRect(xPosition, 0, weekWidth, HeaderMonthHeight, GetCSSClass() + "-cell-primary"));
            svg.Append(CreateSVGText(xPosition + weekWidth / 2, HeaderMonthHeight / 2, weekText, GetCSSClass() + "-primary-text"));

            xPosition += weekWidth;
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
        double xPosition = 0;

        while (currentDate <= end)
        {
            // Day display: "Mon 17", "Tue 18", etc.
            var dayText = $"{currentDate:ddd} {currentDate.Day}";

            // Render day header cell
            svg.Append(CreateSVGRect(xPosition, HeaderMonthHeight, DayWidth, HeaderDayHeight, GetCSSClass() + "-cell-secondary"));
            svg.Append(CreateSVGText(xPosition + DayWidth / 2, HeaderMonthHeight + HeaderDayHeight / 2, dayText, GetCSSClass() + "-secondary-text"));

            xPosition += DayWidth;
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
