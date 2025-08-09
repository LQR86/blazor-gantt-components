using GanttComponents.Models;

namespace GanttComponents.Services;

/// <summary>
/// Implementation of timeline tooltip service that calculates viewport edge tooltips
/// for hidden timeline periods. Designed for reusability, performance, and testability.
/// </summary>
public class TimelineTooltipService : ITimelineTooltipService
{
    private readonly ITimelineHeaderService _headerService;
    private readonly IUniversalLogger _logger;

    public TimelineTooltipService(ITimelineHeaderService headerService, IUniversalLogger logger)
    {
        _headerService = headerService ?? throw new ArgumentNullException(nameof(headerService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Calculate tooltip data for both left and right edges in a single operation.
    /// This avoids duplicate header generation and improves performance.
    /// </summary>
    public TimelineTooltipResult CalculateTooltips(TimelineTooltipRequest request)
    {
        if (request == null)
            return new TimelineTooltipResult();

        try
        {
            // Single header generation call for both tooltips
            var headerResult = _headerService.GenerateHeaderPeriods(
                request.StartDate,
                request.EndDate,
                request.EffectiveDayWidth,
                request.ZoomLevel,
                _logger);

            var periods = request.Options.UsePrimaryPeriods
                ? headerResult.PrimaryPeriods
                : headerResult.SecondaryPeriods;

            return new TimelineTooltipResult
            {
                LeftTooltip = CalculateLeftTooltip(request, periods),
                RightTooltip = CalculateRightTooltip(request, periods)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error calculating timeline tooltips: {ex.Message}");
            return new TimelineTooltipResult();
        }
    }

    /// <summary>
    /// Calculate left edge tooltip for partially hidden period.
    /// </summary>
    private string CalculateLeftTooltip(TimelineTooltipRequest request, List<HeaderPeriod> periods)
    {
        if (request.ViewportScrollLeft <= 0)
            return string.Empty;

        var leftEdgeX = request.ViewportScrollLeft;
        var partiallyHiddenPeriod = FindPartiallyHiddenPeriod(periods, leftEdgeX, isLeftEdge: true);

        if (partiallyHiddenPeriod == null)
            return string.Empty;

        if (ShouldShowTooltip(partiallyHiddenPeriod, leftEdgeX, request.Options.HiddenThreshold, isLeftEdge: true))
        {
            return $"{request.Options.LeftArrow}{partiallyHiddenPeriod.Label}";
        }

        return string.Empty;
    }

    /// <summary>
    /// Calculate right edge tooltip for partially hidden period.
    /// </summary>
    private string CalculateRightTooltip(TimelineTooltipRequest request, List<HeaderPeriod> periods)
    {
        var totalWidth = (request.EndDate - request.StartDate).Days * request.EffectiveDayWidth;
        var rightEdgeX = request.ViewportScrollLeft + request.ViewportWidth;

        if (rightEdgeX >= totalWidth)
            return string.Empty;

        var partiallyHiddenPeriod = FindPartiallyHiddenPeriod(periods, rightEdgeX, isLeftEdge: false);

        if (partiallyHiddenPeriod == null)
            return string.Empty;

        if (ShouldShowTooltip(partiallyHiddenPeriod, rightEdgeX, request.Options.HiddenThreshold, isLeftEdge: false))
        {
            return $"{partiallyHiddenPeriod.Label}{request.Options.RightArrow}";
        }

        return string.Empty;
    }

    /// <summary>
    /// Find a period that's partially visible at the specified edge position.
    /// </summary>
    private HeaderPeriod? FindPartiallyHiddenPeriod(List<HeaderPeriod> periods, double edgeX, bool isLeftEdge)
    {
        return periods.FirstOrDefault(p =>
            p.XPosition < edgeX && (p.XPosition + p.Width) > edgeX);
    }

    /// <summary>
    /// Determine if tooltip should be shown based on hidden percentage threshold.
    /// </summary>
    private bool ShouldShowTooltip(HeaderPeriod period, double edgeX, double hiddenThreshold, bool isLeftEdge)
    {
        double hiddenWidth, visibleWidth;

        if (isLeftEdge)
        {
            hiddenWidth = edgeX - period.XPosition;
            visibleWidth = (period.XPosition + period.Width) - edgeX;
        }
        else
        {
            visibleWidth = edgeX - period.XPosition;
            hiddenWidth = (period.XPosition + period.Width) - edgeX;
        }

        // Show tooltip when hidden portion exceeds threshold AND period is still partially visible
        return hiddenWidth > (period.Width * hiddenThreshold) && visibleWidth > 0;
    }
}
