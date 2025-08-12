using GanttComponents.Models;
using GanttComponents.Services;
using GanttComponents.Components.TimelineView.Renderers;
using Microsoft.AspNetCore.Components;

namespace GanttComponents.Components.TimelineView;

/// <summary>
/// TimelineView_Export component optimized for PDF export and static rendering use cases.
/// 
/// KEY FEATURES:
/// - Single unified SVG architecture (no dual container complexity)
/// - No interactive features (clicks, hovers, tooltips, scrolling)
/// - Simplified task rendering without background rows or event handlers
/// - Export-optimized CSS with print-friendly styling
/// - Perfect alignment inheritance from main TimelineView component
/// - Uses same proven header rendering via RendererFactory composition
/// 
/// WHEN TO USE:
/// - PDF export generation
/// - Static timeline screenshots  
/// - Print-friendly timeline views
/// - Any scenario where interaction is not needed
/// 
/// PERFORMANCE CHARACTERISTICS:
/// - Faster rendering (no event handlers or interaction setup)
/// - Lower memory usage (no state tracking for selection/hover)
/// - Vector-perfect output for high-quality exports
/// 
/// ARCHITECTURE:
/// - Reuses TimelineView's proven calculation and header logic
/// - Component separation approach (vs conditional rendering)
/// - Independent of interactive TimelineView - changes don't affect each other
/// </summary>
public partial class TimelineView_Export : ComponentBase
{
    // === DEPENDENCY INJECTION ===
    [Inject] private IUniversalLogger Logger { get; set; } = default!;
    [Inject] private IGanttI18N I18N { get; set; } = default!;
    [Inject] private DateFormatHelper DateFormatter { get; set; } = default!;

    // === COMPONENT PARAMETERS (SIMPLIFIED - NO INTERACTIONS) ===

    /// <summary>List of tasks to render in the timeline</summary>
    [Parameter, EditorRequired] public List<GanttTask> Tasks { get; set; } = new();

    /// <summary>Height of each task row in pixels</summary>
    [Parameter, EditorRequired] public int RowHeight { get; set; } = 32;

    /// <summary>Height of the month header row in pixels</summary>
    [Parameter, EditorRequired] public int HeaderMonthHeight { get; set; } = 32;

    /// <summary>Height of the day header row in pixels</summary>  
    [Parameter, EditorRequired] public int HeaderDayHeight { get; set; } = 24;

    /// <summary>Timeline zoom level determining day width and header grouping</summary>
    [Parameter] public TimelineZoomLevel ZoomLevel { get; set; } = TimelineZoomLevel.MonthWeekOptimal50px;

    /// <summary>Additional zoom factor applied to base day width (export typically uses 1.0)</summary>
    [Parameter] public double ZoomFactor { get; set; } = 1.0;

    // === REMOVED INTERACTIVE PARAMETERS ===
    // NO OnTaskSelected, OnTaskHovered, OnScrollChanged
    // NO SelectedTaskId, HoveredTaskId  
    // NO OnZoomLevelChanged, OnZoomFactorChanged (export is static)

    // === COMPONENT STATE (SIMPLIFIED) ===
    protected const int TaskBarHeight = 20;
    protected const int TaskBarMargin = 6;

    // === COMPONENT ID ===
    protected string ComponentId { get; set; } = Guid.NewGuid().ToString("N")[..8];

    // === COMPOSITION ARCHITECTURE (REUSED) ===
    private BaseTimelineRenderer? currentRenderer;

    // === TIMELINE PROPERTIES ===
    protected DateTime StartDate { get; set; }
    protected DateTime EndDate { get; set; }
    protected int TotalWidth { get; set; }
    protected int TotalHeight { get; set; }

    // === ZOOM CALCULATIONS (REUSED FROM INTERACTIVE) ===
    private double EffectiveDayWidth
    {
        get
        {
            var config = TimelineZoomService.GetConfiguration(ZoomLevel);
            ValidateBaseDayWidth(config.BaseDayWidth);
            var effectiveWidth = config.GetEffectiveDayWidth(ZoomFactor);
            ValidateEffectiveDayWidth(effectiveWidth);
            return effectiveWidth;
        }
    }

    protected double DayWidth => EffectiveDayWidth;
    protected int TotalHeaderHeight => HeaderMonthHeight + HeaderDayHeight;

    // === LIFECYCLE METHODS ===
    protected override void OnParametersSet()
    {
        CalculateTimelineProperties();
    }

