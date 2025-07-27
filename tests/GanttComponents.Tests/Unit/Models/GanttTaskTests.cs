using GanttComponents.Models;
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
            StartDate = startDate,
            EndDate = endDate,
            Duration = "5d",
            Progress = 50,
            ParentId = 10,
            Predecessors = "2FS+3d",
            TaskType = TaskType.FixedWork
        };

        // Assert
        Assert.Equal(1, task.Id);
        Assert.Equal("Test Task", task.Name);
        Assert.Equal(startDate, task.StartDate);
        Assert.Equal(endDate, task.EndDate);
        Assert.Equal("5d", task.Duration);
        Assert.Equal(50, task.Progress);
        Assert.Equal(10, task.ParentId);
        Assert.Equal("2FS+3d", task.Predecessors);
        Assert.Equal(TaskType.FixedWork, task.TaskType);
    }

    [Fact]
    public void GanttTask_Children_ShouldInitializeAsEmptyList()
    {
        // Arrange & Act
        var task = new GanttTask();

        // Assert
        Assert.NotNull(task.Children);
        Assert.Empty(task.Children);
    }

    [Fact]
    public void GanttTask_Assignments_ShouldInitializeAsEmptyList()
    {
        // Arrange & Act
        var task = new GanttTask();

        // Assert
        Assert.NotNull(task.Assignments);
        Assert.Empty(task.Assignments);
    }
}
