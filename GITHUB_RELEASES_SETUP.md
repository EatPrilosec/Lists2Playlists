# GitHub Release & Emby Plugin Setup Guide

## Quick Start: Creating a Release

To create a new release of Lists2Playlists:

```bash
# Tag the commit with a semantic version
git tag v1.0.0
git push origin v1.0.0
```

The GitHub Actions workflow will automatically:
1. ✅ Build the project
2. ✅ Package it as a `.zip` file
3. ✅ Create a GitHub release
4. ✅ Update `repository.json` with version info
5. ✅ Upload the package as a release asset

## Using This as an Emby Plugin Repository

### Option 1: Using the GitHub Repository Directly

Add this repository to Emby's plugin settings:
```
https://raw.githubusercontent.com/YOUR_USERNAME/Lists2Playlists/main/repository.json
```

Then Emby can:
- Discover all available versions
- Auto-update to new versions
- Install directly from your repo

### Option 2: Self-Hosted Repository

If you want to host this separately:

1. Copy `repository.json` to your web server
2. Add the URL to Emby plugin settings:
   ```
   https://your-domain.com/Lists2Playlists/repository.json
   ```

## Repository Configuration

### Files Involved

- **`plugin.json`**: Plugin manifest with metadata
- **`repository.json`**: Plugin repository catalog (auto-updated per release)
- **`.github/workflows/release.yml`**: GitHub Actions workflow
- **`.github/scripts/update-repo.js`**: Script to update repository.json

### Before First Release

1. **Update `plugin.json`** if needed:
   ```json
   {
     "name": "Lists2Playlists",
     "id": "lists2playlists",
     "version": "1.0.0",
     "targetAbi": "4.8.0.0",
     // ... other fields
   }
   ```

2. **Update `repository.json`** with your GitHub username in:
   - `sourceUrl`
   - `downloadUrl`
   - `imageUrl` (optional icon URL)

3. **Customize version tracking**:
   - The workflow uses semantic versioning from tags
   - Use format: `v1.0.0`, `v1.1.0-beta`, etc.

## Installation Methods

### Method 1: Direct GitHub Installation
Users can:
1. Go to Plugins → Catalogs
2. Add catalog: `https://raw.githubusercontent.com/YOUR_USERNAME/Lists2Playlists/main/repository.json`
3. Search for "Lists2Playlists"
4. Click Install

### Method 2: Manual Installation
Users can:
1. Download the `.zip` from GitHub Releases
2. Extract to `{EmbyRoot}/plugins/Lists2Playlists/`
3. Restart Emby Server

### Method 3: Direct URL
Users can add this URL to their catalog list:
```
https://raw.githubusercontent.com/YOUR_USERNAME/Lists2Playlists/main/repository.json
```

## Release Workflow

### Making a Release

```bash
# 1. Make changes and commit
git add .
git commit -m "Add new feature"

# 2. Update version in plugin.json (optional - workflow can do this too)
# Edit plugin.json and update version field

# 3. Create a tag
git tag v1.0.1
git push origin v1.0.1
```

### What Happens Automatically

1. GitHub Actions detects the tag
2. Workflow builds with `dotnet build --configuration Release`
3. Creates `Lists2Playlists-1.0.1.zip` containing:
   - `Lists2Playlists.dll`
   - `manifest.json` (copy of plugin.json)
   - `configPage.html`
4. Creates GitHub Release with the zip file
5. Updates `repository.json` with new version info
6. Commits the updated `repository.json`

## Troubleshooting

### Release Failed
Check the Actions tab on GitHub for error logs.

Common issues:
- `.NET SDK` version mismatch: Update in workflow
- Missing files: Ensure all files exist in project root
- Build errors: Fix compiler errors and retry

### Plugin Won't Install
- Verify `repository.json` is valid JSON
- Check that downloadUrl points to valid zip file
- Verify `targetAbi` matches Emby version (4.8.0.0 for current)

### Can't Add Catalog
- Ensure raw GitHub URL is used:
  ```
  https://raw.githubusercontent.com/...
  ```
  NOT:
  ```
  https://github.com/... (this won't work)
  ```

## Semver Versioning Guide

- **1.0.0** - Major feature release
- **1.0.1** - Bug fix (patch)
- **1.1.0** - New feature (minor)
- **2.0.0** - Breaking change (major)

Use tags: `v1.0.0`, `v1.0.1`, `v1.1.0`, etc.

## File Structure in Release Package

```
Lists2Playlists-1.0.0.zip
├── Lists2Playlists.dll
├── manifest.json
└── configPage.html
```

When installed in Emby:
```
{EmbyRoot}/plugins/Lists2Playlists/
├── Lists2Playlists.dll
├── manifest.json
└── configPage.html
```

## Advanced: Setting Up Private Repository

If you want a private or custom-hosted repository:

1. Host `repository.json` on your server
2. Update `downloadUrl` to point to your CDN/server
3. Add server URL to Emby as plugin catalog

Example `repository.json` entry:
```json
{
  "id": "lists2playlists",
  "version": "1.0.0",
  "downloadUrl": "https://your-cdn.com/plugins/Lists2Playlists-1.0.0.zip",
  "sourceUrl": "https://github.com/YOUR_USERNAME/Lists2Playlists"
}
```

## CI/CD Enhancements

Future improvements could include:
- Automated version bumping
- Pre-release channels (alpha/beta)
- Changelog auto-generation from git commits
- Plugin validation testing
- Documentation deployment
