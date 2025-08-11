using GanttComponents.Models;
using GanttComponents.Services;

namespace GanttComponents.Components.TimelineView.Renderers;

/// <summary>
/// QuarterMonth 60px level renderer for TimelineView composition architecture.
/// Handles custom quarter-month pattern with integral day width validation.
/// Primary Header: Quarter ranges ("Q1 2025", "Q2 2025")
/// Secondary Header: Month names ("Jan", "Feb", "Mar")
/// Cell Width: 60px month cells with 2.0px integral day width (2.0px × 30 days = 60px)
/// Optimized for quarterly planning with monthly breakdown detail.
/// Includes union expansion for complete header rendering at timeline edges.
/// </summary>
public class QuarterMonth60pxRenderer : BaseTimelineRenderer
{
    /// <summary>
    /// Constructor for QuarterMonth 60px renderer with dependency injection.
    /// Uses integral 2.0px day width for perfect SVG coordinate calculations.
    /// Union expansion is handled automatically by the base class.
    /// </summary>
    public QuarterMonth60pxRenderer(
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
               2.0, // INTEGRAL DAY WIDTH: 2.0px day width = 60px month cells (2.0px × 30 days)
               headerMonthHeight, headerDayHeight, zoomLevel, zoomFactor)
    {
        ValidateRenderer();
    }

    /// <summary>
    /// Calculate boundaries for primary header rendering (Quarter ranges).
    /// QuarterMonth pattern: Quarter headers need quarter boundaries for complete quarter coverage.
    /// </summary>
    /// <returns>Quarter boundary dates for primary quarter header complete rendering</returns>
    protected override (DateTime start, DateTime end) CalculatePrimaryBoundaries()
    {
        var quarterBounds = BoundaryCalculationHelpers.GetQuarterBoundaries(StartDate, EndDate);
        return quarterBounds;
    }

    /// <summary>
    /// Calculate boundaries for secondary header rendering (Month names).
    /// QuarterMonth pattern: Month headers need month boundaries for precise month alignment.
    /// This ensures months render completely even when timeline spans partial quarters.
    /// </summary>
    /// <returns>Month boundary dates for secondary month header complete rendering</returns>
    protected override (DateTime start, DateTime end) CalculateSecondaryBoundaries()
    {
        var monthBounds = BoundaryCalculationHelpers.GetMonthBoundaries(StartDate, EndDate);
        return monthBounds;
    }

    /// <summary>
    /// Renders the complete headers for QuarterMonth 60px view.
    /// Primary Header: Quarter ranges with year context
    /// Secondary Header: Month abbreviations with perfect 60px cells
    /// Uses automatic dual boundary expansion from base class.
    /// </summary>
    /// <returns>SVG markup for primary header</returns>
    protected override string RenderPrimaryHeader()
    {
        try
        {
            // Use expanded boundaries calculated by base class union logic
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
    /// Uses automatic dual boundary expansion from base class.
    /// </summary>
    /// <returns>SVG markup for secondary header</returns>
    protected override string RenderSecondaryHeader()
    {
        try
        {
            // Use expanded boundaries calculated by base class union logic
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
        double xPosition = 0;

        while (currentDate <= end)
        {
            var quarterBounds = BoundaryCalculationHelpers.GetQuarterBoundaries(currentDate, currentDate);
            var quarterStart = quarterBounds.start;
            var quarterEnd = quarterBounds.end;

            // Calculate quarter width in pixels
            var quarterDays = (quarterEnd - quarterStart).Days + 1;
            var quarterWidth = quarterDays * DayWidth;

            // Quarter display: "Q1 2025", "Q2 2025", etc.
            var quarter = (quarterStart.Month - 1) / 3 + 1;
            var quarterText = $"Q{quarter} {quarterStart.Year}";

            // Render quarter header cell
            svg.Append(CreateSVGRect(xPosition, 0, quarterWidth, HeaderMonthHeight, GetCSSClass() + "-quarter"));
            svg.Append(CreateSVGText(xPosition + quarterWidth / 2, HeaderMonthHeight / 2, quarterText, GetCSSClass() + "-quarter-text"));

            xPosition += quarterWidth;
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
        double xPosition = 0;

        while (currentDate <= end)
        {
            var monthStart = new DateTime(currentDate.Year, currentDate.Month, 1);
            var monthEnd = monthStart.AddMonths(1).AddDays(-1);

            // Calculate month width (approximately 60px for 30-day months with 2.0px day width)
            var monthDays = (monthEnd - monthStart).Days + 1;
            var monthWidth = monthDays * DayWidth;

            // Month display: "Jan", "Feb", "Mar", etc.
            var monthText = monthStart.ToString("MMM");

            // Render month header cell
            svg.Append(CreateSVGRect(xPosition, HeaderMonthHeight, monthWidth, HeaderDayHeight, GetCSSClass() + "-month"));
            svg.Append(CreateSVGText(xPosition + monthWidth / 2, HeaderMonthHeight + HeaderDayHeight / 2, monthText, GetCSSClass() + "-month-text"));

            xPosition += monthWidth;
            currentDate = monthEnd.AddDays(1);
        }

        return svg.ToString();
    }
}
