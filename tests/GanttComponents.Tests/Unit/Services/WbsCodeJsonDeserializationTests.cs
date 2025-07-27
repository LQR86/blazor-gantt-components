using Xunit;
using GanttComponents.Models;
using GanttComponents.Services;
using GanttComponents.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Moq;
using System.Text.Json;

namespace GanttComponents.Tests.Unit.Services;

public class WbsCodeJsonDeserializationTests
{
    [Fact]
    public void JsonDeserialization_IncludesWbsCode()
    {
        // Arrange
        var json = """
        [
          {
            "Id": 1,
            "Name": "Test Task",
            "WbsCode": "1.2.3",
            "StartDate": "2025-07-27",
            "EndDate": "2025-07-28",
            "Duration": "1d",
            "Progress": 0,
            "TaskType": "FixedDuration",
            "ParentId": null,
            "Predecessors": null
          }
        ]
        """;

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new TaskTypeJsonConverter() }
        };

        // Act
        var tasks = JsonSerializer.Deserialize<List<GanttTask>>(json, options);

        // Assert
        Assert.NotNull(tasks);
        Assert.Single(tasks);
        Assert.Equal("1.2.3", tasks[0].WbsCode);
        Assert.Equal("Test Task", tasks[0].Name);
    }

    [Fact]
    public void JsonDeserialization_HandlesNullWbsCode()
    {
        // Arrange
        var json = """
        [
          {
            "Id": 1,
            "Name": "Test Task",
            "StartDate": "2025-07-27",
            "EndDate": "2025-07-28",
            "Duration": "1d",
            "Progress": 0,
            "TaskType": "FixedDuration",
            "ParentId": null,
            "Predecessors": null
          }
        ]
        """;

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new TaskTypeJsonConverter() }
        };

        // Act
        var tasks = JsonSerializer.Deserialize<List<GanttTask>>(json, options);

        // Assert
        Assert.NotNull(tasks);
        Assert.Single(tasks);
        // WbsCode should default to empty string when not provided in JSON
        Assert.Equal(string.Empty, tasks[0].WbsCode);
    }

    [Fact]
    public void JsonDeserialization_MultipleTasksWithWbsCodes()
    {
        // Arrange
        var json = """
        [
          {
            "Id": 1,
            "Name": "Project Planning",
            "WbsCode": "1",
            "StartDate": "2025-07-27",
            "EndDate": "2025-08-02",
            "Duration": "5d",
            "Progress": 20,
            "TaskType": "FixedDuration",
            "ParentId": null,
            "Predecessors": null
          },
          {
            "Id": 2,
            "Name": "Requirements Analysis", 
            "WbsCode": "1.1",
            "StartDate": "2025-07-28",
            "EndDate": "2025-07-30",
            "Duration": "2d",
            "Progress": 50,
            "TaskType": "FixedDuration",
            "ParentId": 1,
            "Predecessors": null
          }
        ]
        """;

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new TaskTypeJsonConverter() }
        };

        // Act
        var tasks = JsonSerializer.Deserialize<List<GanttTask>>(json, options);

        // Assert
        Assert.NotNull(tasks);
        Assert.Equal(2, tasks.Count);
        Assert.Equal("1", tasks[0].WbsCode);
        Assert.Equal("1.1", tasks[1].WbsCode);
        Assert.Equal("Project Planning", tasks[0].Name);
        Assert.Equal("Requirements Analysis", tasks[1].Name);
    }
}
