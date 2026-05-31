# 🚀 Quick Start: Setting Up GitHub Releases & Emby Plugin Repository

## What Was Added

Your repo now has:
- ✅ **GitHub Actions workflow** - Automatic build & release on tag push
- ✅ **Plugin manifest** (`plugin.json`) - Emby metadata
- ✅ **Plugin repository** (`repository.json`) - Emby catalog file
- ✅ **Release script** - Auto-updates repository.json
- ✅ **Documentation** - Complete setup guides

## 3-Step Setup

### Step 1: Update Your GitHub Username (2 minutes)

Files to edit: **`repository.json`**

Replace `YOUR_USERNAME` with your actual GitHub username in 3 places:

```json
{
  "sourceUrl": "https://github.com/YOUR_USERNAME/Lists2Playlists",
  "downloadUrl": "https://github.com/YOUR_USERNAME/Lists2Playlists/releases/download/v1.0.0/Lists2Playlists-1.0.0.zip",
  "imageUrl": "https://raw.githubusercontent.com/YOUR_USERNAME/Lists2Playlists/main/icon.png"
}
```

### Step 2: Commit & Push (1 minute)

```bash
cd /NVME2/Projects/Lists2Playlists

# Stage all new files
git add .github/ plugin.json repository.json *.md

# Commit
git commit -m "Add GitHub Actions release workflow and Emby plugin support"

# Push to GitHub
git push origin main
```

### Step 3: Create Your First Release (1 minute)

```bash
# Create a version tag (v1.0.0 = your first release)
git tag v1.0.0

# Push the tag (this triggers the workflow)
git push origin v1.0.0
```

**That's it! 🎉**

GitHub Actions will now automatically:
1. Build your project
2. Create a `.zip` package
3. Upload to GitHub Releases
4. Update `repository.json`

## Test It Worked

### Check GitHub Actions
1. Go to your repo on GitHub
2. Click **Actions** tab
3. You should see "Release Plugin" workflow running/completed

### Check Releases
1. Click **Releases** tab on your GitHub repo
2. Should show "v1.0.0" with downloadable `.zip` file

### Test in Emby
1. Open Emby
2. Go to **Plugins** → **Catalog** → **Add Catalog**
3. Paste:
   ```
   https://raw.githubusercontent.com/YOUR_USERNAME/Lists2Playlists/main/repository.json
   ```
4. Click Add
5. Search for "Lists2Playlists"
6. Should appear with Install button

## Usage Going Forward

### Make a New Release

```bash
# Make changes
git add .
git commit -m "Fix matching algorithm"

# Create new version tag
git tag v1.0.1

# Push (triggers workflow)
git push origin v1.0.1
```

**Workflow automatically handles everything else!**

### Versioning

```
v1.0.0 - Initial release
v1.0.1 - Bug fix
v1.1.0 - New feature
v2.0.0 - Major changes
```

## Repository Structure

Your repo now supports:

```
Emby Plugin Catalog Installation:
├── User adds catalog URL
├── Emby reads repository.json
├── Shows available versions
├── User clicks Install
└── Emby downloads & extracts .zip
```

## File Reference

| File | Purpose |
|------|---------|
| `.github/workflows/release.yml` | GitHub Actions workflow |
| `.github/scripts/update-repo.js` | Updates repository.json |
| `plugin.json` | Plugin metadata |
| `repository.json` | Emby plugin catalog |
| `GITHUB_RELEASES_SETUP.md` | Detailed setup guide |
| `GITHUB_RELEASES_CHECKLIST.md` | Step-by-step checklist |
| `REPOSITORY_README.md` | Repo overview for users |

## Complete Release Flow

```
1. git tag v1.0.1
2. git push origin v1.0.1
   ↓
3. GitHub Actions triggers
   ├─ dotnet build
   ├─ Create .zip
   ├─ Create Release
   ├─ Upload .zip as asset
   └─ Update repository.json
   ↓
4. Users can install from Emby
   ├─ Add catalog
   ├─ Search plugin
   ├─ Click Install
   └─ Auto-updates on next release
```

## Troubleshooting

### Workflow Failed?
- Check **Actions** tab → click workflow → view logs
- Make sure all files are committed and pushed

### Plugin Won't Show in Emby?
- Use **raw** GitHub URL: `https://raw.githubusercontent.com/...`
- NOT regular: `https://github.com/...`

### Can't Download .zip?
- Check **Releases** tab - should have `.zip` file
- Verify download URL in `repository.json` is correct

## What Users See

When users add your catalog to Emby, they'll see:

```
Plugins → Catalog → Add Catalog
[Paste URL]
↓
Lists2Playlists v1.0.0
├─ Description: Create Emby playlists from online lists...
├─ Source: Trakt, SIMKL, Letterboxd
├─ Auto-update: Yes
└─ [Install Button]
```

## Advanced: Update Icon

To show a plugin icon in Emby catalogs:

1. Add an `icon.png` (64x64 pixels) to your repo root
2. Update `imageUrl` in `repository.json`:
   ```json
   "imageUrl": "https://raw.githubusercontent.com/YOUR_USERNAME/Lists2Playlists/main/icon.png"
   ```

## Need Help?

- **How do releases work?** → See `GITHUB_RELEASES_SETUP.md`
- **Step-by-step checklist?** → See `GITHUB_RELEASES_CHECKLIST.md`
- **Plugin details?** → See `README.md`
- **Technical info?** → See `COMPLETE_IMPLEMENTATION_DETAILS.md`

---

## Summary

✅ Your repo is now a full-featured Emby plugin repository  
✅ Releases are completely automated via GitHub Actions  
✅ Users can install directly from Emby with automatic updates  
✅ Just push a tag to create a release  

**You're all set! Push your first tag and watch the magic happen! 🎉**
