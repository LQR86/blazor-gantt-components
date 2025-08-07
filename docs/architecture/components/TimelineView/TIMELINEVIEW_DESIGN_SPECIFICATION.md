# 📅 TimelineView Component - Design Specification

## 🎯 **Component Overview**

The **TimelineView** is the most complex and performance-critical component in the BlazorGantt system. It provides an SVG-based visual timeline with advanced zoom capabilities, adaptive headers, and precise pixel-perfect rendering for Gantt chart visualization.

## 📋 **Requirements & Constraints**

### **🔧 Core Requirements**
- **SVG-Based Rendering**: Scalable vector graphics for crisp visualization at all zoom levels
- **Day-Level Precision**: Maximum granularity at day level (no hour/minute scheduling)
- **11-Level Zoom System**: Integral pixel widths from 3px to 97px per day
- **Adaptive Headers**: Dynamic header configuration based on zoom level
- **Task Visualization**: Clickable task bars with hover states and tooltips
- **Real-Time Updates**: Immediate server synchronization without batch operations
- **Pixel-Perfect Alignment**: Exact row alignment with TaskGrid component

### **🚫 Design Constraints**
- **No Batch Operations**: Single-operation CRUD with immediate feedback
- **No Sub-Day Scheduling**: Day-level granularity only
- **Horizontal Scaling Only**: Vertical dimensions (row height, header height) remain fixed
- **Browser Compatibility**: Consistent behavior across modern browsers
- **Performance Target**: Smooth rendering for 500+ tasks with 60fps scrolling

### **📏 Dimensional Standards**
- **Row Height**: Fixed 32px across all zoom levels
- **Header Height**: Total 56px (32px primary + 24px secondary)
- **Minimum Day Width**: 3px (TaskDisplayConstants.MIN_EFFECTIVE_DAY_WIDTH)
- **Maximum Day Width**: 97px (WeekDay97px zoom level)
- **Task Minimum Width**: 12px (TaskDisplayConstants.MIN_TASK_WIDTH)

## 🏗️ **Architecture Overview**

### **📊 Component Structure**
```
TimelineView.razor
├── Header Section (TimelineHeader component)
│   ├── Primary Header (Years, Quarters, Months)
│   └── Secondary Header (Quarters, Months, Days)
├── Body Section (SVG Container)
│   ├── Grid Lines (vertical day separators)
│   ├── Task Bars (rectangular SVG elements)
│   ├── Task Labels (text overlays)
│   └── Interactive Elements (hover/selection)
└── Scroll Container (horizontal/vertical scrolling)
```

### **🔄 Core Dependencies**
- **TimelineHeaderService**: Business logic for header period calculation
- **TimelineZoomService**: Zoom level configuration and day width calculation
- **TimelineHeaderAdapter**: Header configuration based on zoom level
- **DateFormatHelper**: Consistent date formatting across components
- **IGanttI18N**: Internationalization support
- **IUniversalLogger**: Comprehensive logging for debugging

## 🎚️ **Zoom System Architecture**

### **📈 11-Level Integral System**
The zoom system uses a carefully designed progression of integral pixel values:

| Level | Zoom Level | Day Width | Use Case | Header Pattern |
|-------|------------|-----------|----------|----------------|
| 1 | YearQuarter3px | 3px | Strategic overview | Year → Quarter |
| 2 | YearQuarter4px | 4px | Multi-year planning | Year → Quarter |
| 3 | YearQuarter6px | 6px | Annual overview | Year → Quarter |
| 4 | Month8px | 8px | Compact monthly | Month-only |
| 5 | Month12px | 12px | Monthly planning | Month-only |
| 6 | QuarterMonth17px | 17px | Quarterly view | Quarter → Month |
| 7 | QuarterMonth24px | 24px | Project phases | Quarter → Month |
| 8 | MonthDay34px | 34px | Monthly breakdown | Month → Day |
| 9 | MonthDay48px | 48px | Task scheduling | Month → Day |
| 10 | WeekDay68px | 68px | Sprint planning | Week → Day |
| 11 | WeekDay97px | 97px | Daily management | Week → Day |

### **🧮 Width Calculation Formula**
```csharp
EffectiveDayWidth = ZoomLevel.BaseDayWidth × ZoomFactor (clamped to 1.0)
TotalTimelineWidth = TimeSpanDays × EffectiveDayWidth
TaskBarWidth = TaskDurationDays × EffectiveDayWidth
```

### **📅 Timeline Boundary Calculation**
```csharp
// Timeline boundaries with 7-day padding
TimelineStart = TaskStartDate.Min() - 7 days
TimelineEnd = TaskEndDate.Max() + 7 days

// Align to header period boundaries
TimelineStart = TimelineHeaderAdapter.GetPeriodStart(TimelineStart, PrimaryUnit)
TimelineEnd = TimelineHeaderAdapter.GetPeriodEnd(TimelineEnd, PrimaryUnit)
```

## 📊 **Header System Design**

