using GanttComponents.Models;

namespace GanttComponents.Models;

/// <summary>
/// Predefined configurations for TimelineZoomControls to fit different UI scenarios.
/// Eliminates the need to manually configure multiple boolean parameters.
/// </summary>
public enum TimelineZoomControlPreset
{
    /// <summary>
    /// Default full-featured configuration - all controls visible with labels.
    /// Best for: Main Gantt view, dedicated zoom control areas.
    /// Shows: Preset buttons, current state, action buttons, all labels.
    /// </summary>
    Default,

    /// <summary>
    /// Extremely compact - just +/- action buttons with minimal icons.
    /// Best for: Toolbars with severe space constraints, mobile headers.
    /// Shows: Only "+" and "−" action buttons, no labels, no presets.
    /// </summary>
    ExtremelyCompact,

    /// <summary>
    /// Compact with labeled actions - action buttons with text labels.
    /// Best for: Toolbars with moderate space, secondary control areas.
    /// Shows: Action buttons with "Zoom In"/"Zoom Out" labels, no presets.
    /// </summary>
    CompactLabeled,

    /// <summary>
    /// Compact with dropdown - action buttons plus preset level dropdown.
    /// Best for: Control panels, sidebar controls, integrated toolbars.
    /// Shows: Action buttons + preset dropdown, minimal labels.
    /// </summary>
    CompactDropdown,

    /// <summary>
    /// Preset-only configuration - just the preset level buttons.
    /// Best for: Dedicated zoom level selection, clean UI focus.
    /// Shows: Only preset level buttons, no action buttons, clean layout.
    /// </summary>
    PresetOnly,

    /// <summary>
    /// Status display - current zoom level and info, read-only.
    /// Best for: Status bars, info panels, readonly displays.
    /// Shows: Current zoom level, day width, zoom factor - no controls.
    /// </summary>
    StatusOnly,

    /// <summary>
    /// Developer/debug configuration - all features visible for testing.
    /// Best for: Development, debugging, testing all functionalities.
    /// Shows: Everything expanded, day width, zoom factor, all controls.
    /// </summary>
    Debug,

    /// <summary>
    /// Minimal preset strip - horizontal preset buttons without labels.
    /// Best for: Quick level switching, horizontal space available.
    /// Shows: Preset buttons only, no labels, no status, very clean.
    /// </summary>
    MinimalPresets,

    /// <summary>
    /// Action-focused - prominent action buttons with current status.
    /// Best for: Interactive controls where zoom in/out is primary action.
    /// Shows: Large action buttons, current level display, no preset buttons.
    /// </summary>
    ActionFocused,

    /// <summary>
    /// Custom configuration - use individual boolean parameters.
    /// Best for: When none of the presets fit the specific use case.
    /// Shows: Whatever is configured via individual parameters.
    /// </summary>
    Custom
}

/// <summary>
/// Configuration settings for TimelineZoomControls based on preset selection.
/// Encapsulates all the boolean and display options for different scenarios.
/// </summary>
public class TimelineZoomControlConfiguration
{
    public bool ShowLevelPresets { get; set; } = true;
    public bool ShowCurrentState { get; set; } = true;
    public bool ShowQuickActions { get; set; } = true;
    public bool ShowLabels { get; set; } = true;
    public bool ShowActionLabels { get; set; } = false;
    public bool ShowZoomFactor { get; set; } = true;
    public bool ShowDayWidth { get; set; } = false;
    public bool HideCurrentState { get; set; } = false;
    public string CssClass { get; set; } = "";
    public bool UseDropdownForPresets { get; set; } = false;
    public int MaxVisiblePresets { get; set; } = 13; // Show all 13 levels by default

