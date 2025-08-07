# 🔧 TimelineView Component - Implementation Guide

## 🎯 **Implementation Overview**

This guide provides comprehensive implementation details for the TimelineView component, covering the technical aspects of the SVG-based timeline rendering, zoom system integration, and header architecture.

## 🏗️ **Core Architecture Implementation**

### **📁 File Structure**
```
Components/TimelineView/
├── TimelineView.razor              # Main component file
├── TimelineView.razor.cs           # Code-behind (if needed)
└── TimelineView.razor.css          # Component-specific styles

Components/TimelineHeader/
├── TimelineHeader.razor            # Extracted header component
├── TimelineHeaderService.cs        # Header business logic
└── HeaderPeriod.cs                 # Header data model
```

### **🔄 Service Dependencies**
The TimelineView component relies on several core services:

```csharp
@inject IUniversalLogger Logger
@inject IGanttI18N I18N  
@inject DateFormatHelper DateFormatter
@inject ITimelineHeaderService HeaderService  // New: extracted header logic
```

## 📊 **Zoom System Implementation**

### **⚙️ Zoom Parameter Processing**
```csharp
// Effective day width calculation
private double EffectiveDayWidth
{
    get
    {
        var config = TimelineZoomService.GetConfiguration(ZoomLevel);
        return config.GetEffectiveDayWidth(ZoomFactor);
    }
}

// Parameter change detection
protected override void OnParametersSet()
{
    // Validate zoom parameters
    var validatedLevel = TimelineZoomService.ValidateZoomLevel(ZoomLevel);
    var validatedFactor = TimelineZoomService.ValidateZoomFactor(ZoomLevel, ZoomFactor);
    
    if (validatedLevel != ZoomLevel || Math.Abs(validatedFactor - ZoomFactor) > 0.001)
    {
        // Parameters were clamped - notify parent
        InvokeAsync(async () =>
        {
            await OnZoomLevelChanged.InvokeAsync(validatedLevel);
            await OnZoomFactorChanged.InvokeAsync(validatedFactor);
        });
    }
}
```

### **📏 Timeline Boundary Calculation**
```csharp
private DateTime CalculateTimelineStart()
{
    if (Tasks == null || !Tasks.Any()) return DateTime.Today.AddDays(-7);
    
    var minStartDate = Tasks.Min(t => t.StartDate);
    var paddedStart = minStartDate.AddDays(-7);
    
    // Align to header period boundary for clean visual appearance
    var headerConfig = TimelineHeaderAdapter.GetHeaderConfiguration(ZoomLevel, EffectiveDayWidth);
    return TimelineHeaderAdapter.GetPeriodStart(paddedStart, headerConfig.PrimaryUnit);
}

private DateTime CalculateTimelineEnd()
{
    if (Tasks == null || !Tasks.Any()) return DateTime.Today.AddDays(30);
    
    var maxEndDate = Tasks.Max(t => t.EndDate);
    var paddedEnd = maxEndDate.AddDays(7);
    
    // Align to header period boundary
    var headerConfig = TimelineHeaderAdapter.GetHeaderConfiguration(ZoomLevel, EffectiveDayWidth);
    var periodStart = TimelineHeaderAdapter.GetPeriodStart(paddedEnd, headerConfig.PrimaryUnit);
    var increment = TimelineHeaderAdapter.GetDateIncrement(headerConfig.PrimaryUnit);
    return increment(periodStart);
}
```

### **🧮 Pixel Conversion Functions**
```csharp
private double DayToPixel(DateTime date)
{
    var daysDiff = (date - StartDate).Days;
    return daysDiff * EffectiveDayWidth;
}

private DateTime PixelToDay(double pixelX)
{
    var days = pixelX / EffectiveDayWidth;
    return StartDate.AddDays(Math.Round(days));
}

private double CalculateTaskWidth(GanttTask task)
{
    var durationDays = (task.EndDate - task.StartDate).Days + 1; // +1 for inclusive end
    var calculatedWidth = durationDays * EffectiveDayWidth;
    
    // Enforce minimum task width for usability
    return Math.Max(calculatedWidth, TaskDisplayConstants.MIN_TASK_WIDTH);
}
```

## 🎨 **SVG Rendering Implementation**

