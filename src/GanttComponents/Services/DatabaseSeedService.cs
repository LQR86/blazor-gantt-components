using System.Text.Json;
using System.Text.Json.Serialization;
using GanttComponents.Models;
using GanttComponents.Data;

namespace GanttComponents.Services;

public interface IDatabaseSeedService
{
    Task SeedTasksFromJsonAsync(string jsonFilePath);
    Task SeedTasksFromJsonAsync(string jsonFilePath, GanttDbContext context);
}

public class DatabaseSeedService : IDatabaseSeedService
{
    private readonly ILogger<DatabaseSeedService> _logger;
    private readonly IWebHostEnvironment _environment;

    public DatabaseSeedService(ILogger<DatabaseSeedService> logger, IWebHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
    }

    public Task SeedTasksFromJsonAsync(string jsonFilePath)
    {
        // This overload would require a DbContext from DI, but since we're using it during startup,
        // we'll use the other overload that takes context as parameter
        throw new NotImplementedException("Use the overload that takes GanttDbContext as parameter");
    }

    public async Task SeedTasksFromJsonAsync(string jsonFilePath, GanttDbContext context)
    {
        try
        {
            var fullPath = Path.Combine(_environment.ContentRootPath, jsonFilePath);

            if (!File.Exists(fullPath))
            {
                _logger.LogWarning("Seed file not found: {FilePath}", fullPath);
                return;
            }

            var jsonContent = await File.ReadAllTextAsync(fullPath);
            var tasks = JsonSerializer.Deserialize<List<GanttTask>>(jsonContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new TaskTypeJsonConverter() }
            });

            if (tasks == null || !tasks.Any())
            {
                _logger.LogWarning("No tasks found in seed file: {FilePath}", fullPath);
                return;
            }

            // Clear existing data (for development/testing)
            context.Tasks.RemoveRange(context.Tasks);

            // Add new tasks
            context.Tasks.AddRange(tasks);
            await context.SaveChangesAsync();

            _logger.LogInformation("Successfully seeded {Count} tasks from {FilePath}", tasks.Count, jsonFilePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error seeding tasks from JSON file: {FilePath}", jsonFilePath);
            throw;
        }
    }
}

/// <summary>
/// Custom JSON converter for TaskType enum
/// </summary>
public class TaskTypeJsonConverter : JsonConverter<TaskType>
{
    public override TaskType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        return Enum.Parse<TaskType>(value ?? "FixedDuration", true);
    }

    public override void Write(Utf8JsonWriter writer, TaskType value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
