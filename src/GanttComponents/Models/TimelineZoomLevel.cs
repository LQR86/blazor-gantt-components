namespace GanttComponents.Models;

/// <summary>
/// Timeline zoom levels for ABC composition architecture.
/// DEVELOPMENT PHASE: Only implemented levels included for clean codebase.
/// 4 distinct timeline patterns with different planning perspectives.
/// </summary>
public enum TimelineZoomLevel
{
    /// <summary>
    /// Year-Quarter Pattern: 1.0px per day (Strategic Overview)
    /// Primary Header: Years ("2025", "2026") 
    /// Secondary Header: Quarters ("Q1", "Q2", "Q3", "Q4")
    /// Best for: Long-term strategic planning with quarterly milestones
    /// ABC Renderer: YearQuarter90pxRenderer
    /// </summary>
    YearQuarterOptimal70px = 23,

    /// <summary>
    /// Quarter-Month Pattern: 2.0px per day (Quarterly Planning)
    /// Primary Header: Quarters ("Q1 2025", "Q2 2025")
    /// Secondary Header: Months ("Jan", "Feb", "Mar")
    /// Best for: Quarterly planning with monthly breakdown
    /// ABC Renderer: QuarterMonth60pxRenderer
    /// </summary>
    QuarterMonthOptimal60px = 27,

    /// <summary>
    /// Month-Week Pattern: 8px per day (Monthly Planning)
    /// Primary Header: Months ("February 2025", "March 2025")
    /// Secondary Header: Week start dates ("2/17", "2/24", "3/3")
    /// Best for: Monthly planning with weekly breakdown
    /// ABC Renderer: MonthWeek50pxRenderer
    /// </summary>
    MonthWeekOptimal50px = 30,

    /// <summary>
    /// Week-Day Pattern: 50px per day (Weekly Planning)
    /// Primary Header: Week ranges ("February 17-23, 2025")
    /// Secondary Header: Day names with numbers ("Mon 17", "Tue 18")
    /// Best for: Detailed weekly planning with daily breakdown
    /// ABC Renderer: WeekDay50pxRenderer
    /// </summary>
    WeekDayOptimal50px = 34
}