### **📐 Grid Lines Rendering**
```razor
<!-- Vertical grid lines for day separation -->
@for (var day = StartDate; day <= EndDate; day = day.AddDays(1))
{
    var x = DayToPixel(day);
    <line x1="@x" y1="0" x2="@x" y2="@TotalHeight" 
          class="timeline-grid-line"
          stroke="var(--gantt-grid-color, #e0e0e0)"
          stroke-width="1" />
}

<!-- Optional: Month boundary lines (thicker) -->
@if (ShowMonthBoundaries)
{
    for (var month = GetFirstDayOfMonth(StartDate); month <= EndDate; month = month.AddMonths(1))
    {
        var x = DayToPixel(month);
        <line x1="@x" y1="0" x2="@x" y2="@TotalHeight" 
              class="timeline-month-line"
              stroke="var(--gantt-month-boundary-color, #bdbdbd)"
              stroke-width="2" />
    }
}
```

### **📊 Task Bar Rendering**
```razor
@if (Tasks != null)
{
    @for (int i = 0; i < Tasks.Count; i++)
    {
        var task = Tasks[i];
        var y = i * RowHeight;
        var x = DayToPixel(task.StartDate);
        var width = CalculateTaskWidth(task);
        var taskIndex = i; // Capture for lambda
        
        <!-- Task background row for selection highlighting -->
        <rect x="0" y="@y" width="@TotalWidth" height="@RowHeight"
              class="task-row-background @(GetRowCssClass(task, i))"
              fill="transparent" />
        
        <!-- Main task bar -->
        <rect x="@x" y="@(y + TaskBarVerticalMargin)" 
              width="@width" height="@TaskBarHeight"
              class="task-bar @GetTaskCssClass(task)"
              fill="@GetTaskColor(task)"
              stroke="@GetTaskBorderColor(task)"
              stroke-width="1"
              rx="2" ry="2"
              @onclick="() => HandleTaskClick(task, taskIndex)"
              @onmouseover="() => HandleTaskMouseOver(task, taskIndex)"
              @onmouseout="() => HandleTaskMouseOut(task, taskIndex)" />
        
        <!-- Task label (if enabled and space permits) -->
        @if (ShowTaskLabels && width >= MinWidthForLabel)
        {
            <text x="@(x + TaskLabelPadding)" y="@(y + RowHeight/2 + 4)" 
                  class="task-label"
                  fill="@GetTaskLabelColor(task)"
                  font-size="12"
                  font-family="var(--gantt-font-family)"
                  text-anchor="start">
                @task.Name
            </text>
        }
        
        <!-- Progress indicator (if task has progress) -->
        @if (task.Progress > 0)
        {
            var progressWidth = width * (task.Progress / 100.0);
            <rect x="@x" y="@(y + TaskBarVerticalMargin)" 
                  width="@progressWidth" height="@TaskBarHeight"
                  class="task-progress"
                  fill="@GetProgressColor(task)"
                  rx="2" ry="2" />
        }
    }
}
```

### **🎯 Interactive Elements**
```csharp
private async Task HandleTaskClick(GanttTask task, int index)
{
    if (!AllowTaskSelection) return;
    
    Logger.LogDebug("Task clicked: {TaskId} at index {Index}", task.Id, index);
    
    // Update selection state
    SelectedTaskIndex = index;
    StateHasChanged();
    
    // Notify parent component
    await OnTaskSelected.InvokeAsync(task);
}

private async Task HandleTaskMouseOver(GanttTask task, int index)
{
    HoveredTaskIndex = index;
    StateHasChanged();
    
    await OnTaskHovered.InvokeAsync(task);
}

private async Task HandleTaskMouseOut(GanttTask task, int index)
{
    HoveredTaskIndex = null;
    StateHasChanged();
}
```

## 📱 **Header Integration**

### **🔗 TimelineHeader Component Integration**
```razor
<!-- Header rendering using extracted TimelineHeader component -->
<TimelineHeader StartDate="@StartDate" 
               EndDate="@EndDate" 
               EffectiveDayWidth="@EffectiveDayWidth" 
               ZoomLevel="@ZoomLevel"
               HeaderMonthHeight="@HeaderMonthHeight"
               HeaderDayHeight="@HeaderDayHeight"
               CssClass="@GetHeaderCssClass()" />
```

