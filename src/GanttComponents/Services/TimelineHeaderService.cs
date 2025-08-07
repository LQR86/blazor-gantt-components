using GanttComponents.Models;

namespace GanttComponents.Services;

/// <summary>
/// Service for generating timeline header periods with business logic extracted from TimelineView.
/// Designed to produce identical results to the current inline header generation logic.
/// </summary>
public interface ITimelineHeaderService
{
    /// <summary>
    /// Generate header periods for primary and secondary levels based on configuration.
    /// This method will replace the inline @while loops in TimelineView.razor.
    /// </summary>
    TimelineHeaderResult GenerateHeaderPeriods(
        DateTime startDate,
        DateTime endDate,
        double effectiveDayWidth,
        TimelineZoomLevel zoomLevel,
        IUniversalLogger? logger = null);

    /// <summary>
    /// Determine if headers should collapse based on available space and day width.
    /// Replaces the shouldCollapse logic from TimelineView.
    /// </summary>
    bool ShouldCollapseHeaders(
        TimelineHeaderConfiguration headerConfig,
        double effectiveDayWidth,
        int timeSpanDays);
}

/// <summary>
/// Implementation of timeline header service with extracted business logic.
/// Phase 2: Full implementation with identical logic to current TimelineView.
/// </summary>
public class TimelineHeaderService : ITimelineHeaderService
{
    private readonly DateFormatHelper _dateFormatter;

    public TimelineHeaderService(DateFormatHelper dateFormatter)
    {
        _dateFormatter = dateFormatter;
    }
    public TimelineHeaderResult GenerateHeaderPeriods(
        DateTime startDate,
        DateTime endDate,
        double effectiveDayWidth,
        TimelineZoomLevel zoomLevel,
        IUniversalLogger? logger = null)
    {
        // Phase 2: Full implementation with logic extracted from TimelineView.razor
        logger?.LogOperation("TimelineHeaderService", "GenerateHeaderPeriods", new
        {
            startDate,
            endDate,
            effectiveDayWidth,
            zoomLevel
        });

        // Get header configuration using exact same logic as TimelineView
        var headerConfig = TimelineHeaderAdapter.GetHeaderConfigurationFromTemplate(zoomLevel, logger);
        var timeSpanDays = (endDate - startDate).Days + 1;
        var shouldCollapse = TimelineHeaderAdapter.ShouldCollapseHeaders(headerConfig, effectiveDayWidth, timeSpanDays);

        var result = new TimelineHeaderResult
        {
            HeaderConfig = headerConfig,
            ShouldCollapse = shouldCollapse,
            PrimaryPeriods = new List<HeaderPeriod>(),
            SecondaryPeriods = new List<HeaderPeriod>()
        };

        // Generate primary periods (extracted from TimelineView @while loop)
        if (headerConfig.ShowPrimary && !shouldCollapse)
        {
            result.PrimaryPeriods = GeneratePrimaryPeriods(startDate, endDate, effectiveDayWidth, headerConfig, logger);
        }

        // Generate secondary periods (extracted from TimelineView @while loop)
        result.SecondaryPeriods = GenerateSecondaryPeriods(startDate, endDate, effectiveDayWidth, headerConfig, logger);

        logger?.LogOperation("TimelineHeaderService", "Generated header periods", new
        {
            PrimaryCount = result.PrimaryPeriods.Count,
            SecondaryCount = result.SecondaryPeriods.Count,
            ShouldCollapse = shouldCollapse
        });

        return result;
    }

    public bool ShouldCollapseHeaders(
        TimelineHeaderConfiguration headerConfig,
        double effectiveDayWidth,
        int timeSpanDays)
    {
        // Phase 2: Implementation using existing TimelineHeaderAdapter logic
        return TimelineHeaderAdapter.ShouldCollapseHeaders(headerConfig, effectiveDayWidth, timeSpanDays);
    }

