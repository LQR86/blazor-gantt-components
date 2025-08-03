using GanttComponents.Models;

namespace GanttComponents.Services;

/// <summary>
/// Service for managing timeline zoom level configurations.
/// Provides centralized access to zoom settings and calculations.
/// </summary>
public class TimelineZoomService
{
    private static readonly Dictionary<TimelineZoomLevel, ZoomLevelConfiguration> _configurations =
        new Dictionary<TimelineZoomLevel, ZoomLevelConfiguration>
        {
            // Week→Day Pattern (68px, 97px): Top tier shows week range with year, bottom tier shows day numbers
            [TimelineZoomLevel.WeekDay97px] = ZoomLevelConfiguration.Create(
                TimelineZoomLevel.WeekDay97px,
                97.0, // Integral pixel width for crisp rendering
                "ZoomLevel.WeekDay97px",
                "ZoomLevel.WeekDay97px.Description"
            ),
            [TimelineZoomLevel.WeekDay68px] = ZoomLevelConfiguration.Create(
                TimelineZoomLevel.WeekDay68px,
                68.0, // Integral pixel width for crisp rendering
                "ZoomLevel.WeekDay68px",
                "ZoomLevel.WeekDay68px.Description"
            ),

            // Month→Day Pattern (34px, 48px): Top tier shows month with year, bottom tier shows day numbers
            [TimelineZoomLevel.MonthDay48px] = ZoomLevelConfiguration.Create(
                TimelineZoomLevel.MonthDay48px,
                48.0, // Integral pixel width for crisp rendering
                "ZoomLevel.MonthDay48px",
                "ZoomLevel.MonthDay48px.Description"
            ),
            [TimelineZoomLevel.MonthDay34px] = ZoomLevelConfiguration.Create(
                TimelineZoomLevel.MonthDay34px,
                34.0, // Integral pixel width for crisp rendering
                "ZoomLevel.MonthDay34px",
                "ZoomLevel.MonthDay34px.Description"
            ),

            // Quarter→Month Pattern (17px, 24px): Top tier shows quarter with year, bottom tier shows month names
            [TimelineZoomLevel.QuarterMonth24px] = ZoomLevelConfiguration.Create(
                TimelineZoomLevel.QuarterMonth24px,
                24.0, // Integral pixel width for crisp rendering
                "ZoomLevel.QuarterMonth24px",
                "ZoomLevel.QuarterMonth24px.Description"
            ),
            [TimelineZoomLevel.QuarterMonth17px] = ZoomLevelConfiguration.Create(
                TimelineZoomLevel.QuarterMonth17px,
                17.0, // Integral pixel width for crisp rendering
                "ZoomLevel.QuarterMonth17px",
                "ZoomLevel.QuarterMonth17px.Description"
            ),

            // Month-only Pattern (8px, 12px): Single tier showing year and month names only
            [TimelineZoomLevel.Month12px] = ZoomLevelConfiguration.Create(
                TimelineZoomLevel.Month12px,
                12.0, // Integral pixel width for crisp rendering
                "ZoomLevel.Month12px",
                "ZoomLevel.Month12px.Description"
            ),
            [TimelineZoomLevel.Month8px] = ZoomLevelConfiguration.Create(
                TimelineZoomLevel.Month8px,
                8.0, // Integral pixel width for crisp rendering
                "ZoomLevel.Month8px",
                "ZoomLevel.Month8px.Description"
            ),

            // Year→Quarter Pattern (3px, 4px, 6px): Top tier shows year, bottom tier shows quarters
            [TimelineZoomLevel.YearQuarter6px] = ZoomLevelConfiguration.Create(
                TimelineZoomLevel.YearQuarter6px,
                6.0, // Integral pixel width for crisp rendering
                "ZoomLevel.YearQuarter6px",
                "ZoomLevel.YearQuarter6px.Description"
            ),
            [TimelineZoomLevel.YearQuarter4px] = ZoomLevelConfiguration.Create(
                TimelineZoomLevel.YearQuarter4px,
                4.0, // Integral pixel width for crisp rendering
                "ZoomLevel.YearQuarter4px",
                "ZoomLevel.YearQuarter4px.Description"
            ),
            [TimelineZoomLevel.YearQuarter3px] = ZoomLevelConfiguration.Create(
                TimelineZoomLevel.YearQuarter3px,
                3.0, // Minimum integral pixel width for crisp rendering
                "ZoomLevel.YearQuarter3px",
                "ZoomLevel.YearQuarter3px.Description"
            )
        };

