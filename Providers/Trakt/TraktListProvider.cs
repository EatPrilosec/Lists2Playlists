using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Lists2Playlists.Configuration;
using Lists2Playlists.Models;

namespace Lists2Playlists.Providers.Trakt
{
    /// <summary>
    /// Trakt list provider implementation
    /// </summary>
    public class TraktListProvider : IListProvider
    {
        public ListSourceType SourceType => ListSourceType.Trakt;
        public string ProviderName => "Trakt";

        public TraktListProvider(HttpClient? httpClient = null)
        {
        }

        public async Task<List<ListItem>> FetchListItemsAsync(string listUrl, CancellationToken cancellationToken = default)
        {
            // Dummy implementation - no items
            return new List<ListItem>();
        }

        public async Task<(string UserCode, string VerificationUrl, string DeviceCode, int ExpiresIn, int Interval)> GetDeviceCodesAsync(CancellationToken cancellationToken = default)
        {
            // Dummy implementation
            return ("user_code", "https://example.com/device", "device_code", 600, 5);
        }

        public async Task<OAuthToken?> PollForAccessTokenAsync(string deviceCode, int interval, int expiresIn, CancellationToken cancellationToken = default)
        {
            // Dummy implementation - return a fake token after first poll
            return new OAuthToken
            {
                AccessToken = "fake_access_token",
                RefreshToken = "fake_refresh_token",
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                TokenType = "Bearer"
            };
        }

        public OAuthToken? GetCurrentToken() => null;

        public void SetOAuthToken(OAuthToken token) { }
    }
}