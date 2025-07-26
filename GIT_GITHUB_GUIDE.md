# Git, GitHub & GitHub Actions - Complete Beginner Guide

## 🎯 **What You'll Learn**
This guide will take you from complete beginner to confidently using Git, GitHub, and GitHub Actions for your Blazor Gantt project.

---

## 📚 **Part 1: Understanding the Basics**

### **What is Git?**
- **Git** = Version control system (tracks changes to your code)
- **Local Repository** = Git folder on your computer (`.git` folder)
- **Working Directory** = Your actual project files
- **Staging Area** = Temporary area before committing changes

### **What is GitHub?**
- **GitHub** = Cloud service that hosts Git repositories
- **Remote Repository** = Your code stored on GitHub's servers
- **Origin** = Default name for your main remote repository

### **What are GitHub Actions?**
- **GitHub Actions** = Automation system that runs when things happen to your code
- **Workflow** = Automated process (like build, test, deploy)
- **Trigger** = Event that starts a workflow (push, pull request, schedule)

---

## 🛠️ **Part 2: Essential Git Commands You'll Use Daily**

### **Checking Status and History**
```bash
# See what files have changed
git status

# See your commit history
git log --oneline

# See what branch you're on
git branch

# See all remotes
git remote -v
```

### **Making Changes**
```bash
# Add specific files to staging
git add filename.cs
git add Components/TaskGrid/

# Add all changes
git add .

# Commit your changes
git commit -m "Add new feature description"

# Push to GitHub
git push

# Pull latest changes from GitHub
git pull
```

### **Working with Branches**
```bash
# Create and switch to new branch
git checkout -b feature/timeline-view

# Switch to existing branch
git checkout main

# See all branches
git branch -a

# Delete a branch
git branch -d feature-name
```

---

## 🌐 **Part 3: GitHub Web Interface Tour**

### **Your Repository Dashboard**
Navigate to: https://github.com/LQR86/blazor-gantt-components

#### **Main Areas:**
1. **📁 Code Tab**: Your files and folders
2. **🔄 Actions Tab**: See running workflows
3. **📊 Insights Tab**: Repository statistics
4. **⚙️ Settings Tab**: Repository configuration

#### **Key Features on Code Tab:**
```
Repository Home Page:
├── 📄 README.md (project description)
├── 🔍 Search box (find files/code)
├── 📝 Latest commit message
├── 🌿 Branch selector (currently: main)
├── 📋 Clone button (get repository URL)
└── 📁 File browser
```

### **Understanding Commits**
- Each commit = snapshot of your code at a point in time
- Click on commit messages to see what changed
- Green = added lines, Red = deleted lines

---

## 🚀 **Part 4: GitHub Actions Deep Dive**

### **Where to Find Your Workflows**
1. Go to https://github.com/LQR86/blazor-gantt-components
2. Click **"Actions"** tab
3. You'll see your 4 workflows:

#### **Your Active Workflows:**
```
📋 CI/CD Pipeline
├── Triggers: Push to main/develop, Pull Requests
├── Purpose: Build, test, deploy your app
└── Status: 🟢 Pass / 🔴 Fail

🔒 Security & Dependencies  
├── Triggers: Weekly schedule (Mondays 9 AM)
├── Purpose: Scan for vulnerabilities
└── Creates: Security reports

⚡ Performance Monitoring
├── Triggers: Daily schedule (2 AM) + Push to main
├── Purpose: Performance tests, bundle size
└── Creates: Performance reports

🔧 Feature Development
├── Triggers: Push to feature branches
├── Purpose: Validate feature development
└── Status: Quick validation checks
```

### **Reading Workflow Status**
```
Workflow Run Status:
🟢 ✅ Success (green checkmark)
🔴 ❌ Failed (red X)
🟡 ⏳ Running (yellow circle)
⚪ ⭕ Cancelled (grey circle)
```

### **Exploring a Workflow Run**
Click on any workflow run to see:
1. **Jobs**: Different tasks (build, test, deploy)
2. **Steps**: Individual commands within each job
3. **Logs**: Detailed output of what happened
4. **Artifacts**: Files created during the run

---

## 📖 **Part 5: Hands-On Practice Guide**

### **Exercise 1: Explore Your Repository**
1. **Visit**: https://github.com/LQR86/blazor-gantt-components
2. **Click around**:
   - Browse your files (Components/, Models/, etc.)
   - Look at README.md
   - Check REQUIREMENTS.md
   - View basic_gantt_plan.md

### **Exercise 2: Check GitHub Actions**
1. **Go to Actions tab**
2. **Look for workflows** (might be running already!)
3. **Click on a workflow run** to see details
4. **Explore the logs** - don't worry if you don't understand everything

### **Exercise 3: Make Your First Change**
```bash
# In your terminal, make sure you're in the project directory
cd /e/CopilotWorkspace/pm2/BlazorGantt

# Check your current status
git status

# Create a simple test file
echo "# Test File" > TEST.md

# Add and commit it
git add TEST.md
git commit -m "Add test file to practice Git workflow"

# Push to GitHub
git push
```

### **Exercise 4: Watch GitHub Actions React**
1. After pushing, go back to **Actions tab**
2. You should see a **new workflow run starting**
3. Click on it and watch it progress
4. This is your CI/CD pipeline responding to your code change!

---

## 🔍 **Part 6: Understanding Your Specific Workflows**

### **CI/CD Pipeline Workflow** (Most Important)
**When it runs**: Every time you push code or create a Pull Request

**What it does**:
```
1. 🔄 Checkout your code
2. ⚙️ Setup .NET environment  
3. 📦 Restore NuGet packages
4. 🔨 Build your Blazor app
5. 🧪 Run tests
6. 📊 Generate reports
7. 🚀 Deploy (if configured)
```

