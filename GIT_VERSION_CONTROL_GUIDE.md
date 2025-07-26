# Git Version Management & Team Collaboration Guide

## 🎯 **What This Guide Covers**
Real-world Git scenarios you'll encounter daily, with practical examples for your Blazor Gantt project.

---

## 📋 **Part 1: Version Management Fundamentals**

### **Understanding Git's Timeline Model**
```
Git thinks of your project as a series of snapshots:

main:     A---B---C---D---E (latest)
          ↑   ↑   ↑   ↑   ↑
         commit snapshots of your entire project
```

### **The Three Areas of Git**
```
Working Directory  →  Staging Area  →  Repository (.git)
    (your files)        (git add)        (git commit)

Example:
📁 TaskGrid.razor (modified) → git add → git commit → saved forever
```

---

## 🌿 **Part 2: Branch Management - When & Why**

### **What Are Branches?**
Think of branches as **parallel universes** of your code:

```
main:           A---B---C---F---G (stable production code)
                    \         /
feature/timeline:    D---E---/  (experimental work)
```

### **Common Branch Scenarios**

#### **Scenario 1: Starting a New Feature**
```bash
# You want to add TimelineView component
git checkout main                    # Start from stable code
git pull                            # Get latest changes
git checkout -b feature/timeline-view # Create new branch

# Now you're in a safe space to experiment!
```

#### **Scenario 2: Daily Feature Work**
```bash
# Working on your feature
git add Components/TimelineView/
git commit -m "Add SVG coordinate system"
git push -u origin feature/timeline-view

# Keep working...
git add .
git commit -m "Implement day-to-pixel conversion"
git push
```

#### **Scenario 3: Switching Between Features**
```bash
# You're on feature/timeline-view but need to fix a bug
git checkout main                    # Switch to main
git checkout -b bugfix/task-grid-alignment # New branch for bug
# Fix the bug...
git checkout feature/timeline-view   # Back to feature work
```

### **Branch Naming Conventions**
```bash
# Feature branches
feature/timeline-svg-rendering
feature/task-grid-drag-drop
feature/dependency-arrows

# Bug fixes
bugfix/alignment-issue
bugfix/memory-leak-taskgrid

# Hot fixes (urgent production fixes)
hotfix/critical-performance-bug

# Experiments
experiment/virtual-scrolling
experiment/web-workers
```

---

## 🔄 **Part 3: Commit Management**

### **Making Good Commits**

#### **Scenario: Building TimelineView Component**
```bash
# Bad approach - one giant commit
git add .
git commit -m "Add timeline stuff"

# Good approach - logical commits
git add Components/TimelineView/TimelineView.razor
git commit -m "Add basic TimelineView component structure"

git add Components/TimelineView/TimelineView.razor.cs
git commit -m "Implement day-to-pixel coordinate conversion"

git add Components/TimelineView/TimelineView.razor.css
git commit -m "Add Material Design styling for timeline"
```

### **Commit Message Best Practices**
```bash
# Template: [Type]: [Short description]

# Good examples for your project:
git commit -m "feat: Add TaskGrid row selection with keyboard navigation"
git commit -m "fix: Resolve timeline alignment issue with TaskGrid"
git commit -m "perf: Optimize SVG rendering for 1000+ tasks"
git commit -m "docs: Update README with component usage examples"
git commit -m "test: Add unit tests for GanttTask model validation"

# Types you'll use:
# feat:   New feature
# fix:    Bug fix
# perf:   Performance improvement
# docs:   Documentation
# test:   Tests
# style:  Code formatting
# refactor: Code restructuring
```

### **When to Commit**
```bash
# ✅ Good times to commit:
# - Completed a logical piece of work
# - Before trying something risky
# - End of each coding session
# - Fixed a specific bug
# - Added a complete feature

# **❌** Bad times to commit:
# - Code doesn't compile
# - In the middle of refactoring
# - Just to backup (use git stash instead)
```

