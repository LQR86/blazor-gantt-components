# CI Workflow Validation Checklist

**Testing Date**: ___________  
**Tester**: ___________  
**Branch**: `ci/fix-auto-tag-workflow`

## 🧪 Test Plan Overview

This checklist validates the complete branch-specific CI workflow separation implementation. Each test verifies that only the appropriate workflows run for different branch types.

## 📋 Pre-Testing Setup

- [ ] Current branch: `ci/fix-auto-tag-workflow`
- [ ] All workflow files committed and pushed
- [ ] GitHub Actions enabled in repository
- [ ] Required secrets available (`GITHUB_TOKEN`)
- [ ] Branch protection rules configured for main

## 🎯 Test Scenarios

### Test 1: CI Branch Validation (Current Branch)
**Branch**: `ci/fix-auto-tag-workflow`  
**Expected Workflows**: Build & Test + PR Validation

- [ ] Create PR from `ci/fix-auto-tag-workflow` to `main`
- [ ] Verify `build-and-test.yml` runs ✅
- [ ] Verify `pr-validation.yml` runs ✅  
- [ ] Verify `hotfix-validation.yml` does NOT run ❌
- [ ] Verify `docs-validation.yml` does NOT run ❌
- [ ] Check all validations pass
- [ ] Merge PR and verify `post-merge-automation.yml` runs ✅

**Results**: ___________

### Test 2: Documentation Branch
**Branch**: `docs/test-validation-system`  
**Expected Workflows**: Build & Test (skipped) + PR Validation + Docs Validation

- [ ] Create branch `docs/test-validation-system`
- [ ] Make minor documentation change
- [ ] Create PR to `main`
- [ ] Verify `build-and-test.yml` skips build (branch detection) ✅
- [ ] Verify `pr-validation.yml` runs ✅
- [ ] Verify `docs-validation.yml` runs ✅
- [ ] Verify `hotfix-validation.yml` does NOT run ❌
- [ ] Check markdown linting and link checking work
- [ ] Merge and verify post-merge automation

**Results**: ___________

### Test 3: Fix Branch
**Branch**: `fix/test-basic-validation`  
**Expected Workflows**: Build & Test + PR Validation

- [ ] Create branch `fix/test-basic-validation`
- [ ] Make minor code fix
- [ ] Create PR to `main`
- [ ] Verify `build-and-test.yml` runs ✅
- [ ] Verify `pr-validation.yml` runs ✅
- [ ] Verify NO specialized workflows run ❌
- [ ] Check build and test validation passes
- [ ] Merge and verify post-merge automation

**Results**: ___________

### Test 4: Hotfix Branch
**Branch**: `hotfix/test-security-validation`  
**Expected Workflows**: Build & Test + PR Validation + Hotfix Validation

- [ ] Create branch `hotfix/test-security-validation`
- [ ] Make security-related change
- [ ] Create PR to `main`
- [ ] Verify `build-and-test.yml` runs ✅
- [ ] Verify `pr-validation.yml` runs ✅
- [ ] Verify `hotfix-validation.yml` runs ✅
- [ ] Verify security scanning completes
- [ ] Verify critical path testing runs
- [ ] Check expedited validation messaging
- [ ] Merge and verify post-merge automation

**Results**: ___________

### Test 5: Milestone Branch (Future Test)
**Branch**: `feat/v0.5.0-alpha-test-component`  
**Expected Workflows**: Build & Test + PR Validation (same as regular branches)

