using GanttComponents.Models;
using GanttComponents.Services;

namespace GanttComponents.Components.TimelineView.Renderers;

/// <summary>
/// Static factory for creating timeline renderer instances based on zoom level.
/// Provides centralized mapping from TimelineZoomLevel to concrete renderer implementations.
/// </summary>
public static class RendererFactory
{
    /// <summary>
    /// Creates the appropriate renderer instance for the specified zoom level.
    /// </summary>
    /// <param name="zoomLevel">The timeline zoom level</param>
    /// <param name="logger">Universal logger service</param>
    /// <param name="i18n">Internationalization service</param>
    /// <param name="dateFormatter">Date formatting helper</param>
    /// <param name="startDate">Timeline start date</param>
    /// <param name="endDate">Timeline end date</param>
    /// <param name="dayWidth">Width of each day in pixels</param>
    /// <param name="headerMonthHeight">Height of primary header</param>
    /// <param name="headerDayHeight">Height of secondary header</param>
    /// <param name="zoomFactor">Current zoom factor</param>
    /// <returns>Concrete renderer instance</returns>
    /// <exception cref="InvalidOperationException">Thrown for unsupported zoom levels</exception>
    public static BaseTimelineRenderer CreateRenderer(
        TimelineZoomLevel zoomLevel,
        IUniversalLogger logger,
        IGanttI18N i18n,
        DateFormatHelper dateFormatter,
        DateTime startDate,
        DateTime endDate,
        double dayWidth,
        int headerMonthHeight,
        int headerDayHeight,
        double zoomFactor)
    {
        return zoomLevel switch
        {
            // ABC Composition - 4 Full Implementations Only

            // WeekDay50px - ABC implementation (hardcoded 50px day width)
            TimelineZoomLevel.WeekDayOptimal50px => new WeekDay50pxRenderer(
                logger, i18n, dateFormatter, startDate, endDate,
                headerMonthHeight, headerDayHeight, zoomLevel, zoomFactor),

            // MonthWeek50px - ABC implementation (takes dayWidth parameter)
            TimelineZoomLevel.MonthWeekOptimal50px => new MonthWeek50pxRenderer(
                logger, i18n, dateFormatter, startDate, endDate, 8.0,
                headerMonthHeight, headerDayHeight, zoomLevel, zoomFactor),

            // QuarterMonth60px - ABC implementation (hardcoded 2.0px day width)
            TimelineZoomLevel.QuarterMonthOptimal60px => new QuarterMonth60pxRenderer(
                logger, i18n, dateFormatter, startDate, endDate,
                headerMonthHeight, headerDayHeight, zoomLevel, zoomFactor),

            // YearQuarter70px - ABC implementation (using 90px renderer with hardcoded 1.0px day width)
            TimelineZoomLevel.YearQuarterOptimal70px => new YearQuarter90pxRenderer(
                logger, i18n, dateFormatter, startDate, endDate,
                headerMonthHeight, headerDayHeight, zoomLevel, zoomFactor),

            // Unsupported levels (future implementations)
            _ => throw new InvalidOperationException($"Unsupported zoom level: {zoomLevel}. Only ABC composition levels are currently supported by the renderer factory.")
        };
    }
}
