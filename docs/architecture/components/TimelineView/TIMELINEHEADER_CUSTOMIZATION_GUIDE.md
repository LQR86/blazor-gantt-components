# 🎨 TimelineHeader Component - Customization Guide

## 🎯 **Overview**

The TimelineHeader extraction in v0.8.7 transformed header customization from difficult inline modifications to a flexible, service-driven architecture. This guide covers all customization approaches from simple formatting changes to complete header redesigns.

## 🚀 **Quick Start: Common Customizations**

### **📅 Custom Date Formats**
```csharp
public class CustomDateHeaderService : TimelineHeaderService
{
    protected override string GetFormattedLabel(DateTime date, string formatKey, TimelineZoomLevel zoomLevel)
    {
        // Override specific zoom level formats
        return zoomLevel switch
        {
            TimelineZoomLevel.MonthDay48px => date.ToString("MMM dd, yyyy"), // "Jan 15, 2025"
            TimelineZoomLevel.WeekDay97px => date.ToString("ddd MMM dd"),     // "Mon Jan 15"
            TimelineZoomLevel.YearQuarter3px => date.ToString("yyyy"),       // "2025"
            _ => base.GetFormattedLabel(date, formatKey, zoomLevel)
        };
    }
}

// Register in Program.cs
services.AddScoped<ITimelineHeaderService, CustomDateHeaderService>();
```

### **🎨 Custom Header Styling**
```razor
<!-- Custom CSS classes for specific header periods -->
<TimelineHeader StartDate="@StartDate" 
               EndDate="@EndDate" 
               EffectiveDayWidth="@EffectiveDayWidth" 
               ZoomLevel="@ZoomLevel"
               CssClass="project-timeline-header" />
```

```css
/* Custom header styling */
.project-timeline-header .header-primary-period.current-quarter {
    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
    color: white;
    font-weight: 600;
}

.project-timeline-header .header-secondary-period.weekend {
    background: #f8f9fa;
    color: #6c757d;
}

.project-timeline-header .header-secondary-period.milestone {
    border-left: 3px solid #28a745;
    background: rgba(40, 167, 69, 0.1);
}
```

## 🔧 **Service-Based Customization**

### **� I18N Integration (New in v0.8.8)**

The TimelineHeaderService now includes comprehensive internationalization support through IGanttI18N service integration:

```csharp
public class LocalizedHeaderService : TimelineHeaderService
{
    public LocalizedHeaderService(
        DateFormatHelper dateFormatter, 
        IGanttI18N i18n,
        IUniversalLogger logger) 
        : base(dateFormatter, i18n, logger)
    {
        // I18N service automatically available for localization
    }
    
    protected override string GetFormattedLabel(DateTime date, string formatKey, TimelineZoomLevel zoomLevel)
    {
        // Use I18N-enhanced formatting with automatic fallback
        var enhancedFormat = GetI18NEnhancedFormat(formatKey, zoomLevel);
        return date.ToString(enhancedFormat);
    }
}
```

#### **📚 Available I18N Keys**
The system includes comprehensive timeline localization keys:

```
timeline.header.year-format      // "yyyy" / "yyyy年"
timeline.header.quarter-format   // "'Q'q yyyy" / "yyyy年第q季度"  
timeline.header.month-format     // "MMM yyyy" / "yyyy年M月"
timeline.header.week-format      // "'Week' w" / "第w周"
timeline.header.day-format       // "dd MMM" / "M月d日"
timeline.component.*             // Component-level enhancements
timeline.floating.*              // Future floating header features
```

### **�🏗️ Custom Header Service Implementation**

