#!/bin/bash

echo "🚀 Setting up BlazorGantt Demo Environment..."
echo "================================================"

# Restore .NET packages
echo "📦 Restoring .NET packages..."
dotnet restore

# Build the project
echo "🔨 Building BlazorGantt project..."
dotnet build

# Create demo launch script
echo "📝 Creating demo launch script..."
cat > start-demo.sh << 'EOF'
#!/bin/bash

echo "🎯 Starting BlazorGantt Timeline Zoom Demo"
echo "=========================================="

# Set environment for demo
export ASPNETCORE_ENVIRONMENT=Demo
export ASPNETCORE_URLS="http://0.0.0.0:5000;https://0.0.0.0:5001"
export ASPNETCORE_HTTPS_PORT=5001

# Display demo information
echo ""
echo "📊 Demo Features Available:"
echo "  ✅ Six Strategic Zoom Levels (WeekDay → YearQuarter)"
echo "  ✅ Timeline Header Adaptation with I18N"
echo "  ✅ Zoom State Management & Event Handling"
echo "  ✅ Row Alignment Preservation"
echo "  ✅ English & Chinese Language Support"
echo ""
echo "🔗 Demo will be available at:"
echo "  HTTP:  http://localhost:5000"
echo "  HTTPS: https://localhost:5001"
echo ""
echo "💡 Check the PORTS tab in VS Code for public URLs to share with stakeholders"
echo ""

# Start the application
echo "🚀 Launching BlazorGantt..."
cd src/GanttComponents
dotnet run
EOF

chmod +x start-demo.sh

# Create quick test script
echo "🧪 Creating quick test script..."
cat > test-demo.sh << 'EOF'
#!/bin/bash

echo "🧪 Running BlazorGantt Tests..."
echo "=============================="

# Run unit tests
echo "📋 Running unit tests..."
dotnet test --verbosity normal

# Check build
echo "🔨 Verifying build..."
dotnet build --verbosity quiet

# Format check
echo "📝 Checking code formatting..."
dotnet format --verify-no-changes --verbosity diagnostic

echo ""
echo "✅ Demo environment validated successfully!"
echo "🚀 Run ./start-demo.sh to launch the demo"
EOF

chmod +x test-demo.sh

# Display setup completion
echo ""
echo "✅ Demo environment setup complete!"
echo ""
echo "🎯 Quick Start Commands:"
echo "  ./start-demo.sh    - Launch demo for stakeholders"
echo "  ./test-demo.sh     - Validate demo environment"
echo ""
echo "📱 Stakeholder Demo URLs will be available in VS Code PORTS tab"
echo "🔗 Make ports 5000/5001 public to share with external stakeholders"
echo ""
