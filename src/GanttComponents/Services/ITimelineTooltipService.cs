using GanttComponents.Models;

namespace GanttComponents.Services;

/// <summary>
/// Service for calculating timeline viewport tooltips showing hidden periods.
/// Provides reusable, configurable tooltip logic that can be used across components.
/// </summary>
public interface ITimelineTooltipService
{
    /// <summary>
    /// Calculate tooltip data for both left and right edges of the viewport.
    /// Returns both tooltips in a single call to avoid duplicate header calculations.
    /// </summary>
    TimelineTooltipResult CalculateTooltips(TimelineTooltipRequest request);
}

/// <summary>
/// Request object for tooltip calculation containing all necessary viewport and timeline data.
/// </summary>
public class TimelineTooltipRequest
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public double EffectiveDayWidth { get; set; }
    public TimelineZoomLevel ZoomLevel { get; set; }
    public double ViewportScrollLeft { get; set; }
    public double ViewportWidth { get; set; }
    public TimelineTooltipOptions Options { get; set; } = new();
}

/// <summary>
/// Configuration options for tooltip behavior and appearance.
/// </summary>
public class TimelineTooltipOptions
{
    /// <summary>
    /// Minimum percentage of period that must be hidden before showing tooltip.
    /// Default: 0.5 (50%)
    /// </summary>
    public double HiddenThreshold { get; set; } = 0.5;

    /// <summary>
    /// Arrow symbols for left/right tooltips.
    /// </summary>
    public string LeftArrow { get; set; } = "←";
    public string RightArrow { get; set; } = "→";

    /// <summary>
    /// Whether to use primary or secondary header periods for tooltips.
    /// </summary>
    public bool UsePrimaryPeriods { get; set; } = true;
}

/// <summary>
/// Result containing calculated tooltip content for both edges.
/// </summary>
public class TimelineTooltipResult
{
    public string LeftTooltip { get; set; } = string.Empty;
    public string RightTooltip { get; set; } = string.Empty;
    public bool HasLeftTooltip => !string.IsNullOrEmpty(LeftTooltip);
    public bool HasRightTooltip => !string.IsNullOrEmpty(RightTooltip);
}
