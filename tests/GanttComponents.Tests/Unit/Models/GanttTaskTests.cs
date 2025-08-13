using GanttComponents.Models;
using GanttComponents.Models.ValueObjects;
using Xunit;

namespace GanttComponents.Tests.Unit.Models;

public class GanttTaskTests
{
    [Fact]
    public void GanttTask_ShouldInitializeWithDefaults()
    {
        // Arrange & Act
        var task = new GanttTask();

        // Assert
        Assert.Equal(0, task.Id);
        Assert.Equal(string.Empty, task.Name);
        Assert.Equal("1d", task.Duration);
        Assert.Equal(0, task.Progress);
        Assert.Equal(TaskType.FixedDuration, task.TaskType);
        Assert.Null(task.ParentId);
        Assert.Null(task.Predecessors);
    }

    [Fact]
    public void GanttTask_ShouldAcceptValidProperties()
    {
        // Arrange
        var startDate = DateTime.UtcNow.Date;
        var endDate = startDate.AddDays(5);

        // Act
        var task = new GanttTask
        {
            Id = 1,
            Name = "Test Task",
            StartDate = GanttDate.FromDateTime(startDate),
            EndDate = GanttDate.FromDateTime(endDate),
            Duration = "5d",
            Progress = 50,
            ParentId = 10,
            Predecessors = "2FS+3d",
            TaskType = TaskType.FixedWork
        };

        // Assert
        Assert.Equal(1, task.Id);
        Assert.Equal("Test Task", task.Name);
        Assert.Equal(GanttDate.FromDateTime(startDate), task.StartDate);
        Assert.Equal(GanttDate.FromDateTime(endDate), task.EndDate);
        Assert.Equal("5d", task.Duration);
        Assert.Equal(50, task.Progress);
        Assert.Equal(10, task.ParentId);
        Assert.Equal("2FS+3d", task.Predecessors);
        Assert.Equal(TaskType.FixedWork, task.TaskType);
    }

    [Theory]
    [InlineData("1d")]
    [InlineData("5d")]
    [InlineData("8h")]
    [InlineData("2w")]
    public void GanttTask_Duration_ShouldAcceptVariousFormats(string duration)
    {
        // Arrange & Act
        var task = new GanttTask { Duration = duration };

        // Assert
        Assert.Equal(duration, task.Duration);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(25)]
    [InlineData(50)]
    [InlineData(75)]
    [InlineData(100)]
    public void GanttTask_Progress_ShouldAcceptValidValues(int progress)
    {
        // Arrange & Act
        var task = new GanttTask { Progress = progress };

        // Assert
        Assert.Equal(progress, task.Progress);
    }
}
