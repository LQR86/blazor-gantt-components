# Lean CI/CD Strategy - Branch Name Driven

> **🎯 Core Philosophy: Continuous Integration with Small, Independent Features**
> 
> This strategy implements **Lean/Agile development principles**:
> - ✅ **Small, Independent Features** - Each branch is a focused, testable unit
> - ✅ **Frequent Integration** - Daily merges to main branch (no long-lived branches)
> - ✅ **Fast Feedback** - Automated validation within minutes of push
> - ✅ **Minimal Batch Size** - Single feature per branch, single commit per milestone
> - ✅ **Risk Reduction** - Small changes = easier debugging, testing, and rollback
> 
> **Result**: Professional development velocity with enterprise-grade quality gates

## 🎯 Overview

Our CI/CD system is built on a **Branch Name Driven** architecture where the branch name is the single source of truth that determines all automation behavior. This approach combines three key architectural features to create an efficient, convention-driven system.

## 🔄 **Current DevOps Workflow**

Our development process follows a **requirements-driven, design-first** approach with small, independent feature iterations:

### **📋 Phase 1: Strategic Planning**
```
REQUIREMENTS.md → Features_Planning.md → Design Documentation
     ↓                    ↓                      ↓
[Immutable user needs] [Strategic roadmap] [Technical designs]
```

