# Custom Gantt UI Components - Implementation Plan & Milestones (EXPERIMENTAL)

> **⚠️ EXPERIMENTAL DOCUMENT**: This is an experimental implementation guide that may contain uncertainty and evolving approaches. For immutable requirements, see [REQUIREMENTS.md](./REQUIREMENTS.md).

## 🎯 Project Overview
**Goal**: Build custom Gantt UI components from scratch to replace Syncfusion with full source control
**Philosophy**: UI-first approach with independent composable components
**Strategy**: TaskGrid + TimelineView as separate components that work independently or together via GanttComposer
**Framework**: Blazor Server (.NET 8.0) with standard web technologies
**Requirements Source**: All implementation must satisfy requirements in [REQUIREMENTS.md](./REQUIREMENTS.md)

---

## 🔒 **CRITICAL CONSTRAINTS FROM REQUIREMENTS**

> **Note**: These constraints are immutable and defined in [REQUIREMENTS.md](./REQUIREMENTS.md). Implementation approaches may vary but constraints must be satisfied.

### Day-Level Scheduling ONLY (REQUIREMENT 1)
**Constraint**: Maximum hour-level granularity, NO minute/second precision
**Impact on Implementation**: 
- Date-to-pixel conversion uses day boundaries only
- Timeline scales limited to: Hour, Day, Week, Month, Quarter
- Duration format: "5d" or "8h" (no minutes/seconds)
- Dependency offsets in day units: "+3d", "-2d"
- Simplifies all date calculations and timeline rendering

### UTC Timestamps Only (REQUIREMENT 2)
**Constraint**: All timestamps stored and processed in UTC only
**Impact on Implementation**:
- No timezone conversion logic needed
- All date inputs converted to UTC immediately
- UI displays in user's local timezone for viewing only
- Eliminates timezone complexity and edge cases

### No Batch Operations (REQUIREMENT 3)
**Constraint**: Single-operation CRUD only, immediate feedback
**Impact on Implementation**:
- No batch edit modes in grid
- No bulk dependency creation
- Immediate server round-trips for changes
- Simpler state management and UX

### WBS Code Task Identification (REQUIREMENT 12)
**Constraint**: WBS codes as the only user-facing task identifiers
**Impact on Implementation**:
- Hierarchical WBS structure (e.g., "1", "1.1", "1.1.1", "1.2", "2")
- Auto-generation based on task hierarchy
- Dependencies use WBS codes (e.g., "1.2FS+3d")
- Database IDs kept internal, never exposed to users

### I18N Support (REQUIREMENT 14)
**Constraint**: English and Chinese (Simplified) only, LTR text direction
**Impact on Implementation**:
- Resource files for all user-visible text
- Datetime format localization
- No hardcoded strings in components
- Fallback to English for missing translations

### .NET 8.0 Target Framework (REQUIREMENT 18)
**Constraint**: Must use .NET 8.0 as target framework
**Impact on Implementation**:
- Latest C# language features available
- Performance optimizations from .NET 8.0
- Modern Blazor Server capabilities

---

---

## 🛠️ Technology Stack (Requirements-Compliant)

### Mandated Core Framework (REQUIREMENT 18)
- **Blazor Server (.NET 8.0)** - Primary framework (non-negotiable)
- **CSS Grid & Flexbox** - Layout fundamentals (required)
- **SVG** - Timeline graphics rendering (required)
- **Standard Web Technologies** - No exotic dependencies (required)

### Approved Supporting Libraries
- **Virtual Scrolling** - For performance with large datasets (1000+ tasks)
- **date-fns** - For day/hour-level date calculations (UTC handling)
- **Interact.js** - For drag & drop interactions
- **PopperJS** - For tooltips and context menus
- **ResizeObserver API** - For responsive layouts
- **html2canvas + jsPDF** - For PDF export with vector graphics

### Material Design Implementation (REQUIREMENT 13)
- **CSS Custom Properties** - For theming and Material design
- **Material Icons** - For consistent iconography
- **Material Colors** - Standardized color palette

### I18N Implementation (REQUIREMENT 14)
- **Blazor Localization** - Resource files for English/Chinese
- **IStringLocalizer** - Component text localization
- **CultureInfo** - DateTime format localization
- **No RTL Support** - LTR text direction only

---

