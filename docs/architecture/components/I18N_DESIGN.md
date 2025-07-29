# 🌍 Internationalization (I18N) System Design & Implementation

> **Component**: All Components  
> **Feature**: Simple Multi-Language Support  
> **Status**: 📋 Design Phase  
> **Date**: July 29, 2025

## 📋 **Executive Summary**

The I18N System provides simple, lightweight internationalization capabilities for the Gantt Components, supporting English (US) and Simplified Chinese with a focus on UI labels, date formats, and duration units. The system prioritizes simplicity and maintainability over comprehensive localization features.

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
- ✅ **Static Translation System**: Compile-time dictionaries for performance
- ✅ **Culture-Aware Date Formatting**: Leverage .NET's CultureInfo
- ✅ **Component Independence**: Each component handles its own translations
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

### **🗂️ Simple Translation System**

```csharp
public static class GanttI18N
{
    public static string CurrentCulture { get; set; } = "en-US";
    
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
    /// Switch application language
    /// </summary>
    /// <param name="culture">Culture code (en-US or zh-CN)</param>
    public static void SetCulture(string culture)
    {
        if (Translations.ContainsKey(culture))
        {
            CurrentCulture = culture;
            // Update .NET culture for date formatting
            var cultureInfo = new CultureInfo(culture);
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
        }
    }
    
    /// <summary>
    /// Get available cultures
    /// </summary>
    public static IEnumerable<string> GetAvailableCultures() => Translations.Keys;
}
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
```

### **🔧 Component Integration Examples**

### **🏗️ Critical: Component Composition with I18N**

**⚠️ IMPORTANT: Cascading Parameter Propagation**

When composing components that use I18N (like TaskGrid inside GanttComposer), you **must** forward the cascading language parameter:

```csharp
// GanttComposer.razor - REQUIRED for I18N support
@code {
    [Parameter] public List<GanttTask>? Tasks { get; set; }
    [Parameter] public EventCallback<int> OnTaskSelected { get; set; }
    
    // ✅ CRITICAL: Receive cascading language parameter
    [CascadingParameter] public string CurrentLanguage { get; set; } = "en-US";
    
    // Other parameters...
}
```

```html
<!-- ✅ CRITICAL: Wrap child components with CascadingValue -->
<div class="composer-grid">
    <CascadingValue Value="CurrentLanguage">
        <TaskGrid Tasks="@Tasks" 
                 OnTaskSelected="HandleTaskSelection" 
                 SelectedTaskId="@SelectedTaskId" />
    </CascadingValue>
</div>
```

**🚨 Common Issue**: Without proper cascading, TaskGrid headers become "resizable and detached from grid body" in composed contexts because language changes aren't propagated.

### **🎨 CSS Override Strategy - Lessons Learned**

**❌ WRONG: Aggressive !important overrides**
```css
/* This breaks TaskGrid layout in GanttComposer context */
.task-grid-header {
    display: flex !important;           /* ❌ Overrides natural layout */
    height: var(--header-height) !important; /* ❌ Too rigid */
    align-items: center !important;     /* ❌ Forces flexbox */
}
```

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

### **📅 Development Timeline**

| Phase | Duration | Priority | Features |
|-------|----------|----------|----------|
| **Phase 0** | Week 1 | 🔥 Critical | I18N Foundation: GanttI18N class, basic translation system, culture switching |
| **Phase 1** | Week 2 | 🔥 Critical | Timeline Integration: I18N-aware zoom configuration, fixed-width headers |
| **Phase 2** | Week 3 | ⭐ High | Component Integration: TaskGrid headers, overflow system, common UI elements |
| **Phase 3** | Week 4 | ⭐ High | Polish & Testing: Language switching UI, Chinese translations validation |

### **🎯 Success Criteria**

### **📊 Success Targets**
- **Language Switching**: Instant UI language change without page reload
- **Header Rendering**: Perfect fixed-width font display in both languages
- **Date Formatting**: Culture-appropriate date/time display patterns
- **Duration Units**: Proper localized unit display and parsing
- **Professional Feel**: Native-looking Chinese and English interfaces

## 📝 **Testing Strategy**

