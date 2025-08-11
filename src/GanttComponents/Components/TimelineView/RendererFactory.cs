using GanttComponents.Models;
using GanttComponents.Services;

namespace GanttComponents.Components.TimelineView;

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
            // WeekDay Levels - using stub renderers temporarily
            TimelineZoomLevel.WeekDayOptimal30px => new WeekDay30pxRendererStub(
                logger, i18n, dateFormatter, startDate, endDate, dayWidth,
                headerMonthHeight, headerDayHeight, zoomLevel, zoomFactor),

            TimelineZoomLevel.WeekDayOptimal40px => new WeekDay40pxRendererStub(
                logger, i18n, dateFormatter, startDate, endDate, dayWidth,
                headerMonthHeight, headerDayHeight, zoomLevel, zoomFactor),

            TimelineZoomLevel.WeekDayOptimal50px => new WeekDay50pxRenderer(
                logger, i18n, dateFormatter, startDate, endDate, dayWidth,
                headerMonthHeight, headerDayHeight, zoomLevel, zoomFactor),

            TimelineZoomLevel.WeekDayOptimal60px => new WeekDay60pxRendererStub(
                logger, i18n, dateFormatter, startDate, endDate, dayWidth,
                headerMonthHeight, headerDayHeight, zoomLevel, zoomFactor),

            // MonthWeek Levels - using stub renderers temporarily
            TimelineZoomLevel.MonthWeekOptimal30px => new MonthWeek30pxRendererStub(
                logger, i18n, dateFormatter, startDate, endDate, dayWidth,
                headerMonthHeight, headerDayHeight, zoomLevel, zoomFactor),

            TimelineZoomLevel.MonthWeekOptimal40px => new MonthWeek40pxRendererStub(
                logger, i18n, dateFormatter, startDate, endDate, dayWidth,
                headerMonthHeight, headerDayHeight, zoomLevel, zoomFactor),

            TimelineZoomLevel.MonthWeekOptimal50px => new MonthWeek50pxRendererStub(
                logger, i18n, dateFormatter, startDate, endDate, dayWidth,
                headerMonthHeight, headerDayHeight, zoomLevel, zoomFactor),

            TimelineZoomLevel.MonthWeekOptimal60px => new MonthWeek60pxRendererStub(
                logger, i18n, dateFormatter, startDate, endDate, dayWidth,
                headerMonthHeight, headerDayHeight, zoomLevel, zoomFactor),

            // Unsupported levels (future implementations)
            _ => throw new InvalidOperationException($"Unsupported zoom level: {zoomLevel}. Only WeekDay and MonthWeek optimal levels are currently supported by the renderer factory.")
        };
    }
}

// === TEMPORARY STUB RENDERERS ===
// These are minimal implementations that will be replaced with full renderers in later commits

/// <summary>Stub renderer for WeekDay 30px level - temporary implementation</summary>
internal class WeekDay30pxRendererStub : BaseTimelineRenderer
{
    public WeekDay30pxRendererStub(IUniversalLogger logger, IGanttI18N i18n, DateFormatHelper dateFormatter,
        DateTime startDate, DateTime endDate, double dayWidth, int headerMonthHeight, int headerDayHeight,
        TimelineZoomLevel zoomLevel, double zoomFactor)
        : base(logger, i18n, dateFormatter, startDate, endDate, dayWidth, headerMonthHeight, headerDayHeight, zoomLevel, zoomFactor) { }

    protected override (DateTime expandedStart, DateTime expandedEnd) CalculateHeaderBoundaries() => (StartDate, EndDate);
    protected override string RenderPrimaryHeader() => "<!-- WeekDay 30px Primary Header - Stub -->";
    protected override string RenderSecondaryHeader() => "<!-- WeekDay 30px Secondary Header - Stub -->";
    protected override string GetRendererDescription() => "WeekDay 30px (Stub)";
    protected override string GetCSSClass() => "weekday-30px";
}

