# Changelog

All notable changes to this project will be documented in this file.

## [5.0.0] - 2026-04-23

### Added
- **Single-file deployment** - All components embedded in one EXE (70 MB)
- **Auto .NET installation** - Detects and installs .NET Framework 4.5.2 if missing
- **Embedded .NET installer** - net452.exe (66.8 MB) embedded as resource
- **Auto-restart and resume** - After .NET installation, app auto-launches
- **Registry-based state management** - Tracks restart and auto-cleanup
- **Startup management** - Auto-launch after restart, then self-remove
- **Embedded driver packages** - lanx86.zip and wifix86.zip in resources
- **Temp file extraction** - Drivers extract to Desktop during install
- **Auto cleanup** - All temp files removed after installation
- **Hardware detection** - WMI-based network adapter detection
- **Vendor identification** - Detects Realtek, Intel, Atheros, Broadcom
- **Color-coded logging** - 5 color states (Info, Process, Success, Warning, Error)
- **4-phase progress tracking** - 10% → 20% → 60% → 90% → 100%
- **Device Manager quick access** - Button to open devmgmt.msc
- **Admin enforcement** - UAC prompt via app.manifest
- **Disk space validation** - Checks 500MB free before install
- **Exit code handling** - PnPUtil success/restart detection
- **Multi-method hardware rescan** - devcon + pnputil + registry
- **Donate form** - QR codes for MoMo and TCB
- **Professional UI** - Terminal-style log with modern design

### Changed
- Platform target: x86 (32-bit only) for both Debug and Release
- .NET Framework requirement: 4.5.2+ (up from 3.5)
- Deployment model: Single EXE (from 3-file package)
- Driver storage: Embedded resources (from external files)
- Installation flow: Fully automated (from manual steps)

### Fixed
- Designer loading issues in Visual Studio
- Missing .NET Framework error messages
- Registry cleanup after completion
- Temp file cleanup failures
- x64 OS detection and rejection

### Security
- Admin privileges enforced via manifest
- Registry keys scoped to HKEY_CURRENT_USER
- Temp files deleted after use
- No permanent system modifications except drivers

## [4.0.0] - 2026-04-20

### Added
- Multi-method hardware rescan
- Improved error handling
- Color-coded status logging

### Changed
- Updated UI design
- Enhanced progress reporting

## [3.0.0] - 2026-04-15

### Added
- WiFi adapter detection
- Vendor identification
- Device Manager integration

## [2.0.0] - 2026-04-10

### Added
- Automated driver installation
- PnPUtil integration
- Progress tracking

## [1.0.0] - 2026-04-01

### Added
- Initial release
- Basic LAN driver installation
- Manual extraction

---

**Note:** Version numbers follow [Semantic Versioning](https://semver.org/).
