# Development Workflow - Blazor Gantt Components

> **Shared workflow documentation for consistent development practices between human developer and GitHub Copilot**

## 🎯 **Workflow Overview**

This document defines our standard development workflow for implementing features in the Blazor Gantt Components project, ensuring consistency, quality, and proper CI/CD integration.

---

## 🔄 **Standard Feature Development Workflow**

### **Complete Workflow Steps:**
```
1. Create Feature Branch → 2. Develop Locally → 3. Test Manually → 4. Push → 5. GitHub Actions → 6. PR Review → 7. Merge
```

### **Detailed Workflow Process:**

#### **Step 1: Create Feature Branch** 🌿
```bash
# Always start from clean main
git checkout main
git pull origin main
git checkout -b feature/[feature-name]
```

**Branch Naming Convention:**
- `feature/[feature-name]` - New features (e.g., `feature/wbs-codes-support`)
- `fix/[bug-description]` - Bug fixes (e.g., `fix/row-alignment-issue`)
- `docs/[doc-type]` - Documentation updates (e.g., `docs/api-documentation`)
- `test/[test-type]` - Test improvements (e.g., `test/integration-tests`)

#### **Step 2: Break Down into Milestones** 📋

**Every feature must be broken into small, manageable milestones:**
- **Milestone 1**: Data model changes (if needed)
- **Milestone 2**: Service/business logic implementation
- **Milestone 3**: UI component updates
- **Milestone 4**: Tests and documentation

**Each milestone = One focused commit**

#### **Step 3: Local Development Cycle** 💻

**For each milestone:**
```bash
# Always check current directory first
pwd

# Start development with hot reload
dotnet watch run

# Develop the milestone changes
# Test manually in browser at https://localhost:7138/gantt-demo

# Verify build and tests before committing
dotnet build
dotnet test
```

**Local Manual Testing Checklist (per milestone):**
- [ ] **Directory check** - Run `pwd` to confirm correct location
- [ ] **Build succeeds** without errors or warnings
- [ ] **Visual appearance** matches Material Design standards
- [ ] **Functionality works** as expected in browser
- [ ] **Performance feels smooth** (especially for TaskGrid)
- [ ] **No console errors** in browser developer tools
- [ ] **Responsive behavior** on different screen sizes

#### **Step 4: Commit with Standards** 📝

**Commit Message Format:**
```
<type>(<scope>): <description>

[optional body explaining changes]

Refs: REQUIREMENTS.md #<requirement-number>
```

**Types:**
- `feat:` - New feature
- `fix:` - Bug fix
- `docs:` - Documentation changes
- `test:` - Adding or updating tests
- `refactor:` - Code refactoring
- `style:` - CSS/styling changes
- `perf:` - Performance improvements

**Scopes:**
- `models` - Data model changes
- `services` - Service layer changes
- `taskgrid` - TaskGrid component
- `timeline` - TimelineView component
- `demo` - Demo page changes

**Example Commit:**
```bash
git add Models/GanttTask.cs Pages/GanttDemo.razor
git commit -m "feat(models): add WBS code properties to GanttTask

- Add WbsCode property to GanttTask model
- Update demo data with hierarchical WBS codes (1, 1.1, 1.1.1, 1.2, 2)
- Ensure database IDs remain internal only

Refs: REQUIREMENTS.md #12 (WBS Code Task Identification)"
```

#### **Step 5: Push and GitHub Actions** 🚀

```bash
# Push feature branch (triggers GitHub Actions)
git push origin feature/[feature-name]
```

**GitHub Actions automatically run:**
- ✅ Build verification on Windows environment
- ✅ Unit and integration tests
- ✅ Code quality checks
- ✅ Security scans
- ✅ Bundle size validation
- ✅ Windows deployment artifact generation

#### **Step 6: Create Pull Request** 📋

**PR Template:**
```markdown
## 🎯 Feature: [Feature Name]

Brief description of the feature and its purpose.

### ✅ Changes Made
- [x] Milestone 1: Description
- [x] Milestone 2: Description
- [x] Milestone 3: Description
- [x] Milestone 4: Description

### 🔍 Manual Testing Completed
- [x] Visual appearance verified
- [x] Functionality tested across browsers
- [x] Performance validated
- [x] Material Design compliance checked
- [x] Accessibility tested (keyboard navigation)

### 📊 Requirements Satisfied
- ✅ REQUIREMENT #X: Description
- ✅ Performance targets met
- ✅ No regressions introduced

### 🧪 Test Coverage
- Unit tests for business logic
- Integration tests for component interactions
- Manual verification completed
```

#### **Step 7: PR Review and Merge** ✅

**Human Developer Review Checklist:**
- [ ] **GitHub Actions all green** ✅
- [ ] **Code quality** meets standards
- [ ] **Requirements compliance** verified
- [ ] **Manual testing** if needed
- [ ] **Documentation** updated if required

