# Header Type Analysis for Dual Boundary Expansion Strategy

## Overview
This document analyzes the header types produced by each TimelineView renderer to inform the dual boundary expansion strategy. Each renderer produces two header tiers (primary and secondary) that may have different boundary requirements.

## Renderer Analysis Summary

### 1. WeekDay50pxRenderer
- **Primary Header**: Week ranges ("February 17-23, 2025")
  - **Boundary Type**: Week boundaries (Monday to Sunday)
  - **Expansion Method**: GetWeekStart() and GetWeekEnd()
  - **Content**: Week ranges with month context

- **Secondary Header**: Day names with numbers ("Mon 17", "Tue 18")
  - **Boundary Type**: Day boundaries 
  - **Expansion Method**: Day-by-day rendering (no additional expansion needed)
  - **Content**: Daily breakdown with day names

- **Union Strategy**: Week boundaries are wider than day boundaries, so week expansion covers both tiers

### 2. MonthWeek50pxRenderer ⚠️ MISALIGNMENT ISSUE
- **Primary Header**: Month-Year ("February 2025", "March 2025")
  - **Boundary Type**: Month boundaries (1st to last day of month)
  - **Expansion Method**: GetMonthStart() and GetMonthEnd()
  - **Content**: Monthly overview with year context

- **Secondary Header**: Week start dates ("2/17", "2/24", "3/3") - Monday dates
  - **Boundary Type**: Week boundaries (Monday starts)
  - **Expansion Method**: Week-by-week rendering starting from Monday
  - **Content**: Weekly breakdown within months

- **Union Strategy**: ⚠️ **REQUIRES DUAL BOUNDARY CALCULATION**
  - Month boundaries: January 1 - January 31
  - Week boundaries: Monday before January 1 - Sunday after January 31
  - Union: Take the widest span from both boundary types

### 3. QuarterMonth60pxRenderer ⚠️ MISALIGNMENT ISSUE  
- **Primary Header**: Quarter ranges ("Q1 2025", "Q2 2025")
  - **Boundary Type**: Quarter boundaries (start of quarter to end of quarter)
  - **Expansion Method**: GetQuarterStart() and GetQuarterEnd()
  - **Content**: Quarterly overview with year context

- **Secondary Header**: Month names ("Jan", "Feb", "Mar")
  - **Boundary Type**: Month boundaries
  - **Expansion Method**: Month-by-month rendering
  - **Content**: Monthly breakdown within quarters

- **Union Strategy**: ⚠️ **REQUIRES DUAL BOUNDARY CALCULATION**
  - Quarter boundaries: January 1 - March 31 (Q1)
  - Month boundaries: January 1 - March 31 (same for Q1, but different for partial quarters)
  - Union: Take the widest span when quarters are partially visible

### 4. YearQuarter90pxRenderer ✅ NO MISALIGNMENT
- **Primary Header**: Year ranges ("2025", "2026")
  - **Boundary Type**: Year boundaries (January 1 to December 31)
  - **Expansion Method**: Year start/end calculation
  - **Content**: Annual overview

- **Secondary Header**: Quarter labels ("Q1", "Q2", "Q3", "Q4")
  - **Boundary Type**: Quarter boundaries within years
  - **Expansion Method**: Quarter-by-quarter rendering within year span
  - **Content**: Quarterly breakdown within years

- **Union Strategy**: Year boundaries are wider than quarter boundaries, so year expansion covers both tiers

## Boundary Calculation Requirements

### Current Single Boundary Approach (Per Renderer)
```csharp
// Each renderer calculates its own expansion
protected override (DateTime expandedStart, DateTime expandedEnd) CalculateHeaderBoundaries()
{
    // WeekDay: Extend to week boundaries
    var expandedStart = GetWeekStart(StartDate);
    var expandedEnd = GetWeekEnd(EndDate);
    return (expandedStart, expandedEnd);
}
```

