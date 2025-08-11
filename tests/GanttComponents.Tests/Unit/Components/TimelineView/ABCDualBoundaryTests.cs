using Xunit;
using Microsoft.Extensions.Logging;
using GanttComponents.Components.TimelineView.Renderers;
using GanttComponents.Models;
using GanttComponents.Services;
using System;

namespace GanttComponents.Tests.Unit.Components.TimelineView;

/// <summary>
/// Comprehensive test suite for ABC Dual Boundary Enforcement in BaseTimelineRenderer.
/// Validates that the template method pattern automatically enforces dual boundary expansion
/// and that the union calculation works correctly for all renderer patterns.
/// </summary>
public class ABCDualBoundaryTests
{
    private readonly IUniversalLogger _mockLogger;
    private readonly IGanttI18N _mockI18N;
    private readonly DateFormatHelper _dateFormatter;

    public ABCDualBoundaryTests()
    {
        // Create mock dependencies for testing
        _mockLogger = new MockUniversalLogger();
        _mockI18N = new MockGanttI18N();
        _dateFormatter = new DateFormatHelper(_mockI18N);
    }

    /// <summary>
    /// Test that WeekDay50pxRenderer correctly implements ABC dual boundary pattern.
    /// Both primary and secondary should return week boundaries, resulting in identical union.
    /// </summary>
    [Fact]
    public void WeekDay50pxRenderer_ABCDualBoundaries_ProducesCorrectUnion()
    {
        // ARRANGE: Timeline spanning partial weeks (Aug 15-20, 2025)
        var startDate = new DateTime(2025, 8, 15); // Friday
        var endDate = new DateTime(2025, 8, 20);   // Wednesday
        
        var renderer = new WeekDay50pxRenderer(
            _mockLogger, _mockI18N, _dateFormatter,
            startDate, endDate, 50, 30, TimelineZoomLevel.WeekDayOptimal50px, 1.0);

        // ACT: Get boundaries through reflection to test ABC pattern
        var primaryBounds = InvokeProtectedMethod<(DateTime, DateTime)>(renderer, "CalculatePrimaryBoundaries");
        var secondaryBounds = InvokeProtectedMethod<(DateTime, DateTime)>(renderer, "CalculateSecondaryBoundaries");
        var unionBounds = InvokeProtectedMethod<(DateTime, DateTime)>(renderer, "CalculateHeaderBoundaries");

        // ASSERT: Both should be week boundaries
        var expectedWeekStart = new DateTime(2025, 8, 11); // Monday of Aug 15 week
        var expectedWeekEnd = new DateTime(2025, 8, 24);   // Sunday of Aug 20 week

        Assert.Equal(expectedWeekStart, primaryBounds.Item1);
        Assert.Equal(expectedWeekEnd, primaryBounds.Item2);
        Assert.Equal(expectedWeekStart, secondaryBounds.Item1);
        Assert.Equal(expectedWeekEnd, secondaryBounds.Item2);

        // Union should be identical (Week ∪ Week = Week)
        Assert.Equal(expectedWeekStart, unionBounds.Item1);
        Assert.Equal(expectedWeekEnd, unionBounds.Item2);
    }

    /// <summary>
    /// Test ABC union calculation with different boundary types.
    /// Creates a mock renderer to simulate MonthWeek pattern where union should expand.
    /// </summary>
    [Fact]
    public void ABCDualBoundary_UnionCalculation_TakesWidestSpan()
    {
        // ARRANGE: Simulate MonthWeek pattern boundaries
        var startDate = new DateTime(2025, 8, 15);
        var endDate = new DateTime(2025, 9, 15);

        var mockRenderer = new MockDualBoundaryRenderer(
            _mockLogger, _mockI18N, _dateFormatter,
            startDate, endDate, 50, 30, TimelineZoomLevel.MonthWeekOptimal50px, 1.0);

        // Set up different primary vs secondary boundaries
        var monthStart = new DateTime(2025, 8, 1);  // Month boundaries (wider)
        var monthEnd = new DateTime(2025, 9, 30);
        var weekStart = new DateTime(2025, 8, 11);  // Week boundaries (narrower)  
        var weekEnd = new DateTime(2025, 9, 21);

        mockRenderer.SetPrimaryBoundaries(monthStart, monthEnd);
        mockRenderer.SetSecondaryBoundaries(weekStart, weekEnd);

        // ACT: Get union calculation
        var unionBounds = InvokeProtectedMethod<(DateTime, DateTime)>(mockRenderer, "CalculateHeaderBoundaries");

        // ASSERT: Union should take widest span
        // Start: Min(Aug 1, Aug 11) = Aug 1 (month wins)
        // End: Max(Sep 30, Sep 21) = Sep 30 (month wins)
        Assert.Equal(monthStart, unionBounds.Item1);
        Assert.Equal(monthEnd, unionBounds.Item2);
    }

