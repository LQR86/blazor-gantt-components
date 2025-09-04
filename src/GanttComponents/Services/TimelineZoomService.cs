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
            // TEMPLATE-BASED CONFIGURATIONS - 4 Templates
            // ========================================
            // Using duration-to-pixel mapping with template-native units

            // Year-Quarter Template: 24px per quarter (90 days)
            [TimelineZoomLevel.YearQuarterOptimal70px] = ZoomLevelConfiguration.Create(
                TimelineZoomLevel.YearQuarterOptimal70px,
                24.0,   // 24px per quarter
                90.0,   // 90 days per quarter
                4.0,    // Max zoom: 4.0x
                "ZoomLevel.YearQuarterOptimal70px",
                "ZoomLevel.YearQuarterOptimal70px.Description"
            ),

            // Quarter-Month Template: 20px per month (30 days)
            [TimelineZoomLevel.QuarterMonthOptimal60px] = ZoomLevelConfiguration.Create(
                TimelineZoomLevel.QuarterMonthOptimal60px,
                20.0,   // 20px per month
                30.0,   // 30 days per month
                3.5,    // Max zoom: 3.5x
                "ZoomLevel.QuarterMonthOptimal60px",
                "ZoomLevel.QuarterMonthOptimal60px.Description"
            ),

            // Month-Week Template: 18px per week (7 days)
            [TimelineZoomLevel.MonthWeekOptimal50px] = ZoomLevelConfiguration.Create(
                TimelineZoomLevel.MonthWeekOptimal50px,
                18.0,   // 18px per week
                7.0,    // 7 days per week
                3.0,    // Max zoom: 3.0x
                "ZoomLevel.MonthWeekOptimal50px",
                "ZoomLevel.MonthWeekOptimal50px.Description"
            ),

            // Week-Day Template: 12px per day (1 day)
            [TimelineZoomLevel.WeekDayOptimal50px] = ZoomLevelConfiguration.Create(
                TimelineZoomLevel.WeekDayOptimal50px,
                12.0,   // 12px per day
                1.0,    // 1 day per day
                2.5,    // Max zoom: 2.5x
                "ZoomLevel.WeekDayOptimal50px",
                "ZoomLevel.WeekDayOptimal50px.Description"
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
    /// Calculate task pixel width using template-based duration-to-pixel mapping.
    /// Formula: TaskPixelWidth = (TaskDurationDays ÷ TemplateUnitDays) × BaseUnitWidth × ZoomFactor
    /// </summary>
    /// <param name="level">The zoom level</param>
    /// <param name="taskDurationDays">Task duration in days</param>
    /// <param name="zoomFactor">Zoom multiplier (1.0x - template max)</param>
    /// <returns>Task width in pixels</returns>
    public static double CalculateTaskPixelWidth(TimelineZoomLevel level, double taskDurationDays, double zoomFactor)
    {
        var config = GetConfiguration(level);
        return config.CalculateTaskPixelWidth(taskDurationDays, zoomFactor);
    }

    /// <summary>
    /// Calculate effective day width for a zoom level and factor.
    /// BACKWARD COMPATIBILITY: Derived from template-based approach.
    /// </summary>
    /// <param name="level">The zoom level</param>
    /// <param name="zoomFactor">Zoom multiplier (1.0x - template max)</param>
    /// <returns>Effective day width in pixels</returns>
    public static double CalculateEffectiveDayWidth(TimelineZoomLevel level, double zoomFactor)
    {
        var config = GetConfiguration(level);
        return config.GetEffectiveDayWidth(zoomFactor);
    }

    /// <summary>
    /// Validate a zoom factor for a specific template.
    /// Template-native zoom: 1.0x minimum, template-specific maximum.
    /// Also enforces global minimum day width constraint.
    /// </summary>
    /// <param name="level">The zoom level</param>
    /// <param name="zoomFactor">Zoom factor to validate (1.0x - template max)</param>
    /// <returns>Clamped zoom factor within template bounds</returns>
    public static double ClampZoomFactor(TimelineZoomLevel level, double zoomFactor)
    {
        var config = GetConfiguration(level);

        // Template-native approach: 1.0x minimum, template-specific maximum
        var clampedFactor = Math.Max(config.MinZoomFactor, Math.Min(config.MaxZoomFactor, zoomFactor));

        // Ensure minimum day width constraint is met
        var dayWidth = config.GetEffectiveDayWidth(clampedFactor);
        if (dayWidth < TaskDisplayConstants.MIN_EFFECTIVE_DAY_WIDTH)
        {
            // Calculate minimum zoom factor to meet day width requirement
            var minRequiredZoom = (TaskDisplayConstants.MIN_EFFECTIVE_DAY_WIDTH * config.TemplateUnitDays) / config.BaseUnitWidth;
            clampedFactor = Math.Max(clampedFactor, minRequiredZoom);
        }

        return clampedFactor;
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
    /// Check if zoom factor can be decreased further without violating minimum constraints.
    /// Template-native: Can zoom out if current factor > 1.0x and meets minimum day width.
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
    /// Check if zoom factor can be increased further within template maximum.
    /// Template-native: Can zoom in if current factor < template maximum.
    /// </summary>
    /// <param name="level">Current zoom level</param>
    /// <param name="currentZoomFactor">Current zoom factor</param>
    /// <param name="proposedIncrease">Proposed increase amount (e.g., 0.1)</param>
    /// <returns>True if zoom-in is allowed, false if at maximum boundary</returns>
    public static bool CanZoomIn(TimelineZoomLevel level, double currentZoomFactor, double proposedIncrease = 0.1)
    {
        var config = GetConfiguration(level);
        var proposedFactor = currentZoomFactor + proposedIncrease;

        // Can zoom in if proposed factor is within template maximum
        return proposedFactor <= config.MaxZoomFactor;
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
    /// Uses template-based duration-to-pixel mapping.
    /// </summary>
    /// <param name="taskDurationDays">Task duration in days</param>
    /// <param name="level">Current zoom level</param>
    /// <param name="zoomFactor">Current zoom factor</param>
    /// <returns>True if task meets minimum width requirement</returns>
    public static bool IsTaskVisible(double taskDurationDays, TimelineZoomLevel level, double zoomFactor)
    {
        var taskWidth = CalculateTaskPixelWidth(level, taskDurationDays, zoomFactor);
        return TaskDisplayConstants.IsTaskWidthVisible(taskWidth);
    }
}
