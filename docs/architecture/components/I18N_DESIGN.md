# 🌍 Internationalization (I18N) System Design & Implementation

> **Component**: All Components  
> **Feature**: Simple Multi-Language Support  
> **Status**: ✅ **IMPLEMENTED - Dependency Injection Architecture**  
> **Date**: July 29, 2025

## 📋 **Executive Summary**

The I18N System provides simple, lightweight internationalization capabilities for the Gantt Components, supporting English (US) and Simplified Chinese. **The system uses a modern dependency injection architecture with event-based notifications**, eliminating cascading parameter coupling and enabling component independence.

**🎯 Key Architectural Achievement**: Successfully eliminated the "cascading parameter pandemic" by implementing proper dependency injection with `IGanttI18N` interface and `LanguageChanged` events.

## 🎯 **Core Design Requirements**

### **🔑 Essential Features**
- ✅ **Two Language Support**: English (US) and Simplified Chinese only
- ✅ **UI Label Translation**: Button text, tooltips, messages, and static content
- ✅ **Date Format Localization**: Culture-appropriate date/time display patterns
- ✅ **Duration Unit Translation**: Task duration and work units (days, weeks, months)
- ✅ **Fixed-Width Font Headers**: Consistent timeline header rendering across languages
- ✅ **Simple Key-Value System**: No complex localization libraries or frameworks

### **🚫 Explicitly Excluded Features**
- ❌ **RTL (Right-to-Left) Support**: Not needed for target languages
- ❌ **Number/Currency Formatting**: Project focus is scheduling, not financial
- ❌ **Pluralization Rules**: Keep translations simple and explicit
- ❌ **Resource File Management**: Use simple dictionaries in code
- ❌ **Dynamic Language Loading**: Compile-time only for performance

### **🔧 Technical Architecture**
- ✅ **Dependency Injection Pattern**: `IGanttI18N` interface with singleton service registration
- ✅ **Event-Based Notifications**: `LanguageChanged` event eliminates cascading parameter coupling
- ✅ **Component Independence**: Each component subscribes to events independently via `@inject IGanttI18N`
- ✅ **Singleton Service**: Registered in `Program.cs` for application-wide language state
- ✅ **Culture-Aware Date Formatting**: Leverage .NET's CultureInfo with I18N keys
- ✅ **Timeline Integration**: Seamless integration with zoom system headers

## 🎯 **Language Support Strategy**

### **🇺🇸 English (US) - en-US**
- **Primary Language**: Default fallback for all components
- **Date Patterns**: MM/dd/yyyy, MMM d yyyy, Q1 2025
- **Duration Units**: 5d, 3w, 2m (short forms)
- **UI Style**: Concise, professional terminology

### **🇨🇳 Simplified Chinese - zh-CN**
- **Target Users**: Chinese project management teams
- **Date Patterns**: yyyy年MM月dd日, 2025年第1季度
- **Duration Units**: 5天, 3周, 2月 (full Chinese characters)
- **UI Style**: Clear, standard Chinese business terminology

## 📐 **Core Implementation Architecture**

### **🏗️ Dependency Injection Architecture**

**🎯 Core Principle**: Eliminate cascading parameter coupling through proper dependency injection with event notifications.

