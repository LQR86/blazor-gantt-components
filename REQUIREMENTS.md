# Custom Gantt Components - Core Requirements

## 🎯 Overview
This document captures the **immutable requirements** for our custom Gantt components. These requirements are **non-negotiable** and must be satisfied regardless of architecture changes, implementation approaches, or development phases.

---

## 🔒 **CRITICAL DESIGN CONSTRAINTS**

### 1. **Day-Level Scheduling ONLY**
- ✅ **MUST**: Maximum hour-level granularity, NO minute/second precision
- ✅ **MUST**: All date calculations use day boundaries
- ✅ **MUST**: Timeline scales limited to: Hour, Day, Week, Month, Quarter
- ✅ **MUST**: Duration format: "5d" or "8h" (no minutes/seconds)
- ✅ **MUST**: Dependency offsets in day units: "+3d", "-2d"

**Rationale**: Eliminates complexity while serving 99% of project management use cases

### 2. **UTC Timestamps Only by Choice**
- ✅ **MUST**: All timestamps stored and processed in UTC only
- ✅ **MUST**: No timezone conversion or timezone-aware calculations
- ✅ **MUST**: User interface displays dates in user's local timezone for viewing only
- ✅ **MUST**: All date inputs converted to UTC immediately upon entry
- ✅ **MUST**: No timezone selection or timezone management features
- ✅ **MUST**: Simple date arithmetic without timezone complications

**Rationale**: Eliminates timezone complexity, bugs, and edge cases while maintaining global usability through local display conversion

### 3. **No Batch Operations by Choice**
- ✅ **MUST**: Single-operation CRUD only (no batch editing)
- ✅ **MUST**: Immediate feedback for each action
- ✅ **MUST**: Real-time server synchronization (no batch mode)
- ✅ **MUST**: No bulk dependency creation
- ✅ **MUST**: No batch edit modes in grid

**Rationale**: Simpler UX, easier implementation, immediate feedback, reduced complexity

### 4. **Complete Control Over Implementation**
- ✅ **MUST**: No dependency on Syncfusion or other third-party Gantt libraries
- ✅ **MUST**: Full source code ownership and control
- ✅ **MUST**: Ability to customize every aspect of behavior and styling
- ✅ **MUST**: No "mysterious blackboxes" from external vendors

**Rationale**: Avoid vendor lock-in, eliminate debugging third-party issues

---

## 🏗️ **ARCHITECTURAL REQUIREMENTS**

### 5. **Independent Component Design**
- ✅ **MUST**: TaskGrid component works standalone
- ✅ **MUST**: TimelineView component works standalone  
- ✅ **MUST**: Components can be composed together via GanttComposer
- ✅ **MUST**: Each component has its own complete API
- ✅ **MUST**: No tight coupling between components

**Rationale**: Flexibility, reusability, easier testing and maintenance

### 6. **Pixel-Perfect Row Alignment**
- ✅ **MUST**: TaskGrid rows align perfectly with TimelineView task bars
- ✅ **MUST**: Alignment maintained during scroll, zoom, resize
- ✅ **MUST**: Alignment preserved during tree expand/collapse
- ✅ **MUST**: No visual row misalignment under any circumstances
- ✅ **MUST**: Dedicated row alignment service/system

**Rationale**: Row misalignment breaks the entire Gantt user experience

### 7. **UI-First Development Approach**
- ✅ **MUST**: Focus on user interface and interactions first
- ✅ **MUST**: Visual design and UX before complex logic
- ✅ **MUST**: Material Design principles and styling
- ✅ **MUST**: Responsive design for different screen sizes
- ✅ **MUST**: Smooth animations and visual feedback

**Rationale**: User experience is the primary differentiator

---

## 📊 **SYNCFUSION FEATURE PARITY**

### 8. **Core Gantt Functionality**
- ✅ **MUST**: Hierarchical task tree with expand/collapse
- ✅ **MUST**: Task dependencies (FS, SS, FF, SF types)
- ✅ **MUST**: Resource management and assignment
- ✅ **MUST**: Progress tracking and visualization
- ✅ **MUST**: Critical path calculation and highlighting
- ✅ **MUST**: Baseline support (planned vs actual)

### 9. **Grid Functionality**
- ✅ **MUST**: Inline cell editing with validation
- ✅ **MUST**: Column resizing, reordering, show/hide
- ✅ **MUST**: Sorting and filtering capabilities
- ✅ **MUST**: Selection (single/multiple rows)
- ✅ **MUST**: Keyboard navigation support

### 10. **Timeline Functionality**
- ✅ **MUST**: SVG-based timeline rendering
- ✅ **MUST**: Multiple zoom levels (hour to quarter)
- ✅ **MUST**: Pan and zoom interactions
- ✅ **MUST**: Task bar drag and resize
- ✅ **MUST**: Dependency line rendering
- ✅ **MUST**: Today indicator and working time backgrounds

