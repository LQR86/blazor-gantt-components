namespace GanttComponents.Models;

/// <summary>
/// Template-based configuration for timeline zoom levels.
/// Uses duration-to-pixel mapping instead of day width calculations.
/// Formula: TaskPixelWidth = (TaskDurationDays ÷ TemplateUnitDays) × BaseUnitWidth × ZoomFactor
/// </summary>
public class ZoomLevelConfiguration
{
    /// <summary>
    /// The zoom level this configuration applies to.
    /// </summary>
    public TimelineZoomLevel Level { get; init; }

    /// <summary>
    /// Base width in pixels for the template's unit at 1.0x zoom factor.
    /// Year-Quarter: 24px per quarter, Quarter-Month: 20px per month, 
    /// Month-Week: 18px per week, Week-Day: 12px per day
    /// </summary>
    public double BaseUnitWidth { get; init; }

    /// <summary>
    /// Duration in days of the template's base unit.
    /// Year-Quarter: 90 days (quarter), Quarter-Month: 30 days (month),
    /// Month-Week: 7 days (week), Week-Day: 1 day
    /// </summary>
    public double TemplateUnitDays { get; init; }

    /// <summary>
    /// Display name for this zoom level (localization key).
    /// </summary>
    public string DisplayNameKey { get; init; } = string.Empty;

    /// <summary>
    /// Description of this zoom level's optimal use case (localization key).
    /// </summary>
    public string DescriptionKey { get; init; } = string.Empty;

    /// <summary>
    /// Minimum zoom factor for this template (always 1.0).
    /// </summary>
    public double MinZoomFactor { get; init; } = 1.0;

    /// <summary>
    /// Maximum zoom factor for this template.
    /// Year-Quarter: 4.0x, Quarter-Month: 3.5x, Month-Week: 3.0x, Week-Day: 2.5x
    /// </summary>
    public double MaxZoomFactor { get; init; } = 4.0;

    /// <summary>
    /// Calculate task pixel width using template-based duration-to-pixel mapping.
    /// Formula: TaskPixelWidth = (TaskDurationDays ÷ TemplateUnitDays) × BaseUnitWidth × ZoomFactor
    /// </summary>
    /// <param name="taskDurationDays">Task duration in days</param>
    /// <param name="zoomFactor">Zoom multiplier (1.0x - template max)</param>
    /// <returns>Task width in pixels</returns>
    public double CalculateTaskPixelWidth(double taskDurationDays, double zoomFactor)
    {
        var clampedFactor = Math.Max(MinZoomFactor, Math.Min(MaxZoomFactor, zoomFactor));
        return (taskDurationDays / TemplateUnitDays) * BaseUnitWidth * clampedFactor;
    }

    /// <summary>
    /// Calculate effective day width for backward compatibility.
    /// Derived from: DayWidth = BaseUnitWidth × ZoomFactor ÷ TemplateUnitDays
    /// </summary>
    /// <param name="zoomFactor">Zoom multiplier</param>
    /// <returns>Effective day width in pixels</returns>
    public double GetEffectiveDayWidth(double zoomFactor)
    {
        var clampedFactor = Math.Max(MinZoomFactor, Math.Min(MaxZoomFactor, zoomFactor));
        return (BaseUnitWidth * clampedFactor) / TemplateUnitDays;
    }

    /// <summary>
    /// Determine if the given zoom factor is valid for this level.
    /// </summary>
    /// <param name="zoomFactor">Zoom factor to validate</param>
    /// <returns>True if the zoom factor is within valid range</returns>
    public bool IsValidZoomFactor(double zoomFactor)
    {
        return zoomFactor >= MinZoomFactor && zoomFactor <= MaxZoomFactor;
    }

    /// <summary>
    /// Create a template-based configuration for a zoom level.
    /// Uses duration-to-pixel mapping instead of day width calculations.
    /// </summary>
    /// <param name="level">The zoom level</param>
    /// <param name="baseUnitWidth">Base width for template unit in pixels</param>
    /// <param name="templateUnitDays">Duration of template unit in days</param>
    /// <param name="maxZoomFactor">Maximum zoom factor for this template</param>
    /// <param name="displayNameKey">Localization key for display name</param>
    /// <param name="descriptionKey">Localization key for description</param>
    /// <returns>Configured zoom level settings</returns>
    public static ZoomLevelConfiguration Create(
        TimelineZoomLevel level,
        double baseUnitWidth,
        double templateUnitDays,
        double maxZoomFactor,
        string displayNameKey,
        string descriptionKey)
    {
        return new ZoomLevelConfiguration
        {
            Level = level,
            BaseUnitWidth = baseUnitWidth,
            TemplateUnitDays = templateUnitDays,
            MaxZoomFactor = maxZoomFactor,
            DisplayNameKey = displayNameKey,
            DescriptionKey = descriptionKey,
            MinZoomFactor = 1.0  // Template-native: always start at 1.0x
        };
    }
}
