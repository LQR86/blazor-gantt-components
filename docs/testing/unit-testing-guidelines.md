# Unit Testing Guidelines

**Framework**: xUnit.net  
**Approach**: Test-Driven Development (TDD)  
**Coverage Target**: 80%+ for business logic  
**Date**: August 1, 2025  

## 🎯 **Unit Testing Philosophy**

Unit tests form the foundation of our testing strategy, providing:

- ✅ **Fast feedback** during development (< 1 second per test)
- ✅ **Comprehensive coverage** of business logic and calculations
- ✅ **Regression prevention** through automated CI/CD integration
- ✅ **Documentation** of expected behavior and edge cases

## 📋 **What to Unit Test**

### **✅ Always Unit Test**
- **Mathematical calculations** (zoom formulas, date arithmetic)
- **Data transformations** (model mapping, serialization)
- **Business logic methods** (validation, processing)
- **Service method logic** (in isolation with mocked dependencies)
- **Edge cases and boundary conditions**

### **❌ Avoid Unit Testing**
- **Blazor component rendering** (use Interactive Validation instead)
- **Database operations** (use Integration Tests instead)
- **UI interactions** (use Interactive/Manual Testing instead)
- **External API calls** (mock the calls, test the integration layer)

## 🛠 **Test Structure and Organization**

### **Directory Structure**
```
tests/GanttComponents.Tests/Unit/
├── Models/
│   ├── GanttTaskTests.cs
│   ├── WbsCodeTests.cs
│   └── TimelineZoomLevelTests.cs
├── Services/
│   ├── GanttTaskServiceTests.cs
│   ├── TimelineZoomServiceTests.cs
│   └── WbsCodeGenerationServiceTests.cs
├── Components/
│   └── (Logic-only component tests)
└── Utils/
    └── DateCalculationTests.cs
```

### **Test Class Naming Convention**
```csharp
// Pattern: {ClassUnderTest}Tests
public class TimelineZoomServiceTests { }
public class GanttTaskTests { }
public class WbsCodeGenerationServiceTests { }
```

### **Test Method Naming Convention**
```csharp
// Pattern: {MethodUnderTest}_{Scenario}_{ExpectedBehavior}
[Fact]
public void CalculateEffectiveDayWidth_WithValidInputs_ReturnsCorrectWidth() { }

[Fact] 
public void ValidateWbsCode_WithDuplicateCode_ReturnsFalse() { }

[Theory]
public void ParseDuration_WithInvalidFormat_ThrowsArgumentException() { }
```

## 📝 **Test Method Patterns**

### **1. Simple Fact Test**
```csharp
[Fact]
public void ZoomLevelConfiguration_WeekDay_HasCorrectBaseUnitWidth()
{
    // Arrange
    var expectedWidth = 12.0;  // 12px per day at 1.0x zoom
    
    // Act
    var config = TimelineZoomService.GetConfiguration(TimelineZoomLevel.WeekDay);
    
    // Assert
    Assert.Equal(expectedWidth, config.BaseUnitWidth);
}
```

### **2. Theory Test with Multiple Inputs**
```csharp
[Theory]
[InlineData(TimelineZoomLevel.WeekDay, 1.0, 12.0)]        // 12px per day
[InlineData(TimelineZoomLevel.MonthWeek, 1.0, 2.57)]      // 18px per week / 7 days
[InlineData(TimelineZoomLevel.QuarterMonth, 1.0, 0.67)]   // 20px per month / 30 days  
[InlineData(TimelineZoomLevel.YearQuarter, 1.0, 0.27)]    // 24px per quarter / 90 days
public void CalculateEffectiveDayWidth_AllZoomLevels_ReturnsCorrectValues(
    TimelineZoomLevel level, double factor, double expected)
{
    // Act
    var result = TimelineZoomService.CalculateEffectiveDayWidth(level, factor);
    
    // Assert
    Assert.Equal(expected, result, precision: 1); // 1 decimal place tolerance
}
```

### **3. Exception Testing**
```csharp
[Theory]
[InlineData(null)]
[InlineData("")]
[InlineData("   ")]
public void ValidateWbsCode_WithInvalidInput_ThrowsArgumentException(string invalidCode)
{
    // Arrange
    var service = new WbsCodeGenerationService();
    
    // Act & Assert
    Assert.Throws<ArgumentException>(() => service.ValidateWbsCode(invalidCode));
}
```

