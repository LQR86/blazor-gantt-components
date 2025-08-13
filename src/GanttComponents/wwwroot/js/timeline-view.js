// TimelineView Component JavaScript - Immediate Scroll Synchronization
// Eliminates header "tear apart" issue during horizontal scrolling

window.timelineView = {
    
    // Initialize immediate scroll synchronization for a timeline instance
    initializeScrollSync: function(timelineElementId) {
        const timelineContainer = document.querySelector(`#${timelineElementId}`);
        if (!timelineContainer) {
            console.warn('TimelineView: Could not find timeline container:', timelineElementId);
            return;
        }
        
        const headerContainer = timelineContainer.querySelector('.timeline-header-container');
        const bodyContainer = timelineContainer.querySelector('.timeline-scroll-container');
        
        if (!headerContainer || !bodyContainer) {
            return;
        }
        
        // Remove any existing event listener to prevent duplicates
        this.removeScrollSync(bodyContainer);
        
        // SOLUTION: Equalize container widths to prevent scroll boundary mismatch
        this.equalizeContainerWidths(headerContainer, bodyContainer);
        
        // Add immediate scroll synchronization (simplified - no constraints needed)
        const scrollHandler = function(event) {
            // Simple 1:1 synchronization now that widths are equalized
            headerContainer.scrollLeft = bodyContainer.scrollLeft;
        };
        
        // Store handler reference for cleanup
        bodyContainer._timelineScrollHandler = scrollHandler;
        
        // Attach immediate event listener (passive for performance)
        bodyContainer.addEventListener('scroll', scrollHandler, { passive: true });
    },
    
    // Equalize header and body container widths to prevent scroll boundary mismatch
    equalizeContainerWidths: function(headerContainer, bodyContainer) {
        // Calculate scrollbar width difference
        const bodyClientWidth = bodyContainer.clientWidth;
        const headerClientWidth = headerContainer.clientWidth;
        const scrollbarWidth = headerClientWidth - bodyClientWidth;
        
        if (scrollbarWidth > 0) {
            // Add padding to header to match body's reduced width due to scrollbar
            headerContainer.style.paddingRight = `${scrollbarWidth}px`;
        }
    },
    
    // Remove scroll synchronization (for cleanup)
    removeScrollSync: function(bodyContainer) {
        if (bodyContainer && bodyContainer._timelineScrollHandler) {
            bodyContainer.removeEventListener('scroll', bodyContainer._timelineScrollHandler);
            delete bodyContainer._timelineScrollHandler;
        }
    },
    
    // Update viewport information (called from Blazor when needed)
    updateViewport: function(timelineElementId, callback) {
        const bodyContainer = document.querySelector(`#${timelineElementId} .timeline-scroll-container`);
        if (!bodyContainer) {
            console.warn('TimelineView: Could not find body container for viewport update');
            return;
        }
        
        const viewport = {
            scrollLeft: bodyContainer.scrollLeft,
            clientWidth: bodyContainer.clientWidth,
            scrollWidth: bodyContainer.scrollWidth
        };
        
        // Invoke Blazor callback with viewport data
        if (callback && callback.invokeMethodAsync) {
            callback.invokeMethodAsync('UpdateViewportFromJS', viewport);
        }
        
        return viewport;
    },
    
    // Smooth scroll to position (for task centering)
    scrollToPosition: function(timelineElementId, scrollLeft, smooth = true) {
        const bodyContainer = document.querySelector(`#${timelineElementId} .timeline-scroll-container`);
        if (!bodyContainer) {
            console.warn('TimelineView: Could not find body container for scroll');
            return;
        }
        
        bodyContainer.scrollTo({
            left: Math.max(0, scrollLeft),
            behavior: smooth ? 'smooth' : 'auto'
        });
    },
    
    // Get current scroll position
    getScrollPosition: function(timelineElementId) {
        const bodyContainer = document.querySelector(`#${timelineElementId} .timeline-scroll-container`);
        if (!bodyContainer) {
            console.warn('TimelineView: Could not find body container for scroll position');
            return 0;
        }
        
        return bodyContainer.scrollLeft;
    },

    /**
     * Centers a task bar in the TimelineView viewport using DOM-based positioning
     * Future-proof viewport-relative centering independent of SVG dimensions
     * @param {string} timelineElementId - The ID of the timeline component container
     * @param {number} taskId - The ID of the task to center
     */
    centerTaskById: function(timelineElementId, taskId) {
        // Find the timeline scroll container
        const container = document.querySelector(`#${timelineElementId} .timeline-scroll-container`);
        if (!container) {
            console.warn(`TimelineView centering: Could not find scroll container for timeline: ${timelineElementId}`);
            return;
        }

        // Find the specific task bar element (not background row) using specific selector
        const taskBar = container.querySelector(`.timeline-task-bar[data-task-id="${taskId}"]`);
        if (!taskBar) {
            console.warn(`TimelineView centering: Could not find task bar for task ID: ${taskId}`);
            return;
        }

        try {
            // Get viewport dimensions
            const containerWidth = container.clientWidth;
            const containerScrollLeft = container.scrollLeft;

            // Get task bar position using DOM bounding rectangles
            const taskBarRect = taskBar.getBoundingClientRect();
            const containerRect = container.getBoundingClientRect();

            // Calculate task bar position relative to container
            const taskBarRelativeLeft = taskBarRect.left - containerRect.left + containerScrollLeft;
            
            // Use task bar LEFT BORDER position for more intuitive centering
            // This aligns the task start date with viewport center rather than task middle
            const taskBarLeftPosition = taskBarRelativeLeft;

            // Calculate scroll position to center the task bar's LEFT BORDER in viewport
            const targetScrollLeft = taskBarLeftPosition - (containerWidth / 2);

            // Smooth scroll to center the task bar
            container.scrollTo({
                left: Math.max(0, targetScrollLeft),
                behavior: 'smooth'
            });

            console.log(`TimelineView centering: Successfully centered task ${taskId} at position ${Math.max(0, targetScrollLeft)}`);

        } catch (error) {
            console.error(`TimelineView centering: Error centering task ${taskId}:`, error);
        }
    }
};
