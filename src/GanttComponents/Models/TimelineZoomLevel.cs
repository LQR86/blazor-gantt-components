namespace GanttComponents.Models;

/// <summary>
/// Defines the strategic zoom levels for timeline visualization.
/// 13-level system with 4 pattern groups: WeekDay → MonthWeek → QuarterMonth → YearQuarter
/// Each pattern provides multiple granularity options for smooth transitions.
/// </summary>
public enum TimelineZoomLevel
{
    /// <summary>
    /// Week-Day level: 60px per day
    /// Best for: Daily sprint planning with weekly context
    /// Pattern: Week → Day
    /// </summary>
    WeekDay = 0,

    /// <summary>
    /// Week-Day Medium level: 45px per day
    /// Best for: Medium weekly view with daily granularity
    /// Pattern: Week → Day
    /// </summary>
    WeekDayMedium = 1,

    /// <summary>
    /// Week-Day Low level: 35px per day
    /// Best for: Compact weekly view with daily tracking
    /// Pattern: Week → Day
    /// </summary>
    WeekDayLow = 2,

    /// <summary>
    /// Month-Week level: 24px per day (adjusted from 25px in planning)
    /// Best for: Monthly overview with weekly breakdown
    /// Pattern: Quarter → Month (showing Month → Week)
    /// </summary>
    MonthWeek = 3,

    /// <summary>
    /// Month-Week Medium level: 19.2px per day (adjusted from 20px in planning)
    /// Best for: Medium monthly view with weekly periods
    /// Pattern: Quarter → Month (showing Month → Week)
    /// </summary>
    MonthWeekMedium = 4,

    /// <summary>
    /// Month-Week Low level: 16px per day (adjusted from 18px in planning)
    /// Best for: Compact monthly view with weekly periods
    /// Pattern: Quarter → Month (showing Month → Week)
    /// </summary>
    MonthWeekLow = 5,

    /// <summary>
    /// Quarter-Month level: 15px per day
    /// Best for: Quarterly overview with monthly breakdown
    /// Pattern: Quarter → Month
    /// </summary>
    QuarterMonth = 6,

    /// <summary>
    /// Quarter-Month Medium level: 12px per day
    /// Best for: Medium quarterly view with monthly periods
    /// Pattern: Quarter → Month
    /// </summary>
    QuarterMonthMedium = 7,

    /// <summary>
    /// Quarter-Month Low level: 10px per day
    /// Best for: Compact quarterly view with monthly markers
    /// Pattern: Quarter → Month
    /// </summary>
    QuarterMonthLow = 8,

    /// <summary>
    /// Year-Quarter level: 8px per day
    /// Best for: Annual overview with quarterly breakdown
    /// Pattern: Year → Quarter
    /// </summary>
    YearQuarter = 9,

    /// <summary>
    /// Year-Quarter Medium level: 6.5px per day
    /// Best for: Medium annual view with quarterly periods
    /// Pattern: Year → Quarter
    /// </summary>
    YearQuarterMedium = 10,

    /// <summary>
    /// Year-Quarter Low level: 5px per day
    /// Best for: Extended multi-year planning with quarterly markers
    /// Pattern: Year → Quarter
    /// </summary>
    YearQuarterLow = 11,

    /// <summary>
    /// Year-Quarter Minimal level: 3px per day (minimum boundary)
    /// Best for: Long-term strategic view with minimum day width
    /// Pattern: Year → Quarter
    /// Enforces 3px minimum day width to maintain task visibility
    /// </summary>
    YearQuarterMin = 12
}
