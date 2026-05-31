using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MediaBrowser.Common.Configuration;

namespace Lists2Playlists.Configuration
{
    /// <summary>
    /// Plugin configuration model for Lists2Playlists
    /// </summary>
    public class PluginConfiguration : BasePluginConfiguration
    {
        /// <summary>
        /// Gets or sets the list configurations
        /// </summary>
        [DisplayName("List Configurations")]
        [Description("Configure list sources and their corresponding playlists")]
        public List<ListConfiguration> ListConfigurations { get; set; } = new List<ListConfiguration>();

        /// <summary>
        /// Gets or sets a value indicating whether to enable the plugin
        /// </summary>
        [DisplayName("Enable Plugin")]
        [Description("Enable or disable the Lists2Playlists plugin")]
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Gets or sets the sync interval in hours
        /// </summary>
        [DisplayName("Sync Interval (Hours)")]
        [Description("How often to sync lists with playlists (minimum 1 hour)")]
        [Range(1, 24)]
        public int SyncIntervalHours { get; set; } = 6;
    }

    /// <summary>
    /// Configuration for a single list
    /// </summary>
    public class ListConfiguration
    {
        /// <summary>
        /// Unique identifier for this list configuration
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Gets or sets the display name for this list
        /// </summary>
        [DisplayName("List Name")]
        [Description("Display name for this list")]
        [Required]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the URL or identifier of the list
        /// </summary>
        [DisplayName("List URL")]
        [Description("URL or identifier (e.g., trakt.tv/users/username/lists/list-slug or SIMKL list ID)")]
        [Required]
        public string ListUrl { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the source service (Trakt, SIMKL, Letterboxd, etc.)
        /// </summary>
        [DisplayName("Source Service")]
        [Description("The service this list comes from")]
        public ListSourceType SourceType { get; set; } = ListSourceType.Trakt;

        /// <summary>
        /// Gets or sets the playlist name prefix
        /// </summary>
        [DisplayName("Playlist Name Prefix")]
        [Description("Optional prefix to add to the playlist name")]
        public string PlaylistNamePrefix { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the playlist name suffix
        /// </summary>
        [DisplayName("Playlist Name Suffix")]
        [Description("Optional suffix to add to the playlist name")]
        public string PlaylistNameSuffix { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether to include movies
        /// </summary>
        [DisplayName("Include Movies")]
        [Description("Include movies from this list in the playlist")]
        public bool IncludeMovies { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to include shows
        /// </summary>
        [DisplayName("Include Shows")]
        [Description("Include TV shows from this list in the playlist")]
        public bool IncludeShows { get; set; } = true;

        /// <summary>
        /// Gets or sets the sort order for playlist items
        /// </summary>
        [DisplayName("Sort Order")]
        [Description("How to order items in the playlist")]
        public SortOrderType SortOrder { get; set; } = SortOrderType.ListOrder;

        /// <summary>
        /// Gets or sets a value indicating whether items have been manually matched
        /// </summary>
        [DisplayName("Has Manual Matches")]
        [Description("Whether manual matching has been performed for this list")]
        public bool HasManualMatches { get; set; } = false;

        /// <summary>
        /// Gets or sets the blacklisted item IDs (items to exclude)
        /// </summary>
        [DisplayName("Blacklisted Items")]
        [Description("Trakt IDs of items to exclude from the playlist")]
        public List<string> BlacklistedItems { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets manual matches (list item ID -> Emby library item ID)
        /// </summary>
        [DisplayName("Manual Matches")]
        [Description("Manual matches between list items and library items")]
        public Dictionary<string, long> ManualMatches { get; set; } = new Dictionary<string, long>();

        /// <summary>
        /// Gets or sets the last sync time
        /// </summary>
        public DateTime? LastSyncTime { get; set; }
    }

    /// <summary>
    /// List source types
    /// </summary>
    public enum ListSourceType
    {
        Trakt = 0,
        SIMKL = 1,
        Serializd = 2,
        Letterboxd = 3,
        mdblist = 4,
        IMDb = 5
    }

    /// <summary>
    /// Sort order types for playlist items
    /// </summary>
    public enum SortOrderType
    {
        ListOrder = 0,
        Title = 1,
        ReleaseDate = 2,
        Custom = 3
    }
}
