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
    /// Calculate boundaries for primary header rendering (Month-Year displays).
    /// MonthWeek pattern: Month headers need month boundaries for complete month coverage.
    /// </summary>
    /// <returns>Month boundary dates for primary month header complete rendering</returns>
    protected override (DateTime start, DateTime end) CalculatePrimaryBoundaries()
    {
        var monthBounds = BoundaryCalculationHelpers.GetMonthBoundaries(StartDate, EndDate);
        Logger.LogDebugInfo($"MonthWeek50px primary boundaries (Month headers): {monthBounds.start} to {monthBounds.end}");
        return monthBounds;
    }

    /// <summary>
    /// Calculate boundaries for secondary header rendering (Week start dates).
    /// MonthWeek pattern: Week headers need week boundaries since weeks can cross month boundaries.
    /// This is the KEY FIX: Week headers that span across months need week boundaries, not month boundaries.
    /// </summary>
    /// <returns>Week boundary dates for secondary week header complete rendering</returns>
    protected override (DateTime start, DateTime end) CalculateSecondaryBoundaries()
    {
        var weekBounds = BoundaryCalculationHelpers.GetWeekBoundaries(StartDate, EndDate);
        Logger.LogDebugInfo($"MonthWeek50px secondary boundaries (Week headers): {weekBounds.start} to {weekBounds.end}");
        return weekBounds;
    }

    /// <summary>
    /// Renders the primary header with month-year displays.
    /// Uses automatic dual boundary expansion from base class.
    /// </summary>
    /// <returns>SVG markup for primary header</returns>
    protected override string RenderPrimaryHeader()
    {
        // Use expanded boundaries calculated by base class union logic
        return RenderMonthHeader(StartDate, EndDate);
    }

    /// <summary>
    /// Renders the secondary header with week start dates.
    /// Uses automatic dual boundary expansion from base class.
    /// </summary>
    /// <returns>SVG markup for secondary header</returns>
    protected override string RenderSecondaryHeader()
    {
        // Use expanded boundaries calculated by base class union logic
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

            // CRITICAL FIX: Use fixed coordinate system to match taskbar positioning
            var xPosition = SVGRenderingHelpers.DayToSVGX(monthStart, CoordinateSystemStart, DayWidth);

            // Calculate month width in pixels
            var monthDays = (monthEnd - monthStart).Days + 1;
            var monthWidth = monthDays * DayWidth;

            // Month display: "February 2025"
            var monthText = $"{monthStart:MMMM yyyy}";

            // Render month header cell
            svg.Append(CreateSVGRect(xPosition, 0, monthWidth, HeaderMonthHeight, GetCSSClass() + "-cell-primary"));
            svg.Append(CreateSVGText(xPosition + monthWidth / 2, HeaderMonthHeight / 2, monthText, GetCSSClass() + "-primary-text"));

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

        // DIAGNOSTIC: Log first few weeks for debugging
        var weekCount = 0;
        var tempDate = currentDate;
        while (tempDate <= end && weekCount < 3)
        {
            var weekText = $"{tempDate.Month}/{tempDate.Day}";
            Logger.LogDebugInfo($"Week header #{weekCount}: {tempDate:yyyy-MM-dd} ({tempDate.DayOfWeek}) -> '{weekText}'");
            tempDate = tempDate.AddDays(7);
            weekCount++;
        }

        while (currentDate <= end)
        {
            var weekStart = currentDate;
            var weekEnd = currentDate.AddDays(6); // Sunday

            // CRITICAL FIX: Use fixed coordinate system to match taskbar positioning
            var xPosition = SVGRenderingHelpers.DayToSVGX(weekStart, CoordinateSystemStart, DayWidth);

            // Calculate week width (7 days)
            var weekWidth = 7 * DayWidth;

            // Week display: "2/17" (Monday date)
            var weekText = $"{weekStart.Month}/{weekStart.Day}";

            // Render week header cell
            svg.Append(CreateSVGRect(xPosition, HeaderMonthHeight, weekWidth, HeaderDayHeight, GetCSSClass() + "-cell-secondary"));
            svg.Append(CreateSVGText(xPosition + weekWidth / 2, HeaderMonthHeight + HeaderDayHeight / 2, weekText, GetCSSClass() + "-secondary-text"));

            currentDate = currentDate.AddDays(7); // Next Monday
        }

        return svg.ToString();
    }
}
