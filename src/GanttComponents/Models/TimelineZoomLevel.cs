namespace GanttComponents.Models;

/// <summary>
/// Defines the optimal 11-level integral pixel zoom system for timeline visualization.
/// Fibonacci-like progression: [3, 4, 6, 8, 12, 17, 24, 34, 48, 68, 97] px
/// 5 pattern groups with natural transitions: YearQuarter → Month-only → QuarterMonth → MonthDay → WeekDay
/// All day widths are integral pixels for crisp rendering and optimal performance.
/// </summary>
public enum TimelineZoomLevel
{
    /// <summary>
    /// Year-Quarter Minimal level: 3px per day (Level 1)
    /// Best for: Long-term strategic view with minimum day width
    /// Pattern: Year → Quarter
    /// Enforces 3px minimum day width boundary
    /// </summary>
    YearQuarter3px = 0,

    /// <summary>
    /// Year-Quarter Medium level: 4px per day (Level 2)
    /// Best for: Medium annual view with quarterly periods
    /// Pattern: Year → Quarter
    /// </summary>
    YearQuarter4px = 1,

    /// <summary>
    /// Year-Quarter High level: 6px per day (Level 3)
    /// Best for: Annual overview with quarterly breakdown
    /// Pattern: Year → Quarter
    /// </summary>
    YearQuarter6px = 2,

    /// <summary>
    /// Month-only Medium level: 8px per day (Level 4)
    /// Best for: Compact monthly view for overview
    /// Pattern: Month-only
    /// </summary>
    Month8px = 3,

    /// <summary>
    /// Month-only High level: 12px per day (Level 5)
    /// Best for: Month-focused view for project planning
    /// Pattern: Month-only
    /// </summary>
    Month12px = 4,

    /// <summary>
    /// Quarter-Month Medium level: 17px per day (Level 6)
    /// Best for: Medium quarterly view with monthly periods
    /// Pattern: Quarter → Month
    /// </summary>
    QuarterMonth17px = 5,

    /// <summary>
    /// Quarter-Month High level: 24px per day (Level 7)
    /// Best for: Quarterly overview with monthly breakdown
    /// Pattern: Quarter → Month
    /// </summary>
    QuarterMonth24px = 6,

    /// <summary>
    /// Month-Day Medium level: 34px per day (Level 8)
    /// Best for: Medium monthly view with daily periods
    /// Pattern: Month → Day
    /// </summary>
    MonthDay34px = 7,

    /// <summary>
    /// Month-Day High level: 48px per day (Level 9)
    /// Best for: Monthly overview with daily breakdown
    /// Pattern: Month → Day
    /// </summary>
    MonthDay48px = 8,

    /// <summary>
    /// Week-Day High level: 68px per day (Level 10)
    /// Best for: High detail weekly view with daily tracking
    /// Pattern: Week → Day (with year context)
    /// </summary>
    WeekDay68px = 9,

    /// <summary>
    /// Week-Day Maximum level: 97px per day (Level 11)
    /// Best for: Maximum detail weekly view with daily granularity
    /// Pattern: Week → Day (with year context)
    /// </summary>
    WeekDay97px = 10
}
