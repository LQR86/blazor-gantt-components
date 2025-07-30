namespace GanttComponents.Models;

/// <summary>
/// Defines the strategic zoom levels for timeline visualization.
/// Each level optimizes display for different project planning needs.
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
    /// Month-Day level: 25px per day
    /// Best for: Monthly project planning (1-6 months)
    /// Typical use: Feature development cycles, milestone tracking
    /// </summary>
    MonthDay = 1,

    /// <summary>
    /// Month-Week level: 15px per day
    /// Best for: Quarterly planning (3-12 months)
    /// Typical use: Release planning, resource allocation
    /// </summary>
    MonthWeek = 2,

    /// <summary>
    /// Quarter-Week level: 8px per day
    /// Best for: Annual planning (6-24 months)
    /// Typical use: Strategic roadmaps, long-term projects
    /// </summary>
    QuarterWeek = 3,

    /// <summary>
    /// Quarter-Month level: 5px per day
    /// Best for: Multi-year overview (1-5 years)
    /// Typical use: Program management, portfolio planning
    /// </summary>
    QuarterMonth = 4,

    /// <summary>
    /// Year-Quarter level: 3px per day
    /// Best for: Long-term strategic view (2-10 years)
    /// Typical use: Strategic planning, enterprise roadmaps
    /// </summary>
    YearQuarter = 5
}