/// <summary>Stub renderer for WeekDay 40px level - temporary implementation</summary>
internal class WeekDay40pxRendererStub : BaseTimelineRenderer
{
    public WeekDay40pxRendererStub(IUniversalLogger logger, IGanttI18N i18n, DateFormatHelper dateFormatter,
        DateTime startDate, DateTime endDate, double dayWidth, int headerMonthHeight, int headerDayHeight,
        TimelineZoomLevel zoomLevel, double zoomFactor)
        : base(logger, i18n, dateFormatter, startDate, endDate, dayWidth, headerMonthHeight, headerDayHeight, zoomLevel, zoomFactor) { }

    protected override (DateTime expandedStart, DateTime expandedEnd) CalculateHeaderBoundaries() => (StartDate, EndDate);
    protected override string RenderPrimaryHeader() => "<!-- WeekDay 40px Primary Header - Stub -->";
    protected override string RenderSecondaryHeader() => "<!-- WeekDay 40px Secondary Header - Stub -->";
    protected override string GetRendererDescription() => "WeekDay 40px (Stub)";
    protected override string GetCSSClass() => "weekday-40px";
}

/// <summary>Stub renderer for WeekDay 50px level - temporary implementation</summary>
internal class WeekDay50pxRendererStub : BaseTimelineRenderer
{
    public WeekDay50pxRendererStub(IUniversalLogger logger, IGanttI18N i18n, DateFormatHelper dateFormatter,
        DateTime startDate, DateTime endDate, double dayWidth, int headerMonthHeight, int headerDayHeight,
        TimelineZoomLevel zoomLevel, double zoomFactor)
        : base(logger, i18n, dateFormatter, startDate, endDate, dayWidth, headerMonthHeight, headerDayHeight, zoomLevel, zoomFactor) { }

    protected override (DateTime expandedStart, DateTime expandedEnd) CalculateHeaderBoundaries() => (StartDate, EndDate);
    protected override string RenderPrimaryHeader() => "<!-- WeekDay 50px Primary Header - Stub -->";
    protected override string RenderSecondaryHeader() => "<!-- WeekDay 50px Secondary Header - Stub -->";
    protected override string GetRendererDescription() => "WeekDay 50px (Stub)";
    protected override string GetCSSClass() => "weekday-50px";
}

/// <summary>Stub renderer for WeekDay 60px level - temporary implementation</summary>
internal class WeekDay60pxRendererStub : BaseTimelineRenderer
{
    public WeekDay60pxRendererStub(IUniversalLogger logger, IGanttI18N i18n, DateFormatHelper dateFormatter,
        DateTime startDate, DateTime endDate, double dayWidth, int headerMonthHeight, int headerDayHeight,
        TimelineZoomLevel zoomLevel, double zoomFactor)
        : base(logger, i18n, dateFormatter, startDate, endDate, dayWidth, headerMonthHeight, headerDayHeight, zoomLevel, zoomFactor) { }

    protected override (DateTime expandedStart, DateTime expandedEnd) CalculateHeaderBoundaries() => (StartDate, EndDate);
    protected override string RenderPrimaryHeader() => "<!-- WeekDay 60px Primary Header - Stub -->";
    protected override string RenderSecondaryHeader() => "<!-- WeekDay 60px Secondary Header - Stub -->";
    protected override string GetRendererDescription() => "WeekDay 60px (Stub)";
    protected override string GetCSSClass() => "weekday-60px";
}

/// <summary>Stub renderer for MonthWeek 30px level - temporary implementation</summary>
internal class MonthWeek30pxRendererStub : BaseTimelineRenderer
{
    public MonthWeek30pxRendererStub(IUniversalLogger logger, IGanttI18N i18n, DateFormatHelper dateFormatter,
        DateTime startDate, DateTime endDate, double dayWidth, int headerMonthHeight, int headerDayHeight,
        TimelineZoomLevel zoomLevel, double zoomFactor)
        : base(logger, i18n, dateFormatter, startDate, endDate, dayWidth, headerMonthHeight, headerDayHeight, zoomLevel, zoomFactor) { }

