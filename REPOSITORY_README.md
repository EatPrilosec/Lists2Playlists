# Lists2Playlists Plugin

**Status**: This is a plugin repository optimized for Emby's plugin system.

## Quick Links

- 🚀 **[GitHub Releases](https://github.com/YOUR_USERNAME/Lists2Playlists/releases)** - Download latest version
- 📖 **[Setup Guide](./GITHUB_RELEASES_SETUP.md)** - How to use as Emby repository
- ✅ **[Release Checklist](./GITHUB_RELEASES_CHECKLIST.md)** - Step-by-step release instructions
- 🔧 **[Main Documentation](./README.md)** - Plugin usage guide
- 📝 **[Implementation Details](./COMPLETE_IMPLEMENTATION_DETAILS.md)** - Technical reference

## Repository URL

To use this as an Emby plugin catalog, add this URL to Emby's plugin settings:

```
https://raw.githubusercontent.com/YOUR_USERNAME/Lists2Playlists/main/repository.json
```

> **Note**: Replace `YOUR_USERNAME` with the actual GitHub username

## Installation

### In Emby
1. Go to **Plugins** → **Catalog**
2. Click **Add Catalog**
3. Paste the repository URL above
4. Search for "Lists2Playlists"
5. Click **Install**
6. Restart Emby Server

### Manual Installation
1. Download `.zip` from [Releases](https://github.com/YOUR_USERNAME/Lists2Playlists/releases)
2. Extract to `{EmbyRoot}/plugins/Lists2Playlists/`
3. Restart Emby Server

## Version Information

Current Latest Version: **1.0.0**

Supported Emby Versions: **4.8.0+**

### Release History

All versions are maintained in [`repository.json`](./repository.json) and automatically updated on each release.

## Making Releases

To publish a new version:

```bash
# Tag and push
git tag v1.1.0
git push origin v1.1.0
```

GitHub Actions will:
- Build the project
- Package as `.zip`
- Create release
- Auto-update `repository.json`

See [Release Checklist](./GITHUB_RELEASES_CHECKLIST.md) for details.

## Repository Contents

```
.
├── Lists2Playlists.csproj          Project file
├── plugin.json                      Plugin manifest
├── repository.json                  Emby plugin catalog
├── Configuration/
│   ├── PluginConfiguration.cs
│   └── configPage.html
├── Models/
│   └── ListModels.cs
├── Providers/
│   ├── IListProvider.cs
│   ├── LibraryMatcher.cs
│   └── Trakt/
│       └── TraktListProvider.cs
├── Services/
│   ├── ConfigurationService.cs
│   └── PlaylistService.cs
├── Sync/
│   └── SyncOrchestrator.cs
├── ScheduledTasks/
│   └── SyncListsTask.cs
├── .github/
│   ├── workflows/
│   │   └── release.yml              GitHub Actions workflow
│   └── scripts/
│       └── update-repo.js           Repository update script
└── [Documentation files]
```

## Configuration

### Before First Release

Edit `repository.json` and replace `YOUR_USERNAME` with your GitHub username in these URLs:
- `sourceUrl`
- `downloadUrl`
- `imageUrl`

### Plugin Manifest

Edit `plugin.json` to customize:
- Name, description, version
- Target Emby ABI version
- Category and owner info

## Support

- **Installation Help**: See [Setup Guide](./GITHUB_RELEASES_SETUP.md)
- **Plugin Usage**: See [README.md](./README.md)
- **Developer Info**: See [Implementation Details](./COMPLETE_IMPLEMENTATION_DETAILS.md)
- **Issues**: Open a GitHub issue in this repository

---

**This repository is configured as an Emby Plugin Repository and uses GitHub Actions for automated releases.**
