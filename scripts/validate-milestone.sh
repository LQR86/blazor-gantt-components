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

# Find the matching phase - using jq without base64 encoding
PHASE_COUNT=$(echo "$MILESTONE_DATA" | jq '.phases | length')

PHASE_FOUND=false
for ((i=0; i<$PHASE_COUNT; i++)); do
    PHASE_NAME=$(echo "$MILESTONE_DATA" | jq -r ".phases[$i].name")
    
    # If no specific phase requested, validate all phases
    # If specific phase requested, only validate that phase
    if [[ -z "$PHASE" ]] || [[ "$PHASE_NAME" == *"$PHASE"* ]] || [[ "$PHASE" == *"$PHASE_NAME"* ]]; then
        PHASE_FOUND=true
        echo ""
        echo "🔍 Validating Phase: $PHASE_NAME"
        
        # Get validations for this phase
        VALIDATION_COUNT=$(echo "$MILESTONE_DATA" | jq ".phases[$i].validations | length")
        
        for ((j=0; j<$VALIDATION_COUNT; j++)); do
            TYPE=$(echo "$MILESTONE_DATA" | jq -r ".phases[$i].validations[$j].type")
            DESCRIPTION=$(echo "$MILESTONE_DATA" | jq -r ".phases[$i].validations[$j].description")
            
            case $TYPE in
                "file_exists")
                    PATH=$(echo "$MILESTONE_DATA" | jq -r ".phases[$i].validations[$j].path")
                    if [[ -f "$PATH" ]]; then
                        echo "  ✅ $DESCRIPTION: $PATH"
                    else
                        echo "  ❌ $DESCRIPTION: $PATH (missing)"
                        exit 1
                    fi
                    ;;
                    
                "directory_exists")
                    PATH=$(echo "$MILESTONE_DATA" | jq -r ".phases[$i].validations[$j].path")
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
