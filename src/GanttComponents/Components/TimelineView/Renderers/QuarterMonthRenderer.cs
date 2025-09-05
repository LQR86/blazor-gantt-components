using GanttComponents.Models;
using GanttComponents.Services;

namespace GanttComponents.Components.TimelineView.Renderers;

/// <summary>
/// QuarterMonth level renderer for TimelineView composition architecture.
/// Handles custom quarter-month pattern with integral day width validation.
/// Primary Header: Quarter ranges ("Q1 2025", "Q2 2025")
/// Secondary Header: Month names ("Jan", "Feb", "Mar")
/// Optimized for quarterly planning with monthly breakdown detail.
/// Includes union expansion for complete header rendering at timeline edges.
/// </summary>
public class QuarterMonthRenderer : BaseTimelineRenderer
{
    /// <summary>
    /// Constructor for QuarterMonth template renderer with dependency injection.
    /// Uses template-based approach: 30px per month with 4.0x max zoom.
    /// Template unit: 1 month (≈30 days) = 30px base width.
    /// </summary>
    public QuarterMonthRenderer(
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
        ValidateRenderer();
    }

    /// <summary>
    /// Renders the complete headers for QuarterMonth 60px view.
    /// Primary Header: Quarter ranges with year context
    /// Secondary Header: Month abbreviations with perfect 60px cells
    /// Uses template-unit padding from base class.
    /// </summary>
    /// <returns>SVG markup for primary header</returns>
    protected override string RenderPrimaryHeader()
    {
        try
        {
            // Use expanded boundaries calculated by base class template-unit padding
            return RenderQuarterHeader(StartDate, EndDate);
        }
        catch (Exception ex)
        {
            Logger.LogError("Failed to render QuarterMonth60px primary header", ex);
            return $@"<text x=""10"" y=""20"" class=""error"">Quarter Header Error</text>";
        }
    }

    /// <summary>
    /// Renders the secondary header with month names.
    /// Uses template-unit padding from base class.
    /// </summary>
    /// <returns>SVG markup for secondary header</returns>
    protected override string RenderSecondaryHeader()
    {
        try
        {
            // Use expanded boundaries calculated by base class template-unit padding
            return RenderMonthHeader(StartDate, EndDate);
        }
        catch (Exception ex)
        {
            Logger.LogError("Failed to render QuarterMonth60px secondary header", ex);
            return $@"<text x=""10"" y=""40"" class=""error"">Month Header Error</text>";
        }
    }

    /// <summary>
    /// Gets a human-readable description of this renderer for logging and debugging.
    /// </summary>
    /// <returns>Renderer description</returns>
    protected override string GetRendererDescription()
    {
        return "QuarterMonth 60px";
    }

    /// <summary>
    /// Gets the CSS class prefix for this renderer's styling.
    /// </summary>
    /// <returns>CSS class prefix</returns>
    protected override string GetCSSClass()
    {
        return "quarter-month-60px";
    }

    // === HEADER RENDERING METHODS ===

    /// <summary>
    /// Renders the primary header with quarter ranges.
    /// </summary>
    /// <param name="start">Expanded start date</param>
    /// <param name="end">Expanded end date</param>
    /// <returns>SVG markup for quarter header</returns>
    private string RenderQuarterHeader(DateTime start, DateTime end)
    {
        var svg = new System.Text.StringBuilder();
        var currentDate = start;

        while (currentDate <= end)
        {
            var quarterBounds = BoundaryCalculationHelpers.GetQuarterBoundaries(currentDate, currentDate);
            var quarterStart = quarterBounds.start;
            var quarterEnd = quarterBounds.end;

            // Quarter display: "Q1 2025", "Q2 2025", etc.
            var quarter = (quarterStart.Month - 1) / 3 + 1;
            var quarterText = $"Q{quarter} {quarterStart.Year}";

            // SoC BENEFIT: Renderer focuses on WHAT to show, base class handles HOW to position
            svg.Append(CreateValidatedHeaderCell(
                quarterStart, quarterEnd, 0, HeaderMonthHeight,
                quarterText, GetCSSClass() + "-quarter", GetCSSClass() + "-quarter-text"));

            currentDate = quarterEnd.AddDays(1);
        }

        return svg.ToString();
    }

    /// <summary>
    /// Renders the secondary header with month names.
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
            var monthStart = new DateTime(currentDate.Year, currentDate.Month, 1);
            var monthEnd = monthStart.AddMonths(1).AddDays(-1);

            // Month display: "Jan", "Feb", "Mar", etc.
            var monthText = monthStart.ToString("MMM");

            // SoC BENEFIT: Clean, focused code - no coordinate calculations cluttering business logic
            svg.Append(CreateValidatedHeaderCell(
                monthStart, monthEnd, HeaderMonthHeight, HeaderDayHeight,
                monthText, GetCSSClass() + "-month", GetCSSClass() + "-month-text"));

            currentDate = monthEnd.AddDays(1);
        }

        return svg.ToString();
    }
}
