using GanttComponents.Models;
using GanttComponents.Services;

namespace GanttComponents.Components.TimelineView;

/// <summary>
/// Abstract base class for timeline header renderers using template method pattern.
/// Provides common infrastructure for all zoom level implementations while allowing
/// specific header generation logic in concrete subclasses.
/// </summary>
public abstract class BaseTimelineRenderer
{
    // === DEPENDENCY INJECTION ===
    protected IUniversalLogger Logger { get; }
    protected IGanttI18N I18N { get; }
    protected DateFormatHelper DateFormatter { get; }

    // === TIMELINE PROPERTIES ===
    protected DateTime StartDate { get; set; }
    protected DateTime EndDate { get; set; }
    protected double DayWidth { get; set; }
    protected int HeaderMonthHeight { get; set; }
    protected int HeaderDayHeight { get; set; }
    protected TimelineZoomLevel ZoomLevel { get; set; }
    protected double ZoomFactor { get; set; }

    // === COMPUTED PROPERTIES ===
    protected int TotalHeaderHeight => HeaderMonthHeight + HeaderDayHeight;

    /// <summary>
    /// Constructor for dependency injection and configuration.
    /// </summary>
    /// <param name="logger">Universal logger service</param>
    /// <param name="i18n">Internationalization service</param>
    /// <param name="dateFormatter">Date formatting helper</param>
    /// <param name="startDate">Timeline start date</param>
    /// <param name="endDate">Timeline end date</param>
    /// <param name="dayWidth">Width of each day in pixels</param>
    /// <param name="headerMonthHeight">Height of primary header</param>
    /// <param name="headerDayHeight">Height of secondary header</param>
    /// <param name="zoomLevel">Current zoom level</param>
    /// <param name="zoomFactor">Current zoom factor</param>
    protected BaseTimelineRenderer(
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
    {
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        I18N = i18n ?? throw new ArgumentNullException(nameof(i18n));
        DateFormatter = dateFormatter ?? throw new ArgumentNullException(nameof(dateFormatter));
        
        StartDate = startDate;
        EndDate = endDate;
        DayWidth = dayWidth;
        HeaderMonthHeight = headerMonthHeight;
        HeaderDayHeight = headerDayHeight;
        ZoomLevel = zoomLevel;
        ZoomFactor = zoomFactor;
    }

    /// <summary>
    /// Template method for rendering complete headers.
    /// Orchestrates the header generation process using abstract methods.
    /// </summary>
    /// <returns>Complete SVG markup for timeline headers</returns>
    public string RenderHeaders()
    {
        try
        {
            Logger.LogDebugInfo($"Rendering headers - ZoomLevel: {ZoomLevel}, StartDate: {StartDate}, EndDate: {EndDate}, DayWidth: {DayWidth}");

            var primaryHeader = RenderPrimaryHeader();
            var secondaryHeader = RenderSecondaryHeader();

            return $@"
                <!-- {GetRendererDescription()} Headers -->
                <g class=""{GetCSSClass()}-headers"">
                    {primaryHeader}
                    {secondaryHeader}
                </g>";
        }
        catch (Exception ex)
        {
            Logger.LogError($"Error rendering headers for {GetRendererDescription()}: {ex.Message}");
            return $"<!-- Error in {GetRendererDescription()}: {ex.Message} -->";
        }
    }

    // === ABSTRACT METHODS FOR SUBCLASSES ===

    /// <summary>
    /// Renders the primary (top) header for the specific zoom level.
    /// Each zoom level defines its own primary header structure.
    /// </summary>
    /// <returns>SVG markup for primary header</returns>
    protected abstract string RenderPrimaryHeader();

    /// <summary>
    /// Renders the secondary (bottom) header for the specific zoom level.
    /// Each zoom level defines its own secondary header structure.
    /// </summary>
    /// <returns>SVG markup for secondary header</returns>
    protected abstract string RenderSecondaryHeader();

    /// <summary>
    /// Gets a human-readable description of this renderer for logging and debugging.
    /// </summary>
    /// <returns>Renderer description (e.g., "WeekDay 50px")</returns>
    protected abstract string GetRendererDescription();

    /// <summary>
    /// Gets the CSS class prefix for this renderer's styling.
    /// </summary>
    /// <returns>CSS class prefix (e.g., "weekday-50px")</returns>
    protected abstract string GetCSSClass();

    // === SHARED UTILITY METHODS ===

    /// <summary>
    /// Creates an SVG rectangle element with specified properties.
    /// </summary>
    /// <param name="x">X position</param>
    /// <param name="y">Y position</param>
    /// <param name="width">Width</param>
    /// <param name="height">Height</param>
    /// <param name="cssClass">CSS class name</param>
    /// <returns>SVG rect element</returns>
    protected string CreateSVGRect(double x, double y, double width, double height, string cssClass)
    {
        return $@"<rect x=""{x}"" y=""{y}"" width=""{width}"" height=""{height}"" class=""{cssClass}"" />";
    }

    /// <summary>
    /// Creates an SVG text element with specified properties.
    /// </summary>
    /// <param name="x">X position</param>
    /// <param name="y">Y position</param>
    /// <param name="text">Text content</param>
    /// <param name="cssClass">CSS class name</param>
    /// <returns>SVG text element</returns>
    protected string CreateSVGText(double x, double y, string text, string cssClass)
    {
        return $@"<text x=""{x}"" y=""{y}"" class=""{cssClass}"" text-anchor=""middle"" dominant-baseline=""middle"">{text}</text>";
    }

    /// <summary>
    /// Gets the appropriate CSS class for header text based on position.
    /// </summary>
    /// <param name="isPrimary">True for primary header, false for secondary</param>
    /// <returns>CSS class name for header text</returns>
    protected string GetHeaderTextClass(bool isPrimary)
    {
        return isPrimary ? "svg-header-text-primary" : "svg-header-text-secondary";
    }

    /// <summary>
    /// Validates that all required properties are set correctly.
    /// </summary>
    protected void ValidateRenderer()
    {
        if (StartDate == default)
            throw new InvalidOperationException("StartDate must be set");
        if (EndDate == default)
            throw new InvalidOperationException("EndDate must be set");
        if (DayWidth <= 0)
            throw new InvalidOperationException("DayWidth must be positive");
        if (HeaderMonthHeight <= 0)
            throw new InvalidOperationException("HeaderMonthHeight must be positive");
        if (HeaderDayHeight <= 0)
            throw new InvalidOperationException("HeaderDayHeight must be positive");
    }
}
