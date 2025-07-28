# Milestone Validation Configuration

This directory contains JSON configuration files that define what should be validated for each milestone, making the CI/CD workflow scalable and maintainable.

## Structure

- `milestone-1.1.json` - WBS Foundation validation rules
- `milestone-1.2.json` - Auto-Versioning System validation rules  
- `milestone-1.3.json` - GanttComposer validation rules
- `template.json` - Template for new milestone configurations

## Format

Each milestone configuration file defines:
- `milestone`: The milestone identifier
- `phases`: Array of possible phases for this milestone
- `validations`: Array of validation rules (files, directories, etc.)

## Benefits

- **Scalable**: Add new milestones without touching workflow code
- **Maintainable**: Clear separation between validation logic and rules
- **Flexible**: Different phases can have different validation requirements
- **Traceable**: Validation rules are version-controlled alongside code