### 11. **Data Management**
- ✅ **MUST**: Support for large datasets (1000+ tasks)
- ✅ **MUST**: Virtual scrolling for performance
- ✅ **MUST**: Real-time data binding
- ✅ **MUST**: Three-table relationship (Tasks, Resources, Assignments)
- ✅ **MUST**: Data validation and constraint checking

### 12. **WBS Code Task Identification**
- ✅ **MUST**: WBS codes as the only user-facing task identifiers
- ✅ **MUST**: Hierarchical WBS structure (e.g., "1", "1.1", "1.1.1", "1.2", "2")
- ✅ **MUST**: Auto-generation of WBS codes based on task hierarchy
- ✅ **MUST**: WBS codes visible in all user interfaces (grid, timeline, exports)
- ✅ **MUST**: WBS codes used in dependency definitions (e.g., "1.2FS+3d")
- ✅ **MUST**: Database IDs kept internal and never exposed to users
- ✅ **MUST**: WBS code validation and uniqueness enforcement
- ✅ **MUST**: WBS renumbering when task hierarchy changes

**Rationale**: WBS codes provide meaningful, hierarchical identifiers that project managers understand, while database IDs are technical implementation details that should remain hidden from users.

---

## 🎨 **DESIGN & STYLING REQUIREMENTS**

### 13. **Material Design Compliance**
- ✅ **MUST**: Material Design color palette and spacing
- ✅ **MUST**: Material Design typography and iconography
- ✅ **MUST**: CSS custom properties for theming
- ✅ **MUST**: Consistent elevation and shadow system
- ✅ **MUST**: Material motion and animation principles

### 14. **Internationalization (I18N) Support**
- ✅ **MUST**: Support for English and Chinese (Simplified) languages only
- ✅ **MUST**: All user-visible text externalized to resource files
- ✅ **MUST**: Support for left-to-right (LTR) text direction only (no RTL)
- ✅ **MUST**: Datetime format localization (US format vs Chinese format)
- ✅ **MUST**: Common metrics and units localization (duration units, percentages)
- ✅ **MUST**: Resource key management system for translatable strings
- ✅ **MUST**: Fallback to English for missing Chinese translations
- ✅ **MUST**: No hardcoded text strings in component code
- ❌ **NOT**: Full cultural localization (business logic, data formats remain consistent)
- ❌ **NOT**: Support for languages other than English and Chinese (Simplified)

**Rationale**: Focused on English/Chinese markets with datetime and metrics localization for user familiarity, while keeping business logic standardized

### 15. **Visual Standards**
- ✅ **MUST**: Clean, modern interface design
- ✅ **MUST**: Consistent spacing and alignment
- ✅ **MUST**: Professional appearance suitable for enterprise
- ✅ **MUST**: Clear visual hierarchy and information design
- ✅ **MUST**: Accessible color contrast and typography

---

## ⚡ **PERFORMANCE REQUIREMENTS**

### 16. **Performance Targets**
- ✅ **MUST**: TaskGrid handles 1000+ rows with smooth scrolling
- ✅ **MUST**: TimelineView renders 500+ tasks at 60fps
- ✅ **MUST**: Stable memory usage during interactions
- ✅ **MUST**: Bundle size <100KB gzipped for core components
- ✅ **MUST**: Fast initial load and responsive interactions

### 17. **Technical Performance**
- ✅ **MUST**: Efficient DOM updates and rendering
- ✅ **MUST**: Virtual scrolling implementation
- ✅ **MUST**: Optimized paint and layout operations
- ✅ **MUST**: Smooth animations using RAF
- ✅ **MUST**: Memory management and cleanup

---

## 🔧 **TECHNICAL REQUIREMENTS**

### 18. **Technology Stack**
- ✅ **MUST**: Blazor Server as the primary framework
- ✅ **MUST**: CSS Grid and Flexbox for layouts
- ✅ **MUST**: SVG for timeline graphics
- ✅ **MUST**: Standard web technologies (no exotic dependencies)
- ✅ **MUST**: .NET 8.0 target framework

### 19. **Code Quality**
- ✅ **MUST**: Clean, maintainable, well-documented code
- ✅ **MUST**: Comprehensive testing strategy
- ✅ **MUST**: TypeScript-like strong typing in C#
- ✅ **MUST**: SOLID principles and clean architecture
- ✅ **MUST**: Performance monitoring and optimization

---

## 🛡️ **QUALITY & COMPLIANCE**

