using GanttComponents.Models;
using GanttComponents.Services;

namespace GanttComponents.Components.TimelineView.Renderers;

/// <summary>
/// YearQuarter 90px level renderer for TimelineView composition architecture.
/// Handles custom year-quarter pattern with integral day width validation.
/// Primary Header: Year ranges ("2025", "2026")
/// Secondary Header: Quarter labels ("Q1", "Q2", "Q3", "Q4")
/// Cell Width: 90px quarter cells with 1.0px integral day width (1.0px × 90 days = 90px)
/// Optimized for strategic long-term planning with year/quarter overview.
/// Includes union expansion for complete header rendering at timeline edges.
/// </summary>
public class YearQuarter90pxRenderer : BaseTimelineRenderer
{
    /// <summary>
    /// Constructor for YearQuarter template renderer with dependency injection.
    /// Uses template-based approach: 24px per quarter with 4.0x max zoom.
    /// Template unit: 1 quarter (90 days) = 24px base width.
    /// </summary>
    public YearQuarter90pxRenderer(
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
        ValidateRenderer();
    }

    /// <summary>
    /// Renders the primary header with year ranges.
    /// Uses template-unit padding from base class.
    /// </summary>
    /// <returns>SVG markup for primary header</returns>
    protected override string RenderPrimaryHeader()
    {
        try
        {
            // Use expanded boundaries calculated by base class template-unit padding
            return RenderYearHeader(StartDate, EndDate);
        }
        catch (Exception ex)
        {
            Logger.LogError("Failed to render YearQuarter90px primary header", ex);
            return $@"<text x=""10"" y=""20"" class=""error"">Year Header Error</text>";
        }
    }

    /// <summary>
    /// Renders the secondary header with quarter labels.
    /// Uses template-unit padding from base class.
    /// </summary>
    /// <returns>SVG markup for secondary header</returns>
    protected override string RenderSecondaryHeader()
    {
        try
        {
            // Use expanded boundaries calculated by base class template-unit padding
            return RenderQuarterHeader(StartDate, EndDate);
        }
        catch (Exception ex)
        {
            Logger.LogError("Failed to render YearQuarter90px secondary header", ex);
            return $@"<text x=""10"" y=""40"" class=""error"">Quarter Header Error</text>";
        }
    }

    /// <summary>
    /// Gets a human-readable description of this renderer for logging and debugging.
    /// </summary>
    /// <returns>Renderer description</returns>
    protected override string GetRendererDescription()
    {
        return "YearQuarter 90px";
    }

    /// <summary>
    /// Gets the CSS class prefix for this renderer's styling.
    /// </summary>
    /// <returns>CSS class prefix</returns>
    protected override string GetCSSClass()
    {
        return "year-quarter-90px";
    }

    // === HEADER RENDERING METHODS ===

    /// <summary>
    /// Renders the primary header with year labels.
    /// </summary>
    /// <param name="start">Expanded start date</param>
    /// <param name="end">Expanded end date</param>
    /// <returns>SVG markup for year header</returns>
    private string RenderYearHeader(DateTime start, DateTime end)
    {
        var svg = new System.Text.StringBuilder();
        var currentYear = start.Year;

        while (currentYear <= end.Year)
        {
            var yearStart = new DateTime(currentYear, 1, 1);
            var yearEnd = new DateTime(currentYear, 12, 31);

            // Year display: "2025", "2026", etc.
            var yearText = currentYear.ToString();

            // SoC BENEFIT: Renderer focuses on WHAT to show, base class handles HOW to position
            svg.Append(CreateValidatedHeaderCell(
                yearStart, yearEnd, 0, HeaderMonthHeight,
                yearText, GetCSSClass() + "-year", GetCSSClass() + "-year-text"));

            currentYear++;
        }

        return svg.ToString();
    }

    /// <summary>
    /// Renders the secondary header with quarter labels.
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

            // Quarter display: "Q1", "Q2", "Q3", "Q4"
            var quarter = (quarterStart.Month - 1) / 3 + 1;
            var quarterText = $"Q{quarter}";

            // SoC BENEFIT: Clean, focused code - no coordinate calculations cluttering business logic
            svg.Append(CreateValidatedHeaderCell(
                quarterStart, quarterEnd, HeaderMonthHeight, HeaderDayHeight,
                quarterText, GetCSSClass() + "-quarter", GetCSSClass() + "-quarter-text"));

            currentDate = quarterEnd.AddDays(1);
        }

        return svg.ToString();
    }
}