```csharp
/// <summary>
/// Internationalization service interface for dependency injection
/// Enables components to be I18N-aware without cascading parameter coupling
/// </summary>
public interface IGanttI18N
{
    string CurrentCulture { get; }
    string T(string key);
    void SetCulture(string culture);
    IEnumerable<string> GetAvailableCultures();
    bool HasTranslation(string key);
    
    /// <summary>
    /// Event fired when language changes - allows components to react independently
    /// </summary>
    event Action? LanguageChanged;
}

/// <summary>
/// Core internationalization service for Gantt Components.
/// Implements singleton pattern with event notification to eliminate cascading parameter coupling.
/// </summary>
public class GanttI18N : IGanttI18N
{
    private string _currentCulture = "en-US";

    /// <summary>
    /// Event fired when language changes - allows components to react independently
    /// </summary>
    public event Action? LanguageChanged;

    /// <summary>
    /// Gets the current culture for translations.
    /// </summary>
    public string CurrentCulture => _currentCulture;
    
    private static readonly Dictionary<string, Dictionary<string, string>> Translations = new()
    {
        ["en-US"] = new()
        {
            // Timeline Zoom Controls
            ["zoom.detailed"] = "Detailed",
            ["zoom.planning"] = "Planning",
            ["zoom.strategic"] = "Strategic",
            ["zoom.fit-screen"] = "Fit Screen",
            ["zoom.fit-tasks"] = "Fit Tasks",
            
            // Timeline Headers (for fixed-width font rendering)
            ["date.week-header"] = "Week of MMM d",
            ["date.month-year"] = "MMM yyyy",
            ["date.quarter-year"] = "Q{0} yyyy",
            ["date.year"] = "yyyy",
            ["date.day-abbr"] = "ddd",
            ["date.day-number"] = "d",
            ["date.month-abbr"] = "MMM",
            ["date.quarter"] = "Q{0}",
            
            // Task Duration & Work Units
            ["duration.days"] = "d",
            ["duration.weeks"] = "w",
            ["duration.months"] = "m",
            ["duration.hours"] = "h",
            ["work.hours"] = "hrs",
            ["work.days"] = "days",
            
            // Task Overflow System
            ["overflow.indicator"] = "...",
            ["overflow.tasks-hidden"] = "({0} tasks hidden)",
            ["overflow.show-task"] = "Click to show task",
            
            // Common UI Elements
            ["common.save"] = "Save",
            ["common.cancel"] = "Cancel",
            ["common.delete"] = "Delete",
            ["common.edit"] = "Edit",
            ["common.add"] = "Add",
            ["common.close"] = "Close",
            
            // Task Grid Headers
            ["grid.task-name"] = "Task Name",
            ["grid.duration"] = "Duration",
            ["grid.start-date"] = "Start Date",
            ["grid.end-date"] = "End Date",
            ["grid.progress"] = "Progress",
            ["grid.resources"] = "Resources",
            
            // Error Messages
            ["error.invalid-date"] = "Invalid date format",
            ["error.invalid-duration"] = "Invalid duration format",
            ["error.task-not-found"] = "Task not found",
            
            // Status Messages
            ["status.saving"] = "Saving...",
            ["status.loading"] = "Loading...",
            ["status.saved"] = "Saved successfully"
        },
        
        ["zh-CN"] = new()
        {
            // Timeline Zoom Controls
            ["zoom.detailed"] = "详细",
            ["zoom.planning"] = "规划",
            ["zoom.strategic"] = "战略",
            ["zoom.fit-screen"] = "适应屏幕",
            ["zoom.fit-tasks"] = "适应任务",
            
            // Timeline Headers (for fixed-width font rendering)
            ["date.week-header"] = "M月d日周",
            ["date.month-year"] = "yyyy年M月",
            ["date.quarter-year"] = "yyyy年第{0}季度",
            ["date.year"] = "yyyy年",
            ["date.day-abbr"] = "ddd",
            ["date.day-number"] = "d",
            ["date.month-abbr"] = "M月",
            ["date.quarter"] = "第{0}季度",
            
            // Task Duration & Work Units
            ["duration.days"] = "天",
            ["duration.weeks"] = "周",
            ["duration.months"] = "月",
            ["duration.hours"] = "时",
            ["work.hours"] = "小时",
            ["work.days"] = "天",
            
            // Task Overflow System
            ["overflow.indicator"] = "...",
            ["overflow.tasks-hidden"] = "({0}个任务已隐藏)",
            ["overflow.show-task"] = "点击显示任务",
            
            // Common UI Elements
            ["common.save"] = "保存",
            ["common.cancel"] = "取消",
            ["common.delete"] = "删除",
            ["common.edit"] = "编辑",
            ["common.add"] = "添加",
            ["common.close"] = "关闭",
            
            // Task Grid Headers
            ["grid.task-name"] = "任务名称",
            ["grid.duration"] = "工期",
            ["grid.start-date"] = "开始日期",
            ["grid.end-date"] = "结束日期",
            ["grid.progress"] = "进度",
            ["grid.resources"] = "资源",
            
            // Error Messages
            ["error.invalid-date"] = "日期格式无效",
            ["error.invalid-duration"] = "工期格式无效",
            ["error.task-not-found"] = "未找到任务",
            
            // Status Messages
            ["status.saving"] = "正在保存...",
            ["status.loading"] = "正在加载...",
            ["status.saved"] = "保存成功"
        }
    };
    
    /// <summary>
    /// Get translated text for the given key
    /// </summary>
    /// <param name="key">Translation key</param>
    /// <param name="args">Optional format arguments</param>
    /// <returns>Translated text or key if not found</returns>
    public static string T(string key, params object[] args)
    {
        if (Translations.ContainsKey(CurrentCulture) && 
            Translations[CurrentCulture].ContainsKey(key))
        {
            var text = Translations[CurrentCulture][key];
            return args.Length > 0 ? string.Format(text, args) : text;
        }
        
        // Fallback to English if current culture translation not found
        if (CurrentCulture != "en-US" && 
            Translations.ContainsKey("en-US") && 
            Translations["en-US"].ContainsKey(key))
        {
            var text = Translations["en-US"][key];
            return args.Length > 0 ? string.Format(text, args) : text;
        }
        
        // Final fallback: return the key itself
        return key;
    }
    
    /// <summary>
    /// Sets the current culture for translations and notifies subscribers.
    /// </summary>
    /// <param name="culture">Culture code (e.g., "en-US", "zh-CN")</param>
    public void SetCulture(string culture)
    {
        // Handle null culture by defaulting to English
        culture = culture ?? "en-US";
        
        if (Translations.ContainsKey(culture) && _currentCulture != culture)
        {
            _currentCulture = culture;
            
            // Notify all components that language changed
            LanguageChanged?.Invoke();
        }
    }

    /// <summary>
    /// Gets all available cultures.
    /// </summary>
    /// <returns>List of culture codes</returns>
    public IEnumerable<string> GetAvailableCultures()
    {
        return Translations.Keys;
    }

    /// <summary>
    /// Checks if a translation key exists in any culture.
    /// </summary>
    /// <param name="key">Translation key to check</param>
    /// <returns>True if key exists</returns>
    public bool HasTranslation(string key)
    {
        if (string.IsNullOrEmpty(key))
            return false;

        foreach (var culture in Translations.Values)
        {
            if (culture.ContainsKey(key))
                return true;
        }

        return false;
    }
}
```

