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
            [TimelineZoomLevel.WeekDay] = ZoomLevelConfiguration.Create(
                TimelineZoomLevel.WeekDay,
                60.0,
                "ZoomLevel.WeekDay",
                "ZoomLevel.WeekDay.Description"
            ),
            [TimelineZoomLevel.MonthDay] = ZoomLevelConfiguration.Create(
                TimelineZoomLevel.MonthDay,
                25.0,
                "ZoomLevel.MonthDay",
                "ZoomLevel.MonthDay.Description"
            ),
            [TimelineZoomLevel.MonthWeek] = ZoomLevelConfiguration.Create(
                TimelineZoomLevel.MonthWeek,
                15.0,
                "ZoomLevel.MonthWeek",
                "ZoomLevel.MonthWeek.Description"
            ),
            [TimelineZoomLevel.QuarterWeek] = ZoomLevelConfiguration.Create(
                TimelineZoomLevel.QuarterWeek,
                8.0,
                "ZoomLevel.QuarterWeek",
                "ZoomLevel.QuarterWeek.Description"
            ),
            [TimelineZoomLevel.QuarterMonth] = ZoomLevelConfiguration.Create(
                TimelineZoomLevel.QuarterMonth,
                5.0,
                "ZoomLevel.QuarterMonth",
                "ZoomLevel.QuarterMonth.Description"
            ),
            [TimelineZoomLevel.YearQuarter] = ZoomLevelConfiguration.Create(
                TimelineZoomLevel.YearQuarter,
                3.0,
                "ZoomLevel.YearQuarter",
                "ZoomLevel.YearQuarter.Description"
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
    /// <param name="zoomFactor">Zoom multiplier (0.5x - 3.0x)</param>
    /// <returns>Effective day width in pixels</returns>
    public static double CalculateEffectiveDayWidth(TimelineZoomLevel level, double zoomFactor)
    {
        var config = GetConfiguration(level);
        return config.GetEffectiveDayWidth(zoomFactor);
    }

    /// <summary>
    /// Validate a zoom factor for a specific level.
    /// </summary>
    /// <param name="level">The zoom level</param>
    /// <param name="zoomFactor">Zoom factor to validate</param>
    /// <returns>Clamped zoom factor within valid range</returns>
    public static double ClampZoomFactor(TimelineZoomLevel level, double zoomFactor)
    {
        var config = GetConfiguration(level);
        return Math.Max(config.MinZoomFactor, Math.Min(config.MaxZoomFactor, zoomFactor));
    }

    /// <summary>
    /// Get the default zoom settings for backward compatibility.
    /// Current behavior: MonthDay @ 1.6x = 40px (preserves existing 40px day width).
    /// </summary>
    /// <returns>Tuple of (level, factor) that maintains current behavior</returns>
    public static (TimelineZoomLevel level, double factor) GetDefaultZoomSettings()
    {
        return (TaskDisplayConstants.DEFAULT_ZOOM_LEVEL, TaskDisplayConstants.DEFAULT_ZOOM_FACTOR);
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