---

## 🔧 **Part 4: Fixing Mistakes & Time Travel**

### **Scenario 1: "Oops, I made a typo in my last commit"**
```bash
# Fix the typo in your code, then:
git add .
git commit --amend -m "feat: Add TaskGrid row selection (fixed typo)"

# This replaces your last commit entirely
```

### **Scenario 2: "I want to undo my last commit but keep the changes"**
```bash
git reset --soft HEAD~1

# Your commit disappears, but files stay modified
# Useful when you committed too early
```

### **Scenario 3: "I want to completely undo my last commit"**
```bash
# ⚠️⚠️⚠️ **WARNING: This is a destructive command!** ⚠️⚠️⚠️
# Deletes the commit AND the changes in your working directory.
# **Before using this command:**
# 1. Create a backup branch: `git branch backup-branch`
# 2. Or stash your changes: `git stash`
# Use this command ONLY if you're absolutely sure you don't need those changes.

git reset --hard HEAD~1
```

> **🛡️ Safer Alternatives:**
> ```bash
> # Option 1: Keep changes but undo commit
> git reset --soft HEAD~1
> 
> # Option 2: Create backup first, then reset
> git branch backup-$(date +%Y%m%d-%H%M%S)
> git reset --hard HEAD~1
> 
> # Option 3: Use revert instead (safer for shared repositories)
> git revert HEAD
> ```

### **Scenario 4: "I want to undo a commit from 3 commits ago"**
```bash
# See your commit history
git log --oneline

# Example output:
# abc123 feat: Add timeline scrolling
# def456 fix: TaskGrid alignment issue  ← Want to undo this one
# ghi789 feat: Add task progress bars

git revert def456

# Creates a new commit that undoes the changes from def456
# Safe because it doesn't rewrite history
```

### **Scenario 5: "I need to go back to how the code was yesterday"**
```bash
# Find commits from yesterday
git log --since="yesterday" --oneline

# Travel back in time (read-only)
git checkout abc123    # Go to specific commit
# Look around, test things...
git checkout main      # Return to present
```

---

## 👥 **Part 5: Team Collaboration**

### **The Pull Request Workflow**

#### **Step 1: Create Feature Branch**
```bash
git checkout main
git pull                                # Get latest team changes
git checkout -b feature/timeline-view   # Your feature branch
```

#### **Step 2: Do Your Work**
```bash
# Work on TimelineView component...
git add .
git commit -m "feat: Add SVG timeline rendering"
git push -u origin feature/timeline-view
```

#### **Step 3: Create Pull Request (on GitHub)**
1. Go to https://github.com/LQR86/blazor-gantt-components
2. Click "Compare & pull request"
3. Fill out the template:
```markdown
## What This PR Does
Adds TimelineView component with SVG-based rendering

## Changes Made
- ✅ SVG coordinate system setup
- ✅ Day-to-pixel conversion logic
- ✅ Basic task bar rendering
- ✅ Material Design styling

## Testing
- [ ] Component renders correctly
- [ ] Aligns with TaskGrid rows
- [ ] Performance tested with 100+ tasks

## Screenshots
[Add screenshots of the timeline view]
```

#### **Step 4: Code Review Process**
```bash
# Teammate suggests changes in PR comments
# You make the changes:
git add .
git commit -m "fix: Address PR feedback - improve error handling"
git push

# Changes automatically appear in the PR
```

#### **Step 5: Merge the PR**
After approval, choose merge strategy:
- **Merge commit**: Keeps full history
- **Squash and merge**: Combines all commits into one
- **Rebase and merge**: Replays commits without merge commit

### **Keeping Your Branch Updated**

#### **Scenario: Main branch has new changes while you work**
```bash
# You're on feature/timeline-view
git checkout main
git pull                    # Get latest changes
git checkout feature/timeline-view
git merge main             # Bring main changes into your branch

# Or use rebase (cleaner history):
git rebase main
```

