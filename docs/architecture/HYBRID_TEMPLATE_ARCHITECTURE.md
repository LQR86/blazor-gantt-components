# Hybrid Template-Based Architecture

## Overview

The Gantt timeline system uses a **hybrid architecture** that combines:
1. **Template-based semantic configuration** for business logic and zoom level management
2. **Day-width coordinate calculations** for efficient SVG positioning and mathematical operations

This design provides optimal performance, maintainability, and semantic clarity.

## Architecture Components

### 1. Template-Based Configuration Layer

#### TimelineZoomLevel Enum
```csharp
public enum TimelineZoomLevel
{
    YearQuarter = 23,    // Years → Quarters (Strategic planning)
    QuarterMonth = 27,   // Quarters → Months (Quarterly planning)  
    MonthWeek = 30,      // Months → Weeks (Monthly planning)
    WeekDay = 34         // Weeks → Days (Daily planning)
}
```

#### ZoomLevelConfiguration
```csharp
public class ZoomLevelConfiguration
{
    public double BaseUnitWidth { get; init; }     // Base pixels per template unit
    public double TemplateUnitDays { get; init; }  // Days per template unit
    public double MinZoomFactor { get; init; }     // Always 1.0
    public double MaxZoomFactor { get; init; }     // Template-specific maximum
}
```

**Template Configurations**:
- **YearQuarter**: 24px per quarter (90 days), Max Zoom: 4.0x
- **QuarterMonth**: 20px per month (30 days), Max Zoom: 3.5x  
- **MonthWeek**: 18px per week (7 days), Max Zoom: 3.0x
- **WeekDay**: 12px per day (1 day), Max Zoom: 2.5x

### 2. Coordinate Calculation Layer

#### Day-Width Derivation
```csharp
// Derived from template configuration
DayWidth = (BaseUnitWidth * ZoomFactor) / TemplateUnitDays

// Examples:
// WeekDay:       (12px * 2.0) / 1 day = 24px per day
// MonthWeek:     (18px * 2.0) / 7 days = 5.14px per day
// QuarterMonth:  (20px * 2.0) / 30 days = 1.33px per day
// YearQuarter:   (24px * 2.0) / 90 days = 0.53px per day
```

#### SVG Coordinate Conversion
```csharp
public static double DayToSVGX(DateTime date, DateTime startDate, double dayWidth)
{
    return (date - startDate).Days * dayWidth;
}
```

## Calculation Methods

### Primary: Template-Based Task Width
```csharp
// Direct calculation from task duration to pixels
public double CalculateTaskPixelWidth(double taskDurationDays, double zoomFactor)
{
    return (taskDurationDays / TemplateUnitDays) * BaseUnitWidth * zoomFactor;
}
```

### Secondary: Day-Width Coordinate System
```csharp
// Efficient coordinate calculation for positioning
public double GetEffectiveDayWidth(double zoomFactor)
{
    return (BaseUnitWidth * zoomFactor) / TemplateUnitDays;
}
```

**Mathematical Guarantee**: Both methods produce identical results:
```csharp
// These are equivalent:
var width1 = (taskDays / templateDays) * baseWidth * zoom;        // Template method
var width2 = taskDays * ((baseWidth * zoom) / templateDays);     // Day-width method
```

## Renderer Architecture

### BaseTimelineRenderer
```csharp
public abstract class BaseTimelineRenderer
{
    protected ZoomLevelConfiguration TemplateConfig { get; }
    protected double DayWidth => TemplateConfig.GetEffectiveDayWidth(ZoomFactor);
    
    // Template-based task width calculation
    protected double CalculateTaskWidth(double taskDurationDays)
    {
        return TemplateConfig.CalculateTaskPixelWidth(taskDurationDays, ZoomFactor);
    }
    
    // Day-width coordinate conversion
    protected double DateToX(DateTime date)
    {
        return SVGRenderingHelpers.DayToSVGX(date, CoordinateSystemStart, DayWidth);
    }
}
```

### Concrete Renderers
- **YearQuarterRenderer**: Handles yearly quarters with quarterly breakdown
- **QuarterMonthRenderer**: Handles quarterly months with monthly breakdown
- **MonthWeekRenderer**: Handles monthly weeks with weekly breakdown  
- **WeekDayRenderer**: Handles weekly days with daily breakdown

### RendererFactory
```csharp
public static BaseTimelineRenderer CreateRenderer(TimelineZoomLevel level, ...)
{
    return level switch
    {
        TimelineZoomLevel.YearQuarter => new YearQuarterRenderer(...),
        TimelineZoomLevel.QuarterMonth => new QuarterMonthRenderer(...),
        TimelineZoomLevel.MonthWeek => new MonthWeekRenderer(...),
        TimelineZoomLevel.WeekDay => new WeekDayRenderer(...),
        _ => throw new ArgumentException($"Unsupported zoom level: {level}")
    };
}
```

## Service Layer

### TimelineZoomService
```csharp
public static class TimelineZoomService
{
    // Template-based calculations
    public static double CalculateTaskPixelWidth(TimelineZoomLevel level, double taskDurationDays, double zoomFactor)
    
    // Day-width calculations (backward compatibility)
    public static double CalculateEffectiveDayWidth(TimelineZoomLevel level, double zoomFactor)
    
    // Zoom validation
    public static bool ValidateZoomFactor(TimelineZoomLevel level, double zoomFactor)
}
```

## Benefits of Hybrid Approach

### 1. Semantic Clarity
- Template names describe **structure** (YearQuarter) not implementation (YearQuarter90px)
- Business logic works with meaningful zoom levels
- Configuration is declarative and self-documenting

### 2. Performance Optimization
- Day-width calculations avoid repeated division operations
- SVG coordinate conversion is highly efficient
- Template configurations are computed once and cached

### 3. Mathematical Consistency
- Both calculation methods are mathematically equivalent
- Day-width is derived from template configuration
- No possibility of calculation mismatches

### 4. Maintainability
- Clear separation between business logic and coordinate mathematics
- Template changes automatically update day-width calculations
- Single source of truth for all zoom level parameters

### 5. Flexibility
- Easy to add new zoom levels by adding template configurations
- Zoom factors can be adjusted without affecting coordinate calculations
- Renderer implementations are decoupled from calculation methods

## Usage Examples

### Task Width Calculation
```csharp
// Template-based (primary method)
var taskWidth = TimelineZoomService.CalculateTaskPixelWidth(
    TimelineZoomLevel.MonthWeek, 
    taskDurationDays: 14.0, 
    zoomFactor: 2.0
);

// Day-width (coordinate system)
var dayWidth = TimelineZoomService.CalculateEffectiveDayWidth(
    TimelineZoomLevel.MonthWeek, 
    zoomFactor: 2.0
);
var taskWidth2 = taskDurationDays * dayWidth;  // Equivalent result
```

### Coordinate Positioning
```csharp
var dayWidth = TemplateConfig.GetEffectiveDayWidth(ZoomFactor);
var startX = SVGRenderingHelpers.DayToSVGX(task.StartDate, timelineStart, dayWidth);
var endX = SVGRenderingHelpers.DayToSVGX(task.EndDate, timelineStart, dayWidth);
```

## Design Principles

1. **Template-First**: Business logic uses template-based calculations
2. **Day-Width for Coordinates**: Coordinate systems use day-width for efficiency
3. **Mathematical Equivalence**: Both methods produce identical results
4. **Single Source of Truth**: Template configuration drives everything
5. **Performance Balance**: Semantic clarity with computational efficiency

This hybrid architecture represents a mature design pattern that leverages the strengths of both semantic configuration and efficient mathematical operations.
