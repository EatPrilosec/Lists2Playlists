using System;
using System.Net.Http;
using System.Threading.Tasks;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Controller.Plugins;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Playlists;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Model.Serialization;
using Lists2Playlists.Configuration;
using Lists2Playlists.Providers;
using Lists2Playlists.Providers.Trakt;
using Lists2Playlists.Services;
using Lists2Playlists.Sync;
using Lists2Playlists.ScheduledTasks;

namespace Lists2Playlists
{
    /// <summary>
    /// Server entry point for the plugin
    /// </summary>
    public class ServerEntryPoint : IServerEntryPoint
    {
        private readonly ILibraryManager _libraryManager;
        private readonly IPlaylistManager _playlistManager;
        private readonly IApplicationPaths _applicationPaths;
        private readonly IJsonSerializer _jsonSerializer;
        private IConfigurationService? _configurationService;
        private ILibraryMatcher? _libraryMatcher;
        private IPlaylistService? _playlistService;
        private ISyncOrchestrator? _syncOrchestrator;
        private SyncListsTask? _syncTask;
        private bool _isInitialized;

        public ServerEntryPoint(
            ILibraryManager libraryManager,
            IPlaylistManager playlistManager,
            IApplicationPaths applicationPaths,
            IJsonSerializer jsonSerializer)
        {
            _libraryManager = libraryManager ?? throw new ArgumentNullException(nameof(libraryManager));
            _playlistManager = playlistManager ?? throw new ArgumentNullException(nameof(playlistManager));
            _applicationPaths = applicationPaths ?? throw new ArgumentNullException(nameof(applicationPaths));
            _jsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
            
            InitializeServices();
        }

        public string Name => "Lists2Playlists Entry Point";

        public event EventHandler? Disposed;

        public void RunBeforeStartup()
        {
            // Intentionally left empty - initialization happens in constructor
        }

        public void Run()
        {
            // Intentionally left empty - initialization happens in constructor
        }

        public void Dispose()
        {
            Disposed?.Invoke(this, EventArgs.Empty);
        }

        private void InitializeServices()
        {
            if (_isInitialized)
                return;

            try
            {
                // Initialize configuration service
                _configurationService = new ConfigurationService(_applicationPaths, _jsonSerializer);

                // Initialize library matcher
                _libraryMatcher = new LibraryMatcher(_libraryManager);

                // Initialize playlist service
                _playlistService = new PlaylistService(_playlistManager);

                // Initialize providers
                var providers = new System.Collections.Generic.Dictionary<ListSourceType, IListProvider>
                {
                    { ListSourceType.Trakt, new TraktListProvider(new HttpClient()) }
                    // Additional providers can be added here
                };

                // Initialize sync orchestrator
                _syncOrchestrator = new SyncOrchestrator(
                    _configurationService,
                    _libraryMatcher,
                    _playlistService,
                    providers);

                // Initialize and register scheduled task
                _syncTask = new SyncListsTask(_syncOrchestrator, _configurationService);
                
                // TODO: Register the task with Emby's task manager
                // var taskManager = Plugin.Instance?.TaskManager;
                // if (taskManager != null)
                // {
                //     // Task registration logic here
                // }

                System.Diagnostics.Debug.WriteLine("Lists2Playlists services initialized successfully");
                _isInitialized = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing Lists2Playlists: {ex.Message}");
            }
        }
    }
}