## 📋 Implementation Milestones (Requirements-Driven)

> **Validation**: Each milestone must satisfy corresponding requirements in [REQUIREMENTS.md](./REQUIREMENTS.md)

### Phase 1: Independent Components Foundation (Week 1-2)
**Goal**: Build TaskGrid and TimelineView as standalone components (REQUIREMENTS 5, 6)

#### Milestone 1.1: TaskGrid Component with WBS Support
- [ ] **Basic Grid Structure (REQUIREMENT 9)**
  - CSS Grid-based layout for performance
  - Fixed header with scrollable body
  - Column definitions and sizing
  - Row virtualization for 1000+ tasks (REQUIREMENT 16)

- [ ] **WBS Code Implementation (REQUIREMENT 12)**
  - WBS code column as primary task identifier
  - Hierarchical WBS generation ("1", "1.1", "1.1.1", "1.2", "2")
  - Auto-renumbering on hierarchy changes
  - WBS validation and uniqueness enforcement
  - Database IDs kept internal (never exposed to users)

- [ ] **Tree Structure Support (REQUIREMENT 8)**
  - Expandable/collapsible hierarchy
  - Indentation with visual guides
  - Parent-child relationship indicators
  - Efficient tree state management

- [ ] **I18N Implementation (REQUIREMENT 14)**
  - All text externalized to resource files
  - English/Chinese (Simplified) support
  - Localized datetime formats
  - IStringLocalizer integration

- [ ] **Grid Interactions (REQUIREMENT 9)**
  - Row selection (single/multiple)
  - Column resizing, reordering, show/hide
  - Keyboard navigation (arrows, tab, enter)
  - Sorting and filtering capabilities

- [ ] **Cell Rendering with Validation**
  - Text cells with inline editing
  - UTC date picker cells (REQUIREMENT 2)
  - Duration cells ("5d"/"8h" format)
  - Progress percentage cells
  - Resource assignment cells
  - Real-time validation and constraints

#### Milestone 1.2: TimelineView Component (SVG-Based)
- [ ] **SVG Timeline Architecture (REQUIREMENT 18)**
  - SVG-based rendering (required technology)
  - Scalable coordinate system (day → pixel conversion)
  - Zoom levels: Hour, Day, Week, Month, Quarter only
  - UTC timestamp handling throughout

- [ ] **Timeline Foundation**
  - Viewport and pan/zoom calculations
  - Responsive width/height handling
  - Material Design visual standards (REQUIREMENT 13)

- [ ] **Timeline Header**
  - Multi-tier date headers (major/minor scales)
  - Localized date formatting (English/Chinese)
  - Today indicator line
  - Working/non-working time backgrounds (day boundaries)
  - Holiday markers (day precision only)

- [ ] **Task Bar Rendering**
  - SVG rectangular bars for tasks
  - WBS code labels on bars (not database IDs)
  - Progress indicators (filled portion)
  - Milestone markers (diamonds)
  - Baseline indicators (planned vs actual)
  - Critical path highlighting

- [ ] **Dependencies with WBS Codes (REQUIREMENT 8)**
  - Dependency line rendering (FS, SS, FF, SF)
  - WBS-based dependency syntax ("1.2FS+3d")
  - Day-level offset support only (+3d, -2d)
  - Circular dependency detection
  - Multiple dependencies per task

#### Milestone 1.3: Advanced Data Architecture
- [ ] **UTC Data Management (REQUIREMENT 2)**
  - All timestamps stored in UTC
  - Immediate UTC conversion on input
  - Local timezone display only (no timezone logic)
  - Day-boundary calculations

- [ ] **Single-Operation CRUD (REQUIREMENT 3)**
  - No batch operations anywhere
  - Immediate feedback for each action
  - Real-time server synchronization
  - Simple state management

- [ ] **Resource Management (REQUIREMENT 8)**
  - Three-table relationship (Tasks, Resources, Assignments)
  - Resource assignment to tasks
  - Multi-resource assignment per task
  - Day-level utilization tracking
  - Resource cost calculation

- [ ] **Task Scheduling Engine**
  - Auto-scheduling based on WBS dependencies
  - Duration calculations (days/hours only)
  - Critical path calculation
  - Day-level calendar integration
  - Slack time computation (day precision)
  - Dependency validation and warnings