### Required Dual Boundary Approach
```csharp
// ABC composition enforcement with sealed method pattern
public sealed (DateTime expandedStart, DateTime expandedEnd) CalculateUnionBoundaries()
{
    var primaryBoundaries = CalculatePrimaryHeaderBoundaries();
    var secondaryBoundaries = CalculateSecondaryHeaderBoundaries();
    
    // Union: Take widest span
    var unionStart = primaryBoundaries.start < secondaryBoundaries.start 
        ? primaryBoundaries.start : secondaryBoundaries.start;
    var unionEnd = primaryBoundaries.end > secondaryBoundaries.end 
        ? primaryBoundaries.end : secondaryBoundaries.end;
        
    return (unionStart, unionEnd);
}

// Renderers must implement both boundary types
protected abstract (DateTime start, DateTime end) CalculatePrimaryHeaderBoundaries();
protected abstract (DateTime start, DateTime end) CalculateSecondaryHeaderBoundaries();
```

## Misalignment Root Cause Analysis

### MonthWeek Pattern Misalignment
- **Primary**: Month boundaries may start mid-week (e.g., February 1 = Wednesday)
- **Secondary**: Week boundaries start on Monday, extending before/after month
- **Visual Issue**: Week headers may be truncated at month edges
- **Solution**: Union expansion to include complete weeks that overlap month boundaries

### QuarterMonth Pattern Misalignment  
- **Primary**: Quarter boundaries align with month boundaries (Jan 1, Apr 1, Jul 1, Oct 1)
- **Secondary**: Month boundaries within quarters (same boundaries in most cases)
- **Visual Issue**: Minimal misalignment unless partial quarters are displayed
- **Solution**: Union expansion for edge cases where partial quarters are shown

## Implementation Priority

### Phase 1: Foundation (✅ Complete)
- BoundaryCalculationHelpers utility class
- Centralized boundary calculation methods
- Comprehensive test coverage (42 tests)

### Phase 2: ABC Composition Enforcement (Next)
- Enhance BaseTimelineRenderer with dual boundary sealed method
- Force all renderers to implement both primary and secondary boundary methods
- Automatic union calculation with no renderer-specific logic

### Phase 3: Renderer Updates
1. **High Priority**: MonthWeek50pxRenderer (significant misalignment)
2. **Medium Priority**: QuarterMonth60pxRenderer (edge case misalignment)  
3. **Low Priority**: WeekDay50pxRenderer and YearQuarter90pxRenderer (no misalignment, but enforce consistency)

## Boundary Type Mapping

| Renderer | Primary Boundary | Secondary Boundary | Union Required |
|----------|------------------|-------------------|----------------|
| WeekDay50px | Week | Day | No (Week ⊃ Day) |
| MonthWeek50px | Month | Week | **Yes** (Month ≠ Week) |
| QuarterMonth60px | Quarter | Month | **Yes** (Quarter ≠ Month in edge cases) |
| YearQuarter90px | Year | Quarter | No (Year ⊃ Quarter) |

## Testing Strategy

### Dual Boundary Test Cases
```csharp
[Theory]
[InlineData("2025-02-15", "2025-02-25")] // Month boundaries vs week boundaries
[InlineData("2025-03-29", "2025-04-05")] // Cross-month week boundaries  
[InlineData("2025-12-29", "2026-01-05")] // Cross-year boundaries
public void MonthWeek_DualBoundaryExpansion_ShouldCalculateUnion(string start, string end)
{
    // Test that union expansion covers both month and week requirements
}
```

### Regression Test Coverage
- Ensure no header truncation at any zoom level
- Verify consistent visual alignment between primary and secondary headers
- Validate boundary edge cases (leap years, month lengths, week transitions)

## Next Steps (Commit 3)

1. **Enhance BaseTimelineRenderer with sealed union method**
2. **Add abstract methods for primary/secondary boundary calculation**
3. **Implement automatic union logic using BoundaryCalculationHelpers**
4. **Validate ABC composition enforcement pattern**

This analysis provides the foundation for implementing dual boundary expansion across all timeline renderers with pixel-perfect header alignment.
