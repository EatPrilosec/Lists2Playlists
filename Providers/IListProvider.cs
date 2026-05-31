using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Lists2Playlists.Configuration;
using Lists2Playlists.Models;

namespace Lists2Playlists.Providers
{
    /// <summary>
    /// Base interface for list providers
    /// </summary>
    public interface IListProvider
    {
        /// <summary>
        /// Gets the type of list service this provider handles
        /// </summary>
        ListSourceType SourceType { get; }

        /// <summary>
        /// Gets the name of this provider
        /// </summary>
        string ProviderName { get; }

        /// <summary>
        /// Fetches items from a list
        /// </summary>
        Task<List<ListItem>> FetchListItemsAsync(string listUrl, CancellationToken cancellationToken = default);

        /// <summary>
        /// Initiates OAuth device flow authentication
        /// </summary>
        Task<(string UserCode, string VerificationUrl, string DeviceCode, int ExpiresIn, int Interval)> GetDeviceCodesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Polls for OAuth token after user authorization
        /// </summary>
        Task<OAuthToken?> PollForAccessTokenAsync(string deviceCode, int interval, int expiresIn, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the current OAuth token for this provider
        /// </summary>
        OAuthToken? GetCurrentToken();

        /// <summary>
        /// Sets the OAuth token for this provider
        /// </summary>
        void SetOAuthToken(OAuthToken token);
    }

    /// <summary>
    /// Library matching interface for matching list items to Emby library items
    /// </summary>
    public interface ILibraryMatcher
    {
        /// <summary>
        /// Matches a list item to items in the Emby library
        /// </summary>
        Task<MatchingResult> MatchListItemAsync(ListItem listItem, CancellationToken cancellationToken = default);

        /// <summary>
        /// Searches the library for potential matches
        /// </summary>
        Task<List<(long ItemId, string Title, int? Year, MediaType Type)>> SearchLibraryAsync(string title, int? year = null, string? imdbId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets an item from the library by ID
        /// </summary>
        Task<(string Title, int? Year)?> GetLibraryItemAsync(long itemId, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Playlist manager interface for creating and updating playlists
    /// </summary>
    public interface IPlaylistService
    {
        /// <summary>
        /// Creates a new playlist from matched items
        /// </summary>
        Task<long?> CreatePlaylistAsync(string playlistName, List<long> itemIds, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing playlist with new items
        /// </summary>
        Task UpdatePlaylistAsync(long playlistId, List<long> itemIds, CancellationToken cancellationToken = default);

        /// <summary>
        /// Reorders items in a playlist
        /// </summary>
        Task ReorderPlaylistAsync(long playlistId, Dictionary<long, int> itemToNewPosition, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the current items in a playlist
        /// </summary>
        Task<List<long>> GetPlaylistItemsAsync(long playlistId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Finds a playlist by name
        /// </summary>
        Task<long?> FindPlaylistByNameAsync(string playlistName, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a playlist
        /// </summary>
        Task DeletePlaylistAsync(long playlistId, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Configuration service interface for managing plugin settings
    /// </summary>
    public interface IConfigurationService
    {
        /// <summary>
        /// Gets the current plugin configuration
        /// </summary>
        PluginConfiguration GetConfiguration();

        /// <summary>
        /// Saves the plugin configuration
        /// </summary>
        void SaveConfiguration(PluginConfiguration configuration);

        /// <summary>
        /// Gets a specific list configuration
        /// </summary>
        ListConfiguration? GetListConfiguration(string listId);

        /// <summary>
        /// Saves or updates a list configuration
        /// </summary>
        void SaveListConfiguration(ListConfiguration configuration);

        /// <summary>
        /// Removes a list configuration
        /// </summary>
        void RemoveListConfiguration(string listId);
    }

    /// <summary>
    /// Sync orchestration interface
    /// </summary>
    public interface ISyncOrchestrator
    {
        /// <summary>
        /// Performs a full sync for a specific list configuration
        /// </summary>
        Task SyncListAsync(ListConfiguration listConfig, CancellationToken cancellationToken = default);

        /// <summary>
        /// Performs a full sync for all list configurations
        /// </summary>
        Task SyncAllListsAsync(CancellationToken cancellationToken = default);
    }
}