#### **1. Basic Service Extension**
```csharp
public class ProjectHeaderService : TimelineHeaderService
{
    private readonly IProjectService _projectService;
    
    public ProjectHeaderService(
        DateFormatHelper dateFormatter, 
        IGanttI18N i18n,
        IUniversalLogger logger,
        IProjectService projectService) 
        : base(dateFormatter, i18n, logger)
    {
        _projectService = projectService;
    }
    
    public override TimelineHeaderResult GenerateHeaderPeriods(
        DateTime startDate, 
        DateTime endDate, 
        TimelineZoomLevel zoomLevel, 
        double effectiveDayWidth)
    {
        var result = base.GenerateHeaderPeriods(startDate, endDate, zoomLevel, effectiveDayWidth);
        
        // Add project-specific enhancements
        EnhanceWithProjectMilestones(result);
        EnhanceWithSprintBoundaries(result);
        
        return result;
    }
    
    private void EnhanceWithProjectMilestones(TimelineHeaderResult result)
    {
        var milestones = _projectService.GetMilestones();
        
        foreach (var period in result.SecondaryPeriods)
        {
            var milestone = milestones.FirstOrDefault(m => 
                m.Date >= period.Start && m.Date <= period.End);
                
            if (milestone != null)
            {
                period.CssClass += " milestone";
                period.Tooltip = $"Milestone: {milestone.Name}";
                period.CustomData["milestoneId"] = milestone.Id;
            }
        }
    }
}
```

#### **2. Industry-Specific Header Services**

**Agile Development Headers**:
```csharp
public class AgileHeaderService : TimelineHeaderService
{
    protected override List<HeaderPeriod> GenerateSecondaryPeriods(
        DateTime startDate, DateTime endDate, 
        TimelineHeaderConfiguration config, double effectiveDayWidth)
    {
        if (config.SecondaryUnit == TimelineHeaderUnit.Day)
        {
            return GenerateSprintDayPeriods(startDate, endDate, effectiveDayWidth);
        }
        
        return base.GenerateSecondaryPeriods(startDate, endDate, config, effectiveDayWidth);
    }
    
    private List<HeaderPeriod> GenerateSprintDayPeriods(DateTime start, DateTime end, double dayWidth)
    {
        var periods = new List<HeaderPeriod>();
        var current = start;
        
        while (current <= end)
        {
            var period = new HeaderPeriod
            {
                Start = current,
                End = current.AddDays(1),
                Width = dayWidth,
                Label = GetSprintDayLabel(current),
                CssClass = GetSprintDayCssClass(current),
                Tooltip = GetSprintDayTooltip(current)
            };
            
            periods.Add(period);
            current = current.AddDays(1);
        }
        
        return periods;
    }
    
    private string GetSprintDayLabel(DateTime date)
    {
        var dayOfWeek = date.DayOfWeek;
        if (dayOfWeek == DayOfWeek.Saturday || dayOfWeek == DayOfWeek.Sunday)
            return ""; // No label for weekends
            
        return date.Day.ToString();
    }
    
    private string GetSprintDayCssClass(DateTime date)
    {
        var classes = new List<string> { "sprint-day" };
        
        if (date.DayOfWeek == DayOfWeek.Monday) classes.Add("sprint-start");
        if (date.DayOfWeek == DayOfWeek.Friday) classes.Add("sprint-end");
        if (IsSprintReviewDay(date)) classes.Add("review-day");
        if (IsWeekend(date)) classes.Add("weekend");
        
        return string.Join(" ", classes);
    }
}
```

**Manufacturing/Production Headers**:
```csharp
public class ProductionHeaderService : TimelineHeaderService
{
    protected override List<HeaderPeriod> GeneratePrimaryPeriods(
        DateTime startDate, DateTime endDate, 
        TimelineHeaderConfiguration config)
    {
        if (config.PrimaryUnit == TimelineHeaderUnit.Month)
        {
            return GenerateProductionMonthPeriods(startDate, endDate);
        }
        
        return base.GeneratePrimaryPeriods(startDate, endDate, config);
    }
    
    private List<HeaderPeriod> GenerateProductionMonthPeriods(DateTime start, DateTime end)
    {
        var periods = new List<HeaderPeriod>();
        var current = new DateTime(start.Year, start.Month, 1);
        
        while (current <= end)
        {
            var monthEnd = current.AddMonths(1).AddDays(-1);
            var workingDays = GetWorkingDaysInMonth(current);
            var productionTarget = GetMonthlyProductionTarget(current);
            
            var period = new HeaderPeriod
            {
                Start = current,
                End = monthEnd,
                Width = CalculateMonthWidth(current),
                Label = $"{current:MMM} ({workingDays}d)",
                Tooltip = $"Production Target: {productionTarget:N0} units",
                CssClass = GetProductionMonthCssClass(current),
                CustomData = new Dictionary<string, object>
                {
                    ["workingDays"] = workingDays,
                    ["productionTarget"] = productionTarget,
                    ["isHighDemandMonth"] = IsHighDemandMonth(current)
                }
            };
            
            periods.Add(period);
            current = current.AddMonths(1);
        }
        
        return periods;
    }
}
```

