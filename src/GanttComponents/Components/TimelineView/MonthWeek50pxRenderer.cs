using GanttComponents.Models;
using GanttComponents.Services;

namespace GanttComponents.Components.TimelineView;

/// <summary>
/// MonthWeek 50px level renderer for TimelineView composition architecture.
/// Handles zoom level: MonthWeekOptimal50px (level 30).
/// Primary Header: Month-Year ("February 2025", "March 2025")
/// Secondary Header: Week start dates ("2/17", "2/24", "3/3") - Monday dates
/// Cell Width: 56px week cells (8px day width × 7 days/week)
/// Includes union expansion for complete header rendering at timeline edges.
/// </summary>
public class MonthWeek50pxRenderer : BaseTimelineRenderer
{
    /// <summary>
    /// Constructor for MonthWeek 50px renderer with dependency injection.
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
        ValidateRenderer();
        Logger.LogDebugInfo($"MonthWeek50pxRenderer initialized - Range: {startDate} to {endDate} (union expansion will be applied by base class)");
    }

    /// <summary>
    /// Calculates header boundaries with union expansion for complete month coverage.
    /// Extends timeline range to ensure headers are not truncated at edges.
    /// MonthWeek pattern: Extend to month boundaries (first of month to last of month).
    /// </summary>
    /// <returns>Expanded start and end dates for complete header rendering</returns>
    protected override (DateTime expandedStart, DateTime expandedEnd) CalculateHeaderBoundaries()
    {
        // For MonthWeek pattern, extend to complete month boundaries
        var expandedStart = SVGRenderingHelpers.GetMonthStart(StartDate);  // Find first day of month containing StartDate
        var expandedEnd = SVGRenderingHelpers.GetMonthEnd(EndDate);        // Find last day of month containing EndDate

        Logger.LogDebugInfo($"MonthWeek50px union expansion - Original: {StartDate} to {EndDate}, Expanded: {expandedStart} to {expandedEnd}");

        return (expandedStart, expandedEnd);
    }

    /// <summary>
    /// Renders the primary header with month-year displays for MonthWeek50px.
    /// Shows month boundaries like "February 2025", "March 2025" for each month in the timeline.
    /// </summary>
    /// <returns>SVG markup for primary header</returns>
    protected override string RenderPrimaryHeader()
    {
        var monthPeriods = GenerateMonthWeek50pxMonthPeriods();
        var headerElements = new List<string>();

        foreach (var period in monthPeriods)
        {
            // Create background rectangle for the month
            var rect = SVGRenderingHelpers.CreateSVGRect(
                period.XPosition,
                0,
                period.Width,
                HeaderMonthHeight,
                "svg-monthweek-50px-cell-primary"
            );

            // Create centered text label for the month-year
            var textX = period.XPosition + (period.Width / 2);
            var textY = HeaderMonthHeight / 2;
            var textClass = SVGRenderingHelpers.GetHeaderTextClass(ZoomLevel, isPrimary: true);
            var text = SVGRenderingHelpers.CreateSVGText(
                textX,
                textY,
                period.Label,
                textClass
            );

            headerElements.Add(rect);
            headerElements.Add(text);
        }

        return $@"
            <!-- Primary Header: Month-Year (MonthWeek50px) -->
            <g class=""monthweek-50px-primary-header"">
                {string.Join("\n                ", headerElements)}
            </g>";
    }

    /// <summary>
    /// Renders the secondary header with week start dates for MonthWeek50px.
    /// Shows Monday dates for each week like "2/17", "2/24", "3/3".
    /// </summary>
    /// <returns>SVG markup for secondary header</returns>
    protected override string RenderSecondaryHeader()
    {
        var weekPeriods = GenerateMonthWeek50pxWeekPeriods();
        var headerElements = new List<string>();

        foreach (var period in weekPeriods)
        {
            // Create background rectangle for the week
            var rect = SVGRenderingHelpers.CreateSVGRect(
                period.XPosition,
                HeaderMonthHeight,
                period.Width,
                HeaderDayHeight,
                "svg-monthweek-50px-cell-secondary"
            );

            // Create centered text label for the week start date
            var textX = period.XPosition + (period.Width / 2);
            var textY = HeaderMonthHeight + (HeaderDayHeight / 2);
            var textClass = SVGRenderingHelpers.GetHeaderTextClass(ZoomLevel, isPrimary: false);
            var text = SVGRenderingHelpers.CreateSVGText(
                textX,
                textY,
                period.Label,
                textClass
            );

            headerElements.Add(rect);
            headerElements.Add(text);
        }

        return $@"
            <!-- Secondary Header: Week Start Dates (MonthWeek50px) -->
            <g class=""monthweek-50px-secondary-header"">
                {string.Join("\n                ", headerElements)}
            </g>";
    }

    /// <summary>
    /// Gets a human-readable description of this renderer for logging and debugging.
    /// </summary>
    /// <returns>Renderer description</returns>
    protected override string GetRendererDescription() => "MonthWeek 50px";

    /// <summary>
    /// Gets the CSS class prefix for this renderer's styling.
    /// </summary>
    /// <returns>CSS class prefix</returns>
    protected override string GetCSSClass() => "monthweek-50px";

    // === PERIOD GENERATION METHODS ===

    /// <summary>
    /// Generates month periods for MonthWeek50px timeline range.
    /// Each period represents one month with start/end dates, width, and formatted label.
    /// </summary>
    /// <returns>List of HeaderPeriod objects representing months</returns>
    private List<HeaderPeriod> GenerateMonthWeek50pxMonthPeriods()
    {
        var periods = new List<HeaderPeriod>();

        // Start from the first day of the month containing StartDate (already expanded to month boundary)
        var current = new DateTime(StartDate.Year, StartDate.Month, 1);

        // Generate month periods until we cover the entire expanded timeline
        while (current <= EndDate)
        {
            var monthStart = current;
            var monthEnd = current.AddMonths(1).AddDays(-1); // Last day of the month

            // Calculate X position and width for this month
            var xPosition = SVGRenderingHelpers.DayToSVGX(monthStart, StartDate, DayWidth);
            var daysInMonth = (monthEnd - monthStart).Days + 1;
            var width = daysInMonth * DayWidth;

            var period = new HeaderPeriod
            {
                Start = monthStart,
                End = monthEnd,
                XPosition = xPosition,
                Width = width,
                Level = HeaderLevel.Primary,
                Label = FormatMonthWeek50pxMonthYear(monthStart)
            };

            periods.Add(period);

            // Move to first day of next month
            current = current.AddMonths(1);
        }

        return periods;
    }

    /// <summary>
    /// Generates week periods for MonthWeek50px secondary header.
    /// Each period represents one week starting on Monday.
    /// </summary>
    /// <returns>List of HeaderPeriod objects representing weeks</returns>
    private List<HeaderPeriod> GenerateMonthWeek50pxWeekPeriods()
    {
        var periods = new List<HeaderPeriod>();

        // Find the Monday of the week containing StartDate
        var current = StartDate;
        while (current.DayOfWeek != DayOfWeek.Monday)
        {
            current = current.AddDays(-1);
        }

        // Generate week periods until we cover the entire expanded timeline
        while (current <= EndDate)
        {
            var weekStart = current;
            var weekEnd = current.AddDays(6); // Sunday of the same week

            // Calculate the visible portion of this week within our expanded timeline
            var visibleStart = weekStart < StartDate ? StartDate : weekStart;
            var visibleEnd = weekEnd > EndDate ? EndDate : weekEnd;

            // Only add if there's a visible portion
            if (visibleStart <= visibleEnd)
            {
                var period = new HeaderPeriod
                {
                    Start = visibleStart,
                    End = visibleEnd,
                    XPosition = SVGRenderingHelpers.DayToSVGX(visibleStart, StartDate, DayWidth),
                    Width = (visibleEnd - visibleStart).Days * DayWidth + DayWidth, // +1 day for inclusive end
                    Level = HeaderLevel.Secondary,
                    Label = FormatMonthWeek50pxWeekStart(weekStart)
                };

                periods.Add(period);
            }

            // Move to next Monday
            current = current.AddDays(7);
        }

        return periods;
    }

    // === FORMATTING METHODS ===

    /// <summary>
    /// Formats a month-year for MonthWeek50px level display.
    /// Format for 56px week cells with full month names.
    /// </summary>
    /// <param name="date">The date to format</param>
    /// <returns>Formatted month-year string</returns>
    private string FormatMonthWeek50pxMonthYear(DateTime date)
    {
        try
        {
            // Full month name with year for 56px week cells
            return $"{date:MMMM yyyy}";
        }
        catch (Exception ex)
        {
            Logger.LogError($"Error formatting MonthWeek50px month-year for {date:yyyy-MM-dd}: {ex.Message}");
            return $"{date:MMM yyyy}";
        }
    }

    /// <summary>
    /// Formats a week start date for MonthWeek50px level secondary header.
    /// Short format for week start dates (Monday dates).
    /// </summary>
    /// <param name="weekStart">The Monday of the week</param>
    /// <returns>Formatted week start date string</returns>
    private string FormatMonthWeek50pxWeekStart(DateTime weekStart)
    {
        try
        {
            // Short format for week starts: "2/17", "3/3" etc.
            return $"{weekStart.Month}/{weekStart.Day}";
        }
        catch (Exception ex)
        {
            Logger.LogError($"Error formatting MonthWeek50px week start for {weekStart:yyyy-MM-dd}: {ex.Message}");
            return weekStart.Day.ToString();
        }
    }
}
