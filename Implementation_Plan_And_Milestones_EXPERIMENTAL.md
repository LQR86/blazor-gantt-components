# Custom Gantt UI Components - Implementation Plan & Milestones (EXPERIMENTAL)

> **⚠️ EXPERIMENTAL DOCUMENT**: This is an experimental implementation guide that may contain uncertainty and evolving approaches. For immutable requirements, see [REQUIREMENTS.md](./REQUIREMENTS.md).

## ⚡ **IMPLEMENTATION PHILOSOPHY - SIMPLE & REALISTIC FIRST**

> **🎯 GUIDING PRINCIPLE: BUILD SIMPLE, WORKING SOFTWARE**
> 
> This implementation plan follows the **SIMPLE-FIRST METHODOLOGY**:
> - ✅ **WORKING > PERFECT**: Deliver working features before polishing
> - ✅ **SIMPLE > CLEVER**: Choose obvious solutions over elegant complexity
> - ✅ **REALISTIC > AMBITIOUS**: Set achievable milestones we can actually hit
> - ✅ **INCREMENTAL > BIG-BANG**: Small, working steps over large integrations
> 
> **Decision Framework for Every Feature**:
> 1. **Can we build this in 1-2 days?** → If no, break it down further
> 2. **Will this work without external dependencies?** → Prefer browser standards
> 3. **Can someone else understand this in 6 months?** → Keep it obvious
> 4. **Does this solve today's problem?** → Don't build for hypothetical futures
> 
> **Success Pattern**: Week 1 working foundation → Week 2 basic integration → Week 3-4 polish → Week 5-8 essential features
> 
> **Failure Pattern**: Week 1-4 complex architecture → Week 5-8 fighting integration issues → No working software

---

## 🎯 Project Overview - SIMPLIFIED FOR SUCCESS
**Goal**: Build simple, working custom Gantt UI components that satisfy core requirements with minimum complexity
**Philosophy**: Simplicity first - meet core requirements with least possible features, enhance in phases
**Strategy**: Basic TaskGrid + Basic TimelineView that work together (Phase 1), enhance with essential features later
**Framework**: Blazor Server (.NET 8.0) with minimal dependencies
**Requirements Source**: All implementation must satisfy requirements in [REQUIREMENTS.md](./REQUIREMENTS.md)
**Success Metric**: 
- **Phase 1**: Working end-to-end Gantt chart with core functionality
- **Phase 2+**: Essential Gantt features (dependencies, zoom, export, resource management)
- **Final**: Full-featured Gantt replacement for Syncfusion

---

## 🔒 **CRITICAL CONSTRAINTS FROM REQUIREMENTS**

> **Note**: These constraints are immutable and defined in [REQUIREMENTS.md](./REQUIREMENTS.md). Implementation approaches may vary but constraints must be satisfied.

### Day-Level Scheduling ONLY (REQUIREMENT 1)
**Constraint**: DAY precision ONLY - NO hours, minutes, or seconds in timestamps or calculations
**Impact on Implementation**: 
- Date-to-pixel conversion uses day boundaries exclusively
- Timeline scales limited to: Day, Week, Month, Quarter (NO hour scale)
- Duration format: "5d" or "5D" ONLY - NO hour units in scheduling
- Dependency offsets in day units ONLY: "+3d", "-2d"
- Drop/ignore any hour/minute/second precision in all inputs
- Hours ONLY used for daily working time settings (8h workday), NOT for task scheduling
- Simplifies all date calculations and timeline rendering

### UTC Date Storage Only (REQUIREMENT 2)
**Constraint**: All dates stored as DATE only in UTC (no time components whatsoever)
**Impact on Implementation**:
- No timezone conversion logic needed
- All date inputs converted to UTC DATE immediately (time components dropped)
- UI displays dates in user's local date format for viewing only
- Hours used ONLY for working time configuration (e.g., "8 hours per workday")
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
- Date format localization (dates only, no times)
- Duration unit localization ("days", "天") - DAY units only
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

## 🛠️ Technology Stack (Requirements-Compliant) - REVISED

### Mandated Core Framework (REQUIREMENT 18)
- **Blazor Server (.NET 8.0)** - Primary framework (non-negotiable)
- **CSS Grid & Flexbox** - Layout fundamentals (required)
- **SVG** - Timeline graphics rendering (required)
- **Standard Web Technologies** - No exotic dependencies (required)

