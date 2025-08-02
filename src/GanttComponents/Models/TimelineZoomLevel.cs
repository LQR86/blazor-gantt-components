namespace GanttComponents.Models;

/// <summary>
/// Defines the strategic zoom levels for timeline visualization.
/// Each level optimizes display for different project planning needs.
/// Expanded to 13 preset-only levels for smooth zoom transitions.
/// </summary>
public enum TimelineZoomLevel
{
    /// <summary>
    /// Week-Day level: 60px per day
    /// Best for: Short-term detailed scheduling (1-4 weeks)
    /// Typical use: Sprint planning, daily task management
    /// </summary>
    WeekDay = 0,

    /// <summary>
    /// Week-Day Medium level: 45px per day
    /// Best for: Short-term planning with slightly wider view (2-6 weeks)
    /// Typical use: Sprint planning, detailed project phases
    /// </summary>
    WeekDayMedium = 1,

    /// <summary>
    /// Week-Day Low level: 35px per day
    /// Best for: Short to medium-term planning (1-8 weeks)
    /// Typical use: Multi-sprint planning, feature cycles
    /// </summary>
    WeekDayLow = 2,

    /// <summary>
    /// Month-Day level: 25px per day
    /// Best for: Monthly project planning (1-6 months)
    /// Typical use: Feature development cycles, milestone tracking
    /// </summary>
    MonthDay = 3,

    /// <summary>
    /// Month-Day Medium level: 20px per day
    /// Best for: Medium-term project planning (2-8 months)
    /// Typical use: Release planning, phase management
    /// </summary>
    MonthDayMedium = 4,

    /// <summary>
    /// Month-Week level: 15px per day
    /// Best for: Quarterly planning (3-12 months)
    /// Typical use: Release planning, resource allocation
    /// </summary>
    MonthWeek = 5,

    /// <summary>
    /// Month-Week Medium level: 12px per day
    /// Best for: Extended quarterly planning (4-15 months)
    /// Typical use: Product roadmap, resource planning
    /// </summary>
    MonthWeekMedium = 6,

    /// <summary>
    /// Month-Week Low level: 10px per day
    /// Best for: Long-term quarterly planning (6-18 months)
    /// Typical use: Strategic roadmaps, portfolio planning
    /// </summary>
    MonthWeekLow = 7,

    /// <summary>
    /// Quarter-Week level: 8px per day
    /// Best for: Annual planning (6-24 months)
    /// Typical use: Strategic roadmaps, long-term projects
    /// </summary>
    QuarterWeek = 8,

    /// <summary>
    /// Quarter-Week Medium level: 6.5px per day
    /// Best for: Extended annual planning (12-30 months)
    /// Typical use: Multi-year project timelines, portfolio management
    /// </summary>
    QuarterWeekMedium = 9,

    /// <summary>
    /// Quarter-Month level: 5px per day
    /// Best for: Multi-year overview (1-5 years)
    /// Typical use: Program management, portfolio planning
    /// </summary>
    QuarterMonth = 10,

    /// <summary>
    /// Quarter-Month Medium level: 4px per day
    /// Best for: Extended multi-year planning (2-6 years)
    /// Typical use: Strategic initiatives, enterprise planning
    /// </summary>
    QuarterMonthMedium = 11,

    /// <summary>
    /// Year-Quarter level: 3px per day (minimum boundary)
    /// Best for: Long-term strategic view (2-10 years)
    /// Typical use: Strategic planning, enterprise roadmaps
    /// Enforces 3px minimum day width to maintain task visibility
    /// </summary>
    YearQuarter = 12
}