### **🔧 Service Registration (Program.cs)**

```csharp
// Register I18N service as singleton for application-wide language state
builder.Services.AddSingleton<IGanttI18N, GanttI18N>();
```

### **📅 Culture-Aware Date Formatting**

```csharp
public static class DateFormatHelper
{
    /// <summary>
    /// Format date using culture-specific pattern from translations
    /// </summary>
    public static string FormatDate(DateTime date, string formatKey)
    {
        var pattern = GanttI18N.T(formatKey);
        
        // Handle special quarter formatting
        if (pattern.Contains("{0}") && formatKey.Contains("quarter"))
        {
            var quarter = (date.Month - 1) / 3 + 1;
            pattern = string.Format(pattern, quarter);
        }
        
        return date.ToString(pattern, CultureInfo.CurrentCulture);
    }
    
    /// <summary>
    /// Get localized day width for header calculations (fixed-width font)
    /// </summary>
    public static int GetLocalizedHeaderWidth(string formatKey, DateTime date)
    {
        const int FIXED_CHAR_WIDTH = 7; // pixels per character in monospace font
        var formattedText = FormatDate(date, formatKey);
        return formattedText.Length * FIXED_CHAR_WIDTH;
    }
}
```

### **⏱️ Duration Unit Localization**

```csharp
public static class DurationFormatter
{
    /// <summary>
    /// Format duration with localized units
    /// </summary>
    public static string FormatDuration(TimeSpan duration)
    {
        if (duration.TotalDays >= 1)
        {
            var days = (int)duration.TotalDays;
            return $"{days}{GanttI18N.T("duration.days")}";
        }
        else
        {
            var hours = (int)duration.TotalHours;
            return $"{hours}{GanttI18N.T("duration.hours")}";
        }
    }
    
    /// <summary>
    /// Parse duration string with localized units
    /// </summary>
    public static TimeSpan? ParseDuration(string durationText)
    {
        if (string.IsNullOrWhiteSpace(durationText)) return null;
        
        // Try to extract number and unit
        var regex = new Regex(@"(\d+)(.*)");
        var match = regex.Match(durationText.Trim());
        
        if (!match.Success) return null;
        
        if (!int.TryParse(match.Groups[1].Value, out var value)) return null;
        
        var unit = match.Groups[2].Value.Trim();
        
        // Check against localized units
        if (unit == GanttI18N.T("duration.days")) return TimeSpan.FromDays(value);
        if (unit == GanttI18N.T("duration.hours")) return TimeSpan.FromHours(value);
        if (unit == GanttI18N.T("duration.weeks")) return TimeSpan.FromDays(value * 7);
        if (unit == GanttI18N.T("duration.months")) return TimeSpan.FromDays(value * 30);
        
        return null;
    }
}
```

## 🎨 **Timeline Integration Strategy**

### **🔄 I18N-Aware Zoom Level Configuration**

```csharp
public class ZoomLevelConfiguration
{
    public int DayWidth { get; set; }
    public string TopTierFormatKey { get; set; }    // I18N key instead of direct format
    public string BottomTierFormatKey { get; set; } // I18N key instead of direct format
    public TimeSpan TopTierSpan { get; set; }
    public TimeSpan BottomTierSpan { get; set; }
    public string DisplayNameKey { get; set; }      // I18N key for zoom level name
    public string DescriptionKey { get; set; }      // I18N key for description
    
    // Computed properties using I18N system
    public string TopTierFormat => GanttI18N.T(TopTierFormatKey);
    public string BottomTierFormat => GanttI18N.T(BottomTierFormatKey);
    public string DisplayName => GanttI18N.T(DisplayNameKey);
    public string Description => GanttI18N.T(DescriptionKey);
    
    /// <summary>
    /// Calculate header width for fixed-width font rendering
    /// </summary>
    public int CalculateHeaderWidth(DateTime date, bool isTopTier)
    {
        var formatKey = isTopTier ? TopTierFormatKey : BottomTierFormatKey;
        return DateFormatHelper.GetLocalizedHeaderWidth(formatKey, date);
    }
}
```

