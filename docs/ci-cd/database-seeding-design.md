## 🎯 **Simple Task Generator Service & Page**

### **Core Features (Simplified):**

#### **📅 Project Timeline**
- **Project Start Date** (with validation: minimum 6 months duration)
- **Project End Date** (with validation: minimum 6 months duration)
- **Auto-calculation**: Duration display (months/years)

#### **📊 Task Generation Settings**
- **Total Number of Tasks** (calculated suggestion: ~10 tasks/month based on duration)
- **Hierarchy Depth** (dropdown: 2-5 levels)
- **Tasks per Parent** (range: 3-7 children average)
- **Task Duration Range** (min: 1 day, max: 60 days)

#### **🎲 Randomization Settings**
- **Random Seed** (number input for reproducible results)
- **Random Task Names** (simple word combinations)
- **Progress Distribution** (0-100% random assignment)

### **Simplified Service Interface:**

```csharp
public interface ISimpleTaskGeneratorService
{
    Task<List<GanttTask>> GenerateTasksAsync(SimpleTaskGenerationConfig config);
    Task<TaskGenerationPreview> PreviewGenerationAsync(SimpleTaskGenerationConfig config);
    Task SeedDatabaseWithGeneratedTasksAsync(SimpleTaskGenerationConfig config);
}

public class SimpleTaskGenerationConfig
{
    public DateTime ProjectStartDate { get; set; }
    public DateTime ProjectEndDate { get; set; }
    public int TotalTaskCount { get; set; }
    public int HierarchyDepth { get; set; } = 3; // 2-5
    public int MinTasksPerParent { get; set; } = 3;
    public int MaxTasksPerParent { get; set; } = 7;
    public int MinTaskDurationDays { get; set; } = 1;
    public int MaxTaskDurationDays { get; set; } = 60;
    public int? RandomSeed { get; set; }
}
```

### **Simple Page Layout:**

```
/dev/simple-task-generator
├── 🎛️ Basic Parameters (Single Form)
│   ├── Project Start Date [Date Input]
│   ├── Project End Date [Date Input] 
│   ├── Total Tasks [Number Input with suggestion]
│   ├── Hierarchy Depth [Dropdown: 2,3,4,5]
│   ├── Tasks per Parent [Range: 3-7]
│   ├── Task Duration [Range: 1-60 days]
│   └── Random Seed [Number Input, optional]
├── 🔍 Quick Preview
│   ├── "Generate Preview" button
│   └── Simple stats (task count, date range, hierarchy)
└── 💾 Database Operations
    ├── Current database info (task count)
    ├── "Generate & Seed Database" button
    └── Success/error messages
```

### **Core Generation Logic:**

```csharp
// 1. Generate WBS hierarchy automatically (1, 1.1, 1.1.1, etc.)
// 2. Distribute tasks evenly across timeline
// 3. Random task names: "Task Alpha", "Project Beta", "Work Gamma"
// 4. Ensure parent tasks span child task dates
// 5. Random progress 0-100%
// 6. No dependencies for now (keep it simple)
```

### **Random Task Name Patterns:**
```csharp
string[] prefixes = { "Task", "Project", "Work", "Phase", "Item", "Module" };
string[] suffixes = { "Alpha", "Beta", "Gamma", "Delta", "Prime", "Core", "Main", "Sub" };
// Result: "Task Alpha", "Project Beta", "Work Gamma", etc.
```

### **Validation Rules:**
- ✅ Project duration >= 6 months
- ✅ Task count reasonable (suggest ~10/month)
- ✅ Hierarchy depth 2-5 levels
- ✅ Date logic (end > start)
- ✅ Positive numbers only

### **Quick Implementation Plan:**

1. **Create Service**: `SimpleTaskGeneratorService`
2. **Create Page**: `/Pages/Dev/SimpleTaskGenerator.razor`
3. **Basic Algorithm**: WBS generation + date distribution
4. **Simple UI**: Single form with immediate feedback
5. **Database Integration**: Clear existing + seed new

This gives you exactly what you need - a simple, focused tool for generating test data spanning 3-5 years with proper hierarchy and random task names, without the complexity of advanced features you don't currently need.