namespace GanttComponents.Models;

/// <summary>
/// Configuration settings for a specific timeline zoom level.
/// Defines day width and visual presentation parameters.
/// </summary>
public class ZoomLevelConfiguration
{
    /// <summary>
    /// The zoom level this configuration applies to.
    /// </summary>
    public TimelineZoomLevel Level { get; init; }

    /// <summary>
    /// Base day width in pixels at 1.0x zoom factor.
    /// </summary>
    public double BaseDayWidth { get; init; }

    /// <summary>
    /// Display name for this zoom level (localization key).
    /// </summary>
    public string DisplayNameKey { get; init; } = string.Empty;

    /// <summary>
    /// Description of this zoom level's optimal use case (localization key).
    /// </summary>
    public string DescriptionKey { get; init; } = string.Empty;

    /// <summary>
    /// Minimum effective zoom factor for this level.
    /// </summary>
    public double MinZoomFactor { get; init; } = 0.5;

    /// <summary>
    /// Maximum effective zoom factor for this level.
    /// </summary>
    public double MaxZoomFactor { get; init; } = 3.0;

    /// <summary>
    /// Calculate effective day width for a given zoom factor.
    /// </summary>
    /// <param name="zoomFactor">Zoom multiplier (0.5x - 3.0x)</param>
    /// <returns>Effective day width in pixels</returns>
    public double GetEffectiveDayWidth(double zoomFactor)
    {
        var clampedFactor = Math.Max(MinZoomFactor, Math.Min(MaxZoomFactor, zoomFactor));
        return BaseDayWidth * clampedFactor;
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
    /// Create the standard configuration for a zoom level.
    /// For preset-only zoom approach: min and max factors are both 1.0.
    /// </summary>
    /// <param name="level">The zoom level</param>
    /// <param name="baseDayWidth">Base day width in pixels</param>
    /// <param name="displayNameKey">Localization key for display name</param>
    /// <param name="descriptionKey">Localization key for description</param>
    /// <returns>Configured zoom level settings</returns>
    public static ZoomLevelConfiguration Create(
        TimelineZoomLevel level,
        double baseDayWidth,
        string displayNameKey,
        string descriptionKey)
    {
        return new ZoomLevelConfiguration
        {
            Level = level,
            BaseDayWidth = baseDayWidth,
            DisplayNameKey = displayNameKey,
            DescriptionKey = descriptionKey,
            MinZoomFactor = 1.0,  // Preset-only: no manual zoom factors
            MaxZoomFactor = 1.0   // Preset-only: no manual zoom factors
        };
    }
}
