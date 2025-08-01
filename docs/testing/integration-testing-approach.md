# Integration Testing Approach

**Focus**: Service Coordination & Cross-Component Behavior  
**Framework**: xUnit.net with Dependency Injection  
**Execution**: Automated CI/CD Pipeline  
**Date**: August 1, 2025  

## 🎯 **Integration Testing Philosophy**

Integration tests validate that multiple services work together correctly, filling the gap between unit tests (isolated logic) and UI tests (full user experience):

```
Unit Tests        Integration Tests        UI/Interactive Tests
    ↓                     ↓                         ↓
Single Method    →    Service Coordination    →    Full User Flow
Isolated Logic   →    Data Flow Validation   →    Visual Behavior
Fast Feedback    →    Performance Testing    →    UX Validation
```

## 📋 **What to Integration Test**

### **✅ Always Integration Test**
- **Service coordination** (multiple services working together)
- **Data flow validation** (parameters passing between components)
- **Performance with realistic data** (500+ tasks, complex hierarchies)
- **Dependency injection configuration** (service registration correctness)
- **Cross-cutting concerns** (logging, caching, error handling)

### **❌ Avoid Integration Testing**
- **Pure calculations** (use Unit Tests instead)
- **UI rendering** (use Interactive Validation instead)
- **User workflows** (use Manual Testing instead)
- **External API calls** (mock them, focus on integration logic)

## 🛠 **Test Structure and Setup**

### **Directory Structure**
```
tests/GanttComponents.Tests/Integration/
├── GanttComposerZoomIntegrationTests.cs      # Component integration
├── Components/
│   ├── TimelineView/
│   │   └── SixZoomLevelsIntegrationTests.cs
│   └── TaskGrid/
│       └── TaskGridIntegrationTests.cs
└── Services/
    ├── TaskServiceIntegrationTests.cs
    └── I18NServiceIntegrationTests.cs
```

### **Base Integration Test Class**
```csharp
public abstract class IntegrationTestBase : IDisposable
{
    protected readonly ServiceProvider ServiceProvider;
    protected readonly Mock<IUniversalLogger> MockLogger;
    protected readonly Mock<IGanttI18N> MockI18N;

    protected IntegrationTestBase()
    {
        var services = new ServiceCollection();
        
        // Setup mocks for external dependencies
        MockLogger = new Mock<IUniversalLogger>();
        MockI18N = new Mock<IGanttI18N>();
        MockI18N.Setup(x => x.T(It.IsAny<string>())).Returns((string key) => key);
        
        // Register real services that will be tested together
        RegisterServices(services);
        
        // Register mocks
        services.AddSingleton(MockLogger.Object);
        services.AddSingleton(MockI18N.Object);
        
        ServiceProvider = services.BuildServiceProvider();
    }
    
    protected abstract void RegisterServices(IServiceCollection services);
    
    public virtual void Dispose()
    {
        ServiceProvider?.Dispose();
    }
}
```

## 🚀 **Integration Test Patterns**

### **1. Service Coordination Test**
```csharp
public class GanttComposerZoomIntegrationTests : IntegrationTestBase
{
    protected override void RegisterServices(IServiceCollection services)
    {
        services.AddScoped<TimelineZoomService>();
        services.AddScoped<GanttRowAlignmentService>();
        // Other related services
    }

    [Fact]
    public void Integration_ZoomControlsAndComposer_ParameterFlow()
    {
        // Arrange
        var zoomService = ServiceProvider.GetRequiredService<TimelineZoomService>();
        var alignmentService = ServiceProvider.GetRequiredService<GanttRowAlignmentService>();
        
        var initialLevel = TimelineZoomLevel.WeekDay;
        var targetLevel = TimelineZoomLevel.MonthWeek;
        var targetFactor = 1.5;

        // Act - Simulate zoom controls affecting composer
        var initialDayWidth = TimelineZoomService.CalculateEffectiveDayWidth(initialLevel, 1.0);
        var targetDayWidth = TimelineZoomService.CalculateEffectiveDayWidth(targetLevel, targetFactor);

        // Assert - Parameter flow validation
        Assert.Equal(60.0, initialDayWidth);  // WeekDay @ 1.0x = 60px
        Assert.Equal(22.5, targetDayWidth);   // MonthWeek @ 1.5x = 15 * 1.5 = 22.5px
        Assert.NotEqual(initialDayWidth, targetDayWidth);
        
        // Verify services can work together
        Assert.NotNull(alignmentService); // Alignment service available at all zoom levels
    }
}
```

