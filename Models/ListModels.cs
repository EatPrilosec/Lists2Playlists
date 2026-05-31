using System;
using System.Collections.Generic;

namespace Lists2Playlists.Models
{
    /// <summary>
    /// Represents a list item from an online list service
    /// </summary>
    public class ListItem
    {
        /// <summary>
        /// Unique identifier from the list service
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Title of the item
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Release year
        /// </summary>
        public int? Year { get; set; }

        /// <summary>
        /// Type of item (Movie or Show)
        /// </>
        public MediaType MediaType { get; set; }

        /// <summary>
        /// IMDb ID if available
        /// </summary>
        public string? ImdbId { get; set; }

        /// <summary>
        /// TMDB ID if available
        /// </summary>
        public int? TmdbId { get; set; }

        /// <summary>
        /// TVDB ID if available
        /// </summary>
        public int? TvdbId { get; set; }

        /// <summary>
        /// Trakt ID if available
        /// </summary>
        public int? TraktId { get; set; }

        /// <summary>
        /// Position in the list (1-based)
        /// </summary>
        public int ListPosition { get; set; }

        /// <summary>
        /// Matched Emby item ID (if found in library)
        /// </summary>
        public long? MatchedEmbyItemId { get; set; }

        /// <summary>
        /// Indicates if this item was manually matched
        /// </summary>
        public bool IsManualMatch { get; set; }

        /// <summary>
        /// Indicates if this item is blacklisted
        /// </summary>
        public bool IsBlacklisted { get; set; }
    }

    /// <summary>
    /// Media type enumeration
    /// </summary>
    public enum MediaType
    {
        Movie = 0,
        Show = 1
    }

    /// <summary>
    /// Result of matching a list item to an Emby library item
    /// </summary>
    public class MatchingResult
    {
        /// <summary>
        /// The original list item
        /// </summary>
        public ListItem ListItem { get; set; } = new ListItem();

        /// <summary>
        /// The matched Emby item ID
        /// </summary>
        public long? EmbyItemId { get; set; }

        /// <summary>
        /// Confidence level of the match (0-100)
        /// </summary>
        public int ConfidenceScore { get; set; }

        /// <summary>
        /// Indicates if this is an exact match
        /// </summary>
        public bool IsExactMatch { get; set; }

        /// <summary>
        /// Indicates if this match requires user confirmation
        /// </summary>
        public bool RequiresConfirmation { get; set; }

        /// <summary>
        /// Indicates if this match was manually configured
        /// </summary>
        public bool IsManualMatch { get; set; }

        /// <summary>
        /// Additional candidates for manual matching
        /// </summary>
        public List<(long ItemId, string Title, int? Year, int Score)> Candidates { get; set; } = new List<(long, string, int?, int)>();
    }

    /// <summary>
    /// OAuth token information
    /// </summary>
    public class OAuthToken
    {
        /// <summary>
        /// Access token
        /// </summary>
        public string AccessToken { get; set; } = string.Empty;

        /// <summary>
        /// Refresh token
        /// </summary>
        public string? RefreshToken = null;

        /// <summary>
        /// Token expiration time
        /// </summary>
        public DateTime ExpiresAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Token type (usually "Bearer")
        /// </summary>
        public string TokenType { get; set; } = "Bearer";
    }
}