### Phase 2: Advanced UI Features & Compliance (Week 3-4)
**Goal**: Add interactions, accessibility, and Material Design compliance

#### Milestone 2.1: TaskGrid Enhancements (REQUIREMENT 9)
- [ ] **Advanced Grid Features**
  - Sorting by columns with I18N support
  - Filtering capabilities with localized labels
  - Global search functionality
  - Context menus with localized text

- [ ] **Material Design Implementation (REQUIREMENT 13)**
  - Material Design color palette and spacing
  - Material typography and elevation
  - CSS custom properties for theming
  - Material motion and animations

- [ ] **Accessibility Compliance (REQUIREMENT 20)**
  - WCAG AA compliance
  - ARIA labels and roles
  - Keyboard navigation for all features
  - Screen reader support
  - Focus indicators and logical tab order

- [ ] **I18N Polish (REQUIREMENT 14)**
  - All text from resource files
  - Chinese datetime formatting
  - Currency and number localization
  - Fallback to English for missing translations

#### Milestone 2.2: TimelineView Interactions & Performance
- [ ] **Performance Targets (REQUIREMENT 16)**
  - 500+ tasks at 60fps rendering
  - Smooth animations using RAF
  - Efficient DOM updates
  - Memory management and cleanup

- [ ] **Mouse Interactions**
  - Task bar selection with visual feedback
  - Hover effects with Material design
  - WBS-based tooltips (not database IDs)
  - Right-click context menus

- [ ] **Pan and Zoom**
  - Mouse wheel zoom (hour to quarter scales only)
  - Pan with mouse drag
  - Zoom to fit functionality
  - Smooth transitions with Material motion

- [ ] **Advanced Timeline Features (REQUIREMENT 8)**
  - Baseline comparison view
  - Critical path visualization
  - Event markers for milestones
  - Working/non-working time backgrounds

### Phase 3: Component Integration & Row Alignment (Week 5-6)
**Goal**: Achieve pixel-perfect integration (REQUIREMENTS 5, 6)

#### Milestone 3.1: GanttComposer Component (REQUIREMENT 5)
- [ ] **Independent Component Integration**
  - TaskGrid and TimelineView work standalone
  - Composable via GanttComposer
  - Each component has complete API
  - No tight coupling between components

- [ ] **Pixel-Perfect Row Alignment (REQUIREMENT 6 - CRITICAL)**
  - Shared row height state management
  - RowAlignmentManager service
  - Dynamic height calculation for tree nodes
  - Alignment maintained during scroll, zoom, resize
  - Alignment preserved during tree expand/collapse
  - No visual row misalignment under any circumstances

- [ ] **Layout Manager**
  - Splitter between TaskGrid and TimelineView
  - Synchronized row heights
  - Proportional resizing with user preferences
  - Material Design consistent spacing

- [ ] **Synchronized Scrolling**
  - Vertical scroll synchronization
  - Horizontal timeline panning
  - Scroll position persistence
  - Smooth scroll behavior with RAF

- [ ] **Event Coordination**
  - Selection sync between components
  - Row highlighting coordination
  - Focus management
  - Keyboard shortcut handling

#### Milestone 3.2: Drag & Drop Integration
- [ ] **Timeline Editing (Single Operations Only)**
  - Drag task bars to change dates (UTC conversion)
  - Resize bars for duration changes
  - Progress handle dragging
  - Snap-to-day/hour grid functionality
  - Immediate server synchronization (no batch mode)

- [ ] **WBS-Based Interactions**
  - WBS code validation during edits
  - Dependency creation using WBS codes
  - Visual feedback with WBS identifiers
  - Constraint checking with WBS validation

- [ ] **Cross-Component Interactions**
  - Drag from grid to timeline
  - Visual feedback during drags
  - Drop validation and confirmation
  - Real-time constraint checking

### Phase 4: Performance Optimization & Export (Week 7-8)
**Goal**: Meet performance targets and export requirements

#### Milestone 4.1: Performance Compliance (REQUIREMENTS 16, 17)
- [ ] **Performance Targets Achievement**
  - TaskGrid: 1000+ rows with smooth scrolling
  - TimelineView: 500+ tasks at 60fps rendering
  - Bundle size <100KB gzipped for core components
  - Stable memory usage during interactions

