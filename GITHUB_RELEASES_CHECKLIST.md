# Lists2Playlists - GitHub Release Setup Checklist

## Pre-Release Setup (One-time)

### Step 1: Update Repository Information
Edit these files to match your GitHub repo:

**`repository.json`** - Replace `YOUR_USERNAME` with your GitHub username:
```json
{
  "sourceUrl": "https://github.com/YOUR_USERNAME/Lists2Playlists",
  "downloadUrl": "https://github.com/YOUR_USERNAME/Lists2Playlists/releases/download/v1.0.0/Lists2Playlists-1.0.0.zip",
  "imageUrl": "https://raw.githubusercontent.com/YOUR_USERNAME/Lists2Playlists/main/icon.png"
}
```

**`.github/workflows/release.yml`** - Already configured, no changes needed

### Step 2: Set Repository Visibility
1. Go to GitHub repo Settings
2. Under "General", ensure repository is **Public** (so Emby can read `repository.json`)
3. Or configure GitHub Pages if you prefer

### Step 3: Create Initial Release

```bash
# Ensure all changes are committed
git status

# Create a version tag (v1.0.0, v1.0.1, etc.)
git tag v1.0.0

# Push the tag to GitHub
git push origin v1.0.0
```

### Step 4: Verify Workflow
1. Go to GitHub repo → **Actions** tab
2. Should see "Release Plugin" workflow running
3. Wait for ✅ completion
4. Check **Releases** tab for new release package

### Step 5: Test Plugin Installation
1. Copy the raw repository.json URL:
   ```
   https://raw.githubusercontent.com/YOUR_USERNAME/Lists2Playlists/main/repository.json
   ```

2. In Emby → Plugins → Catalog:
   - Click **Add Catalog**
   - Paste the URL
   - Should see Lists2Playlists appear

3. Click **Install**
   - Emby should download and extract the plugin
   - Restart Emby Server when prompted

## Ongoing Releases

### Creating a New Release

```bash
# Make changes
git add .
git commit -m "Your changes"

# Create version tag (follow semantic versioning)
git tag v1.1.0

# Push to GitHub (this triggers the workflow)
git push origin v1.1.0
```

The workflow will automatically:
- ✅ Build the project
- ✅ Create a `.zip` package
- ✅ Upload to GitHub Releases
- ✅ Update `repository.json`
- ✅ Commit updated `repository.json`

### Monitoring Releases

1. Check **Actions** tab for workflow status
2. Check **Releases** tab for downloadable packages
3. Check `repository.json` was updated (git log)

## What Gets Packaged

Each release `.zip` contains:
- `Lists2Playlists.dll` - The plugin binary
- `manifest.json` - Plugin metadata (copy of `plugin.json`)
- `configPage.html` - Configuration UI

## Emby Integration Points

### Users Can Install Via:

**Option A: Catalog Installation (Recommended)**
1. Plugins → Catalog → Add Catalog
2. Paste: `https://raw.githubusercontent.com/YOUR_USERNAME/Lists2Playlists/main/repository.json`
3. Search "Lists2Playlists"
4. Click Install → Auto-update support

**Option B: Direct Download**
1. Visit GitHub Releases
2. Download `Lists2Playlists-X.X.X.zip`
3. Extract to `{EmbyRoot}/plugins/Lists2Playlists/`
4. Restart Emby

**Option C: Custom Catalog URL**
1. If hosting elsewhere, provide custom `repository.json` URL
2. Emby discovers plugin and all versions

## Troubleshooting

### Workflow Failed
- Check **Actions** tab → click workflow → view logs
- Common issue: `dotnet` command not found (check runner OS)
- Verify `plugin.json` exists in root

### Plugin Won't Install
- Verify `repository.json` is valid JSON (use jsonlint.com)
- Check `downloadUrl` points to valid `.zip`
- Ensure `.zip` contains `manifest.json` and `.dll`

### Can't See Plugin in Catalog
- Use **raw** GitHub URL: `https://raw.githubusercontent.com/...`
- NOT: `https://github.com/...`
- Wait a few minutes for Emby cache to refresh

### Emby Won't Find Updates
- Make sure `repository.json` updated after release
- Check version numbers are different
- Verify `targetAbi` matches Emby version (4.8.0.0)

## Versioning Strategy

### Semantic Versioning
- `v1.0.0` - First release
- `v1.0.1` - Bug fixes
- `v1.1.0` - New features
- `v2.0.0` - Major changes

### Tag Format
```bash
git tag v1.0.0   # Major.Minor.Patch
git tag v1.0.0-beta  # Pre-release
```

## Version History Example

After creating multiple releases, `repository.json` will have:
```json
[
  { "version": "1.2.0", "downloadUrl": "..." },  // Latest first
  { "version": "1.1.0", "downloadUrl": "..." },
  { "version": "1.0.1", "downloadUrl": "..." },
  { "version": "1.0.0", "downloadUrl": "..." }
]
```

Emby shows newest version by default, but users can choose older versions if needed.

## Files Changed for Release Support

- ✅ `.github/workflows/release.yml` - GitHub Actions workflow
- ✅ `.github/scripts/update-repo.js` - Version update script
- ✅ `plugin.json` - Plugin manifest (new)
- ✅ `repository.json` - Plugin catalog (new)
- ✅ `GITHUB_RELEASES_SETUP.md` - Setup guide (new)
- ✅ `GITHUB_RELEASES_CHECKLIST.md` - This file (new)

## Next Steps

1. **Replace `YOUR_USERNAME`** in:
   - `repository.json` (3 places)
   - Optional: Update icon URL to point to repo icon

2. **Commit and push all files**:
   ```bash
   git add .github/ plugin.json repository.json *.md
   git commit -m "Add GitHub Actions release workflow and Emby plugin setup"
   git push
   ```

3. **Create your first release**:
   ```bash
   git tag v1.0.0
   git push origin v1.0.0
   ```

4. **Test in Emby**:
   - Add catalog URL
   - Search for Lists2Playlists
   - Click Install

## Support

For issues with:
- **GitHub Actions**: Check `.github/workflows/release.yml`
- **Emby Plugin System**: Refer to Emby plugin documentation
- **Lists2Playlists Plugin**: See main README.md

Your plugin is now ready for public release! 🎉
