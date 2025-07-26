# Copilot Instructions for Custom Gantt UI Components

<!-- Use this file to provide workspace-specific custom instructions to Copilot. For more details, visit https://code.visualstudio.com/docs/copilot/copilot-customization#_use-a-githubcopilotinstructionsmd-file -->

## Project Overview
This is a Blazor Server project for building custom Gantt UI components from scratch. The project follows a component-based architecture with independent TaskGrid and TimelineView components that can work standalone or be composed together.

## Key Design Constraints
- **Day-level scheduling only**: Maximum hour-level granularity, no minute/second precision
- **No batch operations**: Single-operation CRUD with immediate feedback
- **Real-time updates**: Immediate server synchronization, no batch mode
- **Material Design**: Use Material Design principles and CSS custom properties

## Architecture Guidelines
- **Independent Components**: TaskGrid and TimelineView should work standalone
- **Row Alignment**: Critical focus on pixel-perfect alignment between grid and timeline
- **SVG-based Timeline**: Use SVG for timeline rendering with day-to-pixel conversion
- **CSS Grid Layout**: Use CSS Grid for TaskGrid performance
- **Virtual Scrolling**: Implement for large datasets

## Component Structure
```
Components/
├── TaskGrid/           # Data grid with tree structure and editing
├── TimelineView/       # SVG-based timeline with task bars and dependencies
└── GanttComposer/      # Integration component for combined experience
```

## Data Model
- **GanttTask**: id, name, startDate (day precision), duration ("5d"/"8h"), dependencies ("2FS+3d")
- **GanttResource**: id, name, maxUnits, cost
- **GanttAssignment**: taskId, resourceId, units, work

## Development Priorities
1. Start with TaskGrid (easier component)
2. Build TimelineView with SVG rendering
3. Implement row alignment system
4. Add dependency rendering and editing
5. Integrate resource management

## Key Features to Implement
- Hierarchical tree structure with expand/collapse
- Dependency types: FS, SS, FF, SF with day-level offsets
- Resource assignment and utilization tracking
- Critical path calculation and highlighting
- Drag & drop editing for taskbars and dependencies
- Export to PDF with vector graphics

## Performance Targets
- TaskGrid: 1000+ rows with smooth scrolling
- TimelineView: 500+ tasks with 60fps rendering
- Memory: Stable usage during interactions
- Bundle Size: <100KB gzipped for core components

## Code Style
- Use CSS custom properties for theming
- Follow Material Design spacing and typography
- Implement proper ARIA labels and keyboard navigation
- Use date-fns for date calculations (day/hour precision only)
- Apply CSS containment for paint/layout optimization

Please refer to `basic_gantt_plan.md` for detailed implementation milestones and architecture decisions.