### **Handling Merge Conflicts**

#### **When Conflicts Happen**
```bash
# Git tells you there's a conflict:
Auto-merging Components/TaskGrid/TaskGrid.razor
CONFLICT (content): Merge conflict in Components/TaskGrid/TaskGrid.razor
```

#### **Resolving Conflicts**
1. Open the conflicted file:
```html
<<<<<<< HEAD
<div class="task-grid-new-style">
=======
<div class="task-grid-old-style">
>>>>>>> main
```

2. Choose what to keep:
```html
<div class="task-grid-new-style">
```

3. Complete the merge:
```bash
git add Components/TaskGrid/TaskGrid.razor
git commit -m "merge: Resolve TaskGrid styling conflict"
```

---

## 🚀 **Part 6: Advanced Collaboration Commands**

### **Working with Remote Branches**
```bash
# See all branches (local and remote)
git branch -a

# Track a teammate's branch
git checkout -b feature/their-feature origin/feature/their-feature

# Push a new branch to GitHub
git push -u origin feature/my-new-feature

# Delete remote branch
git push origin --delete feature/old-branch
```

### **Stashing - Temporary Storage**
```bash
# Scenario: You're working but need to switch branches urgently
git stash                           # Save current work temporarily
git checkout main                   # Switch branches
# Do urgent work...
git checkout feature/timeline-view  # Back to your branch
git stash pop                       # Restore your work
```

### **Cherry-picking - Selective Commits**
```bash
# Scenario: You want just one commit from another branch
git log feature/other-branch --oneline  # Find the commit you want
git cherry-pick abc123                  # Apply just that commit
```

---

## 📖 **Part 7: Real-World Scenarios for Your Project**

### **Scenario 1: Daily Development**
```bash
# Morning routine
git checkout main
git pull                              # Get team updates
git checkout feature/timeline-view    # Your current feature
git merge main                        # Update your branch

# During development
git add Components/TimelineView/
git commit -m "feat: Add zoom level controls"
git push

# End of day
git add .
git commit -m "wip: Timeline interaction work in progress"
git push  # Backup your work
```

### **Scenario 2: Emergency Bug Fix**
```bash
# Critical bug reported in production
git checkout main
git pull
git checkout -b hotfix/critical-memory-leak

# Fix the bug quickly
git add .
git commit -m "hotfix: Fix memory leak in TaskGrid virtualization"
git push -u origin hotfix/critical-memory-leak

# Create PR for immediate review and merge
```

### **Scenario 3: Experimenting Safely**
```bash
# Want to try a risky approach
git checkout -b experiment/web-workers-timeline

# Try the experiment
# If it works: merge back
# If it fails: just delete the branch
git checkout main
git branch -D experiment/web-workers-timeline  # Delete failed experiment
```

### **Scenario 4: Preparing for Release**
```bash
# Create release branch
git checkout main
git pull
git checkout -b release/v1.0.0

# Final polish, bug fixes, version updates
git add .
git commit -m "chore: Prepare v1.0.0 release"

# Tag the release
git tag -a v1.0.0 -m "Version 1.0.0 - Initial release"
git push origin v1.0.0
```

---

## 🛠️ **Part 8: Team Workflow Best Practices**

### **Branch Strategy for Your Project**
```
main                    # Production-ready code
├── develop            # Integration branch  
├── feature/           # New features
│   ├── timeline-view
│   ├── drag-drop
│   └── dependencies
├── bugfix/           # Bug fixes
│   ├── alignment-issue
│   └── performance-fix
└── hotfix/           # Emergency fixes
    └── critical-bug
```

### **Code Review Guidelines**
```markdown
# What to Look For in PR Reviews:

✅ **Code Quality**
- Follows project conventions
- Proper error handling
- No performance issues

✅ **Architecture**
- Maintains component independence
- Follows Material Design
- Day-level precision maintained

✅ **Testing**
- Components work standalone
- Row alignment preserved
- Performance targets met

✅ **Documentation**
- Clear commit messages
- Updated README if needed
- Comments for complex logic
```