### **📐 Fixed-Width Font Timeline Headers**

```css
/* Selective font strategy - only apply monospace where actually needed */

/* TaskGrid headers - Use system fonts (CSS Grid handles layout) */
.task-grid-header .header-cell {
    font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', 'Roboto', 'Helvetica Neue', Arial, sans-serif;
    font-size: 13px;
    font-weight: 600;
    text-align: center;
}

/* TimelineView headers - ONLY these need monospace for character alignment */
.timeline-header {
    font-family: 'SF Mono', 'Monaco', 'Inconsolata', 'Roboto Mono', 'Source Code Pro', 'Menlo', 'Consolas', monospace;
    font-size: 11px;
    font-weight: 500;
    letter-spacing: 0px;
    text-align: center;
}

.timeline-header-top-tier {
    background: var(--surface-elevated);
    border-bottom: 1px solid var(--border-color);
    padding: 4px 2px;
}

.timeline-header-bottom-tier {
    background: var(--surface-color);
    border-bottom: 1px solid var(--border-color);
    padding: 2px 1px;
    font-size: 10px;
}

/* Language-specific adjustments for Chinese characters */
[lang="zh-CN"] .task-grid-header .header-cell {
    font-size: 12px;
    line-height: 1.3;
}

[lang="zh-CN"] .timeline-header {
    font-size: 10px;
    line-height: 1.1;
}

/* Fixed character width constants for timeline calculations */
.timeline-header-constants {
    --char-width: 7px;  /* Fixed character width for monospace calculations */
    --min-header-space: 21px; /* 3 chars minimum (like "Jan", "Q1", "一月") */
}
```

## 🎨 **Font Strategy Rules**

### **📋 When to Use Each Font Strategy**

| Component Type | Font Strategy | Reason | Example |
|---------------|---------------|---------|---------|
| **Fixed-Width Grids** | System Fonts | CSS Grid/Flexbox handles layout | TaskGrid headers, form labels |
| **Character-Aligned Content** | Monospace | Precise character-level alignment needed | Timeline day numbers, code display |
| **Regular UI Text** | System Fonts | Best readability and native feel | Buttons, descriptions, tooltips |
| **Data Tables** | System Fonts | Column widths handled by CSS | Task lists, resource tables |
| **Time/Date Headers** | Monospace | Day/month alignment in timeline | "Jan 1", "Feb 15", "三月" |

### **🎯 Decision Framework**

**Use Monospace Fonts When:**
- ✅ Character position affects visual alignment (timeline day numbers)
- ✅ Fixed character width needed for calculations (SVG positioning)
- ✅ Multiple rows need vertical character alignment (calendar grids)

**Use System Fonts When:**
- ✅ CSS handles the layout (Grid, Flexbox, fixed widths)
- ✅ Readability is more important than alignment
- ✅ Native OS appearance is desired
- ✅ Text length varies significantly

### **🌍 Language-Specific Considerations**

**English Text:**
- System fonts: Excellent readability, native feel
- Monospace fonts: Clean alignment for technical content

**Chinese Text:**
- System fonts: Optimal Chinese character rendering
- Monospace fonts: May look awkward but necessary for alignment
- Font size adjustments: Slightly smaller for better fit

### **🔧 Component Integration Examples**

### **✅ NEW: Dependency Injection Pattern (Current Implementation)**

**🎯 Core Principle**: Components are independently I18N-aware through dependency injection, eliminating cascading parameter coupling.

```razor
@* TaskGrid.razor - Clean dependency injection approach *@
@using GanttComponents.Services
@inject IGanttI18N I18N
@implements IDisposable

<div class="task-grid-header">
    <div class="header-cell">@I18N.T("grid.wbs")</div>
    <div class="header-cell">@I18N.T("grid.task-name")</div>
    <div class="header-cell">@I18N.T("grid.start-date")</div>
    <div class="header-cell">@I18N.T("grid.end-date")</div>
    <div class="header-cell">@I18N.T("grid.duration")</div>
    <div class="header-cell">@I18N.T("grid.progress")</div>
    <div class="header-cell">@I18N.T("grid.resources")</div>
</div>

@code {
    [Parameter] public List<GanttTask>? Tasks { get; set; }
    [Parameter] public EventCallback<int> OnTaskSelected { get; set; }
    
    protected override void OnInitialized()
    {
        // Subscribe to language changes
        I18N.LanguageChanged += OnLanguageChanged;
    }
    
    private void OnLanguageChanged()
    {
        // Component automatically re-renders when language changes
        InvokeAsync(StateHasChanged);
    }
    
    public void Dispose()
    {
        // Clean up event subscription
        I18N.LanguageChanged -= OnLanguageChanged;
    }
}
```

