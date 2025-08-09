namespace GanttComponents.Models;

/// <summary>
/// Defines the optimal 16-level cell-size-first zoom system for timeline visualization.
/// REVOLUTIONARY APPROACH: Optimizes for 30-70px bottom-tier cell sizes instead of taskbar visibility.
/// This creates professional, balanced visual density across ALL zoom levels.
/// 
/// Previous approach: [3, 4, 6, 8, 12, 17, 24, 34, 48, 68, 97] px → Sparse, empty-looking headers
/// New approach: [0.33→70] px day widths → ALL levels have perfect 30-70px cell density!
/// 
/// 4 pattern groups with perfect symmetry: YearQuarter(4) → QuarterMonth(4) → MonthWeek(4) → WeekDay(4)
/// All bottom-tier cells maintain 30-70px for maximum visual impact and professional appearance.
/// </summary>
public enum TimelineZoomLevel
{
    // ========================================
    // LEGACY VALUES (Phase 1 - DEPRECATED)
    // ========================================
    // These values are kept for backward compatibility but marked obsolete.
    // Will be removed in a future version. Use the new optimal values instead.

    /// <summary>
    /// [DEPRECATED] Legacy Year-Quarter level: 3px per day
    /// REPLACED BY: YearQuarterOptimal30px (better visual density)
    /// </summary>
    [Obsolete("Use YearQuarterOptimal30px instead. This will be removed in a future version.")]
    YearQuarter3px = 0,

    /// <summary>
    /// [DEPRECATED] Legacy Year-Quarter level: 4px per day
    /// REPLACED BY: YearQuarterOptimal40px (better visual density)
    /// </summary>
    [Obsolete("Use YearQuarterOptimal40px instead. This will be removed in a future version.")]
    YearQuarter4px = 1,

    /// <summary>
    /// [DEPRECATED] Legacy Year-Quarter level: 6px per day
    /// REPLACED BY: YearQuarterOptimal50px (better visual density)
    /// </summary>
    [Obsolete("Use YearQuarterOptimal50px instead. This will be removed in a future version.")]
    YearQuarter6px = 2,

    /// <summary>
    /// [DEPRECATED] Legacy Month-only level: 8px per day
    /// REPLACED BY: QuarterMonthOptimal30px (better header pattern)
    /// </summary>
    [Obsolete("Use QuarterMonthOptimal30px instead. This will be removed in a future version.")]
    Month8px = 3,

    /// <summary>
    /// [DEPRECATED] Legacy Month-only level: 12px per day
    /// REPLACED BY: QuarterMonthOptimal40px (better header pattern)
    /// </summary>
    [Obsolete("Use QuarterMonthOptimal40px instead. This will be removed in a future version.")]
    Month12px = 4,

    /// <summary>
    /// [DEPRECATED] Legacy Quarter-Month level: 17px per day
    /// REPLACED BY: MonthWeekOptimal30px (better visual density)
    /// </summary>
    [Obsolete("Use MonthWeekOptimal30px instead. This will be removed in a future version.")]
    QuarterMonth17px = 5,

    /// <summary>
    /// [DEPRECATED] Legacy Quarter-Month level: 24px per day
    /// REPLACED BY: MonthWeekOptimal50px (better visual density)
    /// </summary>
    [Obsolete("Use MonthWeekOptimal50px instead. This will be removed in a future version.")]
    QuarterMonth24px = 6,

    /// <summary>
    /// [DEPRECATED] Legacy Month-Day level: 34px per day
    /// REPLACED BY: WeekDayOptimal30px (better header pattern)
    /// </summary>
    [Obsolete("Use WeekDayOptimal30px instead. This will be removed in a future version.")]
    MonthDay34px = 7,

    /// <summary>
    /// [DEPRECATED] Legacy Month-Day level: 48px per day
    /// REPLACED BY: WeekDayOptimal45px (better header pattern)
    /// </summary>
    [Obsolete("Use WeekDayOptimal45px instead. This will be removed in a future version.")]
    MonthDay48px = 8,