    /// <summary>
    /// Test that ABC pattern prevents renderer from overriding CalculateHeaderBoundaries.
    /// The base class method should not be virtual, ensuring dual expansion is enforced.
    /// </summary>
    [Fact]
    public void ABCDualBoundary_BaseClassMethod_IsNotVirtual()
    {
        // ARRANGE: Get CalculateHeaderBoundaries method from BaseTimelineRenderer
        var baseType = typeof(BaseTimelineRenderer);
        var method = baseType.GetMethod("CalculateHeaderBoundaries", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        // ASSERT: Method should exist but not be virtual (cannot be overridden)
        Assert.NotNull(method);
        Assert.False(method.IsVirtual, "CalculateHeaderBoundaries should not be virtual to enforce ABC pattern");
        Assert.False(method.IsAbstract, "CalculateHeaderBoundaries should be concrete with union logic");
    }

    /// <summary>
    /// Test that abstract boundary methods are properly defined in base class.
    /// Ensures the ABC composition contract is correctly established.
    /// </summary>
    [Fact]
    public void ABCDualBoundary_AbstractMethods_AreProperlyDefined()
    {
        // ARRANGE: Get abstract methods from BaseTimelineRenderer
        var baseType = typeof(BaseTimelineRenderer);
        var primaryMethod = baseType.GetMethod("CalculatePrimaryBoundaries",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var secondaryMethod = baseType.GetMethod("CalculateSecondaryBoundaries",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        // ASSERT: Both methods should be abstract
        Assert.NotNull(primaryMethod);
        Assert.NotNull(secondaryMethod);
        Assert.True(primaryMethod.IsAbstract, "CalculatePrimaryBoundaries should be abstract");
        Assert.True(secondaryMethod.IsAbstract, "CalculateSecondaryBoundaries should be abstract");

        // Return types should be (DateTime, DateTime)
        var expectedReturnType = typeof((DateTime, DateTime));
        Assert.Equal(expectedReturnType, primaryMethod.ReturnType);
        Assert.Equal(expectedReturnType, secondaryMethod.ReturnType);
    }

    /// <summary>
    /// Performance test: Ensure dual boundary calculation doesn't impact render speed.
    /// ABC pattern should be efficient even with dual calculations.
    /// </summary>
    [Fact]
    public void ABCDualBoundary_Performance_EfficientCalculation()
    {
        // ARRANGE: Large timeline spanning multiple months
        var startDate = new DateTime(2025, 1, 1);
        var endDate = new DateTime(2025, 12, 31);
        
        var renderer = new WeekDay50pxRenderer(
            _mockLogger, _mockI18N, _dateFormatter,
            startDate, endDate, 50, 30, TimelineZoomLevel.WeekDayOptimal50px, 1.0);

        // ACT & ASSERT: Multiple boundary calculations should be fast
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        for (int i = 0; i < 1000; i++)
        {
            var bounds = InvokeProtectedMethod<(DateTime, DateTime)>(renderer, "CalculateHeaderBoundaries");
        }
        
        stopwatch.Stop();
        
        // Should complete 1000 calculations in under 100ms
        Assert.True(stopwatch.ElapsedMilliseconds < 100, 
            $"ABC dual boundary calculation too slow: {stopwatch.ElapsedMilliseconds}ms for 1000 operations");
    }

    /// <summary>
    /// Test edge case: Single day timeline should work correctly with ABC pattern.
    /// </summary>
    [Fact]
    public void ABCDualBoundary_SingleDay_HandlesCorrectly()
    {
        // ARRANGE: Single day timeline
        var singleDate = new DateTime(2025, 8, 15); // Friday
        
        var renderer = new WeekDay50pxRenderer(
            _mockLogger, _mockI18N, _dateFormatter,
            singleDate, singleDate, 50, 30, TimelineZoomLevel.WeekDayOptimal50px, 1.0);

        // ACT: Get union boundaries
        var unionBounds = InvokeProtectedMethod<(DateTime, DateTime)>(renderer, "CalculateHeaderBoundaries");

        // ASSERT: Should expand to complete week
        var expectedWeekStart = new DateTime(2025, 8, 11); // Monday
        var expectedWeekEnd = new DateTime(2025, 8, 17);   // Sunday
        
        Assert.Equal(expectedWeekStart, unionBounds.Item1);
        Assert.Equal(expectedWeekEnd, unionBounds.Item2);
    }

    // === HELPER METHODS ===

    /// <summary>
    /// Helper method to invoke protected methods via reflection for testing.
    /// </summary>
    private T InvokeProtectedMethod<T>(object obj, string methodName, params object[] parameters)
    {
        var method = obj.GetType().GetMethod(methodName, 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (method == null)
            throw new InvalidOperationException($"Method {methodName} not found on {obj.GetType().Name}");
        
        var result = method.Invoke(obj, parameters);
        if (result == null)
            throw new InvalidOperationException($"Method {methodName} returned null");
            
        return (T)result;
    }
}

/// <summary>
/// Mock renderer for testing ABC dual boundary pattern with different boundary types.
/// Simulates MonthWeek or QuarterMonth patterns where primary and secondary differ.
/// </summary>
public class MockDualBoundaryRenderer : BaseTimelineRenderer
{
    private (DateTime start, DateTime end) _primaryBounds;
    private (DateTime start, DateTime end) _secondaryBounds;

    public MockDualBoundaryRenderer(
        IUniversalLogger logger,
        IGanttI18N i18n,
        DateFormatHelper dateFormatter,
        DateTime startDate,
        DateTime endDate,
        int headerMonthHeight,
        int headerDayHeight,
        TimelineZoomLevel zoomLevel,
        double zoomFactor)
        : base(logger, i18n, dateFormatter, startDate, endDate,
               50.0, headerMonthHeight, headerDayHeight, zoomLevel, zoomFactor)
    {
    }

    public void SetPrimaryBoundaries(DateTime start, DateTime end)
    {
        _primaryBounds = (start, end);
    }

    public void SetSecondaryBoundaries(DateTime start, DateTime end)
    {
        _secondaryBounds = (start, end);
    }

    protected override (DateTime start, DateTime end) CalculatePrimaryBoundaries()
    {
        return _primaryBounds;
    }

    protected override (DateTime start, DateTime end) CalculateSecondaryBoundaries()
    {
        return _secondaryBounds;
    }

    protected override string RenderPrimaryHeader() => "<g>Mock Primary</g>";
    
    protected override string RenderSecondaryHeader() => "<g>Mock Secondary</g>";
    
    protected override string GetRendererDescription() => "Mock Dual Boundary Renderer";
    
    protected override string GetCSSClass() => "mock-renderer";
}

/// <summary>
/// Mock logger implementation for testing.
/// </summary>
public class MockUniversalLogger : IUniversalLogger
{
    public void LogOperation(string category, string operation, object? data = null) { }
    public void LogDebugInfo(string message, object? data = null) { }
    public void LogError(string message, Exception? exception = null) { }
    public void LogWarning(string message, object? data = null) { }
    public void LogInfo(string message, object? data = null) { }
    public void LogTaskGridOperation(string operation, object? data = null) { }
    public void LogTimelineOperation(string operation, object? data = null) { }
    public void LogRowAlignment(string operation, object? data = null) { }
    public void LogWbsOperation(string operation, object? data = null) { }
    public void LogDependencyOperation(string operation, object? data = null) { }
    public void LogDateOperation(string taskName, DateOnly? date, string dateType) { }
    public void LogDurationCalculation(string operation, int days, object? context = null) { }
    public void LogComponentLifecycle(string componentName, string lifecycle, object? data = null) { }
    public void LogStateChange(string component, string property, object? oldValue, object? newValue) { }
    public void LogUserAction(string action, object? context = null) { }
    public void LogPerformance(string operation, TimeSpan duration, object? metadata = null) { }
    public void LogDatabaseOperation(string operation, object? data = null) { }
}

/// <summary>
/// Mock internationalization service for testing.
/// </summary>
public class MockGanttI18N : IGanttI18N
{
    public string CurrentCulture => "en-US";
    public string T(string key) => key;
    public void SetCulture(string culture) { }
    public IEnumerable<string> GetAvailableCultures() => new[] { "en-US", "zh-CN" };
    public bool HasTranslation(string key) => !string.IsNullOrEmpty(key);
    
#pragma warning disable CS0067 // Event is never used - this is a mock for testing
    public event Action? LanguageChanged;
#pragma warning restore CS0067
}
