using GanttComponents.Models;
using GanttComponents.Services;

namespace GanttComponents.Tests.Unit.Services;

/// <summary>
/// Unit tests for the Timeline Header Template Service.
/// Validates preset template configurations for all zoom levels.
/// </summary>
public class TimelineHeaderTemplateServiceTests
{
    [Fact]
    public void AllZoomLevels_HaveDefinedTemplates()
    {
        // Arrange
        var allZoomLevels = Enum.GetValues<TimelineZoomLevel>();

        // Act & Assert
        foreach (var zoomLevel in allZoomLevels)
        {
            var template = TimelineHeaderTemplateService.GetTemplate(zoomLevel);

            Assert.NotNull(template);
            Assert.NotEmpty(template.PrimaryFormat);
            Assert.NotEmpty(template.SecondaryFormat);
            Assert.NotEmpty(template.Description);
        }
    }

    [Fact]
    public void ValidateAllTemplatesDefined_ReturnsTrue()
    {
        // Act
        var result = TimelineHeaderTemplateService.ValidateAllTemplatesDefined();

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData(TimelineZoomLevel.WeekDay, "date.month-year-verbose", "date.day-number")]
    [InlineData(TimelineZoomLevel.MonthDay, "date.month-short", "date.day-number")]
    [InlineData(TimelineZoomLevel.YearQuarter, "date.decade-minimal", "date.year-minimal")]
    public void GetTemplate_ReturnsCorrectFormats(TimelineZoomLevel zoomLevel, string expectedPrimaryFormat, string expectedSecondaryFormat)
    {
        // Act
        var template = TimelineHeaderTemplateService.GetTemplate(zoomLevel);

        // Assert
        Assert.Equal(expectedPrimaryFormat, template.PrimaryFormat);
        Assert.Equal(expectedSecondaryFormat, template.SecondaryFormat);
    }

    [Fact]
    public void GetTemplate_UnknownZoomLevel_ReturnsDefaultTemplate()
    {
        // Arrange
        var invalidZoomLevel = (TimelineZoomLevel)999;

        // Act
        var template = TimelineHeaderTemplateService.GetTemplate(invalidZoomLevel);

        // Assert - Should return default template (MonthDay)
        Assert.NotNull(template);
        var defaultTemplate = TimelineHeaderTemplateService.GetTemplate(TaskDisplayConstants.DEFAULT_ZOOM_LEVEL);
        Assert.Equal(defaultTemplate.PrimaryFormat, template.PrimaryFormat);
        Assert.Equal(defaultTemplate.SecondaryFormat, template.SecondaryFormat);
    }

    [Fact]
    public void YearQuarter_HasPrimaryHidden()
    {
        // Act
        var template = TimelineHeaderTemplateService.GetTemplate(TimelineZoomLevel.YearQuarter);

        // Assert
        Assert.False(template.ShowPrimary); // Too cramped at 3px day width
        Assert.True(template.ShowSecondary);
    }

    [Fact]
    public void AllOtherLevels_ShowBothHeaders()
    {
        // Arrange
        var allLevels = Enum.GetValues<TimelineZoomLevel>()
            .Where(level => level != TimelineZoomLevel.YearQuarter);

        // Act & Assert
        foreach (var level in allLevels)
        {
            var template = TimelineHeaderTemplateService.GetTemplate(level);
            Assert.True(template.ShowPrimary, $"Level {level} should show primary header");
            Assert.True(template.ShowSecondary, $"Level {level} should show secondary header");
        }
    }

    [Fact]
    public void GetTemplateDescription_ReturnsNonEmptyDescription()
    {
        // Act
        var description = TimelineHeaderTemplateService.GetTemplateDescription(TimelineZoomLevel.WeekDay);

        // Assert
        Assert.NotEmpty(description);
        Assert.Contains("sprint planning", description.ToLower());
    }

    [Fact]
    public void GetAllTemplates_Returns13Templates()
    {
        // Act
        var allTemplates = TimelineHeaderTemplateService.GetAllTemplates();

        // Assert
        Assert.Equal(13, allTemplates.Count);

        // Verify all zoom levels are covered
        var allZoomLevels = Enum.GetValues<TimelineZoomLevel>();
        foreach (var level in allZoomLevels)
        {
            Assert.True(allTemplates.ContainsKey(level), $"Missing template for {level}");
        }
    }
}