    /// <summary>
    /// Generate primary header periods - extracted from TimelineView @while loop logic
    /// </summary>
    private List<HeaderPeriod> GeneratePrimaryPeriods(
        DateTime startDate,
        DateTime endDate,
        double effectiveDayWidth,
        TimelineHeaderConfiguration headerConfig,
        IUniversalLogger? logger)
    {
        var periods = new List<HeaderPeriod>();
        var primaryStart = TimelineHeaderAdapter.GetPeriodStart(startDate, headerConfig.PrimaryUnit, logger);
        var increment = TimelineHeaderAdapter.GetDateIncrement(headerConfig.PrimaryUnit);
        var current = primaryStart;
        var xPosition = 0.0;

        // Exact logic from TimelineView primary @while loop
        while (current <= endDate)
        {
            var periodWidth = TimelineHeaderAdapter.GetPeriodWidth(current, headerConfig.PrimaryUnit, effectiveDayWidth);
            var nextPeriod = increment(current);

            // Adjust width if period extends beyond timeline
            if (nextPeriod > endDate.AddDays(1))
            {
                var visibleDays = (endDate.AddDays(1) - current).Days;
                periodWidth = visibleDays * effectiveDayWidth;
            }

            // Skip if period starts after timeline end
            if (current > endDate)
            {
                break;
            }

            // Create period with formatting identical to TimelineView
            var period = new HeaderPeriod
            {
                Start = current,
                End = nextPeriod.AddDays(-1), // End of period
                Width = periodWidth,
                Label = FormatPeriodLabel(current, headerConfig.PrimaryFormat),
                Level = HeaderLevel.Primary,
                XPosition = xPosition
            };

            periods.Add(period);
            xPosition += periodWidth;
            current = nextPeriod;
        }

        return periods;
    }

    /// <summary>
    /// Generate secondary header periods - extracted from TimelineView @while loop logic
    /// </summary>
    private List<HeaderPeriod> GenerateSecondaryPeriods(
        DateTime startDate,
        DateTime endDate,
        double effectiveDayWidth,
        TimelineHeaderConfiguration headerConfig,
        IUniversalLogger? logger)
    {
        var periods = new List<HeaderPeriod>();
        var secondaryStart = TimelineHeaderAdapter.GetPeriodStart(startDate, headerConfig.SecondaryUnit, logger);
        var secondaryIncrement = TimelineHeaderAdapter.GetDateIncrement(headerConfig.SecondaryUnit);
        var secondaryCurrent = secondaryStart;
        var xPosition = 0.0;

        // Exact logic from TimelineView secondary @while loop
        while (secondaryCurrent <= endDate)
        {
            var secondaryWidth = TimelineHeaderAdapter.GetPeriodWidth(secondaryCurrent, headerConfig.SecondaryUnit, effectiveDayWidth);
            var nextSecondary = secondaryIncrement(secondaryCurrent);

            // Adjust width if period extends beyond timeline
            if (nextSecondary > endDate.AddDays(1))
            {
                var visibleDays = (endDate.AddDays(1) - secondaryCurrent).Days;
                secondaryWidth = visibleDays * effectiveDayWidth;
            }

            // Skip if period starts after timeline end
            if (secondaryCurrent > endDate)
            {
                break;
            }

            // Create period with formatting identical to TimelineView
            var period = new HeaderPeriod
            {
                Start = secondaryCurrent,
                End = nextSecondary.AddDays(-1), // End of period
                Width = secondaryWidth,
                Label = FormatPeriodLabel(secondaryCurrent, headerConfig.SecondaryFormat),
                Level = HeaderLevel.Secondary,
                XPosition = xPosition
            };

            periods.Add(period);
            xPosition += secondaryWidth;
            secondaryCurrent = nextSecondary;
        }

        return periods;
    }

    /// <summary>
    /// Format period label using exact same logic as TimelineView DateFormatter
    /// </summary>
    private string FormatPeriodLabel(DateTime date, string format)
    {
        // Phase 2: Use DateFormatHelper for identical formatting to TimelineView
        return _dateFormatter.FormatTimelineHeader(date, format);
    }
}