1. **REQUIREMENTS.md** - Immutable user value contract (never changes during development)
2. **Features_Planning.md** - Strategic feature roadmap with requirements mapping
3. **docs/** folder - Detailed technical designs and architecture decisions

### **⚡ Phase 2: Tactical Implementation**
```
TEMP_FILES/temp-short-term-plan.md → feat/v{version}-{feature} → PR → Merge → Repeat
           ↓                              ↓                    ↓     ↓
    [Daily milestones]           [Small feature branch]    [Review] [Integration]
```

4. **TEMP_FILES/temp-short-term-plan.md** - Break features into small, independent daily tasks
5. **Feature branch creation** - Single-day scope following branch naming conventions
6. **PR creation and review** - Automated validation and manual review
7. **Merge to main** - Automatic tagging/releases for version-tagged features
8. **Next iteration** - Immediately start next small feature

### **🎯 Key Workflow Principles**
- ✅ **Requirements stability** - REQUIREMENTS.md never changes during development
- ✅ **Strategic planning** - Features_Planning.md provides roadmap and requirements mapping
- ✅ **Design-first** - Technical designs in docs/ before implementation
- ✅ **Small iterations** - Daily features planned in temp-short-term-plan.md
- ✅ **Fast feedback** - PR/merge cycles complete within hours, not days
- ✅ **Continuous integration** - No long-lived feature branches

### **📁 File Organization**
```
Root/
├── REQUIREMENTS.md              # Immutable user needs (stakeholder contract)
├── Features_Planning.md         # Strategic roadmap (requirements → features)
├── docs/                        # Technical designs & architecture
│   ├── architecture/
│   ├── ci-cd/
│   └── testing/
├── TEMP_FILES/                  # Temporary workspace (not tracked by git)
│   ├── temp-short-term-plan.md  # Daily milestone planning
│   ├── *.json, *.csv           # Temporary data files
│   └── *.py, *.md              # Scripts, notes, experiments
└── src/                         # Source code implementation
```

## 🏗️ Three Core Architectural Features

### 1. **Convention Driven**
- **Branch naming conventions** determine all CI/CD behavior
- **No manual configuration files** - branch name contains all necessary information
- **Self-documenting workflow** - branch name tells you exactly what will happen
- **Zero configuration drift** - impossible for branch name and automation to be out of sync

### 2. **Branch-Type Filtering**
- **Different branch prefixes** trigger different job sets
- **Intelligent resource allocation** - only relevant jobs run
- **Specialized validation** - each branch type gets appropriate checks
- **Performance optimized** - no wasted CI resources

### 3. **Phase Filtering**
- **PR Phase**: Validation, testing, quality gates (runs on pull requests)
- **Post-Merge Phase**: Tagging, releases, deployment prep (runs after merge to main)
- **Clear separation of concerns** - validation vs deployment automation
- **Conditional execution** - post-merge jobs only run when appropriate

## 🌲 Branch Naming Conventions

### **Feature Branches (Version-Tagged)**
```bash
feat/v{version}-{feature-name}
├── feat/v0.5.0-task-centering          # Task bar centering feature  
├── feat/v0.6.0-dependency-rendering    # Dependency visualization
└── feat/v1.0.0-stable-release          # Production-ready release

# Automatic Extraction:
# Branch: feat/v0.5.0-task-centering
# → Version: 0.5.0
# → Feature: Task Centering
# → Tag: v0.5.0
# → Release: "v0.5.0: Task Centering Feature"
```

### **Non-Version Branches**
```bash
fix/{description}           # Bug fixes (no version tagging)
├── fix/timeline-scrolling-bug
├── fix/memory-leak-taskgrid
└── fix/date-parsing-issue

hotfix/{description}        # Critical urgent fixes
├── hotfix/security-vulnerability
└── hotfix/production-crash

docs/{description}          # Documentation updates
├── docs/update-readme
└── docs/api-documentation

chore/{description}         # Maintenance tasks
├── chore/update-dependencies
└── chore/cleanup-unused-code

ci/{description}            # CI/CD infrastructure changes
├── ci/fix-build-pipeline
└── ci/add-security-scanning

style/{description}         # Code formatting/style
refactor/{description}      # Code refactoring
test/{description}          # Test improvements
perf/{description}          # Performance optimizations
```

## 🎯 Job Categories & Filtering Matrix

### **PR Phase Jobs** (Validation & Quality Gates)

| Job Category | feat/v* | fix/* | hotfix/* | docs/* | ci/* | chore/* |
|--------------|---------|-------|----------|--------|------|---------|
| **Build & Test** | ✅ Full | ✅ Full | ✅ Full | ✅ Skip Build | ✅ Full | ✅ Full |
| **PR Validation** | ✅ + Version | ✅ Basic | ✅ Basic | ✅ Basic | ✅ Basic | ✅ Basic |
| **Hotfix Validation** | ❌ | ❌ | ✅ Security | ❌ | ❌ | ❌ |
| **Docs Validation** | ❌ | ❌ | ❌ | ✅ Lint/Links | ❌ | ❌ |
| **Dependencies** | ❌ | ❌ | ❌ | ❌ | ❌ | ✅ Security |

### **Post-Merge Phase Jobs** (Automation & Deployment)

| Job Category | Trigger | Branch Filter | Purpose |
|--------------|---------|---------------|---------|
| **Auto-Tag** | Any merge to main | `feat:` commits with version tags | Extract version from commit message, create Git tag |
| **Auto-Release** | Tag created | `feat:` commits with version tags | Create GitHub release with extracted feature name |
| **Version Tracking** | Feature merge | `feat:` commits with version tags | Update project version tracking |

## 🚀 Developer Workflow

### **Complete Development Cycle**

#### **📋 1. Strategic Planning Phase**
```bash
# Review strategic documents
cat REQUIREMENTS.md              # Understand user needs
cat Features_Planning.md         # Check feature roadmap
ls docs/                         # Review technical designs
```

#### **⚡ 2. Daily Planning Phase**
```bash
# Plan small, independent features
vim TEMP_FILES/temp-short-term-plan.md    # Break down next feature
# - Define single-day scope
# - Identify dependencies
# - Set clear success criteria
```

#### **🔧 3. Implementation Phase**
```bash
# Create feature branch with proper naming
git checkout -b feat/v0.5.0-i18n-foundation

# Implement single feature (max 1 day scope)
# ... write code ...

# Validate locally before PR
dotnet build && dotnet test
dotnet format --verify-no-changes
```

#### **🔄 4. Integration Phase**
```bash
# Create PR with conventional commit format
# Title: "feat: Add I18N foundation service (v0.5.0)"

# PR gets automatic validation:
# - Build and test
# - Code formatting check
# - Branch naming validation

# After merge → Automatic tag and release (for versioned features)
```

### **Starting a Feature (Version-Tagged)**
```bash
# 1. Plan in temp-short-term-plan.md first
echo "feat/v0.5.0-task-centering - Add horizontal task bar centering" >> TEMP_FILES/temp-short-term-plan.md

# 2. Create branch with version and feature name
git checkout -b feat/v0.5.0-task-centering

# 3. Implement feature (single-day scope)
# ... write code ...

# 4. Create PR with version tag in title
# Title: "feat: Add horizontal task bar centering (v0.5.0)"

# 5. Merge → Automatic tag and release creation
```

### **Starting a Fix/Improvement (Non-Versioned)**
```bash
# 1. Plan in temp-short-term-plan.md if needed
echo "fix/timeline-scrolling-bug - Fix timeline scroll synchronization" >> TEMP_FILES/temp-short-term-plan.md

# 2. Create branch following convention
git checkout -b fix/timeline-scrolling-bug

# 3. Fix the issue
# ... write fix ...

# 4. Create PR with conventional title
# Title: "fix: resolve timeline scrolling issue"

# 5. Merge → No version tag (included in next feature release)
```

### **Local Development Tasks**

These local tasks ensure code quality and prevent CI failures:

#### **Available VS Code Tasks**
1. **Format Code (Pre-commit)** ⚡
   - **Command**: `dotnet format --verbosity diagnostic`
   - **When**: Before committing changes
   - **VS Code**: `Ctrl+Shift+P` → "Tasks: Run Task" → "Format Code (Pre-commit)"

2. **Format and Verify (CI Check)** 🔍
   - **Command**: `dotnet format --verify-no-changes --verbosity diagnostic`
   - **When**: Before pushing (simulates CI check)
   - **VS Code**: `Ctrl+Shift+P` → "Tasks: Run Task" → "Format and Verify (CI Check)"

3. **Build** 🔨
   - **Command**: `dotnet build`
   - **VS Code**: `Ctrl+Shift+P` → "Tasks: Run Task" → "Build"

4. **Run** 🚀
   - **Command**: `dotnet run --project src/GanttComponents/`
   - **VS Code**: `Ctrl+Shift+P` → "Tasks: Run Task" → "Run"

#### **Recommended Pre-Commit Workflow**
```bash
# 1. Format code
dotnet format --verbosity diagnostic

# 2. Build and test
dotnet build && dotnet test

# 3. Verify formatting (simulates CI)
dotnet format --verify-no-changes --verbosity diagnostic

# 4. Commit if all checks pass
git add . && git commit -m "your message"
```

#### **VS Code Configuration**
Set up format-on-save for automatic formatting:
```json
{
  "editor.formatOnSave": true,
  "omnisharp.enableEditorConfigSupport": true
}
```

#### **⚠️ Critical: Formatting Enforcement**
- **GitHub Actions will FAIL** if code is not properly formatted
- **Use local Format Code task** before committing to avoid CI failures
- **All tasks use same .NET SDK** version as GitHub Actions (8.0.x)

## 🔄 Workflow Architecture

### **PR Validation Workflow** (`pr-validation.yml`)
**Triggers**: All pull requests  
**Purpose**: Branch naming and PR title validation

```yaml
# Branch Type Detection
if [[ "$BRANCH_NAME" =~ ^feat/v([0-9]+\.[0-9]+\.[0-9]+[^-]*)-(.+)$ ]]; then
  # Feature branch - require version tag in PR title
  echo "branch-type=feature-version"
elif [[ "$BRANCH_NAME" =~ ^fix/ ]]; then
  # Fix branch - standard validation
  echo "branch-type=fix"
# ... other branch types
```

### **Build & Test Workflow** (`build-and-test.yml`)
**Triggers**: All pull requests  
**Purpose**: Universal build validation with branch-specific optimizations

```yaml
# Intelligent build skipping for docs branches
if [[ "$BRANCH_NAME" =~ ^docs/ ]]; then
  echo "📚 Documentation branch - skipping build"
else
  dotnet build && dotnet test
fi
```

### **Specialized Workflows** (Branch-Type Filtered)
- **`hotfix-validation.yml`**: Only `hotfix/*` branches
- **`docs-validation.yml`**: Only `docs/*` branches  
- **`dependencies.yml`**: Only `chore/*` branches + scheduled runs

### **Post-Merge Automation** (`post-merge-automation.yml`)
**Triggers**: Push to main branch  
**Purpose**: Extract version info from merged commits and create releases

```yaml
# Handle both merge commits and squash merges
LATEST_COMMIT=$(git log -n 1 --pretty=format:"%H")
COMMIT_MESSAGE=$(git log --format=%B -n 1 "$LATEST_COMMIT")

# Check if commit message contains version tag pattern (indicates feature branch)
if [[ "$COMMIT_MESSAGE" =~ \(v([0-9]+\.[0-9]+\.[0-9]+[^)]*)\) ]]; then
  VERSION="${BASH_REMATCH[1]}"           # 0.4.0-alpha
  
  # Extract feature name from commit message
  if [[ "$COMMIT_MESSAGE" =~ ^feat:\ (.+)\ \(v[^)]+\) ]]; then
    FEATURE_NAME="${BASH_REMATCH[1]}"    # Complete GanttComposer Component
  fi
  
  # Create tag and release
  git tag -a "v$VERSION" -m "$FEATURE_NAME"
  gh release create "v$VERSION" --title "v$VERSION: $FEATURE_NAME"
fi
```

**Key Features**:
- **Squash Merge Support**: Works with both regular merges and squash merges
- **Version Detection**: Extracts version from commit message patterns
- **Smart Filtering**: Only `feat:` commits with version tags create releases
- **Graceful Handling**: Non-feature commits are safely ignored

## 📊 Performance Benefits

### **Before: Monolithic Workflow**
- All branches ran the same comprehensive validation
- Documentation changes triggered full build + security scans
- Average PR time: 8-12 minutes
- High resource waste on irrelevant checks

### **After: Branch Name Driven Filtering**
- Only relevant jobs run for each branch type
- Documentation PRs: 1-2 minutes (markdown linting only)
- Feature PRs: 3-8 minutes (full validation)
- **~60% reduction** in unnecessary workflow runs
- **~90% faster** feedback for non-feature branches

### **Resource Optimization**
| Branch Type | Before | After | Improvement |
|-------------|--------|-------|-------------|
| `docs/*` | 8-12 min | 1-2 min | 85% faster |
| `fix/*` | 8-12 min | 3-5 min | 50% faster |
| `feat/v*` | 8-12 min | 8-15 min | Focused validation |
| `hotfix/*` | 8-12 min | 2-3 min | 75% faster |

## 🔧 Advanced Features

### **Intelligent Version Extraction**
```bash
# From commit message: feat: Complete GanttComposer Component (v0.4.0-alpha)
# Extracts:
VERSION="0.4.0-alpha"                   # From version tag pattern
FEATURE_NAME="Complete GanttComposer Component"  # From commit message

# Creates:
TAG="v0.4.0-alpha"
RELEASE_TITLE="v0.4.0-alpha: Complete GanttComposer Component"
```

### **Merge Detection Methods**
1. **Primary**: Parse commit message for version tags
2. **Squash Merge Compatible**: Works with GitHub's squash merge
3. **Validation**: Regex pattern matching for version patterns
4. **Graceful Fallback**: Non-feature commits safely ignored

### **Conditional Release Creation**
- Only commits with version tags `(v*.*.*)` create releases
- Non-feature commits are safely ignored
- Prevents tag pollution from CI/docs/fix commits

## 🚨 Error Handling & Validation

### **Invalid Branch Names**
```bash
# ❌ Invalid
my-feature-branch
feature/new-component
feat/new-component  # Missing version

# ✅ Valid
feat/v1.0.0-new-component
fix/component-bug
docs/update-readme
```

### **PR Title Validation**
```bash
# ✅ Feature branch (feat/v*) - MUST include version tag
"feat: Add horizontal task bar centering (v0.5.0)"
"feat: Complete GanttComposer Component (v0.4.0)"
"feat: Implement dependency rendering (v0.6.0)"

# ✅ Non-feature branch - MUST NOT include version tag
"fix: resolve timeline scrolling issue"
"docs: update API documentation"
"chore: update dependencies"
```

### **Automated Error Messages**
- Clear guidance on proper branch naming
- Specific format requirements for PR titles
- Links to documentation for quick reference

## 🔄 Migration from Legacy System

### **What We Removed**
- ❌ `version.json` manual configuration file
- ❌ Complex milestone validation scripts
- ❌ Monolithic `version-management.yml` workflow
- ❌ Manual version synchronization

### **What We Gained**
- ✅ Zero configuration drift
- ✅ Convention-driven automation
- ✅ Faster CI feedback loops
- ✅ Clearer developer experience
- ✅ Self-documenting system

## 📚 Related Documentation

- **Workflow Validation Checklist**: `docs/testing/workflow-validation-checklist.md`
- **GitHub Actions Implementation**: `.github/workflows/`
- **Project Requirements**: `REQUIREMENTS.md`
- **Development Workflow**: `DEVELOPMENT_WORKFLOW.md`

## 🎯 Summary

The **Branch Name Driven CI/CD Strategy** combines three architectural features:

1. **Convention Driven**: Branch names contain all automation instructions
2. **Branch-Type Filtering**: Different branch types trigger different job sets  
3. **Phase Filtering**: PR validation vs post-merge automation

**Result**: A fast, efficient, self-documenting CI/CD system that scales with minimal maintenance and provides targeted feedback based on the type of change being made.
