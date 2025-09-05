# Continuous Zooming Implementation

## Overview
This document describes the continuous zooming feature implementation for TimelineZoomControls, which enables smooth zoom factor adjustments within each template's defined range while maintaining the existing discrete zoom level switching capability.

## Problem Statement
Previously, the zoom controls only supported discrete switching between zoom levels (YearQuarter, QuarterMonth, MonthWeek, WeekDay). Users needed more granular control over zoom factors to fine-tune timeline display without changing the template structure.

## Solution: Hybrid Continuous Zooming

### Core Concept
The implementation uses a hybrid approach that combines:
1. **Continuous Factor Zoom**: Adjust zoom factor within current template's range (1.0x to template max)
2. **Level Switching**: Switch between templates when factor limits are reached
3. **Template Following**: Respect each template's configuration limits

### Architecture

#### Zoom Behavior Logic
```csharp
// Priority 1: Try continuous zoom within current template
if (TimelineZoomService.CanZoomIn(CurrentZoomLevel, CurrentZoomFactor, 0.1))
{
    // Adjust factor by 0.1x increments
    var newFactor = Math.Round(CurrentZoomFactor + 0.1, 1);
    await OnZoomFactorChanged.InvokeAsync(clampedFactor);
}
else
{
    // Priority 2: Switch to next zoom level when factor limit reached
    await OnZoomLevelSelected(nextLevel);
}
```

#### Template-Aware Factor Limits
Each zoom level has its own factor range defined in `ZoomLevelConfiguration`:
- **YearQuarter**: 1.0x - 4.0x (can zoom up to 4x from base)
- **QuarterMonth**: 1.0x - 3.5x (can zoom up to 3.5x from base)
- **MonthWeek**: 1.0x - 3.0x (can zoom up to 3x from base)
- **WeekDay**: 1.0x - 2.5x (can zoom up to 2.5x from base)

### UI Components

#### Enhanced Zoom Buttons
The existing zoom in/out buttons now:
- First attempt continuous factor adjustment (0.1x steps)
- Fall back to level switching when factor limits reached
- Update disabled states based on both factor and level boundaries

#### Zoom Factor Slider (Optional)
New optional slider control for precise factor adjustment:
```razor
@if (EffectiveConfig.ShowZoomFactorSlider)
{
    <input type="range" 
           min="@config.MinZoomFactor"
           max="@config.MaxZoomFactor"
           step="0.1"
           value="@CurrentZoomFactor"
           @onchange="OnZoomFactorSliderChanged" />
}
```

### Configuration System

#### Preset Integration
The slider can be enabled through preset configurations:
```csharp
TimelineZoomControlPreset.Debug => new TimelineZoomControlConfiguration
{
    ShowZoomFactorSlider = true, // Enable slider for testing
    // ... other properties
}
```

#### Individual Parameter Control
When using `Preset = Custom`, individual control:
```razor
<TimelineZoomControls ShowZoomFactorSlider="true" />
```

### State Management

#### Enhanced Boundary Checks
Updated `IsAtMaxZoom()` and `IsAtMinZoom()` methods:
```csharp
private bool IsAtMaxZoom()
{
    // Check both factor and level boundaries
    var isAtFactorMax = !TimelineZoomService.CanZoomIn(CurrentZoomLevel, CurrentZoomFactor, 0.1);
    var isAtLevelMax = CurrentZoomLevel == levels.Last();
    
    // At max when both factor and level are at maximum
    return isAtFactorMax && isAtLevelMax;
}
```

#### Event Handling
Two separate events for different zoom actions:
- `OnZoomFactorChanged`: For continuous factor adjustments
- `OnZoomLevelChanged`: For discrete level switching

### Benefits

1. **Granular Control**: Users can fine-tune zoom without changing template structure
2. **Smooth Progression**: 0.1x increments provide smooth zooming experience
3. **Template Respect**: Always follows template-specific zoom factor limits
4. **Backward Compatibility**: Existing level switching behavior preserved
5. **Configurable UI**: Slider can be enabled/disabled per use case
6. **Hybrid Approach**: Best of both continuous and discrete zooming

### Usage Examples

#### Basic Continuous Zooming
```razor
<TimelineZoomControls CurrentZoomLevel="@zoomLevel"
                     CurrentZoomFactor="@zoomFactor"
                     OnZoomLevelChanged="HandleLevelChange"
                     OnZoomFactorChanged="HandleFactorChange" />
```

#### With Zoom Factor Slider
```razor
<TimelineZoomControls Preset="TimelineZoomControlPreset.Debug"
                     CurrentZoomLevel="@zoomLevel"
                     CurrentZoomFactor="@zoomFactor"
                     OnZoomLevelChanged="HandleLevelChange"
                     OnZoomFactorChanged="HandleFactorChange" />
```

#### Custom Configuration
```razor
<TimelineZoomControls Preset="TimelineZoomControlPreset.Custom"
                     ShowZoomFactorSlider="true"
                     ShowQuickActions="true"
                     CurrentZoomLevel="@zoomLevel"
                     CurrentZoomFactor="@zoomFactor"
                     OnZoomLevelChanged="HandleLevelChange"
                     OnZoomFactorChanged="HandleFactorChange" />
```

### Testing Strategy

#### Manual Testing Points
1. **Factor Progression**: Verify 0.1x increments within template ranges
2. **Level Transitions**: Confirm level switches when factor limits reached
3. **Slider Functionality**: Test real-time slider updates
4. **Button States**: Check disabled states at boundaries
5. **Template Limits**: Validate each template's MaxZoomFactor respected

#### Test Coverage
- All 4 zoom levels (YearQuarter, QuarterMonth, MonthWeek, WeekDay)
- Factor boundaries (1.0x and template maximum)
- Level boundaries (YearQuarter to WeekDay transitions)
- Slider and button interaction consistency

## Future Enhancements

### Potential Improvements
1. **Configurable Step Size**: Allow custom increment values (e.g., 0.05x, 0.2x)
2. **Keyboard Shortcuts**: Add keyboard support for zoom operations
3. **Animation**: Smooth visual transitions during zoom changes
4. **Preset Zoom Factors**: Quick-access buttons for common factors (1.0x, 2.0x, max)

### Extensibility
The configuration system supports easy addition of new zoom controls and behaviors without breaking existing functionality.

## Related Files
- `src/GanttComponents/Components/TimelineZoomControls/TimelineZoomControls.razor`
- `src/GanttComponents/Models/TimelineZoomControlPreset.cs`
- `src/GanttComponents/Services/TimelineZoomService.cs`
- `src/GanttComponents/Models/ZoomLevelConfiguration.cs`
- `src/GanttComponents/Pages/GanttComposerDemo.razor`