    /// <summary>
    /// [DEPRECATED] Legacy Week-Day level: 68px per day
    /// REPLACED BY: WeekDayOptimal60px (optimal cell size)
    /// </summary>
    [Obsolete("Use WeekDayOptimal60px instead. This will be removed in a future version.")]
    WeekDay68px = 9,

    /// <summary>
    /// [DEPRECATED] Legacy Week-Day level: 97px per day
    /// REPLACED BY: WeekDayOptimal70px (optimal cell size)
    /// </summary>
    [Obsolete("Use WeekDayOptimal70px instead. This will be removed in a future version.")]
    WeekDay97px = 10,

    // ========================================
    // NEW VALUES (Phase 2 - OPTIMAL APPROACH)
    // ========================================
    // Revolutionary cell-size-first approach: ALL levels have perfect 30-70px visual density!

    // Year-Quarter Pattern (4 levels): 30px-70px quarter cells
    // Day widths: 0.33px-0.78px (sub-pixel) → Use color hints for task visibility

    /// <summary>
    /// Year-Quarter Optimal 30px: 0.33px per day (Level 1)
    /// Best for: Maximum timeline overview with minimal detail
    /// Pattern: Year → Quarter (30px quarter cells)
    /// Task Strategy: Color-coded background hints and density indicators
    /// Visual Quality: Perfect density, professional appearance
    /// </summary>
    YearQuarterOptimal30px = 20,

    /// <summary>
    /// Year-Quarter Optimal 40px: 0.44px per day (Level 2)
    /// Best for: Wide timeline overview with quarter breakdown
    /// Pattern: Year → Quarter (40px quarter cells)
    /// Task Strategy: Enhanced color hints with task count indicators
    /// Visual Quality: Perfect density, excellent readability
    /// </summary>
    YearQuarterOptimal40px = 21,

    /// <summary>
    /// Year-Quarter Optimal 50px: 0.56px per day (Level 3)
    /// Best for: Strategic overview with quarter detail
    /// Pattern: Year → Quarter (50px quarter cells)
    /// Task Strategy: Rich color coding with hover details
    /// Visual Quality: Perfect density, superior appearance
    /// </summary>
    YearQuarterOptimal50px = 22,

    /// <summary>
    /// Year-Quarter Optimal 70px: 0.78px per day (Level 4)
    /// Best for: Maximum quarter visibility with year context
    /// Pattern: Year → Quarter (70px quarter cells)
    /// Task Strategy: Color gradients with summary tooltips
    /// Visual Quality: Perfect density, maximum quarter readability
    /// </summary>
    YearQuarterOptimal70px = 23,

    // Quarter-Month Pattern (4 levels): 30px-70px month cells
    // Day widths: 1.0px-2.33px → Use thin lines and mini indicators

    /// <summary>
    /// Quarter-Month Optimal 30px: 1.0px per day (Level 5)
    /// Best for: Quarterly planning with monthly breakdown
    /// Pattern: Quarter → Month (30px month cells)
    /// Task Strategy: 1px vertical lines with clustering indicators
    /// Visual Quality: Perfect density, clean monthly overview
    /// </summary>
    QuarterMonthOptimal30px = 24,

    /// <summary>
    /// Quarter-Month Optimal 40px: 1.33px per day (Level 6)
    /// Best for: Enhanced quarterly view with month details
    /// Pattern: Quarter → Month (40px month cells)
    /// Task Strategy: Enhanced 1px lines with duration hints
    /// Visual Quality: Perfect density, excellent monthly readability
    /// </summary>
    QuarterMonthOptimal40px = 25,

    /// <summary>
    /// Quarter-Month Optimal 50px: 1.67px per day (Level 7)
    /// Best for: Detailed quarterly planning with month focus
    /// Pattern: Quarter → Month (50px month cells)
    /// Task Strategy: 2px lines with smart clustering
    /// Visual Quality: Perfect density, superior monthly visibility
    /// </summary>
    QuarterMonthOptimal50px = 26,