### **🔄 Dynamic Header Configuration**
Headers adapt based on zoom level using TimelineHeaderAdapter:

```csharp
public static class TimelineHeaderAdapter
{
    public static TimelineHeaderConfiguration GetHeaderConfiguration(
        TimelineZoomLevel zoomLevel, 
        double effectiveDayWidth)
    {
        return zoomLevel switch
        {
            TimelineZoomLevel.YearQuarter3px => new()
            {
                PrimaryUnit = TimelineHeaderUnit.Year,
                SecondaryUnit = TimelineHeaderUnit.Quarter,
                PrimaryFormat = "date.year",
                SecondaryFormat = "date.quarter-short"
            },
            // ... 11 total configurations
        };
    }
}
```

### **📏 Header Collapse Logic**
Headers automatically collapse to single row when space is insufficient:

```csharp
public static bool ShouldCollapseHeaders(
    TimelineHeaderConfiguration config, 
    double effectiveDayWidth, 
    int timeSpanDays)
{
    var estimatedPrimaryUnits = GetEstimatedPrimaryUnits(config.PrimaryUnit, timeSpanDays);
    var averagePrimaryWidth = (timeSpanDays * effectiveDayWidth) / estimatedPrimaryUnits;
    
    return averagePrimaryWidth < config.MinPrimaryWidth;
}
```

### **📐 Period Width Calculation**
```csharp
public static double GetPeriodWidth(
    DateTime periodStart, 
    TimelineHeaderUnit unit, 
    double effectiveDayWidth)
{
    var periodEnd = GetDateIncrement(unit)(periodStart);
    var days = (periodEnd - periodStart).Days;
    return days * effectiveDayWidth;
}
```

## 🎨 **Rendering Engine**

### **🖼️ SVG Architecture**
The timeline uses SVG for scalable, crisp rendering:

```razor
<svg class="timeline-svg" width="@(TotalWidth)" height="@(TotalHeight)">
    <!-- Grid lines for visual structure -->
    @for (var day = StartDate; day <= EndDate; day = day.AddDays(1))
    {
        var x = DayToPixel(day);
        <line x1="@x" y1="0" x2="@x" y2="@TotalHeight" class="timeline-grid-line" />
    }
    
    <!-- Task bars with interaction -->
    @if (Tasks != null)
    {
        @for (int i = 0; i < Tasks.Count; i++)
        {
            var task = Tasks[i];
            var y = i * RowHeight;
            var x = DayToPixel(task.StartDate);
            var width = CalculateTaskWidth(task);
            
            <rect x="@x" y="@(y + 4)" width="@width" height="@(RowHeight - 8)"
                  class="task-bar @GetTaskCssClass(task)"
                  @onclick="() => OnTaskSelected(task)"
                  @onmouseover="() => OnTaskHovered(task)" />
        }
    }
</svg>
```

### **📍 Pixel Conversion Functions**
```csharp
private double DayToPixel(DateTime date)
{
    return (date - StartDate).Days * EffectiveDayWidth;
}

private DateTime PixelToDay(double x)
{
    var days = x / EffectiveDayWidth;
    return StartDate.AddDays(days);
}

private double CalculateTaskWidth(GanttTask task)
{
    var durationDays = (task.EndDate - task.StartDate).Days + 1;
    return Math.Max(durationDays * EffectiveDayWidth, TaskDisplayConstants.MIN_TASK_WIDTH);
}
```

## 🔄 **Component Lifecycle**

### **🚀 Initialization Sequence**
1. **Parameter Validation**: Validate zoom level and factor parameters
2. **Service Injection**: Inject required services (logging, i18n, date formatting)
3. **Timeline Calculation**: Calculate start/end dates with padding
4. **Header Configuration**: Get appropriate header config for zoom level
5. **Dimension Calculation**: Calculate total width and height
6. **Initial Render**: Render headers and task bars

### **📊 Update Lifecycle**
1. **Parameter Change Detection**: OnParametersSet() handles parameter changes
2. **Zoom Recalculation**: Recalculate effective day width if zoom parameters change
3. **Layout Recalculation**: Update total width and pixel positions
4. **Header Update**: Regenerate header periods if needed
5. **Task Repositioning**: Update task bar positions and widths
6. **Re-render**: Trigger component re-render with StateHasChanged()

### **🧹 Cleanup Process**
```csharp
public void Dispose()
{
    // Remove event handlers
    // Clear cached calculations
    // Release resources
}
```

## 🎯 **Public API Design**

### **📋 Component Parameters**

#### **Core Data Parameters**
```csharp
[Parameter] public List<GanttTask>? Tasks { get; set; }
[Parameter] public EventCallback<GanttTask> OnTaskSelected { get; set; }
[Parameter] public EventCallback<GanttTask> OnTaskHovered { get; set; }
```