    /// <summary>
    /// Get configuration for a specific zoom level.
    /// </summary>
    /// <param name="level">The zoom level</param>
    /// <returns>Configuration for the zoom level</returns>
    public static ZoomLevelConfiguration GetConfiguration(TimelineZoomLevel level)
    {
        return _configurations.TryGetValue(level, out var config)
            ? config
            : _configurations[TaskDisplayConstants.DEFAULT_ZOOM_LEVEL];
    }

    /// <summary>
    /// Get all available zoom levels with their configurations.
    /// </summary>
    /// <returns>All zoom level configurations</returns>
    public static IReadOnlyDictionary<TimelineZoomLevel, ZoomLevelConfiguration> GetAllConfigurations()
    {
        return _configurations;
    }

    /// <summary>
    /// Calculate effective day width for a zoom level and factor.
    /// </summary>
    /// <param name="level">The zoom level</param>
    /// <param name="zoomFactor">Zoom multiplier (preset-only: always 1.0x)</param>
    /// <returns>Effective day width in pixels</returns>
    public static double CalculateEffectiveDayWidth(TimelineZoomLevel level, double zoomFactor)
    {
        var config = GetConfiguration(level);
        return config.GetEffectiveDayWidth(zoomFactor);
    }

    /// <summary>
    /// Validate a zoom factor for a specific level.
    /// With preset-only zoom levels, this ensures factors stay within 1.0x bounds.
    /// Also enforces global 3px minimum day width constraint.
    /// </summary>
    /// <param name="level">The zoom level</param>
    /// <param name="zoomFactor">Zoom factor to validate (should always be 1.0 for preset levels)</param>
    /// <returns>Clamped zoom factor (1.0 for preset-only approach)</returns>
    public static double ClampZoomFactor(TimelineZoomLevel level, double zoomFactor)
    {
        var config = GetConfiguration(level);

        // For preset-only approach, always use 1.0x factor
        // All zoom control is handled via level switching
        return Math.Max(config.MinZoomFactor, Math.Min(config.MaxZoomFactor, zoomFactor));
    }

    /// <summary>
    /// Get the default zoom settings for preset-only approach.
    /// Current behavior: MonthDay @ 1.0x = 25px (preset-only default).
    /// </summary>
    /// <returns>Tuple of (level, factor) for preset-only behavior</returns>
    public static (TimelineZoomLevel level, double factor) GetDefaultZoomSettings()
    {
        return (TaskDisplayConstants.DEFAULT_ZOOM_LEVEL, TaskDisplayConstants.DEFAULT_ZOOM_FACTOR);
    }

    /// <summary>
    /// Check if zoom factor can be decreased further without violating minimum day width.
    /// In preset-only approach, this is always false since factors are fixed at 1.0x.
    /// </summary>
    /// <param name="level">Current zoom level</param>
    /// <param name="currentZoomFactor">Current zoom factor</param>
    /// <param name="proposedDecrease">Proposed decrease amount (e.g., 0.1)</param>
    /// <returns>True if zoom-out is allowed, false if at minimum boundary</returns>
    public static bool CanZoomOut(TimelineZoomLevel level, double currentZoomFactor, double proposedDecrease = 0.1)
    {
        var proposedFactor = currentZoomFactor - proposedDecrease;
        var clampedFactor = ClampZoomFactor(level, proposedFactor);

        // Can zoom out if the clamped factor is different from current (within precision)
        return Math.Abs(clampedFactor - currentZoomFactor) > 0.001;
    }

    /// <summary>
    /// Check if current zoom settings are at the minimum day width boundary.
    /// </summary>
    /// <param name="level">Current zoom level</param>
    /// <param name="zoomFactor">Current zoom factor</param>
    /// <returns>True if at minimum boundary</returns>
    public static bool IsAtMinimumDayWidth(TimelineZoomLevel level, double zoomFactor)
    {
        var effectiveDayWidth = CalculateEffectiveDayWidth(level, zoomFactor);
        return Math.Abs(effectiveDayWidth - TaskDisplayConstants.MIN_EFFECTIVE_DAY_WIDTH) < 0.001;
    }

    /// <summary>
    /// Check if a task should be visible at the current zoom settings.
    /// </summary>
    /// <param name="taskDurationDays">Task duration in days</param>
    /// <param name="level">Current zoom level</param>
    /// <param name="zoomFactor">Current zoom factor</param>
    /// <returns>True if task meets minimum width requirement</returns>
    public static bool IsTaskVisible(double taskDurationDays, TimelineZoomLevel level, double zoomFactor)
    {
        var dayWidth = CalculateEffectiveDayWidth(level, zoomFactor);
        var taskWidth = taskDurationDays * dayWidth;
        return TaskDisplayConstants.IsTaskWidthVisible(taskWidth);
    }
}
