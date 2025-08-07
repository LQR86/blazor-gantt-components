# 📚 TimelineView Component - Usage Guide

## 🎯 **Quick Start**

The TimelineView component is the core timeline visualization component that can be used standalone or integrated with other Gantt components.

### **🚀 Basic Usage**
```razor
@page "/simple-timeline"

<TimelineView Tasks="@tasks" 
              OnTaskSelected="HandleTaskSelected"
              ZoomLevel="TimelineZoomLevel.MonthDay48px" />

@code {
    private List<GanttTask> tasks = new()
    {
        new() { Id = 1, Name = "Design Phase", StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(14) },
        new() { Id = 2, Name = "Development", StartDate = DateTime.Today.AddDays(7), EndDate = DateTime.Today.AddDays(35) },
        new() { Id = 3, Name = "Testing", StartDate = DateTime.Today.AddDays(30), EndDate = DateTime.Today.AddDays(42) }
    };
    
    private void HandleTaskSelected(GanttTask task)
    {
        Console.WriteLine($"Selected task: {task.Name}");
    }
}
```

## 📋 **Parameter Reference**

### **🔧 Core Parameters**

#### **Data Binding**
```razor
<!-- Required: Task data -->
<TimelineView Tasks="@taskList" />

<!-- Optional: Event handlers -->
<TimelineView Tasks="@taskList"
              OnTaskSelected="HandleTaskSelection"
              OnTaskHovered="HandleTaskHover" />
```

#### **Zoom Control**
```razor
<!-- Basic zoom configuration -->
<TimelineView Tasks="@taskList"
              ZoomLevel="TimelineZoomLevel.QuarterMonth24px"
              ZoomFactor="1.0" />

<!-- With zoom change notifications -->
<TimelineView Tasks="@taskList"
              ZoomLevel="@currentZoomLevel"
              ZoomFactor="@currentZoomFactor"
              OnZoomLevelChanged="@((level) => currentZoomLevel = level)"
              OnZoomFactorChanged="@((factor) => currentZoomFactor = factor)" />
```

#### **Layout Customization**
```razor
<!-- Custom dimensions -->
<TimelineView Tasks="@taskList"
              RowHeight="40"
              HeaderMonthHeight="36"
              HeaderDayHeight="28" />

<!-- Custom styling -->
<TimelineView Tasks="@taskList"
              CssClass="custom-timeline" />
```

#### **Feature Toggles**
```razor
<!-- Control visual features -->
<TimelineView Tasks="@taskList"
              ShowGridLines="true"
              ShowTaskLabels="true"
              AllowTaskSelection="true" />

<!-- For GanttComposer integration -->
<TimelineView Tasks="@taskList"
              HideScrollbar="true" />
```

### **📊 Parameter Details**

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Tasks` | `List<GanttTask>?` | `null` | Task data to display |
| `OnTaskSelected` | `EventCallback<GanttTask>` | - | Task selection event |
| `OnTaskHovered` | `EventCallback<GanttTask>` | - | Task hover event |
| `ZoomLevel` | `TimelineZoomLevel` | `QuarterMonth24px` | Zoom preset level |
| `ZoomFactor` | `double` | `1.0` | Zoom multiplier (currently locked to 1.0) |
| `OnZoomLevelChanged` | `EventCallback<TimelineZoomLevel>` | - | Zoom level change event |
| `OnZoomFactorChanged` | `EventCallback<double>` | - | Zoom factor change event |
| `RowHeight` | `int` | `32` | Height of each task row in pixels |
| `HeaderMonthHeight` | `int` | `32` | Height of primary header row |
| `HeaderDayHeight` | `int` | `24` | Height of secondary header row |
| `ShowGridLines` | `bool` | `true` | Show vertical day grid lines |
| `ShowTaskLabels` | `bool` | `true` | Show task names on bars |
| `AllowTaskSelection` | `bool` | `true` | Enable task selection |
| `HideScrollbar` | `bool` | `false` | Hide scrollbars (for integration) |
| `CssClass` | `string` | `""` | Additional CSS classes |

## 🎚️ **Zoom System Usage**

### **📈 Available Zoom Levels**
```csharp
// Strategic overview levels (3-6px per day)
TimelineZoomLevel.YearQuarter3px     // Long-term planning
TimelineZoomLevel.YearQuarter4px     // Multi-year roadmaps  
TimelineZoomLevel.YearQuarter6px     // Annual overview