    /// <summary>
    /// Quarter-Month Optimal 60px: 2.33px per day (Level 8)
    /// Best for: Maximum monthly visibility with quarter context
    /// Pattern: Quarter → Month (60px month cells)
    /// Task Strategy: 2-3px bars with progress indicators
    /// Visual Quality: Perfect density, maximum monthly readability
    /// </summary>
    QuarterMonthOptimal60px = 27,

    // Month-Week Pattern (4 levels): 30px-70px week cells
    // Day widths: 4.29px-10px → Use mini taskbars with labels

    /// <summary>
    /// Month-Week Optimal 30px: 4.29px per day (Level 9)
    /// Best for: Monthly planning with weekly breakdown
    /// Pattern: Month → Week (30px week cells)
    /// Task Strategy: 4px mini taskbars with simple labels
    /// Visual Quality: Perfect density, clear weekly overview
    /// </summary>
    MonthWeekOptimal30px = 28,

    /// <summary>
    /// Month-Week Optimal 40px: 5.71px per day (Level 10)
    /// Best for: Enhanced monthly view with week details
    /// Pattern: Month → Week (40px week cells)
    /// Task Strategy: 5px taskbars with text labels
    /// Visual Quality: Perfect density, good weekly readability
    /// </summary>
    MonthWeekOptimal40px = 29,

    /// <summary>
    /// Month-Week Optimal 50px: 7.14px per day (Level 11)
    /// Best for: Detailed monthly planning with week focus
    /// Pattern: Month → Week (50px week cells)
    /// Task Strategy: 7px taskbars with text and progress
    /// Visual Quality: Perfect density, excellent weekly readability
    /// </summary>
    MonthWeekOptimal50px = 30,

    /// <summary>
    /// Month-Week Optimal 60px: 8.57px per day (Level 12)
    /// Best for: Maximum weekly visibility with month context
    /// Pattern: Month → Week (60px week cells)
    /// Task Strategy: 8px taskbars with full labeling
    /// Visual Quality: Perfect density, maximum weekly readability
    /// </summary>
    MonthWeekOptimal60px = 31,

    // Week-Day Pattern (4 levels): 30px-70px day cells
    // Day widths: 30px-70px → Use full taskbars with rich details

    /// <summary>
    /// Week-Day Optimal 30px: 30px per day (Level 13)
    /// Best for: Weekly planning with daily breakdown
    /// Pattern: Week → Day (30px day cells)
    /// Task Strategy: Full taskbars with names and basic progress
    /// Visual Quality: Perfect density, excellent daily overview
    /// </summary>
    WeekDayOptimal30px = 32,

    /// <summary>
    /// Week-Day Optimal 40px: 40px per day (Level 14)
    /// Best for: Enhanced weekly view with day details
    /// Pattern: Week → Day (40px day cells)
    /// Task Strategy: Rich taskbars with icons and progress
    /// Visual Quality: Perfect density, good daily readability
    /// </summary>
    WeekDayOptimal40px = 33,

    /// <summary>
    /// Week-Day Optimal 50px: 50px per day (Level 15)
    /// Best for: Detailed weekly planning with full day information
    /// Pattern: Week → Day (50px day cells)
    /// Task Strategy: Full taskbars with team, progress, and status
    /// Visual Quality: Perfect density, excellent daily readability
    /// </summary>
    WeekDayOptimal50px = 34,

    /// <summary>
    /// Week-Day Optimal 60px: 60px per day (Level 16)
    /// Best for: Maximum daily visibility with complete information
    /// Pattern: Week → Day (60px day cells)
    /// Task Strategy: Complete taskbars with all details and interactions
    /// Visual Quality: Perfect density, maximum daily readability
    /// </summary>
    WeekDayOptimal60px = 35
}
