using GanttComponents.Models;
using GanttComponents.Services;

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
/// Enhanced with I18N integration for localized header content.
/// </summary>
public class TimelineHeaderService : ITimelineHeaderService
{
    private readonly DateFormatHelper _dateFormatter;
    private readonly IGanttI18N _i18n;

    public TimelineHeaderService(DateFormatHelper dateFormatter, IGanttI18N i18n)
    {
        _dateFormatter = dateFormatter;
        _i18n = i18n;
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
    /// Format period label using I18N-enhanced logic for localized headers.
    /// Maintains backward compatibility with existing DateFormatHelper while adding I18N support.
    /// </summary>
    private string FormatPeriodLabel(DateTime date, string format)
    {
        // Phase 2: Enhanced with I18N-aware format selection while maintaining DateFormatHelper compatibility
        var i18nFormat = GetI18NEnhancedFormat(format);
        return _dateFormatter.FormatTimelineHeader(date, i18nFormat);
    }

    /// <summary>
    /// Get I18N-enhanced format string for temporal units.
    /// Provides foundation for future floating headers and information-dense headers.
    /// Maps generic formats to culture-aware format keys.
    /// </summary>
    private string GetI18NEnhancedFormat(string originalFormat)
    {
        // Phase 2: Maintain backward compatibility - disable enhanced format selection for now
        // The timeline.header.* keys are available but need proper integration with DateFormatHelper
        // Future enhancement: Map to culture-specific timeline header formats when properly integrated

        return originalFormat; // Always use original format for backward compatibility

        /* Future enhancement when DateFormatHelper integration is complete:
        return originalFormat switch
        {
            // Timeline-specific header formats (future enhancement for dense headers)
            var f when f.Contains("quarter") => _i18n.HasTranslation("timeline.header.quarter-format")
                ? "timeline.header.quarter-format"
                : f,
            var f when f.Contains("year") => _i18n.HasTranslation("timeline.header.year-format")
                ? "timeline.header.year-format"
                : f,
            var f when f.Contains("month") => _i18n.HasTranslation("timeline.header.month-format")
                ? "timeline.header.month-format"
                : f,
            var f when f.Contains("week") => _i18n.HasTranslation("timeline.header.week-format")
                ? "timeline.header.week-format"
                : f,
            var f when f.Contains("day") => _i18n.HasTranslation("timeline.header.day-format")
                ? "timeline.header.day-format"
                : f,
            _ => originalFormat // Fallback to original format for full backward compatibility
        };
        */
    }
}
