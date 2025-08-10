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

    // === TIMELINE PROPERTIES ===
    private DateTime StartDate { get; set; }
    private DateTime EndDate { get; set; }
    private int TotalWidth { get; set; }
    private int TotalHeight { get; set; }

    // === PATTERN DETECTION (Used by partial classes) ===
    protected bool IsWeekDayPattern => ZoomLevel >= TimelineZoomLevel.WeekDayOptimal30px &&
                                      ZoomLevel <= TimelineZoomLevel.WeekDayOptimal60px;

    protected bool IsMonthWeekPattern => ZoomLevel >= TimelineZoomLevel.MonthWeekOptimal30px &&
                                        ZoomLevel <= TimelineZoomLevel.MonthWeekOptimal60px;

    protected bool IsQuarterMonthPattern => ZoomLevel >= TimelineZoomLevel.QuarterMonthOptimal30px &&
                                           ZoomLevel <= TimelineZoomLevel.QuarterMonthOptimal40px;

    protected bool IsYearQuarterPattern => ZoomLevel == TimelineZoomLevel.YearQuarterOptimal30px;

    // === ZOOM CALCULATIONS ===
    private double EffectiveDayWidth
    {
        get
        {
            var config = TimelineZoomService.GetConfiguration(ZoomLevel);
            return config.GetEffectiveDayWidth(ZoomFactor);
        }
    }

    protected double DayWidth => EffectiveDayWidth; // Alias for easier usage in patterns

    private int TotalHeaderHeight => HeaderMonthHeight + HeaderDayHeight;

    // === MAIN SVG HEADER RENDERING ORCHESTRATOR ===
    /// <summary>
    /// Main orchestrator method that delegates to the appropriate pattern implementation.
    /// Each pattern is implemented in its own partial class file for true independence.
    /// </summary>
    protected string RenderSVGHeaders()
    {
        try
        {
            Logger.LogDebugInfo($"RenderSVGHeaders - ZoomLevel: {ZoomLevel}, IsWeekDayPattern: {IsWeekDayPattern}");

            // Level-specific routing for maximum independence
            return ZoomLevel switch
            {
                // WeekDay Levels (Individual partial classes)
                TimelineZoomLevel.WeekDayOptimal30px => RenderWeekDay30pxHeaders(),
                TimelineZoomLevel.WeekDayOptimal40px => RenderWeekDay40pxHeaders(),
                TimelineZoomLevel.WeekDayOptimal50px => RenderWeekDay50pxHeaders(),
                TimelineZoomLevel.WeekDayOptimal60px => RenderWeekDay60pxHeaders(),
                
                // MonthWeek Pattern (Will be split in Commit 6)
                _ when IsMonthWeekPattern => RenderMonthWeekHeaders(),
                
                // Future patterns
                _ when IsQuarterMonthPattern => RenderQuarterMonthHeaders(),
                _ when IsYearQuarterPattern => RenderYearQuarterHeaders(),
                
                _ => throw new InvalidOperationException($"Unsupported zoom level: {ZoomLevel}")
            };
        }
        catch (Exception ex)
        {
            Logger.LogError($"Error rendering SVG headers for zoom level {ZoomLevel}: {ex.Message}");
            return $"<!-- Error rendering headers: {ex.Message} -->";
        }
    }    // === PATTERN METHODS (Implemented in partial classes) ===
    // These methods will be implemented in separate partial class files:
    // - TimelineView.WeekDay.cs: RenderWeekDayHeaders() ✅ IMPLEMENTED
    // - TimelineView.MonthWeek.cs: RenderMonthWeekHeaders()  
    // - TimelineView.QuarterMonth.cs: RenderQuarterMonthHeaders()
    // - TimelineView.YearQuarter.cs: RenderYearQuarterHeaders()

    // Temporary placeholder methods to prevent compilation errors
    // These will be removed as pattern files are created
    private string RenderQuarterMonthHeaders() => "<!-- QuarterMonth pattern not implemented yet -->";
    private string RenderYearQuarterHeaders() => "<!-- YearQuarter pattern not implemented yet -->";

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
        // Simple approach for SVG architecture
        DateTime taskStartDate, taskEndDate;

        if (!Tasks.Any())
        {
            taskStartDate = DateTime.UtcNow.Date.AddDays(-30);
            taskEndDate = DateTime.UtcNow.Date.AddDays(90);
        }
        else
        {
            taskStartDate = Tasks.Min(t => t.StartDate).Date.AddDays(-7);
            taskEndDate = Tasks.Max(t => t.EndDate).Date.AddDays(7);
        }

        // Direct calculation without legacy TimelineHeaderAdapter
        StartDate = taskStartDate;
        EndDate = taskEndDate;

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

    // === TOOLTIP FUNCTIONALITY ===
    private TimelineTooltipResult GetTooltipResult()
    {
        // Simple placeholder for SVG architecture - will be implemented later
        return new TimelineTooltipResult
        {
            LeftTooltip = "",
            RightTooltip = ""
        };
    }

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