    // === CALCULATION METHODS (SIMPLIFIED FROM INTERACTIVE) ===
    private void CalculateTimelineProperties()
    {
        // Step 1: Calculate base task date range
        if (Tasks == null || !Tasks.Any())
        {
            StartDate = DateTime.Today;
            EndDate = DateTime.Today.AddDays(30);
        }
        else
        {
            StartDate = Tasks.Min(t => t.StartDate).Date;
            EndDate = Tasks.Max(t => t.EndDate).Date;
        }

        // Step 2: Get expanded boundaries for consistent header rendering
        // This ensures export component uses same coordinate system as interactive
        var expandedBounds = GetExpandedTimelineBounds();
        StartDate = expandedBounds.start;
        EndDate = expandedBounds.end;

        // Calculate dimensions
        var totalDays = (EndDate - StartDate).Days + 1;
        TotalWidth = Math.Max(100, (int)(totalDays * DayWidth));
        TotalHeight = Math.Max(50, Tasks?.Count * RowHeight ?? 0);
    }

    /// <summary>
    /// Gets expanded timeline boundaries from renderer for consistent coordinate system.
    /// Simplified version for export - no alignment validation needed.
    /// </summary>
    private (DateTime start, DateTime end) GetExpandedTimelineBounds()
    {
        try
        {
            var tempRenderer = RendererFactory.CreateRenderer(
                ZoomLevel,
                Logger,
                I18N,
                DateFormatter,
                StartDate,
                EndDate,
                DayWidth,
                HeaderMonthHeight,
                HeaderDayHeight,
                ZoomFactor
            );

            if (tempRenderer == null)
            {
                Logger.LogWarning($"Could not create renderer for boundary calculation, using original dates");
                return (StartDate, EndDate);
            }

            var expandedBounds = tempRenderer.CalculateHeaderBoundaries();
            return (expandedBounds.expandedStart, expandedBounds.expandedEnd);
        }
        catch (Exception ex)
        {
            Logger.LogError($"Error calculating expanded timeline bounds: {ex.Message}");
            return (StartDate, EndDate);
        }
    }

    protected double DayToPixel(DateTime date)
    {
        var days = (date.Date - StartDate).TotalDays;
        return days * DayWidth;
    }

    /// <summary>
    /// Calculate task width in pixels. Simplified for export - no complex logic needed.
    /// </summary>
    protected double CalculateTaskWidth(GanttTask task)
    {
        var duration = (task.EndDate.Date - task.StartDate.Date).TotalDays + 1;
        return duration * DayWidth;
    }

    // === VALIDATION METHODS (REUSED) ===
    private void ValidateBaseDayWidth(double baseDayWidth)
    {
        if (Math.Abs(baseDayWidth - Math.Round(baseDayWidth)) > 0.001)
        {
            throw new InvalidOperationException(
                $"Base day width must be integral for proper alignment. Got: {baseDayWidth}");
        }
    }

    private void ValidateEffectiveDayWidth(double effectiveWidth)
    {
        if (Math.Abs(effectiveWidth - Math.Round(effectiveWidth)) > 0.001)
        {
            throw new InvalidOperationException(
                $"Effective day width must be integral for proper alignment. Got: {effectiveWidth}");
        }
    }

    // === HEADER RENDERING (REUSED FROM INTERACTIVE) ===
    /// <summary>
    /// Main orchestrator method that delegates to the appropriate pattern implementation.
    /// Uses composition architecture - all zoom levels use BaseTimelineRenderer.
    /// </summary>
    protected string RenderSVGHeaders()
    {
        try
        {
            // PURE COMPOSITION ARCHITECTURE: All zoom levels use BaseTimelineRenderer
            currentRenderer = RendererFactory.CreateRenderer(
                ZoomLevel,
                Logger,
                I18N,
                DateFormatter,
                StartDate,
                EndDate,
                DayWidth,
                HeaderMonthHeight,
                HeaderDayHeight,
                ZoomFactor
            );

            if (currentRenderer == null)
            {
                Logger.LogError($"RendererFactory returned null for zoom level {ZoomLevel}");
                return "<!-- Renderer creation failed -->";
            }

            var headerResult = currentRenderer.RenderHeaders();

            // Defensive programming: ensure we never return null
            if (string.IsNullOrEmpty(headerResult))
            {
                Logger.LogWarning($"Renderer {currentRenderer.GetType().Name} returned null/empty headers");
                return "<!-- No headers rendered -->";
            }

            return headerResult;
        }
        catch (Exception ex)
        {
            Logger.LogError($"Error rendering SVG headers for zoom level {ZoomLevel}: {ex.Message}");
            return $"<!-- Error rendering headers: {ex.Message} -->";
        }
    }
}
