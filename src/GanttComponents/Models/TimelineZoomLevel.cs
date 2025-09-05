namespace GanttComponents.Models;

/// <summary>
/// Timeline zoom levels for template-based architecture.
/// Uses duration-to-pixel mapping: TaskPixelWidth = (TaskDurationDays ÷ TemplateUnitDays) × BaseUnitWidth × ZoomFactor
/// ZoomFactor range: 1.0x (template-native) to template-specific maximum.
/// </summary>
public enum TimelineZoomLevel
{
    /// <summary>
    /// Year-Quarter Template: 24px per quarter (90 days), Max Zoom: 4.0x
    /// Primary Header: Years ("2025", "2026") 
    /// Secondary Header: Quarters ("Q1", "Q2", "Q3", "Q4")
    /// Best for: Long-term strategic planning with quarterly milestones
    /// Day Width Range: 0.27px (1.0x) - 1.07px (4.0x)
    /// </summary>
    YearQuarter = 23,

    /// <summary>
    /// Quarter-Month Template: 20px per month (30 days), Max Zoom: 3.5x
    /// Primary Header: Quarters ("Q1 2025", "Q2 2025")
    /// Secondary Header: Months ("Jan", "Feb", "Mar")
    /// Best for: Quarterly planning with monthly breakdown
    /// Day Width Range: 0.67px (1.0x) - 2.33px (3.5x)
    /// </summary>
    QuarterMonth = 27,

    /// <summary>
    /// Month-Week Template: 18px per week (7 days), Max Zoom: 3.0x
    /// Primary Header: Months ("February 2025", "March 2025")
    /// Secondary Header: Week start dates ("2/17", "2/24", "3/3")
    /// Best for: Monthly planning with weekly breakdown
    /// Day Width Range: 2.57px (1.0x) - 7.71px (3.0x)
    /// </summary>
    MonthWeek = 30,

    /// <summary>
    /// Week-Day Template: 12px per day (1 day), Max Zoom: 2.5x
    /// Primary Header: Week ranges ("February 17-23, 2025")
    /// Secondary Header: Day names with numbers ("Mon 17", "Tue 18")
    /// Best for: Detailed weekly planning with daily breakdown
    /// Day Width Range: 12px (1.0x) - 30px (2.5x)
    /// </summary>
    WeekDay = 34
}
