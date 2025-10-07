# Optiviera Desktop Deployment Guide

## Overview

This guide covers the complete deployment process for Optiviera ERP as a desktop application using Electron and ASP.NET Core.

## Prerequisites

- Node.js 16+ and npm
- .NET 6.0 SDK
- Electron Builder
- Git

## Project Structure

```
Optiviera/
├── Optiviera/                 # ASP.NET Core application
├── electron/                  # Electron wrapper
├── LicenseGenerator/          # License key generator tool
├── build-release.js          # Build automation script
└── DEPLOYMENT.md             # This file
```

## Build Process

### 1. Install Dependencies

```bash
# Install Electron dependencies
cd electron
npm install

# Install .NET dependencies
cd ../Optiviera
dotnet restore
```

### 2. Build ASP.NET Core Application

```bash
# Build for all platforms
cd ..
node build-release.js all

# Or build for specific platform
node build-release.js win    # Windows
node build-release.js mac    # macOS
node build-release.js linux  # Linux
```

### 3. Build Electron Application

```bash
cd electron

# Build for all platforms
npm run build:all

# Or build for specific platform
npm run build:win     # Windows
npm run build:mac     # macOS
npm run build:linux   # Linux
```

## Deployment Components

### 1. License System

- **Models**: `License.cs`, `LicenseHistory.cs`
- **Service**: `LicenseService.cs` with validation logic
- **Middleware**: `LicenseMiddleware.cs` for request interception
- **UI**: License management pages and warnings

### 2. Electron Desktop App

- **Main Process**: `main.js` - App lifecycle and ASP.NET Core process management
- **Preload**: `preload.js` - Secure IPC communication
- **Build Config**: `package.json` with electron-builder configuration

### 3. Database Setup

- **Initial Database**: Pre-seeded SQLite with admin user
- **Trial License**: 365-day trial license created automatically
- **Admin Credentials**: `admin@optiviera.local` / `Admin123!`

### 4. Update System

- **Update Service**: Checks for updates from akkistech.com
- **Version API**: `/api/optiviera/version.json`
- **Download URLs**: Platform-specific download links

## Installation Packages

### Windows (NSIS Installer)
- **Output**: `dist/Optiviera ERP Setup 1.0.0.exe`
- **Features**: Desktop shortcut, Start menu entry, auto-start option
- **Requirements**: Windows 10+

### macOS (DMG)
- **Output**: `dist/Optiviera ERP-1.0.0.dmg`
- **Features**: Drag-to-install, code signing support
- **Requirements**: macOS 10.15+

### Linux (AppImage/Deb)
- **Output**: `dist/Optiviera ERP-1.0.0.AppImage`, `dist/optiviera-erp_1.0.0_amd64.deb`
- **Features**: AppImage for portability, .deb for system integration
- **Requirements**: Ubuntu 18.04+

## License Management

### License Key Format
```
OPTV-XXXX-XXXX-XXXX-XXXX
```

### License Types
- **Trial**: 365-day trial period
- **Full**: 1-year renewable license

### License Generator Tool
```bash
cd LicenseGenerator
dotnet run -- --email customer@example.com --type Full --years 1
```

## Web Hosting Setup

### Download Page Structure
```
akkistech.com/
├── optiviera/
│   ├── download/              # Download page
│   ├── purchase/              # Purchase page
│   └── support/               # Support page
└── api/
    └── optiviera/
        ├── version.json       # Version information
        └── validate-license   # License validation endpoint
```

### API Endpoints

#### Version Check
```json
GET /api/optiviera/version.json
{
  "version": "1.0.1",
  "releaseDate": "2025-01-07",
  "releaseNotes": "Bug fixes and improvements",
  "downloadUrl": "https://akkistech.com/downloads/optiviera/latest/",
  "isCritical": false,
  "fileSize": 150000000,
  "checksum": "sha256:abc123..."
}
```

#### License Validation
```json
POST /api/optiviera/validate-license
{
  "licenseKey": "OPTV-XXXX-XXXX-XXXX-XXXX",
  "machineId": "abc123...",
  "customerEmail": "customer@example.com"
}
```

## Security Considerations

### License Protection
- Machine ID binding prevents license sharing
- Cryptographic license key validation
- Grace period for expired licenses (15 days)
- Online/offline validation support

### Application Security
- Context isolation in Electron
- Secure IPC communication
- External link protection
- SQL injection prevention

## Testing Checklist

### Installation Testing
- [ ] Fresh installation on clean system
- [ ] Admin user login with default credentials
- [ ] Trial license activation
- [ ] License key validation
- [ ] Update notification system

### Functionality Testing
- [ ] All application features work
- [ ] License warnings display correctly
- [ ] Grace period behavior
- [ ] System tray functionality
- [ ] Auto-start option

### Performance Testing
- [ ] Cold start time < 5 seconds
- [ ] Memory usage < 200MB idle
- [ ] Database operations < 100ms
- [ ] UI responsiveness

## Troubleshooting

### Common Issues

#### Application Won't Start
- Check if ASP.NET Core process is running
- Verify database file exists
- Check license validity

#### License Issues
- Verify license key format
- Check machine ID binding
- Validate license expiry date

#### Update Problems
- Check internet connectivity
- Verify API endpoint accessibility
- Check version comparison logic

### Log Files
- **Electron**: `%APPDATA%/Optiviera/logs/electron.log`
- **ASP.NET Core**: `%APPDATA%/Optiviera/logs/aspnet.log`
- **License**: `%APPDATA%/Optiviera/logs/license.log`

## Support

### Documentation
- User manual (8 languages)
- Installation guide
- Troubleshooting FAQ
- API documentation

### Contact
- **Email**: support@akkistech.com
- **Website**: https://akkistech.com/support
- **Documentation**: https://akkistech.com/docs/optiviera

## Release Process

### 1. Version Bump
```bash
# Update version in package.json
# Update version in Optiviera.csproj
# Update version in Program.cs
```

### 2. Build Release
```bash
# Build all platforms
node build-release.js all

# Test installation packages
# Verify functionality
# Generate checksums
```

### 3. Deploy to Web
```bash
# Upload to CDN
# Update download links
# Update version API
# Send notifications
```

### 4. Documentation Update
```bash
# Update changelog
# Update user documentation
# Update API documentation
```

## Monitoring

### Analytics
- Download statistics
- License activation rates
- Error reporting
- Performance metrics

### Alerts
- License validation failures
- Update check failures
- Critical errors
- Security incidents

## Maintenance

### Regular Tasks
- Monitor license usage
- Update security patches
- Backup customer data
- Performance optimization

### Quarterly Reviews
- License compliance audit
- Security assessment
- Performance analysis
- Feature roadmap planning