- [ ] **Technical Performance Optimization**
  - Virtual scrolling implementation
  - Efficient DOM updates and rendering
  - Optimized paint and layout operations
  - RAF-based smooth animations
  - Memory management and cleanup

- [ ] **Rendering Optimization**
  - SVG optimization for timeline
  - Debounced resize handling
  - CSS containment for paint optimization
  - GPU acceleration where appropriate

#### Milestone 4.2: Export & Enterprise Features (REQUIREMENT 22)
- [ ] **PDF Export with Vector Graphics**
  - SVG → PDF pipeline for scalable output
  - Multiple page sizes (A4, Letter, Legal, Custom)
  - Portrait/Landscape orientation support
  - Print-optimized layouts with page breaks

- [ ] **Export Configuration**
  - Date range selection for export
  - Column selection (include/exclude)
  - Quality settings and compression
  - WBS-based export organization

- [ ] **Professional Polish (REQUIREMENT 15)**
  - Clean, modern interface design
  - Consistent Material Design spacing
  - Professional appearance for enterprise
  - Clear visual hierarchy
  - Accessible color contrast

#### Milestone 4.3: Final Integration & Testing
- [ ] **Browser Compatibility (REQUIREMENT 21)**
  - Modern browser support (Chrome, Firefox, Safari, Edge)
  - Cross-platform compatibility testing
  - Responsive design validation
  - Consistent behavior verification

- [ ] **Final Validation Against Requirements**
  - All 27 requirement sections satisfied
  - Performance benchmarks met
  - Accessibility compliance verified
  - Export functionality validated
  - WBS code system fully functional
  - I18N implementation complete

---

## 🏗️ Component Architecture (UI-Focused)

### Independent Components Design
```
TaskGrid (Standalone)          TimelineView (Standalone)
├── GridHeader                 ├── TimelineHeader
├── GridBody                   ├── TimelineBody
│   ├── VirtualRows           │   ├── TaskBars (SVG)
│   └── GridCells             │   ├── GridLines
└── GridScrollbar             │   └── TodayLine
                              └── TimelineScrollbar

GanttComposer (Integration)
├── Splitter
├── TaskGrid (embedded)
├── TimelineView (embedded)
├── SyncManager
└── RowAlignmentManager
```

### Critical: Row Alignment Strategy
**The Challenge**: Ensuring TaskGrid rows perfectly align with TimelineView task bars
**Why Critical**: Misalignment breaks the entire Gantt experience

#### Alignment Approaches
**Option A: Shared Row Height State (Recommended)**
```css
/* Both components use same CSS custom properties */
:root {
  --gantt-row-height: 32px;
  --gantt-header-height: 40px;
  --gantt-border-width: 1px;
}
```

**Option B: Master-Slave Pattern**
- TaskGrid as master (calculates row heights)
- TimelineView as slave (receives height data)
- Event-driven height synchronization

**Option C: Virtual Row Manager**
- Centralized row positioning service
- Both components query for row positions
- Handles dynamic heights, expand/collapse

#### Implementation Details
**Row Height Calculation**
```typescript
interface RowMetrics {
  index: number;
  height: number;
  top: number;
  visible: boolean;
  expanded?: boolean;
}

class RowAlignmentManager {
  calculateRowPositions(data: any[]): RowMetrics[]
  getRowAtPosition(y: number): RowMetrics
  syncComponents(): void
}
```

**Synchronization Points**
- [ ] Initial render alignment
- [ ] Scroll position sync
- [ ] Row height changes (expand/collapse)
- [ ] Data updates (add/remove rows)
- [ ] Zoom level changes
- [ ] Resize operations

### Technology Choices for Each Component

#### TaskGrid - Proven Approaches
- **CSS Grid Layout** - Native browser performance
- **Virtual Scrolling** - react-window/react-virtualized patterns
- **Intersection Observer** - For efficient rendering
- **CSS Containment** - Optimize paint and layout

#### TimelineView - Battle-Tested Options
- **SVG Rendering** (Recommended start)
  - Similar to D3.js approach
  - Precise positioning and scaling
  - Easy hover/click detection
  - Good accessibility support
  
- **Canvas Optimization** (Phase 2)
  - When performance becomes critical
  - Offscreen canvas for pre-rendering
  - WebGL for extreme performance

