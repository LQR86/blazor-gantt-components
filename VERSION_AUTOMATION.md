# 🤖 Automated Version Management Guide

## 🎯 **Zero-Manual-Work Approach**

This system **automatically handles versioning** so you never forget steps!

## 🚀 **How to Start a New Milestone (Automated)**

### **Option A: Use the Script (Recommended)**
```bash
# Create milestone 1.2 branch automatically
./scripts/create-milestone-branch.sh 1.2 "TimelineView Component"

# This automatically:
# ✅ Creates feature branch: feature/v0.2.0-timeline-component
# ✅ Updates version.json to v0.2.0-alpha
# ✅ Updates progress tracking files
# ✅ Sets milestone status to "in-progress"
```

### **Option B: Manual (If Script Fails)**
```bash
git checkout -b feature/v0.2.0-timeline-component
# Edit version.json manually
# Create PR when ready
```

## 🤖 **What Happens Automatically**

### **When You Create a PR:**
1. **CI validates version.json** matches branch name
2. **Milestone checker** ensures required files exist
3. **Build and test** run automatically
4. **PR template** shows milestone-specific checklist

### **When You Merge to Main:**
1. **Git tag created** automatically (v0.2.0-alpha)
2. **Release notes** generated from commits
3. **Next milestone** status updated
4. **Progress tracking** files updated

### **Never Forget Again:**
- ✅ Version numbers managed automatically
- ✅ Git tags created on merge
- ✅ Milestone validation enforced
- ✅ Progress tracking updated
- ✅ Release documentation generated

## 📋 **Current Workflow (Simplified)**

```
1. Run script: ./scripts/create-milestone-branch.sh 1.2 "TimelineView"
2. Implement features
3. Create PR (template auto-filled)
4. Merge (tags and releases automatic)
5. Repeat for next milestone
```

## � **Scalable Milestone Validation System**

### **Configuration-Driven Validation**
The system now uses **JSON configuration files** instead of hardcoded validation logic:

```
.github/milestone-validations/
├── milestone-1.1.json    # WBS Foundation requirements
├── milestone-1.2.json    # Auto-Versioning & TimelineView requirements  
├── milestone-1.3.json    # GanttComposer requirements
└── template.json         # Template for new milestones
```

### **Adding New Milestones**
```bash
# 1. Copy template
cp .github/milestone-validations/template.json .github/milestone-validations/milestone-1.4.json

# 2. Edit the configuration
{
  "milestone": "1.4",
  "description": "Resource Management System",
  "phases": [
    {
      "name": "Resource Assignment Component",
      "validations": [
        {
          "type": "file_exists",
          "path": "src/GanttComponents/Components/ResourceManager/ResourceManager.razor",
          "description": "Resource manager component"
        }
      ]
    }
  ]
}

# 3. No workflow changes needed! ✅
```

### **Benefits of New System**
- ✅ **Scalable**: Add milestones without touching workflow code
- ✅ **Maintainable**: Clear separation of validation rules
- ✅ **Flexible**: Different phases can have different requirements
- ✅ **Version-Controlled**: Validation rules tracked with code

## �🔧 **Emergency Manual Override**

If automation fails, you can always:
```bash
# Update version manually
jq '.version = "0.2.0-alpha" | .milestone = "1.2"' version.json > version.json.tmp
mv version.json.tmp version.json

# Create tag manually
git tag -a v0.2.0-alpha -m "Milestone 1.2: TimelineView Component"
git push origin v0.2.0-alpha
```

## 🎯 **Summary: Simplified Automated Versioning**

### **What You Get:**
1. **🤖 Zero Manual Work**: Script handles everything
2. **✅ Never Forget**: CI/CD enforces version updates
3. **📋 Auto-Validation**: Milestone requirements checked automatically via JSON config
4. **🏷️ Auto-Tagging**: Git tags created on merge
5. **📊 Progress Tracking**: Files updated automatically
6. **🔧 Scalable Validation**: Add new milestones without touching workflow code

### **Your Simple Workflow:**
```bash
# 1. Start new milestone (everything automated)
./scripts/create-milestone-branch.sh 1.2 "TimelineView Component"

# 2. Implement features
# ... code TimelineView component ...

# 3. Create PR (template pre-filled)
git push -u origin feature/v0.2.0-timeline-component

# 4. Merge (tags and releases automatic)

# 5. Add future milestones (just create JSON config - no workflow changes!)
cp .github/milestone-validations/template.json .github/milestone-validations/milestone-1.4.json
# Edit milestone-1.4.json with your requirements
```

### **How It Prevents Forgetting:**
- ❌ **Can't create PR** without updating version.json
- ❌ **Can't merge** without required milestone files (validated via JSON config)
- ❌ **Can't deploy** without passing version validation
- ✅ **Auto-creates tags** when you merge
- ✅ **Auto-updates progress** tracking
- ✅ **Scalable validation** via configuration files (no hardcoded logic!)

### **Validation System Evolution:**
- **Before**: Hardcoded validation logic in GitHub workflow (unmaintainable)
- **Now**: JSON configuration files + dynamic validation script (scalable)
- **Future**: Add new milestones by just creating JSON files!