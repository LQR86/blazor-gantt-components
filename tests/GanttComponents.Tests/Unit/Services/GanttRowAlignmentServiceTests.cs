using GanttComponents.Services;
using Xunit;

namespace GanttComponents.Tests.Unit.Services;

public class GanttRowAlignmentServiceTests
{
    [Fact]
    public void DefaultRowHeight_ShouldInitializeTo32()
    {
        // Arrange & Act
        var service = new GanttRowAlignmentService();

        // Assert
        Assert.Equal(32, service.DefaultRowHeight);
    }

    [Fact]
    public void HeaderHeight_ShouldInitializeTo40()
    {
        // Arrange & Act
        var service = new GanttRowAlignmentService();

        // Assert
        Assert.Equal(40, service.HeaderHeight);
    }

    [Fact]
    public void RowPositions_ShouldInitializeAsEmpty()
    {
        // Arrange & Act
        var service = new GanttRowAlignmentService();

        // Assert
        Assert.NotNull(service.RowPositions);
        Assert.Empty(service.RowPositions);
    }

    [Fact]
    public void UpdateRowPosition_ShouldTriggerRowPositionsChangedEvent()
    {
        // Arrange
        var service = new GanttRowAlignmentService();
        bool eventTriggered = false;
        
        service.RowPositionsChanged += _ => eventTriggered = true;

        var metrics = new RowMetrics
        {
            Index = 0,
            Height = 32,
            Top = 40,
            Visible = true,
            Expanded = true,
            TaskId = 1
        };

        // Act
        service.UpdateRowPosition(0, metrics);

        // Assert
        Assert.True(eventTriggered);
        Assert.Single(service.RowPositions);
        Assert.Equal(metrics, service.RowPositions[0]);
    }

    [Fact]
    public void GetRowByIndex_ShouldReturnCorrectRow()
    {
        // Arrange
        var service = new GanttRowAlignmentService();
        var metrics = new RowMetrics
        {
            Index = 1,
            Height = 32,
            Top = 72,
            Visible = true,
            Expanded = false,
            TaskId = 2
        };

        service.UpdateRowPosition(1, metrics);

        // Act
        var result = service.GetRowByIndex(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(metrics, result);
    }

    [Fact]
    public void GetRowByIndex_WithInvalidIndex_ShouldReturnNull()
    {
        // Arrange
        var service = new GanttRowAlignmentService();

        // Act
        var result = service.GetRowByIndex(999);

        // Assert
        Assert.Null(result);
    }
}
