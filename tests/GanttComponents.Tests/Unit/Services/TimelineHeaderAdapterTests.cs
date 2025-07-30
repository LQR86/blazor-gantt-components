using GanttComponents.Models;
using GanttComponents.Services;

namespace GanttComponents.Tests.Unit.Services;

public class TimelineHeaderAdapterTests
{
    [Fact]
    public void GetHeaderConfiguration_WeekDayLevel_ReturnsCorrectConfiguration()
    {
        // Arrange
        var zoomLevel = TimelineZoomLevel.WeekDay;
        var effectiveDayWidth = 60.0;

        // Act
        var config = TimelineHeaderAdapter.GetHeaderConfiguration(zoomLevel, effectiveDayWidth);

        // Assert
        Assert.Equal(TimelineHeaderUnit.Month, config.PrimaryUnit);
        Assert.Equal("date.month-year", config.PrimaryFormat);
        Assert.Equal(TimelineHeaderUnit.Day, config.SecondaryUnit);
        Assert.Equal("date.day-number", config.SecondaryFormat);
        Assert.True(config.ShowPrimary);
        Assert.True(config.ShowSecondary);
    }

    [Fact]
    public void GetHeaderConfiguration_MonthDayLevel_ReturnsCorrectConfiguration()
    {
        // Arrange
        var zoomLevel = TimelineZoomLevel.MonthDay;
        var effectiveDayWidth = 25.0;

        // Act
        var config = TimelineHeaderAdapter.GetHeaderConfiguration(zoomLevel, effectiveDayWidth);

        // Assert
        Assert.Equal(TimelineHeaderUnit.Month, config.PrimaryUnit);
        Assert.Equal("date.month-year", config.PrimaryFormat);
        Assert.Equal(TimelineHeaderUnit.Day, config.SecondaryUnit);
        Assert.Equal("date.day-number", config.SecondaryFormat);
        Assert.True(config.ShowPrimary);
        Assert.True(config.ShowSecondary);
    }

    [Fact]
    public void GetHeaderConfiguration_MonthWeekLevel_ReturnsCorrectConfiguration()
    {
        // Arrange
        var zoomLevel = TimelineZoomLevel.MonthWeek;
        var effectiveDayWidth = 15.0;

        // Act
        var config = TimelineHeaderAdapter.GetHeaderConfiguration(zoomLevel, effectiveDayWidth);

        // Assert
        Assert.Equal(TimelineHeaderUnit.Quarter, config.PrimaryUnit);
        Assert.Equal("date.quarter-year", config.PrimaryFormat);
        Assert.Equal(TimelineHeaderUnit.Month, config.SecondaryUnit);
        Assert.Equal("date.month-short", config.SecondaryFormat);
    }

    [Fact]
    public void GetPeriodStart_Day_ReturnsStartOfDay()
    {
        // Arrange
        var date = new DateTime(2025, 7, 15, 14, 30, 0);

        // Act
        var result = TimelineHeaderAdapter.GetPeriodStart(date, TimelineHeaderUnit.Day);

        // Assert
        Assert.Equal(new DateTime(2025, 7, 15, 0, 0, 0), result);
    }

    [Fact]
    public void GetPeriodStart_Month_ReturnsStartOfMonth()
    {
        // Arrange
        var date = new DateTime(2025, 7, 15);

        // Act
        var result = TimelineHeaderAdapter.GetPeriodStart(date, TimelineHeaderUnit.Month);

        // Assert
        Assert.Equal(new DateTime(2025, 7, 1), result);
    }

    [Fact]
    public void GetPeriodStart_Quarter_ReturnsStartOfQuarter()
    {
        // Arrange
        var date = new DateTime(2025, 8, 15); // Q3

        // Act
        var result = TimelineHeaderAdapter.GetPeriodStart(date, TimelineHeaderUnit.Quarter);

        // Assert
        Assert.Equal(new DateTime(2025, 7, 1), result); // Q3 starts July 1
    }

    [Fact]
    public void GetDateIncrement_Day_AddsOneDay()
    {
        // Arrange
        var date = new DateTime(2025, 7, 15);
        var increment = TimelineHeaderAdapter.GetDateIncrement(TimelineHeaderUnit.Day);

        // Act
        var result = increment(date);

        // Assert
        Assert.Equal(new DateTime(2025, 7, 16), result);
    }

    [Fact]
    public void GetDateIncrement_Month_AddsOneMonth()
    {
        // Arrange
        var date = new DateTime(2025, 7, 15);
        var increment = TimelineHeaderAdapter.GetDateIncrement(TimelineHeaderUnit.Month);

        // Act
        var result = increment(date);

        // Assert
        Assert.Equal(new DateTime(2025, 8, 15), result);
    }

    [Fact]
    public void GetPeriodWidth_CalculatesCorrectWidth()
    {
        // Arrange
        var periodStart = new DateTime(2025, 7, 1);
        var effectiveDayWidth = 25.0;

        // Act - July has 31 days
        var result = TimelineHeaderAdapter.GetPeriodWidth(periodStart, TimelineHeaderUnit.Month, effectiveDayWidth);

        // Assert
        Assert.Equal(31 * 25.0, result);
    }
}