// Compact planning levels (8-12px per day)
TimelineZoomLevel.Month8px           // Minimal monthly view
TimelineZoomLevel.Month12px          // Standard monthly view

// Project management levels (17-24px per day)
TimelineZoomLevel.QuarterMonth17px   // Quarterly planning
TimelineZoomLevel.QuarterMonth24px   // Default project view

// Task scheduling levels (34-48px per day)
TimelineZoomLevel.MonthDay34px       // Monthly task view
TimelineZoomLevel.MonthDay48px       // Detailed scheduling

// Sprint management levels (68-97px per day)
TimelineZoomLevel.WeekDay68px        // Sprint planning
TimelineZoomLevel.WeekDay97px        // Daily task management
```

### **🔄 Zoom Control Examples**
```razor
@* Manual zoom level selection *@
<select @bind="selectedZoomLevel">
    @foreach (var level in Enum.GetValues<TimelineZoomLevel>())
    {
        <option value="@level">@GetZoomLevelName(level)</option>
    }
</select>

<TimelineView Tasks="@tasks" ZoomLevel="@selectedZoomLevel" />

@* Programmatic zoom control *@
<button @onclick="ZoomIn">Zoom In</button>
<button @onclick="ZoomOut">Zoom Out</button>
<button @onclick="ResetZoom">Reset Zoom</button>

@code {
    private TimelineZoomLevel selectedZoomLevel = TimelineZoomLevel.QuarterMonth24px;
    
    private void ZoomIn()
    {
        var currentIndex = (int)selectedZoomLevel;
        if (currentIndex < 10) // Max zoom level
        {
            selectedZoomLevel = (TimelineZoomLevel)(currentIndex + 1);
        }
    }
    
    private void ZoomOut()
    {
        var currentIndex = (int)selectedZoomLevel;
        if (currentIndex > 0) // Min zoom level
        {
            selectedZoomLevel = (TimelineZoomLevel)(currentIndex - 1);
        }
    }
    
    private void ResetZoom()
    {
        selectedZoomLevel = TimelineZoomLevel.QuarterMonth24px;
    }
}
```

## 🎯 **Common Usage Patterns**

### **📊 Project Dashboard**
```razor
@* High-level project overview *@
<div class="project-dashboard">
    <h3>Project Timeline Overview</h3>
    <TimelineView Tasks="@projectTasks"
                  ZoomLevel="TimelineZoomLevel.MonthDay34px"
                  OnTaskSelected="ShowTaskDetails"
                  ShowGridLines="true"
                  ShowTaskLabels="true"
                  RowHeight="36" />
</div>

@code {
    private List<GanttTask> projectTasks = GetProjectTasks();
    
    private void ShowTaskDetails(GanttTask task)
    {
        // Navigate to task detail page or show modal
        NavigationManager.NavigateTo($"/tasks/{task.Id}");
    }
}
```

### **🗓️ Sprint Planning**
```razor
@* Detailed sprint timeline *@
<div class="sprint-timeline">
    <h3>Sprint 15 - Daily Tasks</h3>
    <TimelineView Tasks="@sprintTasks"
                  ZoomLevel="TimelineZoomLevel.WeekDay68px"
                  OnTaskSelected="EditTask"
                  OnTaskHovered="ShowTaskTooltip"
                  RowHeight="32" />
</div>