    /// <summary>
    /// Gets the predefined configuration for a specific preset.
    /// </summary>
    public static TimelineZoomControlConfiguration GetPresetConfiguration(TimelineZoomControlPreset preset)
    {
        return preset switch
        {
            TimelineZoomControlPreset.Default => new TimelineZoomControlConfiguration
            {
                ShowLevelPresets = true,
                ShowCurrentState = true,
                ShowQuickActions = true,
                ShowLabels = true,
                ShowActionLabels = false,
                ShowZoomFactor = true,
                ShowDayWidth = false,
                HideCurrentState = false,
                CssClass = "",
                UseDropdownForPresets = false,
                MaxVisiblePresets = 13
            },

            TimelineZoomControlPreset.ExtremelyCompact => new TimelineZoomControlConfiguration
            {
                ShowLevelPresets = false,
                ShowCurrentState = false,
                ShowQuickActions = true,
                ShowLabels = false,
                ShowActionLabels = false,
                ShowZoomFactor = false,
                ShowDayWidth = false,
                HideCurrentState = true,
                CssClass = "compact minimal",
                UseDropdownForPresets = false,
                MaxVisiblePresets = 0
            },

            TimelineZoomControlPreset.CompactLabeled => new TimelineZoomControlConfiguration
            {
                ShowLevelPresets = false,
                ShowCurrentState = false,
                ShowQuickActions = true,
                ShowLabels = false,
                ShowActionLabels = true,
                ShowZoomFactor = false,
                ShowDayWidth = false,
                HideCurrentState = true,
                CssClass = "compact",
                UseDropdownForPresets = false,
                MaxVisiblePresets = 0
            },

            TimelineZoomControlPreset.CompactDropdown => new TimelineZoomControlConfiguration
            {
                ShowLevelPresets = true,
                ShowCurrentState = false,
                ShowQuickActions = true,
                ShowLabels = false,
                ShowActionLabels = false,
                ShowZoomFactor = false,
                ShowDayWidth = false,
                HideCurrentState = true,
                CssClass = "compact",
                UseDropdownForPresets = true,
                MaxVisiblePresets = 13
            },

            TimelineZoomControlPreset.PresetOnly => new TimelineZoomControlConfiguration
            {
                ShowLevelPresets = true,
                ShowCurrentState = false,
                ShowQuickActions = false,
                ShowLabels = false,
                ShowActionLabels = false,
                ShowZoomFactor = false,
                ShowDayWidth = false,
                HideCurrentState = true,
                CssClass = "",
                UseDropdownForPresets = false,
                MaxVisiblePresets = 13
            },

            TimelineZoomControlPreset.StatusOnly => new TimelineZoomControlConfiguration
            {
                ShowLevelPresets = false,
                ShowCurrentState = true,
                ShowQuickActions = false,
                ShowLabels = true,
                ShowActionLabels = false,
                ShowZoomFactor = true,
                ShowDayWidth = true,
                HideCurrentState = false,
                CssClass = "",
                UseDropdownForPresets = false,
                MaxVisiblePresets = 0
            },

            TimelineZoomControlPreset.Debug => new TimelineZoomControlConfiguration
            {
                ShowLevelPresets = true,
                ShowCurrentState = true,
                ShowQuickActions = true,
                ShowLabels = true,
                ShowActionLabels = true,
                ShowZoomFactor = true,
                ShowDayWidth = true,
                HideCurrentState = false,
                CssClass = "",
                UseDropdownForPresets = false,
                MaxVisiblePresets = 13
            },

            TimelineZoomControlPreset.MinimalPresets => new TimelineZoomControlConfiguration
            {
                ShowLevelPresets = true,
                ShowCurrentState = false,
                ShowQuickActions = false,
                ShowLabels = false,
                ShowActionLabels = false,
                ShowZoomFactor = false,
                ShowDayWidth = false,
                HideCurrentState = true,
                CssClass = "minimal",
                UseDropdownForPresets = false,
                MaxVisiblePresets = 13
            },

            TimelineZoomControlPreset.ActionFocused => new TimelineZoomControlConfiguration
            {
                ShowLevelPresets = false,
                ShowCurrentState = true,
                ShowQuickActions = true,
                ShowLabels = true,
                ShowActionLabels = true,
                ShowZoomFactor = false,
                ShowDayWidth = false,
                HideCurrentState = false,
                CssClass = "",
                UseDropdownForPresets = false,
                MaxVisiblePresets = 0
            },

            TimelineZoomControlPreset.Custom => new TimelineZoomControlConfiguration
            {
                // Custom preset returns defaults - user will override via individual parameters
                ShowLevelPresets = true,
                ShowCurrentState = true,
                ShowQuickActions = true,
                ShowLabels = true,
                ShowActionLabels = false,
                ShowZoomFactor = true,
                ShowDayWidth = false,
                HideCurrentState = false,
                CssClass = "",
                UseDropdownForPresets = false,
                MaxVisiblePresets = 13
            },

            _ => throw new ArgumentOutOfRangeException(nameof(preset), preset, "Unknown preset configuration")
        };
    }

    /// <summary>
    /// Gets a human-readable description of what each preset is optimized for.
    /// </summary>
    public static string GetPresetDescription(TimelineZoomControlPreset preset)
    {
        return preset switch
        {
            TimelineZoomControlPreset.Default => "Full-featured configuration with all controls and labels visible",
            TimelineZoomControlPreset.ExtremelyCompact => "Minimal +/- action buttons only, no labels or presets",
            TimelineZoomControlPreset.CompactLabeled => "Action buttons with text labels, no preset buttons",
            TimelineZoomControlPreset.CompactDropdown => "Action buttons plus preset level dropdown menu",
            TimelineZoomControlPreset.PresetOnly => "Only preset level buttons, no action buttons or status",
            TimelineZoomControlPreset.StatusOnly => "Read-only display of current zoom state, no controls",
            TimelineZoomControlPreset.Debug => "All features visible including developer info like day width",
            TimelineZoomControlPreset.MinimalPresets => "Clean preset buttons strip without labels or status",
            TimelineZoomControlPreset.ActionFocused => "Prominent action buttons with current level display",
            TimelineZoomControlPreset.Custom => "Use individual boolean parameters for custom configuration",
            _ => "Unknown preset configuration"
        };
    }
}
