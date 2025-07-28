# 🤖 Automated Version Management Guide

## 🎯 **Zero-Manual-Work Approach**

This system **automatically handles versioning** so you never forget steps!

## 📋 **Version Tagging Rules (ENFORCED)**

### **✅ MUST Include Version Tags:**
- **Feature branches with milestones**: `feat/v0.3.0-alpha-timelineview-component`
- **Breaking changes**: `feat!/v1.0.0-breaking-api-change`
- **Release branches**: `release/v1.0.0`
- **Major architectural changes**: `feat/v0.4.0-alpha-ganttcomposer-integration`

### **❌ MUST NOT Include Version Tags:**
- **Bug fixes**: `fix/timeline-scrolling-bug`
- **Hotfixes**: `hotfix/critical-security-patch`
- **Documentation**: `docs/update-readme`
- **Chores**: `chore/update-dependencies`
- **CI/CD fixes**: `ci/fix-build-pipeline`
- **Code formatting**: `style/eslint-fixes`
- **Refactoring**: `refactor/cleanup-services`

### **🤖 Automated Enforcement:**
- **CI/CD validates** version tags only for milestone-significant branches
- **Script creates** proper version-tagged branches automatically
- **PR validation** ensures version.json matches for versioned branches
- **Non-versioned branches** skip version checks entirely

### **Why This Convention:**
- **Features = New Versions**: Milestone components warrant version increments
- **Fixes = Patch Releases**: Multiple fixes batched into patch releases
- **Semantic Versioning**: Only significant changes drive version planning
- **Clear Intent**: Branch name immediately shows impact level

## 🚀 **How to Start a New Milestone (Automated)**

### **Option A: Use the Script (Recommended) - For Milestone Features**
```bash
# Create milestone 1.3 branch automatically
./scripts/create-milestone-branch.sh 1.3 "GanttComposer Component"

# This automatically:
# ✅ Creates feature branch: feature/v0.3.0-alpha-ganttcomposer-component
# ✅ Updates version.json to v0.3.0-alpha
# ✅ Updates progress tracking files
# ✅ Sets milestone status to "in-progress"
# ✅ Ensures proper version tagging for milestone features
```

### **Option B: Manual Branch Creation - For Non-Milestone Work**
```bash
# For bug fixes (no version tag)
git checkout -b fix/timeline-scrolling-bug

# For documentation (no version tag)
git checkout -b docs/update-api-documentation

# For chores (no version tag)
git checkout -b chore/update-dependencies

# Note: No version.json updates needed for non-milestone branches
```

### **Option C: Manual Milestone Branch (If Script Fails)**
```bash
git checkout -b feature/v0.3.0-alpha-ganttcomposer-component
# Edit version.json manually
# Create PR when ready
```

## 🤖 **What Happens Automatically**

### **For Milestone Feature Branches (with version tags):**
1. **CI validates version.json** matches branch name
2. **Milestone checker** ensures required files exist
3. **Build and test** run automatically
4. **PR template** shows milestone-specific checklist
5. **Version validation** enforced strictly

### **For Non-Milestone Branches (without version tags):**
1. **Build and test** run automatically
2. **Code quality checks** performed
3. **Security scanning** executed
4. **No version validation** (skipped)
5. **Standard PR template** used

### **When You Merge to Main:**

#### **Milestone Features:**
1. **Git tag created** automatically (v0.3.0-alpha)
2. **Release notes** generated from commits
3. **Next milestone** status updated
4. **Progress tracking** files updated

#### **Non-Milestone Changes:**
1. **No tag created** (waits for next milestone release)
2. **Changes included** in next milestone tag
3. **Standard merge** process

### **Never Forget Again:**
- ✅ Version numbers managed automatically
- ✅ Git tags created on merge
- ✅ Milestone validation enforced
- ✅ Progress tracking updated
- ✅ Release documentation generated

## 📋 **Current Workflow (Simplified)**

### **For Milestone Features:**
```
1. Run script: ./scripts/create-milestone-branch.sh 1.3 "GanttComposer Component"
2. Implement milestone features
3. Create PR with version tag in title: "feat: Complete GanttComposer Component (v0.3.0-alpha)"
4. Merge (tags and releases automatic)
5. Repeat for next milestone
```

### **For Bug Fixes & Maintenance:**
```
1. Create branch: git checkout -b fix/timeline-scrolling-bug
2. Fix the issue
3. Create PR with conventional commit: "fix: resolve timeline scrolling issue"
4. Merge (no version tag, included in next milestone)
5. Continue with normal development
```

### **Branch Naming Convention:**
```bash
# ✅ Milestone features (VERSION REQUIRED)
feat/v0.3.0-alpha-ganttcomposer-component
feat/v0.4.0-alpha-resource-management
feat/v1.0.0-beta-stable-release

# ✅ Non-milestone work (NO VERSION)
fix/timeline-scrolling-bug
fix/memory-leak-taskgrid
hotfix/critical-security-patch
docs/update-readme
chore/update-dependencies
ci/fix-build-pipeline
style/format-components
refactor/cleanup-services
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
- ❌ **Can't create milestone PR** without updating version.json
- ❌ **Can't merge milestone PR** without required milestone files (validated via JSON config)
- ❌ **Can't deploy milestone** without passing version validation
- ✅ **Auto-creates tags** when you merge milestone features
- ✅ **Auto-updates progress** tracking for milestones
- ✅ **Scalable validation** via configuration files (no hardcoded logic!)
- ✅ **Skips version checks** for non-milestone branches automatically

### **Validation Rules By Branch Type:**

#### **Milestone Branches (`feat/v*.*.*-*`):**
- ✅ **ENFORCED**: Version.json must match branch version
- ✅ **ENFORCED**: Milestone validation files must exist
- ✅ **ENFORCED**: PR title must include version tag
- ✅ **REQUIRED**: Milestone completion checklist

#### **Non-Milestone Branches (`fix/*`, `docs/*`, `chore/*`, etc.):**
- ❌ **SKIPPED**: No version validation
- ❌ **SKIPPED**: No milestone file validation
- ✅ **ENFORCED**: Standard build and test checks
- ✅ **OPTIONAL**: Standard PR template

### **Validation System Evolution:**
- **Before**: Hardcoded validation logic in GitHub workflow (unmaintainable)
- **Now**: JSON configuration files + dynamic validation script (scalable)
- **Future**: Add new milestones by just creating JSON files!

## 🔧 **PR Title Version Tagging (CRITICAL FIX NEEDED)**

### **Current Issue Identified:**
- ✅ **PR #17**: Proper version tag format: `feat: Add GanttTaskService with WBS foundation (v0.2.0-alpha)`
- ❌ **PR #19**: Missing version tag: `feat: Complete TimelineView Component (Milestone 1.2)`

### **Required Fix:**
The automation system needs to **enforce version tags in PR titles** for milestone branches:

#### **Correct Format for Milestone PRs:**
```
feat: Complete GanttComposer Component (v0.3.0-alpha)
feat: Add Resource Management System (v0.4.0-alpha)
feat!: Breaking API Changes (v1.0.0-beta)
```

#### **Correct Format for Non-Milestone PRs:**
```
fix: resolve timeline scrolling issue
docs: update API documentation  
chore: update dependencies
```

### **Enforcement Strategy:**
1. **GitHub Action**: Validate PR title format based on branch type
2. **Script Enhancement**: Auto-generate proper PR title template
3. **Branch Detection**: Automatically determine if version tag is required
4. **Clear Error Messages**: Guide developers to correct format

### **Implementation Priority:**
This fix should be implemented before the next milestone to ensure consistency across all future releases.