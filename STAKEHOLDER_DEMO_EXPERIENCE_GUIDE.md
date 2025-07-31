## 🎯 **Stakeholder Demo Experience Guide**

### **As the Development Team (You):**

#### 1. **Launch the Demo Codespace**
```bash
# Go to your GitHub repository
# https://github.com/LQR86/blazor-gantt-components
```

1. Click the green **"Code"** button
2. Click **"Codespaces"** tab  
3. Click **"Create codespace on main"**
4. Wait 2-3 minutes for automatic setup

#### 2. **Start the Demo**
Once the Codespace loads:
```bash
# The setup is automatic, just run:
./start-demo.sh
```

**If you get "dotnet: command not found":**
```bash
# Install .NET SDK manually:
wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
chmod +x ./dotnet-install.sh
./dotnet-install.sh --version latest
export PATH="$PATH:$HOME/.dotnet"
export DOTNET_ROOT="$HOME/.dotnet"

# Then run the demo:
./start-demo.sh
```

**If you get "ICU package" error:**
```bash
# For Alpine Linux (install ICU libraries for globalization support):
sudo apk update
sudo apk add icu-libs icu-dev

# Then run the demo:
./start-demo.sh
```

You'll see output like:
```
🎯 BlazorGantt Stakeholder Demo Launcher
========================================
📊 Current Implementation Status:
  ✅ Phase 1: Foundation (v0.7.0-v0.7.2) - Complete
  ✅ Phase 2.1: Zoom State Management (v0.8.0) - Complete  
  ✅ Phase 2.2: Six Zoom Levels (v0.8.1) - Complete
  ✅ Phase 2.3: Timeline Header Adaptation - Complete
  🚧 Phase 2.4: Basic Zoom UI Controls (v0.8.2) - Current

🎨 Available Demo Features:
  🔍 Six Strategic Zoom Levels:
    • WeekDay (60px) - Detailed daily view
    • MonthDay (25px) - Standard monthly view  
    • MonthWeek (15px) - Weekly overview
    • QuarterMonth (8px) - Quarterly planning
    • YearQuarter (5px) - Annual overview
    • YearMonth (3px) - Multi-year view

  🌐 I18N Support: English & Chinese (Simplified)
  📏 Row Alignment: Pixel-perfect across all zoom levels
  🎛️ Zoom Controls: State management & event handling
  📅 Timeline Headers: Dynamic adaptation per zoom level

🚀 Launching BlazorGantt Timeline Zoom Demo...
```

#### 3. **Make Ports Public for Stakeholders**
1. In VS Code, go to **PORTS** tab (bottom panel)
2. Find the running port (usually **5234** for HTTP or **7138** for HTTPS)
3. **Right-click** → **Port Visibility** → **Public**
4. **Copy the generated URL** (e.g., `https://scaling-waddle-xxx.github.dev`)

**Important Navigation:**
- Base URL shows "Not Found" - this is normal
- **Use HTTP URLs**: Add `/timeline-demo` to the HTTP URL
- **Correct format**: `https://your-codespace-url.github.dev/timeline-demo`
- Available demo pages:
  - `/timeline-demo` - Timeline zoom features (MAIN DEMO)
  - `/gantt-composer-demo` - Full Gantt experience
  - `/wbs-demo` - Work breakdown structure

**Troubleshooting:**
- If you see 307 redirects, the app is redirecting HTTP to HTTPS
- If URL redirects to `github.dev/pf-signin`, restart demo (it will auto-detect Codespace)
- **Auto-detection**: Demo script automatically uses Codespace profile for external access
- Use the public HTTPS URL from the PORTS tab
- Clear browser cache if pages don't load

**Common Issues:**
- **Authentication redirect**: Restart `./start-demo.sh` - it auto-detects Codespace environment
- **Port not public**: Right-click port in PORTS tab → Port Visibility → Public
- **Wrong URL**: Use `https://your-url/timeline-demo` not just base URL

### **As a Stakeholder (External User):**

#### 1. **Receive Demo Link**
You'll get a message like:
> **BlazorGantt Timeline Zoom Demo**  
> 🔗 **Demo URL**: https://scaling-waddle-xxx.github.dev  
> ⏰ **Available**: Next 4 hours  
> 💬 **Feedback**: Please share thoughts via GitHub Issues

#### 2. **Access the Demo** 
- **No GitHub account required** - just click the URL
- **Navigate to demo pages**: Add `/timeline-demo` to the URL
- **Full URL example**: `https://scaling-waddle-xxx.github.dev/timeline-demo`
- Works on any device (desktop, tablet, mobile)
- Full Blazor Server functionality

#### 3. **Test the Features**
**What stakeholders can test:**

🔍 **Zoom Level Testing:**
- Navigate to timeline demo page
- Try different zoom level buttons (if UI controls are implemented)
- Test zoom transitions and performance

📅 **Timeline Header Adaptation:**
- Check headers change appropriately per zoom level
- Test I18N language switching (EN ↔ ZH)
- Verify fixed-width font consistency

⚡ **Performance Testing:**
- Test smooth zoom transitions
- Check row alignment preservation
- Test with different browser sizes

📱 **Accessibility Testing:**
- Keyboard navigation
- Screen reader compatibility
- Touch interaction (mobile)

#### 4. **Provide Feedback**
Stakeholders can:
- Comment directly on functionality
- Report issues or suggestions
- Share via the auto-created GitHub issue

### **Current Demo Capabilities** 

Based on your **temp-short-term-plan.md**, stakeholders can test:

✅ **Available Now:**
- Six strategic zoom levels with proper day widths
- Timeline header adaptation (dynamic headers per zoom level)  
- I18N support (English & Chinese)
- Row alignment preservation
- Zoom state management and event handling

🚧 **In Development (Iteration 2.4):**
- Basic zoom UI controls (preset level buttons)
- Current zoom level display
- Integrated zoom controls

⏳ **Coming Soon:**
- Manual zoom controls with slider
- Task overflow detection
- Smart zoom shortcuts (Fit Viewport/Tasks)

### **Demo Session Management**

You can also use the GitHub Actions workflow:
1. Go to **Actions** tab in your repository
2. Find **"🚀 Setup Stakeholder Demo"** workflow
3. Click **"Run workflow"**
4. Fill in demo details (purpose, stakeholder group, duration)
5. This creates a tracking issue for organized feedback collection

### **⚠️ Important: Codespace Billing & Timing**

**How Billing Works:**
- You're charged for **Codespace runtime** (when VS Code is active)
- Public URLs don't add extra costs - they're tied to runtime
- **Auto-sleep**: Codespaces sleep after 30 minutes of inactivity
- **Manual control**: Stop Codespace immediately after demos

**Free Tier Limits:**
- 120 core hours/month (2-core machine)
- 60 core hours/month (4-core machine)

**Best Practices:**
- ✅ **Just-in-Time**: Start Codespace right before demo
- ✅ **Manual Stop**: Stop immediately after demo session  
- ✅ **Time Windows**: "Available for next 2 hours"
- ✅ **Multiple Sessions**: Start/stop for different stakeholder groups

**Example Demo Schedule:**
```
Monday 2PM: Start Codespace → Demo with PM team → Stop Codespace
Tuesday 10AM: Start Codespace → Demo with stakeholders → Stop Codespace  
Wednesday 3PM: Start Codespace → Demo with executives → Stop Codespace
```

This approach keeps you well within free tier limits while providing professional demo experiences.