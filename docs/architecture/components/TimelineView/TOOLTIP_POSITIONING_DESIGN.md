# 🏷️ Timeline Header Tooltips - Positioning Design

## 📋 Overview

This document describes the design and implementation of viewport-fixed tooltips for hidden timeline periods in the TimelineView component. The tooltips appear at the left and right edges of the timeline viewport to indicate hidden content when users scroll horizontally.

## 🎯 User Experience Goals

- **Left Edge Tooltip**: Shows what's hidden to the left (e.g., "←(2024 Q3 Aug)")
- **Right Edge Tooltip**: Shows what's hidden to the right (e.g., "(2028 Q1 Jan)→")
- **Responsive Content**: Tooltip text adapts to current zoom level
- **Consistent Behavior**: Works identically in standalone and embedded layouts
- **Non-Intrusive**: Appears only when content is actually hidden

## 🏗️ Architecture Overview

```
┌─────────────────────────────────────────────────────────────┐
│ .timeline-container (position: relative) ← POSITIONING ROOT │
│ ┌─────────────────────────────────────────────────────────┐ │
│ │ [←(Q3)] Timeline Header                    [(Q1)→]     │ │
│ │         ────────────────────────────────────────        │ │
│ │ Timeline Content (scrollable)                           │ │
│ └─────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────┘
```

## 🔧 Technical Implementation

### **Core Components**

1. **ITimelineTooltipService**: Service interface for tooltip calculation logic
2. **TimelineTooltipService**: Service implementation with edge detection and content generation
3. **CSS Positioning**: Absolute positioning relative to `.timeline-container`
4. **Conditional Rendering**: Blazor conditional blocks based on service results

### **Service Architecture**

```csharp
public interface ITimelineTooltipService
{
    TimelineTooltipResult CalculateTooltips(TimelineTooltipRequest request);
}

public class TimelineTooltipRequest
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public double EffectiveDayWidth { get; set; }
    public TimelineZoomLevel ZoomLevel { get; set; }
    public double ViewportScrollLeft { get; set; }
    public double ViewportWidth { get; set; }
    public TimelineTooltipOptions Options { get; set; }
}

public class TimelineTooltipResult
{
    public bool HasLeftTooltip { get; set; }
    public bool HasRightTooltip { get; set; }
    public string LeftTooltip { get; set; }
    public string RightTooltip { get; set; }
}
```

### **Razor Template**

```razor
@{
    var tooltipResult = GetTooltipResult();
}

@if (tooltipResult.HasLeftTooltip)
{
    <div class="timeline-viewport-tooltip timeline-viewport-tooltip-left">
        @tooltipResult.LeftTooltip
    </div>
}

@if (tooltipResult.HasRightTooltip)
{
    <div class="timeline-viewport-tooltip timeline-viewport-tooltip-right">
        @tooltipResult.RightTooltip
    </div>
}
```

## 🎨 CSS Positioning Strategy

### **The Positioning Challenge**

Timeline tooltips need to appear at consistent viewport edges regardless of layout context:

- **Timeline Demo**: TimelineView used standalone in page flow
- **Gantt Composer**: TimelineView embedded within positioned layout containers

### **Solution: Consistent Positioning Root**

```css
.timeline-container {
    position: relative;  /* ⭐ KEY: Forces consistent positioning context */
    display: flex;
    flex-direction: column;
    height: 100%;
}

.timeline-viewport-tooltip {
    position: absolute;  /* Positions relative to .timeline-container */
    top: calc(var(--header-month-height) / 2);
    transform: translateY(-50%);
    z-index: 100;
    pointer-events: none;
}

.timeline-viewport-tooltip-left {
    left: 8px;   /* 8px from left edge of timeline container */
}

.timeline-viewport-tooltip-right {
    right: 8px;  /* 8px from right edge of timeline container */
}
```

### **Why This Works**

With `position: relative` on `.timeline-container`:

1. **Consistent Ancestor**: Tooltips always position relative to the same element
2. **Layout Independence**: Works in both standalone and embedded scenarios
3. **No Side Effects**: `position: relative` with no offsets doesn't affect layout
4. **Predictable Behavior**: Tooltips appear at consistent positions within the timeline

## 📊 Positioning Context Comparison

### **Before Fix (Inconsistent Behavior)**

```
Timeline Demo:
└── .demo-section
    └── .timeline-container (no positioning)
        └── tooltips (position: absolute) → positions relative to body

Gantt Composer:
└── .gantt-composer (position: relative)
    └── .composer-timeline (position: absolute)
        └── .timeline-container (no positioning)
            └── tooltips (position: absolute) → positions relative to .composer-timeline
```

### **After Fix (Consistent Behavior)**

```
Timeline Demo:
└── .demo-section
    └── .timeline-container (position: relative) ← POSITIONING ROOT
        └── tooltips (position: absolute) → positions relative to .timeline-container

Gantt Composer:
└── .gantt-composer (position: relative)
    └── .composer-timeline (position: absolute)
        └── .timeline-container (position: relative) ← POSITIONING ROOT
            └── tooltips (position: absolute) → positions relative to .timeline-container
```

