using GanttComponents.Models;
using GanttComponents.Models.Filtering;
using GanttComponents.Services;
using GanttComponents.Components.TimelineView.Renderers;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace GanttComponents.Components.TimelineView;

/// <summary>
/// TimelineView component with partial classes architecture for pure SVG rendering.
/// Base component logic and state management for timeline with independent zoom patterns.
/// </summary>
public partial class TimelineView : ComponentBase, IDisposable
{
    // === DEPENDENCY INJECTION ===
    [Inject] private IUniversalLogger Logger { get; set; } = default!;
    [Inject] private DateFormatHelper DateFormatter { get; set; } = default!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

    // === COMPONENT PARAMETERS ===
    [Parameter, EditorRequired] public List<GanttTask> Tasks { get; set; } = new();
    [Parameter] public EventCallback<int> OnTaskSelected { get; set; }
    [Parameter] public EventCallback<EventArgs> OnScrollChanged { get; set; }
    [Parameter] public int? SelectedTaskId { get; set; }
    [Parameter, EditorRequired] public int RowHeight { get; set; } = 32;
    [Parameter, EditorRequired] public int HeaderMonthHeight { get; set; } = 32;
    [Parameter, EditorRequired] public int HeaderDayHeight { get; set; } = 24;
    [Parameter] public EventCallback<int?> OnTaskHovered { get; set; }
    [Parameter] public int? HoveredTaskId { get; set; }
    [Parameter] public TimelineZoomLevel ZoomLevel { get; set; } = TimelineZoomLevel.MonthWeek;
    [Parameter] public double ZoomFactor { get; set; } = 1.0;  // Template-native: 1.0x to template maximum
    [Parameter] public EventCallback<TimelineZoomLevel> OnZoomLevelChanged { get; set; }
    [Parameter] public EventCallback<double> OnZoomFactorChanged { get; set; }
    [Parameter] public TaskFilterCriteria? FilterCriteria { get; set; }

    // === COMPONENT STATE ===
    private double ViewportScrollLeft { get; set; } = 0;
    private double ViewportWidth { get; set; } = 1000;
    private const int TaskBarHeight = 20;
    private const int TaskBarMargin = 6;

    // === COMPONENT ID ===
    private string ComponentId { get; set; } = Guid.NewGuid().ToString("N")[..8];

    // === COMPOSITION ARCHITECTURE ===
    /// <summary>
    /// Current renderer instance for composition architecture.
    /// Used for zoom levels that have been migrated from partial class to composition.
    /// </summary>
    private BaseTimelineRenderer? currentRenderer;

    // === TIMELINE PROPERTIES ===
    private DateTime StartDate { get; set; }
    private DateTime EndDate { get; set; }
    private int TotalWidth { get; set; }
    private int TotalHeight { get; set; }

    // === LEVEL-BASED ARCHITECTURE ===
    // Individual zoom levels implemented in dedicated partial class files
    // No pattern detection needed - each level has direct routing

    // === COMPOSITION PATTERN DETECTION ===
    /// <summary>
    /// Determines if the given zoom level uses the composition architecture.
    /// ALL zoom levels now use composition - partial classes have been removed for clean ABC architecture.
    /// </summary>
    /// <param name="zoomLevel">The zoom level to check</param>
    /// <returns>Always true - all patterns use composition architecture</returns>
    private bool IsCompositionPattern(TimelineZoomLevel zoomLevel)
    {
        // PURE COMPOSITION ARCHITECTURE: All zoom levels use BaseTimelineRenderer subclasses
        // Partial classes have been removed for clean ABC composition design
        return true;
    }

    // === FILTERING ===
    /// <summary>
    /// Returns tasks filtered by current FilterCriteria, including tiny task filtering
    /// </summary>
    private List<GanttTask> FilteredTasks
    {
        get
        {
            if (Tasks == null) return new List<GanttTask>();

            var filteredTasks = new List<GanttTask>();

            // Calculate tiny task IDs if filtering is enabled
            HashSet<int>? tinyTaskIds = null;
            if (FilterCriteria != null)
            {
                tinyTaskIds = CalculateTinyTaskIds();
            }

            foreach (var task in Tasks)
            {
                // Apply all filters including tiny task filtering
                if (FilterCriteria != null && !FilterCriteria.PassesFilter(task, tinyTaskIds))
                    continue;

                filteredTasks.Add(task);
            }

            return filteredTasks;
        }
    }

