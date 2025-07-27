using Xunit;
using GanttComponents.Models;
using GanttComponents.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace GanttComponents.Tests.Unit.Services;

public class WbsCodeGenerationServiceTests
{
    private readonly WbsCodeGenerationService _service;
    private readonly Mock<IUniversalLogger> _mockLogger;

    public WbsCodeGenerationServiceTests()
    {
        _mockLogger = new Mock<IUniversalLogger>();
        _service = new WbsCodeGenerationService(_mockLogger.Object);
    }

    [Fact]
    public async Task GenerateWbsCodesAsync_ShouldGenerateCorrectHierarchy()
    {
        // Arrange
        var tasks = new List<GanttTask>
        {
            new GanttTask { Id = 1, Name = "Project 1", ParentId = null },
            new GanttTask { Id = 2, Name = "Task 1.1", ParentId = 1 },
            new GanttTask { Id = 3, Name = "Task 1.2", ParentId = 1 },
            new GanttTask { Id = 4, Name = "Project 2", ParentId = null },
            new GanttTask { Id = 5, Name = "Task 1.1.1", ParentId = 2 },
            new GanttTask { Id = 6, Name = "Task 2.1", ParentId = 4 }
        };

        // Act
        var result = await _service.GenerateWbsCodesAsync(tasks);

        // Assert
        Assert.Equal("1", result.First(t => t.Id == 1).WbsCode);
        Assert.Equal("1.1", result.First(t => t.Id == 2).WbsCode);
        Assert.Equal("1.2", result.First(t => t.Id == 3).WbsCode);
        Assert.Equal("2", result.First(t => t.Id == 4).WbsCode);
        Assert.Equal("1.1.1", result.First(t => t.Id == 5).WbsCode);
        Assert.Equal("2.1", result.First(t => t.Id == 6).WbsCode);
    }

    [Theory]
    [InlineData("1", true)]
    [InlineData("1.2", true)]
    [InlineData("1.2.3", true)]
    [InlineData("", false)]
    [InlineData("0", false)]
    [InlineData("1.0", false)]
    [InlineData("1.a", false)]
    [InlineData("1.", false)]
    [InlineData(".1", false)]
    public void IsValidWbsCode_ShouldValidateCorrectly(string wbsCode, bool expected)
    {
        // Act
        var result = _service.IsValidWbsCode(wbsCode);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void GenerateWbsCode_ShouldGenerateCorrectCodes()
    {
        // Test root level
        Assert.Equal("1", _service.GenerateWbsCode(null, 1));
        Assert.Equal("2", _service.GenerateWbsCode(null, 2));

        // Test child level
        Assert.Equal("1.1", _service.GenerateWbsCode("1", 1));
        Assert.Equal("1.2", _service.GenerateWbsCode("1", 2));
        Assert.Equal("1.1.1", _service.GenerateWbsCode("1.1", 1));
    }

    [Fact]
    public void IsWbsCodeUnique_ShouldCheckUniquenessCorrectly()
    {
        // Arrange
        var tasks = new List<GanttTask>
        {
            new GanttTask { Id = 1, WbsCode = "1" },
            new GanttTask { Id = 2, WbsCode = "1.1" },
            new GanttTask { Id = 3, WbsCode = "2" }
        };

        // Act & Assert
        Assert.False(_service.IsWbsCodeUnique("1", tasks));
        Assert.False(_service.IsWbsCodeUnique("1.1", tasks));
        Assert.True(_service.IsWbsCodeUnique("1.2", tasks));
        Assert.True(_service.IsWbsCodeUnique("3", tasks));

        // Test excluding a task
        Assert.True(_service.IsWbsCodeUnique("1", tasks, 1));
        Assert.False(_service.IsWbsCodeUnique("1.1", tasks, 1));
    }

    [Fact]
    public void GetNextAvailableWbsCode_ShouldReturnCorrectNext()
    {
        // Arrange
        var tasks = new List<GanttTask>
        {
            new GanttTask { Id = 1, WbsCode = "1" },
            new GanttTask { Id = 2, WbsCode = "1.1" },
            new GanttTask { Id = 3, WbsCode = "1.3" }, // Gap at 1.2
            new GanttTask { Id = 4, WbsCode = "3" }   // Gap at 2
        };

        // Act & Assert
        Assert.Equal("2", _service.GetNextAvailableWbsCode(tasks, null));
        Assert.Equal("1.2", _service.GetNextAvailableWbsCode(tasks, "1"));
        Assert.Equal("3.1", _service.GetNextAvailableWbsCode(tasks, "3"));
    }

    [Fact]
    public void ValidateWbsHierarchy_ShouldDetectErrors()
    {
        // Arrange - tasks with hierarchy issues
        var tasks = new List<GanttTask>
        {
            new GanttTask { Id = 1, Name = "Task 1", WbsCode = "1", ParentId = null },
            new GanttTask { Id = 2, Name = "Task 2", WbsCode = "1", ParentId = null }, // Duplicate WBS
            new GanttTask { Id = 3, Name = "Task 3", WbsCode = "1.1.1", ParentId = null }, // Missing parent 1.1
            new GanttTask { Id = 4, Name = "Task 4", WbsCode = "invalid", ParentId = null }, // Invalid format
            new GanttTask { Id = 5, Name = "Task 5", WbsCode = "", ParentId = null } // Empty WBS
        };

        // Act
        var errors = _service.ValidateWbsHierarchy(tasks);

        // Assert
        Assert.NotEmpty(errors);
        Assert.Contains(errors, e => e.Contains("Duplicate WBS code"));
        Assert.Contains(errors, e => e.Contains("non-existent parent"));
        Assert.Contains(errors, e => e.Contains("invalid WBS code format"));
        Assert.Contains(errors, e => e.Contains("empty WBS code"));
    }
}
