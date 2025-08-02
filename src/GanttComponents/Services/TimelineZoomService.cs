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
                96.0, // 60 * 1.6 for backward compatibility
                "ZoomLevel.WeekDay",
                "ZoomLevel.WeekDay.Description"
            ),
            [TimelineZoomLevel.WeekDayMedium] = ZoomLevelConfiguration.Create(
                TimelineZoomLevel.WeekDayMedium,
                72.0, // 45 * 1.6 for backward compatibility
                "ZoomLevel.WeekDayMedium",
                "ZoomLevel.WeekDayMedium.Description"
            ),
            [TimelineZoomLevel.WeekDayLow] = ZoomLevelConfiguration.Create(
                TimelineZoomLevel.WeekDayLow,
                56.0, // 35 * 1.6 for backward compatibility
                "ZoomLevel.WeekDayLow",
                "ZoomLevel.WeekDayLow.Description"
            ),
            [TimelineZoomLevel.MonthWeek] = ZoomLevelConfiguration.Create(
                TimelineZoomLevel.MonthWeek,
                40.0, // 25 * 1.6 for backward compatibility - matches GanttResources.resx
                "ZoomLevel.MonthWeek",
                "ZoomLevel.MonthWeek.Description"
            ),
            [TimelineZoomLevel.MonthWeekMedium] = ZoomLevelConfiguration.Create(
                TimelineZoomLevel.MonthWeekMedium,
                32.0, // 20 * 1.6 for backward compatibility - matches GanttResources.resx
                "ZoomLevel.MonthWeekMedium",
                "ZoomLevel.MonthWeekMedium.Description"
            ),
            [TimelineZoomLevel.MonthWeekLow] = ZoomLevelConfiguration.Create(
                TimelineZoomLevel.MonthWeekLow,
                28.8, // 18 * 1.6 for backward compatibility - matches GanttResources.resx
                "ZoomLevel.MonthWeekLow",
                "ZoomLevel.MonthWeekLow.Description"
            ),
            [TimelineZoomLevel.QuarterMonth] = ZoomLevelConfiguration.Create(
                TimelineZoomLevel.QuarterMonth,
                24.0, // 15 * 1.6 for backward compatibility (15px/day from GanttResources.resx)
                "ZoomLevel.QuarterMonth",
                "ZoomLevel.QuarterMonth.Description"
            ),
            [TimelineZoomLevel.QuarterMonthMedium] = ZoomLevelConfiguration.Create(
                TimelineZoomLevel.QuarterMonthMedium,
                19.2, // 12 * 1.6 for backward compatibility (12px/day from GanttResources.resx)
                "ZoomLevel.QuarterMonthMedium",
                "ZoomLevel.QuarterMonthMedium.Description"
            ),
            [TimelineZoomLevel.QuarterMonthLow] = ZoomLevelConfiguration.Create(
                TimelineZoomLevel.QuarterMonthLow,
                16.0, // 10 * 1.6 for backward compatibility (10px/day from GanttResources.resx)
                "ZoomLevel.QuarterMonthLow",
                "ZoomLevel.QuarterMonthLow.Description"
            ),
            [TimelineZoomLevel.YearQuarter] = ZoomLevelConfiguration.Create(
                TimelineZoomLevel.YearQuarter,
                12.8, // 8 * 1.6 for backward compatibility (8px/day from GanttResources.resx)
                "ZoomLevel.YearQuarter",
                "ZoomLevel.YearQuarter.Description"
            ),
            [TimelineZoomLevel.YearQuarterMedium] = ZoomLevelConfiguration.Create(
                TimelineZoomLevel.YearQuarterMedium,
                10.4, // 6.5 * 1.6 for backward compatibility (6.5px/day from GanttResources.resx)
                "ZoomLevel.YearQuarterMedium",
                "ZoomLevel.YearQuarterMedium.Description"
            ),
            [TimelineZoomLevel.YearQuarterLow] = ZoomLevelConfiguration.Create(
                TimelineZoomLevel.YearQuarterLow,
                8.0, // 5 * 1.6 for backward compatibility (5px/day from GanttResources.resx)
                "ZoomLevel.YearQuarterLow",
                "ZoomLevel.YearQuarterLow.Description"
            ),
            [TimelineZoomLevel.YearQuarterMin] = ZoomLevelConfiguration.Create(
                TimelineZoomLevel.YearQuarterMin,
                3.0, // Minimum day width constraint (3px/day from GanttResources.resx)
                "ZoomLevel.YearQuarterMin",
                "ZoomLevel.YearQuarterMin.Description"
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