#### **Zoom Control Parameters**
```csharp
[Parameter] public TimelineZoomLevel ZoomLevel { get; set; } = TimelineZoomLevel.QuarterMonth24px;
[Parameter] public double ZoomFactor { get; set; } = 1.0;
[Parameter] public EventCallback<TimelineZoomLevel> OnZoomLevelChanged { get; set; }
[Parameter] public EventCallback<double> OnZoomFactorChanged { get; set; }
```

#### **Layout Parameters**
```csharp
[Parameter] public int RowHeight { get; set; } = 32;
[Parameter] public int HeaderMonthHeight { get; set; } = 32;
[Parameter] public int HeaderDayHeight { get; set; } = 24;
[Parameter] public string CssClass { get; set; } = string.Empty;
```

#### **Behavior Parameters**
```csharp
[Parameter] public bool ShowGridLines { get; set; } = true;
[Parameter] public bool ShowTaskLabels { get; set; } = true;
[Parameter] public bool AllowTaskSelection { get; set; } = true;
[Parameter] public bool HideScrollbar { get; set; } = false; // For GanttComposer integration
```

### **📤 Public Methods**
```csharp
// Zoom control methods
public void ZoomIn() => SetZoomLevel(GetNextZoomLevel(ZoomLevel));
public void ZoomOut() => SetZoomLevel(GetPreviousZoomLevel(ZoomLevel));
public void SetZoomLevel(TimelineZoomLevel level);
public void ResetZoom() => SetZoomLevel(TimelineZoomLevel.QuarterMonth24px);

// Navigation methods
public void ScrollToTask(int taskId);
public void ScrollToDate(DateTime date);
public void CenterOnTask(int taskId);

// Utility methods
public double GetCurrentDayWidth() => EffectiveDayWidth;
public DateTime GetTimelineStart() => StartDate;
public DateTime GetTimelineEnd() => EndDate;
```

## ⚡ **Performance Optimizations**

### **🖼️ Rendering Optimizations**
- **Integral Pixels**: All day widths use integral pixels for browser optimization
- **CSS Containment**: `contain: layout style paint` for isolated rendering
- **Virtual Scrolling Ready**: Architecture supports virtual scrolling implementation
- **Minimal Re-renders**: Strategic use of `StateHasChanged()` to avoid unnecessary updates

### **📊 Calculation Optimizations**
- **Cached Calculations**: Timeline dimensions cached until zoom parameters change
- **Lazy Header Generation**: Headers only regenerated when configuration changes
- **Efficient Date Math**: Optimized date calculations using days instead of DateTime operations

### **🎯 Memory Management**
- **Component Disposal**: Proper cleanup of event handlers and resources
- **Garbage Collection Friendly**: Minimal object allocation during rendering
- **Service Lifecycle**: Singleton services for shared calculations

## 🧪 **Testing Strategy**

### **🔍 Unit Tests Coverage**
- **Zoom Calculations**: Verify effective day width calculations across all levels
- **Timeline Boundaries**: Test date padding and alignment logic
- **Task Positioning**: Validate pixel conversion functions
- **Header Configuration**: Test adaptive header selection
- **Parameter Validation**: Ensure proper parameter clamping and validation

### **🔄 Integration Tests**
- **Component Rendering**: Verify SVG output at different zoom levels
- **Task Interaction**: Test click and hover event handling
- **Scroll Behavior**: Validate scroll synchronization with parent components
- **Zoom Transitions**: Test smooth transitions between zoom levels

### **📊 Performance Tests**
- **Large Dataset Rendering**: Test with 500+ tasks
- **Zoom Transition Speed**: Measure transition performance
- **Memory Usage**: Monitor memory allocation during operations
- **Scroll Performance**: Validate 60fps scrolling performance

## 🌐 **Internationalization Support**

### **📅 Date Formatting**
```csharp
// Localized date formats through IGanttI18N
public string FormatHeaderDate(DateTime date, string formatKey)
{
    return I18N.FormatDate(date, formatKey);
}

// Header format examples by zoom level
TimelineZoomLevel.WeekDay97px => "date.week-with-year" // "Week 15, 2025"
TimelineZoomLevel.MonthDay48px => "date.month-short"   // "Mar"
TimelineZoomLevel.YearQuarter3px => "date.year"       // "2025"
```

### **🎯 Resource Keys**
All zoom levels have dedicated internationalization keys:
- `ZoomLevel.{LevelName}` - Display name
- `ZoomLevel.{LevelName}.Description` - Usage description
- Header format keys for consistent date display

## 🔐 **Backward Compatibility**

### **✅ API Preservation**
- All existing TimelineView parameters remain unchanged
- Default zoom parameters maintain current behavior (24px day width)
- CSS custom properties preserved for external styling
- Event signatures unchanged for component integration

### **🔄 Migration Path**
- **Phase 1**: New zoom parameters with safe defaults
- **Phase 2**: Enhanced features (animations, gestures)
- **Phase 3**: Advanced optimizations (virtual scrolling, LOD)

---

*This specification serves as the authoritative design document for TimelineView component architecture, ensuring consistent implementation and maintenance of this critical component.*
