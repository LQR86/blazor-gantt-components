# Header Cell Boundary Implementation

## Overview
This document describes the logical unit boundary expansion implementation that ensures header cells in the TimelineView are always displayed as complete logical units, preventing partial headers at timeline edges.

## Problem Statement
Previously, header cells at timeline boundaries could be cut in half due to simple day-based padding calculations. This created visual inconsistencies where users would see partial weeks, months, quarters, or years at the edges of the timeline.

## Solution: Logical Unit Boundary Expansion

### Core Concept
Instead of using arbitrary day-based padding, the timeline range is expanded to accommodate complete logical units based on the zoom level being rendered. This ensures header cells always display meaningful, complete time periods.

### Architecture

#### BaseTimelineRenderer Changes
The abstract base class now defines a logical unit boundary expansion pattern:

```csharp
// Abstract method for each renderer to define its logical boundaries
protected abstract (DateOnly expandedStart, DateOnly expandedEnd) GetLogicalUnitBoundaries(
    DateOnly originalStart, DateOnly originalEnd);

// Modified boundary calculation to use logical units
protected (DateOnly start, DateOnly end) CalculateHeaderBoundaries(
    DateOnly timelineStart, DateOnly timelineEnd)
{
    return GetLogicalUnitBoundaries(timelineStart, timelineEnd);
}
```

#### Per-Renderer Implementation

**WeekDayRenderer**
- Expands to complete weeks (Monday to Sunday)
- Uses `BoundaryCalculationHelpers.GetWeekBoundaries()`
- Ensures no partial weeks at timeline edges

**MonthWeekRenderer**
- Calculates union of month boundaries and week boundaries
- Guarantees complete months AND complete weeks
- Takes the widest span to accommodate both logical units

**QuarterMonthRenderer**
- Calculates union of quarter boundaries and month boundaries
- Ensures complete quarters AND complete months
- Expands to encompass both logical unit types

**YearQuarterRenderer**
- Calculates union of year boundaries and quarter boundaries
- Guarantees complete years AND complete quarters
- Uses the broadest span for both logical units

### Implementation Details

#### Boundary Calculation Pattern
All renderers follow this union pattern for multi-level displays:

```csharp
protected override (DateOnly expandedStart, DateOnly expandedEnd) GetLogicalUnitBoundaries(
    DateOnly originalStart, DateOnly originalEnd)
{
    var primaryBoundaries = BoundaryCalculationHelpers.GetPrimaryUnitBoundaries(originalStart, originalEnd);
    var secondaryBoundaries = BoundaryCalculationHelpers.GetSecondaryUnitBoundaries(originalStart, originalEnd);
    
    // Union: take the earliest start and latest end
    return (
        primaryBoundaries.start < secondaryBoundaries.start ? primaryBoundaries.start : secondaryBoundaries.start,
        primaryBoundaries.end > secondaryBoundaries.end ? primaryBoundaries.end : secondaryBoundaries.end
    );
}
```

#### BoundaryCalculationHelpers Usage
The implementation leverages existing boundary calculation utilities:
- `GetWeekBoundaries()` - Monday to Sunday spans
- `GetMonthBoundaries()` - First to last day of months
- `GetQuarterBoundaries()` - Quarter start/end dates
- `GetYearBoundaries()` - January 1st to December 31st spans

### Benefits

1. **Visual Consistency**: Header cells are always complete logical units
2. **User Experience**: No confusing partial time periods displayed
3. **Coordinate Integrity**: Timeline coordinate system remains unchanged
4. **Maintainable**: Clear abstraction pattern for future renderers
5. **Performance**: Minimal overhead for boundary calculations

### Backward Compatibility
- No breaking changes to existing coordinate system
- Template unit calculations remain unchanged
- All existing alignment validation continues to work
- API surface unchanged for consuming components

### Testing Validation
- All zoom levels display complete header cells at edges
- TaskGrid alignment maintained across all scenarios
- No regression in timeline functionality
- Coordinate calculations remain accurate

## Future Considerations
- Pattern can be extended for new zoom levels
- Boundary calculation helpers can be enhanced for additional logical units
- Implementation supports custom logical unit definitions if needed

## Related Files
- `src/GanttComponents/Components/TimelineView/Renderers/BaseTimelineRenderer.cs`
- `src/GanttComponents/Components/TimelineView/Renderers/WeekDayRenderer.cs`
- `src/GanttComponents/Components/TimelineView/Renderers/MonthWeekRenderer.cs`
- `src/GanttComponents/Components/TimelineView/Renderers/QuarterMonthRenderer.cs`
- `src/GanttComponents/Components/TimelineView/Renderers/YearQuarterRenderer.cs`
- `src/GanttComponents/Utilities/BoundaryCalculationHelpers.cs`
