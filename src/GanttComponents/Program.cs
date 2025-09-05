using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using GanttComponents.Data;
using GanttComponents.Services;
using GanttComponents.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog logging early
GanttComponents.Services.LoggingConfiguration.ConfigureLogging();
builder.Host.UseSerilog();

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
builder.Services.AddScoped<IUniversalLogger, UniversalLogger>();
builder.Services.AddScoped<IWbsCodeGenerationService, WbsCodeGenerationService>();
builder.Services.AddScoped<ISimpleTaskGeneratorService, SimpleTaskGeneratorService>();

// DateFormatHelper configured for English date formatting
builder.Services.AddScoped<DateFormatHelper>();

var app = builder.Build();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<GanttDbContext>();
    var seedService = scope.ServiceProvider.GetRequiredService<IDatabaseSeedService>();

    context.Database.EnsureCreated();

    // Seed with sample data if database is empty
    if (!context.Tasks.Any())
    {
        seedService.SeedSampleTasksAsync(context).Wait();
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

// Ensure proper logging cleanup on application shutdown
var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
lifetime.ApplicationStopping.Register(() =>
{
    GanttComponents.Services.LoggingConfiguration.CloseLogging();
});

app.Run();