### **4. Service Test with Mocked Dependencies**
```csharp
public class GanttTaskServiceTests : IDisposable
{
    private readonly GanttDbContext _context;
    private readonly GanttTaskService _taskService;
    private readonly Mock<ILogger<GanttTaskService>> _mockLogger;
    private readonly Mock<IUniversalLogger> _mockUniversalLogger;

    public GanttTaskServiceTests()
    {
        // Setup in-memory database
        var options = new DbContextOptionsBuilder<GanttDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new GanttDbContext(options);
        
        // Setup mocks
        _mockLogger = new Mock<ILogger<GanttTaskService>>();
        _mockUniversalLogger = new Mock<IUniversalLogger>();
        
        // Create service with mocked dependencies
        _taskService = new GanttTaskService(_context, _mockLogger.Object, _mockUniversalLogger.Object);
    }

    [Fact]
    public async Task GetAllTasksAsync_WithValidData_ReturnsAllTasks()
    {
        // Arrange
        var testTasks = CreateTestTasks(3);
        _context.Tasks.AddRange(testTasks);
        await _context.SaveChangesAsync();

        // Act
        var result = await _taskService.GetAllTasksAsync();

        // Assert
        Assert.Equal(3, result.Count);
        Assert.All(result, task => Assert.NotNull(task.WbsCode));
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
```

## 🧮 **Testing Mathematical Calculations**

### **Zoom Level Calculations**
```csharp
public class TimelineZoomTests
{
    [Theory]
    [InlineData(1.0, 25.0)]   // Base factor
    [InlineData(1.6, 4.11)]   // 1.6x zoom factor  
    [InlineData(0.5, 1.29)]   // Below minimum gets clamped to 1.0x
    [InlineData(3.0, 7.71)]   // Maximum factor for MonthWeek
    public void CalculateEffectiveDayWidth_MonthWeek_ScalesCorrectly(
        double zoomFactor, double expectedWidth)
    {
        // Arrange
        var config = TimelineZoomService.GetConfiguration(TimelineZoomLevel.MonthWeek);

        // Act
        var effectiveWidth = config.GetEffectiveDayWidth(zoomFactor);

        // Assert
        Assert.Equal(expectedWidth, effectiveWidth, precision: 2);
    }

    [Theory]
    [InlineData(-0.5, 1.0)]   // Below minimum gets clamped to 1.0x
    [InlineData(0.3, 1.0)]    // Below minimum gets clamped to 1.0x
    [InlineData(3.5, 3.0)]    // Above maximum gets clamped to 3.0x
    [InlineData(5.0, 3.0)]    // Above maximum gets clamped to 3.0x
    public void ZoomFactorClamping_OutOfBounds_ClampsToValidRange(
        double input, double expected)
    {
        // Arrange
        var config = TimelineZoomService.GetConfiguration(TimelineZoomLevel.MonthWeek);

        // Act
        var clampedWidth = config.GetEffectiveDayWidth(input);
        var expectedWidth = (config.BaseUnitWidth * expected) / config.TemplateUnitDays;

        // Assert
        Assert.Equal(expectedWidth, clampedWidth, precision: 2);
    }
}
```

### **Date Calculations**
```csharp
public class DateCalculationTests
{
    [Theory]
    [InlineData("2025-08-01", "5d", "2025-08-06")]
    [InlineData("2025-08-01", "2w", "2025-08-15")]
    [InlineData("2025-08-01", "1m", "2025-09-01")]
    public void CalculateEndDate_WithDuration_ReturnsCorrectDate(
        string startDateStr, string duration, string expectedEndDateStr)
    {
        // Arrange
        var startDate = DateTime.Parse(startDateStr);
        var expectedEndDate = DateTime.Parse(expectedEndDateStr);

        // Act
        var result = DateCalculationHelper.CalculateEndDate(startDate, duration);

        // Assert
        Assert.Equal(expectedEndDate, result);
    }
}
```

## 🎭 **Mocking Guidelines**

### **When to Mock**
- ✅ **External dependencies** (databases, APIs, file systems)
- ✅ **Logging interfaces** (to verify logging behavior)
- ✅ **Complex services** (to isolate the unit under test)
- ✅ **Time-dependent operations** (to control time)

