using GanttComponents.Services;

namespace GanttComponents.Models;

/// <summary>
/// Predefined header template configuration for a specific timeline zoom level.
/// Follows the preset-only approach to ensure predictable, testable header behavior.
/// </summary>
public class TimelineHeaderTemplate
{
    /// <summary>
    /// Primary header format (top tier) - usually larger time units like months, quarters, years.
    /// </summary>
    public string PrimaryFormat { get; set; } = string.Empty;

    /// <summary>
    /// Secondary header format (bottom tier) - usually smaller time units like days, weeks, months.
    /// </summary>
    public string SecondaryFormat { get; set; } = string.Empty;

    /// <summary>
    /// Whether to show the primary header tier.
    /// </summary>
    public bool ShowPrimary { get; set; } = true;

    /// <summary>
    /// Whether to show the secondary header tier.
    /// </summary>
    public bool ShowSecondary { get; set; } = true;

    /// <summary>
    /// Primary header time unit for calculations.
    /// </summary>
    public TimelineHeaderUnit PrimaryUnit { get; set; }

    /// <summary>
    /// Secondary header time unit for calculations.
    /// </summary>
    public TimelineHeaderUnit SecondaryUnit { get; set; }

    /// <summary>
    /// Format for primary header tooltip when text might be truncated.
    /// </summary>
    public string PrimaryTooltipFormat { get; set; } = string.Empty;

    /// <summary>
    /// Format for secondary header tooltip when text might be truncated.
    /// </summary>
    public string SecondaryTooltipFormat { get; set; } = string.Empty;

    /// <summary>
    /// Description of this template's intended use case.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Creates a standard header template configuration.
    /// </summary>
    /// <param name="primaryUnit">Primary header time unit</param>
    /// <param name="primaryFormat">Primary header format key</param>
    /// <param name="secondaryUnit">Secondary header time unit</param>
    /// <param name="secondaryFormat">Secondary header format key</param>
    /// <param name="description">Template description</param>
    /// <param name="showPrimary">Whether to show primary header</param>
    /// <param name="showSecondary">Whether to show secondary header</param>
    /// <returns>Configured header template</returns>
    public static TimelineHeaderTemplate Create(
        TimelineHeaderUnit primaryUnit,
        string primaryFormat,
        TimelineHeaderUnit secondaryUnit,
        string secondaryFormat,
        string description,
        bool showPrimary = true,
        bool showSecondary = true)
    {
        return new TimelineHeaderTemplate
        {
            PrimaryUnit = primaryUnit,
            PrimaryFormat = primaryFormat,
            SecondaryUnit = secondaryUnit,
            SecondaryFormat = secondaryFormat,
            ShowPrimary = showPrimary,
            ShowSecondary = showSecondary,
            Description = description,
            PrimaryTooltipFormat = primaryFormat + "-verbose",
            SecondaryTooltipFormat = secondaryFormat + "-verbose"
        };
    }
}
