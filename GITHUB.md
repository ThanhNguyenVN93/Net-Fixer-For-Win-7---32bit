# 🚀 GitHub Upload Instructions

## Quick Upload

```bash
# 1. Initialize Git
git init

# 2. Add all files (respects .gitignore)
git add .

# 3. Commit
git commit -m "Initial commit: Network Driver Installer v5.0 - Single-File Deployment"

# 4. Add remote (replace YOUR_USERNAME)
git remote add origin https://github.com/YOUR_USERNAME/frmnetx86.git

# 5. Push
git branch -M main
git push -u origin main
```

## Create Release

After pushing:
1. Go to GitHub → Releases → Create new release
2. Tag: `v5.0`
3. Title: `Network Driver Installer v5.0 - Single-File Deployment`
4. Upload from `bin\Release\`:
   - `Net Fixer For Win 7 - 32bit.exe` (Single file - all drivers embedded)

## Project Structure

```
Essential Files (Upload to GitHub):
├── Source Code (7 files)
│   ├── Form1.cs, Form1.Designer.cs, Form1.resx
│   ├── Form2.cs, Form2.Designer.cs, Form2.resx
│   └── Program.cs
├── Resources (3 files)
│   ├── lanx86.zip (referenced by Resources.resx)
│   ├── wifix86.zip (referenced by Resources.resx)
│   └── net_fixer_x86.ico
├── Configuration (5 files)
│   ├── frmnetx86.csproj, frmnetx86.slnx
│   ├── App.config, packages.config
│   └── app.manifest
└── Documentation (3 files)
    ├── README.md
    ├── LICENSE
    └── .gitignore

Excluded (via .gitignore):
├── bin/ (build output)
├── obj/ (intermediate)
└── packages/ (NuGet - restore automatically)
```

## Build Release

```bash
# Clean build
msbuild /t:Clean

# Build Release
msbuild /p:Configuration=Release /p:Platform=x86

# Output: bin\Release\Net Fixer For Win 7 - 32bit.exe (~3.5 MB)
```

## Key Features

✅ Single-file EXE deployment
✅ Drivers embedded as resources
✅ No external dependencies
✅ Portable - runs from any location
✅ 32-bit Windows only
✅ .NET Framework 4.5.2+