### **🎛️ Dynamic Service Selection**

#### **Runtime Service Switching**
```csharp
public class AdaptiveHeaderService : ITimelineHeaderService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IProjectContext _projectContext;
    
    public AdaptiveHeaderService(IServiceProvider serviceProvider, IProjectContext projectContext)
    {
        _serviceProvider = serviceProvider;
        _projectContext = projectContext;
    }
    
    public TimelineHeaderResult GenerateHeaderPeriods(
        DateTime startDate, DateTime endDate, 
        TimelineZoomLevel zoomLevel, double effectiveDayWidth)
    {
        var headerService = GetHeaderServiceForCurrentContext();
        return headerService.GenerateHeaderPeriods(startDate, endDate, zoomLevel, effectiveDayWidth);
    }
    
    private ITimelineHeaderService GetHeaderServiceForCurrentContext()
    {
        var project = _projectContext.CurrentProject;
        
        return project.Type switch
        {
            ProjectType.AgileSDLC => _serviceProvider.GetRequiredService<AgileHeaderService>(),
            ProjectType.Manufacturing => _serviceProvider.GetRequiredService<ProductionHeaderService>(),
            ProjectType.Research => _serviceProvider.GetRequiredService<ResearchHeaderService>(),
            ProjectType.Marketing => _serviceProvider.GetRequiredService<CampaignHeaderService>(),
            _ => _serviceProvider.GetRequiredService<TimelineHeaderService>()
        };
    }
}

// Registration in Program.cs
services.AddScoped<TimelineHeaderService>();
services.AddScoped<AgileHeaderService>();
services.AddScoped<ProductionHeaderService>();
services.AddScoped<ResearchHeaderService>();
services.AddScoped<CampaignHeaderService>();
services.AddScoped<ITimelineHeaderService, AdaptiveHeaderService>();
```

#### **Configuration-Based Service Selection**
```csharp
public class ConfigurableHeaderService : ITimelineHeaderService
{
    private readonly Dictionary<string, ITimelineHeaderService> _headerServices;
    private readonly IConfiguration _configuration;
    
    public ConfigurableHeaderService(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        _configuration = configuration;
        _headerServices = new Dictionary<string, ITimelineHeaderService>
        {
            ["default"] = serviceProvider.GetRequiredService<TimelineHeaderService>(),
            ["agile"] = serviceProvider.GetRequiredService<AgileHeaderService>(),
            ["production"] = serviceProvider.GetRequiredService<ProductionHeaderService>(),
            ["research"] = serviceProvider.GetRequiredService<ResearchHeaderService>()
        };
    }
    
    public TimelineHeaderResult GenerateHeaderPeriods(...)
    {
        var headerType = _configuration["Timeline:HeaderType"] ?? "default";
        var service = _headerServices.GetValueOrDefault(headerType) ?? _headerServices["default"];
        
        return service.GenerateHeaderPeriods(startDate, endDate, zoomLevel, effectiveDayWidth);
    }
}
```

## 🎨 **Component-Level Customization**

### **📋 Custom Header Component**