    /// <summary>
    /// Calculates which tasks should be considered tiny based on pixel width
    /// </summary>
    private HashSet<int> CalculateTinyTaskIds()
    {
        var tinyTaskIds = new HashSet<int>();

        if (Tasks == null || FilterCriteria == null) return tinyTaskIds;

        foreach (var task in Tasks)
        {
            var pixelWidth = CalculateTaskWidth(task);
            if (pixelWidth < FilterCriteria.TinyTaskPixelThreshold)
            {
                tinyTaskIds.Add(task.Id);
            }
        }

        return tinyTaskIds;
    }

    // === TEMPLATE-BASED CALCULATIONS ===
    private double TemplateUnitWidth
    {
        get
        {
            var config = TimelineZoomService.GetConfiguration(ZoomLevel);
            return config.BaseUnitWidth * ZoomFactor;
        }
    }

    private double TemplateUnitDays
    {
        get
        {
            var config = TimelineZoomService.GetConfiguration(ZoomLevel);
            return config.TemplateUnitDays;
        }
    }

    // Day width calculated from template unit dimensions
    protected double DayWidth => TemplateUnitWidth / TemplateUnitDays;

    private int TotalHeaderHeight => HeaderMonthHeight + HeaderDayHeight;

    // === MAIN SVG HEADER RENDERING ORCHESTRATOR ===
    /// <summary>
    /// Main orchestrator method that delegates to the appropriate pattern implementation.
    /// Hybrid approach: Uses composition for migrated zoom levels, partial classes for others.
    /// </summary>
    protected string RenderSVGHeaders()
    {
        try
        {
            // TEMPLATE-BASED ARCHITECTURE: All zoom levels use template configurations
            currentRenderer = RendererFactory.CreateRenderer(
                ZoomLevel,
                Logger,
                DateFormatter,
                StartDate,
                EndDate,
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

    // === LEVEL-SPECIFIC METHODS (Implemented in individual partial classes) ===
    // True level-level independence achieved through dedicated files:
    // MonthWeek Levels:
    // - TimelineView.MonthWeek30px.cs: RenderMonthWeek30pxHeaders() ✅ IMPLEMENTED
    // - TimelineView.MonthWeek40px.cs: RenderMonthWeek40pxHeaders() ✅ IMPLEMENTED  
    // - TimelineView.MonthWeek50px.cs: RenderMonthWeek50pxHeaders() ✅ IMPLEMENTED
    // - TimelineView.MonthWeek60px.cs: RenderMonthWeek60pxHeaders() ✅ IMPLEMENTED
    // WeekDay Levels:
    // - TimelineView.WeekDay30px.cs: RenderWeekDay30pxHeaders() ✅ IMPLEMENTED
    // - TimelineView.WeekDay40px.cs: RenderWeekDay40pxHeaders() ✅ IMPLEMENTED
    // - TimelineView.WeekDay50px.cs: RenderWeekDay50pxHeaders() ✅ IMPLEMENTED
    // - TimelineView.WeekDay60px.cs: RenderWeekDay60pxHeaders() ✅ IMPLEMENTED
    // Future Patterns:
    // - TimelineView.QuarterMonth##px.cs: Individual level files (planned)
    // - TimelineView.YearQuarter##px.cs: Individual level files (planned)
    // Each future pattern will have dedicated partial class files for level-level independence

    // === COMPONENT LIFECYCLE ===
    protected override void OnInitialized()
    {
        CalculateTimelineRange();
    }

    protected override void OnParametersSet()
    {
        ValidateRequiredParameters();
        CalculateTimelineRange();
        StateHasChanged();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            try
            {
                // Initialize viewport width
                ViewportWidth = await JSRuntime.InvokeAsync<double>("eval",
                    "document.querySelector('.timeline-scroll-container')?.clientWidth || 1000");

                // Initialize immediate scroll synchronization to eliminate header "tear apart"
                await JSRuntime.InvokeVoidAsync("timelineView.initializeScrollSync", $"timeline-{ComponentId}");

                StateHasChanged();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error initializing timeline view: {ex.Message}");
                ViewportWidth = 1000;
            }
        }
    }

    // === TIMELINE CALCULATIONS ===
    private void CalculateTimelineRange()
    {
        // Step 1: Calculate base task date range
        if (!Tasks.Any())
        {
            // Default range for empty task list
            StartDate = DateTime.UtcNow.Date.AddDays(-30);
            EndDate = DateTime.UtcNow.Date.AddDays(90);
        }
        else
        {
            // Use exact task date range
            StartDate = Tasks.Min(t => t.StartDate).ToUtcDateTime().Date;
            EndDate = Tasks.Max(t => t.EndDate).ToUtcDateTime().Date;
        }

        // Step 2: Get expanded boundaries for SVG canvas sizing
        // The headers need expanded boundaries to prevent truncation, so the SVG canvas must be wide enough
        var expandedBounds = GetExpandedTimelineBounds();
        var expandedDays = (expandedBounds.end - expandedBounds.start).Days;

        // CRITICAL FIX: Use expanded boundaries as the unified coordinate system reference
        // This ensures headers and taskbars use the same coordinate system
        StartDate = expandedBounds.start;
        EndDate = expandedBounds.end;

        TotalWidth = Math.Max(100, (int)(expandedDays * (TemplateUnitWidth / TemplateUnitDays))); // Minimum 100px width
        TotalHeight = Math.Max(50, FilteredTasks.Count * RowHeight); // Minimum 50px height

        // ALIGNMENT VALIDATION: Ensure header and body use identical widths
        var headerViewBox = SVGRenderingHelpers.GetHeaderViewBox(TotalWidth, TotalHeaderHeight);
        var bodyViewBox = SVGRenderingHelpers.GetBodyViewBox(TotalWidth, (int)TotalHeight);
        var headerWidth = headerViewBox.Split(' ')[2]; // Extract width from "0 0 width height"
        var bodyWidth = bodyViewBox.Split(' ')[2];

        if (headerWidth != bodyWidth)
        {
            throw new InvalidOperationException($"Timeline alignment error: Header width ({headerWidth}) != Body width ({bodyWidth}). ViewBoxes: Header='{headerViewBox}', Body='{bodyViewBox}'");
        }
    }

    /// <summary>
    /// Gets the expanded timeline boundaries that the renderers will use for header rendering.
    /// This ensures the SVG canvas is wide enough to contain the expanded headers.
    /// </summary>
    /// <returns>Expanded start and end dates for timeline rendering</returns>
    private (DateTime start, DateTime end) GetExpandedTimelineBounds()
    {
        try
        {
            // Create a temporary renderer to get the expanded boundaries
            var tempRenderer = RendererFactory.CreateRenderer(
                ZoomLevel,
                Logger,
                DateFormatter,
                StartDate,
                EndDate,
                HeaderMonthHeight,
                HeaderDayHeight,
                ZoomFactor
            );

            if (tempRenderer == null)
            {
                Logger.LogWarning($"Could not create renderer for boundary calculation, using original dates");
                return (StartDate, EndDate);
            }

            // Get the expanded boundaries from the renderer
            var expandedBounds = tempRenderer.CalculateHeaderBoundaries();

            return (expandedBounds.expandedStart, expandedBounds.expandedEnd);
        }
        catch (Exception ex)
        {
            Logger.LogError($"Error calculating expanded timeline bounds: {ex.Message}");
            return (StartDate, EndDate);
        }
    }

    /// <summary>
    /// Converts a date to pixel position using template-unit based calculation.
    /// Template-pure approach: No day-width concept, direct template-unit mapping.
    /// </summary>
    private double DateToPixel(DateTime date)
    {
        var config = TimelineZoomService.GetConfiguration(ZoomLevel);
        var days = (date.Date - StartDate).TotalDays;

        // Template-pure calculation: Days → Template Units → Pixels
        var templateUnits = days / config.TemplateUnitDays;
        var pixelPosition = templateUnits * config.BaseUnitWidth * ZoomFactor;

        return pixelPosition;
    }

    private double CalculateTaskWidth(GanttTask task)
    {
        // COORDINATE SYSTEM FIX: Calculate width using template-based coordinate system
        // instead of direct duration to ensure alignment between start and end positions
        var startPixel = DateToPixel(task.StartDate.ToUtcDateTime());
        var endPixel = DateToPixel(task.EndDate.ToUtcDateTime()); // Exclusive end date (standard Gantt practice)
        var width = Math.Max(1.0, endPixel - startPixel); // Minimum 1px width

        return width;
    }    /// <summary>
         /// Determines if a task should be rendered as a tiny marker instead of a task bar
         /// </summary>
    private bool ShouldRenderAsTinyMarker(GanttTask task, double pixelWidth)
    {
        // Use FilterCriteria if available, otherwise default behavior (show tiny tasks with 3px threshold)
        if (FilterCriteria != null)
        {
            return FilterCriteria.ShouldRenderAsTinyMarker(task, pixelWidth);
        }

        // Default behavior: show tiny markers for tasks < 3px width
        return pixelWidth < 3.0;
    }

    private int GetMonthWidth(DateTime month)
    {
        var daysInMonth = DateTime.DaysInMonth(month.Year, month.Month);
        return (int)(daysInMonth * (TemplateUnitWidth / TemplateUnitDays));
    }

    // === USER INTERACTIONS ===
    public async Task SelectTask(int taskId)
    {
        await SelectTaskInternal(taskId, notifyParent: true);
    }

    /// <summary>
    /// Internal selection method that can optionally skip parent notification to prevent loops
    /// </summary>
    public async Task SelectTaskInternal(int taskId, bool notifyParent = true)
    {
        SelectedTaskId = taskId;
        Logger.LogInfo($"Task {taskId} selected in TimelineView");

        // Auto-center the selected task in viewport (works in both standalone and composed contexts)
        await CenterSelectedTask();

        if (notifyParent && OnTaskSelected.HasDelegate)
        {
            await OnTaskSelected.InvokeAsync(taskId);
        }

        StateHasChanged();
    }

    private async Task HoverTask(int? taskId)
    {
        if (HoveredTaskId != taskId)
        {
            HoveredTaskId = taskId;
            await OnTaskHovered.InvokeAsync(taskId);
        }
    }

    private async Task OnScroll(EventArgs e)
    {
        try
        {
            ViewportScrollLeft = await JSRuntime.InvokeAsync<double>("eval",
                "document.querySelector('.timeline-scroll-container').scrollLeft");
            ViewportWidth = await JSRuntime.InvokeAsync<double>("eval",
                "document.querySelector('.timeline-scroll-container').clientWidth");
            StateHasChanged();
        }
        catch (Exception ex)
        {
            Logger.LogError($"Error updating viewport data during scroll: {ex.Message}");
        }

        if (OnScrollChanged.HasDelegate)
        {
            await OnScrollChanged.InvokeAsync(e);
        }
    }

    // === VIEWPORT MANAGEMENT ===
    /// <summary>
    /// Updates viewport information from JavaScript (called when needed).
    /// JavaScript handles immediate scroll sync, this method updates Blazor state.
    /// </summary>
    [JSInvokable]
    public async Task UpdateViewportFromJS(ViewportData viewport)
    {
        try
        {
            ViewportScrollLeft = viewport.ScrollLeft;
            ViewportWidth = viewport.ClientWidth;
            StateHasChanged();

            // Notify parent component of scroll change if needed
            if (OnScrollChanged.HasDelegate)
            {
                await OnScrollChanged.InvokeAsync(EventArgs.Empty);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError($"Error updating viewport from JavaScript: {ex.Message}");
        }
    }

    /// <summary>
    /// Data structure for viewport information from JavaScript
    /// </summary>
    public class ViewportData
    {
        public double ScrollLeft { get; set; }
        public double ClientWidth { get; set; }
        public double ScrollWidth { get; set; }
    }

    // === ZOOM CONTROL METHODS ===
    public async Task SetZoomLevelAsync(TimelineZoomLevel newZoomLevel)
    {
        if (ZoomLevel != newZoomLevel)
        {
            var oldLevel = ZoomLevel;
            ZoomLevel = newZoomLevel;
            CalculateTimelineRange();
            Logger.LogInfo($"Zoom level changed from {oldLevel} to {newZoomLevel}");

            if (OnZoomLevelChanged.HasDelegate)
            {
                await OnZoomLevelChanged.InvokeAsync(newZoomLevel);
            }

            StateHasChanged();
        }
    }

    public async Task SetZoomFactorAsync(double newZoomFactor)
    {
        // Use template-based zoom factor clamping instead of hardcoded limits
        var clampedFactor = TimelineZoomService.ClampZoomFactor(ZoomLevel, newZoomFactor);

        if (Math.Abs(ZoomFactor - clampedFactor) > 0.01)
        {
            var oldFactor = ZoomFactor;
            ZoomFactor = clampedFactor;
            CalculateTimelineRange();
            Logger.LogInfo($"Zoom factor changed from {oldFactor:F2} to {clampedFactor:F2} (template: {ZoomLevel})");

            if (OnZoomFactorChanged.HasDelegate)
            {
                await OnZoomFactorChanged.InvokeAsync(clampedFactor);
            }

            StateHasChanged();
        }
    }

    public async Task ZoomInAsync()
    {
        var proposedFactor = ZoomFactor * 1.2;
        if (TimelineZoomService.CanZoomIn(ZoomLevel, ZoomFactor, 0.2))
        {
            await SetZoomFactorAsync(proposedFactor);
        }
        else
        {
            Logger.LogInfo($"Cannot zoom in further: at template maximum for {ZoomLevel}");
        }
    }

    public async Task ZoomOutAsync()
    {
        var proposedDecrease = ZoomFactor * 0.2; // 20% decrease
        if (TimelineZoomService.CanZoomOut(ZoomLevel, ZoomFactor, proposedDecrease))
        {
            await SetZoomFactorAsync(ZoomFactor / 1.2);
        }
        else
        {
            Logger.LogInfo($"Cannot zoom out further: at template minimum for {ZoomLevel}");
        }
    }

    public async Task ResetZoomAsync() => await SetZoomFactorAsync(1.0);

    /// <summary>
    /// Centers the currently selected task in the timeline viewport.
    /// Uses DOM-based positioning for future-proof centering independent of SVG dimensions.
    /// </summary>
    public async Task CenterSelectedTask()
    {
        if (!SelectedTaskId.HasValue) return;

        try
        {
            await JSRuntime.InvokeVoidAsync("timelineView.centerTaskById",
                $"timeline-{ComponentId}",
                SelectedTaskId.Value);

            Logger.LogInfo($"Task {SelectedTaskId.Value} centered in timeline");
        }
        catch (Exception ex)
        {
            // Graceful degradation - centering is enhancement, not critical feature
            Logger.LogError($"Error centering task {SelectedTaskId}: {ex.Message}");
        }
    }

    // === PUBLIC PROPERTIES ===
    public double CurrentEffectiveDayWidth => TemplateUnitWidth / TemplateUnitDays;
    public string CurrentZoomDescription => $"{ZoomLevel} @ {ZoomFactor:F1}x";

    private int GetWeekOfYear(DateTime date)
    {
        var culture = System.Globalization.CultureInfo.CurrentCulture;
        var dayOfWeek = culture.DateTimeFormat.FirstDayOfWeek;
        var weekRule = culture.DateTimeFormat.CalendarWeekRule;
        return culture.Calendar.GetWeekOfYear(date, weekRule, dayOfWeek);
    }

    // === VALIDATION ===
    private void ValidateRequiredParameters()
    {
        if (RowHeight <= 0)
        {
            throw new InvalidOperationException(
                "TimelineView requires RowHeight > 0. " +
                "This parameter is critical for proper row alignment and tooltip positioning. " +
                "Example: <TimelineView RowHeight=\"32\" ... />");
        }

        if (HeaderMonthHeight <= 0)
        {
            throw new InvalidOperationException(
                "TimelineView requires HeaderMonthHeight > 0. " +
                "This parameter is critical for tooltip positioning (CSS calc). " +
                "Example: <TimelineView HeaderMonthHeight=\"32\" ... />");
        }

        if (HeaderDayHeight <= 0)
        {
            throw new InvalidOperationException(
                "TimelineView requires HeaderDayHeight > 0. " +
                "This parameter is critical for header rendering and alignment. " +
                "Example: <TimelineView HeaderDayHeight=\"24\" ... />");
        }

        if (Tasks == null)
        {
            throw new InvalidOperationException(
                "TimelineView requires a non-null Tasks parameter. " +
                "Pass an empty list if no tasks are available. " +
                "Example: <TimelineView Tasks=\"@(new List<GanttTask>())\" ... />");
        }
    }

    // === UTILITY METHODS ===
    private string GetTaskTooltip(GanttTask task)
    {
        if (task == null) return string.Empty;

        try
        {
            return $"{task.Name} ({task.StartDate:MM/dd/yyyy} - {task.EndDate:MM/dd/yyyy})";
        }
        catch
        {
            return task.Name ?? "Task";
        }
    }

    /// <summary>
    /// Get grid line dates based on secondary header boundaries for the current template.
    /// - YearQuarter: Grid lines at quarter starts
    /// - QuarterMonth: Grid lines at month starts  
    /// - MonthWeek: Grid lines at week starts
    /// - WeekDay: Grid lines at day starts
    /// </summary>
    private List<DateTime> GetSecondaryGridLineDates()
    {
        var gridDates = new List<DateTime>();

        try
        {
            switch (ZoomLevel)
            {
                case TimelineZoomLevel.YearQuarter:
                    // Grid lines at quarter starts
                    var quarterStart = new DateTime(StartDate.Year, ((StartDate.Month - 1) / 3) * 3 + 1, 1);
                    while (quarterStart <= EndDate)
                    {
                        if (quarterStart >= StartDate) gridDates.Add(quarterStart);
                        quarterStart = quarterStart.AddMonths(3);
                    }
                    break;

                case TimelineZoomLevel.QuarterMonth:
                    // Grid lines at month starts
                    var monthStart = new DateTime(StartDate.Year, StartDate.Month, 1);
                    while (monthStart <= EndDate)
                    {
                        if (monthStart >= StartDate) gridDates.Add(monthStart);
                        monthStart = monthStart.AddMonths(1);
                    }
                    break;

                case TimelineZoomLevel.MonthWeek:
                    // Grid lines at week starts (Monday)
                    var weekStart = StartDate.AddDays(-(int)StartDate.DayOfWeek + (int)DayOfWeek.Monday);
                    if (weekStart > StartDate) weekStart = weekStart.AddDays(-7);
                    while (weekStart <= EndDate)
                    {
                        if (weekStart >= StartDate) gridDates.Add(weekStart);
                        weekStart = weekStart.AddDays(7);
                    }
                    break;

                case TimelineZoomLevel.WeekDay:
                    // Grid lines at day starts (daily)
                    for (var day = StartDate; day <= EndDate; day = day.AddDays(1))
                    {
                        gridDates.Add(day);
                    }
                    break;
            }
        }
        catch (Exception ex)
        {
            Logger.LogOperation("TimelineView", $"Error calculating grid line dates: {ex.Message}");
            // Fallback to daily grid lines
            for (var day = StartDate; day <= EndDate; day = day.AddDays(1))
            {
                gridDates.Add(day);
            }
        }

        return gridDates;
    }

    // === DISPOSAL ===
    public void Dispose()
    {
        // Component cleanup - no longer needed for language changes
    }
}
