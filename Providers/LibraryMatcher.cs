using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lists2Playlists.Models;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Model.Entities;

namespace Lists2Playlists.Providers
{
    /// <summary>
    /// Matches list items to Emby library items
    /// </summary>
    public class LibraryMatcher : ILibraryMatcher
    {
        private readonly ILibraryManager _libraryManager;

        public LibraryMatcher(ILibraryManager libraryManager)
        {
            _libraryManager = libraryManager ?? throw new ArgumentNullException(nameof(libraryManager));
        }

        public async Task<MatchingResult> MatchListItemAsync(ListItem listItem, CancellationToken cancellationToken = default)
        {
            var result = new MatchingResult { ListItem = listItem };

            try
            {
                // Try matching by external IDs first (most reliable)
                if (!string.IsNullOrEmpty(listItem.ImdbId))
                {
                    var itemId = await FindByExternalIdAsync(listItem.ImdbId, "IMDb", cancellationToken);
                    if (itemId.HasValue)
                    {
                        result.EmbyItemId = itemId;
                        result.IsExactMatch = true;
                        result.ConfidenceScore = 100;
                        return result;
                    }
                }

                if (listItem.TmdbId.HasValue)
                {
                    var itemId = await FindByExternalIdAsync(listItem.TmdbId.ToString()!, "TMDB", cancellationToken);
                    if (itemId.HasValue)
                    {
                        result.EmbyItemId = itemId;
                        result.IsExactMatch = true;
                        result.ConfidenceScore = 100;
                        return result;
                    }
                }

                // Fall back to searching by title and year
                var candidates = await SearchLibraryAsync(listItem.Title, listItem.Year, null, cancellationToken);
                result.Candidates = candidates.Select(c => (c.ItemId, c.Title, c.Year, 0)).ToList();

                if (candidates.Count > 0)
                {
                    var bestMatch = candidates.First();
                    result.EmbyItemId = bestMatch.ItemId;
                    result.IsExactMatch = true;
                    result.ConfidenceScore = CalculateMatchScore(listItem, bestMatch.Title, bestMatch.Year);
                }
                else
                {
                    result.RequiresConfirmation = true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error matching list item: {ex.Message}");
                result.RequiresConfirmation = true;
            }

            return result;
        }

        public async Task<List<(long ItemId, string Title, int? Year, MediaType Type)>> SearchLibraryAsync(
            string title, int? year = null, string? imdbId = null, CancellationToken cancellationToken = default)
        {
            var results = new List<(long, string, int?, MediaType)>();

            try
            {
                // Get all items matching the title
                var items = _libraryManager.GetItemList(new InternalItemsQuery
                {
                    IncludeItemTypes = new[] { typeof(Movie).Name, typeof(Series).Name },
                    Name = title,
                    Limit = 10
                });

                foreach (var item in items)
                {
                    if (item is Movie movie)
                    {
                        results.Add((item.Id, item.Name, movie.ProductionYear, MediaType.Movie));
                    }
                    else if (item is Series series)
                    {
                        results.Add((item.Id, item.Name, series.ProductionYear, MediaType.Show));
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error searching library: {ex.Message}");
            }

            return results.OrderByDescending(r => CalculateMatchScore(title, year, r.Title, r.Year)).ToList();
        }

        public async Task<(string Title, int? Year)?> GetLibraryItemAsync(long itemId, CancellationToken cancellationToken = default)
        {
            try
            {
                var item = _libraryManager.GetItemById(itemId);
                if (item != null)
                {
                    int? year = null;
                    if (item is Movie movie)
                        year = movie.ProductionYear;
                    else if (item is Series series)
                        year = series.ProductionYear;

                    return (item.Name, year);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting library item: {ex.Message}");
            }

            return null;
        }

        private async Task<long?> FindByExternalIdAsync(string externalId, string providerName, CancellationToken cancellationToken)
        {
            try
            {
                var items = _libraryManager.GetItemList(new InternalItemsQuery
                {
                    IncludeItemTypes = new[] { typeof(Movie).Name, typeof(Series).Name }
                });

                foreach (var item in items)
                {
                    if (item.ProviderIds != null && item.ProviderIds.ContainsKey(providerName))
                    {
                        if (item.ProviderIds[providerName].Equals(externalId, StringComparison.OrdinalIgnoreCase))
                        {
                            return item.Id;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error finding by external ID: {ex.Message}");
            }

            return null;
        }

        private int CalculateMatchScore(ListItem listItem, string libraryTitle, int? libraryYear)
        {
            return CalculateMatchScore(listItem.Title, listItem.Year, libraryTitle, libraryYear);
        }

        private int CalculateMatchScore(string listTitle, int? listYear, string libraryTitle, int? libraryYear)
        {
            int score = 0;

            // Exact title match
            if (listTitle.Equals(libraryTitle, StringComparison.OrdinalIgnoreCase))
                score += 80;
            // Partial match
            else if (listTitle.Contains(libraryTitle, StringComparison.OrdinalIgnoreCase) ||
                     libraryTitle.Contains(listTitle, StringComparison.OrdinalIgnoreCase))
                score += 50;
            // Similar title
            else
                score += LevenshteinDistance(listTitle, libraryTitle) > 0.8 ? 30 : 0;

            // Year match
            if (listYear.HasValue && libraryYear.HasValue && listYear == libraryYear)
                score += 20;

            return Math.Min(100, score);
        }

        private double LevenshteinDistance(string s1, string s2)
        {
            var len1 = s1.Length;
            var len2 = s2.Length;
            var d = new int[len1 + 1, len2 + 1];

            for (int i = 0; i <= len1; i++)
                d[i, 0] = i;

            for (int j = 0; j <= len2; j++)
                d[0, j] = j;

            for (int i = 1; i <= len1; i++)
            {
                for (int j = 1; j <= len2; j++)
                {
                    var cost = s1[i - 1] == s2[j - 1] ? 0 : 1;
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost
                    );
                }
            }

            var max = Math.Max(len1, len2);
            return 1.0 - (d[len1, len2] / (double)max);
        }
    }
}