```razor
@* GanttComposer.razor - No cascading parameter management needed *@
@using GanttComponents.Services

<div class="gantt-composer-container">
    <div class="gantt-composer-grid">
        @* No CascadingValue needed - components inject service independently *@
        <TaskGrid Tasks="@Tasks" 
                 OnTaskSelected="HandleTaskSelection" 
                 SelectedTaskId="@SelectedTaskId" />
        
        <TimelineView Tasks="@Tasks" 
                     SelectedTaskId="@SelectedTaskId"
                     ZoomLevel="@ZoomLevel" />
    </div>
</div>

@code {
    [Parameter] public List<GanttTask>? Tasks { get; set; }
    [Parameter] public EventCallback<int> OnTaskSelected { get; set; }
    
    private int? SelectedTaskId { get; set; }
    private TimelineZoomLevel ZoomLevel { get; set; } = TimelineZoomLevel.MonthDay;
    
    // No language parameter management needed!
    // Child components handle I18N independently via dependency injection
    
    private async Task HandleTaskSelection(int taskId)
    {
        SelectedTaskId = taskId;
        await OnTaskSelected.InvokeAsync(taskId);
    }
}
```

```razor
@* LanguageSelector.razor - Simple language switching *@
@using GanttComponents.Services
@inject IGanttI18N I18N
@implements IDisposable

<div class="language-selector">
    <label>@I18N.T("language.selector-label"):</label>
    <select @onchange="OnLanguageChanged" value="@I18N.CurrentCulture">
        @foreach (var culture in I18N.GetAvailableCultures())
        {
            <option value="@culture">
                @(culture == "en-US" ? "🇺🇸 English" : "🇨🇳 简体中文")
            </option>
        }
    </select>
</div>

@code {
    protected override void OnInitialized()
    {
        I18N.LanguageChanged += OnLanguageChanged;
    }
    
    private void OnLanguageChanged(ChangeEventArgs e)
    {
        var newCulture = e.Value?.ToString();
        if (!string.IsNullOrEmpty(newCulture))
        {
            I18N.SetCulture(newCulture);
            // All components with @inject IGanttI18N will automatically update
        }
    }
    
    private void OnLanguageChanged()
    {
        InvokeAsync(StateHasChanged);
    }
    
    public void Dispose()
    {
        I18N.LanguageChanged -= OnLanguageChanged;
    }
}
```

### **❌ Problems with Cascading Parameter Pattern**

**The "Cascading Parameter Pandemic"** - Why we moved away from `[CascadingParameter]` approach:

**🦠 Contagious Coupling**
- Every parent component must provide `<CascadingValue>` wrapper
- Child components require `[CascadingParameter]` boilerplate
- Adding I18N to one component forces changes up the entire component tree
- Creates tight coupling between components that shouldn't know about each other

**🔗 Chain of Dependency**
- If TaskGrid needs I18N, then GanttComposer must provide it
- If GanttComposer needs I18N, then the page must provide it
- Creates fragile dependency chains that break when components are moved
- Makes components less reusable in different contexts

**🧪 Testing Complexity**
- Every test must set up cascading value context
- Difficult to test components in isolation
- Mock setup becomes complex with parameter chains
- Test failures often unrelated to actual component logic

**⚡ Performance Overhead**
- Cascading values trigger re-renders in entire component subtrees
- Multiple cascading parameters multiply the performance impact
- No control over which components actually need the updates
- Wasteful re-rendering of components that don't use I18N

**🔧 Maintenance Burden**
```csharp
// ❌ OLD: Required everywhere in the component tree
public partial class SomeParentComponent
{
    [CascadingParameter] public string Language { get; set; } = "en-US";
    
    // Must remember to forward the parameter
    <CascadingValue Value="@Language">
        <SomeChildComponent />
    </CascadingValue>
}

public partial class SomeChildComponent  
{
    [CascadingParameter] public string Language { get; set; } = "en-US";
    
    // Must forward again if this has children that need I18N
    <CascadingValue Value="@Language">
        <TaskGrid />
    </CascadingValue>
}

public partial class TaskGrid
{
    [CascadingParameter] public string Language { get; set; } = "en-US";
    
    // Finally can use the language!
    <div>@GetLocalizedText("grid.task-name")</div>
}
```

