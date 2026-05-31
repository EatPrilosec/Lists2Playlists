using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
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
        private const string TraktApiUrl = "https://api.trakt.tv";
        private const string TraktApiVersion = "2";
        private static readonly string ClientId = "9b36d8c0db59eff5038aea7a417d73e69aea75b41aac771816d2ef1b3109cc2f";
        private static readonly string ClientSecret = "9b5da89c83f7cc96dd9e696da20d42f3c4f86d86d7abf1f0c3e50d5e4b8c92c0";

        private readonly HttpClient _httpClient;
        private OAuthToken? _currentToken;

        public ListSourceType SourceType => ListSourceType.Trakt;
        public string ProviderName => "Trakt";

        public TraktListProvider(HttpClient? httpClient = null)
        {
            _httpClient = httpClient ?? new HttpClient();
        }

        public async Task<List<ListItem>> FetchListItemsAsync(string listUrl, CancellationToken cancellationToken = default)
        {
            var items = new List<ListItem>();
            
            try
            {
                // Parse listUrl: e.g., "trakt.tv/users/moses456/lists/marvel-cinematic-universe"
                // Extract: users/{username}/lists/{slug}
                var uri = NormalizeListUrl(listUrl);
                
                var request = new HttpRequestMessage(HttpMethod.Get, $"{TraktApiUrl}/{uri}?extended=full&limit=500");
                request.Headers.Add("trakt-api-version", TraktApiVersion);
                request.Headers.Add("trakt-api-key", ClientId);
                request.Headers.Add("User-Agent", "Lists2Playlists/1.0");

                if (_currentToken != null && !string.IsNullOrEmpty(_currentToken.AccessToken))
                {
                    request.Headers.Add("Authorization", $"Bearer {_currentToken.AccessToken}");
                }

                var response = await _httpClient.SendAsync(request, cancellationToken);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsAsync<dynamic>(cancellationToken);
                
                if (jsonResponse is System.Collections.IEnumerable listItems)
                {
                    int position = 1;
                    foreach (var item in listItems)
                    {
                        var listItem = ParseTraktListItem(item, position);
                        if (listItem != null)
                        {
                            items.Add(listItem);
                            position++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error - would use ILogger in real implementation
                System.Diagnostics.Debug.WriteLine($"Error fetching Trakt list: {ex.Message}");
            }

            return items;
        }

        public async Task<(string UserCode, string VerificationUrl, string DeviceCode, int ExpiresIn, int Interval)> GetDeviceCodesAsync(CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{TraktApiUrl}/oauth/device/code");
            request.Headers.Add("trakt-api-version", TraktApiVersion);
            request.Headers.Add("trakt-api-key", ClientId);
            request.Headers.Add("User-Agent", "Lists2Playlists/1.0");
            request.Content = JsonContent.Create(new { client_id = ClientId });

            var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadAsAsync<TraktDeviceCodeResponse>(cancellationToken);
            
            return (data.user_code, data.verification_url, data.device_code, data.expires_in, data.interval);
        }

        public async Task<OAuthToken?> PollForAccessTokenAsync(string deviceCode, int interval, int expiresIn, CancellationToken cancellationToken = default)
        {
            var endTime = DateTime.UtcNow.AddSeconds(expiresIn);
            
            while (DateTime.UtcNow < endTime)
            {
                await Task.Delay(interval * 1000, cancellationToken);

                try
                {
                    var request = new HttpRequestMessage(HttpMethod.Post, $"{TraktApiUrl}/oauth/device/token");
                    request.Headers.Add("trakt-api-version", TraktApiVersion);
                    request.Headers.Add("trakt-api-key", ClientId);
                    request.Headers.Add("User-Agent", "Lists2Playlists/1.0");
                    request.Content = JsonContent.Create(new
                    {
                        code = deviceCode,
                        client_id = ClientId,
                        client_secret = ClientSecret
                    });

                    var response = await _httpClient.SendAsync(request, cancellationToken);

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var data = await response.Content.ReadAsAsync<TraktTokenResponse>(cancellationToken);
                        _currentToken = new OAuthToken
                        {
                            AccessToken = data.access_token,
                            RefreshToken = data.refresh_token,
                            ExpiresAt = DateTime.UtcNow.AddSeconds(data.expires_in),
                            TokenType = data.token_type
                        };
                        return _currentToken;
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                             response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        // Still pending or denied, continue polling
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error polling Trakt token: {ex.Message}");
                }
            }

            return null;
        }

        public OAuthToken? GetCurrentToken() => _currentToken;

        public void SetOAuthToken(OAuthToken token) => _currentToken = token;

        private string NormalizeListUrl(string listUrl)
        {
            // Convert various formats to the API format
            // Examples:
            // "trakt.tv/users/moses456/lists/marvel-cinematic-universe" -> "users/moses456/lists/marvel-cinematic-universe"
            // "https://trakt.tv/users/moses456/lists/marvel-cinematic-universe" -> "users/moses456/lists/marvel-cinematic-universe"
            
            listUrl = listUrl.Replace("https://", "").Replace("http://", "").Replace("trakt.tv/", "");
            if (listUrl.StartsWith("/"))
                listUrl = listUrl.Substring(1);
            
            return $"lists/{listUrl.Split('/').Last()}";
        }

        private ListItem? ParseTraktListItem(dynamic item, int position)
        {
            try
            {
                // Trakt list items have either a 'movie' or 'show' property
                var movieData = item.movie;
                var showData = item.show;

                if (movieData != null)
                {
                    return new ListItem
                    {
                        Id = movieData.ids.trakt.ToString(),
                        Title = movieData.title,
                        Year = movieData.year,
                        MediaType = Models.MediaType.Movie,
                        TraktId = movieData.ids.trakt,
                        ImdbId = movieData.ids.imdb,
                        TmdbId = movieData.ids.tmdb,
                        ListPosition = position
                    };
                }
                else if (showData != null)
                {
                    return new ListItem
                    {
                        Id = showData.ids.trakt.ToString(),
                        Title = showData.title,
                        Year = showData.year,
                        MediaType = Models.MediaType.Show,
                        TraktId = showData.ids.trakt,
                        ImdbId = showData.ids.imdb,
                        TmdbId = showData.ids.tmdb,
                        TvdbId = showData.ids.tvdb,
                        ListPosition = position
                    };
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error parsing Trakt list item: {ex.Message}");
            }

            return null;
        }

        private class TraktDeviceCodeResponse
        {
            public string device_code { get; set; } = string.Empty;
            public string user_code { get; set; } = string.Empty;
            public string verification_url { get; set; } = string.Empty;
            public int expires_in { get; set; }
            public int interval { get; set; }
        }

        private class TraktTokenResponse
        {
            public string access_token { get; set; } = string.Empty;
            public string token_type { get; set; } = "Bearer";
            public int expires_in { get; set; }
            public string refresh_token { get; set; } = string.Empty;
            public string scope { get; set; } = string.Empty;
        }
    }
}
