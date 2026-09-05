# Network Driver Installer (x86) 🌐

[![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.5.2-blue.svg)](https://dotnet.microsoft.com/download/dotnet-framework/net452)
[![Platform](https://img.shields.io/badge/platform-Windows%20x86-lightgrey.svg)](https://www.microsoft.com/windows)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)

A professional network driver installer for 32-bit Windows systems with automated LAN/WiFi driver installation, intelligent hardware detection, and comprehensive error handling.

## ✨ Features

- 🚀 **One-Click Installation** - Automated LAN and WiFi driver installation
- 💻 **Non-Blocking UI** - Responsive interface using BackgroundWorker
- 📊 **Real-Time Progress** - 4-phase progress tracking (10%-100%)
- 🔍 **Hardware Detection** - Automatic vendor identification (Realtek, Intel, Atheros, Broadcom)
- 💾 **Disk Space Check** - Validates 500MB free space before installation
- 🔄 **Force Update** - Driver replacement support with /force parameter
- 🔧 **Device Rescan** - Multiple hardware rescan methods
- 🎨 **Color-Coded Logging** - Terminal-style log with color indicators
- 🛡️ **Admin Enforcement** - Automatic administrator privileges via manifest

## 🖥️ System Requirements

- **OS**: Windows 7 SP1 or later (32-bit / x86 only)
- **.NET Framework**: 4.5.2 or higher
- **Disk Space**: 500MB free on C: drive
- **Privileges**: Administrator rights (auto-enforced)

## 📦 Installation

### Pre-built Release
1. Download the latest release from [Releases](../../releases)
2. **Single file** - Just download `Net Fixer For Win 7 - 32bit.exe`
3. Run the EXE (UAC prompt will appear for Administrator rights)
4. That's it! Driver packages are embedded inside the EXE

### Build from Source
```bash
# Clone repository
git clone https://github.com/yourusername/frmnetx86.git
cd frmnetx86

# Restore NuGet packages
nuget restore

# Build with Visual Studio 2026 or MSBuild
msbuild frmnetx86.csproj /p:Configuration=Release /p:Platform=x86
```

**Note**: Driver packages (`lanx86.zip`, `wifix86.zip`) are embedded as resources in the EXE.

## 🚀 Usage

1. **Launch Application** - Run as Administrator (enforced automatically)
2. **Install LAN Driver** - Click "Install LAN" button
3. **Wait for Completion** - Progress bar shows 10% → 100%
4. **Install WiFi Driver** - Click "Install Wi-Fi" button (unlocked after LAN)
5. **Verify Installation** - Click "Open Device Manager" to confirm

### Screenshots

```
┌─────────────────────────────────────────────┐
│ Network Driver Installer (x86)              │
├─────────────────────────────────────────────┤
│ System Information                          │
│   Windows 10 - 32-bit                       │
├─────────────────────────────────────────────┤
│ Driver Installation                         │
│  [Install LAN]  Status  [Install Wi-Fi]     │
│                                             │
│  Progress & Log                             │
│  ████████████████░░░░ 80%                   │
│  > Step 3/4: Rescanning hardware...         │
│  > ✓ Installation complete                  │
└─────────────────────────────────────────────┘
```

## 📋 Features Breakdown

### 🎯 Core Functionality
- **Automated Extraction** - Unzips driver packages to Desktop
- **PnPUtil Integration** - Uses Windows PnPUtil with /install /subdirs /force
- **Sequential Lock** - WiFi installation requires LAN completion
- **Cleanup** - Automatic removal of temporary files

### 🛡️ Safety Features
- **Disk Space Check** - Validates 500MB free before extraction
- **Exit Code Handling** - Recognizes success (0), restart required (3010)
- **Error Recovery** - Graceful fallback strategies

### 🎨 User Interface
- **Color-Coded Logs**
  - ⚪ White - System information
  - 🟡 Yellow - Process/Activity
  - 🟢 Lime - Success
  - 🟠 Orange - Warning (restart needed)
  - 🔴 Red - Critical error
- **Progress Phases**
  - 10-20%: Extraction
  - 30-60%: Installation
  - 70-90%: Rescan & Cleanup
  - 100%: Complete

## 🔧 Configuration

### Driver Packages
Place driver ZIP files in the application directory:
- `lanx86.zip` - LAN driver package
- `wifix86.zip` - WiFi driver package

### Supported Hardware Vendors
- **Realtek** (VEN_10EC)
- **Intel** (VEN_8086)
- **Atheros** (VEN_168C)
- **Broadcom** (VEN_14E4)

## 📖 Documentation

- [README.md](README.md) - Quick start guide (this file)
- [FINAL_SPEC.md](FINAL_SPEC.md) - Complete technical specification
- [FINAL_ENHANCEMENTS.md](FINAL_ENHANCEMENTS.md) - V5.0 safety features
- [PROJECT_COMPLETION.md](PROJECT_COMPLETION.md) - Project completion report

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- Thanks to all contributors
- Built with .NET Framework 4.5.2
- Powered by PnPUtil (Windows Driver Management)

## 📧 Contact

- **Developer**: [Your Name]
- **Email**: your.email@example.com
- **Website**: [https://anhdv.com](https://anhdv.com)

## ⚠️ Disclaimer

This tool is provided as-is without warranty. Always backup your system before installing drivers.

---

**Made with ❤️ for Windows x86 systems**