**How to read it**:
- Each step shows ✅ or ❌
- Click on failed steps to see error details
- Green = everything working
- Red = something needs fixing

### **Security Workflow** (Weekly)
**When it runs**: Every Monday at 9 AM UTC

**What it does**:
```
1. 🔍 Scan for security vulnerabilities
2. 📋 Check dependency licenses
3. 🛡️ Run CodeQL analysis
4. 📄 Generate security reports
```

### **Performance Workflow** (Daily)
**When it runs**: Every day at 2 AM UTC + when you push to main

**What it does**:
```
1. 🚀 Start your Blazor app
2. 📊 Run Lighthouse performance tests
3. 📏 Measure bundle sizes
4. 🎯 Check against performance targets
```

### **Feature Development** (Branch-specific)
**When it runs**: When you work on feature branches

**What it does**:
```
1. ✅ Quick validation
2. 🧪 Component-specific tests
3. 📈 Phase progress tracking
4. 🏷️ Automatic PR labeling
```

---

## 🎯 **Part 7: Daily Workflow Patterns**

### **Typical Development Day**
```bash
# Morning: Start fresh
git pull                    # Get latest changes
git status                  # Check what you have

# During development
git add .                   # Stage your changes
git commit -m "Add feature" # Commit with clear message
git push                    # Send to GitHub

# Watch Actions tab to see if build passes
```

### **When Working on Features**
```bash
# Create feature branch
git checkout -b feature/timeline-svg-setup

# Make your changes...
# Commit frequently with good messages
git add .
git commit -m "Add SVG coordinate system setup"
git push

# When done, create Pull Request on GitHub web interface
```

### **Checking Your Project Health**
**Daily checks**:
1. Visit **Actions tab** - all workflows green?
2. Check **Security tab** - any alerts?
3. Look at **Insights → Pulse** - what changed recently?

---

## 🚨 **Part 8: Common Scenarios & Solutions**

### **Scenario 1: Workflow Fails**
**What you see**: Red ❌ in Actions tab

**What to do**:
1. Click on the failed workflow
2. Click on the failed job
3. Read the error message
4. Fix the issue in your code
5. Commit and push again

### **Scenario 2: Merge Conflicts**
**What happens**: Git can't automatically merge changes

**What to do**:
```bash
git pull                    # Try to get latest changes
# If conflicts, Git will tell you which files
# Edit those files, remove conflict markers
git add .
git commit -m "Resolve merge conflict"
git push
```

### **Scenario 3: Want to Undo Something**
```bash
# Undo last commit (keep changes)
git reset --soft HEAD~1

# Discard all local changes
git checkout .

# See what would be discarded first
git status
```

---

## 📊 **Part 9: Monitoring Your Project**

### **Key Metrics to Watch**
1. **Build Status**: Always keep workflows green
2. **Performance Targets**: 
   - TaskGrid <50KB
   - TimelineView <75KB  
   - 60fps rendering
3. **Security**: Zero critical vulnerabilities
4. **Code Quality**: Clean, passing tests

### **GitHub Notifications**
- **Watch your repository** (click Watch → All Activity)
- Get emails when workflows fail
- See when Dependabot creates PRs

### **Project Insights**
Visit **Insights tab** for:
- Commit activity graphs
- Code frequency
- Contributor statistics
- Repository traffic

---

## 🎓 **Part 10: Next Level Skills**

### **When You're Comfortable (Later)**
```bash
# Advanced Git commands
git rebase                  # Clean up commit history
git cherry-pick            # Apply specific commits
git stash                   # Temporarily save changes

# GitHub CLI (optional)
gh repo view               # View repo info
gh pr create               # Create pull request from terminal
```

### **Customizing Your Workflows**
- Edit `.github/workflows/*.yml` files
- Add new triggers or steps
- Integrate with external services

---

## 🛡️ **Part 11: Best Practices**

### **Commit Messages**
```bash
# Good commit messages
git commit -m "Add TaskGrid row selection functionality"
git commit -m "Fix alignment issue in TimelineView"
git commit -m "Update dependencies to latest versions"

# Bad commit messages  
git commit -m "fix stuff"
git commit -m "wip"
git commit -m "changes"
```

### **Branch Naming**
```bash
# Good branch names
feature/timeline-svg-rendering
bugfix/task-grid-alignment
hotfix/critical-performance-issue

# Bad branch names
test
my-branch
fixes
```

### **When to Commit**
- ✅ **DO**: Commit when you complete a logical piece of work
- ✅ **DO**: Commit before trying something risky
- ❌ **DON'T**: Commit broken code to main branch
- ❌ **DON'T**: Commit sensitive information (passwords, keys)

---

## 🎯 **Quick Reference Card**

### **Essential Commands**
```bash
git status              # What's changed?
git add .               # Stage all changes
git commit -m "msg"     # Save changes
git push                # Send to GitHub
git pull                # Get latest changes
git checkout -b branch  # Create new branch
```

### **Key GitHub URLs**
- **Your Repo**: https://github.com/LQR86/blazor-gantt-components
- **Actions**: https://github.com/LQR86/blazor-gantt-components/actions
- **Settings**: https://github.com/LQR86/blazor-gantt-components/settings

### **Workflow Status Check**
```
🟢 All workflows passing = Good to go!
🔴 Any workflows failing = Fix before continuing
🟡 Workflows running = Wait for completion
```

---

## 🚀 **Ready to Explore!**

Now you're ready to:
1. **Confidently navigate** your GitHub repository
2. **Understand** what GitHub Actions are doing
3. **Read workflow results** and fix issues
4. **Make changes** with proper Git workflow
5. **Monitor** your project health

**Next Step**: Let's explore your live GitHub Actions workflows and see what they're telling us about your Blazor Gantt project!
