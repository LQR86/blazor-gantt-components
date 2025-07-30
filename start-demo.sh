#!/bin/bash

echo "🎯 BlazorGantt Stakeholder Demo Launcher"
echo "========================================"

# Display current implementation status
echo "📊 Current Implementation Status:"
echo "  ✅ Phase 1: Foundation (v0.7.0-v0.7.2) - Complete"
echo "  ✅ Phase 2.1: Zoom State Management (v0.8.0) - Complete"  
echo "  ✅ Phase 2.2: Six Zoom Levels (v0.8.1) - Complete"
echo "  ✅ Phase 2.3: Timeline Header Adaptation - Complete"
echo "  🚧 Phase 2.4: Basic Zoom UI Controls (v0.8.2) - Current"
echo ""

# Check if we're in a Codespace
if [ -n "$CODESPACES" ]; then
    echo "🌐 Running in GitHub Codespace"
    echo "Repository: $GITHUB_REPOSITORY"
    echo "Codespace: $CODESPACE_NAME"
    echo ""
else
    echo "💻 Running in local development environment"
    echo ""
fi

# Set demo environment
export ASPNETCORE_ENVIRONMENT=Demo
export ASPNETCORE_URLS="http://0.0.0.0:5000;https://0.0.0.0:5001"
export ASPNETCORE_HTTPS_PORT=5001

# Display demo features
echo "🎨 Available Demo Features:"
echo "  🔍 Six Strategic Zoom Levels:"
echo "    • WeekDay (60px) - Detailed daily view"
echo "    • MonthDay (25px) - Standard monthly view"  
echo "    • MonthWeek (15px) - Weekly overview"
echo "    • QuarterMonth (8px) - Quarterly planning"
echo "    • YearQuarter (5px) - Annual overview"
echo "    • YearMonth (3px) - Multi-year view"
echo ""
echo "  🌐 I18N Support: English & Chinese (Simplified)"
echo "  📏 Row Alignment: Pixel-perfect across all zoom levels"
echo "  🎛️ Zoom Controls: State management & event handling"
echo "  📅 Timeline Headers: Dynamic adaptation per zoom level"
echo ""

# Display access information
echo "🔗 Demo Access Information:"
if [ -n "$CODESPACES" ]; then
    echo "  📱 For Stakeholder Access:"
    echo "    1. Go to VS Code PORTS tab"
    echo "    2. Click globe icon next to port 5000"
    echo "    3. Copy the public URL"
    echo "    4. Share with stakeholders"
    echo ""
    echo "  🔒 Security: Public URLs allow external access without GitHub login"
else
    echo "  🏠 Local Access:"
    echo "    • HTTP:  http://localhost:5000"
    echo "    • HTTPS: https://localhost:5001"
fi
echo ""

# Start the application
echo "🚀 Launching BlazorGantt Timeline Zoom Demo..."
echo "⏱️  Startup time: ~10-15 seconds"
echo "🛑 Press Ctrl+C to stop the demo"
echo ""

cd src/GanttComponents
dotnet run