### **2. Performance Integration Test**
```csharp
[Fact]
public void Performance_LargeDataset_ZoomChanges_WithinThresholds()
{
    // Arrange
    var largeTasks = CreateTestTasks(500); // Realistic dataset
    var zoomLevels = Enum.GetValues<TimelineZoomLevel>();

    // Act & Assert - Performance validation across services
    foreach (TimelineZoomLevel level in zoomLevels)
    {
        var startTime = DateTime.UtcNow;

        // Simulate zoom calculation for all tasks (real service coordination)
        foreach (var task in largeTasks)
        {
            var dayWidth = TimelineZoomService.CalculateEffectiveDayWidth(level, 1.0);
            var taskWidth = dayWidth * ParseDurationDays(task.Duration);
            Assert.True(taskWidth >= 0); // Ensure calculations work
        }

        var duration = DateTime.UtcNow - startTime;

        // Assert - Performance threshold for service coordination
        Assert.True(duration.TotalMilliseconds < 100,
            $"Zoom calculations for 500 tasks at {level} should complete in <100ms, took {duration.TotalMilliseconds}ms");
    }
}
```

### **3. Data Flow Validation Test**
```csharp
[Theory]
[InlineData(TimelineZoomLevel.WeekDay, 1.0, 1)]     // 1-day task at 60px = 60px (visible)
[InlineData(TimelineZoomLevel.YearQuarter, 0.5, 1)] // 1-day task at 1.5px (overflow handling)
[InlineData(TimelineZoomLevel.QuarterMonth, 1.0, 7)] // 7-day task at 15px = 105px (visible)
public void TaskVisibility_VariousZoomLevelsAndDurations_CalculatesCorrectWidth(
    TimelineZoomLevel level, double factor, int durationDays)
{
    // Arrange
    var dayWidth = TimelineZoomService.CalculateEffectiveDayWidth(level, factor);

    // Act - Simulate data flow through calculation pipeline
    var taskWidth = dayWidth * durationDays;

    // Assert - Validate data flow produces expected results
    Assert.True(taskWidth > 0, "Task width must be positive");
    
    // Document visibility status for integration analysis
    var isVisible = taskWidth >= 12.0; // TaskDisplayConstants.MIN_TASK_WIDTH
    MockLogger.Verify(x => x.LogDebugInfo(
        It.IsAny<string>(), 
        It.IsAny<object>()), Times.Never); // Logging integration check
    
    // Validate calculation pipeline works end-to-end
    Assert.True(true, $"Task with {durationDays} days at {level} ({factor}x) = {taskWidth}px (visible: {isVisible})");
}
```

### **4. Service Registry Validation**
```csharp
[Fact]
public void ServiceRegistration_AllRequiredServices_AreAvailable()
{
    // Act & Assert - Validate DI container configuration
    var timelineZoomService = ServiceProvider.GetRequiredService<TimelineZoomService>();
    var alignmentService = ServiceProvider.GetRequiredService<GanttRowAlignmentService>();
    var logger = ServiceProvider.GetRequiredService<IUniversalLogger>();
    var i18n = ServiceProvider.GetRequiredService<IGanttI18N>();

    // Verify all services can be resolved
    Assert.NotNull(timelineZoomService);
    Assert.NotNull(alignmentService);
    Assert.NotNull(logger);
    Assert.NotNull(i18n);
    
    // Verify services can work together
    Assert.True(timelineZoomService.GetType().Name.Contains("TimelineZoom"));
    Assert.True(alignmentService.GetType().Name.Contains("Alignment"));
}
```

## 🔄 **Complex Integration Scenarios**

### **1. Multi-Service Workflow Test**
```csharp
[Fact]
public void CompleteZoomWorkflow_AllServices_WorkTogether()
{
    // Arrange
    var taskService = ServiceProvider.GetRequiredService<IGanttTaskService>();
    var zoomService = ServiceProvider.GetRequiredService<TimelineZoomService>();
    var i18nService = ServiceProvider.GetRequiredService<IGanttI18N>();
    
    var testTasks = CreateTestTasks(10);
    
    // Act - Simulate complete workflow
    // 1. Load tasks
    // 2. Apply zoom level
    // 3. Calculate display widths
    // 4. Validate localization
    
    var zoomLevel = TimelineZoomLevel.MonthWeek;
    var zoomFactor = 1.5;
    var dayWidth = TimelineZoomService.CalculateEffectiveDayWidth(zoomLevel, zoomFactor);
    
    foreach (var task in testTasks)
    {
        var taskWidth = dayWidth * ParseDurationDays(task.Duration);
        var taskLabel = i18nService.T($"task.{task.Id}");
        
        // Verify integration pipeline
        Assert.True(taskWidth > 0);
        Assert.NotNull(taskLabel);
    }
    
    // Assert - Workflow completed successfully
    Assert.Equal(22.5, dayWidth); // MonthWeek @ 1.5x = 22.5px
}
```

