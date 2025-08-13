using Microsoft.EntityFrameworkCore;
using GanttComponents.Models;
using GanttComponents.Models.ValueObjects;
using GanttComponents.Data.Converters;

namespace GanttComponents.Data;

public class GanttDbContext : DbContext
{
    public GanttDbContext(DbContextOptions<GanttDbContext> options) : base(options)
    {
    }

    public DbSet<GanttTask> Tasks { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure GanttTask
        modelBuilder.Entity<GanttTask>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Duration).HasMaxLength(20);
            entity.Property(e => e.Predecessors).HasMaxLength(500);

            // === CHOKE POINT: DATABASE SCHEMA ENFORCEMENT ===
            // These configurations ensure that ALL dates stored in the database
            // are UTC DATE-only, regardless of what the application code does.

            // When GanttTask is migrated to use GanttDate:
            // entity.Property(e => e.StartDate)
            //       .HasColumnType("DATE")                    // Force DATE-only schema
            //       .HasConversion<GanttDateConverter>();     // Enforce constraints

            // entity.Property(e => e.EndDate)  
            //       .HasColumnType("DATE")                    // Force DATE-only schema
            //       .HasConversion<GanttDateConverter>();     // Enforce constraints

            // CHOKE POINT 2: Database enforcement with GanttDate converter
            entity.Property(e => e.StartDate)
                  .HasColumnType("DATE")                          // Database enforces DATE-only
                  .HasConversion<GanttDateConverter>();           // Enforce constraints

            entity.Property(e => e.EndDate)
                  .HasColumnType("DATE")                          // Database enforces DATE-only  
                  .HasConversion<GanttDateConverter>();           // Enforce constraints

            // Ignore complex properties that can't be easily mapped to database
            entity.Ignore(e => e.CustomFields);
            entity.Ignore(e => e.Baseline);

            // Following Syncfusion's self-referential pattern - no navigation properties needed
            // Hierarchy is managed via ParentId field only
        });
    }
}
