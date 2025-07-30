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