### **Communication in PRs**
```markdown
# Good PR Description Template:

## 🎯 What This Does
Brief description of the feature/fix

## 🔧 Changes Made
- Bullet points of specific changes
- Focus on the "what", not the "how"

## 🧪 Testing Done
- How you verified it works
- Edge cases tested
- Performance impact

## 📷 Screenshots/Demo
- Visual proof it works
- Before/after if applicable

## ❓ Questions for Reviewers
- Specific things you want feedback on
- Areas you're unsure about
```

---

## 🚨 **Part 9: Common Problems & Solutions**

### **Problem: "I accidentally committed to main"**
```bash
# Move the commit to a new branch
git branch feature/accidental-commit  # Create branch with the commit
git reset --hard HEAD~1               # Remove commit from main
git checkout feature/accidental-commit # Switch to new branch
```

### **Problem: "My branch is way behind main"**
```bash
# Option 1: Merge (preserves history)
git checkout feature/my-branch
git merge main

# Option 2: Rebase (cleaner history)
git checkout feature/my-branch
git rebase main
```

### **Problem: "I need to combine multiple commits"**
```bash
# Interactive rebase to squash commits
git rebase -i HEAD~3  # Last 3 commits

# Editor opens - change 'pick' to 'squash' for commits to combine
# Save and close, then edit the combined commit message
```

### **Problem: "Someone force-pushed and broke my branch"**
```bash
# Never force push to shared branches!
# If someone did, you'll need to:
git fetch origin
git reset --hard origin/branch-name  # ⚠️ Loses local changes

# Better: communicate with your team first
```

---

## 📊 **Part 10: Monitoring Your Git Health**

### **Check Your Repository Status**
```bash
# Daily health check
git status                    # What's changed locally?
git log --oneline -10        # Recent commits
git branch -a                # All branches
git remote -v                # Connected repositories

# See what's different from main
git diff main                # All changes
git diff main --name-only    # Just file names
```

### **Understanding Git History**
```bash
# Visual history
git log --graph --oneline --all

# Find when a bug was introduced
git bisect start
git bisect bad              # Current commit is bad
git bisect good v1.0.0     # Known good commit
# Git will help you find the problematic commit
```

### **Repository Cleanup**
```bash
# Remove merged branches
git branch --merged main | grep -v main | xargs git branch -d

# Clean up tracking branches
git remote prune origin

# See disk usage
git count-objects -vH
```

---

## 🎯 **Quick Reference: When to Use What**

### **Daily Commands**
```bash
git status              # "What's changed?"
git add .               # "Stage everything"
git commit -m "msg"     # "Save this snapshot"
git push                # "Send to GitHub"
git pull                # "Get latest changes"
```

### **Branch Management**
```bash
git checkout -b name    # "Start new feature"
git checkout main       # "Switch to main"
git merge branch        # "Bring changes together"
git branch -d name      # "Delete finished branch"
```

### **Fixing Problems**
```bash
git commit --amend      # "Fix last commit"
git reset --soft HEAD~1 # "Undo commit, keep changes"
git revert abc123       # "Safely undo old commit"
git stash               # "Temporarily save work"
```

### **Team Collaboration**
```bash
git fetch               # "Check for team updates"
git rebase main         # "Update my branch cleanly"
git cherry-pick abc123  # "Take just this commit"
```

---

## 🚀 **Ready for Team Development!**

Now you understand:
1. **When to create branches** and how to name them
2. **How to make meaningful commits** that tell a story
3. **How to safely undo mistakes** without losing work
4. **How to collaborate** through Pull Requests
5. **How to handle conflicts** when they arise
6. **Daily workflows** for productive development

**Next**: Let's explore your live GitHub Actions and see how they integrate with this Git workflow!
