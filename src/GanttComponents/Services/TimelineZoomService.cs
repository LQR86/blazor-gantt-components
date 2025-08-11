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
            // ========================================
            // ABC COMPOSITION RENDERERS - 4 Implementations Only
            // ========================================
            // Pure ABC architecture with perfect CSS alignment

            // WeekDay50px - Day cells at 50px - Daily detailed work
            [TimelineZoomLevel.WeekDayOptimal50px] = ZoomLevelConfiguration.Create(
                TimelineZoomLevel.WeekDayOptimal50px,
                50.0, // Perfect: 50px day cells with full detail
                "ZoomLevel.WeekDayOptimal50px",
                "ZoomLevel.WeekDayOptimal50px.Description"
            ),

            // MonthWeek50px - Week cells at 50px (8px day width) - Weekly planning
            [TimelineZoomLevel.MonthWeekOptimal50px] = ZoomLevelConfiguration.Create(
                TimelineZoomLevel.MonthWeekOptimal50px,
                8.0, // Perfect: 8px day width = 56px week cells (8px × 7 days) - INTEGRAL
                "ZoomLevel.MonthWeekOptimal50px",
                "ZoomLevel.MonthWeekOptimal50px.Description"
            ),

            // QuarterMonth60px - Month cells at 60px (2px day width) - Monthly planning
            [TimelineZoomLevel.QuarterMonthOptimal60px] = ZoomLevelConfiguration.Create(
                TimelineZoomLevel.QuarterMonthOptimal60px,
                2.0, // Perfect: 2px day width = 60px month cells (2px × 30 days) - INTEGRAL
                "ZoomLevel.QuarterMonthOptimal60px",
                "ZoomLevel.QuarterMonthOptimal60px.Description"
            ),

            // YearQuarter70px - Quarter cells at 70px (0.78px day width) - Strategic overview
            [TimelineZoomLevel.YearQuarterOptimal70px] = ZoomLevelConfiguration.Create(
                TimelineZoomLevel.YearQuarterOptimal70px,
                0.78, // Perfect: 0.78px day width = 70px quarter cells (0.78px × 90 days)
                "ZoomLevel.YearQuarterOptimal70px",
                "ZoomLevel.YearQuarterOptimal70px.Description"
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