#### **1. Enhanced Header Component**
```razor
@* CustomTimelineHeader.razor *@
@inherits TimelineHeaderBase
@inject IProjectService ProjectService

<div class="custom-timeline-header @CssClass">
    @if (!HeaderResult.IsCollapsed)
    {
        <!-- Primary header with project context -->
        <div class="header-primary-row" style="height: @(HeaderMonthHeight)px">
            @foreach (var period in HeaderResult.PrimaryPeriods)
            {
                <div class="header-primary-period @GetPrimaryPeriodCssClass(period)" 
                     style="width: @(period.Width)px"
                     title="@GetPrimaryPeriodTooltip(period)">
                    
                    @if (HasProjectMilestone(period))
                    {
                        <i class="milestone-icon fas fa-flag"></i>
                    }
                    
                    <span class="period-label">@period.Label</span>
                    
                    @if (ShowProjectMetrics && HasMetrics(period))
                    {
                        <div class="period-metrics">
                            <span class="metric">@GetPeriodMetric(period)</span>
                        </div>
                    }
                </div>
            }
        </div>
    }
    
    <!-- Secondary header with enhanced day information -->
    <div class="header-secondary-row" style="height: @(HeaderDayHeight)px">
        @foreach (var period in HeaderResult.SecondaryPeriods)
        {
            <div class="header-secondary-period @GetSecondaryPeriodCssClass(period)" 
                 style="width: @(period.Width)px"
                 title="@GetSecondaryPeriodTooltip(period)"
                 @onclick="() => OnPeriodClick(period)">
                
                @if (IsToday(period.Start))
                {
                    <div class="today-indicator"></div>
                }
                
                <span class="period-label">@period.Label</span>
                
                @if (HasTasks(period))
                {
                    <div class="task-count-badge">@GetTaskCount(period)</div>
                }
            </div>
        }
    </div>
</div>

@code {
    [Parameter] public bool ShowProjectMetrics { get; set; } = true;
    [Parameter] public bool ShowTaskCounts { get; set; } = true;
    [Parameter] public EventCallback<HeaderPeriod> OnPeriodClick { get; set; }
    
    private string GetPrimaryPeriodCssClass(HeaderPeriod period)
    {
        var classes = new List<string> { "primary-period" };
        
        if (IsCurrentPeriod(period)) classes.Add("current");
        if (HasProjectMilestone(period)) classes.Add("has-milestone");
        if (IsOverbudget(period)) classes.Add("overbudget");
        if (IsAheadOfSchedule(period)) classes.Add("ahead-schedule");
        
        return string.Join(" ", classes);
    }
    
    private bool HasProjectMilestone(HeaderPeriod period)
    {
        return ProjectService.HasMilestoneInPeriod(period.Start, period.End);
    }
    
    private string GetPeriodMetric(HeaderPeriod period)
    {
        var metric = ProjectService.GetPeriodMetric(period.Start, period.End);
        return metric.DisplayValue;
    }
}
```

#### **2. Themed Header Variants**
```razor
@* Create themed header components *@

@* DarkThemeTimelineHeader.razor *@
<TimelineHeader @attributes="AllOtherAttributes" CssClass="dark-theme @CssClass" />

@* MinimalTimelineHeader.razor *@
<TimelineHeader @attributes="AllOtherAttributes" CssClass="minimal-design @CssClass" />

@* ProjectDashboardHeader.razor *@
<CustomTimelineHeader @attributes="AllOtherAttributes" 
                     ShowProjectMetrics="true"
                     ShowTaskCounts="true"
                     CssClass="dashboard-header @CssClass" />
```

### **🎯 Usage Examples**

#### **1. Replace Default Header**
```razor
@* Use custom header in TimelineView *@
<TimelineView Tasks="@tasks" ZoomLevel="@zoomLevel">
    <HeaderTemplate>
        <CustomTimelineHeader StartDate="@context.StartDate"
                            EndDate="@context.EndDate"
                            EffectiveDayWidth="@context.EffectiveDayWidth"
                            ZoomLevel="@context.ZoomLevel"
                            OnPeriodClick="HandlePeriodClick" />
    </HeaderTemplate>
</TimelineView>
```

#### **2. Conditional Header Selection**
```razor
@if (IsAgileProject)
{
    <AgileTimelineHeader @attributes="headerAttributes" />
}
else if (IsManufacturingProject)
{
    <ProductionTimelineHeader @attributes="headerAttributes" />
}
else
{
    <TimelineHeader @attributes="headerAttributes" />
}
```