- [ ] Create branch `feat/v0.5.0-alpha-test-component`
- [ ] Update `version.json` to `0.5.0-alpha`
- [ ] Add milestone test component
- [ ] Create PR with title: `feat: Test Component (v0.5.0-alpha)`
- [ ] Verify `build-and-test.yml` runs (same as other branches) ✅
- [ ] Verify `pr-validation.yml` runs and validates version tag in title ✅
- [ ] Check that NO special milestone validation occurs (it doesn't exist)
- [ ] Confirm build, test, and artifact creation work normally
- [ ] Merge and verify post-merge automation creates tag + release

**Note**: Milestone branches get the same build/test as other branches. The "milestone" concept only matters for PR title validation and post-merge release creation.

**Results**: ___________

### Test 6: Chore Branch
**Branch**: `chore/test-dependency-validation`  
**Expected Workflows**: Build & Test + PR Validation + Dependency Validation

- [ ] Create branch `chore/test-dependency-validation`
- [ ] Update a dependency (or simulate dependency change)
- [ ] Create PR to `main`
- [ ] Verify `build-and-test.yml` runs ✅
- [ ] Verify `pr-validation.yml` runs ✅
- [ ] Verify dependency validation triggers
- [ ] Check vulnerability scanning
- [ ] Check license compliance checking
- [ ] Merge and verify post-merge automation

**Results**: ___________

## 🔧 Post-Merge Automation Testing

### Test 7: Auto-Tag Creation
**Trigger**: Any merge to main  

- [ ] Merge any PR to main
- [ ] Verify `post-merge-automation.yml` runs automatically
- [ ] Check version.json validation passes
- [ ] Verify Git tag creation (if version changed)
- [ ] Check tag annotation includes milestone info
- [ ] Verify tag pushed to repository

**Results**: ___________

### Test 8: Auto-Release Creation
**Trigger**: Merge milestone with status "complete"

- [ ] Set up completed milestone in version.json
- [ ] Merge milestone PR to main
- [ ] Verify auto-tag runs first
- [ ] Verify auto-release job runs after successful tag
- [ ] Check GitHub release created with correct title
- [ ] Verify comprehensive release notes generated
- [ ] Check release marked as "latest"

**Results**: ___________

## 🚨 Error Scenario Testing

### Test 9: Invalid Branch Names
- [ ] Create branch `invalid-branch-name`
- [ ] Create PR and verify `pr-validation.yml` fails ❌
- [ ] Check error message provides clear guidance
- [ ] Verify other workflows don't run unnecessarily

### Test 10: Invalid PR Titles
- [ ] Create milestone branch `feat/v0.5.0-alpha-test`
- [ ] Create PR with title missing version tag
- [ ] Verify `pr-validation.yml` fails ❌
- [ ] Check error message specifies required format

### Test 11: Missing version.json for Milestone
**Note**: This test is not applicable since no special milestone validation exists. Milestone branches are treated the same as regular branches in build-and-test.yml.

- [ ] ~~Create milestone branch without updating version.json~~
- [ ] ~~Verify milestone validation fails~~
- [ ] **Updated Test**: Verify that milestone branches build normally regardless of version.json
- [ ] Confirm PR title validation catches missing version tags in titles

## 📊 Performance Validation

### Test 12: Workflow Duration Tracking
Record actual durations and compare to targets:

| Workflow | Target Duration | Actual Duration | Status |
|----------|----------------|-----------------|---------|
| Build & Test (docs - skipped) | 1-2 min | _____ | _____ |
| Build & Test (code branches) | 3-8 min | _____ | _____ |
| PR Validation | 1-2 min | _____ | _____ |
| Hotfix Validation | 2-3 min | _____ | _____ |
| Post-merge Automation | 3-5 min | _____ | _____ |

## ✅ Final Validation

### Overall System Health
- [ ] All new workflows execute successfully
- [ ] No workflow dependency errors
- [ ] Resource consumption reduced as expected
- [ ] Developer feedback improved
- [ ] Security scanning still comprehensive
- [ ] Release automation reliable

### Legacy Workflow Status
- [ ] `version-management.yml` properly deprecated
- [ ] No unintended executions of deprecated workflow
- [ ] Legacy functionality completely replaced

### Workflow Integration
- [ ] New workflows complement existing build-and-test.yml
- [ ] Branch detection works correctly
- [ ] No duplicate validations
- [ ] Clear separation of concerns

## 📝 Test Results Summary

**Total Tests**: 12  
**Passed**: _____  
**Failed**: _____  
**Issues Found**: _____

### Critical Issues
- [ ] None identified
- [ ] List any critical issues: _______________

### Performance Issues  
- [ ] None identified
- [ ] List any performance issues: _______________

### Integration Issues
- [ ] None identified
- [ ] List any integration issues: _______________

### Recommendations
- [ ] Ready for production deployment
- [ ] Requires additional fixes: _______________
- [ ] Schedule for future improvements: _______________

## 🎉 Sign-off

**Testing Completed By**: ___________  
**Date**: ___________  
**Status**: [ ] APPROVED FOR PRODUCTION [ ] REQUIRES FIXES  
**Notes**: _______________

---

**Next Actions**:
- [ ] Deploy to production if approved
- [ ] Create fix branches for any issues
- [ ] Document lessons learned
- [ ] Update monitoring and alerting
- [ ] Remove deprecated workflows after validation period

## 🔗 Related Documentation

- [Workflow Separation Implementation](../ci-cd/workflow-separation-implementation.md) - Technical implementation details
- [Branch-Specific CI Strategy](../../BRANCH_SPECIFIC_CI_STRATEGY.md) - Overall strategy
- [GitHub Actions Workflows](../../GITHUB_ACTIONS_WORKFLOWS.md) - Complete workflow documentation