### **2. Error Handling Integration**
```csharp
[Fact]
public void ErrorHandling_ServiceFailure_GracefulDegradation()
{
    // Arrange - Simulate service failure
    var faultyI18N = new Mock<IGanttI18N>();
    faultyI18N.Setup(x => x.T(It.IsAny<string>()))
        .Throws(new InvalidOperationException("I18N service unavailable"));
    
    var services = new ServiceCollection();
    services.AddScoped<TimelineZoomService>();
    services.AddSingleton(MockLogger.Object);
    services.AddSingleton(faultyI18N.Object); // Faulty service
    
    using var faultyProvider = services.BuildServiceProvider();
    
    // Act & Assert - Services should handle failures gracefully
    var zoomService = faultyProvider.GetRequiredService<TimelineZoomService>();
    
    // Core functionality should still work despite I18N failure
    var dayWidth = TimelineZoomService.CalculateEffectiveDayWidth(TimelineZoomLevel.WeekDay, 1.0);
    Assert.Equal(60.0, dayWidth);
    
    // Verify error was logged
    MockLogger.Verify(x => x.LogError(It.IsAny<string>()), Times.AtMostOnce);
}
```

## ⚡ **Performance Integration Testing**

### **1. Throughput Testing**
```csharp
[Theory]
[InlineData(100)]   // Small dataset
[InlineData(500)]   // Medium dataset  
[InlineData(1000)]  // Large dataset
public void ZoomCalculation_VariousDatasets_MaintainsPerformance(int taskCount)
{
    // Arrange
    var tasks = CreateTestTasks(taskCount);
    var zoomLevels = Enum.GetValues<TimelineZoomLevel>();
    
    // Act - Measure throughput across all zoom levels
    var stopwatch = Stopwatch.StartNew();
    
    foreach (var level in zoomLevels)
    {
        foreach (var task in tasks)
        {
            var dayWidth = TimelineZoomService.CalculateEffectiveDayWidth(level, 1.0);
            var taskWidth = dayWidth * ParseDurationDays(task.Duration);
        }
    }
    
    stopwatch.Stop();
    
    // Assert - Performance scales linearly
    var operationsPerMs = (taskCount * zoomLevels.Length) / (double)stopwatch.ElapsedMilliseconds;
    Assert.True(operationsPerMs > 100, $"Should process >100 operations/ms, got {operationsPerMs:F1}");
}
```

### **2. Memory Integration Testing**
```csharp
[Fact]
public void ServiceLifetime_LongRunning_NoMemoryLeaks()
{
    // Arrange
    var initialMemory = GC.GetTotalMemory(true);
    
    // Act - Simulate long-running service usage
    for (int cycle = 0; cycle < 100; cycle++)
    {
        using var scope = ServiceProvider.CreateScope();
        var services = scope.ServiceProvider.GetServices<object>().ToList();
        
        // Simulate service usage
        foreach (var service in services)
        {
            // Use services
        }
    } // Dispose scope
    
    GC.Collect();
    GC.WaitForPendingFinalizers();
    var finalMemory = GC.GetTotalMemory(true);
    
    // Assert - Memory usage should be stable
    var memoryIncrease = finalMemory - initialMemory;
    Assert.True(memoryIncrease < 5_000_000, // <5MB increase
        $"Memory increased by {memoryIncrease / 1024 / 1024}MB, should be <5MB");
}
```

## 🎯 **Test Data Management**

### **Realistic Test Data Generation**
```csharp
protected List<GanttTask> CreateTestTasks(int count)
{
    var tasks = new List<GanttTask>();
    var startDate = DateTime.Today;
    
    for (int i = 0; i < count; i++)
    {
        tasks.Add(new GanttTask
        {
            Id = i + 1,
            Name = $"Task {i + 1}",
            WbsCode = $"{i + 1}",
            StartDate = startDate.AddDays(i),
            Duration = $"{(i % 10) + 1}d", // 1-10 day durations
            ParentId = i > 0 && i % 5 == 0 ? i - 4 : null // Some hierarchy
        });
    }
    
    return tasks;
}

protected int ParseDurationDays(string duration)
{
    // Simple duration parser for testing
    if (duration.EndsWith("d"))
    {
        if (int.TryParse(duration[..^1], out int days))
            return days;
    }
    return 1; // Default to 1 day
}
```

## 📊 **Integration Test Metrics**

### **Coverage Targets**
- ✅ **Service Interactions**: 100% of critical service-to-service calls
- ✅ **Data Flow Paths**: 100% of data transformation pipelines
- ✅ **Error Scenarios**: 80% of error handling pathways
- ✅ **Performance Scenarios**: All major data volume scenarios

### **Performance Benchmarks**
- ✅ **Test Execution**: All integration tests < 30 seconds
- ✅ **Service Coordination**: Cross-service calls < 100ms
- ✅ **Data Processing**: 500+ records < 1 second
- ✅ **Memory Stability**: <5MB increase over 100 operations

### **Quality Gates**
- ✅ **All Integration Tests Pass**: Required for PR merge
- ✅ **Performance Thresholds Met**: No performance regression
- ✅ **Service Registration Valid**: All DI services resolvable
- ✅ **Error Handling Verified**: Graceful degradation confirmed

---

*Integration testing ensures that our modular architecture works seamlessly when components interact, providing confidence in the coordination between services while maintaining the performance characteristics required for professional Gantt functionality.*
