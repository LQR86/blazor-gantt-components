using GanttComponents.Models;
using GanttComponents.Services;

namespace GanttComponents.Components.TimelineView.Renderers;

/// <summary>
/// Static factory for creating timeline renderer instances based on zoom level.
/// Provides centralized mapping from TimelineZoomLevel to concrete renderer implementations.
/// Uses template-based approach instead of day width calculations.
/// </summary>
public static class RendererFactory
{
    /// <summary>
    /// Creates the appropriate renderer instance for the specified zoom level.
    /// Uses template-based configuration instead of explicit day width.
    /// </summary>
    /// <param name="zoomLevel">The timeline zoom level</param>
    /// <param name="logger">Universal logger service</param>
    /// <param name="dateFormatter">Date formatting helper</param>
    /// <param name="startDate">Timeline start date</param>
    /// <param name="endDate">Timeline end date</param>
    /// <param name="headerMonthHeight">Height of primary header</param>
    /// <param name="headerDayHeight">Height of secondary header</param>
    /// <param name="zoomFactor">Current zoom factor</param>
    /// <returns>Concrete renderer instance</returns>
    /// <exception cref="InvalidOperationException">Thrown for unsupported zoom levels</exception>
    public static BaseTimelineRenderer CreateRenderer(
        TimelineZoomLevel zoomLevel,
        IUniversalLogger logger,
        DateFormatHelper dateFormatter,
        DateTime startDate,
        DateTime endDate,
        int headerMonthHeight,
        int headerDayHeight,
        double zoomFactor)
    {
        return zoomLevel switch
        {
            // Template-Based Renderers - 4 Full Implementations

            // Week-Day Template: 12px per day, max 2.5x zoom
            TimelineZoomLevel.WeekDay => new WeekDayRenderer(
                logger, dateFormatter, startDate, endDate,
                zoomLevel, zoomFactor, headerMonthHeight, headerDayHeight),

            // Month-Week Template: 18px per week, max 3.0x zoom
            TimelineZoomLevel.MonthWeek => new MonthWeekRenderer(
                logger, dateFormatter, startDate, endDate,
                zoomLevel, zoomFactor, headerMonthHeight, headerDayHeight),

            // Quarter-Month Template: 30px per month, max 4.0x zoom
            TimelineZoomLevel.QuarterMonth => new QuarterMonthRenderer(
                logger, dateFormatter, startDate, endDate,
                zoomLevel, zoomFactor, headerMonthHeight, headerDayHeight),

            // Year-Quarter Template: 24px per quarter, max 4.0x zoom
            TimelineZoomLevel.YearQuarter => new YearQuarterRenderer(
                logger, dateFormatter, startDate, endDate,
                zoomLevel, zoomFactor, headerMonthHeight, headerDayHeight),

            // Unsupported levels (future implementations)
            _ => throw new InvalidOperationException($"Unsupported zoom level: {zoomLevel}. Only template-based levels are currently supported by the renderer factory.")
        };
    }
}
