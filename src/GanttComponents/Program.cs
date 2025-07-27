using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using GanttComponents.Data;
using GanttComponents.Services;
using GanttComponents.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();

// Add SQLite database
builder.Services.AddDbContext<GanttDbContext>(options =>
    options.UseSqlite("Data Source=gantt.db"));

// Add Gantt services
builder.Services.AddScoped<GanttRowAlignmentService>();
builder.Services.AddScoped<IGanttTaskService, GanttTaskService>();
builder.Services.AddScoped<IDatabaseSeedService, DatabaseSeedService>();

var app = builder.Build();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<GanttDbContext>();
    var seedService = scope.ServiceProvider.GetRequiredService<IDatabaseSeedService>();
    
    context.Database.EnsureCreated();
    
    // Seed with sample data from JSON if database is empty
    if (!context.Tasks.Any())
    {
        seedService.SeedTasksFromJsonAsync("Data/SeedData/sample-tasks.json", context).Wait();
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