@code {
    private List<GanttTask> sprintTasks = GetCurrentSprintTasks();
    
    private void EditTask(GanttTask task)
    {
        // Open task editor
        ShowTaskEditor(task);
    }
    
    private void ShowTaskTooltip(GanttTask task)
    {
        // Show tooltip with task details
        tooltipContent = $"{task.Name} - {task.Progress}% complete";
        showTooltip = true;
    }
}
```

### **📈 Portfolio Management**
```razor
@* Strategic portfolio view *@
<div class="portfolio-view">
    <h3>5-Year Portfolio Roadmap</h3>
    <TimelineView Tasks="@portfolioMilestones"
                  ZoomLevel="TimelineZoomLevel.YearQuarter4px"
                  OnTaskSelected="DrillDownToProject"
                  ShowTaskLabels="false"
                  RowHeight="28" />
</div>

@code {
    private List<GanttTask> portfolioMilestones = GetPortfolioMilestones();
    
    private void DrillDownToProject(GanttTask milestone)
    {
        // Navigate to detailed project view
        currentProjectId = milestone.ProjectId;
        currentView = "project-detail";
    }
}
```

## 🔗 **Integration Patterns**

### **🎛️ With TimelineZoomControls**
```razor
<div class="timeline-with-controls">
    <!-- Zoom controls -->
    <TimelineZoomControls @bind-ZoomLevel="currentZoomLevel"
                         @bind-ZoomFactor="currentZoomFactor"
                         Preset="TimelineZoomControlPreset.ProjectManager" />
    
    <!-- Timeline view -->
    <TimelineView Tasks="@tasks"
                  ZoomLevel="@currentZoomLevel"
                  ZoomFactor="@currentZoomFactor" />
</div>

@code {
    private TimelineZoomLevel currentZoomLevel = TimelineZoomLevel.QuarterMonth24px;
    private double currentZoomFactor = 1.0;
}
```

### **🔄 With State Management**
```razor
@* Coordinated state across multiple components *@
<div class="coordinated-view">
    <TaskList Tasks="@tasks" 
              SelectedTaskId="@selectedTaskId"
              OnTaskSelected="@((taskId) => selectedTaskId = taskId)" />
    
    <TimelineView Tasks="@tasks"
                  OnTaskSelected="@((task) => selectedTaskId = task.Id)"
                  ZoomLevel="@currentZoomLevel" />
</div>

@code {
    private int? selectedTaskId;
    private TimelineZoomLevel currentZoomLevel = TimelineZoomLevel.MonthDay48px;
    private List<GanttTask> tasks = LoadTasks();
}
```

### **📱 Responsive Usage**
```razor
@* Responsive timeline for different screen sizes *@
<div class="responsive-timeline">
    <TimelineView Tasks="@tasks"
                  ZoomLevel="@GetResponsiveZoomLevel()"
                  RowHeight="@GetResponsiveRowHeight()"
                  ShowTaskLabels="@IsLargeScreen()"
                  CssClass="@GetResponsiveCssClass()" />
</div>

@code {
    private TimelineZoomLevel GetResponsiveZoomLevel()
    {
        // Adjust zoom based on screen size
        return IsLargeScreen() ? TimelineZoomLevel.MonthDay48px : TimelineZoomLevel.MonthDay34px;
    }
    
    private int GetResponsiveRowHeight()
    {
        return IsLargeScreen() ? 36 : 28;
    }
    
    private bool IsLargeScreen()
    {
        // Implement screen size detection
        return true; // Placeholder
    }
}
```

## 🎨 **Styling and Theming**

### **🎨 CSS Custom Properties**
```css
/* Custom timeline theme */
.custom-timeline {
    --gantt-timeline-background: #f8f9fa;
    --gantt-grid-color: #dee2e6;
    --gantt-task-color: #007bff;
    --gantt-task-border: #0056b3;
    --gantt-selection-color: #fd7e14;
    --gantt-hover-background: rgba(0, 123, 255, 0.1);
    --gantt-font-family: 'Inter', sans-serif;
}

/* Dark theme */
.dark-timeline {
    --gantt-timeline-background: #1a1a1a;
    --gantt-grid-color: #404040;
    --gantt-task-color: #4dabf7;
    --gantt-task-border: #339af0;
    --gantt-task-label-color: #ffffff;
}

/* Compact view */
.compact-timeline {
    --task-bar-height: 20px;
    --task-bar-margin: 6px;
}
```

### **🎯 Custom Task Styling**
```razor
<TimelineView Tasks="@tasks" CssClass="priority-colored-timeline" />

