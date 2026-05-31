using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lists2Playlists.Configuration;
using Lists2Playlists.Models;
using Lists2Playlists.Providers;
using Lists2Playlists.Services;

namespace Lists2Playlists.Sync
{
    /// <summary>
    /// Orchestrates the sync process for list configurations
    /// </summary>
    public class SyncOrchestrator : ISyncOrchestrator
    {
        private readonly IConfigurationService _configurationService;
        private readonly Dictionary<ListSourceType, IListProvider> _providers;
        private readonly ILibraryMatcher _libraryMatcher;
        private readonly IPlaylistService _playlistService;

        public SyncOrchestrator(
            IConfigurationService configurationService,
            ILibraryMatcher libraryMatcher,
            IPlaylistService playlistService,
            Dictionary<ListSourceType, IListProvider> providers)
        {
            _configurationService = configurationService ?? throw new ArgumentNullException(nameof(configurationService));
            _libraryMatcher = libraryMatcher ?? throw new ArgumentNullException(nameof(libraryMatcher));
            _playlistService = playlistService ?? throw new ArgumentNullException(nameof(playlistService));
            _providers = providers ?? new Dictionary<ListSourceType, IListProvider>();
        }

        public async Task SyncListAsync(ListConfiguration listConfig, CancellationToken cancellationToken = default)
        {
            if (listConfig == null)
                throw new ArgumentNullException(nameof(listConfig));

            try
            {
                // Get the appropriate provider
                if (!_providers.TryGetValue(listConfig.SourceType, out var provider))
                {
                    System.Diagnostics.Debug.WriteLine($"No provider found for {listConfig.SourceType}");
                    return;
                }

                // Fetch list items
                var listItems = await provider.FetchListItemsAsync(listConfig.ListUrl, cancellationToken);

                // Filter by media type
                if (!listConfig.IncludeMovies)
                    listItems = listItems.Where(li => li.MediaType != MediaType.Movie).ToList();
                if (!listConfig.IncludeShows)
                    listItems = listItems.Where(li => li.MediaType != MediaType.Show).ToList();

                // Match items to library
                var matchedItems = await MatchItemsAsync(listItems, listConfig, cancellationToken);

                // Filter out blacklisted items
                matchedItems = matchedItems
                    .Where(m => !listConfig.BlacklistedItems.Contains(m.ListItem.Id))
                    .ToList();

                // Get ordered item IDs based on sort order
                var orderedItemIds = GetOrderedItemIds(matchedItems, listConfig);

                // Create or update playlist
                var playlistName = BuildPlaylistName(listConfig);
                var existingPlaylistId = await _playlistService.FindPlaylistByNameAsync(playlistName, cancellationToken);

                if (existingPlaylistId.HasValue)
                {
                    await _playlistService.UpdatePlaylistAsync(existingPlaylistId.Value, orderedItemIds, cancellationToken);
                }
                else
                {
                    await _playlistService.CreatePlaylistAsync(playlistName, orderedItemIds, cancellationToken);
                }

                // Update last sync time
                listConfig.LastSyncTime = DateTime.UtcNow;
                _configurationService.SaveListConfiguration(listConfig);

                System.Diagnostics.Debug.WriteLine($"Successfully synced list: {listConfig.Name}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error syncing list {listConfig.Name}: {ex.Message}");
                throw;
            }
        }

        public async Task SyncAllListsAsync(CancellationToken cancellationToken = default)
        {
            var config = _configurationService.GetConfiguration();
            
            if (!config.Enabled)
                return;

            foreach (var listConfig in config.ListConfigurations)
            {
                try
                {
                    await SyncListAsync(listConfig, cancellationToken);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error syncing list {listConfig.Name}: {ex.Message}");
                }
            }
        }

        private async Task<List<MatchingResult>> MatchItemsAsync(
            List<ListItem> listItems, 
            ListConfiguration listConfig, 
            CancellationToken cancellationToken)
        {
            var results = new List<MatchingResult>();

            foreach (var item in listItems)
            {
                // Check if there's a manual match
                if (listConfig.ManualMatches.TryGetValue(item.Id, out var embyItemId))
                {
                    results.Add(new MatchingResult
                    {
                        ListItem = item,
                        EmbyItemId = embyItemId,
                        IsExactMatch = true,
                        ConfidenceScore = 100,
                        IsManualMatch = true
                    });
                }
                else
                {
                    var result = await _libraryMatcher.MatchListItemAsync(item, cancellationToken);
                    results.Add(result);
                }
            }

            return results;
        }

        private List<long> GetOrderedItemIds(List<MatchingResult> matchedItems, ListConfiguration listConfig)
        {
            var itemsWithIds = matchedItems
                .Where(m => m.EmbyItemId.HasValue)
                .ToList();

            return listConfig.SortOrder switch
            {
                SortOrderType.ListOrder => itemsWithIds
                    .OrderBy(m => m.ListItem.ListPosition)
                    .Select(m => m.EmbyItemId!.Value)
                    .ToList(),
                
                SortOrderType.Title => itemsWithIds
                    .OrderBy(m => m.ListItem.Title)
                    .Select(m => m.EmbyItemId!.Value)
                    .ToList(),
                
                SortOrderType.ReleaseDate => itemsWithIds
                    .OrderBy(m => m.ListItem.Year ?? int.MaxValue)
                    .Select(m => m.EmbyItemId!.Value)
                    .ToList(),
                
                SortOrderType.Custom => itemsWithIds
                    .OrderBy(m => m.ListItem.ListPosition)
                    .Select(m => m.EmbyItemId!.Value)
                    .ToList(),
                
                _ => itemsWithIds
                    .OrderBy(m => m.ListItem.ListPosition)
                    .Select(m => m.EmbyItemId!.Value)
                    .ToList()
            };
        }

        private string BuildPlaylistName(ListConfiguration listConfig)
        {
            var name = listConfig.Name;
            
            if (!string.IsNullOrEmpty(listConfig.PlaylistNamePrefix))
                name = $"{listConfig.PlaylistNamePrefix} {name}";
            
            if (!string.IsNullOrEmpty(listConfig.PlaylistNameSuffix))
                name = $"{name} {listConfig.PlaylistNameSuffix}";
            
            return name;
        }
    }
}
