using Microsoft.EntityFrameworkCore;
using GanttComponents.Models;

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
            
            // Ignore complex properties that can't be easily mapped to database
            entity.Ignore(e => e.CustomFields);
            entity.Ignore(e => e.Baseline);
            
            // Following Syncfusion's self-referential pattern - no navigation properties needed
            // Hierarchy is managed via ParentId field only
        });
    }
}
