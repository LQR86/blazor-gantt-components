# GitHub Codespaces Demo Configuration

This directory contains the GitHub Codespaces configuration for the BlazorGantt Timeline Zoom Demo.

## 🎯 Stakeholder Demo Features

- **Six Strategic Zoom Levels**: WeekDay (60px) → YearQuarter (3px)
- **Timeline Header Adaptation**: Dynamic headers with I18N support
- **Zoom State Management**: Event handling and validation
- **Row Alignment Preservation**: Pixel-perfect alignment across zoom levels
- **Multilingual Support**: English and Chinese (Simplified)

## 🚀 Quick Start for Stakeholders

### 1. Launch Codespace
```bash
# In GitHub repository: Code → Codespaces → Create codespace
# Wait for automatic setup (2-3 minutes)
```

### 2. Start Demo
```bash
./start-demo.sh
```

### 3. Share Public URL
- Go to **PORTS** tab in VS Code
- Click globe icon next to port 5000 to make **public**
- Copy the generated URL (e.g., `https://scaling-waddle-xxx.github.dev`)
- Share with stakeholders

## 🔧 Configuration Files

- **`devcontainer.json`**: Main Codespace configuration
- **`setup-demo.sh`**: Automated environment setup
- **Demo Scripts**: `start-demo.sh`, `test-demo.sh`

## 📋 Demo Workflow

### For Development Team
```bash
# Validate demo environment
./test-demo.sh

# Launch demo
./start-demo.sh

# Share public URL from PORTS tab
```

### For Stakeholders
1. **Access Demo URL** (provided by development team)
2. **Test Zoom Features**:
   - Try different zoom level buttons
   - Test timeline header adaptation
   - Verify I18N support (EN/ZH toggle)
   - Check row alignment at all levels
3. **Provide Feedback** via GitHub Issues

## 🎨 Current Demo State

Based on implementation plan (`temp-short-term-plan.md`):

- ✅ **Phase 1 Complete**: Foundation (v0.7.0-v0.7.2)
- ✅ **Phase 2 Iterations 2.1-2.3**: Core zoom implementation  
- 🚧 **Phase 2 Iteration 2.4**: Basic Zoom UI Controls (v0.8.2)
- ⏳ **Phase 2 Iteration 2.5**: GanttComposer Integration Validation
- ⏳ **Phase 3**: Advanced Features (v0.8.3-v0.8.7)

## 🔗 External Access

Codespace public URLs allow external stakeholders to:
- Access live demo without GitHub account
- Test real Blazor Server functionality
- Provide immediate feedback
- View exact repository state

## 💡 Best Practices

1. **Demo Sessions**: Schedule 1-2 hour focused sessions
2. **Version Control**: Demo always matches main branch state  
3. **Feedback Collection**: Use GitHub Issues for structured feedback
4. **Updates**: Restart Codespace after merging new features
5. **Clean State**: Each Codespace starts with fresh environment

---

*This configuration supports professional stakeholder demonstrations of the BlazorGantt Timeline Zoom System with zero infrastructure setup required.*