    protected override (DateTime expandedStart, DateTime expandedEnd) CalculateHeaderBoundaries() => (StartDate, EndDate);
    protected override string RenderPrimaryHeader() => "<!-- MonthWeek 30px Primary Header - Stub -->";
    protected override string RenderSecondaryHeader() => "<!-- MonthWeek 30px Secondary Header - Stub -->";
    protected override string GetRendererDescription() => "MonthWeek 30px (Stub)";
    protected override string GetCSSClass() => "monthweek-30px";
}

/// <summary>Stub renderer for MonthWeek 40px level - temporary implementation</summary>
internal class MonthWeek40pxRendererStub : BaseTimelineRenderer
{
    public MonthWeek40pxRendererStub(IUniversalLogger logger, IGanttI18N i18n, DateFormatHelper dateFormatter,
        DateTime startDate, DateTime endDate, double dayWidth, int headerMonthHeight, int headerDayHeight,
        TimelineZoomLevel zoomLevel, double zoomFactor)
        : base(logger, i18n, dateFormatter, startDate, endDate, dayWidth, headerMonthHeight, headerDayHeight, zoomLevel, zoomFactor) { }

    protected override (DateTime expandedStart, DateTime expandedEnd) CalculateHeaderBoundaries() => (StartDate, EndDate);
    protected override string RenderPrimaryHeader() => "<!-- MonthWeek 40px Primary Header - Stub -->";
    protected override string RenderSecondaryHeader() => "<!-- MonthWeek 40px Secondary Header - Stub -->";
    protected override string GetRendererDescription() => "MonthWeek 40px (Stub)";
    protected override string GetCSSClass() => "monthweek-40px";
}

/// <summary>Stub renderer for MonthWeek 50px level - temporary implementation</summary>
internal class MonthWeek50pxRendererStub : BaseTimelineRenderer
{
    public MonthWeek50pxRendererStub(IUniversalLogger logger, IGanttI18N i18n, DateFormatHelper dateFormatter,
        DateTime startDate, DateTime endDate, double dayWidth, int headerMonthHeight, int headerDayHeight,
        TimelineZoomLevel zoomLevel, double zoomFactor)
        : base(logger, i18n, dateFormatter, startDate, endDate, dayWidth, headerMonthHeight, headerDayHeight, zoomLevel, zoomFactor) { }

    protected override (DateTime expandedStart, DateTime expandedEnd) CalculateHeaderBoundaries() => (StartDate, EndDate);
    protected override string RenderPrimaryHeader() => "<!-- MonthWeek 50px Primary Header - Stub -->";
    protected override string RenderSecondaryHeader() => "<!-- MonthWeek 50px Secondary Header - Stub -->";
    protected override string GetRendererDescription() => "MonthWeek 50px (Stub)";
    protected override string GetCSSClass() => "monthweek-50px";
}

/// <summary>Stub renderer for MonthWeek 60px level - temporary implementation</summary>
internal class MonthWeek60pxRendererStub : BaseTimelineRenderer
{
    public MonthWeek60pxRendererStub(IUniversalLogger logger, IGanttI18N i18n, DateFormatHelper dateFormatter,
        DateTime startDate, DateTime endDate, double dayWidth, int headerMonthHeight, int headerDayHeight,
        TimelineZoomLevel zoomLevel, double zoomFactor)
        : base(logger, i18n, dateFormatter, startDate, endDate, dayWidth, headerMonthHeight, headerDayHeight, zoomLevel, zoomFactor) { }

    protected override (DateTime expandedStart, DateTime expandedEnd) CalculateHeaderBoundaries() => (StartDate, EndDate);
    protected override string RenderPrimaryHeader() => "<!-- MonthWeek 60px Primary Header - Stub -->";
    protected override string RenderSecondaryHeader() => "<!-- MonthWeek 60px Secondary Header - Stub -->";
    protected override string GetRendererDescription() => "MonthWeek 60px (Stub)";
    protected override string GetCSSClass() => "monthweek-60px";
}