**Merge Process:**
- Use **Squash Merge** to maintain clean Git history
- Update main branch triggers deployment pipeline
- Delete feature branch after successful merge

---

## 🧪 **Testing Strategy**

### **Test Project Structure**
```
tests/GanttComponents.Tests/
├── Unit/
│   ├── Models/         # Model validation tests
│   ├── Services/       # Service logic tests
│   └── Components/     # Component unit tests
├── Integration/
│   ├── TaskGrid/       # TaskGrid integration tests
│   ├── Timeline/       # TimelineView integration tests
│   └── EndToEnd/       # Full workflow tests
└── Performance/
    ├── LargeDataset/   # 1000+ tasks performance
    └── Memory/         # Memory usage tests
```

### **Test Categories**

**Unit Tests** (Fast, Isolated):
- Model validation and business logic
- Service method functionality
- Component rendering logic
- WBS code generation
- Date calculations (UTC handling)

**Integration Tests** (Component Interactions):
- TaskGrid with data binding
- Row alignment between components
- Event handling and communication
- Service integration

**Manual Testing** (Human Verification):
- Visual appearance and UX
- Cross-browser compatibility
- Performance feel and responsiveness
- Accessibility compliance
- Material Design adherence

---

## 📋 **Database Development**

### **Development Phase Approach**
- **No migrations needed** during active development
- **Delete and recreate** database as needed
- **Update seeding data** for new features
- **Focus on model design** rather than migration complexity

### **Data Management**
```bash
# When model changes require database reset
# Human developer will:
# 1. Delete existing database file
# 2. Let application recreate with updated models
# 3. Copilot updates seeding data as needed
```

---

## 🎯 **Feature Implementation Example: WBS Codes**

### **Milestone Breakdown:**
1. **Data Model Updates** → `feat(models): add WBS code properties`
2. **Service Implementation** → `feat(services): add WBS generation service`
3. **UI Integration** → `feat(taskgrid): add WBS code display`
4. **Tests & Polish** → `test: add WBS code comprehensive tests`

### **Timeline:**
- **Milestone 1-2**: Core implementation (1-2 commits)
- **Milestone 3**: UI integration (1 commit)
- **Milestone 4**: Testing and polish (1 commit)
- **Total**: One feature branch with 3-4 focused commits

---

## 🚀 **Quality Standards**

### **Code Quality Requirements:**
- ✅ **Material Design compliance** for all UI elements
- ✅ **Performance targets** maintained (1000+ rows, 60fps)
- ✅ **Accessibility standards** (WCAG AA compliance)
- ✅ **Cross-browser compatibility** (Chrome, Edge, Firefox)
- ✅ **Requirements traceability** (link to REQUIREMENTS.md)

### **Commit Quality Standards:**
- ✅ **Small, focused changes** - one concept per commit
- ✅ **Meaningful messages** - clear intent and impact
- ✅ **Working builds** - every commit should build and run
- ✅ **Test coverage** - tests updated with functionality

### **PR Quality Standards:**
- ✅ **Complete feature** - all milestones implemented
- ✅ **Documentation updated** - README, API docs as needed
- ✅ **Manual testing completed** - checklist verified
- ✅ **GitHub Actions passing** - all automated checks green

---

## 🤝 **Collaboration Guidelines**

### **Human Developer Responsibilities:**
- 🎯 **Feature prioritization** and requirements clarification
- 🔍 **Manual testing** and UX validation
- 📋 **PR review** and final approval
- 🚀 **Deployment** and release management

### **GitHub Copilot Responsibilities:**
- 💻 **Feature implementation** following this workflow
- 🧪 **Test development** and automation
- 📝 **Documentation** updates
- 🔧 **Code quality** and standards compliance

### **Communication:**
- **Always reference** REQUIREMENTS.md for feature constraints
- **Break down features** into clear milestones
- **Provide context** for implementation decisions
- **Update this workflow** as we learn and improve

---

## 📈 **Success Metrics**

### **Development Velocity:**
- ✅ **Fast feedback** - commits build and work immediately
- ✅ **Predictable delivery** - milestone-based progress
- ✅ **Quality consistency** - automated and manual verification
- ✅ **Low rework** - comprehensive testing prevents regressions

### **Code Quality:**
- ✅ **Clean Git history** - meaningful commits and messages
- ✅ **Requirements compliance** - all features satisfy constraints
- ✅ **Performance maintenance** - targets consistently met
- ✅ **Professional standards** - enterprise-grade quality

---

## 🔄 **Workflow Evolution**

This workflow document should be updated as we:
- 🎯 **Learn from experience** and optimize our process
- 🔧 **Add new tools** or change technologies
- 📋 **Refine requirements** or constraints
- 🚀 **Scale the project** to additional developers

---

**This workflow ensures we maintain high quality, consistent development practices, and clear communication throughout the Blazor Gantt Components project development.**

**Next Step**: Ready to implement WBS Codes feature following this workflow! 🎉
