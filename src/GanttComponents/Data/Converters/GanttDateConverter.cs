using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using GanttComponents.Models.ValueObjects;

namespace GanttComponents.Data.Converters;

/// <summary>
/// EF Core value converter for GanttDate that enforces database-level constraints.
/// 
/// This is the ULTIMATE CHOKE POINT - no data can enter the database without
/// passing through this converter, which guarantees:
/// 1. All dates are stored as UTC DATE only (no time components)
/// 2. Database schema enforces DATE column type
/// 3. No accidental DateTime with time components can be persisted
/// 
/// DESIGN PRINCIPLE: Even if application code bypasses GanttDate validation,
/// this converter acts as the final enforcement layer at the database boundary.
/// </summary>
public class GanttDateConverter : ValueConverter<GanttDate, DateOnly>
{
    /// <summary>
    /// Converter that handles both directions of GanttDate <-> Database mapping.
    /// 
    /// To Database: GanttDate -> DateOnly (guaranteed UTC date-only)
    /// From Database: DateOnly -> GanttDate (already date-only, safe)
    /// </summary>
    public GanttDateConverter() : base(
        // WRITE TO DATABASE: GanttDate -> DateOnly
        ganttDate => ganttDate.ToDateOnly(),

        // READ FROM DATABASE: DateOnly -> GanttDate  
        dateOnly => new GanttDate(dateOnly))
    {
    }
}

/// <summary>
/// Alternative converter for nullable GanttDate fields.
/// Handles NULL values correctly while maintaining constraint enforcement.
/// </summary>
public class NullableGanttDateConverter : ValueConverter<GanttDate?, DateOnly?>
{
    public NullableGanttDateConverter() : base(
        // WRITE TO DATABASE: GanttDate? -> DateOnly?
        ganttDate => ganttDate.HasValue ? ganttDate.Value.ToDateOnly() : (DateOnly?)null,

        // READ FROM DATABASE: DateOnly? -> GanttDate?
        dateOnly => dateOnly.HasValue ? new GanttDate(dateOnly.Value) : (GanttDate?)null)
    {
    }
}

/// <summary>
/// Legacy converter for gradual migration from DateTime to GanttDate.
/// 
/// This allows existing DateTime properties to be gradually migrated while
/// still enforcing constraints at the database level. During migration:
/// 1. Change property type from DateTime to GanttDate in model
/// 2. EF Core automatically uses this converter
/// 3. Database stays as DATE column (no schema change needed)
/// 4. All constraint enforcement happens transparently
/// </summary>
public class DateTimeToGanttDateConverter : ValueConverter<GanttDate, DateTime>
{
    public DateTimeToGanttDateConverter() : base(
        // WRITE TO DATABASE: GanttDate -> DateTime (UTC midnight)
        ganttDate => ganttDate.ToUtcDateTime(),

        // READ FROM DATABASE: DateTime -> GanttDate (force UTC date-only)
        dateTime => GanttDate.FromDateTime(dateTime))
    {
    }
}