### Minimal Supporting Technologies (Start Simple)
- **CSS Custom Properties** - For theming and Material design (already using)
- **Material Icons** - For consistent iconography (already using)
- **Native Browser APIs** - ResizeObserver, IntersectionObserver for performance

### I18N Implementation (REQUIREMENT 14) - Built-in .NET
- **Blazor Localization** - Resource files for English/Chinese
- **IStringLocalizer** - Component text localization  
- **CultureInfo** - Date format localization (dates only, no times)
- **No RTL Support** - LTR text direction only

### Libraries to Add Only When Needed (Phase 2+)
- **Virtual Scrolling** - Custom implementation first, library if needed
- **Date Utilities** - Custom day-level math first, date-fns if complexity grows
- **PDF Export** - Simple print CSS first, then consider libraries

### Technologies to Avoid (Unless Proven Necessary)
- ❌ **Interact.js** - Use native HTML5 drag & drop first
- ❌ **PopperJS** - Use CSS positioning first  
- ❌ **html2canvas** - Use CSS print media queries first
- ❌ **Complex charting libraries** - We're building custom SVG

**Philosophy**: Start with browser standards and .NET built-ins. Add external dependencies only when we hit clear limitations.

---

## 📋 Implementation Milestones (Requirements-Driven) - REVISED

> **Validation**: Each milestone must satisfy corresponding requirements in [REQUIREMENTS.md](./REQUIREMENTS.md)
> **Strategy**: Walking Skeleton approach - get basic end-to-end working first, then enhance

### Phase 1: Foundation & Walking Skeleton (Week 1-2)
**Goal**: Establish foundation and get basic end-to-end integration working (REQUIREMENTS 5, 6, 12)

#### Milestone 1.1: WBS Code Foundation (CRITICAL FIRST) ✅ COMPLETED
**Why First**: WBS codes are fundamental to everything - UI labels, dependencies, user interactions
- [COMPLETED] **WBS Code Data Model (REQUIREMENT 12)**
  - Add WbsCode property to GanttTask model
  - Hierarchical WBS structure ("1", "1.1", "1.1.1", "1.2", "2")
  - Database IDs kept internal (never exposed to users)
  - Update existing demo data with WBS codes

- [COMPLETED] **WBS Generation Service (REQUIREMENT 12)**
  - WbsCodeGenerationService for auto-generation
  - Auto-renumbering on hierarchy changes
  - WBS validation and uniqueness enforcement
  - Integration with existing GanttTaskService
  - Interactive demo page for validation

#### Milestone 1.2: I18N Foundation Setup (NEXT)
**Why Early**: Better to build features with I18N from start than retrofit
- [NEXT] **I18N Infrastructure (REQUIREMENT 14)**
  - Resource files for English/Chinese (Simplified)
  - IStringLocalizer integration in existing components
  - Localized datetime formatting setup
  - Fallback to English for missing translations
  - Update existing TaskGrid with I18N

#### Milestone 1.3: Enhanced TaskGrid with WBS Support
- [ ] **Basic Grid Enhancement (REQUIREMENT 9)**
  - Add WBS code column as primary identifier 
  - Replace ID display with WBS codes (simple column swap)
  - Keep existing tree structure and selection working
  - NO new features - just WBS integration

#### Milestone 1.4: Minimal Timeline Component (Prove It Works)
**Goal**: Simplest possible timeline that proves row alignment works
- [ ] **Ultra-Simple SVG Timeline (REQUIREMENT 18)**
  - Fixed scale: 40px/day (no zoom initially)
  - Simple month/day header (no multi-tier complexity)
  - Basic rectangles for task bars with WBS labels
  - UTC date handling (REQUIREMENT 2) - dates only, no time components
  
- [ ] **Basic Row Alignment (REQUIREMENT 6)**
  - Fixed row heights using CSS custom properties
  - Simple row alignment with TaskGrid
  - NO dynamic heights initially (expand/collapse can wait)
  - Prove basic alignment works first

#### Milestone 1.5: Simple Integration (Walking Skeleton)
**Goal**: Basic side-by-side layout that works
- [ ] **Simple Layout (REQUIREMENT 5)**
  - TaskGrid and Timeline side by side (no splitter initially)
  - Fixed proportions (e.g., 50/50 split)
  - Same data in both components
  - Selection sync (basic click synchronization)

