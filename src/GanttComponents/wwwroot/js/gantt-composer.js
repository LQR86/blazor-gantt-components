// JavaScript functions for GanttComposer coordinate-based layout and scroll synchronization
window.ganttComposer = {
    // Active splitter state
    activeSplitter: null,
    
    // Initialize coordinate-based splitter functionality
    initializeCoordinateSplitter: function(composerId, initialGridWidth, splitterWidth, minGridWidth, maxGridWidthRatio) {
        const composer = document.getElementById(composerId);
        if (!composer) return;
        
        const splitter = composer.querySelector('.composer-splitter');
        const taskGrid = composer.querySelector('.composer-grid');
        const timeline = composer.querySelector('.composer-timeline');
        
        if (!splitter || !taskGrid || !timeline) return;
        
        // Store references and configuration
        const config = {
            composer: composer,
            splitter: splitter,
            taskGrid: taskGrid,
            timeline: timeline,
            splitterWidth: splitterWidth,
            minGridWidth: minGridWidth,
            maxGridWidthRatio: maxGridWidthRatio,
            gridWidth: initialGridWidth
        };
        
        splitter.addEventListener('mousedown', (e) => {
            e.preventDefault();
            
            // Get current grid width from DOM instead of initial config
            const currentGridWidth = taskGrid.offsetWidth;
            
            this.activeSplitter = {
                ...config,
                startX: e.clientX,
                startGridWidth: currentGridWidth, // Use actual current width
                containerWidth: composer.offsetWidth,
                gridWidth: currentGridWidth // Update current width
            };
            
            // Add visual feedback
            splitter.classList.add('dragging');
            document.body.style.cursor = 'ew-resize';
            document.body.style.userSelect = 'none';
            document.body.style.webkitUserSelect = 'none';
            
            // Attach document-level events
            document.addEventListener('mousemove', this.handleCoordinateSplitterMove);
            document.addEventListener('mouseup', this.handleCoordinateSplitterEnd);
        });
        
        // Initial layout update
        this.updateCoordinateLayout(config);
    },
    
    // Handle mouse move during coordinate-based splitter drag
    handleCoordinateSplitterMove: function(e) {
        if (!window.ganttComposer.activeSplitter) return;
        
        const { startX, startGridWidth, containerWidth, minGridWidth, maxGridWidthRatio } = window.ganttComposer.activeSplitter;
        const deltaX = e.clientX - startX;
        const newGridWidth = startGridWidth + deltaX;
        
        // Constrain width
        const maxGridWidth = containerWidth * maxGridWidthRatio;
        const clampedWidth = Math.max(minGridWidth, Math.min(maxGridWidth, newGridWidth));
        
        // Update component configuration
        window.ganttComposer.activeSplitter.gridWidth = clampedWidth;
        
        // Update layout with new coordinates immediately
        window.ganttComposer.updateCoordinateLayout(window.ganttComposer.activeSplitter);
        
        // Skip Blazor updates during drag to prevent conflicts
        // The final position will be updated on mouseup
    },
    
    // Handle mouse up (end coordinate-based drag)
    handleCoordinateSplitterEnd: function(e) {
        if (!window.ganttComposer.activeSplitter) return;
        
        // Get final width
        const finalWidth = window.ganttComposer.activeSplitter.gridWidth;
        
        // Remove visual feedback
        window.ganttComposer.activeSplitter.splitter.classList.remove('dragging');
        document.body.style.cursor = '';
        document.body.style.userSelect = '';
        document.body.style.webkitUserSelect = '';
        
        // Update Blazor component with final position
        try {
            const blazorComponent = window.ganttComposer.getBlazorComponentReference(window.ganttComposer.activeSplitter.composer);
            if (blazorComponent) {
                blazorComponent.invokeMethodAsync('UpdateGridWidth', finalWidth);
            }
        } catch (error) {
            console.warn('Blazor interop failed during drag end:', error);
        }
        
        // Clean up
        document.removeEventListener('mousemove', window.ganttComposer.handleCoordinateSplitterMove);
        document.removeEventListener('mouseup', window.ganttComposer.handleCoordinateSplitterEnd);
        window.ganttComposer.activeSplitter = null;
    },
    
    // Update coordinate-based layout positions
    updateCoordinateLayout: function(config) {
        const { taskGrid, splitter, timeline, gridWidth, splitterWidth } = config;
        
        // Position TaskGrid at (0, 0) with natural width
        taskGrid.style.left = '0px';
        taskGrid.style.width = `${gridWidth}px`;
        
        // Position splitter at TaskGrid boundary
        splitter.style.left = `${gridWidth}px`;
        
        // Position Timeline after splitter, taking remaining space
        timeline.style.left = `${gridWidth + splitterWidth}px`;
        timeline.style.right = '0px'; // Take remaining space naturally
    },
    
    // Get Blazor component reference (simplified approach)
    getBlazorComponentReference: function(composerElement) {
        // This is a simplified version - in a real implementation, you might need
        // to store the DotNetObjectReference during initialization
        return null; // For now, we'll rely on direct coordinate updates
    },
    // Synchronize vertical scrolling between TaskGrid and TimelineView
    synchronizeVerticalScroll: function(gridElementId, timelineElementId) {
        const gridScrollContainer = document.querySelector(`#${gridElementId} .task-grid-body`);
        const timelineScrollContainer = document.querySelector(`#${timelineElementId} .timeline-scroll-container`);
        
        if (gridScrollContainer && timelineScrollContainer) {
            // TaskGrid scroll affects TimelineView
            gridScrollContainer.addEventListener('scroll', function() {
                if (!timelineScrollContainer.dataset.syncing) {
                    timelineScrollContainer.dataset.syncing = 'true';
                    timelineScrollContainer.scrollTop = gridScrollContainer.scrollTop;
                    setTimeout(() => delete timelineScrollContainer.dataset.syncing, 10);
                }
            });
            
            // TimelineView scroll affects TaskGrid
            timelineScrollContainer.addEventListener('scroll', function() {
                if (!gridScrollContainer.dataset.syncing) {
                    gridScrollContainer.dataset.syncing = 'true';
                    gridScrollContainer.scrollTop = timelineScrollContainer.scrollTop;
                    setTimeout(() => delete gridScrollContainer.dataset.syncing, 10);
                }
            });
            
            console.log('Scroll synchronization initialized between:', gridElementId, 'and', timelineElementId);
        } else {
            console.warn('Could not find scroll containers for synchronization:', {
                gridContainer: gridScrollContainer,
                timelineContainer: timelineScrollContainer
            });
        }
    },
    
    // Set up row alignment debugging
    toggleRowAlignmentDebug: function(composerId, show) {
        const composer = document.getElementById(composerId);
        if (composer) {
            if (show) {
                composer.classList.add('debug-alignment');
            } else {
                composer.classList.remove('debug-alignment');
            }
        }
    },
    
    // Legacy function for backward compatibility (no longer needed with parameter-based alignment)
    enforceRowAlignment: function(composerId) {
        // Parameter-based alignment handles this automatically
        // This function is kept for compatibility but does nothing
        console.log('Row alignment is now handled via component parameters');
    },
    
    // Center a task bar horizontally in the TimelineView viewport
    scrollTaskBarToCenter: function(timelineElementId, taskStartDate, startDate, dayWidth) {
        const container = document.querySelector(`#${timelineElementId} .timeline-scroll-container`);
        if (!container) {
            console.warn('Could not find timeline scroll container for centering:', timelineElementId);
            return;
        }
        
        try {
            // Calculate task bar X position (same logic as DayToPixel in C#)
            const taskDate = new Date(taskStartDate);
            const baseDate = new Date(startDate);
            const days = (taskDate - baseDate) / (1000 * 60 * 60 * 24);
            const taskX = days * dayWidth;
            
            // Center the task bar in viewport
            const containerWidth = container.clientWidth;
            const centerOffset = containerWidth / 2;
            const scrollLeft = taskX - centerOffset;
            
            // Smooth scroll to center the task bar
            container.scrollTo({
                left: Math.max(0, scrollLeft),
                behavior: 'smooth'
            });
            
            console.log('Centered task bar at X position:', taskX, 'with scroll left:', Math.max(0, scrollLeft));
        } catch (error) {
            console.error('Error centering task bar:', error);
        }
    }
};