### **⚙️ Header Service Implementation**
```csharp
public class TimelineHeaderService : ITimelineHeaderService
{
    private readonly DateFormatHelper _dateFormatter;
    private readonly IUniversalLogger _logger;
    
    public TimelineHeaderResult GenerateHeaderPeriods(
        DateTime startDate, 
        DateTime endDate, 
        TimelineZoomLevel zoomLevel, 
        double effectiveDayWidth)
    {
        var config = TimelineHeaderAdapter.GetHeaderConfiguration(zoomLevel, effectiveDayWidth);
        var timeSpanDays = (endDate - startDate).Days;
        var shouldCollapse = TimelineHeaderAdapter.ShouldCollapseHeaders(config, effectiveDayWidth, timeSpanDays);
        
        if (shouldCollapse)
        {
            // Single row header when space is constrained
            return new TimelineHeaderResult
            {
                PrimaryPeriods = GeneratePrimaryPeriods(startDate, endDate, config),
                SecondaryPeriods = new List<HeaderPeriod>(),
                IsCollapsed = true
            };
        }
        else
        {
            // Two-row header with primary and secondary periods
            return new TimelineHeaderResult
            {
                PrimaryPeriods = GeneratePrimaryPeriods(startDate, endDate, config),
                SecondaryPeriods = GenerateSecondaryPeriods(startDate, endDate, config, effectiveDayWidth),
                IsCollapsed = false
            };
        }
    }
    
    private List<HeaderPeriod> GeneratePrimaryPeriods(DateTime start, DateTime end, TimelineHeaderConfiguration config)
    {
        var periods = new List<HeaderPeriod>();
        var current = TimelineHeaderAdapter.GetPeriodStart(start, config.PrimaryUnit);
        var increment = TimelineHeaderAdapter.GetDateIncrement(config.PrimaryUnit);
        
        while (current < end)
        {
            var nextPeriod = increment(current);
            var periodWidth = TimelineHeaderAdapter.GetPeriodWidth(current, config.PrimaryUnit, effectiveDayWidth);
            
            periods.Add(new HeaderPeriod
            {
                Start = current,
                End = nextPeriod,
                Width = periodWidth,
                Label = _dateFormatter.FormatDate(current, config.PrimaryFormat),
                Level = HeaderLevel.Primary
            });
            
            current = nextPeriod;
        }
        
        return periods;
    }
}
```

## 🎨 **CSS Implementation**

### **🎯 Component Styles**
```css
/* timeline-view.css */
.timeline-container {
    position: relative;
    overflow: hidden;
    height: 100%;
    
    /* CSS custom properties for theming */
    --header-month-height: 32px;
    --header-day-height: 24px;
    --task-bar-height: 24px;
    --task-bar-margin: 4px;
    
    /* Performance optimizations */
    contain: layout style paint;
}

.timeline-scroll-container {
    width: 100%;
    height: 100%;
    overflow: auto;
    scroll-behavior: smooth;
}

.timeline-svg {
    display: block;
    background: var(--gantt-timeline-background, #ffffff);
}

/* Grid lines */
.timeline-grid-line {
    stroke: var(--gantt-grid-color, #e0e0e0);
    stroke-width: 1px;
    shape-rendering: crispEdges; /* Crisp 1px lines */
}

.timeline-month-line {
    stroke: var(--gantt-month-boundary-color, #bdbdbd);
    stroke-width: 2px;
    shape-rendering: crispEdges;
}

/* Task bars */
.task-row-background {
    transition: fill 0.15s ease;
}

.task-row-background.selected {
    fill: var(--gantt-selection-background, rgba(33, 150, 243, 0.1));
}

.task-row-background.hovered {
    fill: var(--gantt-hover-background, rgba(0, 0, 0, 0.04));
}

.task-bar {
    cursor: pointer;
    transition: stroke-width 0.15s ease, opacity 0.15s ease;
    fill: var(--gantt-task-color, #2196f3);
    stroke: var(--gantt-task-border, #1976d2);
}

.task-bar:hover {
    stroke-width: 2px;
    filter: brightness(1.1);
}

.task-bar.selected {
    stroke: var(--gantt-selection-color, #ff5722);
    stroke-width: 2px;
}

.task-bar.critical {
    fill: var(--gantt-critical-task-color, #f44336);
    stroke: var(--gantt-critical-task-border, #d32f2f);
}

.task-progress {
    fill: var(--gantt-progress-color, #4caf50);
    pointer-events: none; /* Don't interfere with task bar clicks */
}

/* Task labels */
.task-label {
    pointer-events: none;
    user-select: none;
    font-family: var(--gantt-font-family, 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif);
    fill: var(--gantt-task-label-color, #333333);
}

/* Responsive adjustments */
@media (max-width: 768px) {
    .timeline-container {
        --task-bar-height: 20px;
        --task-bar-margin: 6px;
    }
    
    .task-label {
        font-size: 10px;
    }
}

/* High DPI display optimizations */
@media (-webkit-min-device-pixel-ratio: 2), (min-resolution: 192dpi) {
    .timeline-svg {
        shape-rendering: geometricPrecision;
    }
}
```

## ⚡ **Performance Implementation**