- [ ] **Walking Skeleton Validation**
  - End-to-end working with demo data
  - WBS codes displayed consistently
  - I18N working (English/Chinese text switching)
  - Basic performance check (is it smooth?)

### Phase 2: Essential Polish Only (Week 3-4)
**Goal**: Make it look professional and meet accessibility requirements

#### Milestone 2.1: Visual Polish (Keep It Simple)
- [ ] **Material Design Basics (REQUIREMENT 13)**
  - Apply Material colors and spacing
  - Clean typography (no fancy animations yet)
  - Basic hover effects
  - Make it look professional but simple

- [ ] **Timeline Essentials**
  - Task progress bars (filled rectangles)
  - Basic hover tooltips with WBS codes
  - Simple selection highlighting
  - NO zoom, NO pan initially

#### Milestone 2.2: Accessibility & Core Features
- [ ] **Accessibility Compliance (REQUIREMENT 20)**
  - ARIA labels and roles
  - Keyboard navigation for core features
  - Focus indicators
  - Color contrast compliance

- [ ] **TaskGrid Essentials**
  - Basic sorting (one column at a time)
  - Simple filtering (text search)
  - Keyboard navigation for core features
  - NO complex features initially

### Phase 3: Essential Gantt Features (Week 5-6)
**Goal**: Add essential features that make it a proper Gantt chart replacement

#### Milestone 3.1: Enhanced Dependencies & Timeline
- [ ] **All Dependency Types (REQUIREMENT 8 - ESSENTIAL)**
  - Support all 4 types: FS, SS, FF, SF
  - Enhanced dependency line rendering
  - Day-level offsets: "+3d", "-2d" (day units only)
  - Circular dependency detection

- [ ] **Timeline Interactions (REQUIREMENT 10 - ESSENTIAL)**
  - Multiple zoom levels (Day, Week, Month, Quarter) - NO hour-level zoom
  - Pan and zoom interactions (mouse wheel, drag)
  - Task bar drag and resize capabilities using day increments only
  - Snap to day boundaries

#### Milestone 3.2: Enhanced TaskGrid Features
- [ ] **Advanced Grid Features (REQUIREMENT 9 - ESSENTIAL)**
  - Column resizing, reordering, show/hide
  - Advanced sorting and filtering capabilities
  - Multi-column sorting
  - Search and filter persistence

#### Milestone 3.3: Resource Management Foundation
- [ ] **Basic Resource Support (REQUIREMENT 8 - ESSENTIAL)**
  - Resource data model and assignments
  - Resource assignment to tasks
  - Basic resource utilization display
  - WBS-based resource assignment tracking

### Phase 4: Advanced Features & Export (Week 7-8)
**Goal**: Complete the Syncfusion replacement with advanced features

#### Milestone 4.1: Critical Path & Baselines
- [ ] **Advanced Scheduling (REQUIREMENT 8 - ESSENTIAL)**
  - Critical path calculation and highlighting
  - Baseline support (planned vs actual)
  - Auto-scheduling based on dependencies
  - Task slack time computation

#### Milestone 4.2: Professional Export (REQUIREMENT 22 - ESSENTIAL)
- [ ] **Vector PDF Export**
  - SVG → PDF pipeline for scalable output
  - Multiple page sizes (A4, Letter, Legal, Custom)
  - Portrait/Landscape orientation support
  - Export configuration options

#### Milestone 4.3: Performance & Polish
- [ ] **Performance Optimization**
  - Test with 200-500 tasks (realistic dataset)
  - Smooth interactions and memory management
  - Cross-browser compatibility testing
  - Bundle size optimization (target <200KB)

### Phase 5: Future Enhancements (Post-MVP)
**Goal**: Advanced features for enterprise use

#### Future Considerations (Not Required for Initial Success)
- [ ] **Large Dataset Support** (1000+ tasks if needed)
- [ ] **Virtual Scrolling** (if performance becomes issue)
- [ ] **Advanced Animations** and polish
- [ ] **Context Menus** and additional UX features
- [ ] **Advanced Export Options** (Excel, CSV, etc.)
- [ ] **Custom Calendar Support**

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
- **date-fns** - Lightweight, immutable, tree-shakeable (day precision only)
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
- [ ] **Day-Level Offsets**: "+3d", "-2d" syntax for delays/leads (day precision only)
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
- [ ] **Day-Week-Month Scales**: Day precision only, no hour/minute/second precision
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
    public DateTime StartDate { get; set; } // UTC, day precision only
    public DateTime EndDate { get; set; }   // UTC, day precision only
    public string Duration { get; set; } = ""; // "5d" or "5D" format only
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
**Phase 1**: WBS codes, UTC date handling, and day-level precision from the beginning
**Phase 2**: I18N support and Material Design compliance
**Phase 3**: Pixel-perfect row alignment and component integration
**Phase 4**: Performance targets and export functionality

