using GanttComponents.Models;
using GanttComponents.Services;
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
    [Inject] private IGanttI18N I18N { get; set; } = default!;
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
    [Parameter] public TimelineZoomLevel ZoomLevel { get; set; } = TimelineZoomLevel.MonthWeekOptimal50px;
    [Parameter] public double ZoomFactor { get; set; } = 1.6;
    [Parameter] public EventCallback<TimelineZoomLevel> OnZoomLevelChanged { get; set; }
    [Parameter] public EventCallback<double> OnZoomFactorChanged { get; set; }

    // === COMPONENT STATE ===
    private double ViewportScrollLeft { get; set; } = 0;
    private double ViewportWidth { get; set; } = 1000;
    private const int TaskBarHeight = 20;
    private const int TaskBarMargin = 6;

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

    // === ZOOM CALCULATIONS ===
    private double EffectiveDayWidth
    {
        get
        {
            var config = TimelineZoomService.GetConfiguration(ZoomLevel);

            // VALIDATION 1: Base Day Width must be integral
            ValidateBaseDayWidth(config.BaseDayWidth);

            var effectiveWidth = config.GetEffectiveDayWidth(ZoomFactor);

            // VALIDATION 2: Effective Day Width must be integral  
            ValidateEffectiveDayWidth(effectiveWidth);

            return effectiveWidth;
        }
    }

    protected double DayWidth => EffectiveDayWidth; // Alias for easier usage in patterns

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
            Logger.LogDebugInfo($"RenderSVGHeaders - ZoomLevel: {ZoomLevel}");

            // COMPOSITION ARCHITECTURE: Use renderer for migrated zoom levels
            if (ZoomLevel == TimelineZoomLevel.WeekDayOptimal50px)
            {
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
                
                Logger.LogDebugInfo($"Using composition renderer: {currentRenderer.GetType().Name}");
                return currentRenderer.RenderHeaders();
            }

            // LEGACY PARTIAL CLASS ARCHITECTURE: Fallback for non-migrated zoom levels
            Logger.LogDebugInfo($"Using legacy partial class architecture for {ZoomLevel}");
            return ZoomLevel switch
            {
                // WeekDay Levels (Individual partial classes) - WILL BE MIGRATED
                TimelineZoomLevel.WeekDayOptimal30px => RenderWeekDay30pxHeaders(),
                TimelineZoomLevel.WeekDayOptimal40px => RenderWeekDay40pxHeaders(),
                // TimelineZoomLevel.WeekDayOptimal50px - MIGRATED TO COMPOSITION ✅
                TimelineZoomLevel.WeekDayOptimal60px => RenderWeekDay60pxHeaders(),

                // MonthWeek Levels (Individual partial classes) - WILL BE MIGRATED
                TimelineZoomLevel.MonthWeekOptimal30px => RenderMonthWeek30pxHeaders(),
                TimelineZoomLevel.MonthWeekOptimal40px => RenderMonthWeek40pxHeaders(),
                TimelineZoomLevel.MonthWeekOptimal50px => RenderMonthWeek50pxHeaders(),
                TimelineZoomLevel.MonthWeekOptimal60px => RenderMonthWeek60pxHeaders(),

                // Unsupported levels (future implementations)
                _ => throw new InvalidOperationException($"Unsupported zoom level: {ZoomLevel}. Only WeekDay and MonthWeek optimal levels are currently implemented.")
            };
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
        Logger.LogComponentLifecycle("TimelineView", "OnInitialized", new { TaskCount = Tasks.Count });
        I18N.LanguageChanged += OnLanguageChanged;
        CalculateTimelineRange();
    }

    protected override void OnParametersSet()
    {
        ValidateRequiredParameters();
        Logger.LogDebugInfo($"TimelineView header heights - Month: {HeaderMonthHeight}, Day: {HeaderDayHeight}, Total: {TotalHeaderHeight}");
        CalculateTimelineRange();
        StateHasChanged();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            try
            {
                ViewportWidth = await JSRuntime.InvokeAsync<double>("eval",
                    "document.querySelector('.timeline-scroll-container')?.clientWidth || 1000");
                StateHasChanged();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error initializing viewport width: {ex.Message}");
                ViewportWidth = 1000;
            }
        }
    }

    // === TIMELINE CALCULATIONS ===
    private void CalculateTimelineRange()
    {
        // Simple timeline range based directly on task dates - no buffer complexity
        if (!Tasks.Any())
        {
            // Default range for empty task list
            StartDate = DateTime.UtcNow.Date.AddDays(-30);
            EndDate = DateTime.UtcNow.Date.AddDays(90);
        }
        else
        {
            // Use exact task date range
            StartDate = Tasks.Min(t => t.StartDate).Date;
            EndDate = Tasks.Max(t => t.EndDate).Date;
        }

        Logger.LogDebugInfo($"Simple timeline range: {StartDate:yyyy-MM-dd} to {EndDate:yyyy-MM-dd}");

        var totalDays = (EndDate - StartDate).Days + 1;
        TotalWidth = (int)(totalDays * EffectiveDayWidth);
        TotalHeight = Tasks.Count * RowHeight;
    }

    private double DayToPixel(DateTime date)
    {
        var days = (date.Date - StartDate).TotalDays;
        return days * EffectiveDayWidth;
    }

    private double CalculateTaskWidth(GanttTask task)
    {
        var duration = (task.EndDate.Date - task.StartDate.Date).TotalDays + 1;
        return duration * EffectiveDayWidth;
    }

    private int GetMonthWidth(DateTime month)
    {
        var daysInMonth = DateTime.DaysInMonth(month.Year, month.Month);
        return (int)(daysInMonth * EffectiveDayWidth);
    }

    // === USER INTERACTIONS ===
    private async Task SelectTask(int taskId)
    {
        SelectedTaskId = taskId;
        Logger.LogUserAction("TimelineView_TaskSelected", new { TaskId = taskId });

        if (OnTaskSelected.HasDelegate)
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

    private async Task OnScrollAsync(EventArgs e)
    {
        try
        {
            // Update viewport data
            ViewportScrollLeft = await JSRuntime.InvokeAsync<double>("eval",
                "document.querySelector('.timeline-scroll-container').scrollLeft");
            ViewportWidth = await JSRuntime.InvokeAsync<double>("eval",
                "document.querySelector('.timeline-scroll-container').clientWidth");

            // Sync header horizontal scroll with body scroll
            await JSRuntime.InvokeVoidAsync("eval", $@"
                const headerContainer = document.querySelector('.timeline-header-container');
                const bodyContainer = document.querySelector('.timeline-scroll-container');
                if (headerContainer && bodyContainer) {{
                    headerContainer.scrollLeft = bodyContainer.scrollLeft;
                }}
            ");

            StateHasChanged();
        }
        catch (Exception ex)
        {
            Logger.LogError($"Error synchronizing header scroll: {ex.Message}");
        }

        if (OnScrollChanged.HasDelegate)
        {
            await OnScrollChanged.InvokeAsync(e);
        }
    }

    private void OnLanguageChanged()
    {
        try
        {
            InvokeAsync(StateHasChanged);
        }
        catch (ObjectDisposedException) { }
        catch (InvalidOperationException) { }
    }

    // === ZOOM CONTROL METHODS ===
    public async Task SetZoomLevelAsync(TimelineZoomLevel newZoomLevel)
    {
        if (ZoomLevel != newZoomLevel)
        {
            ZoomLevel = newZoomLevel;
            CalculateTimelineRange();
            Logger.LogUserAction("TimelineView_ZoomLevelChanged",
                new { OldLevel = ZoomLevel, NewLevel = newZoomLevel });

            if (OnZoomLevelChanged.HasDelegate)
            {
                await OnZoomLevelChanged.InvokeAsync(newZoomLevel);
            }

            StateHasChanged();
        }
    }

    public async Task SetZoomFactorAsync(double newZoomFactor)
    {
        var clampedFactor = Math.Max(0.5, Math.Min(3.0, newZoomFactor));

        if (Math.Abs(ZoomFactor - clampedFactor) > 0.01)
        {
            var oldFactor = ZoomFactor;
            ZoomFactor = clampedFactor;
            CalculateTimelineRange();
            Logger.LogUserAction("TimelineView_ZoomFactorChanged",
                new { OldFactor = oldFactor, NewFactor = clampedFactor });

            if (OnZoomFactorChanged.HasDelegate)
            {
                await OnZoomFactorChanged.InvokeAsync(clampedFactor);
            }

            StateHasChanged();
        }
    }

    public async Task ZoomInAsync() => await SetZoomFactorAsync(ZoomFactor * 1.2);
    public async Task ZoomOutAsync() => await SetZoomFactorAsync(ZoomFactor / 1.2);
    public async Task ResetZoomAsync() => await SetZoomFactorAsync(1.0);

    // === PUBLIC PROPERTIES ===
    public double CurrentEffectiveDayWidth => EffectiveDayWidth;
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

    /// <summary>
    /// INTEGRAL DAY WIDTH VALIDATION 1: Validates that the base day width is integral.
    /// This enforces the "Integral Day Widths" architectural requirement at the configuration level.
    /// </summary>
    /// <param name="baseDayWidth">The base day width from zoom configuration</param>
    /// <exception cref="InvalidOperationException">Thrown when base day width is not integral</exception>
    private void ValidateBaseDayWidth(double baseDayWidth)
    {
        if (Math.Abs(baseDayWidth - Math.Round(baseDayWidth)) > 0.001)
        {
            throw new InvalidOperationException(
                $"INTEGRAL DAY WIDTH VIOLATION (Base): {ZoomLevel} has fractional BaseDayWidth = {baseDayWidth:F3}px. " +
                $"Pure SVG TimelineView requires integral base day widths for clean coordinate calculations. " +
                $"Configuration should use whole numbers like {Math.Round(baseDayWidth):F0}px instead. " +
                $"This ensures predictable effective day widths across all zoom factors.");
        }

        if (baseDayWidth <= 0)
        {
            throw new InvalidOperationException(
                $"DAY WIDTH VALIDATION: {ZoomLevel} has invalid BaseDayWidth = {baseDayWidth}px. " +
                $"Day width must be positive.");
        }
    }

    /// <summary>
    /// INTEGRAL DAY WIDTH VALIDATION 2: Validates that the effective day width is integral.
    /// This provides a safety net for the computed width (BaseDayWidth × ZoomFactor).
    /// </summary>
    /// <param name="effectiveDayWidth">The computed effective day width</param>
    /// <exception cref="InvalidOperationException">Thrown when effective day width is not integral</exception>
    private void ValidateEffectiveDayWidth(double effectiveDayWidth)
    {
        if (Math.Abs(effectiveDayWidth - Math.Round(effectiveDayWidth)) > 0.001)
        {
            throw new InvalidOperationException(
                $"INTEGRAL DAY WIDTH VIOLATION (Effective): {ZoomLevel} @ {ZoomFactor:F1}x = {effectiveDayWidth:F3}px effective day width. " +
                $"Pure SVG TimelineView requires integral effective day widths for clean SVG coordinate calculations. " +
                $"Try adjusting ZoomFactor to achieve a whole number result, such as {Math.Round(effectiveDayWidth):F0}px. " +
                $"BaseDayWidth × ZoomFactor must result in integral pixel values.");
        }

        if (effectiveDayWidth <= 0)
        {
            throw new InvalidOperationException(
                $"EFFECTIVE DAY WIDTH VALIDATION: {ZoomLevel} @ {ZoomFactor:F1}x = {effectiveDayWidth}px. " +
                $"Effective day width must be positive.");
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

    // === DISPOSAL ===
    public void Dispose()
    {
        try
        {
            if (I18N != null)
            {
                I18N.LanguageChanged -= OnLanguageChanged;
            }
        }
        catch (ObjectDisposedException) { }
    }
}
