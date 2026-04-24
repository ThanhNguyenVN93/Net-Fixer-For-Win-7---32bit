# Network Driver Installer (x86) 🌐

[![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.5.2-blue.svg)](https://dotnet.microsoft.com/download/dotnet-framework/net452)
[![Platform](https://img.shields.io/badge/platform-Windows%20x86-lightgrey.svg)](https://www.microsoft.com/windows)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)

A professional single-file network driver installer for Windows 7 32-bit systems with automated .NET Framework installation, embedded drivers, and intelligent hardware detection.

## ✨ Key Features

### 🎯 Single-File Deployment
- **One EXE file** - No external dependencies
- **70 MB total** - Drivers and .NET installer embedded
- **Portable** - Copy and run from anywhere
- **Offline capable** - Works without internet

### 🚀 Auto .NET Installation
- Detects .NET Framework 4.5.2+ automatically
- Embeds .NET installer (66.8 MB)
- Auto-installs if missing
- Auto-restarts and resumes
- Clean registry cleanup after completion

### 💻 Smart Driver Installation
- **LAN drivers** - Realtek, Intel, Atheros, Broadcom
- **WiFi drivers** - Embedded in single EXE
- **Automatic extraction** - To temp folder during install
- **Auto cleanup** - No traces left after installation
- **Hardware detection** - Identifies network adapters
- **Vendor detection** - Recognizes manufacturer

## 📦 What's Inside

```
Net Fixer For Win 7 - 32bit.exe (70.21 MB)
├── Application Code (~3 MB)
├── LAN Drivers - lanx86.zip (~0.7 MB)
├── WiFi Drivers - wifix86.zip (~1.5 MB)
└── .NET Framework 4.5.2 Installer (~66.8 MB)
```

## 💻 System Requirements

- **OS:** Windows 7 SP1 (32-bit only)
- **.NET:** 4.5.2+ (auto-installed if missing)
- **Privileges:** Administrator (auto-enforced)
- **Disk Space:** 500 MB free on C:\

## 🚀 Quick Start

1. Download the EXE from [Releases](../../releases)
2. Run the EXE (UAC prompt will appear)
3. First run: Auto-installs .NET if missing → Restart → Auto-resume
4. Click **Install LAN** → Wait → Click **Install Wi-Fi**
5. Done!

## 🔧 Building from Source

```bash
git clone https://github.com/yourusername/frmnetx86.git
cd frmnetx86
msbuild /t:restore
msbuild /p:Configuration=Release /p:Platform=x86
```

## 📄 License

MIT License - Copyright (c) 2026 Network Driver Installer Contributors

See [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- Microsoft .NET Framework team
- Windows Driver Kit (WDK)
- Community contributors

---

**Made with ❤️ for Windows 7 (x86)**
