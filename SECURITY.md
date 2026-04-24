# Security Policy

## Supported Versions

| Version | Supported          |
| ------- | ------------------ |
| 5.0.x   | :white_check_mark: |
| < 5.0   | :x:                |

## Reporting a Vulnerability

If you discover a security vulnerability in this project, please follow these steps:

1. **Do NOT** open a public issue
2. Email: security@anhdv.com (or create private security advisory on GitHub)
3. Include:
   - Description of vulnerability
   - Steps to reproduce
   - Potential impact
   - Suggested fix (if any)

## What to Expect

- **Response time:** Within 48 hours
- **Fix timeline:** Critical issues within 7 days, others within 30 days
- **Credit:** Security researchers will be credited (if desired)
- **Disclosure:** Coordinated disclosure after patch release

## Security Best Practices

When using this tool:

1. **Verify source:** Download only from official GitHub releases
2. **Check hash:** Verify file integrity (SHA256 provided in releases)
3. **Test first:** Use in VM or test environment before production
4. **Backup:** Always backup system before driver installation
5. **Antivirus:** Some AV may flag due to embedded EXE (false positive)

## Known Security Considerations

### Admin Privileges
- App requires Administrator rights (by design)
- UAC prompt always appears
- Use only on trusted systems

### Embedded .NET Installer
- net452.exe is official Microsoft installer
- SHA256 verified during build
- Extracted to temp folder, then deleted

### Registry Access
- Uses HKEY_CURRENT_USER only (user scope)
- All entries cleaned up after completion
- No permanent registry modifications

### Temp File Usage
- Drivers extracted to %TEMP% and Desktop
- Auto-deleted after installation
- Failed cleanup leaves non-malicious files

## Code Signing

Currently the EXE is **not code-signed**. This may trigger SmartScreen warnings.

For production use, we recommend:
- Code signing certificate from trusted CA
- Timestamp the signature
- Include publisher information

## Third-Party Components

- **.NET Framework 4.5.2** - From Microsoft (official installer)
- **PnPUtil** - Windows built-in (trusted)
- **Driver packages** - User-provided (verify before use)

## Contact

For security issues: security@anhdv.com  
For general issues: [GitHub Issues](../../issues)
