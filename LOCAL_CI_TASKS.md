# Local CI/CD Tasks for VS Code

This document describes the local development tasks that help ensure code quality before pushing to GitHub.

## 📋 Available Tasks

### 1. **Format Code (Pre-commit)** ⚡
- **Command**: `dotnet format --verbosity diagnostic`
- **Purpose**: Automatically formats all C# code files according to project style rules
- **When to use**: Before committing changes to ensure consistent code formatting
- **VS Code**: `Ctrl+Shift+P` → "Tasks: Run Task" → "Format Code (Pre-commit)"

### 2. **Format and Verify (CI Check)** 🔍  
- **Command**: `dotnet format --verify-no-changes --verbosity diagnostic`
- **Purpose**: Verifies that all code is properly formatted (same check as GitHub Actions)
- **When to use**: Before pushing to verify your code will pass CI formatting checks
- **VS Code**: `Ctrl+Shift+P` → "Tasks: Run Task" → "Format and Verify (CI Check)"

### 3. **Build** 🔨
- **Command**: `dotnet build`
- **Purpose**: Builds the entire solution
- **VS Code**: `Ctrl+Shift+P` → "Tasks: Run Task" → "Build"

### 4. **Run** 🚀
- **Command**: `dotnet run --project src/GanttComponents/`
- **Purpose**: Starts the Blazor application
- **VS Code**: `Ctrl+Shift+P` → "Tasks: Run Task" → "Run"

## 🔄 Recommended Workflow

### Before Committing:
1. **Format Code**: Run "Format Code (Pre-commit)" task
2. **Build**: Run "Build" task to ensure no compilation errors
3. **Test**: Run `dotnet test` to ensure all tests pass
4. **Commit**: Git commit your changes

### Before Pushing:
1. **Verify Formatting**: Run "Format and Verify (CI Check)" task
2. **Final Test**: Run `dotnet test` one more time
3. **Push**: Git push to remote repository

## 🚨 GitHub Actions Checks

The following checks run automatically on every push/PR and must pass:

### Build and Test Job:
- ✅ Restore dependencies
- ✅ Build Debug configuration  
- ✅ Build Release configuration
- ✅ Run all tests
- ✅ Publish Windows artifacts

### Code Analysis Job:
- ✅ **Code formatting check** (`dotnet format --verify-no-changes`)
- ⚠️ **This will FAIL if code is not properly formatted**

### Security Scan Job:
- ✅ Check for vulnerable packages
- ✅ Check for deprecated packages

## 💡 Pro Tips

1. **Set up format-on-save** in VS Code:
   ```json
   {
     "editor.formatOnSave": true,
     "omnisharp.enableEditorConfigSupport": true
   }
   ```

2. **Use keyboard shortcuts** for common tasks:
   - `Ctrl+Shift+B` - Run default build task
   - `Ctrl+Shift+P` → "Tasks: Run Task" - Quick task runner

3. **Check task output** for detailed formatting information and any issues

4. **Always run Format Code task** before committing to avoid CI failures

## ⚠️ Important Notes

- The GitHub Actions **code formatting check will fail** if any files need formatting
- Use the local "Format Code (Pre-commit)" task to fix formatting before pushing
- The "Format and Verify" task simulates the exact CI check that runs on GitHub
- All tasks use the same .NET SDK version as GitHub Actions (8.0.x)

## 🔧 Task Configuration

Tasks are defined in `.vscode/tasks.json` and can be customized as needed. Each task includes:
- Proper problem matchers for error detection
- Detailed output presentation
- Appropriate task grouping (build, test, etc.)