**Requirements Benefits**:
- **Day-level precision**: Faster development, simpler testing, reduced complexity
- **UTC date storage**: No timezone bugs, simpler date calculations
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
  - Keyboard navigation for core features

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

## 🚀 Implementation Starting Strategy - REVISED

### Week 1: Foundation First (Simple & Focused)
1. **Day 1-2**: WBS Code Foundation
   - Add WbsCode to GanttTask model
   - Create simple WbsCodeGenerationService (basic logic)
   - Update demo data with WBS hierarchy (5-10 tasks max)
   - Replace ID with WBS in existing TaskGrid display

2. **Day 3-4**: I18N Foundation Setup
   - Resource files for English/Chinese (10-20 key strings)
   - IStringLocalizer integration (basic setup)
   - Update existing TaskGrid with localized column headers
   - Test language switching works

3. **Day 5-7**: Enhanced TaskGrid with WBS
   - Add WBS code column (replace ID column)
   - Ensure all UI shows WBS codes
   - Keep everything else the same (no new features)
   - Test with demo data

### Week 2: Minimal Working Gantt (Prove It Works)
1. **Day 1-3**: Basic Timeline Component
   - Simple SVG with fixed 40px/day scale
   - Basic month/day header (no fancy multi-tier)
   - Rectangle task bars with WBS labels
   - Fixed row heights matching TaskGrid

2. **Day 4-5**: Simple Side-by-Side Layout
   - TaskGrid and Timeline next to each other (fixed 50/50)
   - NO splitter initially (keep it simple)
   - Basic selection sync (click one, highlight other)
   - Same demo data in both

3. **Day 6-7**: Walking Skeleton Validation
   - End-to-end working Gantt chart
   - WBS codes everywhere
   - English/Chinese switching works
   - Performance test with 20-50 tasks max

### Success Criteria for Week 1-2 (Phase 1 MVP)
- ✅ WBS codes working (no database IDs visible)
- ✅ Basic I18N working (English/Chinese toggle)
- ✅ TaskGrid + Timeline side by side
- ✅ Same data displayed in both components
- ✅ Basic row alignment working
- ✅ Smooth performance with 20-50 tasks

**After Week 2**: We have a minimal but working Gantt chart that proves the concept!

### Success Criteria for Week 3-4 (Phase 2 Enhancement)
- ✅ Professional Material Design appearance
- ✅ Basic accessibility compliance
- ✅ Simple dependencies (FS only)
- ✅ Basic drag & drop functionality
- ✅ Essential timeline features (zoom, pan)

### Success Criteria for Week 5-8 (Phase 3-4 Essential Features)
- ✅ All 4 dependency types working
- ✅ Resource management implemented
- ✅ Critical path and baseline support
- ✅ Advanced grid features (resize, reorder, filter)
- ✅ Vector PDF export capability
- ✅ Ready to replace Syncfusion for day-level project management

**After Week 8**: Complete Gantt chart replacement with all essential features!

---

## 🎯 **PLAN REVISIONS & RATIONALE**

### **Why These Changes Improve Success Rate**

#### **1. WBS Codes First Strategy**
- **Original Problem**: WBS codes buried in complex milestone, treated as minor feature
- **New Approach**: WBS codes as Milestone 1.1 - fundamental to everything
- **Benefits**: All subsequent features built with WBS codes correctly from start
- **Risk Mitigation**: Avoids massive refactoring to retrofit WBS codes later

#### **2. Walking Skeleton Approach**  
- **Original Problem**: No end-to-end integration until Phase 3 (weeks away)
- **New Approach**: Basic integration working by end of Week 2
- **Benefits**: Proves architecture early, finds integration issues fast
- **Risk Mitigation**: Architecture problems discovered in weeks, not months

#### **3. I18N Foundation Early**
- **Original Problem**: I18N spread across multiple phases, retrofit approach
- **New Approach**: I18N foundation in Week 1, features built with I18N
- **Benefits**: No text strings hardcoded, localization natural from start
- **Risk Mitigation**: Avoids expensive I18N retrofitting