#### Date/Time Handling
- **date-fns** - Lightweight, immutable, tree-shakeable (day/hour precision)
- **Custom Timeline Math** - Day-to-pixel conversion utilities
- **Simplified Calendar** - Day-level working time calculations

---

## 🎨 UI Implementation Strategy

### Material Design Implementation
```css
/* CSS Custom Properties for theming */
:root {
  --gantt-primary: #1976d2;
  --gantt-surface: #ffffff;
  --gantt-on-surface: #000000;
  --gantt-outline: #e0e0e0;
  --gantt-elevation-1: 0 1px 3px rgba(0,0,0,0.12);
  --gantt-border-radius: 4px;
  --gantt-spacing-unit: 8px;
}
```

### Component-Specific Styling
- **TaskGrid**: CSS Grid with sticky headers
- **TimelineView**: SVG with CSS transforms for zoom/pan
- **Splitter**: CSS resize handles with smooth animations
- **Interactions**: CSS transitions with Material motion curves

### Responsive Design Strategy
- **Mobile**: Stack components vertically
- **Tablet**: Reduced feature set, touch-friendly
- **Desktop**: Full feature set with keyboard shortcuts

---

## 📄 Export & Printing Strategy

### PDF Export Options
**Vector-Based Export (Recommended)**
- **SVG → PDF Pipeline**: Convert our SVG timeline directly to vector PDF
- **Libraries**: jsPDF with SVG plugin, or server-side with Puppeteer
- **Benefits**: Scalable, crisp text, small file sizes
- **Challenges**: Complex layout calculations

**Canvas-Based Export (Alternative)**
- **html2canvas → PDF**: Screenshot approach with canvas rendering
- **Libraries**: html2canvas + jsPDF
- **Benefits**: What-you-see-is-what-you-get accuracy
- **Challenges**: Raster graphics, larger file sizes

### Print Optimization
**CSS Print Media Queries**
```css
@media print {
  .gantt-container {
    /* Remove shadows, optimize colors */
    box-shadow: none;
    background: white;
  }
  
  .timeline-view {
    /* Ensure proper page breaks */
    page-break-inside: avoid;
  }
  
  .task-grid {
    /* Optimize table for printing */
    font-size: 10pt;
    line-height: 1.2;
  }
}
```

**Multi-Page Layouts**
- **Horizontal Pagination**: Split timeline across multiple pages
- **Vertical Pagination**: Handle large task lists
- **Page Headers/Footers**: Add project info, dates, page numbers
- **Scale-to-Fit Options**: Auto-resize to fit page width

### Export Configuration UI
- [ ] **Format Selection**: PDF, PNG, SVG, Print
- [ ] **Page Setup**: A4, Letter, Legal, Custom sizes
- [ ] **Layout Options**: Portrait/Landscape, Margins
- [ ] **Content Options**: Include/exclude columns, date ranges
- [ ] **Quality Settings**: DPI for raster, compression for PDF

### Implementation Approach
**Phase 1**: Basic print CSS optimization
**Phase 2**: Simple PDF export (screenshot-based)
**Phase 3**: Vector PDF export with proper pagination
**Phase 4**: Advanced export options and templates

---

## ⚙️ Row Alignment & Synchronization (Critical Technical Challenge)

### The Alignment Problem
**Core Issue**: TaskGrid (DOM-based) and TimelineView (SVG-based) must maintain pixel-perfect row alignment
**Complexity Factors**:
- Variable row heights (tree expand/collapse)
- Virtual scrolling with different rendering approaches
- Browser rendering differences
- Dynamic content (text wrapping, icons)
- Responsive design considerations

### Synchronization Solutions

#### 1. Shared State Management
```typescript
// Blazor service for row state management
public class GanttRowState
{
    public Dictionary<int, RowMetrics> RowPositions { get; set; }
    public int DefaultRowHeight { get; set; } = 32;
    public int HeaderHeight { get; set; } = 40;
    
    public event Action<Dictionary<int, RowMetrics>>? RowPositionsChanged;
    
    public void UpdateRowPosition(int index, RowMetrics metrics)
    {
        RowPositions[index] = metrics;
        RowPositionsChanged?.Invoke(RowPositions);
    }
}
```