## 📊 **Advanced Customization Patterns**

### **🔗 Data-Driven Headers**

#### **1. Database-Driven Configurations**
```csharp
public class DatabaseHeaderService : TimelineHeaderService
{
    private readonly IHeaderConfigurationRepository _configRepo;
    
    public override TimelineHeaderResult GenerateHeaderPeriods(...)
    {
        var config = await _configRepo.GetHeaderConfigurationAsync(zoomLevel);
        
        // Use database configuration to customize headers
        var result = base.GenerateHeaderPeriods(...);
        
        // Apply database-driven customizations
        ApplyDatabaseCustomizations(result, config);
        
        return result;
    }
    
    private void ApplyDatabaseCustomizations(TimelineHeaderResult result, HeaderConfiguration config)
    {
        foreach (var period in result.PrimaryPeriods)
        {
            var customization = config.GetCustomizationForPeriod(period.Start);
            if (customization != null)
            {
                period.Label = customization.CustomLabel ?? period.Label;
                period.CssClass += $" {customization.CssClass}";
                period.Tooltip = customization.Tooltip;
            }
        }
    }
}
```

#### **2. User Preference-Based Headers**
```csharp
public class UserPreferenceHeaderService : TimelineHeaderService
{
    private readonly IUserPreferenceService _userPrefs;
    
    protected override string GetFormattedLabel(DateTime date, string formatKey, TimelineZoomLevel zoomLevel)
    {
        var userFormat = _userPrefs.GetPreferredDateFormat(zoomLevel);
        if (!string.IsNullOrEmpty(userFormat))
        {
            return date.ToString(userFormat);
        }
        
        return base.GetFormattedLabel(date, formatKey, zoomLevel);
    }
    
    protected override bool ShouldCollapseHeaders(TimelineHeaderConfiguration config, double effectiveDayWidth, int timeSpanDays)
    {
        var userCollapsePref = _userPrefs.GetHeaderCollapsePreference();
        
        return userCollapsePref switch
        {
            HeaderCollapsePreference.Never => false,
            HeaderCollapsePreference.Always => timeSpanDays > 30,
            HeaderCollapsePreference.Auto => base.ShouldCollapseHeaders(config, effectiveDayWidth, timeSpanDays),
            _ => base.ShouldCollapseHeaders(config, effectiveDayWidth, timeSpanDays)
        };
    }
}
```

### **🎨 CSS-Only Customizations**

#### **1. Theme-Based Styling**
```css
/* Corporate theme */
.timeline-header.corporate-theme {
    --header-primary-bg: #2c3e50;
    --header-primary-color: #ffffff;
    --header-secondary-bg: #34495e;
    --header-secondary-color: #ecf0f1;
    --header-border-color: #7f8c8d;
    
    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
}

/* Agile theme */
.timeline-header.agile-theme {
    --header-primary-bg: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
    --header-primary-color: #ffffff;
    --header-secondary-bg: #f8f9fa;
    --header-secondary-color: #495057;
    
    border-radius: 4px 4px 0 0;
}

/* Dark theme */
.timeline-header.dark-theme {
    --header-primary-bg: #1a1a1a;
    --header-primary-color: #ffffff;
    --header-secondary-bg: #2d2d2d;
    --header-secondary-color: #cccccc;
    --header-border-color: #404040;
}
```

#### **2. Responsive Header Styling**
```css
/* Responsive header adjustments */
@media (max-width: 768px) {
    .timeline-header {
        --header-font-size: 12px;
        --header-padding: 2px 4px;
    }
    
    .header-primary-period {
        min-width: 40px;
    }
    
    .header-secondary-period {
        min-width: 20px;
    }
    
    /* Hide secondary headers on mobile for very small zoom levels */
    .timeline-header[data-zoom-level="YearQuarter3px"] .header-secondary-row,
    .timeline-header[data-zoom-level="YearQuarter4px"] .header-secondary-row {
        display: none;
    }
}
```

## 🧪 **Testing Custom Headers**

