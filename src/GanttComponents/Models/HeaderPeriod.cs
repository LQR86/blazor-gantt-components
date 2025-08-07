using GanttComponents.Services;

namespace GanttComponents.Models;

/// <summary>
/// Represents a single period in the timeline header (e.g., a month, quarter, or year).
/// This model enables data-driven header rendering instead of inline Razor logic.
/// </summary>
public class HeaderPeriod
{
    /// <summary>
    /// Start date of this header period
    /// </summary>
    public DateTime Start { get; set; }

    /// <summary>
    /// End date of this header period
    /// </summary>
    public DateTime End { get; set; }

    /// <summary>
    /// Width in pixels for this period based on EffectiveDayWidth calculation
    /// </summary>
    public double Width { get; set; }

    /// <summary>
    /// Display label for this period (formatted using DateFormatHelper)
    /// </summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// Header level (Primary or Secondary) for styling and layout
    /// </summary>
    public HeaderLevel Level { get; set; }

    /// <summary>
    /// X-position offset for absolute positioning (calculated during generation)
    /// </summary>
    public double XPosition { get; set; }
}

/// <summary>
/// Header level enumeration for clear separation of primary and secondary headers
/// </summary>
public enum HeaderLevel
{
    Primary,    // Top level (quarters, years, decades)
    Secondary   // Bottom level (months, quarters, years)
}

/// <summary>
/// Complete result of header period generation including collapse state
/// </summary>
public class TimelineHeaderResult
{
    /// <summary>
    /// Primary header periods (top row)
    /// </summary>
    public List<HeaderPeriod> PrimaryPeriods { get; set; } = new();

    /// <summary>
    /// Secondary header periods (bottom row)
    /// </summary>
    public List<HeaderPeriod> SecondaryPeriods { get; set; } = new();

    /// <summary>
    /// Whether headers should collapse based on available space
    /// </summary>
    public bool ShouldCollapse { get; set; }

    /// <summary>
    /// Header configuration used for generation (for debugging/validation)
    /// </summary>
    public TimelineHeaderConfiguration? HeaderConfig { get; set; }
}
