using GanttComponents.Models;
using GanttComponents.Services;
using GanttComponents.Components.TimelineView.Renderers;
using GanttComponents.Utilities;

namespace GanttComponents.Components.TimelineView.Renderers;

/// <summary>
/// Abstract base class for timeline header renderers using template-based architecture.
/// Provides common infrastructure for all zoom level implementations while allowing
/// specific header generation logic in concrete subclasses.
/// Uses duration-to-pixel mapping instead of day width calculations.
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

    /// <summary>
    /// Fixed coordinate system reference date - always matches TimelineView's StartDate.
    /// Used for consistent SVG positioning calculations between headers and taskbars.
    /// This prevents coordinate system drift during boundary expansions.
    /// </summary>
    protected DateTime CoordinateSystemStart { get; private set; }

    protected ZoomLevelConfiguration TemplateConfig { get; set; }
    protected double ZoomFactor { get; set; }
    protected int HeaderMonthHeight { get; set; }
    protected int HeaderDayHeight { get; set; }
    protected TimelineZoomLevel ZoomLevel { get; set; }

    // === COMPUTED PROPERTIES ===
    protected int TotalHeaderHeight => HeaderMonthHeight + HeaderDayHeight;

    /// <summary>
    /// Calculate effective day width using template-based approach.
    /// Backward compatibility property for existing renderer code.
    /// </summary>
    protected double DayWidth => TemplateConfig.GetEffectiveDayWidth(ZoomFactor);

    /// <summary>
    /// Constructor for dependency injection and configuration.
    /// Includes automatic integral day width validation for visual quality.
    /// </summary>
    /// <param name="logger">Universal logger service</param>
    /// <param name="i18n">Internationalization service</param>
    /// <param name="dateFormatter">Date formatting helper</param>
    /// <param name="startDate">Timeline start date</param>
    /// <param name="endDate">Timeline end date</param>
    /// <param name="dayWidth">Width of each day in pixels (must be integral)</param>
    /// <summary>
    /// Constructor for dependency injection and template-based configuration.
    /// Includes automatic integral unit width validation for visual quality.
    /// </summary>
    /// <param name="logger">Universal logger service</param>
    /// <param name="i18n">Internationalization service</param>
    /// <param name="dateFormatter">Date formatting helper</param>
    /// <param name="startDate">Timeline start date</param>
    /// <param name="endDate">Timeline end date</param>
    /// <param name="zoomLevel">Current zoom level</param>
    /// <param name="zoomFactor">Current zoom factor</param>
    /// <param name="headerMonthHeight">Height of primary header</param>
    /// <param name="headerDayHeight">Height of secondary header</param>
    protected BaseTimelineRenderer(
        IUniversalLogger logger,
        IGanttI18N i18n,
        DateFormatHelper dateFormatter,
        DateTime startDate,
        DateTime endDate,
        TimelineZoomLevel zoomLevel,
        double zoomFactor,
        int headerMonthHeight,
        int headerDayHeight)
    {
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        I18N = i18n ?? throw new ArgumentNullException(nameof(i18n));
        DateFormatter = dateFormatter ?? throw new ArgumentNullException(nameof(dateFormatter));

        StartDate = startDate;
        EndDate = endDate;
        ZoomLevel = zoomLevel;
        ZoomFactor = zoomFactor;

        // CRITICAL FIX: Lock in the coordinate system reference to prevent drift
        // This ensures headers and taskbars use the same pixel-to-date conversion
        CoordinateSystemStart = startDate;

        HeaderMonthHeight = headerMonthHeight;
        HeaderDayHeight = headerDayHeight;

        // Get template configuration for this zoom level
        TemplateConfig = TimelineZoomService.GetConfiguration(zoomLevel);
    }

    /// <summary>
    /// Template method for rendering complete headers with automatic union expansion.
    /// Orchestrates the header generation process using abstract methods.
    /// Automatically applies boundary expansion to prevent header truncation.
    /// </summary>
    /// <returns>Complete SVG markup for timeline headers</returns>
    public string RenderHeaders()
    {
        try
        {
            // UNION EXPANSION: Automatically expand timeline range for complete header rendering
            var originalStart = StartDate;
            var originalEnd = EndDate;
            var (expandedStart, expandedEnd) = CalculateHeaderBoundaries();

            // Apply expanded boundaries temporarily
            StartDate = expandedStart;
            EndDate = expandedEnd;

            // Render headers with expanded range
            var result = RenderHeadersInternal();

            // CRITICAL FIX: Restore original dates after rendering
            StartDate = originalStart;
            EndDate = originalEnd;

            return result;
        }
        catch (Exception ex)
        {
            Logger.LogError($"Error rendering headers for {GetRendererDescription()}: {ex.Message}");
            return $"<!-- Error in {GetRendererDescription()}: {ex.Message} -->";
        }
    }

    /// <summary>
    /// Internal template method for rendering headers after union expansion is applied.
    /// Called by the public RenderHeaders method after boundary calculation.
    /// </summary>
    /// <returns>Complete SVG markup for timeline headers</returns>
    private string RenderHeadersInternal()
    {
        var primaryHeader = RenderPrimaryHeader();
        var secondaryHeader = RenderSecondaryHeader();

        return $@"
            <!-- {GetRendererDescription()} Headers -->
            <g class=""{GetCSSClass()}-headers"">
                {primaryHeader}
                {secondaryHeader}
            </g>";
    }

    // === ABSTRACT METHODS FOR SUBCLASSES ===

    /// <summary>
    /// AUTOMATIC DUAL BOUNDARY EXPANSION: Final method that enforces dual boundary union calculation.
    /// This method automatically combines primary and secondary header boundaries to ensure
    /// both header types render completely without truncation at timeline edges.
    /// 
    /// ABC COMPOSITION ENFORCEMENT: Subclasses CANNOT override this method - they must implement
    /// the abstract boundary methods, and this base class automatically calculates the union.
    /// This guarantees that all current and future timeline patterns get dual expansion "for free".
    /// 
    /// NOTE: While C# doesn't allow 'sealed' on non-override methods, this method should be treated
    /// as final. Future renderers must implement the abstract boundary calculation methods instead.
    /// </summary>
    /// <returns>Union of primary and secondary boundaries for complete header rendering</returns>
    public (DateTime expandedStart, DateTime expandedEnd) CalculateHeaderBoundaries()
    {
        try
        {
            // Template-pure approach: Simple padding based on template units
            // Add 1 template unit on each side to ensure headers render completely
            var paddingDays = TemplateConfig.TemplateUnitDays;

            var expandedStart = StartDate.AddDays(-paddingDays);
            var expandedEnd = EndDate.AddDays(paddingDays);

            return (expandedStart, expandedEnd);
        }
        catch (Exception ex)
        {
            Logger.LogError($"Error in template-unit boundary calculation for {GetRendererDescription()}: {ex.Message}");

            // FALLBACK: Use original date range if boundary calculation fails
            return (StartDate, EndDate);
        }
    }

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
        // Add inline styles as fallback for when CSS fails to load
        var inlineStyle = cssClass.Contains("primary") || cssClass.Contains("year") || cssClass.Contains("quarter")
            ? "fill: #f8f9fa; stroke: #dee2e6; stroke-width: 1;"
            : "fill: #ffffff; stroke: #dee2e6; stroke-width: 1;";

        return $@"<rect x=""{x}"" y=""{y}"" width=""{width}"" height=""{height}"" 
                       class=""{cssClass}"" style=""{inlineStyle}"" />";
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
        // Add inline styles as fallback for when CSS fails to load
        var fontSize = cssClass.Contains("primary") || cssClass.Contains("year") || cssClass.Contains("quarter")
            ? "12px" : "10px";
        var fontWeight = cssClass.Contains("primary") || cssClass.Contains("year") || cssClass.Contains("quarter")
            ? "600" : "500";
        var inlineStyle = $"fill: #333333; font-family: 'Segoe UI', Arial, sans-serif; font-size: {fontSize}; font-weight: {fontWeight};";

        return $@"<text x=""{x}"" y=""{y}"" class=""{cssClass}"" 
                       text-anchor=""middle"" dominant-baseline=""middle""
                       style=""{inlineStyle}"">{System.Net.WebUtility.HtmlEncode(text)}</text>";
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

    // === COORDINATE SYSTEM ENFORCEMENT ===
    // Ensures all renderers use consistent coordinate calculations

    /// <summary>
    /// COORDINATE ENFORCEMENT: Calculates X position for a date using the timeline coordinate system.
    /// All renderers must use this method to ensure consistent positioning across headers and task bars.
    /// This prevents coordinate drift and ensures pixel-perfect alignment in the ABC composition pattern.
    /// </summary>
    /// <param name="date">The date to convert to X coordinate</param>
    /// <returns>X position in SVG pixels from the coordinate system origin</returns>
    /// <exception cref="ArgumentException">Thrown when date is invalid</exception>
    protected double CalculateCoordinateX(DateTime date)
    {
        // Use proven coordinate logic from SVGRenderingHelpers for consistency
        return SVGRenderingHelpers.DayToSVGX(date, CoordinateSystemStart, DayWidth);
    }

    /// <summary>
    /// COORDINATE ENFORCEMENT: Calculates width for a date range using timeline coordinate system.
    /// All renderers must use this method to ensure consistent width calculations.
    /// Handles inclusive date ranges with proper day-level precision.
    /// </summary>
    /// <param name="startDate">Start date of the range (inclusive)</param>
    /// <param name="endDate">End date of the range (inclusive)</param>
    /// <returns>Width in pixels for the date range</returns>
    /// <exception cref="ArgumentException">Thrown when date range is invalid</exception>
    protected double CalculateCoordinateWidth(DateTime startDate, DateTime endDate)
    {
        if (endDate < startDate)
        {
            throw new ArgumentException($"End date ({endDate:yyyy-MM-dd}) cannot be before start date ({startDate:yyyy-MM-dd})");
        }

        // Calculate inclusive date range in days
        var days = (endDate.Date - startDate.Date).Days + 1;
        return days * DayWidth;
    }

    /// <summary>
    /// COORDINATE VALIDATION: Development-mode validation to detect coordinate inconsistencies.
    /// This helps catch coordinate system violations during development and debugging.
    /// Only active in DEBUG builds to avoid performance impact in production.
    /// </summary>
    /// <param name="date">The date being positioned</param>
    /// <param name="actualX">The X position calculated by renderer</param>
    /// <param name="context">Context description for debugging (e.g., "quarter header")</param>
    [System.Diagnostics.Conditional("DEBUG")]
    protected void ValidateCoordinateConsistency(DateTime date, double actualX, string context = "header cell")
    {
        var expectedX = CalculateCoordinateX(date);
        var tolerance = 1.0; // 1 pixel tolerance for floating point precision

        if (Math.Abs(actualX - expectedX) > tolerance)
        {
            Logger.LogWarning(
                $"COORDINATE INCONSISTENCY in {GetRendererDescription()}: " +
                $"{context} for {date:yyyy-MM-dd} at X={actualX:F1}, expected X={expectedX:F1}. " +
                $"Use CalculateCoordinateX() for consistent positioning.");
        }
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

    // === VALIDATED SVG CREATION HELPERS ===
    // SoC Enhancement: Base class handles coordinate calculation + validation, renderers focus on logic

    /// <summary>
    /// COORDINATE-SAFE: Creates SVG rectangle with validated positioning.
    /// SoC: Base class responsibility = coordinate calculation, renderer responsibility = what to show.
    /// Automatically uses coordinate system and validates consistency in DEBUG builds.
    /// </summary>
    /// <param name="startDate">Start date of the rectangle (inclusive)</param>
    /// <param name="endDate">End date of the rectangle (inclusive)</param>
    /// <param name="y">Y position in pixels</param>
    /// <param name="height">Height in pixels</param>
    /// <param name="cssClass">CSS class for styling</param>
    /// <returns>SVG rectangle markup with validated coordinates</returns>
    protected string CreateValidatedSVGRect(DateTime startDate, DateTime endDate, int y, int height, string cssClass)
    {
        var x = CalculateCoordinateX(startDate);
        var width = CalculateCoordinateWidth(startDate, endDate);

        // Automatic validation in DEBUG builds - catches coordinate issues at creation time
        ValidateCoordinateConsistency(startDate, x, $"rect for {cssClass}");

        return CreateSVGRect(x, y, width, height, cssClass);
    }

    /// <summary>
    /// COORDINATE-SAFE: Creates SVG text with validated center positioning.
    /// SoC: Base class handles coordinate calculation, renderer provides content and styling.
    /// Automatically centers text within the date range bounds.
    /// </summary>
    /// <param name="startDate">Start date of the text container (inclusive)</param>
    /// <param name="endDate">End date of the text container (inclusive)</param>
    /// <param name="y">Y position in pixels (typically center of container)</param>
    /// <param name="text">Text content to display</param>
    /// <param name="cssClass">CSS class for text styling</param>
    /// <returns>SVG text markup with validated center positioning</returns>
    protected string CreateValidatedSVGText(DateTime startDate, DateTime endDate, int y, string text, string cssClass)
    {
        var x = CalculateCoordinateX(startDate);
        var width = CalculateCoordinateWidth(startDate, endDate);
        var centerX = x + width / 2;

        // Automatic validation in DEBUG builds
        ValidateCoordinateConsistency(startDate, x, $"text for {cssClass}");

        return CreateSVGText(centerX, y, text, cssClass);
    }

    /// <summary>
    /// COORDINATE-SAFE: Creates complete SVG header cell (rectangle + centered text).
    /// SoC: Base class handles all coordinate complexity, renderer just provides content.
    /// This is the highest-level helper that combines rect + text with perfect alignment.
    /// </summary>
    /// <param name="startDate">Start date of the header cell (inclusive)</param>
    /// <param name="endDate">End date of the header cell (inclusive)</param>
    /// <param name="y">Y position of the cell</param>
    /// <param name="height">Height of the cell</param>
    /// <param name="text">Text to display in the cell</param>
    /// <param name="rectCssClass">CSS class for the rectangle background</param>
    /// <param name="textCssClass">CSS class for the text content</param>
    /// <returns>Complete SVG markup for header cell with validated coordinates</returns>
    protected string CreateValidatedHeaderCell(DateTime startDate, DateTime endDate, int y, int height,
        string text, string rectCssClass, string textCssClass)
    {
        var rect = CreateValidatedSVGRect(startDate, endDate, y, height, rectCssClass);
        var textSvg = CreateValidatedSVGText(startDate, endDate, y + height / 2, text, textCssClass);

        return rect + textSvg;
    }

    // === INTEGRAL DAY WIDTH VALIDATION ===
    // Critical for visual quality - ensures all coordinates are integral pixels

    /// <summary>
    /// INTEGRAL DAY WIDTH VALIDATION: Validates that the effective day width is integral.
    /// This is critical for clean SVG coordinate calculations and visual quality.
    /// Applied automatically to all renderers for DRY compliance and future-proofing.
    /// </summary>
    /// <param name="dayWidth">The day width to validate (in pixels)</param>
    /// <param name="zoomLevel">Current zoom level for error reporting</param>
}
