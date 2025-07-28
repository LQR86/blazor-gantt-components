#!/bin/bash

# Milestone Validation Script
# This script dynamically validates milestones based on configuration files
# instead of hardcoded logic in the GitHub Actions workflow

set -e

MILESTONE=$1
PHASE=$2

if [[ -z "$MILESTONE" ]]; then
    echo "❌ Usage: $0 <milestone> [phase]"
    exit 1
fi

CONFIG_FILE=".github/milestone-validations/milestone-${MILESTONE}.json"

if [[ ! -f "$CONFIG_FILE" ]]; then
    echo "❌ No validation configuration found for milestone $MILESTONE"
    echo "Expected file: $CONFIG_FILE"
    exit 1
fi

echo "🎯 Validating Milestone: $MILESTONE"
if [[ -n "$PHASE" ]]; then
    echo "📝 Phase: $PHASE"
fi

# Parse the configuration file
MILESTONE_DATA=$(cat "$CONFIG_FILE")
DESCRIPTION=$(echo "$MILESTONE_DATA" | jq -r '.description')

echo "📋 Description: $DESCRIPTION"

# Find the matching phase
PHASES=$(echo "$MILESTONE_DATA" | jq -r '.phases[] | @base64')

PHASE_FOUND=false
for phase_data in $PHASES; do
    PHASE_JSON=$(echo "$phase_data" | base64 --decode)
    PHASE_NAME=$(echo "$PHASE_JSON" | jq -r '.name')
    
    # If no specific phase requested, validate all phases
    # If specific phase requested, only validate that phase
    if [[ -z "$PHASE" ]] || [[ "$PHASE_NAME" == *"$PHASE"* ]] || [[ "$PHASE" == *"$PHASE_NAME"* ]]; then
        PHASE_FOUND=true
        echo ""
        echo "🔍 Validating Phase: $PHASE_NAME"
        
        # Get validations for this phase
        VALIDATIONS=$(echo "$PHASE_JSON" | jq -r '.validations[] | @base64')
        
        for validation_data in $VALIDATIONS; do
            VALIDATION_JSON=$(echo "$validation_data" | base64 --decode)
            TYPE=$(echo "$VALIDATION_JSON" | jq -r '.type')
            DESCRIPTION=$(echo "$VALIDATION_JSON" | jq -r '.description')
            
            case $TYPE in
                "file_exists")
                    PATH=$(echo "$VALIDATION_JSON" | jq -r '.path')
                    if [[ -f "$PATH" ]]; then
                        echo "  ✅ $DESCRIPTION: $PATH"
                    else
                        echo "  ❌ $DESCRIPTION: $PATH (missing)"
                        exit 1
                    fi
                    ;;
                    
                "directory_exists")
                    PATH=$(echo "$VALIDATION_JSON" | jq -r '.path')
                    if [[ -d "$PATH" ]]; then
                        echo "  ✅ $DESCRIPTION: $PATH/"
                    else
                        echo "  ❌ $DESCRIPTION: $PATH/ (missing)"
                        exit 1
                    fi
                    ;;
                    
                "integration_test")
                    echo "  🧪 $DESCRIPTION (integration test - implement as needed)"
                    ;;
                    
                *)
                    echo "  ⚠️  Unknown validation type: $TYPE"
                    ;;
            esac
        done
    fi
done

if [[ "$PHASE_FOUND" == "false" ]] && [[ -n "$PHASE" ]]; then
    echo "❌ Phase '$PHASE' not found in milestone $MILESTONE configuration"
    exit 1
fi

echo ""
echo "✅ Milestone $MILESTONE validation completed successfully!"