### **Mocking Patterns**
```csharp
// 1. Simple mock setup
var mockLogger = new Mock<IUniversalLogger>();
mockLogger.Setup(x => x.LogDebugInfo(It.IsAny<string>())).Verifiable();

// 2. Mock with return values
var mockI18N = new Mock<IGanttI18N>();
mockI18N.Setup(x => x.T(It.IsAny<string>())).Returns((string key) => key);

// 3. Mock verification
mockLogger.Verify(x => x.LogDebugInfo(
    It.Is<string>(s => s.Contains("Expected message"))), 
    Times.Once);

// 4. Mock with complex behavior
mockTaskService.Setup(x => x.GetTaskAsync(It.IsAny<int>()))
    .Returns<int>(id => Task.FromResult(testTasks.FirstOrDefault(t => t.Id == id)));
```

## ⚡ **Performance Testing in Unit Tests**

### **Algorithm Performance**
```csharp
[Fact]
public void ZoomCalculation_LargeDataset_CompletesWithinThreshold()
{
    // Arrange
    var levels = Enum.GetValues<TimelineZoomLevel>();
    var factors = Enumerable.Range(1, 100).Select(i => i * 0.01).ToArray();
    
    // Act
    var stopwatch = Stopwatch.StartNew();
    foreach (var level in levels)
    {
        foreach (var factor in factors)
        {
            var result = TimelineZoomService.CalculateEffectiveDayWidth(level, factor);
        }
    }
    stopwatch.Stop();
    
    // Assert - 600 calculations should complete in < 10ms
    Assert.True(stopwatch.ElapsedMilliseconds < 10);
}
```

### **Memory Usage Testing**
```csharp
[Fact]
public void TaskGeneration_LargeSet_DoesNotLeakMemory()
{
    // Arrange
    var initialMemory = GC.GetTotalMemory(true);
    
    // Act
    for (int i = 0; i < 1000; i++)
    {
        var task = new GanttTask { Id = i, Name = $"Task {i}" };
        // Process task
    }
    
    GC.Collect();
    GC.WaitForPendingFinalizers();
    var finalMemory = GC.GetTotalMemory(true);
    
    // Assert - Memory increase should be reasonable
    var memoryIncrease = finalMemory - initialMemory;
    Assert.True(memoryIncrease < 1_000_000); // Less than 1MB
}
```

## 📊 **Test Data Management**

### **Test Data Builders**
```csharp
public static class TestDataBuilder
{
    public static GanttTask CreateTask(int id = 1, string name = "Test Task")
    {
        return new GanttTask
        {
            Id = id,
            Name = name,
            WbsCode = $"{id}",
            StartDate = new DateTime(2025, 8, 1),
            Duration = "5d",
            ParentId = null
        };
    }
    
    public static List<GanttTask> CreateTaskHierarchy(int rootCount = 3, int childrenPerRoot = 2)
    {
        var tasks = new List<GanttTask>();
        int currentId = 1;
        
        for (int i = 0; i < rootCount; i++)
        {
            var rootTask = CreateTask(currentId++, $"Root Task {i + 1}");
            tasks.Add(rootTask);
            
            for (int j = 0; j < childrenPerRoot; j++)
            {
                var childTask = CreateTask(currentId++, $"Child Task {i + 1}.{j + 1}");
                childTask.ParentId = rootTask.Id;
                tasks.Add(childTask);
            }
        }
        
        return tasks;
    }
}
```

### **Test Categories and Traits**
```csharp
[Fact]
[Trait("Category", "Fast")]
[Trait("Component", "ZoomService")]
public void QuickZoomCalculation_Test() { }

[Fact]
[Trait("Category", "Slow")]
[Trait("Component", "Database")]
public void DatabaseIntensive_Test() { }
```

## 🚀 **Continuous Integration**

### **Test Execution in CI/CD**
```yaml
# Example: GitHub Actions test execution
- name: Run Unit Tests
  run: dotnet test --configuration Release --logger trx --collect:"XPlat Code Coverage"
  
- name: Generate Coverage Report
  run: dotnet tool install -g dotnet-reportgenerator-globaltool && reportgenerator -reports:**/coverage.cobertura.xml -targetdir:coverage -reporttypes:Html
```

### **Quality Gates**
- ✅ **All unit tests** must pass before PR merge
- ✅ **Code coverage** must be >= 80% for new code
- ✅ **Test execution time** must be < 30 seconds for full suite
- ✅ **No flaky tests** - tests must be deterministic

---

*Unit testing provides the foundation for confident development through fast, comprehensive validation of business logic and calculations.*
