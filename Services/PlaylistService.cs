using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lists2Playlists.Configuration;
using Lists2Playlists.Models;
using Lists2Playlists.Providers;
using MediaBrowser.Controller.Playlists;
using MediaBrowser.Model.Playlists;
using MediaBrowser.Controller.Entities;

namespace Lists2Playlists.Services
{
    /// <summary>
    /// Service for creating and managing playlists in Emby
    /// </summary>
    public class PlaylistService : IPlaylistService
    {
        private readonly IPlaylistManager _playlistManager;

        public PlaylistService(IPlaylistManager playlistManager)
        {
            _playlistManager = playlistManager ?? throw new ArgumentNullException(nameof(playlistManager));
        }

        public async Task<long?> CreatePlaylistAsync(string playlistName, List<long> itemIds, CancellationToken cancellationToken = default)
        {
            try
            {
                var request = new PlaylistCreationRequest
                {
                    Name = playlistName,
                    ItemIdList = itemIds,
                    MediaType = PlaylistMediaType.Video
                };

                var result = await _playlistManager.CreatePlaylist(request);
                return result.Id;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating playlist: {ex.Message}");
                return null;
            }
        }

        public async Task UpdatePlaylistAsync(long playlistId, List<long> itemIds, CancellationToken cancellationToken = default)
        {
            try
            {
                var playlist = _playlistManager.GetPlaylistsFolder()
                    .GetChildren(null, null)
                    .OfType<Playlist>()
                    .FirstOrDefault(p => p.Id == playlistId);

                if (playlist == null)
                    throw new InvalidOperationException($"Playlist with ID {playlistId} not found");

                // Remove all existing items
                var existingItemIds = playlist.LinkedChildren?.Select(c => c.ItemId ?? 0).ToList() ?? new List<long>();
                if (existingItemIds.Count > 0)
                {
                    await _playlistManager.RemoveFromPlaylist(playlistId, existingItemIds.ToArray());
                }

                // Add new items
                if (itemIds.Count > 0)
                {
                    await _playlistManager.AddToPlaylist(playlistId, itemIds.ToArray(), null);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating playlist: {ex.Message}");
                throw;
            }
        }

        public async Task ReorderPlaylistAsync(long playlistId, Dictionary<long, int> itemToNewPosition, CancellationToken cancellationToken = default)
        {
            try
            {
                var playlist = _playlistManager.GetPlaylistsFolder()
                    .GetChildren(null, null)
                    .OfType<Playlist>()
                    .FirstOrDefault(p => p.Id == playlistId);

                if (playlist == null)
                    throw new InvalidOperationException($"Playlist with ID {playlistId} not found");

                // Reorder items by moving each one to its target position
                foreach (var (entryId, newIndex) in itemToNewPosition.OrderBy(x => x.Value))
                {
                    var linkedChild = playlist.LinkedChildren?.FirstOrDefault(c => c.ItemId == entryId);
                    if (linkedChild != null)
                    {
                        await _playlistManager.MoveItem(playlistId, linkedChild.Id, newIndex);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error reordering playlist: {ex.Message}");
                throw;
            }
        }

        public async Task<List<long>> GetPlaylistItemsAsync(long playlistId, CancellationToken cancellationToken = default)
        {
            try
            {
                var playlist = _playlistManager.GetPlaylistsFolder()
                    .GetChildren(null, null)
                    .OfType<Playlist>()
                    .FirstOrDefault(p => p.Id == playlistId);

                if (playlist == null)
                    return new List<long>();

                return playlist.LinkedChildren?
                    .Select(c => c.ItemId ?? 0)
                    .Where(id => id > 0)
                    .ToList() ?? new List<long>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting playlist items: {ex.Message}");
                return new List<long>();
            }
        }

        public async Task<long?> FindPlaylistByNameAsync(string playlistName, CancellationToken cancellationToken = default)
        {
            try
            {
                var playlist = _playlistManager.GetPlaylistsFolder()
                    .GetChildren(null, null)
                    .OfType<Playlist>()
                    .FirstOrDefault(p => p.Name == playlistName);

                return playlist?.Id;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error finding playlist: {ex.Message}");
                return null;
            }
        }

        public async Task DeletePlaylistAsync(long playlistId, CancellationToken cancellationToken = default)
        {
            try
            {
                var playlist = _playlistManager.GetPlaylistsFolder()
                    .GetChildren(null, null)
                    .OfType<Playlist>()
                    .FirstOrDefault(p => p.Id == playlistId);

                if (playlist != null)
                {
                    // Delete the playlist item
                    // This is a simplified approach - actual implementation may vary
                    var playlistsFolder = _playlistManager.GetPlaylistsFolder();
                    playlistsFolder.RemoveChild(playlist, CancellationToken.None);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting playlist: {ex.Message}");
                throw;
            }
        }
    }
}