#### 2. CSS-Based Coordination
```css
/* Shared variables for consistent sizing */
.gantt-container {
  --row-height: 32px;
  --header-height: 40px;
  --border-width: 1px;
  --padding-vertical: 4px;
}

.task-grid-row, .timeline-task-bar {
  height: var(--row-height);
  border-bottom: var(--border-width) solid var(--gantt-outline);
}
```

#### 3. Measurement & Correction System
```typescript
// Blazor component for alignment validation
public class AlignmentValidator
{
    public async Task ValidateAlignment()
    {
        var gridRows = await GetGridRowPositions();
        var timelineRows = await GetTimelineRowPositions();
        
        var misalignments = FindMisalignments(gridRows, timelineRows);
        if (misalignments.Any())
        {
            await CorrectAlignment(misalignments);
        }
    }
}
```

### Specific Challenge Solutions

#### Tree Expand/Collapse Synchronization
- [ ] **State Coordination**: Both components listen to tree state changes
- [ ] **Animation Sync**: Coordinated expand/collapse animations
- [ ] **Height Recalculation**: Immediate position updates for all rows below

#### Virtual Scrolling Alignment
- [ ] **Viewport Coordination**: Same virtual window calculations
- [ ] **Row Index Mapping**: Consistent virtual-to-actual index conversion
- [ ] **Render Timing**: Ensure both components render simultaneously

#### Dynamic Content Handling
- [ ] **Text Overflow**: Consistent text wrapping/truncation rules
- [ ] **Icon Spacing**: Standardized icon and indentation metrics
- [ ] **Content Changes**: Trigger alignment recalculation on data updates

### Debugging & Validation Tools
- [ ] **Alignment Overlay**: Visual debugging tool to show row boundaries
- [ ] **Position Logger**: Console output of row positions for both components
- [ ] **Automated Tests**: Unit tests for alignment calculations
- [ ] **Browser Compatibility**: Cross-browser alignment validation

### Performance Considerations
- [ ] **Throttled Updates**: Debounce alignment recalculations
- [ ] **Efficient Measurements**: Batch DOM measurements
- [ ] **Memory Management**: Clean up event listeners
- [ ] **RAF Optimization**: Use requestAnimationFrame for smooth updates

---

## 🎯 Syncfusion Feature Parity Analysis

### Core Syncfusion Features We Must Support
Based on Syncfusion's comprehensive feature set, our architecture needs to handle:

#### **Data Management**
- [ ] **Hierarchical Data**: Self-referencing tree structures with ParentID
- [ ] **Large Datasets**: Virtual loading for parent records, lazy-load children
- [ ] **Multiple Data Sources**: Tasks, Resources, Assignments (3-table relationship)
- [ ] **Single-Operation CRUD**: Create, read, update, delete with immediate validation
- [ ] **Real-time Updates**: No batch mode, immediate server sync

#### **Task Dependencies (Simplified)**
- [ ] **4 Dependency Types**: FS (Finish-to-Start), SS (Start-to-Start), FF (Finish-to-Finish), SF (Start-to-Finish)
- [ ] **Multiple Dependencies**: "4FS,5SS" syntax support
- [ ] **Day-Level Offsets**: "+3d", "-2d" syntax for delays/leads (no sub-day precision)
- [ ] **Circular Detection**: Prevent and warn about circular dependencies
- [ ] **Real-time Validation**: Immediate dependency validation during editing

#### **Resource Management (Simplified)**
- [ ] **Resource Collections**: Separate resource data with MaxUnits
- [ ] **Assignment Mapping**: TaskID ↔ ResourceID relationship table
- [ ] **Multi-Resource Tasks**: Multiple resources per task with different units
- [ ] **Day-Level Utilization**: Track availability and overallocation (day boundaries)
- [ ] **Resource Views**: Display resource assignments in grid and timeline

#### **Critical Path & Scheduling (Day-Level)**
- [ ] **Auto-Scheduling**: Recalculate dates based on dependencies (day precision)
- [ ] **Critical Path**: Identify and highlight critical tasks
- [ ] **Slack Calculation**: Determine task float/buffer time (day units)
- [ ] **Task Types**: Fixed Duration, Fixed Work, Fixed Units
- [ ] **Day-Level Calendars**: Working days, holidays, custom calendars