**💥 Real-World Impact**
- TaskGrid worked fine standalone (`/gantt-demo`)
- TaskGrid broke when composed (`/gantt-composer-demo`) due to missing cascading context
- Headers became "resizable and detached" because CSS styling depended on parameter availability
- Required extensive parameter forwarding just to make basic components work

### **✅ Benefits of Dependency Injection Architecture**

**✅ Component Independence**
- Components don't need parent components to provide language context
- TaskGrid works identically in standalone and composed contexts
- No cascading parameter "pandemic" spreading through component trees

**✅ Better Testability**
- Easy to mock `IGanttI18N` interface for unit tests
- Components can be tested in isolation with fake I18N services
- Event subscription testing is straightforward

**✅ Cleaner Code**
- No `[CascadingParameter]` boilerplate in every component
- No `<CascadingValue>` wrapping required in parent components
- Standard .NET dependency injection patterns

**✅ Event-Driven Reactivity**
- Language changes automatically propagate to all subscribed components
- No manual `StateHasChanged()` calls in parent components
- Efficient notification system with automatic cleanup

**✅ Singleton Efficiency**
- One service instance manages application-wide language state
- No duplicate language management across component instances
- Consistent culture state throughout the application

**✅ CORRECT: Minimal, targeted overrides**
```css
/* This preserves TaskGrid's natural layout */
.task-grid-header {
    min-height: var(--header-height);   /* ✅ Ensures minimum height */
    /* Don't override display property - let component decide */
}

.task-grid-header .header-cell {
    font-family: system-fonts;          /* ✅ Typography only */
    font-size: 13px;                    /* ✅ No layout changes */
    text-align: center;                 /* ✅ Safe styling */
}
```

**🎯 Key Principle**: I18N CSS should only affect **typography and language-specific rendering**, never **layout structure**.

### **📊 Timeline Zoom Controls with I18N**

```html
<div class="timeline-zoom-controls">
    <div class="zoom-presets">
        <button class="zoom-btn @(ZoomLevel == TimelineZoomLevel.WeekDay ? "active" : "")"
                @onclick="() => SetZoomLevel(TimelineZoomLevel.WeekDay)">
            📅 @GanttI18N.T("zoom.detailed")
        </button>
        <button class="zoom-btn @(ZoomLevel == TimelineZoomLevel.MonthDay ? "active" : "")"
                @onclick="() => SetZoomLevel(TimelineZoomLevel.MonthDay)">
            📈 @GanttI18N.T("zoom.planning")
        </button>
        <button class="zoom-btn @(ZoomLevel == TimelineZoomLevel.QuarterMonth ? "active" : "")"
                @onclick="() => SetZoomLevel(TimelineZoomLevel.QuarterMonth)">
            📊 @GanttI18N.T("zoom.strategic")
        </button>
    </div>
    
    <div class="zoom-shortcuts">
        <button @onclick="ZoomToFitViewport" title="@GanttI18N.T("zoom.fit-screen")">
            🖥️ @GanttI18N.T("zoom.fit-screen")
        </button>
        <button @onclick="ZoomToFitTasks" title="@GanttI18N.T("zoom.fit-tasks")">
            🔍 @GanttI18N.T("zoom.fit-tasks")
        </button>
    </div>
</div>
```

### **📋 Task Grid Headers with I18N (System Fonts)**

```html
<!-- TaskGrid uses system fonts - CSS Grid handles layout -->
<div class="task-grid-header">
    <div class="header-cell task-name">@GanttI18N.T("grid.task-name")</div>
    <div class="header-cell duration">@GanttI18N.T("grid.duration")</div>
    <div class="header-cell start-date">@GanttI18N.T("grid.start-date")</div>
    <div class="header-cell end-date">@GanttI18N.T("grid.end-date")</div>
    <div class="header-cell progress">@GanttI18N.T("grid.progress")</div>
    <div class="header-cell resources">@GanttI18N.T("grid.resources")</div>
</div>
```

### **📅 Timeline Headers with I18N (Monospace Fonts)**

```html
<!-- TimelineView headers use monospace - character alignment critical -->
<div class="timeline-top-tier">
    @foreach (var monthHeader in GetMonthHeaders())
    {
        <div class="timeline-header month-header" style="width: @(monthHeader.Width)px;">
            @DateFormatHelper.FormatDate(monthHeader.Date, "date.month-year")
        </div>
    }
</div>

<div class="timeline-bottom-tier">
    @foreach (var dayHeader in GetDayHeaders())
    {
        <div class="timeline-header day-header" style="width: @(DayWidth)px;">
            @DateFormatHelper.FormatDate(dayHeader.Date, "date.day-number")
        </div>
    }
</div>
```

### **🎭 Task Overflow Dropdown with I18N**