#### **4. Simplified Technology Stack**
- **Original Problem**: Many external dependencies without justification
- **New Approach**: Browser standards first, libraries only when needed
- **Benefits**: Smaller bundle, fewer dependencies, more control
- **Risk Mitigation**: Less complexity, easier debugging, fewer breaking changes

#### **5. Realistic Phase Boundaries**
- **Original Problem**: Phase 1 included complex features like full dependency rendering
- **New Approach**: Phase 1 focuses on solid foundation only
- **Benefits**: Each phase builds on proven, working foundation
- **Risk Mitigation**: No "big bang" integration, continuous working software

### **Requirements Compliance Maintained**
All 27 requirements from REQUIREMENTS.md still satisfied:
- ✅ **Day-level precision**: Maintained throughout
- ✅ **UTC date storage**: Foundation established early  
- ✅ **WBS codes**: Now first-class citizen from start
- ✅ **No batch operations**: Architecture supports this
- ✅ **I18N English/Chinese**: Foundation established early
- ✅ **Performance targets**: Validated incrementally
- ✅ **Material Design**: Applied from basic components

### **Development Risk Reduction Through Simplification**
- **Technical Risk**: Simple fixed layouts vs complex responsive/resizable
- **Feature Risk**: Basic features working well vs many features working poorly
- **Integration Risk**: Fixed positioning vs dynamic row alignment complexity
- **Scope Risk**: Clear "good enough" boundaries vs feature creep
- **Timeline Risk**: Working software in weeks vs months of development

### **Features We're Deliberately Deferring to Later Phases (Essential But Not Phase 1)**

#### **Core Gantt Features (Phase 2-3 - MUST HAVE in Final Product)**
- 🔄 **All 4 dependency types** (FS, SS, FF, SF) - Phase 2
- 🔄 **Resource management and assignment** - Phase 3  
- 🔄 **Critical path calculation** - Phase 3
- 🔄 **Baseline support** (planned vs actual) - Phase 3
- 🔄 **Multiple zoom levels** (hour to quarter) - Phase 2
- 🔄 **Pan and zoom interactions** - Phase 2
- 🔄 **Task bar drag and resize** - Phase 2
- 🔄 **Dependency line rendering** - Phase 2
- 🔄 **Column resizing, reordering, show/hide** - Phase 2
- 🔄 **Advanced sorting and filtering** - Phase 2
- 🔄 **Vector PDF export** - Phase 3
- 🔄 **Export configuration options** - Phase 3

#### **Features We're Removing/Simplifying (Not Essential)**
- ❌ **Large dataset support** (1000+ tasks) - removed, focus on 50-200 tasks
- ❌ **Virtual scrolling** - removed, use standard scrolling
- ❌ **60fps performance targets** - removed, focus on "smooth enough"
- ❌ **<100KB bundle size** - removed, focus on functionality first
- ❌ **Complex animations** - basic hover effects only
- ❌ **Context menus** - defer to later phases

#### **Phase 1 Focus (Minimal Viable Gantt)**
- ✅ **Fixed timeline scale** - 40px/day initially
- ✅ **Fixed 50/50 layout** - no splitter initially  
- ✅ **Fixed row heights** - no dynamic sizing initially
- ✅ **Basic FS dependencies only** - simple straight lines
- ✅ **Basic drag left/right** - no resize initially
- ✅ **Basic print CSS** - no vector PDF initially
- ✅ **Simple sorting** - one column at a time
- ✅ **Basic selection sync** - click synchronization

### **What We're Keeping (Requirements-Driven)**
- ✅ **WBS codes everywhere** (REQUIREMENT 12)
- ✅ **Day-level precision** (REQUIREMENT 1)
- ✅ **UTC timestamps** (REQUIREMENT 2)
- ✅ **Single operations** (REQUIREMENT 3)
- ✅ **English/Chinese I18N** (REQUIREMENT 14)
- ✅ **Material Design basics** (REQUIREMENT 13)
- ✅ **Accessibility fundamentals** (REQUIREMENT 20)
- ✅ **Basic performance** (REQUIREMENT 16)
- ✅ **Row alignment** (REQUIREMENT 6)
- ✅ **Independent components** (REQUIREMENT 5)

This revised plan significantly improves our probability of successful delivery by focusing on "good enough" rather than "enterprise complexity".

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
