using Xunit;
using GanttComponents.Services;
using GanttComponents.Models;
using Microsoft.Extensions.Logging.Abstractions;

namespace GanttComponents.Tests.Unit.Services;

/// <summary>
/// Unit tests for TimelineHeaderService - Phase 1 Foundation
/// These tests validate the service interface and stub behavior.
/// Phase 2 will add comprehensive tests for business logic accuracy.
/// </summary>
public class TimelineHeaderServiceTests
{
    private readonly ITimelineHeaderService _service;
    private readonly IUniversalLogger _logger;

    public TimelineHeaderServiceTests()
    {
        var i18n = new GanttI18N();
        var dateFormatter = new DateFormatHelper(i18n);
        _service = new TimelineHeaderService(dateFormatter, i18n);
        _logger = new UniversalLogger(NullLogger<UniversalLogger>.Instance);
    }

    [Fact]
    public void GenerateHeaderPeriods_Phase1Stub_ReturnsValidResult()
    {
        // Arrange
        var startDate = new DateTime(2025, 1, 1);
        var endDate = new DateTime(2025, 3, 31);
        var effectiveDayWidth = 40.0;
        var zoomLevel = TimelineZoomLevel.QuarterMonth24px;

        // Act
        var result = _service.GenerateHeaderPeriods(startDate, endDate, effectiveDayWidth, zoomLevel, _logger);

        // Assert - Updated for real implementation: Basic structure validation
        Assert.NotNull(result);
        Assert.NotNull(result.PrimaryPeriods);
        Assert.NotNull(result.SecondaryPeriods);

        // Since we now have real implementation, expect actual periods for 3 months
        Assert.True(result.SecondaryPeriods.Count >= 3, "Should have at least 3 months of periods");

        // Verify each period has valid data
        foreach (var period in result.SecondaryPeriods)
        {
            Assert.True(period.Width > 0, "Period width should be positive");
            Assert.NotNull(period.Label);
            Assert.NotEqual(string.Empty, period.Label);
        }
    }

    [Fact]
    public void GenerateHeaderPeriods_ValidatesParameterTypes()
    {
        // Arrange
        var startDate = DateTime.Today;
        var endDate = DateTime.Today.AddDays(30);
        var effectiveDayWidth = 25.5;
        var zoomLevel = TimelineZoomLevel.WeekDay68px;

        // Act
        var result = _service.GenerateHeaderPeriods(startDate, endDate, effectiveDayWidth, zoomLevel);

        // Assert - Validate interface compliance
        Assert.NotNull(result);
        Assert.IsType<List<HeaderPeriod>>(result.PrimaryPeriods);
        Assert.IsType<List<HeaderPeriod>>(result.SecondaryPeriods);
        Assert.IsType<bool>(result.ShouldCollapse);
    }

    [Fact]
    public void ShouldCollapseHeaders_Phase1Stub_ReturnsFalse()
    {
        // Arrange
        var headerConfig = new TimelineHeaderConfiguration
        {
            PrimaryUnit = TimelineHeaderUnit.Quarter,
            SecondaryUnit = TimelineHeaderUnit.Month
        };
        var effectiveDayWidth = 40.0;
        var timeSpanDays = 90;

        // Act
        var result = _service.ShouldCollapseHeaders(headerConfig, effectiveDayWidth, timeSpanDays);

        // Assert - Phase 1: Stub behavior
        Assert.False(result);
    }

    [Theory]
    [InlineData(TimelineZoomLevel.QuarterMonth24px)]
    [InlineData(TimelineZoomLevel.WeekDay68px)]
    [InlineData(TimelineZoomLevel.MonthDay48px)]
    public void GenerateHeaderPeriods_AllZoomLevels_ReturnsValidStructure(TimelineZoomLevel zoomLevel)
    {
        // Arrange
        var startDate = new DateTime(2025, 1, 1);
        var endDate = new DateTime(2025, 12, 31);
        var effectiveDayWidth = 30.0;

        // Act
        var result = _service.GenerateHeaderPeriods(startDate, endDate, effectiveDayWidth, zoomLevel);

        // Assert - Interface compliance across all zoom levels
        Assert.NotNull(result);
        Assert.NotEmpty(result.PrimaryPeriods);
        Assert.NotEmpty(result.SecondaryPeriods);

        // Validate HeaderPeriod properties
        foreach (var period in result.PrimaryPeriods)
        {
            Assert.True(period.Start <= period.End);
            Assert.True(period.Width >= 0);
            Assert.NotNull(period.Label);
            Assert.Equal(HeaderLevel.Primary, period.Level);
        }

        foreach (var period in result.SecondaryPeriods)
        {
            Assert.True(period.Start <= period.End);
            Assert.True(period.Width >= 0);
            Assert.NotNull(period.Label);
            Assert.Equal(HeaderLevel.Secondary, period.Level);
        }
    }
}

/// <summary>
/// Integration tests for TimelineHeaderService with actual TimelineHeaderAdapter
/// Phase 1: Foundation tests
/// Phase 2: Will add comprehensive integration tests
/// </summary>
public class TimelineHeaderServiceIntegrationTests
{
    [Fact]
    public void Service_IntegratesWithExistingTimelineHeaderAdapter()
    {
        // Phase 1: Validate service can work with existing infrastructure
        var zoomLevel = TimelineZoomLevel.QuarterMonth24px;
        var config = TimelineHeaderAdapter.GetHeaderConfigurationFromTemplate(zoomLevel);

        Assert.NotNull(config);
        Assert.NotEqual(TimelineHeaderUnit.Day, config.PrimaryUnit); // Should be higher level

        // Service should be able to use this config in Phase 2
        var i18n = new GanttI18N();
        var dateFormatter = new DateFormatHelper(i18n);
        var service = new TimelineHeaderService(dateFormatter, i18n);
        Assert.NotNull(service);
    }
}
