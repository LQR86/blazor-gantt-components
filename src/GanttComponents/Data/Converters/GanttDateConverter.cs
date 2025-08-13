using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using GanttComponents.Models.ValueObjects;

namespace GanttComponents.Data.Converters;

/// <summary>
/// CHOKE POINT 2: EF Core value converter enforcing database-level constraints.
/// 
/// This is the ULTIMATE choke point - no data can enter the database without
/// passing through this converter, which guarantees:
/// 1. All dates are stored as UTC DATE only (no time components)
/// 2. Database schema enforces DATE column type
/// 3. No accidental DateTime with time components can be persisted
/// </summary>
public class GanttDateConverter : ValueConverter<GanttDate, DateOnly>
{
    /// <summary>
    /// Essential converter: GanttDate <-> Database mapping.
    /// To Database: GanttDate -> DateOnly (guaranteed UTC date-only)
    /// From Database: DateOnly -> GanttDate (already date-only, safe)
    /// </summary>
    public GanttDateConverter() : base(
        ganttDate => ganttDate.ToDateOnly(),
        dateOnly => new GanttDate(dateOnly))
    {
    }
}
