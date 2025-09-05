# Template-Based Zoom Design

## Overview

The timeline system uses **template-based zoom levels** that provide semantic meaning for different planning scales. Each template is optimized for specific use cases and time horizons.

## Template Hierarchy

### 1. YearQuarter Template
**Purpose**: Strategic planning and long-term roadmaps

```csharp
TimelineZoomLevel.YearQuarter
├── BaseUnitWidth: 24px per quarter
├── TemplateUnitDays: 90 days per quarter  
├── MaxZoomFactor: 4.0x
├── Day Width Range: 0.27px (1.0x) - 1.07px (4.0x)
├── Primary Header: Years ("2025", "2026")
└── Secondary Header: Quarters ("Q1", "Q2", "Q3", "Q4")
```

**Optimal For**:
- Multi-year strategic planning
- Quarterly milestone tracking
- Executive dashboard views
- Long-term resource allocation

### 2. QuarterMonth Template
**Purpose**: Quarterly planning with monthly breakdown

```csharp
TimelineZoomLevel.QuarterMonth
├── BaseUnitWidth: 20px per month
├── TemplateUnitDays: 30 days per month
├── MaxZoomFactor: 3.5x
├── Day Width Range: 0.67px (1.0x) - 2.33px (3.5x)
├── Primary Header: Quarters ("Q1 2025", "Q2 2025")
└── Secondary Header: Months ("Jan", "Feb", "Mar")
```

**Optimal For**:
- Quarterly planning cycles
- Monthly deliverable tracking
- Budget planning periods
- Department-level coordination

### 3. MonthWeek Template
**Purpose**: Monthly planning with weekly breakdown

```csharp
TimelineZoomLevel.MonthWeek
├── BaseUnitWidth: 18px per week
├── TemplateUnitDays: 7 days per week
├── MaxZoomFactor: 3.0x
├── Day Width Range: 2.57px (1.0x) - 7.71px (3.0x)
├── Primary Header: Months ("February 2025", "March 2025")
└── Secondary Header: Week starts ("2/17", "2/24", "3/3")
```

**Optimal For**:
- Sprint planning (2-4 week sprints)
- Monthly project management
- Team coordination
- Short-term task scheduling

### 4. WeekDay Template
**Purpose**: Weekly planning with daily breakdown

```csharp
TimelineZoomLevel.WeekDay
├── BaseUnitWidth: 12px per day
├── TemplateUnitDays: 1 day
├── MaxZoomFactor: 2.5x
├── Day Width Range: 12px (1.0x) - 30px (2.5x)
├── Primary Header: Week ranges ("February 17-23, 2025")
└── Secondary Header: Day names ("Mon 17", "Tue 18")
```

**Optimal For**:
- Daily task management
- Detailed scheduling
- Individual contributor planning
- Time tracking applications

## Zoom Factor Scaling

### Template-Native Scaling
Each template has a **native zoom range** from 1.0x to its maximum:

```csharp
// All templates start at 1.0x (template native resolution)
MinZoomFactor = 1.0

// Maximum varies by template complexity
YearQuarter:   MaxZoomFactor = 4.0x   // Can scale up 4x from base
QuarterMonth:  MaxZoomFactor = 3.5x   // Can scale up 3.5x from base  
MonthWeek:     MaxZoomFactor = 3.0x   // Can scale up 3x from base
WeekDay:       MaxZoomFactor = 2.5x   // Can scale up 2.5x from base
```

### Day-Width Progression
As zoom factor increases, effective day width scales proportionally:

```csharp
// Examples at different zoom factors:
YearQuarter at 1.0x:   0.27px per day (strategic overview)
YearQuarter at 4.0x:   1.07px per day (detailed strategic)

MonthWeek at 1.0x:     2.57px per day (weekly overview)  
MonthWeek at 3.0x:     7.71px per day (detailed weekly)

WeekDay at 1.0x:       12px per day (daily overview)
WeekDay at 2.5x:       30px per day (detailed daily)
```

## Template Selection Logic

### Automatic Template Selection
The system can automatically suggest templates based on date range:

```csharp
public static TimelineZoomLevel SuggestTemplate(DateTime startDate, DateTime endDate)
{
    var daySpan = (endDate - startDate).Days;
    
    return daySpan switch
    {
        <= 30 => TimelineZoomLevel.WeekDay,      // Up to 1 month: daily detail
        <= 120 => TimelineZoomLevel.MonthWeek,   // Up to 4 months: weekly planning
        <= 365 => TimelineZoomLevel.QuarterMonth, // Up to 1 year: monthly planning
        _ => TimelineZoomLevel.YearQuarter       // Over 1 year: strategic planning
    };
}
```

### Manual Template Override
Users can override automatic selection for specific use cases:

- **Strategic Sessions**: Use YearQuarter even for shorter periods
- **Sprint Planning**: Use MonthWeek even for longer periods  
- **Daily Standup**: Use WeekDay for detailed task tracking
- **Executive Review**: Use QuarterMonth for quarterly summaries

## Header Design Philosophy

### Dual-Header System
Each template uses a **primary/secondary header** structure:

```
Primary Header:    [Broader Context] - Shows containing period
Secondary Header:  [Detailed Units] - Shows actionable units
```

### Template-Specific Headers

**YearQuarter**:
```
Primary:   │  2025  │  2026  │  2027  │
Secondary: │Q1│Q2│Q3│Q4│Q1│Q2│Q3│Q4│Q1│Q2│
```

**QuarterMonth**:
```
Primary:   │   Q1 2025   │   Q2 2025   │
Secondary: │Jan│Feb│Mar│Apr│May│Jun│
```

**MonthWeek**:
```
Primary:   │ February 2025 │ March 2025  │
Secondary: │2/3│2/10│2/17│2/24│3/3│3/10│
```

**WeekDay**:
```
Primary:   │February 17-23, 2025│February 24-Mar 2, 2025│
Secondary: │Mo│Tu│We│Th│Fr│Sa│Su│Mo│Tu│We│Th│Fr│Sa│Su│
```

## Configuration Driven Design

### ZoomLevelConfiguration
All template behavior is driven by configuration:

```csharp
var yearQuarterConfig = new ZoomLevelConfiguration
{
    Level = TimelineZoomLevel.YearQuarter,
    BaseUnitWidth = 24.0,      // 24px per quarter at 1.0x
    TemplateUnitDays = 90.0,   // 90 days per quarter
    MinZoomFactor = 1.0,       // Always starts at native resolution
    MaxZoomFactor = 4.0,       // Can zoom up to 4x
    DisplayNameKey = "ZoomLevel.YearQuarter",
    DescriptionKey = "ZoomLevel.YearQuarter.Description"
};
```

### Benefits of Configuration-Driven Design

1. **Consistency**: All templates follow the same calculation patterns
2. **Maintainability**: Easy to adjust template parameters
3. **Extensibility**: New templates can be added by configuration
4. **Testability**: Template behavior is predictable and testable
5. **Localization**: Display names and descriptions are externalized

## Template Transition Strategy

### Zoom-Based Transitions
When zooming reaches template limits, the system can suggest template changes:

```csharp
// Zooming in: When day-width becomes too large for useful display
if (currentDayWidth > 40px && currentTemplate != TimelineZoomLevel.WeekDay)
{
    // Suggest switching to more detailed template
    SuggestTemplate(GetMoreDetailedTemplate(currentTemplate));
}

// Zooming out: When day-width becomes too small for readability  
if (currentDayWidth < 0.5px && currentTemplate != TimelineZoomLevel.YearQuarter)
{
    // Suggest switching to more strategic template
    SuggestTemplate(GetMoreStrategicTemplate(currentTemplate));
}
```

### Template Hierarchy
```
YearQuarter (Strategic)
    ↓ (zoom in)
QuarterMonth (Tactical)
    ↓ (zoom in)  
MonthWeek (Operational)
    ↓ (zoom in)
WeekDay (Detailed)
```

## Design Principles

1. **Semantic Naming**: Template names describe structure, not implementation
2. **Use Case Optimization**: Each template targets specific planning scenarios
3. **Consistent Scaling**: All templates use the same zoom factor approach
4. **Configuration Driven**: Behavior is externalized and maintainable
5. **Header Consistency**: Dual-header system provides context and detail
6. **Performance Balance**: Templates balance visual clarity with performance

This template-based design provides a scalable foundation for timeline visualization that adapts to different planning needs while maintaining consistent user experience and performance characteristics.