### 20. **Accessibility**
- ✅ **MUST**: WCAG AA compliance
- ✅ **MUST**: Screen reader support
- ✅ **MUST**: Keyboard navigation for all features
- ✅ **MUST**: Proper ARIA labels and roles
- ✅ **MUST**: Focus indicators and logical tab order

### 21. **Browser Compatibility**
- ✅ **MUST**: Modern browser support (Chrome, Firefox, Safari, Edge)
- ✅ **MUST**: Responsive design for different screen sizes
- ✅ **MUST**: Cross-platform compatibility
- ✅ **MUST**: Consistent behavior across browsers

---

## 📤 **EXPORT & INTEGRATION**

### 22. **Export Capabilities**
- ✅ **MUST**: PDF export with vector graphics
- ✅ **MUST**: Print-optimized layouts
- ✅ **MUST**: Multiple page sizes and orientations
- ✅ **MUST**: Export configuration options
- ✅ **MUST**: High-quality output suitable for presentations

### 23. **Integration Requirements**
- ✅ **MUST**: Easy integration into existing Blazor applications
- ✅ **MUST**: Well-defined component APIs
- ✅ **MUST**: Event-driven architecture for extensibility
- ✅ **MUST**: Configuration and customization options
- ✅ **MUST**: Documentation and examples

---

## 📈 **DEVELOPMENT REQUIREMENTS**

### 24. **Development Process**
- ✅ **MUST**: Phase-based development (TaskGrid → TimelineView → Integration)
- ✅ **MUST**: UI-first approach with immediate visual feedback
- ✅ **MUST**: Incremental delivery of working features
- ✅ **MUST**: Regular testing and validation
- ✅ **MUST**: Version control with meaningful commit messages

### 25. **DevOps & CI/CD**
- ✅ **MUST**: GitHub version control with comprehensive workflows
- ✅ **MUST**: Automated build, test, and deployment
- ✅ **MUST**: Security scanning and dependency management
- ✅ **MUST**: Performance monitoring and regression testing
- ✅ **MUST**: Quality gates and code review processes

---

## 🎯 **SUCCESS CRITERIA**

### 26. **Primary Goals**
- ✅ **MUST**: Replace Syncfusion Gantt with zero feature regression
- ✅ **MUST**: Eliminate vendor dependency and licensing costs
- ✅ **MUST**: Provide superior user experience and performance
- ✅ **MUST**: Enable unlimited customization and extension
- ✅ **MUST**: Deliver enterprise-grade stability and reliability

### 27. **Long-term Vision**
- ✅ **MUST**: Serve as foundation for future Gantt-related features
- ✅ **MUST**: Demonstrate feasibility of custom component development
- ✅ **MUST**: Create reusable pattern for other complex UI components
- ✅ **MUST**: Establish technical leadership in custom component development

---

## ⚠️ **NON-NEGOTIABLE CONSTRAINTS**

### What We Will NOT Do:
- ❌ **NO** minute/second precision scheduling
- ❌ **NO** batch operations or bulk editing modes  
- ❌ **NO** dependency on Syncfusion or similar libraries
- ❌ **NO** compromise on row alignment quality
- ❌ **NO** complex frameworks that add unnecessary complexity
- ❌ **NO** performance compromises for large datasets
- ❌ **NO** accessibility or browser compatibility shortcuts
- ❌ **NO** exposing database IDs to users (WBS codes only)
- ❌ **NO** timezone management or timezone-aware calculations
- ❌ **NO** full cultural localization (business logic remains standardized)
- ❌ **NO** support for languages other than English and Chinese (Simplified)

### What We Will ALWAYS Do:
- ✅ **ALWAYS** prioritize user experience and visual design
- ✅ **ALWAYS** maintain pixel-perfect row alignment
- ✅ **ALWAYS** provide immediate feedback for user actions
- ✅ **ALWAYS** follow Material Design principles
- ✅ **ALWAYS** ensure enterprise-grade performance and reliability
- ✅ **ALWAYS** use WBS codes as user-facing task identifiers
- ✅ **ALWAYS** store and process timestamps in UTC only
- ✅ **ALWAYS** support UI label translation for English and Chinese (Simplified)
- ✅ **ALWAYS** localize datetime formats and common metrics for user familiarity

---

## 📝 **VALIDATION CHECKLIST**

Before considering any requirement "complete":
- [ ] Tested with 1000+ task dataset
- [ ] Verified pixel-perfect row alignment in all scenarios
- [ ] Confirmed Material Design compliance
- [ ] Validated performance targets
- [ ] Tested accessibility compliance
- [ ] Verified cross-browser compatibility
- [ ] Confirmed export functionality
- [ ] Validated integration capabilities

---

**This document represents the immutable foundation of our custom Gantt components. Any proposed changes must be evaluated against these requirements, and all requirements must be satisfied in the final implementation.**
