using GanttComponents.Models;
using GanttComponents.Services;
using Xunit;

namespace GanttComponents.Tests.Unit.Services;

/// <summary>
/// Tests for MonthWeek zoom level header template implementations.
/// Validates the three MonthWeek pattern levels following the WeekDay implementation pattern.
/// </summary>
public class MonthWeekHeaderTemplateTests
{
    [Fact]
    public void MonthWeek_Template_ShouldHaveCorrectMonthWeekConfiguration()
    {
        // Arrange & Act
        var template = TimelineHeaderTemplateService.GetTemplate(TimelineZoomLevel.MonthWeek);

        // Assert
        Assert.Equal(TimelineHeaderUnit.Month, template.PrimaryUnit);
        Assert.Equal("date.month-year", template.PrimaryFormat);
        Assert.Equal(TimelineHeaderUnit.Week, template.SecondaryUnit);
        Assert.Equal("date.week-start-day", template.SecondaryFormat);
        Assert.True(template.ShowPrimary);
        Assert.True(template.ShowSecondary);
        Assert.Equal("Monthly overview with weekly breakdown", template.Description);
    }

    [Fact]
    public void MonthWeekMedium_Template_ShouldHaveCorrectMonthWeekConfiguration()
    {
        // Arrange & Act
        var template = TimelineHeaderTemplateService.GetTemplate(TimelineZoomLevel.MonthWeekMedium);

        // Assert
        Assert.Equal(TimelineHeaderUnit.Month, template.PrimaryUnit);
        Assert.Equal("date.month-abbrev", template.PrimaryFormat);
        Assert.Equal(TimelineHeaderUnit.Week, template.SecondaryUnit);
        Assert.Equal("date.week-start", template.SecondaryFormat);
        Assert.True(template.ShowPrimary);
        Assert.True(template.ShowSecondary);
        Assert.Equal("Medium monthly view with weekly periods", template.Description);
    }

    [Fact]
    public void MonthWeekLow_Template_ShouldHaveCorrectMonthWeekConfiguration()
    {
        // Arrange & Act
        var template = TimelineHeaderTemplateService.GetTemplate(TimelineZoomLevel.MonthWeekLow);

        // Assert
        Assert.Equal(TimelineHeaderUnit.Month, template.PrimaryUnit);
        Assert.Equal("date.month-abbrev", template.PrimaryFormat);
        Assert.Equal(TimelineHeaderUnit.Week, template.SecondaryUnit);
        Assert.Equal("date.week-start", template.SecondaryFormat);
        Assert.True(template.ShowPrimary);
        Assert.True(template.ShowSecondary);
        Assert.Equal("Compact monthly view with weekly periods", template.Description);
    }

    [Theory]
    [InlineData(TimelineZoomLevel.MonthWeek, 40.0)] // 25 * 1.6 for backward compatibility  
    [InlineData(TimelineZoomLevel.MonthWeekMedium, 32.0)] // 20 * 1.6 for backward compatibility
    [InlineData(TimelineZoomLevel.MonthWeekLow, 28.8)] // 18 * 1.6 for backward compatibility
    public void MonthWeek_ZoomConfiguration_ShouldHaveCorrectDayWidth(TimelineZoomLevel level, double expectedDayWidth)
    {
        // Arrange & Act
        var config = TimelineZoomService.GetConfiguration(level);

        // Assert
        Assert.Equal(level, config.Level);
        Assert.Equal(expectedDayWidth, config.BaseDayWidth);
    }

    [Theory]
    [InlineData(TimelineZoomLevel.MonthWeek)]
    [InlineData(TimelineZoomLevel.MonthWeekMedium)]
    [InlineData(TimelineZoomLevel.MonthWeekLow)]
    public void MonthWeek_HeaderAdapter_ShouldGenerateValidConfiguration(TimelineZoomLevel level)
    {
        // Arrange
        var effectiveDayWidth = TimelineZoomService.GetConfiguration(level).BaseDayWidth;

        // Act
        var config = TimelineHeaderAdapter.GetHeaderConfigurationFromTemplate(level);

        // Assert
        Assert.Equal(TimelineHeaderUnit.Month, config.PrimaryUnit);
        
        // Check the correct format based on the level
        if (level == TimelineZoomLevel.MonthWeek)
        {
            Assert.Equal("date.month-year", config.PrimaryFormat);
            Assert.Equal("date.week-start-day", config.SecondaryFormat);
        }
        else
        {
            Assert.Equal("date.month-abbrev", config.PrimaryFormat);
            Assert.Equal("date.week-start", config.SecondaryFormat);
        }
        
        Assert.Equal(TimelineHeaderUnit.Week, config.SecondaryUnit);
        Assert.True(config.ShowPrimary);
        Assert.True(config.ShowSecondary);
        Assert.True(config.MinPrimaryWidth > 0);
        Assert.True(config.MinSecondaryWidth > 0);
    }

    [Fact]
    public void MonthWeek_Levels_ShouldFollowCorrectProgression()
    {
        // Arrange
        var monthWeekConfig = TimelineZoomService.GetConfiguration(TimelineZoomLevel.MonthWeek);
        var monthWeekMediumConfig = TimelineZoomService.GetConfiguration(TimelineZoomLevel.MonthWeekMedium);
        var monthWeekLowConfig = TimelineZoomService.GetConfiguration(TimelineZoomLevel.MonthWeekLow);

        // Assert - Day widths should decrease for zooming out
        Assert.True(monthWeekConfig.BaseDayWidth > monthWeekMediumConfig.BaseDayWidth);
        Assert.True(monthWeekMediumConfig.BaseDayWidth > monthWeekLowConfig.BaseDayWidth);

        // Assert - All should use Month -> Week pattern
        var monthWeekTemplate = TimelineHeaderTemplateService.GetTemplate(TimelineZoomLevel.MonthWeek);
        var monthWeekMediumTemplate = TimelineHeaderTemplateService.GetTemplate(TimelineZoomLevel.MonthWeekMedium);
        var monthWeekLowTemplate = TimelineHeaderTemplateService.GetTemplate(TimelineZoomLevel.MonthWeekLow);

        Assert.Equal(TimelineHeaderUnit.Month, monthWeekTemplate.PrimaryUnit);
        Assert.Equal(TimelineHeaderUnit.Month, monthWeekMediumTemplate.PrimaryUnit);
        Assert.Equal(TimelineHeaderUnit.Month, monthWeekLowTemplate.PrimaryUnit);

        Assert.Equal(TimelineHeaderUnit.Week, monthWeekTemplate.SecondaryUnit);
        Assert.Equal(TimelineHeaderUnit.Week, monthWeekMediumTemplate.SecondaryUnit);
        Assert.Equal(TimelineHeaderUnit.Week, monthWeekLowTemplate.SecondaryUnit);
    }
}