#### **Simplified Timeline Features**
- [ ] **Baseline Support**: Show planned vs actual progress
- [ ] **Event Markers**: Milestone indicators on timeline
- [ ] **Data Markers**: Custom indicators for status/flags
- [ ] **Hour-Day-Week-Month Scales**: No minute/second precision
- [ ] **Working Time**: Visual distinction for working/non-working periods

#### **Editing Capabilities (Simplified)**
- [ ] **Inline Editing**: Direct cell editing in grid
- [ ] **Dialog Editing**: Modal forms with tabbed interface
- [ ] **Taskbar Editing**: Drag to change dates, resize for duration
- [ ] **Dependency Editing**: Create dependencies by dragging between tasks
- [ ] **Resource Assignment**: Multi-select resources with units
- [ ] **Real-time Updates**: No batch mode, immediate save

#### **UI/UX Features**
- [ ] **Toolbar Integration**: Built-in toolbar with common actions
- [ ] **Column Management**: Show/hide, resize, reorder columns
- [ ] **Filtering**: Column filters and global search
- [ ] **Selection**: Single/multiple row and cell selection
- [ ] **Context Menus**: Right-click actions for tasks and resources

### Architecture Implications

#### **TaskGrid Component Requirements**
```typescript
interface TaskGridCapabilities {
  // Must support resource assignment editing
  resourceColumns: ResourceColumnConfig[];
  
  // Must handle dependency string parsing
  dependencyParser: DependencyStringParser;
  
  // Must support complex editing modes
  editingModes: ['inline', 'dialog']; // No batch mode
  
  // Must integrate with scheduling engine
  schedulingEngine: TaskSchedulingEngine;
}
```

#### **TimelineView Component Requirements**
```typescript
interface TimelineViewCapabilities {
  // Must render multiple visual elements
  taskBars: TaskBarRenderer;
  baselines: BaselineRenderer;
  dependencies: DependencyLineRenderer;
  eventMarkers: EventMarkerRenderer;
  dataMarkers: DataMarkerRenderer;
  
  // Must support complex interactions
  dragAndDrop: TaskBarDragDropHandler;
  dependencyCreation: DependencyDrawingHandler;
  
  // Must handle different view modes
  viewModes: ['task', 'resource', 'critical-path'];
}
```

#### **Data Architecture (Requirements-Compliant)**
```csharp
// Core entities aligned with REQUIREMENTS.md
public class GanttTask
{
    // WBS Code as primary user-facing identifier (REQUIREMENT 12)
    public string WbsCode { get; set; } = ""; // e.g., "1", "1.1", "1.1.1"
    
    // Internal database ID (never exposed to users)
    public int Id { get; set; }
    
    public string Name { get; set; } = "";
    
    // UTC timestamps only (REQUIREMENT 2)
    public DateTime StartDate { get; set; } // Day precision, stored in UTC
    public DateTime EndDate { get; set; }   // Day precision, stored in UTC
    
    // Duration in simplified format (REQUIREMENT 1)
    public string Duration { get; set; } = ""; // "5d" or "8h" format only
    
    public decimal Progress { get; set; } // 0.0 to 1.0
    
    // Hierarchy using WBS codes
    public string? ParentWbsCode { get; set; }
    
    // Dependencies using WBS codes (REQUIREMENT 12)
    public string Dependencies { get; set; } = ""; // "1.2FS+3d,1.3SS-1d" (day precision only)
    
    public TaskType Type { get; set; } = TaskType.FixedDuration;
    public decimal? Work { get; set; } // In hours
    
    // Baseline support (REQUIREMENT 8)
    public GanttBaseline? Baseline { get; set; }
}

public class GanttBaseline
{
    public DateTime StartDate { get; set; } // UTC, day precision
    public DateTime EndDate { get; set; }   // UTC, day precision
    public string Duration { get; set; } = ""; // "5d" or "8h" format
}

public class GanttResource
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public decimal MaxUnits { get; set; } = 1.0m;
    public decimal? Cost { get; set; }
}

public class GanttAssignment
{
    public string TaskWbsCode { get; set; } = ""; // Using WBS code, not task ID
    public int ResourceId { get; set; }
    public decimal Units { get; set; }
    public decimal? Work { get; set; } // In hours
}

public enum TaskType
{
    FixedDuration,
    FixedWork,
    FixedUnits
}
```