<style>
/* Priority-based task coloring */
.priority-colored-timeline .task-bar.high-priority {
    fill: #dc3545;
    stroke: #c82333;
}

.priority-colored-timeline .task-bar.medium-priority {
    fill: #ffc107;
    stroke: #e0a800;
}

.priority-colored-timeline .task-bar.low-priority {
    fill: #28a745;
    stroke: #1e7e34;
}
</style>

@code {
    // Set CSS classes based on task priority
    private string GetTaskCssClass(GanttTask task)
    {
        return task.Priority switch
        {
            TaskPriority.High => "high-priority",
            TaskPriority.Medium => "medium-priority",
            TaskPriority.Low => "low-priority",
            _ => ""
        };
    }
}
```

## ⚡ **Performance Best Practices**

### **📊 Large Dataset Handling**
```razor
@* Optimized for large task lists *@
<TimelineView Tasks="@visibleTasks"
              ShowTaskLabels="@(tasks.Count < 100)"
              ShowGridLines="@(currentZoomLevel >= TimelineZoomLevel.MonthDay34px)" />

@code {
    private List<GanttTask> allTasks = LoadAllTasks();
    
    // Filter to visible tasks only
    private List<GanttTask> visibleTasks => allTasks
        .Where(t => t.EndDate >= timelineStart && t.StartDate <= timelineEnd)
        .OrderBy(t => t.StartDate)
        .ToList();
}
```

### **🔄 State Update Optimization**
```razor
@code {
    private TimelineZoomLevel _lastZoomLevel;
    private double _lastZoomFactor;
    
    protected override bool ShouldRender()
    {
        // Only re-render if zoom parameters actually changed
        var shouldRender = _lastZoomLevel != currentZoomLevel || 
                          Math.Abs(_lastZoomFactor - currentZoomFactor) > 0.001;
        
        if (shouldRender)
        {
            _lastZoomLevel = currentZoomLevel;
            _lastZoomFactor = currentZoomFactor;
        }
        
        return shouldRender;
    }
}
```

## 🧪 **Testing Examples**

### **🔍 Component Testing**
```csharp
[Test]
public void TimelineView_WithTasks_RendersCorrectly()
{
    // Arrange
    var tasks = CreateTestTasks();
    var component = RenderComponent<TimelineView>(parameters => parameters
        .Add(p => p.Tasks, tasks)
        .Add(p => p.ZoomLevel, TimelineZoomLevel.MonthDay48px));
    
    // Assert
    Assert.IsTrue(component.Find(".timeline-svg").Exists);
    Assert.AreEqual(tasks.Count, component.FindAll(".task-bar").Count);
}

[Test]
public async Task TaskSelection_TriggersCallback()
{
    // Arrange
    var taskSelected = false;
    var component = RenderComponent<TimelineView>(parameters => parameters
        .Add(p => p.Tasks, CreateTestTasks())
        .Add(p => p.OnTaskSelected, (GanttTask task) => taskSelected = true));
    
    // Act
    await component.Find(".task-bar").ClickAsync();
    
    // Assert
    Assert.IsTrue(taskSelected);
}
```

## 🌐 **Accessibility**

### **♿ ARIA Support**
```razor
<!-- Timeline with proper ARIA labels -->
<TimelineView Tasks="@tasks"
              role="grid"
              aria-label="Project timeline"
              aria-rowcount="@tasks.Count"
              tabindex="0" />
```

### **⌨️ Keyboard Navigation**
```csharp
// Implement keyboard support
private async Task HandleKeyDown(KeyboardEventArgs e)
{
    switch (e.Key)
    {
        case "ArrowRight":
            await ZoomIn();
            break;
        case "ArrowLeft":
            await ZoomOut();
            break;
        case "Home":
            await ScrollToStart();
            break;
        case "End":
            await ScrollToEnd();
            break;
    }
}
```

---

*This usage guide provides comprehensive examples and best practices for effectively using the TimelineView component in various scenarios.*
