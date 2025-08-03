namespace GanttComponents.Models;

/// <summary>
/// Constants for task display and rendering constraints.
/// These values ensure consistent visual behavior across all zoom levels.
/// </summary>
public static class TaskDisplayConstants
{
    /// <summary>
    /// Minimum task width in pixels before overflow handling kicks in.
    /// Tasks smaller than this will be hidden and shown in overflow dropdown.
    /// Based on usability research: 12px is minimum clickable target size.
    /// </summary>
    public const double MIN_TASK_WIDTH = 12.0;

    /// <summary>
    /// Height of task bars in pixels.
    /// Must align with row height calculations for proper positioning.
    /// </summary>
    public const double TASK_BAR_HEIGHT = 20.0;

    /// <summary>
    /// Vertical margin between task bar and row boundaries.
    /// </summary>
    public const double TASK_VERTICAL_MARGIN = 2.0;

    /// <summary>
    /// Width of overflow indicator ("...") in pixels.
    /// </summary>
    public const double OVERFLOW_INDICATOR_WIDTH = 16.0;

    /// <summary>
    /// Minimum gap between task bars when multiple tasks overlap.
    /// </summary>
    public const double TASK_SPACING = 1.0;

    /// <summary>
    /// Default zoom factor for preset-only zoom approach.
    /// All zoom control is handled via level switching, factor is always 1.0x.
    /// </summary>
    public const double DEFAULT_ZOOM_FACTOR = 1.0;

    /// <summary>
    /// Default zoom level for preset-only zoom approach.
    /// QuarterMonth24px level at 24px provides good balance for most use cases.
    /// Shows quarterly structure with monthly details at crisp integral pixel width.
    /// </summary>
    public const TimelineZoomLevel DEFAULT_ZOOM_LEVEL = TimelineZoomLevel.QuarterMonth24px;

    /// <summary>
    /// Minimum effective day width in pixels to maintain task visibility.
    /// Prevents zoom-out beyond YearQuarter base level (3.0px/day).
    /// This avoids the need for taskbar hiding strategies while maintaining
    /// reasonable strategic planning capability at decade-level views.
    /// </summary>
    public const double MIN_EFFECTIVE_DAY_WIDTH = 3.0;

    /// <summary>
    /// Check if a task width meets the minimum display requirement.
    /// </summary>
    /// <param name="width">Task width in pixels</param>
    /// <returns>True if task should be displayed, false if it should be hidden</returns>
    public static bool IsTaskWidthVisible(double width)
    {
        return width >= MIN_TASK_WIDTH;
    }

    /// <summary>
    /// Calculate the effective task width, ensuring it meets minimum requirements.
    /// </summary>
    /// <param name="calculatedWidth">Raw calculated width</param>
    /// <returns>Effective width (either calculated or minimum)</returns>
    public static double GetEffectiveTaskWidth(double calculatedWidth)
    {
        return Math.Max(calculatedWidth, MIN_TASK_WIDTH);
    }
}
