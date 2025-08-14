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
    /// Constructor for YearQuarter 90px renderer with dependency injection.
    /// Uses calculated day width for flexible year/quarter cell sizing.
    /// Union expansion is handled automatically by the base class.
    /// </summary>
    public YearQuarter90pxRenderer(
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
    }

    /// <summary>
    /// Calculate boundaries for primary header rendering (Year ranges).
    /// YearQuarter pattern: Year headers need year boundaries for complete year coverage.
    /// </summary>
    /// <returns>Year boundary dates for primary year header complete rendering</returns>
    protected override (DateTime start, DateTime end) CalculatePrimaryBoundaries()
    {
        var yearBounds = BoundaryCalculationHelpers.GetYearBoundaries(StartDate, EndDate);
        return yearBounds;
    }

    /// <summary>
    /// Calculate boundaries for secondary header rendering (Quarter labels).
    /// YearQuarter pattern: Quarter headers need quarter boundaries for precise quarter alignment.
    /// Since quarters fit perfectly within years, this maintains natural alignment.
    /// </summary>
    /// <returns>Quarter boundary dates for secondary quarter header complete rendering</returns>
    protected override (DateTime start, DateTime end) CalculateSecondaryBoundaries()
    {
        var quarterBounds = BoundaryCalculationHelpers.GetQuarterBoundaries(StartDate, EndDate);
        return quarterBounds;
    }

    /// <summary>
    /// Renders the primary header with year ranges.
    /// Uses automatic dual boundary expansion from base class.
    /// </summary>
    /// <returns>SVG markup for primary header</returns>
    protected override string RenderPrimaryHeader()
    {
        try
        {
            // Use expanded boundaries calculated by base class union logic
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
    /// Uses automatic dual boundary expansion from base class.
    /// </summary>
    /// <returns>SVG markup for secondary header</returns>
    protected override string RenderSecondaryHeader()
    {
        try
        {
            // Use expanded boundaries calculated by base class union logic
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

            // COORDINATE ENFORCEMENT: Use base class coordinate system
            var xPosition = CalculateCoordinateX(yearStart);
            var yearWidth = CalculateCoordinateWidth(yearStart, yearEnd);

            // Year display: "2025", "2026", etc.
            var yearText = currentYear.ToString();

            // Render year header cell
            svg.Append(CreateSVGRect(xPosition, 0, yearWidth, HeaderMonthHeight, GetCSSClass() + "-year"));
            svg.Append(CreateSVGText(xPosition + yearWidth / 2, HeaderMonthHeight / 2, yearText, GetCSSClass() + "-year-text"));

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

            // COORDINATE ENFORCEMENT: Use base class coordinate system
            var xPosition = CalculateCoordinateX(quarterStart);
            var quarterWidth = CalculateCoordinateWidth(quarterStart, quarterEnd);

            // Quarter display: "Q1", "Q2", "Q3", "Q4"
            var quarter = (quarterStart.Month - 1) / 3 + 1;
            var quarterText = $"Q{quarter}";

            // Render quarter header cell
            svg.Append(CreateSVGRect(xPosition, HeaderMonthHeight, quarterWidth, HeaderDayHeight, GetCSSClass() + "-quarter"));
            svg.Append(CreateSVGText(xPosition + quarterWidth / 2, HeaderMonthHeight + HeaderDayHeight / 2, quarterText, GetCSSClass() + "-quarter-text"));

            currentDate = quarterEnd.AddDays(1);
        }

        return svg.ToString();
    }
}
