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
        var template = TimelineHeaderTemplateService.GetTemplate(TimelineZoomLevel.MonthDay48px);

        // Assert - MonthDay48px in 11-level system: Month→Day
        Assert.Equal(TimelineHeaderUnit.Month, template.PrimaryUnit);
        Assert.Equal("date.month-year", template.PrimaryFormat);
        Assert.Equal(TimelineHeaderUnit.Day, template.SecondaryUnit);
        Assert.Equal("date.day-number", template.SecondaryFormat);
        Assert.True(template.ShowPrimary);
        Assert.True(template.ShowSecondary);
        Assert.Equal("Monthly overview with daily breakdown", template.Description);
    }

    [Fact]
    public void MonthWeekMedium_Template_ShouldHaveCorrectMonthWeekConfiguration()
    {
        // Arrange & Act
        var template = TimelineHeaderTemplateService.GetTemplate(TimelineZoomLevel.MonthDay34px);

        // Assert - MonthDay34px in 11-level system: Month→Day
        Assert.Equal(TimelineHeaderUnit.Month, template.PrimaryUnit);
        Assert.Equal("date.month-abbrev", template.PrimaryFormat);
        Assert.Equal(TimelineHeaderUnit.Day, template.SecondaryUnit);
        Assert.Equal("date.day-number", template.SecondaryFormat);
        Assert.True(template.ShowPrimary);
        Assert.True(template.ShowSecondary);
        Assert.Equal("Compact monthly view with daily tracking", template.Description);
    }

    [Fact]
    public void MonthWeekLow_Template_ShouldHaveCorrectMonthWeekConfiguration()
    {
        // Arrange & Act
        var template = TimelineHeaderTemplateService.GetTemplate(TimelineZoomLevel.QuarterMonth24px);

        // Assert - QuarterMonth24px in 11-level system: Quarter→Month
        Assert.Equal(TimelineHeaderUnit.Quarter, template.PrimaryUnit);
        Assert.Equal("date.quarter-year", template.PrimaryFormat);
        Assert.Equal(TimelineHeaderUnit.Month, template.SecondaryUnit);
        Assert.Equal("date.month-abbrev", template.SecondaryFormat);
        Assert.True(template.ShowPrimary);
        Assert.True(template.ShowSecondary);
        Assert.Equal("Quarterly overview with monthly breakdown", template.Description);
    }

    [Theory]
    [InlineData(TimelineZoomLevel.MonthDay48px, 48.0)]      // 11-level system: 48px integral
    [InlineData(TimelineZoomLevel.MonthDay34px, 34.0)]      // 11-level system: 34px integral
    [InlineData(TimelineZoomLevel.QuarterMonth24px, 24.0)]  // 11-level system: 24px integral
    public void MonthWeek_ZoomConfiguration_ShouldHaveCorrectDayWidth(TimelineZoomLevel level, double expectedDayWidth)
    {
        // Arrange & Act
        var config = TimelineZoomService.GetConfiguration(level);

        // Assert
        Assert.Equal(level, config.Level);
        Assert.Equal(expectedDayWidth, config.BaseDayWidth);
    }

    [Theory]
    [InlineData(TimelineZoomLevel.MonthDay48px)]
    [InlineData(TimelineZoomLevel.MonthDay34px)]
    [InlineData(TimelineZoomLevel.QuarterMonth24px)]
    public void MonthWeek_HeaderAdapter_ShouldGenerateValidConfiguration(TimelineZoomLevel level)
    {
        // Arrange
        var effectiveDayWidth = TimelineZoomService.GetConfiguration(level).BaseDayWidth;

        // Act
        var config = TimelineHeaderAdapter.GetHeaderConfigurationFromTemplate(level);

        // Assert - Check the correct configuration based on the 11-level system
        if (level == TimelineZoomLevel.MonthDay48px)
        {
            // MonthDay48px: Month→Day with year format
            Assert.Equal(TimelineHeaderUnit.Month, config.PrimaryUnit);
            Assert.Equal("date.month-year", config.PrimaryFormat);
            Assert.Equal(TimelineHeaderUnit.Day, config.SecondaryUnit);
            Assert.Equal("date.day-number", config.SecondaryFormat);
        }
        else if (level == TimelineZoomLevel.MonthDay34px)
        {
            // MonthDay34px: Month→Day with abbrev format (observed behavior)
            Assert.Equal(TimelineHeaderUnit.Month, config.PrimaryUnit);
            Assert.Equal("date.month-abbrev", config.PrimaryFormat);
            Assert.Equal(TimelineHeaderUnit.Day, config.SecondaryUnit);
            Assert.Equal("date.day-number", config.SecondaryFormat);
        }
        else if (level == TimelineZoomLevel.QuarterMonth24px)
        {
            // QuarterMonth level: Quarter→Month  
            Assert.Equal(TimelineHeaderUnit.Quarter, config.PrimaryUnit);
            Assert.Equal("date.quarter-year", config.PrimaryFormat);
            Assert.Equal(TimelineHeaderUnit.Month, config.SecondaryUnit);
            Assert.Equal("date.month-abbrev", config.SecondaryFormat);
        }

        Assert.True(config.ShowPrimary);
        Assert.True(config.ShowSecondary);
        Assert.True(config.MinPrimaryWidth > 0);
        Assert.True(config.MinSecondaryWidth > 0);
    }

    [Fact]
    public void MonthWeek_Levels_ShouldFollowCorrectProgression()
    {
        // Arrange
        var monthWeekConfig = TimelineZoomService.GetConfiguration(TimelineZoomLevel.MonthDay48px);
        var monthWeekMediumConfig = TimelineZoomService.GetConfiguration(TimelineZoomLevel.MonthDay34px);
        var monthWeekLowConfig = TimelineZoomService.GetConfiguration(TimelineZoomLevel.QuarterMonth24px);

        // Assert - Day widths should decrease for zooming out
        Assert.True(monthWeekConfig.BaseDayWidth > monthWeekMediumConfig.BaseDayWidth);
        Assert.True(monthWeekMediumConfig.BaseDayWidth > monthWeekLowConfig.BaseDayWidth);

        // Assert - 11-level system progression: MonthDay → MonthDay → QuarterMonth
        var monthWeekTemplate = TimelineHeaderTemplateService.GetTemplate(TimelineZoomLevel.MonthDay48px);
        var monthWeekMediumTemplate = TimelineHeaderTemplateService.GetTemplate(TimelineZoomLevel.MonthDay34px);
        var monthWeekLowTemplate = TimelineHeaderTemplateService.GetTemplate(TimelineZoomLevel.QuarterMonth24px);

        // MonthDay levels use Month primary
        Assert.Equal(TimelineHeaderUnit.Month, monthWeekTemplate.PrimaryUnit);
        Assert.Equal(TimelineHeaderUnit.Month, monthWeekMediumTemplate.PrimaryUnit);
        // QuarterMonth level uses Quarter primary
        Assert.Equal(TimelineHeaderUnit.Quarter, monthWeekLowTemplate.PrimaryUnit);

        // MonthDay levels use Day secondary
        Assert.Equal(TimelineHeaderUnit.Day, monthWeekTemplate.SecondaryUnit);
        Assert.Equal(TimelineHeaderUnit.Day, monthWeekMediumTemplate.SecondaryUnit);
        // QuarterMonth level uses Month secondary
        Assert.Equal(TimelineHeaderUnit.Month, monthWeekLowTemplate.SecondaryUnit);
    }
}
