# Task Date Semantics - StartDate(Inclusive) EndDate(Exclusive)

## Overview

This document defines the official date semantics used throughout the Blazor Gantt Components system. Understanding these semantics is crucial for proper task duration calculations, timeline rendering, and date arithmetic.

## Core Semantics

### StartDate (Inclusive)
- The task **begins** at the start of this date
- Work is performed **on** the start date
- Example: StartDate = "2025-01-15" means work begins on January 15th

### EndDate (Exclusive)  
- The task **ends** at the start of this date
- Work is **NOT** performed on the end date
- The end date represents the first day after the task is complete
- Example: EndDate = "2025-01-18" means work stops at the end of January 17th

## Duration Calculation

**Formula**: `Duration = EndDate - StartDate` (in days)

**Examples**:
- Single day task: StartDate="2025-01-15", EndDate="2025-01-16" = 1 day
- Multi-day task: StartDate="2025-01-15", EndDate="2025-01-20" = 5 days
- Week-long task: StartDate="2025-01-13", EndDate="2025-01-20" = 7 days

## Implementation Guidelines

### ✅ Correct Patterns

```csharp
// Duration calculation - NO +1 needed
var duration = (task.EndDate.ToUtcDateTime() - task.StartDate.ToUtcDateTime()).TotalDays;

// Timeline width calculation
var width = duration * dayWidth;

// Day counting between dates
var days = (endDate.Date - startDate.Date).Days;
```

### ❌ Incorrect Patterns (Legacy)

```csharp
// WRONG - Don't add +1 with exclusive end dates
var duration = (task.EndDate.ToUtcDateTime() - task.StartDate.ToUtcDateTime()).TotalDays + 1;

// WRONG - This treats EndDate as inclusive
var days = (endDate.Date - startDate.Date).Days + 1;
```

## Files Using Date Calculations

The following files have been updated to use proper inclusive/exclusive semantics:

### Core Models
- `Models/GanttTask.cs` - StartDate/EndDate documentation
- `Models/ValueObjects/GanttDate.cs` - Date-only value object

### Duration Calculations
- `Components/TimelineView/Renderers/BaseTimelineRenderer.cs` - CalculateCoordinateWidth
- `Components/TimelineView/TimelineView_Export.razor.cs` - CalculateTaskWidth, timeline properties
- `Components/TimelineView/TimelineView.razor.cs` - expanded bounds calculation
- `Components/GanttComposer/GanttComposer.razor` - tiny task detection
- `Components/TimelineView/Renderers/SVGRenderingHelpers.cs` - CalculateDaysBetween
- `Models/Filtering/TaskFilterCriteria.cs` - duration filtering

## Testing Date Semantics

When testing task dates, verify these scenarios:

### Single Day Tasks
```csharp
var task = new GanttTask 
{
    StartDate = new GanttDate(2025, 1, 15),  // January 15th
    EndDate = new GanttDate(2025, 1, 16)     // January 16th (exclusive)
};
// Expected duration: 1 day
// Expected work period: January 15th only
```

### Multi-Day Tasks
```csharp
var task = new GanttTask 
{
    StartDate = new GanttDate(2025, 1, 15),  // January 15th
    EndDate = new GanttDate(2025, 1, 20)     // January 20th (exclusive)
};
// Expected duration: 5 days
// Expected work period: January 15th through 19th
```

### Weekend Handling
Weekend dates are treated the same way - the semantics apply to calendar days, not just business days.

## Database Considerations

When storing tasks in the database:
- StartDate should be stored as the actual start date
- EndDate should be stored as the day AFTER the last work day
- Duration field (if present) should reflect the calculated duration using the exclusive end date

## Migration Notes

This change fixes a previous inconsistency where some code treated EndDate as inclusive while other code treated it as exclusive. All duration calculations now consistently use the inclusive/exclusive pattern.

### Breaking Changes
- Task durations will be calculated as 1 day shorter than before
- Timeline taskbar widths will be slightly narrower
- Tiny task detection thresholds may need adjustment

### Validation
After implementing these changes:
1. Verify single-day tasks render as expected width
2. Check that multi-day task calculations are correct
3. Test edge cases around month/year boundaries
4. Validate timeline coordinates align properly

## Industry Standards

This inclusive/exclusive pattern aligns with industry standards:
- Microsoft Project uses this pattern
- ISO 8601 duration standards
- Most project management tools
- Standard date range semantics in software development

The pattern prevents off-by-one errors and makes date arithmetic more intuitive.
