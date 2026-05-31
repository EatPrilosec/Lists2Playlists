# Lists2Playlists - Emby Plugin

Create Emby playlists from online list services like Trakt, SIMKL, Letterboxd, mdblist, IMDb, and Serializd.

## Features

- **Multiple List Sources**: Support for Trakt, SIMKL, Letterboxd, mdblist, IMDb, and Serializd
- **Automatic Matching**: Intelligently matches list items to your Emby library using:
  - IMDb IDs (most reliable)
  - TMDB IDs
  - Title and year matching
  - Fuzzy matching for difficult titles
- **Manual Matching**: UI for manually matching items that weren't auto-matched
- **Blacklisting**: Exclude specific items from playlists
- **Flexible Sorting**: Order playlist items by:
  - List order (default)
  - Title
  - Release date
  - Custom (maintains manual order)
- **Customizable Playlist Names**: Add prefixes and suffixes to playlist names
- **Media Type Filtering**: Choose to include/exclude movies and TV shows
- **OAuth Device Flow**: Trakt authentication without needing API keys (device code flow)
- **Automatic Sync**: Scheduled task to keep playlists up-to-date
- **Default List**: Marvel Cinematic Universe list included by default (removable)

## Installation

1. Download the latest release of Lists2Playlists
2. Extract the plugin to your Emby plugins directory
3. Restart Emby Server
4. The plugin should now appear in your Plugins section

## Configuration

### Plugin Settings
- **Enable Plugin**: Turn the plugin on/off
- **Sync Interval**: How often to sync lists (1-24 hours)

### Adding List Configurations

1. Go to Lists2Playlists settings
2. Click "Add New List"
3. Fill in the following information:
   - **List Name**: Display name for the list
   - **List URL**: The URL or identifier for the list
   - **Source Service**: Which service the list is from (Trakt, SIMKL, etc.)
   - **Playlist Name Prefix/Suffix**: Optional text to add to the playlist name
   - **Include Movies/Shows**: Whether to include these media types
   - **Sort Order**: How to arrange items in the playlist

### Authentication

#### Trakt
The plugin uses Trakt's device flow authentication, requiring no API keys:
1. Click "Authenticate with Trakt"
2. You'll see a code to enter
3. Visit the verification URL and enter the code
4. The plugin automatically completes the authentication

## Default List

The plugin includes a default Marvel Cinematic Universe list from Trakt:
- **URL**: `trakt.tv/users/moses456/lists/marvel-cinematic-universe`
- This can be removed from the configuration at any time

## Manual Matching & Blacklisting

For items that don't auto-match:
1. Go to the list configuration
2. In the "Manual Matching" section, search for the correct item in your library
3. To exclude an item, add it to the "Blacklist"

## Troubleshooting

### Items Not Matching
- Check that your Emby library has correct metadata (titles, years, IMDb IDs)
- Try manually matching items through the configuration UI
- Check the plugin logs for matching errors

### Playlist Not Created
- Ensure the list URL is correct
- Check that you have items matching your media type filters
- Verify the plugin is enabled

### Sync Not Running
- Check that "Enable Plugin" is turned on
- Verify the sync interval is set appropriately
- Look for any errors in the Emby server logs

## Architecture

### Core Components

- **IListProvider**: Interface for list service integrations
- **ILibraryMatcher**: Matches list items to Emby library items
- **IPlaylistService**: Creates and manages Emby playlists
- **ISyncOrchestrator**: Orchestrates the sync process
- **IConfigurationService**: Manages plugin settings

### Supported List Services

- **Trakt**: Full support including device flow authentication
- **SIMKL**: Planned
- **Letterboxd**: Planned
- **mdblist**: Planned
- **IMDb**: Planned
- **Serializd**: Planned

## Development

### Building

```bash
cd /NVME2/Projects/Lists2Playlists
dotnet build
```

### Project Structure

```
Lists2Playlists/
├── Configuration/
│   ├── PluginConfiguration.cs    # Plugin configuration model
│   └── configPage.html           # Web UI for configuration
├── Models/
│   └── ListModels.cs             # Data models
├── Providers/
│   ├── IListProvider.cs          # Provider interfaces
│   ├── LibraryMatcher.cs         # Library matching logic
│   └── Trakt/
│       └── TraktListProvider.cs  # Trakt implementation
├── Services/
│   ├── PlaylistService.cs        # Playlist management
│   └── ConfigurationService.cs   # Configuration management
├── Sync/
│   └── SyncOrchestrator.cs       # Sync orchestration
├── ScheduledTasks/
│   └── SyncListsTask.cs          # Scheduled sync task
├── Plugin.cs                      # Main plugin class
└── ServerEntryPoint.cs            # Server entry point
```

## Contributing

Contributions are welcome! Please feel free to submit pull requests or issues.

## License

[License information to be added]

## Future Enhancements

- [ ] Additional list service providers (SIMKL, Letterboxd, etc.)
- [ ] WebUI API endpoints for managing matches
- [ ] Advanced filtering options
- [ ] Playlist merge functionality
- [ ] Reverse sync (Emby to Trakt)
- [ ] Episode-level support for TV show lists
- [ ] Performance optimizations
- [ ] Internationalization support
