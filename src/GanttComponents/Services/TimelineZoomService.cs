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
            // ========================================
            // OPTIMAL CONFIGURATIONS (Phase 2 - RECOMMENDED)
            // ========================================
            // Revolutionary cell-size-first approach: ALL levels have perfect 30-70px visual density!

            // Week-Day Pattern (4 levels): 30px-60px day cells - Perfect for detailed daily work
            [TimelineZoomLevel.WeekDayOptimal60px] = ZoomLevelConfiguration.Create(
                TimelineZoomLevel.WeekDayOptimal60px,
                60.0, // Perfect: 60px day cells with rich information and team details
                "ZoomLevel.WeekDayOptimal60px",
                "ZoomLevel.WeekDayOptimal60px.Description"
            ),
            [TimelineZoomLevel.WeekDayOptimal50px] = ZoomLevelConfiguration.Create(
                TimelineZoomLevel.WeekDayOptimal50px,
                50.0, // Perfect: 50px day cells with full detail and team information
                "ZoomLevel.WeekDayOptimal50px",
                "ZoomLevel.WeekDayOptimal50px.Description"
            ),
            [TimelineZoomLevel.WeekDayOptimal40px] = ZoomLevelConfiguration.Create(
                TimelineZoomLevel.WeekDayOptimal40px,
                40.0, // Perfect: 40px day cells with enhanced visibility and icons
                "ZoomLevel.WeekDayOptimal40px",
                "ZoomLevel.WeekDayOptimal40px.Description"
            ),
            [TimelineZoomLevel.WeekDayOptimal30px] = ZoomLevelConfiguration.Create(
                TimelineZoomLevel.WeekDayOptimal30px,
                30.0, // Perfect: 30px day cells with clean daily overview
                "ZoomLevel.WeekDayOptimal30px",
                "ZoomLevel.WeekDayOptimal30px.Description"
            ),

            // Month-Week Pattern (4 levels): 35px-70px week cells - Perfect for weekly planning
            [TimelineZoomLevel.MonthWeekOptimal60px] = ZoomLevelConfiguration.Create(
                TimelineZoomLevel.MonthWeekOptimal60px,
                10.0, // Perfect: 10px day width = 70px week cells (10px × 7 days) - INTEGRAL
                "ZoomLevel.MonthWeekOptimal60px",
                "ZoomLevel.MonthWeekOptimal60px.Description"
            ),
            [TimelineZoomLevel.MonthWeekOptimal50px] = ZoomLevelConfiguration.Create(
                TimelineZoomLevel.MonthWeekOptimal50px,
                8.0, // Perfect: 8px day width = 56px week cells (8px × 7 days) - INTEGRAL
                "ZoomLevel.MonthWeekOptimal50px",
                "ZoomLevel.MonthWeekOptimal50px.Description"
            ),
            [TimelineZoomLevel.MonthWeekOptimal40px] = ZoomLevelConfiguration.Create(
                TimelineZoomLevel.MonthWeekOptimal40px,
                6.0, // Perfect: 6px day width = 42px week cells (6px × 7 days) - INTEGRAL
                "ZoomLevel.MonthWeekOptimal40px",
                "ZoomLevel.MonthWeekOptimal40px.Description"
            ),
            [TimelineZoomLevel.MonthWeekOptimal30px] = ZoomLevelConfiguration.Create(
                TimelineZoomLevel.MonthWeekOptimal30px,
                5.0, // Perfect: 5px day width = 35px week cells (5px × 7 days) - INTEGRAL
                "ZoomLevel.MonthWeekOptimal30px",
                "ZoomLevel.MonthWeekOptimal30px.Description"
            ),

            // Quarter-Month Pattern (4 levels): 30px-60px month cells - Perfect for monthly planning
            [TimelineZoomLevel.QuarterMonthOptimal60px] = ZoomLevelConfiguration.Create(
                TimelineZoomLevel.QuarterMonthOptimal60px,
                2.33, // Perfect: 2.33px day width = 70px month cells (2.33px × 30 days)
                "ZoomLevel.QuarterMonthOptimal60px",
                "ZoomLevel.QuarterMonthOptimal60px.Description"
            ),
            [TimelineZoomLevel.QuarterMonthOptimal50px] = ZoomLevelConfiguration.Create(
                TimelineZoomLevel.QuarterMonthOptimal50px,
                1.67, // Perfect: 1.67px day width = 50px month cells (1.67px × 30 days)
                "ZoomLevel.QuarterMonthOptimal50px",
                "ZoomLevel.QuarterMonthOptimal50px.Description"
            ),
            [TimelineZoomLevel.QuarterMonthOptimal40px] = ZoomLevelConfiguration.Create(
                TimelineZoomLevel.QuarterMonthOptimal40px,
                1.33, // Perfect: 1.33px day width = 40px month cells (1.33px × 30 days)
                "ZoomLevel.QuarterMonthOptimal40px",
                "ZoomLevel.QuarterMonthOptimal40px.Description"
            ),
            [TimelineZoomLevel.QuarterMonthOptimal30px] = ZoomLevelConfiguration.Create(
                TimelineZoomLevel.QuarterMonthOptimal30px,
                1.0, // Perfect: 1.0px day width = 30px month cells (1.0px × 30 days)
                "ZoomLevel.QuarterMonthOptimal30px",
                "ZoomLevel.QuarterMonthOptimal30px.Description"
            ),

            // Year-Quarter Pattern (4 levels): 30px-70px quarter cells - Perfect for strategic overview
            [TimelineZoomLevel.YearQuarterOptimal70px] = ZoomLevelConfiguration.Create(
                TimelineZoomLevel.YearQuarterOptimal70px,
                0.78, // Perfect: 0.78px day width = 70px quarter cells (0.78px × 90 days)
                "ZoomLevel.YearQuarterOptimal70px",
                "ZoomLevel.YearQuarterOptimal70px.Description"
            ),
            [TimelineZoomLevel.YearQuarterOptimal50px] = ZoomLevelConfiguration.Create(
                TimelineZoomLevel.YearQuarterOptimal50px,
                0.56, // Perfect: 0.56px day width = 50px quarter cells (0.56px × 90 days)
                "ZoomLevel.YearQuarterOptimal50px",
                "ZoomLevel.YearQuarterOptimal50px.Description"
            ),
            [TimelineZoomLevel.YearQuarterOptimal40px] = ZoomLevelConfiguration.Create(
                TimelineZoomLevel.YearQuarterOptimal40px,
                0.44, // Perfect: 0.44px day width = 40px quarter cells (0.44px × 90 days)
                "ZoomLevel.YearQuarterOptimal40px",
                "ZoomLevel.YearQuarterOptimal40px.Description"
            ),
            [TimelineZoomLevel.YearQuarterOptimal30px] = ZoomLevelConfiguration.Create(
                TimelineZoomLevel.YearQuarterOptimal30px,
                0.33, // Perfect: 0.33px day width = 30px quarter cells (0.33px × 90 days)
                "ZoomLevel.YearQuarterOptimal30px",
                "ZoomLevel.YearQuarterOptimal30px.Description"
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