## 🔍 Edge Detection Logic

### **Viewport Tracking**

```javascript
// JavaScript interop for real-time viewport data
ViewportScrollLeft = document.querySelector('.timeline-scroll-container').scrollLeft;
ViewportWidth = document.querySelector('.timeline-scroll-container').clientWidth;
```

### **Visibility Conditions**

```csharp
// Left tooltip: Show when content is scrolled past left edge
bool HasLeftTooltip = ViewportScrollLeft > (EffectiveDayWidth * HiddenThreshold);

// Right tooltip: Show when timeline extends beyond right edge
bool HasRightTooltip = (TotalTimelineWidth - ViewportScrollLeft) > ViewportWidth;
```

### **Content Generation**

```csharp
// Adaptive content based on zoom level
string LeftTooltip = ZoomLevel switch
{
    TimelineZoomLevel.YearQuarter3px => "←(2024 Q3)",
    TimelineZoomLevel.QuarterMonth17px => "←(2024 Q3 Aug)",
    TimelineZoomLevel.MonthDay52px => "←(Aug 15)",
    TimelineZoomLevel.WeekDay68px => "←(Week 32)",
    _ => "←"
};
```

## 🎛️ Configuration Options

### **TimelineTooltipOptions**

```csharp
public class TimelineTooltipOptions
{
    public double HiddenThreshold { get; set; } = 0.5;    // Show when >50% hidden
    public string LeftArrow { get; set; } = "←";         // Left arrow symbol
    public string RightArrow { get; set; } = "→";        // Right arrow symbol
    public bool UsePrimaryPeriods { get; set; } = true;  // Use primary period labels
}
```

### **Zoom Level Adaptation**

| Zoom Level | Primary Period | Tooltip Example |
|------------|----------------|-----------------|
| YearQuarter3px | Quarter | `←(2024 Q3)` |
| QuarterMonth17px | Quarter + Month | `←(2024 Q3 Aug)` |
| MonthDay52px | Day | `←(Aug 15)` |
| WeekDay68px | Week | `←(Week 32)` |

## 🧪 Testing Strategy

### **Cross-Layout Testing**

1. **Timeline Demo** (`/timeline-demo`):
   - Verify tooltips appear at timeline edges
   - Test horizontal scrolling behavior
   - Validate zoom level content adaptation

2. **Gantt Composer Demo** (`/gantt-composer-demo`):
   - Confirm identical tooltip behavior
   - Test with different grid splitter positions
   - Verify tooltips don't interfere with grid interactions

### **Responsive Testing**

- Browser window resizing
- Different screen sizes and resolutions
- Mobile viewport simulation
- High DPI display compatibility

## ⚡ Performance Considerations

### **Efficient Updates**

- **Service-based calculation**: Centralized logic reduces code duplication
- **Conditional rendering**: Tooltips only exist in DOM when needed
- **Minimal JavaScript**: Only viewport tracking, no complex positioning
- **CSS-only styling**: Hardware-accelerated transforms and positioning

### **Memory Management**

- **No event listeners**: Uses Blazor's built-in scroll event handling
- **Stateless service**: TimelineTooltipService has no instance state
- **Garbage collection friendly**: Minimal object allocation during scroll

## 🔄 Future Enhancements

### **Potential Improvements**

1. **Hover Activation**: Show tooltips only on edge hover (reduced visual noise)
2. **Animation**: Smooth fade-in/out transitions
3. **Click Actions**: Navigate to hidden periods on tooltip click
4. **Accessibility**: ARIA live regions for screen reader support
5. **Customization**: User-defined tooltip templates and styling

### **Extensibility Points**

- **ITimelineTooltipService**: Replace with custom implementation
- **CSS Custom Properties**: Theme-aware styling
- **TimelineTooltipOptions**: Runtime configuration
- **Service Registration**: Dependency injection flexibility

## 📚 Related Documentation

- [TimelineView Architecture Summary](./TIMELINEVIEW_ARCHITECTURE_SUMMARY.md)
- [TimelineView Implementation Guide](./TIMELINEVIEW_IMPLEMENTATION_GUIDE.md)
- [Timeline Zooming Design](../TIMELINE_ZOOMING_DESIGN.md)
- [Gantt Composer Architecture](../GANTT_COMPOSER_ARCHITECTURE.md)

## 📝 Changelog

### v1.0 - Initial Implementation
- Basic edge detection and tooltip rendering
- Service-based architecture
- Cross-layout positioning support

### v1.1 - Positioning Fix
- Added `position: relative` to `.timeline-container`
- Resolved inconsistent behavior between standalone and embedded usage
- Improved CSS documentation and comments

---

*Last Updated: August 9, 2025*
*Author: GitHub Copilot*
*Version: 1.1*
