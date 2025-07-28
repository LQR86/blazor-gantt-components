#!/bin/bash
# scripts/create-milestone-branch.sh
# Usage: ./scripts/create-milestone-branch.sh 1.2 "TimelineView Component"

set -e

MILESTONE=$1
DESCRIPTION=$2

if [[ -z "$MILESTONE" ]] || [[ -z "$DESCRIPTION" ]]; then
    echo "Usage: $0 <milestone> <description>"
    echo "Example: $0 1.2 'TimelineView Component'"
    exit 1
fi

# Calculate version from milestone
MAJOR=0
MINOR=${MILESTONE#*.}
PATCH=0
VERSION="$MAJOR.$MINOR.$PATCH-alpha"

echo "🚀 Creating milestone branch for v$VERSION"

# Create and switch to new branch
BRANCH_NAME="feature/v$VERSION-$(echo $DESCRIPTION | tr '[:upper:]' '[:lower:]' | tr ' ' '-')"
git checkout -b "$BRANCH_NAME"

# Update version.json
echo "📝 Updating version.json"
jq --arg version "$VERSION" \
   --arg milestone "$MILESTONE" \
   --arg phase "$DESCRIPTION" \
   --arg status "in-progress" \
   '.version = $version | .milestone = $milestone | .phase = $phase | .status = $status' \
   version.json > version.json.tmp && mv version.json.tmp version.json

# Update progress tracking file
echo "📋 Updating progress tracking"
PROGRESS_FILE="TEMP_FILES/feature-TaskGrid-TimelineView-GanttComposer-foundation-setup-progress.md"
if [[ -f "$PROGRESS_FILE" ]]; then
    # Update current status section
    sed -i "s/- ⏳ \*\*TimelineView\*\*: Does not exist yet/- 🔄 **TimelineView**: In progress (v$VERSION)/" "$PROGRESS_FILE"
fi

echo "✅ Branch $BRANCH_NAME created and configured"
echo "📌 Next steps:"
echo "   1. Implement milestone features"
echo "   2. Update tests"
echo "   3. Create PR when ready"
echo "   4. CI/CD will validate version and create tags automatically"