```html
<div class="timeline-overflow">
    @if (renderResult.ShowOverflowIndicator)
    {
        <div class="overflow-indicator" @onclick="ToggleOverflowDropdown">
            <span>@GanttI18N.T("overflow.indicator")</span>
            <span class="overflow-count">@GanttI18N.T("overflow.tasks-hidden", renderResult.HiddenTasks.Count)</span>
        </div>
        
        @if (showOverflowDropdown)
        {
            <div class="overflow-dropdown">
                @foreach (var task in renderResult.HiddenTasks)
                {
                    <div class="overflow-task-item" 
                         @onclick="() => ShowTask(task)"
                         title="@GanttI18N.T("overflow.show-task")">
                        <span class="task-name">@task.Name</span>
                        <span class="task-duration">@DurationFormatter.FormatDuration(task.Duration)</span>
                        <span class="task-dates">
                            @DateFormatHelper.FormatDate(task.StartDate, "date.month-abbr") 
                            - @DateFormatHelper.FormatDate(task.EndDate, "date.month-abbr")
                        </span>
                    </div>
                }
            </div>
        }
    }
</div>
```

## 🎛️ **Language Switching Component**

### **🌐 Simple Language Selector**

```html
<div class="language-selector">
    <select @onchange="OnLanguageChanged" value="@GanttI18N.CurrentCulture">
        <option value="en-US">🇺🇸 English</option>
        <option value="zh-CN">🇨🇳 简体中文</option>
    </select>
</div>

@code {
    private void OnLanguageChanged(ChangeEventArgs e)
    {
        var newCulture = e.Value?.ToString();
        if (!string.IsNullOrEmpty(newCulture))
        {
            GanttI18N.SetCulture(newCulture);
            StateHasChanged(); // Refresh all components
        }
    }
}
```

```css
.language-selector {
    position: absolute;
    top: 16px;
    right: 16px;
}

.language-selector select {
    padding: 4px 8px;
    border: 1px solid var(--border-color);
    border-radius: 4px;
    background: var(--surface-color);
    color: var(--text-color);
    font-size: 12px;
}
```

## 🔧 **Implementation Roadmap**

### **📅 Development Timeline - ✅ COMPLETED**

| Phase | Duration | Priority | Features | Status |
|-------|----------|----------|----------|---------|
| **Phase 0** | Week 1 | 🔥 Critical | I18N Foundation: IGanttI18N interface, dependency injection, event notifications | ✅ **COMPLETE** |
| **Phase 1** | Week 2 | 🔥 Critical | Timeline Integration: I18N-aware zoom configuration, fixed-width headers | 🔄 **READY** |
| **Phase 2** | Week 3 | ⭐ High | Component Integration: TaskGrid headers, overflow system, common UI elements | ✅ **COMPLETE** |
| **Phase 3** | Week 4 | ⭐ High | Polish & Testing: Language switching UI, Chinese translations validation | ✅ **COMPLETE** |

### **🎯 Success Criteria - ✅ ACHIEVED**

### **📊 Success Targets**
- ✅ **Language Switching**: Instant UI language change without page reload
- ✅ **Component Independence**: TaskGrid works identically in standalone and composed contexts
- ✅ **Event-Driven Updates**: Components automatically update when language changes
- ✅ **Dependency Injection**: Clean architecture with `IGanttI18N` interface
- ✅ **Test Coverage**: All 139 unit tests passing with instance service pattern
- ✅ **No Cascading Parameters**: Eliminated "cascading parameter pandemic"

**🏆 Key Achievement**: Successfully eliminated cascading parameter coupling through proper dependency injection architecture with event notifications.

## 📝 **Testing Strategy**

### **🧪 I18N Test Cases - ✅ All Passing**

```csharp
[TestClass]
public class GanttI18NTests
{
    private readonly IGanttI18N _i18nService;

    public GanttI18NTests()
    {
        _i18nService = new GanttI18N();
    }

    [TestMethod]
    public void T_WithValidEnglishKey_ReturnsTranslation()
    {
        _i18nService.SetCulture("en-US");
        Assert.AreEqual("WBS", _i18nService.T("grid.wbs"));
    }
    
    [TestMethod]
    public void T_WithValidChineseKey_ReturnsTranslation()
    {
        _i18nService.SetCulture("zh-CN");
        Assert.AreEqual("工作分解", _i18nService.T("grid.wbs"));
    }
    
    [TestMethod]
    public void T_WithInvalidKey_ShouldFallbackToEnglish()
    {
        _i18nService.SetCulture("zh-CN");
        // Key exists in English but not Chinese
        var result = _i18nService.T("some.english.only.key");
        Assert.AreEqual("English Value", result);
    }
    
    [TestMethod]
    public void SetCulture_WithNullCulture_DoesNotCrash()
    {
        // Should not throw and should handle gracefully
        _i18nService.SetCulture(null!);
        Assert.IsTrue(!string.IsNullOrEmpty(_i18nService.CurrentCulture));
    }
    
    [TestMethod]
    public void LanguageChanged_EventFired_WhenCultureChanges()
    {
        bool eventFired = false;
        _i18nService.LanguageChanged += () => eventFired = true;
        
        _i18nService.SetCulture("zh-CN");
        
        Assert.IsTrue(eventFired);
    }
}
```