### **🚀 Rendering Optimizations**
```csharp
private bool _dimensionsCalculated = false;
private double _cachedTotalWidth;
private double _cachedTotalHeight;

private double TotalWidth
{
    get
    {
        if (!_dimensionsCalculated)
        {
            CalculateDimensions();
        }
        return _cachedTotalWidth;
    }
}

private void CalculateDimensions()
{
    var timeSpanDays = (EndDate - StartDate).Days;
    _cachedTotalWidth = timeSpanDays * EffectiveDayWidth;
    _cachedTotalHeight = (Tasks?.Count ?? 0) * RowHeight;
    _dimensionsCalculated = true;
}

protected override bool ShouldRender()
{
    // Only re-render if essential parameters changed
    return _lastZoomLevel != ZoomLevel || 
           Math.Abs(_lastZoomFactor - ZoomFactor) > 0.001 ||
           _lastTaskCount != (Tasks?.Count ?? 0);
}
```

### **📊 Memory Management**
```csharp
public void Dispose()
{
    // Clear event handlers
    OnTaskSelected = default;
    OnTaskHovered = default;
    OnZoomLevelChanged = default;
    OnZoomFactorChanged = default;
    
    // Clear cached data
    _cachedTotalWidth = 0;
    _cachedTotalHeight = 0;
    _dimensionsCalculated = false;
    
    Logger.LogDebug("TimelineView disposed");
}
```

## 🧪 **Testing Implementation**

### **🔍 Unit Test Examples**
```csharp
[Test]
public void EffectiveDayWidth_WithDifferentZoomLevels_ReturnsCorrectValues()
{
    // Arrange
    var component = CreateTimelineViewComponent();
    
    // Act & Assert
    component.SetZoomLevel(TimelineZoomLevel.YearQuarter3px);
    Assert.AreEqual(3.0, component.GetCurrentDayWidth(), 0.1);
    
    component.SetZoomLevel(TimelineZoomLevel.WeekDay97px);
    Assert.AreEqual(97.0, component.GetCurrentDayWidth(), 0.1);
}

[Test]
public void TaskWidth_WithMinimumConstraint_RespectsMinimumWidth()
{
    // Arrange
    var component = CreateTimelineViewComponent();
    var shortTask = new GanttTask { StartDate = DateTime.Today, EndDate = DateTime.Today };
    
    // Act
    var width = component.CalculateTaskWidth(shortTask);
    
    // Assert
    Assert.GreaterOrEqual(width, TaskDisplayConstants.MIN_TASK_WIDTH);
}
```

### **🔄 Integration Test Examples**
```csharp
[Test]
public async Task TaskSelection_UpdatesStateAndNotifiesParent()
{
    // Arrange
    var taskSelectedCalled = false;
    var component = CreateTimelineViewComponent();
    component.OnTaskSelected = EventCallback.Factory.Create<GanttTask>(this, (task) => 
    {
        taskSelectedCalled = true;
    });
    
    // Act
    await component.HandleTaskClick(testTask, 0);
    
    // Assert
    Assert.IsTrue(taskSelectedCalled);
    Assert.AreEqual(0, component.SelectedTaskIndex);
}
```

## 🌐 **Internationalization Implementation**

### **📅 Date Formatting Integration**
```csharp
private string FormatHeaderDate(DateTime date, string formatKey)
{
    try
    {
        return I18N.FormatDate(date, formatKey);
    }
    catch (Exception ex)
    {
        Logger.LogWarning(ex, "Failed to format date {Date} with key {FormatKey}", date, formatKey);
        return date.ToString("MMM yyyy"); // Fallback format
    }
}

// Example format keys by zoom level
private string GetPrimaryFormatKey(TimelineZoomLevel zoomLevel)
{
    return zoomLevel switch
    {
        TimelineZoomLevel.WeekDay97px => "date.week-with-year",
        TimelineZoomLevel.MonthDay48px => "date.month-with-year", 
        TimelineZoomLevel.YearQuarter3px => "date.year",
        _ => "date.month-short"
    };
}
```

### **🎯 Resource Key Management**
```csharp
// Timeline zoom level display names
private string GetZoomLevelDisplayName(TimelineZoomLevel level)
{
    var key = $"ZoomLevel.{level}";
    return I18N.GetString(key, level.ToString());
}

// Accessible labels for screen readers
private string GetTaskAriaLabel(GanttTask task)
{
    return I18N.GetString("timeline.task.aria-label", 
        task.Name, 
        I18N.FormatDate(task.StartDate, "date.short"),
        I18N.FormatDate(task.EndDate, "date.short"));
}
```

---

*This implementation guide provides the technical foundation for building and maintaining the TimelineView component with proper architecture, performance, and maintainability.*
