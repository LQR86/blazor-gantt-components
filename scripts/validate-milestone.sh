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

# Check if jq is available and set JQ_CMD
JQ_CMD=""
if [[ -f "/usr/bin/jq" ]]; then
    echo "Found jq at /usr/bin/jq"
    JQ_CMD="/usr/bin/jq"
elif [[ -f "/usr/local/bin/jq" ]]; then
    echo "Found jq at /usr/local/bin/jq"
    JQ_CMD="/usr/local/bin/jq"
elif command -v jq &> /dev/null; then
    JQ_CMD=$(command -v jq)
    echo "Found jq in PATH at: $JQ_CMD"
else
    echo "❌ jq is required but not found"
    echo "Current PATH: $PATH"
    exit 1
fi

echo "Using jq: $JQ_CMD"

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
DESCRIPTION=$($JQ_CMD -r '.description' <<< "$MILESTONE_DATA")

echo "📋 Description: $DESCRIPTION"

# Find the matching phase - using jq without base64 encoding
PHASE_COUNT=$($JQ_CMD '.phases | length' <<< "$MILESTONE_DATA")

PHASE_FOUND=false
for ((i=0; i<$PHASE_COUNT; i++)); do
    PHASE_NAME=$($JQ_CMD -r ".phases[$i].name" <<< "$MILESTONE_DATA")
    
    # If no specific phase requested, validate all phases
    # If specific phase requested, only validate that phase
    if [[ -z "$PHASE" ]] || [[ "$PHASE_NAME" == *"$PHASE"* ]] || [[ "$PHASE" == *"$PHASE_NAME"* ]]; then
        PHASE_FOUND=true
        echo ""
        echo "🔍 Validating Phase: $PHASE_NAME"
        
        # Get validations for this phase
        VALIDATION_COUNT=$($JQ_CMD ".phases[$i].validations | length" <<< "$MILESTONE_DATA")
        
        for ((j=0; j<$VALIDATION_COUNT; j++)); do
            TYPE=$($JQ_CMD -r ".phases[$i].validations[$j].type" <<< "$MILESTONE_DATA")
            DESCRIPTION=$($JQ_CMD -r ".phases[$i].validations[$j].description" <<< "$MILESTONE_DATA")
            
            case $TYPE in
                "file_exists")
                    PATH=$($JQ_CMD -r ".phases[$i].validations[$j].path" <<< "$MILESTONE_DATA")
                    if [[ -f "$PATH" ]]; then
                        echo "  ✅ $DESCRIPTION: $PATH"
                    else
                        echo "  ❌ $DESCRIPTION: $PATH (missing)"
                        exit 1
                    fi
                    ;;
                    
                "directory_exists")
                    PATH=$($JQ_CMD -r ".phases[$i].validations[$j].path" <<< "$MILESTONE_DATA")
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
