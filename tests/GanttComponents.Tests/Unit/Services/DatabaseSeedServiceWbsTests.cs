using Xunit;
using GanttComponents.Models;
using GanttComponents.Services;
using GanttComponents.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Moq;

namespace GanttComponents.Tests.Unit.Services;

public class DatabaseSeedServiceWbsTests : IDisposable
{
    private readonly GanttDbContext _context;
    private readonly DatabaseSeedService _seedService;
    private readonly string _tempFilePath;

    public DatabaseSeedServiceWbsTests()
    {
        var options = new DbContextOptionsBuilder<GanttDbContext>()
            .UseSqlite($"Data Source=:memory:")
            .Options;
        
        _context = new GanttDbContext(options);
        _context.Database.OpenConnection();
        _context.Database.EnsureCreated();

        var mockLogger = new Mock<ILogger<DatabaseSeedService>>();
        var mockEnvironment = new Mock<IWebHostEnvironment>();
        
        // Create a temporary JSON file for testing
        _tempFilePath = Path.Combine(Path.GetTempPath(), $"test-tasks-{Guid.NewGuid()}.json");
        var testJson = """
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
          },
          {
            "Id": 3,
            "Name": "System Design",
            "WbsCode": "2",
            "StartDate": "2025-08-03",
            "EndDate": "2025-08-10",
            "Duration": "6d",
            "Progress": 0,
            "TaskType": "FixedDuration",
            "ParentId": null,
            "Predecessors": "1FS"
          }
        ]
        """;
        
        File.WriteAllText(_tempFilePath, testJson);
        mockEnvironment.Setup(e => e.ContentRootPath).Returns(Path.GetTempPath());

        _seedService = new DatabaseSeedService(mockLogger.Object, mockEnvironment.Object);
    }

    [Fact]
    public async Task SeedTasksFromJsonAsync_LoadsWbsCodesCorrectly()
    {
        // Arrange
        var fileName = Path.GetFileName(_tempFilePath);

        // Act
        await _seedService.SeedTasksFromJsonAsync(fileName, _context);

        // Assert
        var tasks = await _context.Tasks.OrderBy(t => t.Id).ToListAsync();
        Assert.Equal(3, tasks.Count);
        
        Assert.Equal("1", tasks[0].WbsCode);
        Assert.Equal("Project Planning", tasks[0].Name);
        
        Assert.Equal("1.1", tasks[1].WbsCode);
        Assert.Equal("Requirements Analysis", tasks[1].Name);
        
        Assert.Equal("2", tasks[2].WbsCode);
        Assert.Equal("System Design", tasks[2].Name);
    }

    [Fact]
    public async Task SeedTasksFromJsonAsync_PersistsWbsCodesToDatabaseCorrectly()
    {
        // Arrange
        var fileName = Path.GetFileName(_tempFilePath);

        // Act
        await _seedService.SeedTasksFromJsonAsync(fileName, _context);

        // Assert - Verify data is persisted by reading from a fresh context
        using var freshContext = new GanttDbContext(new DbContextOptionsBuilder<GanttDbContext>()
            .UseSqlite(_context.Database.GetDbConnection())
            .Options);
            
        var tasks = await freshContext.Tasks.OrderBy(t => t.Id).ToListAsync();
        Assert.Equal(3, tasks.Count);
        Assert.All(tasks, task => Assert.NotNull(task.WbsCode));
        Assert.Equal("1", tasks[0].WbsCode);
        Assert.Equal("1.1", tasks[1].WbsCode);
        Assert.Equal("2", tasks[2].WbsCode);
    }

    public void Dispose()
    {
        _context?.Database.CloseConnection();
        _context?.Dispose();
        if (File.Exists(_tempFilePath))
        {
            File.Delete(_tempFilePath);
        }
    }
}
