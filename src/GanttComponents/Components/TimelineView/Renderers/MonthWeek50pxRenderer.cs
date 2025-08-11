using GanttComponents.Models;
using GanttComponents.Services;

namespace GanttComponents.Components.TimelineView.Renderers;

/// <summary>
/// MonthWeek 50px level renderer for TimelineView composition architecture.
/// Handles month-week pattern with calculated day width validation.
/// Primary Header: Month-Year ("February 2025", "March 2025")
/// Secondary Header: Week start dates ("2/17", "2/24", "3/3") - Monday dates
/// Cell Width: Calculated week cells based on day width (typically 8px day = 56px week)
/// Optimized for medium-range planning with weekly breakdown by month.
/// Includes union expansion for complete header rendering at timeline edges.
/// </summary>
public class MonthWeek50pxRenderer : BaseTimelineRenderer
{
    /// <summary>
    /// Constructor for MonthWeek 50px renderer with dependency injection.
    /// Uses calculated day width for flexible week cell sizing.
    /// Union expansion is handled automatically by the base class.
    /// </summary>
    public MonthWeek50pxRenderer(
        IUniversalLogger logger,
        IGanttI18N i18n,
        DateFormatHelper dateFormatter,
        DateTime startDate,
        DateTime endDate,
        double dayWidth,
        int headerMonthHeight,
        int headerDayHeight,
        TimelineZoomLevel zoomLevel,
        double zoomFactor)
        : base(logger, i18n, dateFormatter, startDate, endDate, dayWidth,
               headerMonthHeight, headerDayHeight, zoomLevel, zoomFactor)
    {
        Logger.LogDebugInfo($"MonthWeek50pxRenderer initialized - StartDate: {startDate}, EndDate: {endDate}, DayWidth: {DayWidth}");
    }

    /// <summary>
    /// Calculates header boundaries with union expansion for complete month coverage.
    /// Extends timeline range to ensure month headers are not truncated at edges.
    /// MonthWeek pattern: Extend to month boundaries (first of month to last of month).
    /// </summary>
    /// <returns>Expanded start and end dates for complete header rendering</returns>
    protected override (DateTime expandedStart, DateTime expandedEnd) CalculateHeaderBoundaries()
    {
        // Extend to month boundaries for complete header coverage
        var expandedStart = GetMonthStart(StartDate);
        var expandedEnd = GetMonthEnd(EndDate);

        Logger.LogDebugInfo($"MonthWeek50px union expansion - Original: {StartDate} to {EndDate}, Expanded: {expandedStart} to {expandedEnd}");

        return (expandedStart, expandedEnd);
    }

    /// <summary>
    /// Renders the primary header with month-year displays.
    /// </summary>
    /// <returns>SVG markup for primary header</returns>
    protected override string RenderPrimaryHeader()
    {
        var (expandedStart, expandedEnd) = CalculateHeaderBoundaries();
        return RenderMonthHeader(expandedStart, expandedEnd);
    }

    /// <summary>
    /// Renders the secondary header with week start dates.
    /// </summary>
    /// <returns>SVG markup for secondary header</returns>
    protected override string RenderSecondaryHeader()
    {
        var (expandedStart, expandedEnd) = CalculateHeaderBoundaries();
        return RenderWeekHeader(expandedStart, expandedEnd);
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

    // === MONTH BOUNDARY UTILITIES ===

    /// <summary>
    /// Gets the first day of the month containing the given date.
    /// </summary>
    /// <param name="date">Date within the month</param>
    /// <returns>First day of the month</returns>
    private DateTime GetMonthStart(DateTime date)
    {
        return new DateTime(date.Year, date.Month, 1);
    }

    /// <summary>
    /// Gets the last day of the month containing the given date.
    /// </summary>
    /// <param name="date">Date within the month</param>
    /// <returns>Last day of the month</returns>
    private DateTime GetMonthEnd(DateTime date)
    {
        return new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
    }

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
        double xPosition = 0;

        while (currentDate <= end)
        {
            var monthStart = GetMonthStart(currentDate);
            var monthEnd = GetMonthEnd(currentDate);

            // Calculate month width in pixels
            var monthDays = (monthEnd - monthStart).Days + 1;
            var monthWidth = monthDays * DayWidth;

            // Month display: "February 2025"
            var monthText = $"{monthStart:MMMM yyyy}";

            // Render month header cell
            svg.Append(CreateSVGRect(xPosition, 0, monthWidth, HeaderMonthHeight, GetCSSClass() + "-cell-primary"));
            svg.Append(CreateSVGText(xPosition + monthWidth / 2, HeaderMonthHeight / 2, monthText, GetCSSClass() + "-primary-text"));

            xPosition += monthWidth;
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
        var currentDate = GetWeekStart(start); // Start from Monday of first week
        double xPosition = 0;

        while (currentDate <= end)
        {
            var weekStart = currentDate;
            var weekEnd = currentDate.AddDays(6); // Sunday

            // Calculate week width (7 days)
            var weekWidth = 7 * DayWidth;

            // Week display: "2/17" (Monday date)
            var weekText = $"{weekStart.Month}/{weekStart.Day}";

            // Render week header cell
            svg.Append(CreateSVGRect(xPosition, HeaderMonthHeight, weekWidth, HeaderDayHeight, GetCSSClass() + "-cell-secondary"));
            svg.Append(CreateSVGText(xPosition + weekWidth / 2, HeaderMonthHeight + HeaderDayHeight / 2, weekText, GetCSSClass() + "-secondary-text"));

            xPosition += weekWidth;
            currentDate = currentDate.AddDays(7); // Next Monday
        }

        return svg.ToString();
    }
}