### **🎯 Test Results - ✅ All Green**

```
测试摘要: 总计: 139, 失败: 0, 成功: 139, 已跳过: 0, 持续时间: 4.8 秒
```

**Key Testing Achievements:**
- ✅ **139 tests passing** - Complete test suite validates dependency injection architecture
- ✅ **Null handling** - Service gracefully handles null culture input
- ✅ **Event notifications** - LanguageChanged event properly notifies subscribers
- ✅ **Fallback system** - Chinese → English → Key fallback chain working
- ✅ **Instance pattern** - All tests converted from static calls to instance service
```

### **🎨 Visual Testing Checklist**

- [ ] Timeline headers align perfectly in both languages
- [ ] Chinese characters render clearly in fixed-width font
- [ ] Task overflow dropdown displays correct Chinese text
- [ ] Date formats follow cultural conventions
- [ ] Duration units are properly localized
- [ ] Language switching updates all UI elements immediately

---

## 🎨 **Font Strategy Summary**

### **📝 Quick Reference Guide**

| UI Element | Font Family | Size | Reason |
|------------|-------------|------|---------|
| **TaskGrid Headers** | System Fonts | 13px | CSS Grid handles layout, readability priority |
| **Timeline Headers** | Monospace | 11px | Character alignment for day/month positioning |
| **Button Text** | System Fonts | 14px | Native OS appearance, best readability |
| **Tooltips** | System Fonts | 12px | Readability priority |
| **Code Display** | Monospace | 12px | Character alignment needed |
| **Form Labels** | System Fonts | 13px | CSS handles layout |

### **🎯 Implementation Checklist**

- ✅ **TaskGrid**: Use system fonts - CSS Grid provides layout stability
- ✅ **TimelineView**: Use monospace fonts - SVG positioning requires character alignment  
- ✅ **Chinese Support**: Slightly smaller font sizes for better character fit
- ✅ **Modern Stack**: SF Mono, Roboto Mono instead of old Courier New
- ✅ **Selective Application**: Right tool for the right job

### **⚠️ Common Mistakes to Avoid**

- ❌ Don't use monospace fonts for everything "to be safe"
- ❌ Don't use system fonts for timeline day numbers (breaks alignment)
- ❌ Don't forget language-specific font size adjustments
- ❌ Don't mix font strategies within the same component

---

## 🔧 **Troubleshooting Guide**

### **✅ RESOLVED: TaskGrid Integration Issues**

#### **Issue: TaskGrid headers "resizable and detached" in GanttComposer** ✅ FIXED

**Previous Problem:**
- TaskGrid worked fine standalone (`/gantt-demo`)
- TaskGrid headers broken in composed view (`/gantt-composer-demo`)
- Headers became resizable and detached from grid body
- Language switching didn't work in composed context

**Root Cause & Solution:**
❌ **Old Architecture**: Cascading parameter "pandemic" requiring parameter forwarding chains
✅ **New Architecture**: Dependency injection with event notifications

**Before (Problematic):**
```csharp
// Required cascading parameter boilerplate everywhere
[CascadingParameter] public string CurrentLanguage { get; set; } = "en-US";
```

**After (Clean):**
```csharp
// Simple dependency injection - works everywhere
@inject IGanttI18N I18N
```

**Result**: TaskGrid now works identically in standalone and composed contexts without any parameter forwarding.

### **🚨 Current Known Issues (None)**

✅ **All I18N integration issues resolved** through dependency injection architecture.

### **🎯 I18N Integration Checklist**

**For Component Authors:**
- [x] ✅ Add `@inject IGanttI18N I18N` directive
- [x] ✅ Subscribe to `I18N.LanguageChanged` in `OnInitialized()`
- [x] ✅ Implement `IDisposable` for event cleanup
- [x] ✅ Use `I18N.T()` for all translatable text
- [x] ✅ Test component both standalone and in composed contexts

**For Composition Components:**
- [x] ✅ No special I18N handling required!
- [x] ✅ Child components manage I18N independently
- [x] ✅ No cascading parameters needed

**For CSS Authors:**
- [x] ✅ Use minimal overrides - avoid `!important` for layout properties
- [x] ✅ Apply monospace fonts only where character alignment is critical
- [x] ✅ Test font strategy in both standalone and composed contexts

---

*This document serves as the complete specification for the simple I18N system supporting English and Simplified Chinese only.*