### Implementation Priority (Requirements-Driven)
**Phase 1**: WBS codes, UTC handling, and day-level precision from the beginning
**Phase 2**: I18N support and Material Design compliance
**Phase 3**: Pixel-perfect row alignment and component integration
**Phase 4**: Performance targets and export functionality

**Requirements Benefits**:
- **Day-level precision**: Faster development, simpler testing, reduced complexity
- **UTC only**: No timezone bugs, simpler date calculations
- **WBS codes**: Meaningful identifiers for users, professional project management
- **Single operations**: Simpler UX, immediate feedback, easier implementation
- **I18N support**: Professional localization for English/Chinese markets
- Better performance with fewer precision requirements
- Easier maintenance and troubleshooting

This ensures our custom components can replace Syncfusion for day-level project management without unnecessary complexity.

---

## 🧪 Testing Strategy (UI-Focused)

### Component Testing
- [ ] **TaskGrid Tests**
  - Virtual scrolling performance
  - Tree expand/collapse
  - Cell editing workflows
  - Keyboard navigation

- [ ] **TimelineView Tests**
  - Zoom/pan smoothness
  - Task bar rendering accuracy
  - Date-to-pixel conversion
  - Selection and hover states

- [ ] **Integration Tests**
  - Scroll synchronization
  - Selection coordination
  - Responsive behavior
  - Cross-browser compatibility

### Performance Benchmarks
- **TaskGrid**: 1000+ rows with smooth scrolling
- **TimelineView**: 500+ tasks with 60fps rendering
- **Memory**: Stable memory usage during interactions
- **Bundle Size**: <100KB gzipped for core components

---

## 📦 Phase Deliverables (Requirements-Validated)

### Phase 1: Independent Components Foundation
- **TaskGrid**: Fully functional grid with WBS codes and I18N
- **TimelineView**: SVG timeline with UTC handling and day precision
- **Demo Pages**: Each component working independently
- **Foundation**: Requirements-compliant architecture

### Phase 2: Material Design & Accessibility
- **TaskGrid**: Material Design and WCAG AA compliance
- **TimelineView**: Performance targets and visual polish
- **I18N**: Complete English/Chinese localization
- **Testing**: Requirements validation coverage

### Phase 3: Pixel-Perfect Integration
- **GanttComposer**: Row-aligned combined experience
- **Synchronization**: Smooth scroll and selection sync
- **WBS Editing**: Cross-component drag & drop with WBS validation
- **UX**: Enterprise-grade user experience

### Phase 4: Production Ready
- **Performance**: All benchmarks satisfied (1000+ tasks, 60fps)
- **Export**: Vector PDF with WBS organization
- **Enterprise**: Professional appearance and reliability
- **Validation**: All 27 requirement sections satisfied

---

## 🚀 Implementation Starting Strategy

### Week 1: TaskGrid Foundation (Requirements-First)
1. **Day 1-2**: WBS code system and basic CSS Grid layout
2. **Day 3-4**: Tree structure with WBS hierarchy
3. **Day 5-7**: I18N implementation and UTC date handling

### Week 2: TimelineView Foundation (SVG + UTC)
1. **Day 1-2**: SVG coordinate system with day-to-pixel conversion
2. **Day 3-4**: Timeline header with localized date formatting
3. **Day 5-7**: Task bars with WBS labels and baseline support

### Starting Point Recommendation
**Begin with TaskGrid** - WBS code implementation and I18N setup provide solid foundation for all subsequent components.

---

## ⚠️ **EXPERIMENTAL DOCUMENT DISCLAIMER**

> **This document contains experimental implementation approaches that may evolve during development. For immutable requirements that must be satisfied regardless of implementation changes, always refer to [REQUIREMENTS.md](./REQUIREMENTS.md).**

### Key Relationships:
- **[REQUIREMENTS.md](./REQUIREMENTS.md)**: Immutable contract with stakeholders (27 sections)
- **This Document**: Experimental technical implementation guide
- **Validation Rule**: All experimental approaches must satisfy requirements

### Implementation Philosophy:
- **Requirements-Driven**: Every implementation decision validates against REQUIREMENTS.md
- **Experimental Safety**: Technical approaches can change as long as requirements are met
- **Strategic Separation**: User value (requirements) separated from technical uncertainty (implementation)

**Always validate experimental implementations against the 27 requirement sections before considering any milestone complete.**
