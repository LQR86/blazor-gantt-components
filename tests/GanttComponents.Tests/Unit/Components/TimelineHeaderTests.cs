using Xunit;
using GanttComponents.Models;
using GanttComponents.Services;
using Microsoft.Extensions.Logging.Abstractions;

namespace GanttComponents.Tests.Unit.Components;

/// <summary>
/// Unit tests for TimelineHeader component integration - Phase 3.5 isolation testing
/// These tests validate the component's service integration and parameter handling.
/// </summary>
public class TimelineHeaderTests
{
    private readonly ITimelineHeaderService _headerService;
    private readonly IUniversalLogger _logger;

    public TimelineHeaderTests()
    {
        var i18n = new GanttI18N();
        var dateFormatter = new DateFormatHelper(i18n);
        _headerService = new TimelineHeaderService(dateFormatter);
        _logger = new UniversalLogger(NullLogger<UniversalLogger>.Instance);
    }

    [Fact]
    public void TimelineHeader_ServiceIntegration_GeneratesHeaderData()
    {
        // Arrange - Parameters matching component interface
        var startDate = new DateTime(2025, 1, 1);
        var endDate = new DateTime(2025, 1, 31);
        var dayWidth = 30.0;
        var zoomLevel = TimelineZoomLevel.MonthDay34px;

        // Act - Test the service call that component would make
        var headerResult = _headerService.GenerateHeaderPeriods(startDate, endDate, dayWidth, zoomLevel, _logger);

        // Assert - Validate data structure component expects
        Assert.NotNull(headerResult);
        Assert.NotNull(headerResult.PrimaryPeriods);
        Assert.NotNull(headerResult.SecondaryPeriods);

        // Verify secondary periods always exist (component requirement)
        Assert.True(headerResult.SecondaryPeriods.Any(), "TimelineHeader component requires secondary periods");

        // Verify each period has required properties for rendering
        foreach (var period in headerResult.SecondaryPeriods)
        {
            Assert.True(period.Width > 0, "Period width must be positive for CSS rendering");
            Assert.NotNull(period.Label);
            Assert.NotEqual(string.Empty, period.Label);
        }
    }

    [Fact]
    public void TimelineHeader_HandlesExtremeDayWidths_GeneratesValidData()
    {
        // Arrange - Test extreme zoom levels that component must handle
        var startDate = new DateTime(2025, 1, 1);
        var endDate = new DateTime(2025, 1, 7); // One week

        // Test with very small day width (zoomed out)
        var result1 = _headerService.GenerateHeaderPeriods(startDate, endDate, 1.0, TimelineZoomLevel.YearQuarter3px, _logger);

        // Test with very large day width (zoomed in)  
        var result2 = _headerService.GenerateHeaderPeriods(startDate, endDate, 200.0, TimelineZoomLevel.WeekDay97px, _logger);

        // Assert both generate valid data for component rendering
        Assert.True(result1.SecondaryPeriods.Any(), "Extreme zoom out should still generate periods");
        Assert.True(result2.SecondaryPeriods.Any(), "Extreme zoom in should still generate periods");

        // Verify widths are valid for CSS
        Assert.All(result1.SecondaryPeriods, p => Assert.True(p.Width > 0));
        Assert.All(result2.SecondaryPeriods, p => Assert.True(p.Width > 0));
    }

    [Fact]
    public void TimelineHeader_ParameterValidation_HandlesEdgeCases()
    {
        // Arrange - Test edge cases component might encounter
        var startDate = new DateTime(2025, 1, 1);
        var endDate = startDate; // Same day
        var dayWidth = 24.0;
        var zoomLevel = TimelineZoomLevel.QuarterMonth24px;

        // Act
        var result = _headerService.GenerateHeaderPeriods(startDate, endDate, dayWidth, zoomLevel, _logger);

        // Assert - Component should handle single-day timelines
        Assert.NotNull(result);
        Assert.True(result.SecondaryPeriods.Any(), "Single day should still generate at least one period");

        var period = result.SecondaryPeriods.First();
        Assert.True(period.Width > 0, "Single day period should have positive width");
        Assert.NotNull(period.Label);
    }

    [Theory]
    [InlineData(TimelineZoomLevel.YearQuarter3px)]
    [InlineData(TimelineZoomLevel.QuarterMonth24px)]
    [InlineData(TimelineZoomLevel.WeekDay97px)]
    public void TimelineHeader_AllZoomLevels_GenerateValidData(TimelineZoomLevel zoomLevel)
    {
        // Arrange
        var startDate = new DateTime(2025, 1, 1);
        var endDate = new DateTime(2025, 1, 15);
        var dayWidth = TimelineZoomService.CalculateEffectiveDayWidth(zoomLevel, 1.0);

        // Act
        var result = _headerService.GenerateHeaderPeriods(startDate, endDate, dayWidth, zoomLevel, _logger);

        // Assert - All zoom levels should generate valid component data
        Assert.NotNull(result);
        Assert.True(result.SecondaryPeriods.Any(), $"Zoom level {zoomLevel} should generate secondary periods");

        // Verify data integrity for component rendering
        Assert.All(result.SecondaryPeriods, p =>
        {
            Assert.True(p.Width > 0, $"Period width invalid for zoom {zoomLevel}");
            Assert.NotNull(p.Label);
        });
    }
}