### **🧪 I18N Test Cases**

```csharp
[TestClass]
public class I18NTests
{
    [TestMethod]
    public void T_ShouldReturnEnglishTranslation()
    {
        GanttI18N.SetCulture("en-US");
        Assert.AreEqual("Detailed", GanttI18N.T("zoom.detailed"));
    }
    
    [TestMethod]
    public void T_ShouldReturnChineseTranslation()
    {
        GanttI18N.SetCulture("zh-CN");
        Assert.AreEqual("详细", GanttI18N.T("zoom.detailed"));
    }
    
    [TestMethod]
    public void T_ShouldFallbackToEnglish()
    {
        GanttI18N.SetCulture("zh-CN");
        Assert.AreEqual("Detailed", GanttI18N.T("nonexistent.key"));
    }
    
    [TestMethod]
    public void FormatDuration_ShouldUseLocalizedUnits()
    {
        GanttI18N.SetCulture("zh-CN");
        var result = DurationFormatter.FormatDuration(TimeSpan.FromDays(5));
        Assert.AreEqual("5天", result);
    }
    
    [TestMethod]
    public void DateFormat_ShouldRenderFixedWidth()
    {
        GanttI18N.SetCulture("zh-CN");
        var width = DateFormatHelper.GetLocalizedHeaderWidth("date.month-abbr", DateTime.Now);
        Assert.IsTrue(width > 0 && width % 7 == 0); // Must be multiple of char width
    }
}
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

### **🚨 Common I18N Integration Issues**

#### **Issue: TaskGrid headers "resizable and detached" in GanttComposer**

**Symptoms:**
- TaskGrid works fine standalone (`/gantt-demo`)
- TaskGrid headers broken in composed view (`/gantt-composer-demo`)
- Headers become resizable and detach from grid body
- Language switching doesn't work in composed context

**Root Cause:**
1. **Missing Cascading Parameter**: Child TaskGrid not receiving language updates
2. **CSS Override Conflicts**: I18N CSS overriding component's natural layout

**Solution:**
```csharp
// 1. Add cascading parameter to parent component
[CascadingParameter] public string CurrentLanguage { get; set; } = "en-US";
```

```html
<!-- 2. Wrap child components with CascadingValue -->
<CascadingValue Value="CurrentLanguage">
    <TaskGrid Tasks="@Tasks" ... />
</CascadingValue>
```

```css
/* 3. Use minimal CSS overrides - avoid !important for layout */
.task-grid-header {
    min-height: var(--header-height);  /* ✅ Safe constraint */
    /* Don't override display, flex properties */
}
```

#### **Issue: Chinese characters break timeline alignment**

**Symptoms:**
- Timeline day numbers misaligned with Chinese text
- Header widths inconsistent between languages

**Solution:**
- Use monospace fonts ONLY for timeline headers
- Apply system fonts for TaskGrid headers (CSS Grid handles layout)

#### **Issue: Language switching doesn't update all components**

**Symptoms:**
- Some components update, others don't
- Partial translation updates

**Solution:**
- Ensure all parent components have `[CascadingParameter] CurrentLanguage`
- Wrap component trees with `<CascadingValue Value="CurrentLanguage">`
- Call `StateHasChanged()` after `GanttI18N.SetCulture()`

### **🎯 I18N Integration Checklist**

**For Component Authors:**
- [ ] Add `[CascadingParameter] public string CurrentLanguage { get; set; } = "en-US";`
- [ ] Use `GanttI18N.T()` for all translatable text
- [ ] Test component both standalone and in composed contexts
- [ ] Verify language switching triggers re-renders

**For Composition Components (like GanttComposer):**
- [ ] Receive cascading language parameter from parent
- [ ] Forward parameter to child components via `<CascadingValue>`
- [ ] Test language switching in composed context specifically

**For CSS Authors:**
- [ ] Use minimal overrides - avoid `!important` for layout properties
- [ ] Apply monospace fonts only where character alignment is critical
- [ ] Test font strategy in both standalone and composed contexts
- [ ] Verify no layout breakage when switching languages

---

*This document serves as the complete specification for the simple I18N system supporting English and Simplified Chinese only.*