### **🔍 Unit Testing**
```csharp
[TestClass]
public class CustomHeaderServiceTests
{
    [TestMethod]
    public void AgileHeaderService_GeneratesSprintBoundaries()
    {
        // Arrange
        var service = new AgileHeaderService(mockDateFormatter, mockLogger);
        var startDate = new DateTime(2025, 1, 6); // Monday
        var endDate = new DateTime(2025, 1, 17);   // Friday
        
        // Act
        var result = service.GenerateHeaderPeriods(startDate, endDate, 
            TimelineZoomLevel.WeekDay97px, 97.0);
        
        // Assert
        Assert.IsTrue(result.SecondaryPeriods.Any(p => p.CssClass.Contains("sprint-start")));
        Assert.IsTrue(result.SecondaryPeriods.Any(p => p.CssClass.Contains("sprint-end")));
    }
    
    [TestMethod]
    public void ProductionHeaderService_IncludesWorkingDaysInLabel()
    {
        // Arrange
        var service = new ProductionHeaderService(mockDateFormatter, mockLogger);
        var startDate = new DateTime(2025, 1, 1);
        var endDate = new DateTime(2025, 3, 31);
        
        // Act
        var result = service.GenerateHeaderPeriods(startDate, endDate, 
            TimelineZoomLevel.QuarterMonth24px, 24.0);
        
        // Assert
        Assert.IsTrue(result.PrimaryPeriods.All(p => 
            p.Label.Contains("d)") || p.Label.Contains("days")));
    }
}
```

### **🔄 Integration Testing**
```csharp
[TestMethod]
public void CustomTimelineHeader_RendersCorrectly()
{
    // Arrange
    using var ctx = new TestContext();
    ctx.Services.AddScoped<ITimelineHeaderService, AgileHeaderService>();
    
    // Act
    var component = ctx.RenderComponent<CustomTimelineHeader>(parameters => parameters
        .Add(p => p.StartDate, new DateTime(2025, 1, 1))
        .Add(p => p.EndDate, new DateTime(2025, 1, 31))
        .Add(p => p.ZoomLevel, TimelineZoomLevel.MonthDay48px)
        .Add(p => p.ShowProjectMetrics, true));
    
    // Assert
    Assert.IsTrue(component.Find(".milestone-icon").Exists);
    Assert.IsTrue(component.Find(".period-metrics").Exists);
    Assert.IsTrue(component.Find(".task-count-badge").Exists);
}
```

## 📋 **Best Practices**

### **✅ Do's**
- ✅ **Extend services** rather than modifying core classes
- ✅ **Use dependency injection** for flexible service registration
- ✅ **Test custom logic** with comprehensive unit tests
- ✅ **Cache expensive calculations** in custom services
- ✅ **Use CSS custom properties** for consistent theming
- ✅ **Follow naming conventions** for CSS classes and service methods

### **❌ Don'ts**
- ❌ **Don't modify core TimelineView** component for customizations
- ❌ **Don't hardcode values** in custom services - use configuration
- ❌ **Don't ignore performance** when adding complex customizations
- ❌ **Don't break accessibility** with custom styling
- ❌ **Don't forget responsive design** in custom components

## 🎯 **Migration from v0.8.6**

If you had custom header modifications in the old inline system:

### **Before (v0.8.6 - Inline Modifications)**
```razor
@* Direct modifications to TimelineView.razor - NOT RECOMMENDED *@
@while (current <= EndDate)
{
    // Custom logic mixed with presentation
    var customLabel = GetMyCustomLabel(current);
    <div class="my-custom-header">@customLabel</div>
}
```

### **After (v0.8.7 - Service-Based)**
```csharp
// Clean service extension
public class MyCustomHeaderService : TimelineHeaderService
{
    protected override string GetFormattedLabel(DateTime date, string formatKey, TimelineZoomLevel zoomLevel)
    {
        return GetMyCustomLabel(date); // Your existing logic
    }
}
```

---

*This customization guide demonstrates the powerful flexibility achieved through the v0.8.7 header extraction, enabling everything from simple format changes to complete header redesigns while maintaining full backward compatibility.*
